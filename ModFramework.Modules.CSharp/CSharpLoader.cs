/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.DependencyModel;
using ModFramework.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace ModFramework.Modules.CSharp
{
    [MonoMod.MonoModIgnore]
    public delegate bool AssemblyFoundHandler(string filepath);

    [MonoMod.MonoModIgnore]
    public class CSharpLoader
    {
        const string ConsolePrefix = "CSharp";
        const string ModulePrefix = "CSharpScript_";

        public ModFwModder Modder { get; set; }

        public static event AssemblyFoundHandler AssemblyFound;
        public static List<string> GlobalAssemblies { get; } = new List<string>();

        public bool AutoLoadAssemblies { get; set; } = true;
        public MarkdownDocumentor MarkdownDocumentor { get; set; }

        public CSharpLoader SetAutoLoadAssemblies(bool autoLoad)
        {
            AutoLoadAssemblies = autoLoad;
            return this;
        }

        public CSharpLoader SetModder(ModFwModder modder)
        {
            Modder = modder;

            modder.OnReadMod += (m, module) =>
            {
                if (module.Assembly.Name.Name.StartsWith(ModulePrefix))
                {
                    // remove the top level program class
                    var tlc = module.GetType("<Program>$");
                    if (tlc != null)
                    {
                        module.Types.Remove(tlc);
                    }
                    Modder.RelinkAssembly(module);
                }
            };

            return this;
        }

        IEnumerable<MetadataReference> LoadExternalRefs(string path)
        {
            var refs_path = Path.Combine(path, "Metadata.refs");

            if (File.Exists(refs_path))
            {
                var refs = File.ReadLines(refs_path);

                var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

                foreach (var ref_file in refs)
                {
                    var sys_path = Path.Combine(assemblyPath, ref_file);

                    if (File.Exists(ref_file))
                        yield return MetadataReference.CreateFromFile(ref_file);

                    else if (File.Exists(sys_path))
                        yield return MetadataReference.CreateFromFile(sys_path);

                    else throw new Exception($"Unable to resolve external reference: {ref_file} (Metadata.refs) in dir {Environment.CurrentDirectory}");
                }
            }
        }

        class CreateContextOptions
        {
            public MetaData Meta { get; set; }
            public string AssemblyName { get; set; }
            public string OutDir { get; set; }
            public OutputKind OutputKind { get; set; }
            public IEnumerable<CompilationFile> CompilationFiles { get; set; }
            public IEnumerable<string> Constants { get; set; }
            public IEnumerable<string> OutAsmPath { get; set; }
            public IEnumerable<string> OutPdbPath { get; set; }
        }

        CompilationContext CreateContext(CreateContextOptions options)
        {
            var root = Path.Combine("csharp", "modifications");

            var assemblyPath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            var dllStream = new MemoryStream();
            var pdbStream = new MemoryStream();
            var xmlStream = new MemoryStream();

            var assemblyName = ModulePrefix + options.AssemblyName;

            var outAsmPath = Path.Combine(options.OutDir, $"{assemblyName}.dll");
            var outPdbPath = Path.Combine(options.OutDir, $"{assemblyName}.pdb");
            var outXmlPath = Path.Combine(options.OutDir, $"{assemblyName}.xml");

            var refs = new List<MetadataReference>();

            var referenceAssemblies = DependencyContext.Default.CompileLibraries
                .Where(cl => cl.Type == "referenceassembly")
                .SelectMany(x => x.ResolveReferencePaths())
                .Select(asm => MetadataReference.CreateFromFile(asm))
                .ToArray();

            foreach (var mref in options.Meta.MetadataReferences
                    .Concat(GlobalAssemblies.Select(globalPath => MetadataReference.CreateFromFile(globalPath)))
            //.Concat(referenceAssemblies)
            )
            {
                if (!refs.Any(x => x.Display == mref.Display))
                {
                    refs.Add(mref);
                }
            }

            //refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            var compile_options = new CSharpCompilationOptions(options.OutputKind)
                    .WithOptimizationLevel(OptimizationLevel.Debug)
                    .WithPlatform(Platform.AnyCpu)
                    .WithAllowUnsafe(true);

            var syntaxTrees = options.CompilationFiles.Select(x => x.SyntaxTree);

            var compilation = CSharpCompilation
                .Create(assemblyName, syntaxTrees, options: compile_options)
                .AddReferences(refs)
            ;

            var libs = ((String)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
                .Split(Path.PathSeparator)
                .Where(x => !x.StartsWith(Environment.CurrentDirectory));
            foreach (var lib in libs)
            {
                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(lib));
            }

            var emitOptions = new EmitOptions(
                debugInformationFormat: DebugInformationFormat.PortablePdb,
                pdbFilePath: outPdbPath
            );

            return new CompilationContext()
            {
                Compilation = compilation,
                EmitOptions = emitOptions,
                CompilationOptions = compile_options,
                DllStream = dllStream,
                PdbStream = pdbStream,
                XmlStream = xmlStream,
                DllPath = outAsmPath,
                PdbPath = outPdbPath,
                XmlPath = outXmlPath,
                CompilationFiles = options.CompilationFiles,
            };
        }

        class CompilationFile
        {
            public string File { get; set; }
            public SyntaxTree SyntaxTree { get; set; }
            public EmbeddedText EmbeddedText { get; set; }
        }

        void ProcessXmlSyntax(string filePath, string type, SyntaxTree encoded)
        {
            var root = encoded?.GetRoot();
            var syntax = root as CompilationUnitSyntax;
            if (syntax is not null)
            {
                foreach (var member in syntax.Members)
                {
                    ProcessXmlMember(filePath, type, member);
                }
            }
        }

        public CSharpLoader SetMarkdownDocumentor(MarkdownDocumentor documentor)
        {
            MarkdownDocumentor = documentor;
            return this;
        }

        MarkdownDocumentor GetMarkdownDocumentor()
        {
            if (MarkdownDocumentor is not null)
                return MarkdownDocumentor;

            return Modder?.MarkdownDocumentor;
        }

        void ProcessXmlComment(string filePath, string type, SyntaxTrivia trivia)
        {
            var xml_node = trivia.GetStructure();

            if (xml_node is DocumentationCommentTriviaSyntax dcts)
            {
                var xml = dcts.Content;

                foreach (var node in dcts.Content)
                {
                    if (node is XmlElementSyntax xes)
                    {
                        foreach (var item in xes.Content)
                        {
                            if (item is XmlTextSyntax xts)
                            {
                                var comments = String.Join(string.Empty, xts.TextTokens.Select(x => x.Text));
                                var cleaned = String.Join(" ", comments.Trim().Split(Environment.NewLine));
                                var doc = GetMarkdownDocumentor();

                                if (!doc.Find<BasicComment>(r => r.FilePath == filePath && r.Comments == cleaned).Any())
                                    GetMarkdownDocumentor().Add(new BasicComment()
                                    {
                                        Comments = String.Join(" ", comments.Trim().Split(Environment.NewLine)),
                                        Type = type,
                                        FilePath = filePath,
                                    });
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
            else
            {

            }
        }

        void ProcessXmlMember(string filePath, string type, MemberDeclarationSyntax member)
        {
            var trivias = member.GetLeadingTrivia();

            foreach (var trv in trivias)
            {
                //var content = trv.ToFullString();
                //if (trv.IsKind(SyntaxKind.MultiLineCommentTrivia))
                //{
                //    var is_gpl = content.Contains("GNU General Public License");
                //    if(!is_gpl)
                //    {
                //        ProcessXmlComment(category, content);
                //    }
                //}
                if (trv.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                {
                    ProcessXmlComment(filePath, type, trv);
                }
                else if (trv.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    ProcessXmlComment(filePath, type, trv);
                }
                else
                {

                }
            }

            if (member is NamespaceDeclarationSyntax nds)
            {
                foreach (var child in nds.Members)
                {
                    ProcessXmlMember(filePath, type, child);
                }
            }
            else
            {

            }
        }

        IEnumerable<CompilationFile> PrepareFiles(IEnumerable<string> files, IEnumerable<string> constants, string type)
        {
            foreach (var file in files)
            {
                var folder = Path.GetFileName(Path.GetDirectoryName(file));

                var encoding = System.Text.Encoding.UTF8;
                var parse_options = CSharpParseOptions.Default
                    .WithKind(SourceCodeKind.Regular)
                    .WithPreprocessorSymbols(constants.Select(s => s.Replace("#define ", "")))
                    .WithDocumentationMode(DocumentationMode.Parse)
                    .WithLanguageVersion(LanguageVersion.Preview); // allows toplevel functions

                var src = File.ReadAllText(file);
                var source = SourceText.From(src, encoding);
                var encoded = CSharpSyntaxTree.ParseText(source, parse_options, file);
                var embedded = EmbeddedText.FromSource(file, source);

                if (GetMarkdownDocumentor() is not null)
                {
                    ProcessXmlSyntax(file, type, encoded);
                }

                yield return new CompilationFile()
                {
                    File = file,
                    SyntaxTree = encoded,
                    EmbeddedText = embedded,
                };
            }
        }

        class CompilationContext : IDisposable
        {
            public CSharpCompilation Compilation { get; set; }
            public EmitOptions EmitOptions { get; set; }
            public CSharpCompilationOptions CompilationOptions { get; set; }
            public MemoryStream DllStream { get; set; }
            public MemoryStream PdbStream { get; set; }
            public MemoryStream XmlStream { get; set; }
            public string DllPath { get; set; }
            public string XmlPath { get; set; }
            public string PdbPath { get; set; }
            public IEnumerable<CompilationFile> CompilationFiles { get; set; }

            public EmitResult Compile()
            {
                return Compilation.Emit(
                      peStream: DllStream,
                      pdbStream: PdbStream,
                      xmlDocumentationStream: XmlStream,
                      embeddedTexts: CompilationFiles.Select(x => x.EmbeddedText),
                      options: EmitOptions
                );
            }

            public void Dispose()
            {
                Compilation = null;
                EmitOptions = null;
                //EmbeddedTexts = null;
                XmlStream?.Dispose();
                PdbStream?.Dispose();
                DllStream?.Dispose();
                DllPath = null;
                XmlPath = null;
                PdbPath = null;
            }
        }

        void ProcessCompilation(string errorName, CompilationContext ctx, EmitResult compilationResult)
        {
            if (compilationResult.Success)
            {
                ctx.DllStream.Seek(0, SeekOrigin.Begin);
                ctx.PdbStream.Seek(0, SeekOrigin.Begin);
                ctx.XmlStream.Seek(0, SeekOrigin.Begin);

                File.WriteAllBytes(ctx.DllPath, ctx.DllStream.ToArray());
                File.WriteAllBytes(ctx.PdbPath, ctx.PdbStream.ToArray());
                File.WriteAllBytes(ctx.XmlPath, ctx.XmlStream.ToArray());

                if (AutoLoadAssemblies)
                {
                    var asm = PluginLoader.AssemblyLoader.Load(ctx.DllStream, ctx.PdbStream);
                    PluginLoader.AddAssembly(asm);

                    if (Modder != null)
                        Modder.ReadMod(ctx.DllPath);
                    else Modifier.Apply(ModType.Runtime, null, new[] { asm }); // relay on the runtime hook
                }
            }
            else
            {
                //Console.WriteLine($"Compilation errors for file: {Path.GetFileName(file)}");

                foreach (var diagnostic in compilationResult.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error))
                {
                    //Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    Console.Error.WriteLine(diagnostic.ToString());
                }

                throw new Exception($"Compilation errors above for file: {errorName}");
            }
        }

        string LoadScripts(MetaData meta, IEnumerable<string> files, OutputKind outputKind, string assemblyName, string type)
        {
            try
            {
                var compilationFiles = PrepareFiles(files, meta.Constants, type);

                using var ctx = CreateContext(new CreateContextOptions()
                {
                    Meta = meta,
                    AssemblyName = assemblyName,
                    OutDir = meta.OutputDirectory,
                    OutputKind = outputKind,
                    CompilationFiles = compilationFiles,
                    Constants = meta.Constants,
                });

                var compilationResult = ctx.Compile();

                ProcessCompilation(assemblyName, ctx, compilationResult);

                if (compilationResult.Success)
                    return ctx.DllPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{ConsolePrefix}] Load error: {ex}");
                throw;
            }
            return null;
        }

        void LoadSingleScripts(MetaData meta, string folder, OutputKind outputKind, string type)
        {
            var files = Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (AssemblyFound?.Invoke(file) == false)
                    continue; // event was cancelled, they do not wish to use this file. skip to the next.

                Console.WriteLine($"[{ConsolePrefix}] Loading script: {file}");

                var assemblyName = Path.GetFileNameWithoutExtension(file);

                LoadScripts(meta, new[] { file }, outputKind, assemblyName, type);
            }
        }

        void LoadTopLevelScripts(MetaData meta)
        {
            var toplevel = Path.Combine(RootDirectory, "toplevel");
            if (Directory.Exists(toplevel))
                LoadSingleScripts(meta, toplevel, OutputKind.ConsoleApplication, "toplevel");
        }

        void LoadPatches(MetaData meta)
        {
            var patches = Path.Combine(RootDirectory, "patches");
            if (Directory.Exists(patches))
                LoadSingleScripts(meta, patches, OutputKind.DynamicallyLinkedLibrary, "patch");
        }

        public List<string> LoadModules(MetaData meta, string folder)
        {
            var paths = new List<string>();
            var modules = Path.Combine(RootDirectory, folder);
            if (Directory.Exists(modules))
            {
                var moduleNames = Directory.EnumerateDirectories(modules, "*", SearchOption.TopDirectoryOnly);

                foreach (var dir in moduleNames)
                {
                    var files = Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories)
                        .Where(file =>
                        {
                            return AssemblyFound?.Invoke(file) != false;
                        });
                    if (files.Any())
                    {
                        var moduleName = Path.GetFileName(dir);
                        Console.WriteLine($"[{ConsolePrefix}] Loading module: {moduleName}");
                        var path = LoadScripts(meta, files, OutputKind.DynamicallyLinkedLibrary, moduleName, "module");
                        if (File.Exists(path))
                            paths.Add(path);
                    }
                    else
                    {
                        // skipped - not needed for this target.
                    }
                }
            }
            return paths;
        }

        public class MetaData
        {
            public IEnumerable<string> Constants { get; set; }
            public IEnumerable<MetadataReference> MetadataReferences { get; set; }
            public string OutputDirectory { get; set; }
        }

        public string RootDirectory => Path.Combine("csharp", "plugins");

        public MetaData CreateMetaData()
        {
            const string constants_path = "AutoGenerated.cs";
            var constants = File.Exists(constants_path)
                ? File.ReadAllLines(constants_path) : Enumerable.Empty<string>(); // bring across the generated constants

            var refs = LoadExternalRefs(RootDirectory).ToList();

            var outDir = Path.Combine("csharp", "generated");
            if (Directory.Exists(outDir)) Directory.Delete(outDir, true);
            Directory.CreateDirectory(outDir);

            return new MetaData()
            {
                MetadataReferences = refs,
                Constants = constants,
                OutputDirectory = outDir,
            };
        }

        /// <summary>
        /// Discovers .cs modifications or top-level scripts, compiles and registers them with MonoMod or ModFramework accordingly
        /// </summary>
        public MetaData LoadModifications()
        {
            if (Directory.Exists(RootDirectory))
            {
                var meta = CreateMetaData();

                LoadTopLevelScripts(meta);
                LoadPatches(meta);
                LoadModules(meta, "modules");
                return meta;
            }
            return null;
        }
    }
}
