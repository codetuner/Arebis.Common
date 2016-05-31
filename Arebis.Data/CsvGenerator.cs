using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;

namespace Arebis.Data
{
    /// <summary>
    /// Generates CSV output based on a DataReader.
    /// </summary>
    [Serializable]
    public class CsvGenerator
    {
        /// <summary>
        /// Constructs a CsvGenerator using the current format provider (culture).
        /// </summary>
        public CsvGenerator()
            : this(null)
        { }

        /// <summary>
        /// Constructs a CsvGenerator using the given format provider (culture).
        /// </summary>
        public CsvGenerator(CsvGeneratorSettings settings)
        {
            this.Settings = settings ?? new CsvGeneratorSettings();
        }

        public CsvGeneratorSettings Settings { get; set; }

        /// <summary>
        /// Generate CSV formatted output for the given reader.
        /// </summary>
        public string Generate(System.Data.Common.DbDataReader reader)
        {
            var builder = new StringBuilder();

            var schema = reader.GetSchemaTable();
            var colcount = reader.FieldCount;
            var nullable = new bool[colcount];
            var datatype = new Type[colcount];
            var typename = new string[colcount];

            for(int c=0; c<colcount; c++)
            {
                nullable[c] = true;
                datatype[c] = reader.GetFieldType(c);
                typename[c] = reader.GetDataTypeName(c);

                if (c == 0)
                {
                    if (this.Settings.AddLineNumbers)
                    {
                        if (this.Settings.QuotedStrings) builder.Append(this.Settings.StringQuote);
                        builder.Append("Line");
                        if (this.Settings.QuotedStrings) builder.Append(this.Settings.StringQuote);
                        builder.Append(this.Settings.FieldSeparator);
                    }
                }
                else
                {
                    builder.Append(this.Settings.FieldSeparator);
                }
                if (this.Settings.QuotedStrings) builder.Append(this.Settings.StringQuote);
                builder.Append(reader.GetName(c));
                if (this.Settings.QuotedStrings) builder.Append(this.Settings.StringQuote);
            }

            builder.Append(this.Settings.LineSeparator);

            var lineNumber = 0;
            while (reader.Read())
            {
                lineNumber++;

                for (int c = 0; c < colcount; c++)
                {
                    if (c == 0)
                    {
                        if (this.Settings.AddLineNumbers)
                        {
                            builder.Append(lineNumber);
                            builder.Append(this.Settings.FieldSeparator);
                        }
                    }
                    else
                    {
                        builder.Append(this.Settings.FieldSeparator);
                    }

                    if (nullable[c] && reader.IsDBNull(c))
                    {
                    }
                    else
                    {
                        if (datatype[c] == typeof(String))
                        {
                            if (this.Settings.QuotedStrings) builder.Append(this.Settings.StringQuote);
                            builder.Append(ToCsvableString(reader.GetString(c)));                            
                            if (this.Settings.QuotedStrings) builder.Append(this.Settings.StringQuote);
                        }
                        else if (datatype[c] == typeof(DateTime))
                        {
                            builder.Append(reader.GetDateTime(c).ToString(this.Settings.DateTimeFormat, this.Settings.FormatProvider));
                        }
                        else if (datatype[c] == typeof(Boolean))
                        {
                            builder.Append(reader.GetBoolean(c) ? this.Settings.BooleanTrue : this.Settings.BooleanFalse);
                        }
                        else
                        {
                            builder.AppendFormat(this.Settings.FormatProvider, "{0}", reader.GetValue(c));
                        }
                    }
                }

                builder.Append(this.Settings.LineSeparator);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Transforms a string to a valid value in CSV format.
        /// </summary>
        protected virtual string ToCsvableString(string value)
        {
            if (this.Settings.LineSeparatorSubstitute != null)
                value = value.Replace(this.Settings.LineSeparator, this.Settings.LineSeparatorSubstitute);

            if (this.Settings.QuotedStrings)
            {
                if (this.Settings.StringQuoteSubstitute != null)
                    value = value.Replace(this.Settings.StringQuote, this.Settings.StringQuoteSubstitute);
            }
            else
            {
                if (this.Settings.FieldSeparatorSubstitute != null)
                    value = value.Replace(this.Settings.FieldSeparator, this.Settings.FieldSeparatorSubstitute);
            }

            return value;
        }
    }

    [Serializable]
    public class CsvGeneratorSettings
    {
        /// <summary>
        /// Constructs a CsvGenerator using the current format provider (culture).
        /// </summary>
        public CsvGeneratorSettings()
            : this(null)
        { }

        /// <summary>
        /// Constructs a CsvGenerator using the given format provider (culture).
        /// </summary>
        public CsvGeneratorSettings(CultureInfo formatProvider = null, string fieldSeparator = null, string lineSeparator = "\r\n", bool quotedStrings = true, string stringQuote = "\"", string dateTimeFormat = "yyyy-MM-dd HH:mm:ss", bool addLineNumbers = false, string booleanTrue = "True", string booleanFalse = "False")
        {
            this.FormatProvider = formatProvider ?? CultureInfo.CurrentCulture;
            this.FieldSeparator = fieldSeparator ?? this.FormatProvider.TextInfo.ListSeparator;
            this.LineSeparator = lineSeparator;
            this.QuotedStrings = quotedStrings;
            this.StringQuote = stringQuote;
            this.DateTimeFormat = dateTimeFormat;
            this.AddLineNumbers = addLineNumbers;
            this.FieldSeparatorSubstitute = (this.FieldSeparator != ",") ? "," : ".";
            this.LineSeparatorSubstitute = lineSeparator;
            this.StringQuoteSubstitute = stringQuote + stringQuote;
            this.BooleanTrue = booleanTrue;
            this.BooleanFalse = booleanFalse;
        }

        /// <summary>
        /// The format provider (culture).
        /// </summary>
        public CultureInfo FormatProvider { get; set; }

        /// <summary>
        /// Character sequence to separate fields.
        /// </summary>
        public string FieldSeparator { get; set; }

        /// <summary>
        /// By what to replace occurences of FieldSeparator within strings when string are not quoted.
        /// </summary>
        public string FieldSeparatorSubstitute { get; set; }

        /// <summary>
        /// Character sequence to separate records.
        /// </summary>
        public string LineSeparator { get; set; }

        /// <summary>
        /// By what to replace occurences of LineSeparator within strings.
        /// </summary>
        public string LineSeparatorSubstitute { get; set; }

        /// <summary>
        /// Whether strings are to be quoted.
        /// </summary>
        public bool QuotedStrings { get; set; }

        /// <summary>
        /// Quotation mark to delimit string values.
        /// </summary>
        public string StringQuote { get; set; }

        /// <summary>
        /// By what to replace occurences of StringQuote within strings when strings are quoted.
        /// </summary>
        public string StringQuoteSubstitute { get; set; }

        /// <summary>
        /// Format string for DateTime values.
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// True value for booleans
        /// </summary>
        public string BooleanTrue { get; set; }

        /// <summary>
        /// False value for booleans
        /// </summary>
        public string BooleanFalse { get; set; }

        /// <summary>
        /// Whether to add a first column with line numbers.
        /// </summary>
        public bool AddLineNumbers { get; set; }
    }
}
