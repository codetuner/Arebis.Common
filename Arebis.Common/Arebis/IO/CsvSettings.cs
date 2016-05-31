using System;
using System.Globalization;

namespace Arebis.Common.IO
{
    /// <summary>
    /// Settings for reading and writing CSV streams.
    /// </summary>
    public class CsvSettings
    {
        private char? fieldDelimiter;
        private char? quoteCharacter;
        private char? escapeCharacter;
        private string dateTimeFormat;
        private string dateFormat;

        /// <summary>
        /// Creates a CsvSettings for the current culture.
        /// </summary>
        public CsvSettings()
            : this(CultureInfo.CurrentCulture)
        { }

        /// <summary>
        /// Creates a CsvSettings for the given culture.
        /// </summary>
        public CsvSettings(CultureInfo culture)
        {
            this.Locale = culture;
            this.QuoteCharacter = '"';
            this.EscapeCharacter = '"';
        }

        /// <summary>
        /// The culture the CSV file is formatted in.
        /// </summary>
        public CultureInfo Locale { get; set; }

        /// <summary>
        /// Whether the CsvReader should try to identify Dates, DateTimes, Numbers and Uris.
        /// </summary>
        public bool TypedParsing { get; set; }

        /// <summary>
        /// The field delimiter. By default matches the list separator of the current culture.
        /// </summary>
        public char? FieldDelimiter
        {
            get {
                if (fieldDelimiter.HasValue)
                {
                    return fieldDelimiter.Value;
                }
                else if (this.Locale != null)
                {
                    return this.Locale.TextInfo.ListSeparator[0];
                }
                else
                {
                    return ',';
                }
            }
            set { fieldDelimiter = value; }
        }

        /// <summary>
        /// The character to quote values.
        /// </summary>
        public char QuoteCharacter
        {
            get
            {
                if (quoteCharacter.HasValue)
                    return quoteCharacter.Value;
                else
                    return '"';
            }
            set { quoteCharacter = value; }
        }

        /// <summary>
        /// Escape character within quoted values.
        /// </summary>
        public char EscapeCharacter
        {
            get
            {
                if (escapeCharacter.HasValue)
                    return escapeCharacter.Value;
                else
                    return '"';
            }
            set { escapeCharacter = value; }
        }

        /// <summary>
        /// The format dates are written in CSV. By default the short date format of the
        /// current culture.
        /// </summary>
        public string DateFormat
        {
            get
            {
                if (dateFormat != null)
                {
                    return dateFormat;
                }
                else if (this.Locale != null)
                {
                    return this.Locale.DateTimeFormat.ShortDatePattern.Replace("'", "");
                }
                else
                {
                    return null;
                }
            }
            set { dateFormat = value; }
        }

        /// <summary>
        /// The format datetimes are written in CSV. By default the short date format 
        /// and the long time format (including seconds) of the current culture.
        /// </summary>
        public string DateTimeFormat
        {
            get {
                if (dateTimeFormat != null)
                {
                    return dateTimeFormat;
                }
                else if (this.Locale != null)
                {
                    return this.Locale.DateTimeFormat.ShortDatePattern.Replace("'", "")
                        + " "
                        + this.Locale.DateTimeFormat.LongTimePattern.Replace("'", "");
                }
                else
                {
                    return null;
                }
            }
            set { dateTimeFormat = value; }
        }
    }
}
