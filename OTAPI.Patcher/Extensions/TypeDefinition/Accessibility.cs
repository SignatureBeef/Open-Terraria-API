using Mono.Cecil;
using System.Linq;

namespace OTAPI.Patcher.Extensions
{
    public static partial class TypeDefinitionExtensions
    {
        /// <summary>
        /// Ensures all members of the type are publicly accessible
        /// </summary>
        /// <param name="type">The type to be made accessible</param>
        /// <param name="nested">To make all nested classes public as well.</param>
        public static void MakePublic(this TypeDefinition type, bool nested = true,
            bool isPublic = true,
            bool isFamily = false,
            bool isFamilyAndAssembly = false,
            bool isFamilyOrAssembly = false,
            bool isPrivate = false)
        {
            if (!nested) type.IsPublic = isPublic;

            foreach (var itm in type.Methods)
            {
                itm.IsPublic = isPublic;
                if (itm.IsFamily != isFamily) itm.IsFamily = isFamily;
                if (itm.IsFamilyAndAssembly != isFamilyAndAssembly) itm.IsFamilyAndAssembly = isFamilyAndAssembly;
                if (itm.IsFamilyOrAssembly != isFamilyOrAssembly) itm.IsFamilyOrAssembly = isFamilyOrAssembly;
                if (itm.IsPrivate != isPrivate) itm.IsPrivate = isPrivate;
            }
            foreach (var itm in type.Fields)
            {
                if (itm.IsFamily != isFamily) itm.IsFamily = isFamily;
                if (itm.IsFamilyAndAssembly != isFamilyAndAssembly) itm.IsFamilyAndAssembly = isFamilyAndAssembly;
                if (itm.IsFamilyOrAssembly != isFamilyOrAssembly) itm.IsFamilyOrAssembly = isFamilyOrAssembly;
                if (itm.IsPrivate != isPrivate)
                {
                    if (type.Events.Where(x => x.Name == itm.Name).Count() == 0)
                        itm.IsPrivate = isPrivate;
                    else
                    {
                        continue;
                    }
                }

                itm.IsPublic = isPublic;
            }
            foreach (var itm in type.Properties)
            {
                if (null != itm.GetMethod)
                {
                    if (itm.GetMethod.IsPublic != isPublic) itm.GetMethod.IsPublic = isPublic;
                    if (itm.GetMethod.IsFamily != isFamily) itm.GetMethod.IsFamily = isFamily;
                    if (itm.GetMethod.IsFamilyAndAssembly != isFamilyAndAssembly) itm.GetMethod.IsFamilyAndAssembly = isFamilyAndAssembly;
                    if (itm.GetMethod.IsFamilyOrAssembly != isFamilyOrAssembly) itm.GetMethod.IsFamilyOrAssembly = isFamilyOrAssembly;
                    if (itm.GetMethod.IsPrivate != isPrivate) itm.GetMethod.IsPrivate = isPrivate;
                }
                if (null != itm.SetMethod)
                {
                    if (itm.SetMethod.IsPublic != isPublic) itm.SetMethod.IsPublic = isPublic;
                    if (itm.SetMethod.IsFamily != isFamily) itm.SetMethod.IsFamily = isFamily;
                    if (itm.SetMethod.IsFamilyAndAssembly != isFamilyAndAssembly) itm.SetMethod.IsFamilyAndAssembly = isFamilyAndAssembly;
                    if (itm.SetMethod.IsFamilyOrAssembly != isFamilyOrAssembly) itm.SetMethod.IsFamilyOrAssembly = isFamilyOrAssembly;
                    if (itm.SetMethod.IsPrivate != isPrivate) itm.SetMethod.IsPrivate = isPrivate;
                }
            }

            foreach (var nt in type.NestedTypes)
                nt.MakePublic(nested, isPublic, isFamily, isFamilyAndAssembly, isFamilyOrAssembly, isPrivate);
        }
    }
}
