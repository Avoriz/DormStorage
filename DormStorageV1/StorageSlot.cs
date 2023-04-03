using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DormStorageV1
{
    public class StorageSlot
    {
        public XmlDocument Stall;
        public string Id { get; set; }
        public string Owner { get; set; }
        public string Room { get; set; }
        public string ItemTotal { get; set; }
        public string PaidStatus { get; set; }
        public List<string> Items { get; set; }
        public Dictionary<string, string> StorageDetails { get; set; }
        public string FilePath { get; private set; }

        public StorageSlot(string storageRoot, string filepath) 
        {
            LoadStorageSlot(storageRoot, filepath);
        }

        public void LoadStorageSlot(string storageRoot, string filepath)
        {
            string file = Path.Combine(storageRoot, filepath);
            if (!File.Exists(file))
            {
                Helpers.PublishWarning("No file was found.", ConsoleColor.Red);
            }
            Stall = new XmlDocument();
            Items = new List<string>();
            Stall.Load(file);
            FilePath = filepath;
            try
            {
                Id = Stall.SelectSingleNode("/StorageSlot/id").InnerXml;
                Owner = Stall.SelectSingleNode("/StorageSlot/owner").InnerXml;
                Room = Stall.SelectSingleNode("/StorageSlot/room").InnerXml;
                ItemTotal = Stall.SelectSingleNode("/StorageSlot/itemTotal").InnerXml;
                PaidStatus = Stall.SelectSingleNode("/StorageSlot/paid").InnerXml;
                XmlNode itemlist = Stall.SelectSingleNode("StorageSlot/ItemList");
                foreach (XmlElement item in itemlist.ChildNodes)
                {
                    Items.Add(item.InnerText);
                }
            }
            catch
            {
            }
        }
    }
}
