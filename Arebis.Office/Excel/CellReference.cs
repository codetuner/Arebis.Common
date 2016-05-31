using System;
using System.Diagnostics;

namespace Arebis.Office.Excel
{
    /// <summary>
    /// An immutable spreadsheet cell reference representation.
    /// </summary>
    public sealed class CellReference
    {
        /// <summary>
        /// Creates cell reference.
        /// </summary>
        /// <param name="reference">Cell reference in A1 format.</param>
        public CellReference(string reference)
        {
            int row = 0;
            int col = 0;
            foreach (var c in reference)
            { 
                if (c >= '0' && c <= '9')
                    row = (row * 10) + (c - '0');
                else if (c >= 'A' && c <= 'Z')
                    col = (col * 26) + (c - 'A');
                else if (c >= 'a' && c <= 'z')
                    col = (col * 26) + (c - 'a');
                else if (c == '$')
                    Debug.Assert(true); // Ignore
            }

            this.Row = row;
            this.Column = col + 1;
        }

        /// <summary>
        /// Creates cell reference.
        /// </summary>
        /// <param name="row">Row number starting at 1.</param>
        /// <param name="column">Column number starting at 1.</param>
        public CellReference(int row, int column)
        {
            if (row < 1) throw new ArgumentOutOfRangeException("row", "Row of CellReference must be 1 or more.");
            if (column < 1) throw new ArgumentOutOfRangeException("column", "Column of CellReference must be 1 or more.");
            this.Row = (int)row;
            this.Column = (int)column;
        }

        /// <summary>
        /// The row number.
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// The column number.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// The column name (A, B, C, ... AA, AB, etc).
        /// </summary>
        public string ColumnName
        {
            get
            {
                // From: https://social.msdn.microsoft.com/Forums/office/en-US/81ad2c70-aa0b-4336-8eca-7b99f89fb77a/using-r1c1-style-references-with-openxml-sdk-25-spreadsheetml?forum=oxmlsdk
                int dividend = this.Column;
                string columnName = String.Empty;
                int modulo;

                while (dividend > 0)
                {
                    modulo = (dividend - 1) % 26;
                    columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                    dividend = (int)((dividend - modulo) / 26);
                }

                return columnName;
            }
        }

        /// <summary>
        /// This cell reference in A1 format.
        /// </summary>
        public string AsA1
        {
            get
            {
                return this.ColumnName + this.Row;
            }
        }

        /// <summary>
        /// This cell reference in R1C1 format.
        /// </summary>
        public string AsR1C1
        {
            get
            {
                return "R" + this.Row + "C" + this.Column;
            }
        }

        /// <summary>
        /// Returns a new CellReference relative to the current one.
        /// </summary>
        public CellReference Add(int rowDelta, int columnDelta)
        {
            return new CellReference(this.Row + rowDelta, this.Column + columnDelta);
        }

        /// <summary>
        /// Returns a new CellReference on the given row and the same column.
        /// </summary>
        public CellReference SetRow(int row)
        {
            return new CellReference(row, this.Column);
        }

        /// <summary>
        /// Returns a new CellReference on the same row and the given column.
        /// </summary>
        public CellReference SetColumn(int column)
        {
            return new CellReference(this.Row, column);
        }

        /// <summary>
        /// Return the cell reference in A1 format.
        /// </summary>
        public override string ToString()
        {
            return this.AsA1;
        }

        /// <summary>
        /// Whether both cell references point to the same cell.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(CellReference))
            {
                var other = (CellReference)obj;
                return ((this.Row == other.Row) && (this.Column == other.Column));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// A hash code for this cell reference.
        /// </summary>
        public override int GetHashCode()
        {
            return this.GetType().GetHashCode() ^ (104743 * Column) ^ Row;
        }

        /// <summary>
        /// Converts a string in A1 or R1C1 format to a CellReference.
        /// </summary>
        public static implicit operator CellReference(string s)
        {
            return new CellReference(s);
        }

        /// <summary>
        /// Converts a CellReference to a string.
        /// </summary>
        public static implicit operator string(CellReference r)
        {
            return r.ToString();
        }

        /// <summary>
        /// Private static helper method to convert column names to numbers.
        /// </summary>
        private static int ColumnNameToColumn(string name)
        {
            name = name.ToUpperInvariant();
            var col = 0;
            foreach (var c in name)
            {
                col *= 26;
                col += (c - 64);
            }

            return col;
        }
    }
}
