using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// Base class for numeral systems that have symbols to represent unitary values.
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class SymbolicNumeralSystem : NumeralSystem
    {
        /// <summary>
        /// Creates a numeral system using given symbols.
        /// </summary>
        protected SymbolicNumeralSystem(string symbols)
            : this(symbols.Length, symbols)
        { }

        /// <summary>
        /// Creates a numeral system for a base up to 64 with symbolic representation.
        /// </summary>
        /// <param name="base">Base of the numeral system, max 64.</param>
        protected SymbolicNumeralSystem(int @base)
            : this(@base, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+/")
        { }

        private SymbolicNumeralSystem(int @base, string symbols)
            : base(@base)
        {
            if (@base > symbols.Length)
                throw new ArgumentException("SymbolicNumeralSystem with base x requires a symbols string with at least x symbols.");

            this.Symbols = symbols.Substring(0, @base);
        }

        /// <summary>
        /// The symbols of the numeral system.
        /// </summary>
        [DataMember]
        public string Symbols { get; private set; }
        
        public override char GetSymbolFor(int value)
        {
            return this.Symbols[value % this.Base];
        }

        public override int GetValueForSymbol(char symbol)
        {
            return this.Symbols.IndexOf(symbol);
        }

        /// <summary>
        /// Returns a string representation of the given value expressed in this numeral system.
        /// </summary>
        public override string ValueToString(string value)
        {
            return value;
        }
    }
}
