using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Arebis.Reflection
{
	/// <summary>
	/// Reads and represents a method's body.
	/// </summary>
	/// <remarks>Source: http://www.codeproject.com/KB/cs/sdilreader.aspx (Sorin Serban)</remarks>
	public class MethodBodyReader
	{
		private readonly Type[] NoTypes = new Type[0];

		private static OpCode[] singleByteOpCodes;
		private static OpCode[] multiByteOpCodes;

		private MethodBase method = null;
		private ILanguageInfo language = null;
		private byte[] il = null;
		private List<ILInstruction> instructions = null;

		/// <summary>
		/// Static constructor, initializes the OpCode arrays.
		/// </summary>
		static MethodBodyReader()
		{
			singleByteOpCodes = new OpCode[0x100];
			multiByteOpCodes = new OpCode[0x100];
			foreach (FieldInfo fieldInfo in typeof(OpCodes).GetFields())
			{
				//Console.WriteLine("  {0} = {1}", (OpCode)fieldInfo.GetValue(null), ((OpCode)fieldInfo.GetValue(null)).Value);

				if (fieldInfo.FieldType == typeof(OpCode))
				{
					OpCode code1 = (OpCode)fieldInfo.GetValue(null);
					ushort num2 = unchecked((ushort)code1.Value);

					//if (num2 == -512)
					//    System.Diagnostics.Debugger.Break();

					if (num2 < 0x100)
					{
						singleByteOpCodes[num2] = code1;
					}
					else
					{
						if ((num2 & 0xff00) != 0xfe00)
						{
							throw new Exception("Invalid OpCode.");
						}
						multiByteOpCodes[num2 & 0xff] = code1;
					}
				}
			}
		}

		/// <summary>
		/// MethodBodyReader constructor
		/// </summary>
		/// <param name="method">The method to read the body from.</param>
		public MethodBodyReader(MethodBase method)
			: this(method, null)
		{
		}

		/// <summary>
		/// MethodBodyReader constructor
		/// </summary>
		/// <param name="method">The method to read the body from.</param>
		/// <param name="language">Code language to use for text representations of code.</param>
		public MethodBodyReader(MethodBase method, ILanguageInfo language)
		{
			try
			{
				this.method = method;
				this.language = language ?? new DefaultLanguageInfo();
				MethodBody body = method.GetMethodBody();
				if (body != null)
				{
					il = body.GetILAsByteArray();
					ConstructInstructions(method.Module);
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(String.Format("Failed to read the body of {0}'s {1}.", method.DeclaringType, method), ex);
			}
		}

		/// <summary>
		/// IL code of the method body.
		/// </summary>
		public byte[] IL
		{
			get { return this.il; }
		}

		/// <summary>
		/// Instructions of the method body.
		/// </summary>
		public IList<ILInstruction> Instructions
		{
			get { return this.instructions; }
		}

		/// <summary>
		/// The current method.
		/// </summary>
		public MethodBase Method
		{
			get { return this.method; }
		}

		/// <summary>
		/// Returns a list of methods called.
		/// </summary>
		public IList<MethodBase> GetCalledMethods(bool includePropertyAccessors, bool includeOperators)
		{
			List<MethodBase> result = new List<MethodBase>();

			// Loop over all instructions:
			if (this.instructions != null)
			{
				foreach (ILInstruction instruction in this.instructions)
				{
					MethodBase calledMethod = (instruction.Operand as MethodBase);

					// Skip non-methodcalls:
					if (calledMethod == null)
						continue;

					// Filter methods:
					if (calledMethod.IsSpecialName)
					{
						// Skip property accessors:
						if ((includePropertyAccessors == false) && ((calledMethod.Name.StartsWith("get_")) || (calledMethod.Name.StartsWith("set_"))))
							continue;

						// Skip operators:
						if ((includeOperators == false) && (calledMethod.Name.StartsWith("op_")))
							continue;
					}

					// Add methodcall to result:
					result.Add(calledMethod);
				}
			}

			// Return result:
			return result;
		}

		/// <summary>
		/// Gets the IL code of the method
		/// </summary>
		public string GetBodyCode()
		{
			string result = "";
			if (instructions != null)
			{
				for (int i = 0; i < instructions.Count; i++)
				{
					result += instructions[i].GetCode() + Environment.NewLine;
				}
			}
			return result;
		}

		/// <summary>
		/// Constructs the array of ILInstructions according to the IL byte code.
		/// </summary>
		private void ConstructInstructions(Module module)
		{
			byte[] il = this.il;
			int position = 0;
			instructions = new List<ILInstruction>();
			while (position < il.Length)
			{
				ILInstruction instruction = new ILInstruction(this.language);

				// get the operation code of the current instruction
				OpCode code = OpCodes.Nop;
				ushort value = il[position++];
				if (value != 0xfe)
				{
					code = singleByteOpCodes[(int)value];
				}
				else
				{
					value = il[position++];
					code = multiByteOpCodes[(int)value];
					value = (ushort)(value | 0xfe00);
				}
				instruction.Code = code;
				instruction.Offset = position - 1;
				int metadataToken = 0;
				// get the operand of the current operation
				switch (code.OperandType)
				{
					case OperandType.InlineBrTarget:
						metadataToken = ReadInt32(il, ref position);
						metadataToken += position;
						instruction.Operand = metadataToken;
						break;
					case OperandType.InlineField:
						metadataToken = ReadInt32(il, ref position);
                        if (this.method.GetType().Name == "RuntimeConstructorInfo")
							instruction.Operand = module.ResolveField(metadataToken, this.method.DeclaringType.GetGenericArguments(), NoTypes);
						else
							instruction.Operand = module.ResolveField(metadataToken, this.method.DeclaringType.GetGenericArguments(), this.method.GetGenericArguments());
						break;
					case OperandType.InlineMethod:
						metadataToken = ReadInt32(il, ref position);
						try
						{
							if (this.method.GetType().Name == "RuntimeConstructorInfo")
								instruction.Operand = module.ResolveMethod(metadataToken, this.method.DeclaringType.GetGenericArguments(), NoTypes);
							else
								instruction.Operand = module.ResolveMethod(metadataToken, this.method.DeclaringType.GetGenericArguments(), this.method.GetGenericArguments());
						}
						catch
						{
                            if (this.method.GetType().Name == "RuntimeConstructorInfo")
								instruction.Operand = module.ResolveMember(metadataToken, this.method.DeclaringType.GetGenericArguments(), NoTypes);
							else
								instruction.Operand = module.ResolveMember(metadataToken, this.method.DeclaringType.GetGenericArguments(), this.method.GetGenericArguments());
						}
						break;
					case OperandType.InlineSig:
						metadataToken = ReadInt32(il, ref position);
						instruction.Operand = module.ResolveSignature(metadataToken);
						break;
					case OperandType.InlineTok:
						metadataToken = ReadInt32(il, ref position);
						try
						{
                            if (this.method.GetType().Name == "RuntimeConstructorInfo")
								instruction.Operand = module.ResolveType(metadataToken, this.method.DeclaringType.GetGenericArguments(), NoTypes);
							else
								instruction.Operand = module.ResolveType(metadataToken, this.method.DeclaringType.GetGenericArguments(), this.method.GetGenericArguments());
						}
						catch
						{

						}
						// SSS : see what to do here
						break;
					case OperandType.InlineType:
						metadataToken = ReadInt32(il, ref position);
						// now we call the ResolveType always using the generic attributes type in order
						// to support decompilation of generic methods and classes

						// thanks to the guys from code project who commented on this missing feature

						if (this.method.GetType().Name == "RuntimeConstructorInfo")
							instruction.Operand = module.ResolveType(metadataToken, this.method.DeclaringType.GetGenericArguments(), NoTypes);
						else
							instruction.Operand = module.ResolveType(metadataToken, this.method.DeclaringType.GetGenericArguments(), this.method.GetGenericArguments());
						break;
					case OperandType.InlineI:
						{
							instruction.Operand = ReadInt32(il, ref position);
							break;
						}
					case OperandType.InlineI8:
						{
							instruction.Operand = ReadInt64(il, ref position);
							break;
						}
					case OperandType.InlineNone:
						{
							instruction.Operand = null;
							break;
						}
					case OperandType.InlineR:
						{
							instruction.Operand = ReadDouble(il, ref position);
							break;
						}
					case OperandType.InlineString:
						{
							metadataToken = ReadInt32(il, ref position);
							instruction.Operand = module.ResolveString(metadataToken);
							break;
						}
					case OperandType.InlineSwitch:
						{
							int count = ReadInt32(il, ref position);
							int[] casesAddresses = new int[count];
							for (int i = 0; i < count; i++)
							{
								casesAddresses[i] = ReadInt32(il, ref position);
							}
							int[] cases = new int[count];
							for (int i = 0; i < count; i++)
							{
								cases[i] = position + casesAddresses[i];
							}
							break;
						}
					case OperandType.InlineVar:
						{
							instruction.Operand = ReadUInt16(il, ref position);
							break;
						}
					case OperandType.ShortInlineBrTarget:
						{
							instruction.Operand = ReadSByte(il, ref position) + position;
							break;
						}
					case OperandType.ShortInlineI:
						{
							instruction.Operand = ReadSByte(il, ref position);
							break;
						}
					case OperandType.ShortInlineR:
						{
							instruction.Operand = ReadSingle(il, ref position);
							break;
						}
					case OperandType.ShortInlineVar:
						{
							instruction.Operand = ReadByte(il, ref position);
							break;
						}
					default:
						{
							throw new Exception("Unknown operand type.");
						}
				}
				instructions.Add(instruction);
			}
		}

		#region Private IL Read Methods

		private int ReadInt16(byte[] _il, ref int position)
		{
			return ((il[position++] | (il[position++] << 8)));
		}

		private ushort ReadUInt16(byte[] _il, ref int position)
		{
			return (ushort)((il[position++] | (il[position++] << 8)));
		}

		private int ReadInt32(byte[] _il, ref int position)
		{
			return (((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18));
		}

		private ulong ReadInt64(byte[] _il, ref int position)
		{
			return (ulong)(((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18) | (il[position++] << 0x20) | (il[position++] << 0x28) | (il[position++] << 0x30) | (il[position++] << 0x38));
		}

		private double ReadDouble(byte[] _il, ref int position)
		{
			return (((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18) | (il[position++] << 0x20) | (il[position++] << 0x28) | (il[position++] << 0x30) | (il[position++] << 0x38));
		}

		private sbyte ReadSByte(byte[] _il, ref int position)
		{
			return (sbyte)il[position++];
		}

		private byte ReadByte(byte[] _il, ref int position)
		{
			return (byte)il[position++];
		}

		private Single ReadSingle(byte[] _il, ref int position)
		{
			return (Single)(((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18));
		}

		#endregion
	}
}
