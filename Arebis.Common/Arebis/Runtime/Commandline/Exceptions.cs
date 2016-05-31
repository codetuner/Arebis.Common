using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Runtime.Commandline
{
	public class TooManyArgumentsException : Exception
	{
	}

	public class InvalidOption : Exception
	{
	}

	public class InvalidOptionValue : Exception
	{
	}
}
