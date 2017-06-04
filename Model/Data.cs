using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace PirReg
{
    public class Data
    {
        public string ConnectionString { get; set; }

        public string Domicile { get; set; }

        public DateTime ExportFromThisDate { get; set; }

        public TimeSpan ExportFromThisTime { get; set; }

        public DateTime ExportToThisDate { get; set; }

        public TimeSpan ExportToThisTime { get; set; }

        public int Minactivationtimeinrooms { get; set; }

        public virtual List<RoomData> RoomDataList { get; set; }

        public string SaveToFilePath { get; set; }

        public Data(ref string errorLog)
        {
            XDocument xDocument;
            this.RoomDataList = new List<RoomData>();
            errorLog = string.Concat(errorLog, "Loading settings...\n");
            try
            {
                xDocument = XDocument.Load(Data.DataXmlFilePath());
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                errorLog = string.Concat(errorLog, "Error ocured on loading settings. Error: ", exception.Message);
                return;
            }
            this.SaveToFilePath = this.ParseRequiredString((string)xDocument.Root.Element("savefile").Attribute("folderpath"), ref errorLog, "savefile");
            XElement xElement = xDocument.Root.Element("roomutilization");
            this.Minactivationtimeinrooms = Convert.ToInt16((string)xElement.Element("minactivationtimeinrooms").Attribute("minutes"));
            this.ConnectionString = this.ParseRequiredString((string)xElement.Element("sqlconnection").Attribute("string"), ref errorLog, "sqlconnection");
            this.Domicile = (string)xElement.Element("rooms").Attribute("domicile");
            foreach (XElement xElement1 in xElement.Element("rooms").Elements("room"))
            {
                List<RoomData> roomDataList = this.RoomDataList;
                RoomData roomDatum = new RoomData()
                {
                    RoomBuilding = (string)xElement1.Attribute("roombuilding"),
                    RoomLevel = (string)xElement1.Attribute("roomlevel"),
                    RoomName = (string)xElement1.Attribute("roomname"),
                    UniqueRoomName = this.ParseUniqueRoomName((string)xElement1.Attribute("uniqueroomname"), ref errorLog),
                    RoomArea = this.ParseDouble((string)xElement1.Attribute("roomarea"), ref errorLog),
                    RoomFunction = (string)xElement1.Attribute("roomfunction"),
                    Note = (string)xElement1.Attribute("note"),
                    Sql = (string)xElement1.Attribute("sql")
                };
                roomDataList.Add(roomDatum);
            }
            errorLog = string.Concat(errorLog, string.Format("Loaded {0} rooms from settings.\n", this.RoomDataList.Count));
        }

        public static void CreateNewDataXml(FileInfo fileInfo)
        {
            object[] xComment = new object[] { new XComment(string.Concat("created: ", DateTime.Now)), null };
            XName xName = "root";
            object[] xElement = new object[] { new XElement("savefile", new XAttribute("folderpath", "c:\\")), null };
            XName xName1 = "roomutilization";
            object[] objArray = new object[] { new XComment("The sql returns a table where column[0] = dateTime and column[1] = int, string or bool that is '0 or 1' or 'true or false'."), new XElement("sqlconnection", new XAttribute("string", "Data Source=")), new XComment("minactivationtimeinrooms: minutes must be inbetween 0 and 60"), new XElement("minactivationtimeinrooms", new XAttribute("minutes", "10")), null };
            XName xName2 = "rooms";
            object[] xAttribute = new object[] { new XAttribute("domicile", ""), new XComment("The attribute; uniqueroomname is required"), null };
            XName xName3 = "room";
            object[] xAttribute1 = new object[] { new XAttribute("roombuilding", ""), new XAttribute("roomlevel", ""), new XAttribute("roomname", ""), new XAttribute("uniqueroomname", ""), new XAttribute("roomarea", ""), new XAttribute("roomfunction", ""), new XAttribute("note", ""), new XAttribute("sql", "Select *") };
            xAttribute[2] = new XElement(xName3, xAttribute1);
            objArray[4] = new XElement(xName2, xAttribute);
            xElement[1] = new XElement(xName1, objArray);
            xComment[1] = new XElement(xName, xElement);
            XDocument xDocument = new XDocument(xComment);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
            
            xDocument.Save(fileInfo.FullName);
        }

        public static string DataXmlFilePath()
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Utilizer", "Data.xml"));
            if (!fileInfo.Exists)
                Data.CreateNewDataXml(fileInfo);
            
            return fileInfo.FullName;
        }

        private double ParseDouble(string text, ref string errorLog)
        {
            double num = 0;
            if (!double.TryParse(text, out num))
                errorLog = string.Concat(errorLog, string.Format("Error while parsing the area: {0}.\n", text));
            
            return num;
        }

        private string ParseRequiredString(string text, ref string errorLog, string element)
        {
            if (string.IsNullOrEmpty(text))
                errorLog = string.Concat(errorLog, string.Format("Error: the element {0} is missing in the xml syntax. It has to be present.\n", element));

            return text;
        }

        private string ParseUniqueRoomName(string text, ref string errorLog)
        {
            if (string.IsNullOrEmpty(text))
                errorLog = string.Concat(errorLog, "Error: There was a room without UniqueRoomName, which has to be present.\n");

            if ((from o in this.RoomDataList where o.UniqueRoomName == text select o).Any<RoomData>())
                errorLog = string.Concat(errorLog, "Error: There where more than one room that has the UniqueRoomName; ", text, "\n");

            return text;
        }
    }
}