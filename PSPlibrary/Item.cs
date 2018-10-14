using System;
using System.Windows.Media;

namespace PSPlibrary
{
    public class Item
    {
        public string DiscID { get; set; }
        public string Title { get; set; }
        public string Filename { get; set; }
        public string Region { get; set; }
        public string Firmware { get; set; }
        public Int32 Index { get; set; }
        public ItemData Detail { get; set; }

        #region Game PSP Get Item Method.
        public static String GetType(String id)
        {
            String Type = "Network Retail";
            switch (id.Substring(0, 1))
            {
                case "B": Type = "Blu-ray"; break;
                case "S": Type = "CD/DVD"; break;
                case "U": Type = "UMD"; break;
            }
            return Type;
        }
        public static String GetRegion(String id)
        {
            String Language = "N/A";
            switch (id.Substring(2, 1))
            {
                case "A": Language = "Asia"; break;
                case "E": Language = "Europe"; break;
                case "H": Language = "Southeast Asia"; break;
                case "K": Language = "Hong Kong"; break;
                case "I": Language = "Internal (Sony)"; break;
                case "J": Language = "Japan"; break;
                case "U": Language = "USA"; break;
                case "X": Language = "Firmware/SDK Sample"; break;
            }
            return Language;
        }
        #endregion
    }

    public class ItemData
    {
        public String Name;
        public String Icon;
        public String Background;
        public String Title;
        public String Version;
        public String Parental;
        public String Catagory;
        public String Type;
        public Int64 Size;
        public String Created;
        public String Release;
        public String Player;
        public String Genre;
        public String Developer;
        public String Publisher;
        public String Description;
        public String Snapshot1;
        public String Snapshot2;
        public String Snapshot3;
        public String Snapshot4;
        public String Snapshot5;
        public String Snapshot6;

        public String GameDirectory;
    }
}
