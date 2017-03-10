using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmitHelper.Ast.Interfaces
{
	public interface ICompilationContext
	{
		/// <summary>
		/// Declares a local variable.
		/// </summary>
		/// <param name="type">A Type object that represents the type of the local variable.</param>
		/// <returns>The declared local variable.</returns>
		LocalBuilder DeclareLocal(Type type);

		/// <summary>
		/// Declares a new label.
		/// </summary>
		/// <returns>Returns a new label that can be used as a token for branching.</returns>
		Label DefineLabel();

		/// <summary>
		/// Marks the Microsoft intermediate language (MSIL) stream's current position with the given label.
		/// </summary>
		/// <param name="label">The label for which to set an index.</param>
		void MarkLabel(Label label);

		/// <summary>
		/// Begins an exception block for a non-filtered exception.
		/// </summary>
		/// <returns>The label for the end of the block. This will leave you in the correct place to execute finally blocks or to finish the try.</returns>
		Label BeginExceptionBlock();

		/// <summary>
		/// Begins a catch block.
		/// </summary>
		/// <param name="exceptionType">The Type object that represents the exception.</param>
		void BeginCatchBlock(Type exceptionType);

		/// <summary>
		///Ends an exception block.
		/// </summary>
		void EndExceptionBlock();

		/// <summary>
		/// Emits an instruction to throw an exception.  
		/// </summary>
		/// <param name="exType">The class of the type of exception to throw.</param>
		void ThrowException(Type exType);

		/// <summary>
		/// Puts the specified instruction onto the stream of instructions. 
		/// </summary>
		/// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
		void Emit(OpCode opCode);

		/// <summary>
		/// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed by the metadata token for the given string.
		/// </summary>
		/// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
		/// <param name="str">The String to be emitted.</param>
		void Emit(OpCode opCode, string str);

		/// <summary>
		/// Puts the specified instruction and numerical argument onto the Microsoft intermediate language (MSIL) stream of instructions.
		/// </summary>
		/// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
		/// <param name="param">The Int argument pushed onto the stream immediately after the instruction.</param>
		void Emit(OpCode opCode, int param);

		/// <summary>
		/// Puts the specified instruction and metadata token for the specified field onto the Microsoft intermediate language (MSIL) stream of instructions. 
		/// </summary>
		/// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
		/// <param name="field">A FieldInfo representing a field.</param>
		void Emit(OpCode opCode, FieldInfo field);

		/// <summary>
		/// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed by the index of the given local variable.
		/// </summary>
		/// <param name="opCode"></param>
		/// <param name="local">A local variable.</param>
		void Emit(OpCode opCode, LocalBuilder local);

		/// <summary>
		/// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream and leaves space to include a label when fixes are done.
		/// </summary>
		/// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
		/// <param name="label">The label to which to branch from this location.</param>
		void Emit(OpCode opCode, Label label);

		/// <summary>
		/// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed by the metadata token for the given type.
		/// </summary>
		/// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
		/// <param name="cls">A Type.</param>
		void Emit(OpCode opCode, Type cls);

		///// <summary>
		///// Puts the specified instruction and metadata token for the specified constructor onto the Microsoft intermediate language (MSIL) stream of instructions.
		///// </summary>
		///// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
		///// <param name="con">A ConstructorInfo representing a constructor.</param>
		//void Emit(OpCode opCode, ConstructorInfo con);

		/// <summary>
		/// Puts a call or callvirt instruction onto the Microsoft intermediate language (MSIL) stream to call a varargs method. 
		/// </summary>
		/// <param name="opCode">The MSIL instruction to be emitted onto the stream. Must be OpCodes.Call, OpCodes.Callvirt, or OpCodes.Newobj.</param>
		/// <param name="mi">The varargs method to be called.</param>
		void EmitCall(OpCode opCode, MethodInfo mi);

		/// <summary>
		/// Puts the OpCodes.Newobj instruction and metadata token for the specified constructor onto the Microsoft intermediate language (MSIL) stream of instructions.
		/// </summary>
		/// <param name="con">A ConstructorInfo representing a constructor.</param>
		void EmitNewObject(ConstructorInfo con);
	}
}
