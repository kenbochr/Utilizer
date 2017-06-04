using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using PirReg.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace PirReg.Helpers
{
    public class ExportToExcel
	{
        private Data data;

        public ExportToExcel(Data data)
        {
            this.data = data;
            var fileInfo = new FileInfo(Path.Combine(data.ReportFolderPath, string.Concat(DateTime.Now.ToString().Replace(':', '.'), " ", data.Domicile, ".xlsx")));

            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            if (fileInfo.Exists)
                fileInfo.Delete();

            using (var excelPackage = new ExcelPackage(fileInfo))
            {
                var roomDataToExcels = new List<DataToExcel>();
                var dateTimes = EachDayTime(data.FromDate, data.ToDate, data.FromTime, data.ToTime);
                var num = data.RoomDatas.Count();
                for (int i = 0; i < num; i++)
                {
                    Console.Write(string.Format("\r    Data loaded: {0} %", (i * 100) / num));
                    foreach (var dateTime in dateTimes)
                    {
                        var ugedag = dateTime.DayOfWeek == 0 ? 7 : (int)dateTime.DayOfWeek;
                        var roomDataRawFinal = data.RoomDatas[i].RoomDataRawFinal.FirstOrDefault(o => o.Time == dateTime);

                        roomDataToExcels.Add(new DataToExcel()
                        {
                            Domicile = data.Domicile,
                            RoomBuilding = data.RoomDatas[i].RoomBuilding,
                            RoomLevel = data.RoomDatas[i].RoomLevel,
                            RoomName = data.RoomDatas[i].RoomName,
                            UniqueRoomName = data.RoomDatas[i].UniqueRoomName,
                            RoomArea = data.RoomDatas[i].RoomArea,
                            RoomFunction = data.RoomDatas[i].RoomFunction,
                            Note = data.RoomDatas[i].Note,
                            Date = dateTime.ToShortDateString(),
                            Year = dateTime.Year,
                            Month = dateTime.ToString("MMM"),
                            Week = GetWeekOffYear(dateTime),
                            WeekDay = string.Concat(ugedag, " ", dateTime.DayOfWeek),
                            Time = dateTime.ToShortTimeString(),
                            Percent = (roomDataRawFinal != null ? (Convert.ToDecimal(roomDataRawFinal.Minutes) / new decimal(60)) * new decimal(100) : new decimal(0))
                        });
                    }
                }
                Console.Write("\r    Data loaded: 100 %");
                if (!roomDataToExcels.Any())
                    Console.WriteLine("\n    There were no data to export.");
                else if (roomDataToExcels.Count() <= 1048576)
                {
                    Console.WriteLine("\n    There are used " + roomDataToExcels.Count<DataToExcel>() * 100 / 1048576 + " % of the spreadsheet capacity.");
                    var excelRangeBase = AddRawSheet(excelPackage, "RawData", roomDataToExcels);
                    PivotPerDay(excelPackage, "DataPerDay", excelRangeBase);
                    PivotPerWeekDay(excelPackage, "DataPerWeekDay", excelRangeBase);
                    PivotPerWeek(excelPackage, "DataPerWeek", excelRangeBase);
                    excelPackage.Workbook.Properties.Title = "Udnyttelsesgrader";
                    //excelPackage.Workbook.Properties.Author = "Udviklet af PH Metropol Bygninger: keje@phmetropol.dk";
                    Console.WriteLine("    Saves spreadsheet to a file...");
                    excelPackage.Save();
                }
                else
                    Console.WriteLine(string.Format("There was {0}% more data exported than Excel could accommodate.", roomDataToExcels.Count<DataToExcel>() * 100 / 1048576 - 100));

                //System.Diagnostics.Process.Start(filSti);
            }
        }

		private void PivotPerDay(ExcelPackage package, string workSheetName, ExcelRangeBase dataRange)
		{
			var excelWorksheet = package.Workbook.Worksheets.Add(workSheetName);
			excelWorksheet.Cells["A1"].Value = string.Format("Data per day based on data from {0} to {1} in the timespan from {2} to {3}. Min limit of use before registering: {4} minutes", 
                data.FromDate.ToShortDateString(), data.ToDate.ToShortDateString(), data.FromTime.ToString(), data.ToTime.ToString(), data.MinActivityInRoom);
			excelWorksheet.Cells["A1"].Style.Font.Bold = true;
			var excelPivotTable = excelWorksheet.PivotTables.Add(excelWorksheet.Cells["A5"], dataRange, "DataPerDay");
            excelPivotTable.RowFields.Add(excelPivotTable.Fields["Time"]).Sort = eSortType.Ascending;
			excelPivotTable.ColumnFields.Add(excelPivotTable.Fields["UniqueRoomName"]).Sort = eSortType.Ascending;
            excelPivotTable.PageFields.Add(excelPivotTable.Fields["RoomFunction"]);
            excelPivotTable.PageFields.Add(excelPivotTable.Fields["Date"]);
			var excelPivotTableDataField = excelPivotTable.DataFields.Add(excelPivotTable.Fields["Percent"]);
            excelPivotTableDataField.Format = "_ * #0.0_ ;_ * -#0.0_ ;_ * \"  \"??_ ;_ @_ ";
			excelPivotTableDataField.Function = DataFieldFunctions.Average;
            
            int hours = (data.ToTime - data.FromTime).Hours;
            int num = data.RoomDatas.Select(o=> o.UniqueRoomName).Count();
			string address = excelWorksheet.Cells[7, 2, 8 + hours, 2 + num].Address;
			AddConditionalFormatting(excelWorksheet, address, 0, 6, true);
		}

        private void PivotPerWeekDay(ExcelPackage package, string workSheetName, ExcelRangeBase dataRange)
        {
            var excelWorksheet = package.Workbook.Worksheets.Add(workSheetName);
            excelWorksheet.Cells["A1"].Value = string.Format("Data per weekday based on data from {0} to {1} in the timespan from {2} to {3}. Min limit of use before registering: {4} minutes",
                data.FromDate.ToShortDateString(), data.ToDate.ToShortDateString(), data.FromTime.ToString(), data.ToTime.ToString(), data.MinActivityInRoom);
            excelWorksheet.Cells["A1"].Style.Font.Bold = true;

            var excelPivotTable = excelWorksheet.PivotTables.Add(excelWorksheet.Cells["A5"], dataRange, "DataPerWeekDay");
            excelPivotTable.RowFields.Add(excelPivotTable.Fields["Time"]).Sort = eSortType.Ascending;
            excelPivotTable.ColumnFields.Add(excelPivotTable.Fields["WeekDay"]).Sort = eSortType.Ascending;
            excelPivotTable.PageFields.Add(excelPivotTable.Fields["RoomFunction"]);
            excelPivotTable.PageFields.Add(excelPivotTable.Fields["Date"]);
            var excelPivotTableDataField = excelPivotTable.DataFields.Add(excelPivotTable.Fields["Percent"]);
            excelPivotTableDataField.Format = "_ * #0.0_ ;_ * -#0.0_ ;_ * \"  \"??_ ;_ @_ ";
            excelPivotTableDataField.Function = DataFieldFunctions.Average;

            TimeSpan exportToThisTime = data.ToTime - data.FromTime;
            int hours = exportToThisTime.Hours;
            int num = 8;
            string address = excelWorksheet.Cells[7, 2, 8 + hours, 2 + num].Address;
            AddConditionalFormatting(excelWorksheet, address, 0, 6, true);
        }

        private void PivotPerWeek(ExcelPackage package, string workSheetName, ExcelRangeBase dataRange)
		{
            var excelWorksheet = package.Workbook.Worksheets.Add(workSheetName);
            excelWorksheet.Cells["A1"].Value = string.Format("Data per week based on data from {0} to {1} in the timespan from {2} to {3}. Min limit of use before registering: {4} minutes", 
                data.FromDate.ToShortDateString(), data.ToDate.ToShortDateString(), data.FromTime.ToString(), data.ToTime.ToString(), data.MinActivityInRoom);
            excelWorksheet.Cells["A1"].Style.Font.Bold = true;

            var excelPivotTable = excelWorksheet.PivotTables.Add(excelWorksheet.Cells["A5"], dataRange, "DataPerWeek");
            excelPivotTable.RowFields.Add(excelPivotTable.Fields["UniqueRoomName"]).Sort = eSortType.Ascending;
            excelPivotTable.ColumnFields.Add(excelPivotTable.Fields["Week"]).Sort = eSortType.Ascending;
            excelPivotTable.PageFields.Add(excelPivotTable.Fields["RoomFunction"]);
            excelPivotTable.PageFields.Add(excelPivotTable.Fields["Date"]);
            var excelPivotTableDataField = excelPivotTable.DataFields.Add(excelPivotTable.Fields["Percent"]);
            excelPivotTableDataField.Format = "_ * #0.0_ ;_ * -#0.0_ ;_ * \"  \"??_ ;_ @_ ";
            excelPivotTableDataField.Function = DataFieldFunctions.Average;
            
            int num = data.RoomDatas.Select(o => o.UniqueRoomName).Count();
            TimeSpan exportToThisDate = data.ToDate - data.FromDate;
            int num1 = Convert.ToInt32(Math.Ceiling(exportToThisDate.TotalDays / 7));
            string address = excelWorksheet.Cells[7, 2, 8 + num, 2 + num1].Address;
            AddConditionalFormatting(excelWorksheet, address, 0, 6, true);
		}

        private ExcelRangeBase AddRawSheet(ExcelPackage package, string workSheetName, List<DataToExcel> roomDataList)
		{
            var excelWorksheet = package.Workbook.Worksheets.Add(workSheetName);
            excelWorksheet.Cells["A1"].Value = string.Format("Rooms utilization from {0} to {1} in the timespan from {2} to {3}",
                data.FromDate.ToShortDateString(), data.ToDate.ToShortDateString(), data.FromTime.ToString(), data.ToTime.ToString());
            excelWorksheet.Cells["A1"].Style.Font.Bold = true;

			ExcelRangeBase excelRangeBase = excelWorksheet.Cells["A2"].LoadFromCollection<DataToExcel>(roomDataList, true, OfficeOpenXml.Table.TableStyles.Light20);  //Det ver nr 24
			excelWorksheet.Cells[1, 9, excelRangeBase.End.Row, 9].Style.Numberformat.Format = "mm-dd-yy";
			excelWorksheet.Cells[1, 14, excelRangeBase.End.Row, 14].Style.Numberformat.Format = "hh:mm";
			excelWorksheet.Cells[1, 15, excelRangeBase.End.Row, 15].Style.Numberformat.Format = "#,##0.00";
			return excelRangeBase;
		}

		private IEnumerable<DateTime> EachDayTime(DateTime fromDate, DateTime toDate, TimeSpan fromTime, TimeSpan toTime)
		{
			List<DateTime> dateTimes = new List<DateTime>();
			for (DateTime i = fromDate.Date; i.Date <= toDate.Date; i = i.AddDays(1))
				for (double j = fromTime.TotalHours; j <= toTime.TotalHours; j = j + 1)
					dateTimes.Add(i.AddHours(j));

			return dateTimes;
		}

		private int GetWeekOffYear(DateTime date)
		{
			DateTimeFormatInfo currentInfo = DateTimeFormatInfo.CurrentInfo;
			Calendar calendar = currentInfo.Calendar;
			return calendar.GetWeekOfYear(date, currentInfo.CalendarWeekRule, currentInfo.FirstDayOfWeek);
		}

        /// <summary>
        /// Add an attribute to a XML Element.
        /// </summary>
        /// <param name="inXmlElement">XML Element to add the attribute to</param>
        /// <param name="inAttributeName">Attribute name</param>
        /// <param name="inAttributeValue">Attribute value</param>
        private void AddAttribute(XmlElement inXmlElement, string inAttributeName, string inAttributeValue)
        {
            XmlAttribute xmlAttribute = inXmlElement.OwnerDocument.CreateAttribute(inAttributeName);
            xmlAttribute.Value = inAttributeValue;
            inXmlElement.SetAttributeNode(xmlAttribute);
        }

        /// <summary>
        /// Add Conditional Formatting to an EPPlus Worksheet, using the "colorScale" formatting option.
        /// </summary>
        /// <param name="inWorksheet">EPPlus Worksheet object</param>
        /// <param name="inSqref">Range of cells to format</param>
        /// <param name="inColor1">From Color</param>
        /// <param name="inColor2">To Color</param>
        private void AddConditionalFormatting(ExcelWorksheet inWorksheet, string inSqref, int inColor1, int inColor2, bool isPivot = false)
        {
            var worksheetXml = inWorksheet.WorksheetXml;
            var documentElement = worksheetXml.DocumentElement;
            var xmlElement = worksheetXml.CreateElement("conditionalFormatting", documentElement.NamespaceURI);
            AddAttribute(xmlElement, "pivot", "1");
            if (isPivot)
                AddAttribute(xmlElement, "sqref", inSqref);
            
            XmlElement xmlElement1 = worksheetXml.CreateElement("cfRule", documentElement.NamespaceURI);
            AddAttribute(xmlElement1, "type", "colorScale");
            AddAttribute(xmlElement1, "priority", "1");
            var xmlElement2 = worksheetXml.CreateElement("colorScale", documentElement.NamespaceURI);
            var xmlElement3 = worksheetXml.CreateElement("cfvo", documentElement.NamespaceURI);
            AddAttribute(xmlElement3, "type", "min");
            xmlElement2.AppendChild(xmlElement3);
            xmlElement3 = worksheetXml.CreateElement("cfvo", documentElement.NamespaceURI);
            AddAttribute(xmlElement3, "type", "max");
            xmlElement2.AppendChild(xmlElement3);
            xmlElement3 = worksheetXml.CreateElement("color", documentElement.NamespaceURI);
            AddAttribute(xmlElement3, "theme", inColor1.ToString());
            xmlElement2.AppendChild(xmlElement3);
            xmlElement3 = worksheetXml.CreateElement("color", documentElement.NamespaceURI);
            AddAttribute(xmlElement3, "theme", inColor2.ToString());
            xmlElement2.AppendChild(xmlElement3);
            xmlElement1.AppendChild(xmlElement2);
            xmlElement.AppendChild(xmlElement1);
            documentElement.InsertAfter(xmlElement, documentElement.LastChild);
        }
	}
}