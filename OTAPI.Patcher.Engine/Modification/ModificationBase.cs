using Mono.Cecil;
using OTAPI.Patcher.Engine.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

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
		public TypeDefinition Type<T>()
		{
			var type = typeof(T);
			var definition = this.ModificationDefinition.MainModule.Types.SingleOrDefault(
				t => t.FullName == type.FullName
			);

			if (definition == null)
			{
				definition = this.SourceDefinition.MainModule.Types.SingleOrDefault(
					t => t.FullName == type.FullName
				);
			}
			if (definition == null)
				throw new TypeAccessException($"{type.AssemblyQualifiedName} cannot be found.");

			return definition;
		}

		/// <summary>
		/// Returns the MethodDefinition for the specified action
		/// </summary>
		public MethodDefinition Method(Expression<Action> expression)
		{
			var method = (expression.Body as MethodCallExpression).Method;
			var type = this.ResolveType(method.DeclaringType);

			return type.Method(method.Name, method.GetParameters());
		}

		public FieldDefinition Field<TProperty>(Expression<Func<TProperty>> expression)
		{
			var member = (expression.Body as MemberExpression).Member;
			var type = this.ResolveType(member.DeclaringType);

			return type.Field(member.Name);
		}

		private TypeDefinition ResolveType(Type type)
		{
			TypeDefinition definition = this.ModificationDefinition.MainModule.Types.SingleOrDefault(
				t => t.FullName == type.FullName
			);

			if (definition == null)
			{
				definition = this.SourceDefinition.MainModule.Types.SingleOrDefault(
					t => t.FullName == type.FullName
				);
			}
			if (definition == null)
				throw new TypeAccessException($"{type.AssemblyQualifiedName} cannot be found.");

			return definition;
		}
	}
}
