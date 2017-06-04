using PirReg.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace PirReg.Helpers
{
    public class ImportDataFromDB
    {
        private SqlConnection cnt = null;

        public ImportDataFromDB(Data data)
        {
            cnt = new SqlConnection(data.SqlConnection);
            cnt.Open();

            var roomDataRaws = new List<RoomDataRaw>();
            var antalRum = data.RoomDatas.Count<RoomData>();
            Console.WriteLine(string.Format("  Starts importing data for {0} rooms:", antalRum));

            for (int i = 0; i < antalRum; i++)
            {
                Console.Write(string.Format("\r    Imports raw date {0} %", (i * 100) / antalRum));

                if (data.RoomDatas[i].Sql != null)
                    roomDataRaws = MakeDbRequest(data.RoomDatas[i].Sql, data.FromDate, data.ToDate);
                else
                    roomDataRaws = null;

                if (roomDataRaws != null)
                    data.RoomDatas[i].RoomDataRaw = roomDataRaws;
            }

            Console.Write("\r    Imports raw data 100 %");
        }

        private List<RoomDataRaw> MakeDbRequest(string sql, DateTime importFromDate, DateTime importToDate)
        {
            var result = new List<RoomDataRaw>();
            try
            {
                var sqlCommand = new SqlCommand(sql, cnt);
                var sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    var dateTime = DateTime.Parse(sqlDataReader[0].ToString());
                    if (!(dateTime > importFromDate) || !(dateTime < importToDate))
                        continue;

                    result.Add(new RoomDataRaw(Convert.ToBoolean(sqlDataReader[1]), dateTime));
                }
                sqlDataReader.Close();

                return result.OrderBy(o => o.Time).ToList();
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("    An error occoured while reading the xml-file that contains information about sql connection.\n\nThe error ocoured while handling the sql: {0}\n\nThe errormessage: {1}\n\n", sql, exception.Message));
                return null;
            }
        }
    }
}