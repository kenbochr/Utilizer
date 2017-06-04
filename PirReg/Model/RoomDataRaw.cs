using System;

namespace PirReg.Model
{
    public class RoomDataRaw
    {
        public RoomDataRaw(bool state, DateTime time)
        {
            State = state;
            Time = time;
        }

        public bool State { get; set; }

        public DateTime Time { get; set; }
    }
}