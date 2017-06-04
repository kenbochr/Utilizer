using System;
using System.Runtime.CompilerServices;

namespace PirReg.Model
{
    public class DataToExcel
    {
        public string Domicile { get; set; }
        
        public string RoomBuilding { get; set; }
        
        public string RoomLevel { get; set; }
        
        public string RoomName { get; set; }
        
        public string UniqueRoomName { get; set; }
        
        public double RoomArea { get; set; }
        
        public string RoomFunction { get; set; }
        
        public string Note { get; set; }

        public string Date { get; set; }

        public int Year { get; set; }

        public string Month { get; set; }
        
        public int Week { get; set; }

        public string WeekDay { get; set; }
        
        public string Time { get; set; }

        public decimal Percent { get; set; }
    }
}