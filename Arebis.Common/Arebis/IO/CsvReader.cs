using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Arebis.Common.IO
{
    /// <summary>
    /// Reads and parses a CSV source.
    /// </summary>
    public class CsvReader : IDisposable
    {
        private CsvSettings settings;
        private string dateTimeFormat;
        private string dateFormat;

        #region Constructors

        public CsvReader(string path)
            : this(new StreamReader(path), null, true)
        { }

        public CsvReader(string path, CsvSettings settings)
            : this(new StreamReader(path), settings, true)
        { }

        public CsvReader(TextReader reader)
            : this(reader, null, false)
        { }

        public CsvReader(TextReader reader, CsvSettings settings)
            : this(reader, settings, false)
        { }

        protected CsvReader(TextReader reader, CsvSettings settings, bool autoCloseReader)
        {
            this.Reader = reader;
            this.Settings = settings ?? new CsvSettings();
            this.AutoCloseReader = autoCloseReader;
        }

        public virtual void Dispose()
        {
            if (AutoCloseReader) Reader.Dispose();
        }

        #endregion

        /// <summary>
        /// Whether to close the reader automatically at the end.
        /// </summary>
        public bool AutoCloseReader { get; set; }

        /// <summary>
        /// Reader object to the CSV source.
        /// </summary>
        public TextReader Reader { get; private set; }

        /// <summary>
        /// Settings of the source to be read.
        /// </summary>
        public CsvSettings Settings
        {
            get { return settings; }
            set 
            { 
                this.settings = value;
                this.dateTimeFormat = value.DateTimeFormat;
                this.dateFormat = value.DateFormat;
            }
        }

        /// <summary>
        /// Performs reading of the data.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Object[]> ReadAll()
        {
            var currentLine = new List<String>();
            var currentValue = new StringBuilder(64);

            var state = States.Initial;
            while(true)
            {
                int i = Reader.Read();
            process:
                if (i == -1) break;
                var c = (char)i;
                switch (state)
                {
                    case States.Initial:
                        if (c == Settings.QuoteCharacter)
                        {
                            state = States.InQuotedField;
                        }
                        else if (c == Settings.EscapeCharacter)
                        {
                            currentValue.Append(ReadEscapeSequence());
                            state = States.InUnquotedField;
                        }
                        else if (c == Settings.FieldDelimiter.Value)
                        {
                            currentLine.Add(currentValue.ToString());
                            currentValue.Clear();
                            state = States.Initial;
                        }
                        else if (c == '\x0D' || c == '\x0A')
                        {
                            currentLine.Add(currentValue.ToString());
                            yield return TryTypeConversion(currentLine);
                            currentLine.Clear();
                            currentValue.Clear();
                            state = States.Initial;

                            if (c == '\x0D')
                            {
                                i = Reader.Read();
                                if (i != 10) goto process;
                            }
                        }
                        else
                        {
                            currentValue.Append(c);
                            state = States.InUnquotedField;
                        }
                        break;
                    case States.InQuotedField:
                        if (c == Settings.QuoteCharacter && c == Settings.EscapeCharacter)
                        {
                            i = Reader.Read();
                            if (i == (int)Settings.QuoteCharacter)
                            {
                                currentValue.Append(Settings.QuoteCharacter);
                            }
                            else
                            {
                                state = States.InUnquotedField;
                                goto process;
                            }
                        }
                        else if (c == Settings.EscapeCharacter)
                        {
                            currentValue.Append(ReadEscapeSequence());
                        }
                        else if (c == Settings.QuoteCharacter)
                        {
                            state = States.InUnquotedField;
                        }
                        else
                        {
                            currentValue.Append(c);
                        }
                        break;
                    case States.InUnquotedField:
                        if (c == Settings.EscapeCharacter)
                        {
                            currentValue.Append(ReadEscapeSequence());
                        }
                        else if (c == Settings.FieldDelimiter.Value)
                        {
                            currentLine.Add(currentValue.ToString());
                            currentValue.Clear();
                            state = States.Initial;
                        }
                        else if (c == '\x0D' || c == '\x0A')
                        {
                            currentLine.Add(currentValue.ToString());
                            yield return TryTypeConversion(currentLine);
                            currentLine.Clear();
                            currentValue.Clear();
                            state = States.Initial;

                            if (c == '\x0D')
                            {
                                i = Reader.Read();
                                if (i != 10) goto process;
                            }
                        }
                        else
                        {
                            currentValue.Append(c);
                        }
                        break;
                    default:
                        throw new ApplicationException("Unsupported CsvReader state.");
                }
            }

            // Return last data:
            currentLine.Add(currentValue.ToString());
            yield return TryTypeConversion(currentLine);
        }

        private Object[] TryTypeConversion(List<string> values)
        {
            var result = new Object[values.Count];

            if (this.Settings.TypedParsing == false)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = values[i];
                }
            }
            else
            {
                DateTime dt;
                Decimal dec;
                for (int i = 0; i < result.Length; i++)
                {
                    if (Decimal.TryParse(values[i], System.Globalization.NumberStyles.Any, this.Settings.Locale, out dec))
                    {
                        result[i] = dec;
                    }
                    else if (DateTime.TryParseExact(values[i], this.dateTimeFormat, this.Settings.Locale, System.Globalization.DateTimeStyles.None, out dt))
                    {
                        result[i] = dt;
                    }
                    else if (DateTime.TryParseExact(values[i], this.dateFormat, this.Settings.Locale, System.Globalization.DateTimeStyles.None, out dt))
                    {
                        result[i] = dt;
                    }
                    else if (values[i].StartsWith("http://") || values[i].StartsWith("ftp://"))
                    {
                        result[i] = new Uri(values[i]);
                    }
                    else
                    {
                        result[i] = values[i];
                    }
                }
            }

            return result;
        }

        protected virtual string ReadEscapeSequence()
        {
            var i = Reader.Read();
            if (i == -1) return String.Empty;
            return new String((char)i, 1);
        }

        private enum States
        { 
            Initial = 0,
            InUnquotedField = 1,
            InQuotedField = 2,       
        }
    }
}
