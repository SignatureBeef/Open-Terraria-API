using Mono.Cecil;
using Mono.Cecil.Cil;

namespace OTAPI.Patcher.Engine.Extensions
{
	public static partial class WrappingExtensions
	{
		/// <summary>
		/// Emits il for a callback at the start of a method
		/// </summary>
		/// <param name="current">The method being generated</param>
		/// <param name="callback">The callback to be executed at runtime</param>
		/// <param name="callbackRequiresInstance">Indicates the provided callback expects an instance argument at the first index</param>
		/// <param name="isCancelable">Generates il to handle canceling, emits a nop for continuation</param>
		/// <returns>The first instruction this method emitted</returns>
		public static Instruction EmitBeginCallback(this MethodDefinition current, MethodReference callback,
			bool instanceMethod,
			bool callbackRequiresInstance,
			bool isCancelable,
			int parameterOffset = 0)
		{
			Instruction targetInstruction = null;

			//Import the callbacks to the calling methods assembly
			var importedCallback = current.Module.Import(callback);

			//            var instanceMethod = (method.Attributes & MethodAttributes.Static) == 0;

			//Create the il processor so we can alter il
			var il = current.Body.GetILProcessor();

			//If the callback expects the instance to be passed through, then we add the keyword 'this'
			//onto the call stack. 
			//This also expects that the current method is an instance method.
			if (instanceMethod)
				il.Emit(OpCodes.Ldarg_0);

			//Emit the parameters for the callback
			if (current.HasParameters)
			{
				for (var i = 0; i < current.Parameters.Count; i++)
				{
					//Here we are looking at the callback to see if it wants a reference parameter.
					//If it does, and it also expects an instance to be passed, we must move the offset
					//by one to skip the previous ldarg_0 we added before.
					var offset =( callbackRequiresInstance ? 1 : 0) + parameterOffset;
					if (callback.Parameters[i + offset].ParameterType.IsByReference)
					{
						il.Emit(OpCodes.Ldarga, current.Parameters[i]);
					}
					else il.Emit(OpCodes.Ldarg, current.Parameters[i]);
				}
			}

			il.Emit(OpCodes.Call, importedCallback);

			//Used to inform the method caller the first instruction that we created
			//in the case that they need to replace instruction references.
			//Some cases this will eventually be transformed to a Brtrue_S in order
			//to transfer to normal code.
			targetInstruction = null;

			//If the callback can be canceled then we must generate the
			//il to handle it. We use a common function to do this for us.
			if (isCancelable)
			{
				//Nop is used externally in .Wrap to allow it to continue executing
				//code rather than exiting
				il.Append(targetInstruction = il.Create(OpCodes.Nop));
				current.EmitMethodEnding();
			}
			//If the callback has a return type and it has not been handled
			//we must pop the result value from the stack or it will cause 
			//an exception
			else if (importedCallback.ReturnType.Name != importedCallback.Module.TypeSystem.Void.Name)
			{
				targetInstruction = il.Create(OpCodes.Pop);
				il.Append(targetInstruction);
			}

			return targetInstruction;
		}

