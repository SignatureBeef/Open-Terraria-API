using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTAPI.Modification.Tile.Extensions
{
	public static class CecilExtensions
	{
		/// <summary>
		/// Swaps a Tile array type to an array of the replacement type
		/// </summary>
		/// <returns>The to vanilla reference.</returns>
		/// <param name="input">Input.</param>
		/// <param name="replacement">Replacement.</param>
		private static TypeReference SwapToVanillaReference(TypeReference input, TypeReference replacement, string lookFor)
		{
			if (input.FullName == lookFor)
			{
				return replacement;
			}
			else if (input is ArrayType)
			{
				var at = input as ArrayType;
				if (at.ElementType.FullName == lookFor)
				{
					var nt = new ArrayType(replacement);
					nt.Dimensions.Clear();

					foreach (var dm in at.Dimensions)
					{
						nt.Dimensions.Add(dm);
					}

					return input.Module.Import(nt);
				}
			}

			return input;
		}

		public static void TryReplaceArrayWithClassInstance(this ModuleDefinition module, TypeDefinition lookFor, TypeDefinition replacementType, TypeDefinition replacementDespatcher, string callbackSuffix)
		{
			foreach (var ty in module.Types)
			{
				ty.TryReplaceArrayWithClassInstance(lookFor, replacementType, replacementDespatcher, callbackSuffix);
			}
		}

		public static void TryReplaceArrayWithClassInstance(this TypeDefinition typeToSearch, TypeDefinition lookFor, TypeDefinition replacementType, TypeDefinition replacementDespatcher, string callbackSuffix)
		{
			var vt = typeToSearch.Module.Import(replacementType);

			var setCall = typeToSearch.Module.Import(replacementDespatcher.Methods.SingleOrDefault(x => x.Name == "Set" + callbackSuffix));
			var getCall = typeToSearch.Module.Import(replacementDespatcher.Methods.SingleOrDefault(x => x.Name == "Get" + callbackSuffix));

			if (typeToSearch != lookFor)
			{
				if (typeToSearch.HasFields)
					foreach (var fld in typeToSearch.Fields)
					{
						fld.FieldType = SwapToVanillaReference(fld.FieldType, vt, lookFor.FullName);
					}

				if (typeToSearch.HasProperties)
					foreach (var prop in typeToSearch.Properties)
					{
						prop.PropertyType = SwapToVanillaReference(prop.PropertyType, vt, lookFor.FullName);
					}

				foreach (var mth in typeToSearch.Methods)
				{
					if (mth.HasParameters)
					{
						foreach (var prm in mth.Parameters)
						{
							prm.ParameterType = SwapToVanillaReference(prm.ParameterType, vt, lookFor.FullName);
						}
					}

					if (mth.HasBody)
					{
						if (mth.Body.HasVariables)
						{
							foreach (var vrb in mth.Body.Variables)
							{
								vrb.VariableType = SwapToVanillaReference(vrb.VariableType, vt, lookFor.FullName);
							}
						}

						if (mth.Body.Instructions != null)
						{
							for (var i = mth.Body.Instructions.Count - 1; i > 0; i--)
							{
								var ins = mth.Body.Instructions[i];

								{
									if (ins.OpCode == OpCodes.Call && ins.Operand is MemberReference)
									{
										var mr = ins.Operand as MemberReference;

										if (setCall != null && mr.Name == "Set" && mr.DeclaringType is ArrayType && (mr.DeclaringType as ArrayType).ElementType.FullName == replacementType.FullName)
										{
											ins.Operand = typeToSearch.Module.Import(setCall);
										}

										if (getCall != null && mr.Name == "Get" && mr.DeclaringType is ArrayType && (mr.DeclaringType as ArrayType).ElementType.FullName == replacementType.FullName)
										{
											ins.Operand = typeToSearch.Module.Import(getCall);
										}
									}
								}


								if (ins.Operand is MethodReference)
								{
									var meth = ins.Operand as MethodReference;
									if (meth.DeclaringType.FullName == lookFor.FullName)
									{
										if (meth.Name == ".ctor")
										{
											ins.Operand = typeToSearch.Module.Import(vt.Resolve().Methods.Single(x => x.Name == meth.Name && x.Parameters.Count == meth.Parameters.Count));
											continue;
										}

										ins.Operand = typeToSearch.Module.Import(vt.Resolve().Methods.Single(x => x.Name == meth.Name && x.Parameters.Count == meth.Parameters.Count));
										continue;
									}
									else if (meth.DeclaringType is ArrayType)
									{
										var at = meth.DeclaringType as ArrayType;
										if (at.ElementType.FullName == lookFor.FullName)
										{
											meth.DeclaringType = SwapToVanillaReference(meth.DeclaringType, vt, lookFor.FullName);
										}
									}

									if (meth.HasParameters)
										foreach (var prm in meth.Parameters)
										{
											prm.ParameterType = SwapToVanillaReference(prm.ParameterType, vt, lookFor.FullName);
										}

									meth.ReturnType = SwapToVanillaReference(meth.ReturnType, vt, lookFor.FullName);
									meth.MethodReturnType.ReturnType = SwapToVanillaReference(meth.MethodReturnType.ReturnType, vt, lookFor.FullName);
								}
								else if (ins.Operand is TypeReference)
								{
									var typ = ins.Operand as TypeReference;
									if (typ.FullName == lookFor.FullName)
									{
										throw new NotImplementedException();
									}
									else if (typ is ArrayType)
									{
										var at = typ as ArrayType;
										if (at.ElementType.FullName == lookFor.FullName)
										{
											throw new NotImplementedException();
										}
									}
								}
								else if (ins.Operand is FieldReference)
								{
									var fld = ins.Operand as FieldReference;
									if (fld.DeclaringType.FullName == lookFor.FullName)
									{
										//Now, instead map to our property methods

										var il = mth.Body.GetILProcessor();
										if (ins.OpCode == OpCodes.Ldfld)
										{
											//Get
											var prop = typeToSearch.Module.Import(vt.Resolve().Properties.Single(x => x.Name == fld.Name).GetMethod);

											il.Replace(ins, il.Create(OpCodes.Callvirt, prop));
										}
										else if (ins.OpCode == OpCodes.Stfld)
										{
											//Set
											var prop = typeToSearch.Module.Import(vt.Resolve().Properties.Single(x => x.Name == fld.Name).SetMethod);

											il.Replace(ins, il.Create(OpCodes.Callvirt, prop));
										}
										else
										{
											throw new NotImplementedException();
										}
									}
									else if (fld.DeclaringType is ArrayType)
									{
										var at = fld.DeclaringType as ArrayType;
										if (at.ElementType.FullName == lookFor.FullName)
										{
											throw new NotImplementedException();
										}
									}
								}
								else if (ins.Operand is PropertyReference)
								{
									throw new NotImplementedException();
								}
								else if (ins.Operand is VariableReference)
								{
									var vrb = ins.Operand as VariableReference;
									vrb.VariableType = SwapToVanillaReference(vrb.VariableType, vt, lookFor.FullName);
								}
								else if (ins.Operand is MemberReference)
								{
									throw new NotImplementedException();
								}
							}
						}
					}
				}
			}

			if (typeToSearch.HasNestedTypes)
				foreach (var nt in typeToSearch.NestedTypes)
					nt.TryReplaceArrayWithClassInstance(lookFor, replacementType, replacementDespatcher, callbackSuffix);
		}
	}
}
