using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Patcher.Engine.Framework
{
	public class EmbeddedResourceExtractor
	{
		public AssemblyDefinition Assembly { get; }
		public string OutputPath { get; private set; }

		public IEnumerable<string> Extensions { get; set; }

		public EmbeddedResourceExtractor(string input)
		{
			this.OutputPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(input));
			this.Assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(input);
		}

		public List<string> Extract()
		{
			var extracted = new List<string>();
			foreach (var module in this.Assembly.Modules)
			{
				if (module.HasResources)
				{
					foreach (var resource in this.Assembly.MainModule.Resources)
					{
						var er = resource as EmbeddedResource;
						if (er != null)
						{
							var data = er.GetResourceData();
							var saveTo = System.IO.Path.Combine(this.OutputPath, er.Name);
							var extension = System.IO.Path.GetExtension(er.Name);
							
							if (Extensions != null && !Extensions.Any(ext => ext.Equals(extension, System.StringComparison.CurrentCultureIgnoreCase)))
							{
								continue;
							}

							System.IO.File.WriteAllBytes(saveTo, data);
							extracted.Add(saveTo);
						}
					}
				}
			}

			return extracted;
		}
	}
}