		/// <summary>
		/// Emits a method-end callback into the current method.
		/// Optionally, if <param name="callbackRequiresInstance"/> is specified it will insert a 'this' argument that
		/// will be sent to the first parameter of the callback.
		/// </summary>
		public static void EmitEndCallback(this MethodDefinition current, MethodReference callback,
			bool instanceMethod,
			bool callbackRequiresInstance
		)
		{
			//Import the callback to the calling methods assembly
			var importedCallback = current.Module.Import(callback);

			//            var instanceMethod = (method.Attributes & MethodAttributes.Static) == 0;

			//Get the il processor instance so we can modify IL
			var il = current.Body.GetILProcessor();

			//If the callback expects the instance, emit 'this'
			if (instanceMethod)
				il.Emit(OpCodes.Ldarg_0);

			//If there are parameters, add each of them to the stack for the callback
			if (current.HasParameters)
			{
				for (var i = 0; i < current.Parameters.Count; i++)
				{
					//Here we are looking at the callback to see if it wants a reference parameter.
					//If it does, and it also expects an instance to be passed, we must move the offset
					//by one to skip the previous ldarg_0 we added before.
					var offset = callbackRequiresInstance ? 1 : 0;
					if (callback.Parameters[i + offset].ParameterType.IsByReference)
					{
						il.Emit(OpCodes.Ldarga, current.Parameters[i]);
					}
					else il.Emit(OpCodes.Ldarg, current.Parameters[i]);
				}
			}

			//Execute the callback
			il.Emit(OpCodes.Call, importedCallback);

			//If the end call has a value, pop it for the time being.
			//In the case of begin callbacks, we use this value to determine
			//a cancel.
			if (importedCallback.ReturnType.Name != importedCallback.Module.TypeSystem.Void.Name)
				il.Emit(OpCodes.Pop);
		}

		/// <summary>
		/// Emits IL that is used to exit a method.
		/// This expects that you are appending to a method being built.
		/// </summary>
		public static Instruction EmitMethodEnding(this MethodDefinition method, bool noHandling = false)
		{
			Instruction firstInstruction = null;

			//Get the il processor instance so we can alter il
			var il = method.Body.GetILProcessor();

			//If we are working on a method with a return value
			//we will have a value to handle.
			if (false == noHandling && method.ReturnType.Name != method.Module.TypeSystem.Void.Name)
			{
				VariableDefinition vr1;
				method.Body.Variables.Add(vr1 = new VariableDefinition("cancelDefault", method.ReturnType));

				//Initialise the variable
				il.Append(firstInstruction = il.Create(OpCodes.Ldloca_S, vr1));
				il.Emit(OpCodes.Initobj, method.ReturnType);
				il.Emit(OpCodes.Ldloc, vr1);
			}

			//The method is now complete.
			if (firstInstruction == null)
				il.Append(firstInstruction = il.Create(OpCodes.Ret));
			else il.Emit(OpCodes.Ret);

			return firstInstruction;
		}

		/// <summary>
		/// Emits IL that is used to execute a generic method call.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="target"></param>
		/// <param name="methodExpectsInstance"></param>
		/// <param name="emitNonVoidPop"></param>
		/// <returns>The first instruction emitted</returns>
		/// <remarks>We use this to execute a vanilla wrapped method</remarks>
		public static Instruction EmitMethodCallback(this MethodDefinition method, MethodReference target, bool methodExpectsInstance, bool emitNonVoidPop = true)
		{
			Instruction retFirstInstruction = null;
			//            var instanceMethod = (target.Attributes & MethodAttributes.Static) == 0;

			//Get the il processor instance so we can generate il
			var il = method.Body.GetILProcessor();

			//If the method expects an instance we must emit the 'this' value.
			//This flag would also expect that the 'method' is a instance method.
			//TODO: consider checking attributes/IsStatic
			if (methodExpectsInstance)
			{
				var instance = il.Create(OpCodes.Ldarg_0);

				if (null == retFirstInstruction)
					retFirstInstruction = instance;

				il.Append(instance);
			}

			//Create the parameters to pass through to the method
			if (target.HasParameters)
			{
				for (var i = 0; i < target.Parameters.Count; i++)
				{
					var prm = il.Create(OpCodes.Ldarg, target.Parameters[i]);

					if (null == retFirstInstruction)
						retFirstInstruction = prm;

					il.Append(prm);
				}
			}

			//Invoke the method now that we have everything on the stack
			var call = il.Create(OpCodes.Call, target);

			if (null == retFirstInstruction)
				retFirstInstruction = call;

			il.Append(call);

			//If a value is returned, ensure it's removed from the stack
			if (emitNonVoidPop && target.ReturnType.Name != target.Module.TypeSystem.Void.Name)
				il.Emit(OpCodes.Pop);

			return retFirstInstruction;
		}
	}
}
