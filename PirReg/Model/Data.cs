using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;


namespace PirReg.Model
{
    public class Data
    {
        private static FileInfo _settingsXmlPath;

        public string SqlConnection { get; set; }

        public string Domicile { get; set; }

        public DateTime FromDate { get; set; }

        public TimeSpan FromTime { get; set; }

        public DateTime ToDate { get; set; }

        public TimeSpan ToTime { get; set; }

        /// <summary>minActivationInRoom must be between 0 og 60 minutes.</summary>
        public int MinActivityInRoom { get; set; }

        public virtual List<RoomData> RoomDatas { get; set; }

        public string ReportFolderPath { get; set; }

        public Data(ref string log)
        {
            RoomDatas = new List<RoomData>();
            log = string.Concat(log, "  Reads settings:\n");
            try
            {
                var doc = XDocument.Load(Data.SettingsXmlPath.FullName);
                ReportFolderPath = GetAttribute<string>(doc, "reportFolder", "folder", ref log);
                MinActivityInRoom = (int)doc.Root.Element("minActivityInRoom").Attribute("minutes");
                switch ((int)doc.Root.Element("from").Attribute("dateGenerate"))
                {
                    case 0: FromDate = new DateTime(DateTime.Now.Year, 1, 1); break;
                    case 1: FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1); break;
                }
                FromTime = TimeSpan.ParseExact((string)doc.Root.Element("from").Attribute("time"), @"h\:m", CultureInfo.InvariantCulture);
                switch ((int)doc.Root.Element("to").Attribute("dateGenerate"))
                {
                    case 0: ToDate = DateTime.Now; break;
                    case 1: ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1); break;
                }
                ToTime = TimeSpan.ParseExact((string)doc.Root.Element("to").Attribute("time"), @"h\:m", CultureInfo.InvariantCulture);


                SqlConnection = GetAttribute<string>(doc, "sqlConnection", "connection", ref log);
                Domicile = GetAttribute<string>(doc, "rooms", "domicile", ref log);
                foreach (var xElement in doc.Root.Element("rooms").Elements("room"))
                {
                    List<RoomData> roomDataList = this.RoomDatas;
                    RoomData roomDatum = new RoomData()
                    {
                        RoomBuilding = GetAttribute<string>(xElement, "building", ref log),
                        RoomLevel = GetAttribute<string>(xElement, "level", ref log),
                        RoomName = GetAttribute<string>(xElement, "name", ref log),
                        UniqueRoomName = this.ParseUniqueRoomName((string)xElement.Attribute("uniqueName"), ref log),
                        RoomArea = GetAttribute<double>(xElement, "area", ref log),
                        RoomFunction = GetAttribute<string>(xElement, "roomFunction", ref log),
                        Note = GetAttribute<string>(xElement, "note", ref log),
                        Sql = GetAttribute<string>(xElement, "sql", ref log)
                    };
                    roomDataList.Add(roomDatum);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                log = string.Concat(log, "    The following errors occurred: ", exception.Message);
                return;
            }
            log += string.Format("    Loaded {0} rooms from settings.\n", this.RoomDatas.Count);
        }

        public static void CreateNewDataXml(FileInfo fileInfo)
        {
            var xDocument = new XDocument(
                new XComment("File created: " + DateTime.Now),
                new XElement("root",
                    new XComment("reportFolder: Where reports are saved"),
                    new XElement("reportFolder", new XAttribute("folder", "c:\\")),
                    new XComment("sqlConnection: connectionstring to sql that returns a table or a view with: Column[0] = dateTime, Column[1] = int, string or bool like '0 or 1' or 'true or false'"),
                    new XElement("sqlConnection", new XAttribute("connection", "Data Source=")),
                    new XComment("minActivityInRoom: Minutes must be between 0 and 60"),
                    new XElement("minActivityInRoom", new XAttribute("minutes", "20")),

                    new XComment("dateGenerate: 0=Current year start. 1=Past month start"),
                    new XElement("from", new XAttribute("dateGenerate", "0"), new XAttribute("time", "08:00")),

                    new XComment("dateGenerate: 0=dags dato. 1=Forgangne måned slut."),
                    new XElement("to", new XAttribute("dateGenerate", "0"), new XAttribute("time", "18:00")),

                    new XElement("rooms",
                        new XAttribute("domicile", ""),
                        new XElement("room",
                            new XAttribute("building", ""),
                            new XAttribute("level", ""),
                            new XAttribute("name", ""),
                            new XAttribute("uniqueName", ""),
                            new XAttribute("area", ""),
                            new XAttribute("roomFunction", ""),
                            new XAttribute("note", ""),
                            new XAttribute("sql", "Select *")
                        )
                    )
            ));

            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            xDocument.Save(fileInfo.FullName);
        }

        public static FileInfo SettingsXmlPath
        {
            get
            {
                if (_settingsXmlPath == null)
                {
                    var pathToBinFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    _settingsXmlPath = new FileInfo(Path.Combine(pathToBinFolder, "Settings.xml"));

                    if (!_settingsXmlPath.Exists)
                        Data.CreateNewDataXml(_settingsXmlPath);
                }
                return _settingsXmlPath;
            }
        }

        private T GetAttribute<T>(XDocument doc, string elementName, string attributeName, ref string log)
        {
            var element = doc.Root.Element(elementName);
            if (element == null)
            {
                log += string.Format("    Error: Element {0} are missing in xml syntax.\n", elementName);
                return default(T);
            }
            return GetAttribute<T>(element, attributeName, ref log);
        }

        private T GetAttribute<T>(XElement element, string attributeName, ref string log)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                log += string.Format("    Error: Element {0} must have attribute {1} but are missing in xml syntax.\n", element.Name, attributeName);
                return default(T);
            }

            var valueAsString = (string)attribute;

            if (typeof(T) == typeof(double))
            {
                double value = 0;
                var success = false;
                success = double.TryParse(valueAsString, out value);

                if (!success)
                    log += string.Format("    Error: Element {0}, attribute {1} must be a double.\n", element.Name, attributeName);

                return (T)Convert.ChangeType(value, typeof(T));
            }
            else
                return (T)Convert.ChangeType(valueAsString, typeof(T));
        }

        private string ParseUniqueRoomName(string text, ref string errorLog)
        {
            if (string.IsNullOrEmpty(text))
                errorLog = string.Concat(errorLog, "    Error: There was a room without a unique name.\n");

            if ((from o in this.RoomDatas where o.UniqueRoomName == text select o).Any<RoomData>())
                errorLog = string.Concat(errorLog, "    Error: There was more than one room with the same unique name; ", text, "\n");

            return text;
        }
    }
}