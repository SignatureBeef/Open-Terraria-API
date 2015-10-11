using System;
using Mono.Cecil;
using System.Linq;

namespace OTA.Patcher
{
    public class TerrariaOrganiser
    {
        private AssemblyDefinition _asm;

        public TerrariaOrganiser(AssemblyDefinition assembly)
        {
            this._asm = assembly;
        }

        public TypeSystem TypeSystem
        {
            get
            { return _asm.MainModule.TypeSystem; }
        }

        public TypeDefinition Entity
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Entity"); }
        }

        public TypeDefinition Item
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Item"); }
        }

        public TypeDefinition MessageBuffer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "MessageBuffer"); }
        }

        public TypeDefinition NetMessage
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NetMessage"); }
        }

        public TypeDefinition Main
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Main"); }
        }

        //        public TypeDefinition ProgramServer
        //        {
        //            get
        //            { return _asm.MainModule.Types.Single(x => x.Name == "ProgramServer"); }
        //        }

        /// <summary>
        /// Entry class for windows
        /// </summary>
        /// <value>The windows launch.</value>
        public TypeDefinition WindowsLaunch
        {
            get

            { return _asm.MainModule.Types.Single(x => x.Name == "WindowsLaunch"); }
        }

        /// <summary>
        /// Entry class for Mac
        /// </summary>
        /// <value>The program.</value>
        public TypeDefinition Program
        {
            get

            { return _asm.MainModule.Types.Single(x => x.Name == "Program"); }
        }

        public TypeDefinition NPC
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NPC"); }
        }

        public TypeDefinition WorldFile
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "WorldFile"); }
        }

        public TypeDefinition WorldGen
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "WorldGen"); }
        }

        public TypeDefinition Netplay
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Netplay"); }
        }

        public TypeDefinition Player
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Player"); }
        }

        //        public TypeDefinition ServerSock
        //        {
        //            get
        //            { return _asm.MainModule.Types.Single(x => x.Name == "ServerSock"); }
        //        }

        public TypeDefinition RemoteClient
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "RemoteClient"); }
        }

        public TypeDefinition RemoteServer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "RemoteServer"); }
        }

        public TypeDefinition LaunchInitializer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "LaunchInitializer"); }
        }

        public TypeDefinition Lang
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Lang"); }
        }

        public TypeDefinition Projectile
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Projectile"); }
        }

        public TypeReference Import(TypeReference typeReference)
        {
            return _asm.MainModule.Import(typeReference);
        }

        public MethodReference Import(MethodReference methodReference)
        {
            return _asm.MainModule.Import(methodReference);
        }
    }
}

