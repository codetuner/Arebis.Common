using System;
using System.Reflection.Emit;

namespace Arebis.Reflection
{
	/// <summary>
	/// Represents an IL instruction.
	/// </summary>
	/// <remarks>Source: http://www.codeproject.com/KB/cs/sdilreader.aspx (Sorin Serban)</remarks>
	public class ILInstruction
	{
		// Fields
		private ILanguageInfo languageInfo;
		private OpCode code;
		private object operand;
		private byte[] operandData;
		private int offset;

		public ILInstruction(ILanguageInfo languageInfo)
		{
			this.languageInfo = languageInfo ?? new DefaultLanguageInfo();
		}

		// Properties
		public OpCode Code
		{
			get { return code; }
			set { code = value; }
		}

		public object Operand
		{
			get { return operand; }
			set { operand = value; }
		}

		public byte[] OperandData
		{
			get { return operandData; }
			set { operandData = value; }
		}

		public int Offset
		{
			get { return offset; }
			set { offset = value; }
		}

		/// <summary>
		/// Returns a friendly string representation of this instruction.
		/// </summary>
		public string GetCode()
		{
			string result = "";
			result += GetExpandedOffset(offset) + " : " + code;
			if (operand != null)
			{
				switch (code.OperandType)
				{
					case OperandType.InlineField:
						System.Reflection.FieldInfo fOperand = ((System.Reflection.FieldInfo)operand);
						result += " " + this.languageInfo.GetFiendlyName(fOperand.FieldType) + " " +
							this.languageInfo.GetFiendlyName(fOperand.ReflectedType) +
							"::" + fOperand.Name + "";
						break;
					case OperandType.InlineMethod:
						try
						{
							System.Reflection.MethodInfo mOperand = (System.Reflection.MethodInfo)operand;
							result += " ";
							if (!mOperand.IsStatic) result += "instance ";
							result += this.languageInfo.GetFiendlyName(mOperand.ReturnType) +
								" " + this.languageInfo.GetFiendlyName(mOperand.ReflectedType) +
								"::" + mOperand.Name + "()";
						}
						catch
						{
							try
							{
								System.Reflection.ConstructorInfo mOperand = (System.Reflection.ConstructorInfo)operand;
								result += " ";
								if (!mOperand.IsStatic) result += "instance ";
								result += "void " +
									this.languageInfo.GetFiendlyName(mOperand.ReflectedType) +
									"::" + mOperand.Name + "()";
							}
							catch
							{
							}
						}
						break;
					case OperandType.ShortInlineBrTarget:
					case OperandType.InlineBrTarget:
						result += " " + GetExpandedOffset((int)operand);
						break;
					case OperandType.InlineType:
						result += " " + this.languageInfo.GetFiendlyName((Type)operand);
						break;
					case OperandType.InlineString:
						if (operand.ToString() == "\r\n") result += " \"\\r\\n\"";
						else result += " \"" + operand.ToString() + "\"";
						break;
					case OperandType.ShortInlineVar:
						result += operand.ToString();
						break;
					case OperandType.InlineI:
					case OperandType.InlineI8:
					case OperandType.InlineR:
					case OperandType.ShortInlineI:
					case OperandType.ShortInlineR:
						result += operand.ToString();
						break;
					case OperandType.InlineTok:
						if (operand is Type)
							result += ((Type)operand).FullName;
						else
							result += "not supported";
						break;

					default: result += "not supported"; break;
				}
			}
			return result;
		}

		/// <summary>
		/// Add enough zeros to a number as to be represented on 4 characters
		/// </summary>
		/// <param name="offset">
		/// The number that must be represented on 4 characters
		/// </param>
		/// <returns>
		/// </returns>
		private string GetExpandedOffset(long offset)
		{
			string result = offset.ToString();
			for (int i = 0; result.Length < 4; i++)
			{
				result = "0" + result;
			}
			return result;
		}
	}
}
