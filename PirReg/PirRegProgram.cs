using PirReg.Helpers;
using PirReg.Model;
using System;
using System.Linq;

namespace PirReg
{
    public class PirRegProgram
    {
        private Data data;

        public void StartAnalysis()
        {
            var error = string.Empty;
            data = new Data(ref error);
            Console.WriteLine(error);

            Func<RoomDataRawFinal, DateTime> time = null;
            Func<RoomDataRawFinal, DateTime> func = null;
            if (data == null)
            {
                Console.WriteLine("    There was an error loading.\n");
                return;
            }
            if (string.IsNullOrEmpty(data.SqlConnection))
            {
                Console.WriteLine("    You havent filled out the attribute 'string' in the element 'sqlconnection' in the SqlConn.xml. The data cannot be processed.\n");
                return;
            }
            DateTime? selectedDate = DateTime.Now.AddDays(-7);
            DateTime? nullable = DateTime.Now;
            if ((selectedDate.HasValue & nullable.HasValue ? selectedDate.GetValueOrDefault() > nullable.GetValueOrDefault() : false))
            {
                Console.WriteLine("    The value of Export from, has to be smaller than the value of Export to\n");
                return;
            }
            if (data.FromDate < data.ToDate.AddYears(-1))
            {
                Console.WriteLine("    There can not be selected a timespan of more than a year.\n");
                return;
            }

            new ImportDataFromDB(data);
            bool flag = false;
            foreach (RoomData roomDataList in data.RoomDatas)
            {
                if (!roomDataList.RoomDataRaw.Any())
                    continue;

                flag = true;
                break;
            }
            if (!flag)
            {
                Console.WriteLine("There were no data to import. Done exporting data.\n");
                return;
            }

            ProcesSensordata.ProcesRawData(data);
            ProcesSensordata.ProcesCondensedRawData(data);
            Console.WriteLine("\n    Import of data completed.");
            DateTime minValue = DateTime.MinValue;
            DateTime maxValue = DateTime.MaxValue;
            foreach (RoomData roomDatum in data.RoomDatas)
            {
                if (!roomDatum.RoomDataRawFinal.Any())
                    continue;

                time = time == null ? (RoomDataRawFinal o) => o.Time : time;
                var dateTime = roomDatum.RoomDataRawFinal.Min(time);
                func = func == null ? (RoomDataRawFinal o) => o.Time : func;

                if (dateTime < maxValue)
                    maxValue = dateTime;

                var dateTime1 = roomDatum.RoomDataRawFinal.Max(func);
                if (dateTime1 <= minValue)
                    continue;

                minValue = dateTime1;
            }

            Console.WriteLine("\n  Starting report creation:");

            maxValue = new DateTime(maxValue.Year, maxValue.Month, maxValue.Day);
            minValue = (new DateTime(minValue.Year, minValue.Month, minValue.Day)).AddDays(1);
            if (maxValue > DateTime.MaxValue.AddDays(-1) || minValue < DateTime.MinValue.AddDays(1))
            {
                Console.WriteLine("There were no data.\n");
                return;
            }
            if (data.FromDate < maxValue)
            {
                Console.WriteLine(string.Format("    From date {0}, are changed to {1}, because there are no earlier data.", data.FromDate.ToString("d"), maxValue.ToString("d")));
                data.FromDate = maxValue;
            }
            if (data.ToDate > minValue)
            {
                Console.WriteLine(string.Format("    To date {0}, are changed to {1}, because there are no newer data.", data.ToDate.ToString("d"), minValue.ToString("d")));
                data.ToDate = minValue;
            }
            new ExportToExcel(data);
            Console.WriteLine("    Completed report building.");
        }
    }
}
