using System;

namespace PirReg.Model
{
    public class RoomDataRawFinal
    {
        public RoomDataRawFinal(DateTime time, int minutes)
        {
            Time = time;
            Minutes = minutes;
        }

        public int Minutes { get; set; }

        public RoomData RoomData { get; set; }

        public long RoomDataRawFinalId { get; set; }

        public DateTime Time { get; set; }
    }
}