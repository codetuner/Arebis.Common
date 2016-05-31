using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Arebis.Office.Excel
{
    /// <summary>
    /// Tool class to easily create Excel workbooks.
    /// </summary>
    public class WorkbookMaker : IDisposable
    {
        #region Construct and Dispose

        /// <summary>
        /// Creates a new WorkbookMaker for a new, initially blank workbook.
        /// </summary>
        public WorkbookMaker()
            : this(null)
        { }

        /// <summary>
        /// Creates a new WorkbookMaker for a workbook based on the given template file.
        /// </summary>
        public WorkbookMaker(string templateFile)
        {
            if (templateFile == null)
            {
                this.Package = new ExcelPackage();
                this.IsNewDocument = true;
                var styleHeader = this.Package.Workbook.Styles.CreateNamedStyle("Header");
                styleHeader.Style.Font.Bold = true;
                styleHeader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                styleHeader.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }
            else
            {
                this.Package = new ExcelPackage(new FileInfo(templateFile), true);
                this.IsNewDocument = false;
            }
        }

        /// <summary>
        /// Disposes the workbook maker.
        /// </summary>
        public virtual void Dispose()
        {
            this.Package.Dispose();
        }

        #endregion

        /// <summary>
        /// Whether a new document (not based on a template) is being created.
        /// </summary>
        public bool IsNewDocument { get; private set; }

        /// <summary>
        /// Office OpenXML Package object.
        /// </summary>
        public ExcelPackage Package { get; private set; }

        /// <summary>
        /// The currently selected sheet.
        /// </summary>
        public ExcelWorksheet CurrentSheet { get; private set; }

        /// <summary>
        /// The currently selected cell.
        /// </summary>
        public ExcelRange Selection { get; private set; }

        /// <summary>
        /// Selects the given sheet and cell.
        /// If no sheet with the given name is found, one is created.
        /// </summary>
        public void Select(string sheetName, string cellAddress)
        {
            this.CurrentSheet = this.Package.Workbook.Worksheets[sheetName];

            if (this.CurrentSheet == null)
            {
                this.CurrentSheet = this.Package.Workbook.Worksheets.Add(sheetName);
            }

            this.Selection = this.CurrentSheet.Cells[cellAddress];
        }

        /// <summary>
        /// Writes data to the currently selected sheet starting at the
        /// currently selected cell.
        /// </summary>
        /// <param name="data">The data as an enumeration of rows wher each row is an enumeration of values.</param>
        /// <param name="translateFormulas">Whether to consider string values starting with "=" as A1 formula's and string values starting with "/=" as R1C1 formulas.</param>
        /// <param name="headerRows">Number of rows to style as headers. Does only work for new documents.</param>
        /// <param name="headerColumns">Number of columns to style as headers. Does only work for new documents.</param>
        public void Write(IEnumerable<IEnumerable<Object>> data, bool translateFormulas = false, int headerRows = 0, int headerColumns = 0)
        {
            var offsetrow = this.Selection.Start.Row;
            var offsetcol = this.Selection.Start.Column;

            var row = 0;
            foreach (var datarow in data)
            {
                var col = 0;
                foreach (var value in datarow)
                {
                    var cell = this.CurrentSheet.Cells[offsetrow + row, offsetcol + col];

                    if (value is String)
                    {
                        var svalue = (string)value;
                        if (translateFormulas && svalue.StartsWith("/="))
                        {
                            cell.FormulaR1C1 = svalue.Substring(2);
                        }
                        else if (translateFormulas && svalue.StartsWith("="))
                        {
                            cell.Formula = svalue.Substring(1);
                        }
                        else
                        {
                            cell.Value = svalue;
                        }
                    }
                    else if (value is DateTime)
                    {
                        var dvalue = (DateTime)value;
                        cell.Value = dvalue;
                        if (IsNewDocument)
                        {
                            if (dvalue.Date == dvalue)
                                cell.Style.Numberformat.Format = "yyyy/MM/dd";
                            else
                                cell.Style.Numberformat.Format = "yyyy/MM/dd HH:mm:ss";
                        }
                    }
                    else if (value is Uri)
                    {
                        var uvalue = (Uri)value;
                        cell.Value = uvalue.ToString();
                        cell.Hyperlink = uvalue;
                    }
                    else if (value == null)
                    {
                        cell.Value = null;
                    }
                    else if (value is DBNull)
                    {
                        cell.Value = null;
                    }
                    else
                    {
                        cell.Value = value;
                    }

                    if (IsNewDocument && ( row < headerRows || col < headerColumns))
                    {
                        cell.StyleName = "Header";
                    }

                    col++;
                }
                row++;
            }

            this.Selection = this.CurrentSheet.Cells[offsetrow + row, offsetcol];
        }

        public void SaveAs(string targetFile)
        {
            this.Package.SaveAs(new FileInfo(targetFile));
        }
    }
}
