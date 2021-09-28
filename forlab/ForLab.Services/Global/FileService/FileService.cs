using ForLab.Core.Common;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using System;
using System.Collections.Generic;
using System.IO;

namespace ForLab.Services.Global.FileService
{
    public class FileService<T> : IFileService<T>
    {
        public GeneratedFile ExportToExcel(List<T> arg)
        {
            byte[] fileContents;

            using (var fileStream = new MemoryStream())
            {
                IWorkbook workbook;
                workbook = new SXSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Data");

                IRow head = excelSheet.CreateRow(0);

                // Style of the head
                var headFont = workbook.CreateFont();
                headFont.FontHeightInPoints = 11;
                headFont.FontName = "Calibri";
                headFont.Boldweight = (short)FontBoldWeight.Bold;

                var headStyle = workbook.CreateCellStyle();
                headStyle.VerticalAlignment = VerticalAlignment.Center;
                headStyle.Alignment = HorizontalAlignment.Center;
                headStyle.SetFont(headFont);

                var dataStyle = workbook.CreateCellStyle();
                dataStyle.VerticalAlignment = VerticalAlignment.Center;
                dataStyle.Alignment = HorizontalAlignment.Center;


                int i = 0;
                int length = arg.Count;

                while (i < length)
                {
                    var row = excelSheet.CreateRow(i + 1);
                    var item = arg[0];

                    // Write the head of the file
                    if (i == 0)
                    {
                        for (int j = 0; j < item.GetType().GetProperties().Length; j++)
                        {
                            var key = item.GetType().GetProperties()[j].Name;
                            var headCell = head.CreateCell(j);
                            headCell.CellStyle = headStyle;
                            headCell.SetCellValue(key.ToString());

                            var val = item.GetType().GetProperties()[j].GetValue(item, null);
                            if (val == null)
                            {
                                val = "";
                            }
                            var dataCell = row.CreateCell(j);
                            dataCell.CellStyle = dataStyle;

                            if (val.ToString() == "True" || val.ToString() == "False")
                            {
                                if (val.ToString() == "True")
                                    dataCell.SetCellValue("Yes");
                                else
                                    dataCell.SetCellValue("No");
                            }
                            else if (IsDateTime(val.ToString()))
                            {
                                var date = Convert.ToDateTime(val.ToString());
                                dataCell.SetCellValue(date.ToString("dd MMM yyy"));
                            }
                            else
                            {
                                dataCell.SetCellValue(val.ToString());
                            }

                            //excelSheet.AutoSizeColumn(j);
                        }

                    }
                    else
                    {
                        for (int j = 0; j < item.GetType().GetProperties().Length; j++)
                        {
                            // test
                            var type = item.GetType();

                            var val = item.GetType().GetProperties()[j].GetValue(item, null);
                            if (val == null)
                            {
                                val = "";
                            }
                            var dataCell = row.CreateCell(j);
                            dataCell.CellStyle = dataStyle;

                            if (val.ToString() == "True" || val.ToString() == "False")
                            {
                                if (val.ToString() == "True")
                                    dataCell.SetCellValue("Yes");
                                else
                                    dataCell.SetCellValue("No");
                            }
                            else if (IsDateTime(val.ToString()))
                            {
                                var date = Convert.ToDateTime(val.ToString());
                                dataCell.SetCellValue(date.ToString("dd MMM yyy"));
                            }
                            else
                            {
                                dataCell.SetCellValue(val.ToString());
                            }

                            //excelSheet.AutoSizeColumn(j);
                        }
                    }

                    ++i;
                    arg.Remove(item);
                }

                workbook.Write(fileStream);
                fileContents = fileStream.ToArray();
            }

            var returnedFile = new GeneratedFile();
            returnedFile.Content = fileContents;
            returnedFile.Name = "ExportedData";
            returnedFile.Extension = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return returnedFile;
        }

        public static bool IsDateTime(string text)
        {
            DateTime dateTime;

            // Check for empty string.
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            return DateTime.TryParse(text, out dateTime);
        }
    }
}
