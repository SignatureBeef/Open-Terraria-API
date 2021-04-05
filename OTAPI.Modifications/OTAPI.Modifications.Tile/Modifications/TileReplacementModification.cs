using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Modification.Tile.Modifications
{
	public class TileReplacementModification : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.2.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Patching tiles";

		public override void Run()
		{
			//What we need is a overridable tile class, and since the stock tile does not allow this
			//we need to implement our own custom tile that allows people to change logic (ie data stores)
			//but also near-exactly matches the Terraria.Tile signatures...but that's what i previously done in v1.
			//Instead, what i do now is create properties directly in the Terraria.Tile class and change 
			//the existing fields to be the backing fields for said properties.
			//These properties deliberately share the exact same name as the vanilla counterpart to ensure 
			//that we can compile code for both implementations and not change any code (the compiler will for us!).
			//
			//Of course, using properties will for sure cause us issues as the IL that vanilla is currently using, is
			//expecting field references, and not a call to a properties get/set methods.
			//
			//This is where it gets fun, er i mean interesting. But with a bit of luck you might notice something 
			//pretty darn awesome in the below IL comparisons between a snippet of a field reference and a property
			//reference.

			#region snippet
			#region field
			//IL_0000: nop
			//IL_0001: newobj instance void OTAPI.Modification.Tile.Modifications.FieldReferenceTest::.ctor()
			//IL_0006: stloc.0
			//IL_0007: ldloc.0
			//IL_0008: ldc.i4 9939
			//IL_000d: stfld int32 OTAPI.Modification.Tile.Modifications.FieldReferenceTest::myData
			//IL_0012: ldloc.0
			//IL_0013: ldfld int32 OTAPI.Modification.Tile.Modifications.FieldReferenceTest::myData
			//IL_0018: stloc.1
			//IL_0019: ldloc.0
			//IL_001a: dup
			//IL_001b: ldfld int32 OTAPI.Modification.Tile.Modifications.FieldReferenceTest::myData
			//IL_0020: ldc.i4 9939
			//IL_0025: add
			//IL_0026: stfld int32 OTAPI.Modification.Tile.Modifications.FieldReferenceTest::myData
			//IL_002b: ret
			#endregion
			#region property
			//IL_0000: nop
			//IL_0001: newobj instance void OTAPI.Modification.Tile.Modifications.PropertyReferenceTest::.ctor()
			//IL_0006: stloc.0
			//IL_0007: ldloc.0
			//IL_0008: ldc.i4 9939
			//IL_000d: callvirt instance void OTAPI.Modification.Tile.Modifications.PropertyReferenceTest::set_myData(int32)
			//IL_0012: nop
			//IL_0013: ldloc.0
			//IL_0014: callvirt instance int32 OTAPI.Modification.Tile.Modifications.PropertyReferenceTest::get_myData()
			//IL_0019: stloc.1
			//IL_001a: ldloc.0
			//IL_001b: dup
			//IL_001c: callvirt instance int32 OTAPI.Modification.Tile.Modifications.PropertyReferenceTest::get_myData()
			//IL_0021: ldc.i4 9939
			//IL_0026: add
			//IL_0027: callvirt instance void OTAPI.Modification.Tile.Modifications.PropertyReferenceTest::set_myData(int32)
			//IL_002c: nop
			//IL_002d: ret
			#endregion
			#endregion
			//Can you spot what needs to happen?
			//Im sure you can - you'll notice that the IL is pretty much the same, with the only differences being
			//the opcode ldfld changes to a getter method call, and a stfld will change to a setters method call.
			//What's so neat about this is that we don't even need to alter instructions as the ldloc.0 already
			//has the fields type instance on the stack, so we can simply call the property method; and additionally,
			//the setter's parameters will also be satisfied as they are too, on the stack.

			//Get the type definition of Terraria.Tile
			var terrariaTile = this.Type<Terraria.Tile>();

			//var iTile = this.ModificationDefinition.Type("OTAPI.Tile.ITile");
			//terrariaTile.Interfaces.Add(this.SourceDefinition.MainModule.Import(iTile));

			//Enumerate over each instance field and start swapping
			foreach (var field in terrariaTile.Fields.Where(f => !f.IsStatic))
			{
				//Logic:
				//Rename the field to be: <field name>k__BackingField
				//Generate a: public virtual <field name> {get; set;}

				//Our property is taking the fields place
				var propertyName = field.Name;

				//the compiler will create a backing field, so we can cheat an rename the current field
				field.Name = $"<{field.Name}>k__BackingField";
				field.IsPublic = false;
				field.IsPrivate = true;

				//This is required or it will be shown when you decompile
				field.CustomAttributes.Add(new CustomAttribute(
					SourceDefinition.MainModule.Import(
						typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)
							.GetConstructors()
							.Single()
					)
				));

				//Create the property that will soon take the fields place
				var prop = GenerateProperty(propertyName, field.FieldType, declaringType: terrariaTile);

				//Mark the property as virtual. note that IsVirtual actually means override, 
				//and so marking IsNewSlot triggers the virtual keyword
				prop.GetMethod.IsVirtual = prop.GetMethod.IsNewSlot = true;
				prop.SetMethod.IsVirtual = prop.SetMethod.IsNewSlot = true;

				//Add the property to the assembly
				terrariaTile.Properties.Add(prop);

				//var iTileProperty = iTile.Property(prop.Name);

				//Now simply replace all occurrences of the field with the newly added property
				field.ReplaceWith(prop);
			}
		}

		/// <summary>
		/// Generates a property with getter and setter methods.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="propertyType"></param>
		/// <param name="declaringType"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		PropertyDefinition GenerateProperty(string name, TypeReference propertyType,
			TypeDefinition declaringType = null,
			PropertyAttributes attributes = PropertyAttributes.None,
			MethodAttributes getterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
			MethodAttributes setterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig)
		{
			//Create the initial property definition
			var prop = new PropertyDefinition(name, attributes, propertyType);

			//Set the defaults of the property
			prop.HasThis = true;
			if (declaringType != null) prop.DeclaringType = declaringType;

			//Generate the getter
			prop.GetMethod = GenerateGetter(prop, getterAttributes);
			declaringType.Methods.Add(prop.GetMethod);

			//Generate the setter
			prop.SetMethod = GenerateSetter(prop, setterAttributes);
			declaringType.Methods.Add(prop.SetMethod);

			return prop;
		}

		/// <summary>
		/// Generates a property getter method
		/// </summary>
		/// <param name="property"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		MethodDefinition GenerateGetter(PropertyDefinition property, MethodAttributes attributes, bool instance = true)
		{
			//Create the method definition
			var method = new MethodDefinition("get_" + property.Name, attributes, property.PropertyType);

			//Reference - this is what we need to essentially replicate
			//IL_0000: ldarg.0
			//IL_0001: ldfld int32 OTAPI.Modification.Tile.Modifications.PropertyReferenceTest::'<myData>k__BackingField'
			//IL_0006: ret

			//Create the il processor so we can alter il
			var il = method.Body.GetILProcessor();

			//Load the current type instance if required
			if (instance)
				il.Append(il.Create(OpCodes.Ldarg_0));
			//Load the backing field
			il.Append(il.Create(OpCodes.Ldfld, property.DeclaringType.Field($"<{property.Name}>k__BackingField")));
			//Return the backing fields value
			il.Append(il.Create(OpCodes.Ret));

			//Set basic getter method details 
			method.Body.InitLocals = true;
			method.SemanticsAttributes = MethodSemanticsAttributes.Getter;
			method.IsGetter = true;

			//Add the CompilerGeneratedAttribute or if you decompile the getter body will be shown
			method.CustomAttributes.Add(new CustomAttribute(
				SourceDefinition.MainModule.Import(
					typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)
						.GetConstructors()
						.Single()
				)
			));

			//A-ok cap'
			return method;
		}

		/// <summary>
		/// Generates a property setter method
		/// </summary>
		/// <param name="property"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		MethodDefinition GenerateSetter(PropertyDefinition property, MethodAttributes attributes, bool instance = true)
		{
			//Create the method definition
			var method = new MethodDefinition("set_" + property.Name, attributes, property.Module.TypeSystem.Void);

			//Setters always have a 'value' variable, but it's really just a parameter. We need to add this.
			method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, property.PropertyType));

			//Reference - this is what we need to essentially replicate
			//IL_0000: ldarg.0
			//IL_0001: ldarg.1
			//IL_0002: stfld int32 OTAPI.Modification.Tile.Modifications.PropertyReferenceTest::'<myData>k__BackingField'
			//IL_0007: ret

			//Create the il processor so we can alter il
			var il = method.Body.GetILProcessor();

			//Load the current type instance if required
			if (instance)
				il.Append(il.Create(OpCodes.Ldarg_0));
			//Load the 'value' parameter we added (alternatively, we could do il.Create(OpCodes.Ldarg, <parameter definition>)
			il.Append(il.Create(OpCodes.Ldarg_1));
			//Store the parameters value into the backing field
			il.Append(il.Create(OpCodes.Stfld, property.DeclaringType.Field($"<{property.Name}>k__BackingField")));
			//Return from the method as we are done.
			il.Append(il.Create(OpCodes.Ret));

			//Set basic setter method details 
			method.Body.InitLocals = true;
			method.SemanticsAttributes = MethodSemanticsAttributes.Setter;
			method.IsSetter = true;

			//Add the CompilerGeneratedAttribute or if you decompile the getter body will be shown
			method.CustomAttributes.Add(new CustomAttribute(
				SourceDefinition.MainModule.Import(
					typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)
						.GetConstructors()
						.Single()
				)
			));

			//A-ok cap'
			return method;
		}
	}

	//**these are used as IL references to get a grasp of whats required
	//public class FieldReferenceTest
	//{
	//	public int myData;
	//}
	//public class PropertyReferenceTest
	//{
	//	public virtual int myData { get; set; }
	//	public virtual int myData_getter { get; }
	//}
	//public class ReferenceTester
	//{
	//	const int test_set = 9939;

	//	public void TestField()
	//	{
	//		var field = new FieldReferenceTest();

	//		field.myData = test_set;

	//		var fieldGet = field.myData;

	//		field.myData += test_set;
	//	}
	//	public void TestProp()
	//	{
	//		var prop = new PropertyReferenceTest();

	//		prop.myData = test_set;

	//		var propGet = prop.myData;

	//		prop.myData += test_set;
	//	}
	//}
}
