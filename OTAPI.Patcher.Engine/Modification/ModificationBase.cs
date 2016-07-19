using Mono.Cecil;
using OTAPI.Patcher.Engine.Extensions;
using System;

namespace OTAPI.Patcher.Engine.Modification
{
    public abstract class ModificationBase
    {
        public AssemblyDefinition SourceDefinition { get; internal set; }
        public AssemblyDefinition ModificationDefinition { get; private set; }

        public const Int32 DefaultOrder = 5;

        protected ModificationBase()
        {
            var moduleLocation = this.GetType().Assembly.Location;
            ModificationDefinition = AssemblyDefinition.ReadAssembly(moduleLocation);
        }

		/// <summary>
		/// Location of the source definition on disk
		/// </summary>
		internal string SourceDefinitionFilePath { get; set; }

        /// <summary>
        /// Returns the list of applicable assembly targets the modification can be ran against
        /// </summary>
        public abstract System.Collections.Generic.IEnumerable<string> AssemblyTargets { get; }

        /// <summary>
        /// Occurs when the modification is triggered to run
        /// </summary>
        /// <param name="options"></param>
        public abstract void Run();

        /// <summary>
        /// Description of the modification running
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Determines the sort order for the current modification.
        /// </summary>
        /// <returns></returns>
        internal int GetOrder()
        {
            var attr = (OrderedAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(OrderedAttribute), true);
            if (attr != null)
                return attr.Order;

            return DefaultOrder;
        }

        /// <summary>
        /// Returns the TypeDefintion of the type specified
        /// </summary>
        public TypeDefinition Type<T>() => ModificationDefinition.Type<T>();

        /// <summary>
        /// Returns the MethodDefinition for the specified action
        /// </summary>
        public MethodDefinition Method(System.Linq.Expressions.Expression<Action> expression)
        {
            var method = (expression.Body as System.Linq.Expressions.MethodCallExpression).Method;
            var type = this.ModificationDefinition.Type(method.DeclaringType.FullName);

            return type.Method(method.Name);
        }
    }
}
