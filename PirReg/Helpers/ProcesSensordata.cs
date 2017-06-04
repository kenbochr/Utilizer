using PirReg.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PirReg.Helpers
{
    public class ProcesSensordata
    {
        public static void ProcesRawData(Data data)
        {
            Console.WriteLine();
            Console.WriteLine("    Processing raw data");
            DateTime datoStartReg;
            int pos;

            foreach (var item in data.RoomDatas)
            {
                for (int i = 0; i < item.RoomDataRaw.Count(); i++)
                {
                    if (item.RoomDataRaw[i].State)
                    {
                        datoStartReg = item.RoomDataRaw[i].Time;  //Move forward until I find a sensor that matches this and where DataValue=false
                        pos = 0;

                        for (int ii = i; ii < item.RoomDataRaw.Count(); ii++)
                        {
                            pos++;
                            if (!item.RoomDataRaw[ii].State)
                            {
                                i = i + pos - 1;
                                item.RoomDataRawCondensed.Add(new RoomDataRawCondensed(datoStartReg,item.RoomDataRaw[ii].Time));
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void ProcesCondensedRawData(Data data)
        {
            data.MinActivityInRoom = data.MinActivityInRoom > 60 ? 60 : data.MinActivityInRoom;  //must be between 0 og 60 minutter
            var roomDataOrderedList = data.RoomDatas.OrderBy(o => o.UniqueRoomName).ToList();
            var roomDataCount = data.RoomDatas.Count();
            for (int i = 0; i < roomDataCount; i++)
            {
                Console.Write(string.Format("\r    Condensates data {0} %", i * 100 / roomDataCount));

                //I gather data together in utilizerRoomDataList. For example, there may be more 13/5 - 07 = 10, 13/5 - 07 = 12. These should be collected and then transferred to roomList.
                var utilizerRoomDataList = new List<RoomDataRawFinal>();
                foreach (var item in roomDataOrderedList[i].RoomDataRawCondensed.OrderBy(o => o.Start))
                {
                    var dateHoleHourStartReg = new DateTime(item.Start.Year, item.Start.Month, item.Start.Day, item.Start.Hour, 0, 0);
                    int hours = Convert.ToInt32(Math.Floor(item.End.Subtract(dateHoleHourStartReg).TotalHours));
                    if (hours == 0)
                    {
                        var minutes = Convert.ToInt16(item.End.Subtract(item.Start).TotalMinutes);
                        if (minutes >= data.MinActivityInRoom)
                            utilizerRoomDataList.Add(new RoomDataRawFinal(dateHoleHourStartReg,minutes));
                    }
                    else
                    {
                        for (int ii = 0; ii <= hours; ii++)
                        {
                            if (ii == 0)
                            {
                                var minutes = Convert.ToInt16(dateHoleHourStartReg.AddHours(1).Subtract(item.Start).TotalMinutes);
                                utilizerRoomDataList.Add(new RoomDataRawFinal(dateHoleHourStartReg,minutes));
                            }
                            else if (ii == hours)
                            {
                                dateHoleHourStartReg = dateHoleHourStartReg.AddHours(1);
                                var minutes = Convert.ToInt16(item.End.Subtract(dateHoleHourStartReg).TotalMinutes);
                                utilizerRoomDataList.Add(new RoomDataRawFinal(dateHoleHourStartReg, minutes));
                            }
                            else
                            {
                                var minutes = 60;
                                dateHoleHourStartReg = dateHoleHourStartReg.AddHours(1);
                                utilizerRoomDataList.Add(new RoomDataRawFinal(dateHoleHourStartReg,minutes));
                            }
                        }
                    }
                }

                // Everything is gathered. Example: 13/5 at 7 = 10 13/5 at 7 = 12 turns to 13/5 at 7 = 22.
                foreach (var item3 in utilizerRoomDataList.GroupBy(o => o.Time))
                {
                    var time = item3.FirstOrDefault().Time;
                    var minutes = Convert.ToInt16(utilizerRoomDataList.Where(o => o.Time == time).Sum(o => o.Minutes));
                    roomDataOrderedList[i].RoomDataRawFinal.Add(new RoomDataRawFinal(time, minutes));
                }
            }
            Console.Write("\r    Condensates data 100 %");
        }
    }
}