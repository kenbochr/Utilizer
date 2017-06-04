using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PirReg
{
	public class RoomData
	{
        public string Note { get; set; }
		
		public short OrgId{get;set;}
		
		public double RoomArea{get;set;}
		
		public string RoomBuilding{get;set;}
		
		public int RoomDataId{get;set;}
		
		public List<RoomDataRaw> RoomDataRaw{get;set;}
		
		public List<RoomDataRawCondensed> RoomDataRawCondensed{get;set;}
		
		public ICollection<RoomDataRawFinal> RoomDataRawFinal{get;set;}
		
		public string RoomFunction{get;set;}
		
		public string RoomLevel{get;set;}
		
		public string RoomName{get;set;}
		
		public string Sql{get;set;}
		
		public string UniqueRoomName{get;set;}

		public RoomData()
		{
			this.RoomDataRaw = new List<RoomDataRaw>();
			this.RoomDataRawCondensed = new List<RoomDataRawCondensed>();
			this.RoomDataRawFinal = new HashSet<RoomDataRawFinal>();
		}
	}
}