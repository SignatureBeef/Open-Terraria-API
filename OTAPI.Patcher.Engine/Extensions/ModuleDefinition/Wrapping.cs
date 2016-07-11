using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Extensions
{
	public static partial class WrappingExtensions
	{
		const String WrappedMethodNameSuffix = "Direct";

		/// <summary>
		/// Replaces all occurrences of the current method in the assembly with the provided method
		/// </summary>
		/// <param name="method"></param>
		/// <param name="replacement"></param>
		public static void ReplaceWith(this MethodDefinition method, MethodDefinition replacement)
		{
			//Enumerates over each type in the assembly, including nested types
			method.Module.ForEachInstruction((mth, ins) =>
			{
				//Compare instruction operands method references to see if they match the
				//replacement method definition. If it matches, it can be swapped
				if (ins.Operand == method)
					ins.Operand = replacement;
			});
		}

		/// <summary>
		/// Creates a new parameter instruction for a method.
		/// </summary>
		/// <remarks>Useful if you are altering IL in a method and need a parameter reference</remarks>
		public static Instruction CreateMethodParameter(this MethodDefinition method, int index)
		{
			return Instruction.Create(OpCodes.Ldarg, method.Parameters[index]);
		}

		/// <summary>
		/// Emits il for a callback at the start of a 
		/// </summary>
		/// <param name="current"></param>
		/// <param name="callback"></param>
		/// <param name="callbackRequiresInstance"></param>
		/// <param name="isCancelable"></param>
		/// <returns></returns>
		public static Instruction EmitBeginCallback(this MethodDefinition current, MethodReference callback,
			bool instanceMethod,
			bool callbackRequiresInstance,
			bool isCancelable)
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
					var offset = callbackRequiresInstance ? 1 : 0;
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
			//and exception
			else if (importedCallback.ReturnType.Name != "Void")
			{
				targetInstruction = il.Create(OpCodes.Pop);
				il.Append(targetInstruction);
			}

			return targetInstruction;
		}

		/// <summary>
		/// Replaces transfers from other instructions (ie if, try) to a new instruction target
		/// </summary>
		/// <param name="current">The original instruction</param>
		/// <param name="newTarget">The new instruction that will receive the transfer</param>
		/// <param name="originalMethod">The original method that is used to search for transfers</param>
		public static void ReplaceTransfer(this Instruction current, Instruction newTarget, MethodDefinition originalMethod)
		{
			//If a method has a body then check the instruction targets & exceptions
			if (originalMethod.HasBody)
			{
				//Replaces instruction references from the old instruction to the new instruction
				foreach (var ins in originalMethod.Body.Instructions.Where(x => x.Operand == current))
					ins.Operand = newTarget;

				//If there are exception handlers, it's possible that they will also need to be switched over
				if (originalMethod.Body.HasExceptionHandlers)
				{
					foreach (var handler in originalMethod.Body.ExceptionHandlers)
					{
						if (handler.FilterStart == current) handler.FilterStart = newTarget;
						if (handler.HandlerEnd == current) handler.HandlerEnd = newTarget;
						if (handler.HandlerStart == current) handler.HandlerStart = newTarget;
						if (handler.TryEnd == current) handler.TryEnd = newTarget;
						if (handler.TryStart == current) handler.TryStart = newTarget;
					}
				}

				//Update the new target to take the old targets place
				newTarget.Offset = current.Offset;
				newTarget.SequencePoint = current.SequencePoint;
				newTarget.Offset++; //TODO: spend some time to figure out why this is incrementing
			}
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

			//If the end call has a value, pop it for the time being
			//In the case of begin callbacks, we use this value to determine
			//a cancel.
			if (importedCallback.ReturnType.Name != "Void")
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
			if (false == noHandling && method.ReturnType.Name != "Void")
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
		/// Emits IL that is used to execute a callback.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="target"></param>
		/// <param name="callbackExpectsInstance"></param>
		/// <param name="emitNonVoidPop"></param>
		/// <returns>The first instruction emitted</returns>
		public static Instruction EmitMethodCallback(this MethodDefinition method, MethodReference target, bool callbackExpectsInstance, bool emitNonVoidPop = true)
		{
			Instruction retFirstInstruction = null;
			//            var instanceMethod = (target.Attributes & MethodAttributes.Static) == 0;

			//Get the il processor instance so we can generate il
			var il = method.Body.GetILProcessor();

			//If the callback expects an instance we must emit the 'this' value.
			//This flag would also expect that the 'method' is a instance method.
			if (callbackExpectsInstance)
			{
				//Set the instruction to be resumed upon not cancelling, if not already
				var instance = il.Create(OpCodes.Ldarg_0);

				if (null == retFirstInstruction)
					retFirstInstruction = instance;

				il.Append(instance);
			}

			//Create the parameters to pass through to the callback
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

			//Invoke the callback now that we have everything on the stack
			var call = il.Create(OpCodes.Call, target);

			if (null == retFirstInstruction)
				retFirstInstruction = call;

			il.Append(call);

			//If a value is returned, ensure it's removed from the stack
			if (emitNonVoidPop && target.ReturnType.Name != "Void")
				il.Emit(OpCodes.Pop);

			return retFirstInstruction;
		}

		/// <summary>
		/// Wraps the current method with begin/end callbacks.
		/// </summary>
		/// <remarks>
		/// This will rename the current method and replace it with a new method that will take its place.
		/// In the new method it will call the callbacks and perform canceling on the begin callback if required.</remarks>
		/// <param name="current"></param>
		/// <param name="beginCallback"></param>
		/// <param name="endCallback"></param>
		public static MethodDefinition Wrap
		(
			this MethodDefinition current,
			MethodReference beginCallback,
			MethodReference endCallback,
			bool beginIsCancellable,
			bool noEndHandling,
			bool allowCallbackInstance
		)
		{
			if (!current.HasBody)
				throw new InvalidOperationException("Method must have a body.");

			//This method has only yet been tested on void & string return type methods.
			if (new[]
			{
				current.Module.TypeSystem.Void,
				current.Module.TypeSystem.String
			}.Contains(current.ReturnType))
			{
				//Create the new replacement method that will take place of the current method.
				//So we must ensure we clone to meet the signatures.
				var wrapped = new MethodDefinition(current.Name, current.Attributes, current.ReturnType);
				var instanceMethod = (current.Attributes & MethodAttributes.Static) == 0;

				//Rename the existing method, and replace all references to it so that the new 
				//method receives the calls instead.
				current.Name = current.Name + WrappedMethodNameSuffix;
				//If we are renaming a virtual method, it does not need to be virtual anymore
				//as we want exclusive control over it.
				current.IsVirtual = false;
				//Finally replace all instances of the current method with the wrapped method
				//that is about to be generated
				current.ReplaceWith(wrapped);

				//Clone the parameters for the new method
				if (current.HasParameters)
				{
					foreach (var prm in current.Parameters)
					{
						wrapped.Parameters.Add(prm);
					}
				}

				//Place the new method in the declaring type of the method we are cloning
				current.DeclaringType.Methods.Add(wrapped);

				//Generate the il that will call and handle the begin callback.
				var beginResult = wrapped.EmitBeginCallback(beginCallback, instanceMethod, allowCallbackInstance, beginIsCancellable);

				//Emit the il that will execute the actual method that was renamed earlier 
				var insFirstForMethod = wrapped.EmitMethodCallback(current, instanceMethod, current.ReturnType.Name != "String");// && method.ReturnType.Name != "Double");

				//If the begin callback is cancelable, the EmitBeginCallback method will have left a Nop
				//instruction so we can direct where to continue on to, rather than exiting the method.
				if (beginIsCancellable && beginResult != null && beginResult.OpCode == OpCodes.Nop)
				{
					if (current.ReturnType == current.Module.TypeSystem.Void)
					{
						beginResult.OpCode = OpCodes.Brtrue_S;
						beginResult.Operand = insFirstForMethod;
					}
					else if (current.ReturnType == current.Module.TypeSystem.String)
					{
						beginResult.OpCode = OpCodes.Brtrue;
						beginResult.Operand = insFirstForMethod;
					}
				}

				//If a end callback is specified then we can also emit the callback to it as well
				if (endCallback != null)
				{
					wrapped.EmitEndCallback(endCallback, instanceMethod, allowCallbackInstance);
				}

				//The custom method is now fully generated, so we can now complete the il for exiting.
				wrapped.EmitMethodEnding(noEndHandling);

				return wrapped;
			}
			else throw new NotSupportedException("Non Void methods not yet supported");
		}

		/// <summary>
		/// Injects a cancelable (boolean) callback into the current method with the ability to return a value from the 
		/// method using a custom variable.
		/// 
		/// The callback must expect an instance parameter if the current method is instanced.
		/// Additionally, it must also have the result value by reference before any of the current method parameters are specified.
		/// </summary>
		public static void InjectNonVoidBeginCallback(this MethodDefinition current, MethodDefinition callback)
		{
			if (callback.ReturnType.Name == "Void")
				throw new InvalidOperationException("Invalid return type for callback");

			//Get the il processor instance so we can modify il
			var il = current.Body.GetILProcessor();

			//Import our external callback into the current module
			var importedCallback = current.Module.Import(callback);

			//We are processing a begin callback, so we insert before the first instruction.
			var firstInstruction = current.Body.Instructions.First();

			//Create our variable, to hold the modified callback result
			VariableDefinition vrbResult = null;
			current.Body.Variables.Add(vrbResult = new VariableDefinition("otapi_result", importedCallback.ReturnType));

			//If the current method is an instance method, we insert the 'this'/ldarg_0 value
			//for the callback.
			if ((current.Attributes & MethodAttributes.Static) == 0)
				il.InsertBefore(firstInstruction, il.Create(OpCodes.Ldarg_0)); //instance

			//Inject our custom result variable by reference so the callback can alter it
			il.InsertBefore(firstInstruction, il.Create(OpCodes.Ldloca_S, vrbResult));

			//Inject the parameters for the callback to use
			for (var i = 0; i < current.Parameters.Count; i++)
			{
				//Here we are looking at the callback to see if it wants a reference parameter.
				//If it does, and it also expects an instance to be passed, we must move the offset
				//by one to skip the previous ldarg_0 we added before.
				var offset = (current.Attributes & MethodAttributes.Static) == 0 ? 1 : 0;
				if (callback.Parameters[i + offset].ParameterType.IsByReference)
				{
					il.InsertBefore(firstInstruction, il.Create(OpCodes.Ldarga, current.Parameters[i]));
				}
				else il.InsertBefore(firstInstruction, il.Create(OpCodes.Ldarg, current.Parameters[i]));
			}

			//The parameters are now ready for the method call.
			//Here we now inject the actual callback execution instruction.
			il.InsertBefore(firstInstruction, il.Create(OpCodes.Call, importedCallback));

			//If the callback did not cancel, we transfer to the original instruction
			il.InsertBefore(firstInstruction, il.Create(OpCodes.Brtrue_S, firstInstruction));

			//If the callback did in fact cancel it, we can load up the result of our custom
			//variable and return it from the method instead.
			il.InsertBefore(firstInstruction, il.Create(OpCodes.Ldloc, vrbResult));
			il.InsertBefore(firstInstruction, il.Create(OpCodes.Ret));
		}
	}
}
