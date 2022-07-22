/*
 * Copyright (C) 2021 Tris Shores
 * Open source software. Licensed under the MIT license: https://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Sheets = Microsoft.Office.Interop.Excel.Sheets;
using Application = Microsoft.Office.Interop.Excel.Application;
using Range = Microsoft.Office.Interop.Excel.Range;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace PrChecker
{
    internal class ExcelTools
    {
        internal static void WriteExcelFile(ref string filePath, string pipeSeparatedValues)
        {
            // Define Excel objects.
            Application excelApp = null;
            Workbooks excelWorkbooks = null;
            Workbook excelWorkbook = null;
            Sheets excelWorksheets = null;
            Worksheet excelWorksheet = null;
            Range excelRange = null;
            Hyperlinks excelHyperlinks = null;

            try
            {
                // Start Excel and get application object.
                excelApp = new Application();
                excelApp.Visible = false;
                excelApp.UserControl = false;
                excelApp.DisplayAlerts = false;
                if (excelApp == null) return;

                // Open Excel workbook.
                excelWorkbooks = excelApp.Workbooks;
                excelWorkbook = excelWorkbooks.Add("");
                excelWorksheets = excelWorkbook.Sheets;
                excelWorksheet = excelWorksheets[1];
                excelHyperlinks = excelWorksheet.Hyperlinks;

                // Add table headers.
                excelRange = excelWorksheet.Range["A1", "K1"];
                excelRange.Font.Bold = true;
                excelRange.Interior.Color = XlRgbColor.rgbLightGrey;
                Marshal.FinalReleaseComObject(excelRange);

                // Get row data.
                string[] rowsData = pipeSeparatedValues.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                int rowNumber = 1;

                //foreach (var rowData in rowsData.Where(x => x.Contains("1881219") && x.Contains("PR created")))
                foreach (string rowData in rowsData)
                {
                    // Get cell data.
                    string[] columnsData = rowData.Split(" ||| ", StringSplitOptions.RemoveEmptyEntries);

                    // Iterate cells in row.
                    for (int i = 0; i < columnsData.Length; i++)
                    {
                        // Separate cell text from cell comment.
                        string[] cellText = columnsData[i].Split("$$");

                        // Get cell reference.
                        Range cell = excelWorksheet.Cells[rowNumber, i + 1];

                        // Add cell text.
                        cell.Value2 = cellText[0].Trim();

                        // Add cell comment.
                        if (cellText.Length > 1 && cellText[1].Trim().Length > 0)
                            cell.AddComment(cellText[1].Trim());

                        // Add cell hyperlink.
                        if (columnsData[i].Trim().StartsWith("https://")) 
                            excelHyperlinks.Add(excelWorksheet.Cells[rowNumber, i + 1], columnsData[i].Trim());

                        // Release Excel cell resources.
                        Marshal.FinalReleaseComObject(cell);
                    }
                    rowNumber++;
                }

                // Apply formatting:
                excelRange = excelWorksheet.Range["A1", "K1"];
                //excelRange.EntireColumn.WrapText = false;
                excelRange.EntireColumn.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                excelRange.EntireColumn.VerticalAlignment = XlVAlign.xlVAlignCenter;
                excelRange.EntireColumn.AutoFit();
                Marshal.FinalReleaseComObject(excelRange);

                filePath = excelApp.GetSaveAsFilename(InitialFilename: filePath, FileFilter: "Excel files (*.xlsx), *.xlsx", FilterIndex: 1, "Select report filename").ToString();
                excelWorkbook.SaveAs(filePath);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Excel error:\r\n{e.Message}");
            }
            finally
            {
                // Quit Excel.
                excelWorkbook.Close();
                excelWorkbooks.Close();
                excelApp.Application.Quit();
                excelApp.Quit();

                // Release all Excel resources.
                Marshal.ReleaseComObject(excelHyperlinks);
                Marshal.ReleaseComObject(excelRange);
                Marshal.FinalReleaseComObject(excelWorksheet);
                Marshal.FinalReleaseComObject(excelWorksheets);
                Marshal.FinalReleaseComObject(excelWorkbook);
                Marshal.FinalReleaseComObject(excelWorkbooks);
                Marshal.FinalReleaseComObject(excelApp);

                // Force garbage collection.
                GC.Collect();
            }
        }
    }
}
