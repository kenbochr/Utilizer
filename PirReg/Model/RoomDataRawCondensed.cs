using System;

namespace PirReg.Model
{
    public class RoomDataRawCondensed
    {
        public RoomDataRawCondensed( DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}