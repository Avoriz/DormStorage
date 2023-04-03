using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DormStorageV1
{
    internal class StorageHandler
    {
        public XmlDocument StorageDB { get; set; }
        public Dictionary<string, string> AvailSpots;
        public Dictionary<string, string> SpotDetail;
        public string StorageFilePath { get; set; }
        public static ConsoleColor Header { get; set; }
        public static ConsoleColor Information { get; set; }
        public static ConsoleColor Warning { get; set; }
        public static ConsoleColor Error { get; set; }
        public static ConsoleColor UserPrompt { get; set; }
        public static ConsoleColor Occupied { get; set; }
        public static ConsoleColor Available { get; set; }
        public StorageHandler()
        {
        }
        public StorageHandler(string filename)
        {
            StorageFilePath = filename;
            Init();
            LoadStorageManifest(StorageFilePath);
        }

        public void Init()
        {
            CheckForManifest(StorageFilePath);
            Header = ConsoleColor.DarkCyan;
            Information = ConsoleColor.Gray;
            Warning = ConsoleColor.Red;
            Error = ConsoleColor.DarkRed;
            UserPrompt = ConsoleColor.Yellow;
            Occupied = ConsoleColor.DarkGray;
            Available = ConsoleColor.White;
        }

        public void CheckForManifest(string filename)
        {
            if (!File.Exists(filename))
            {
                StorageDB = new XmlDocument();
                StorageDB.PreserveWhitespace = true;
                File.Create(filename).Close();
                StorageDB.LoadXml(
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                    "<DormStorage>\n" +
                    "</DormStorage>");
                StorageDB.Save(filename);
            }
        }

        public void LoadStorageManifest(string filePath)
        {
            AvailSpots = null;
            AvailSpots = new Dictionary<string, string>();
            StorageDB = new XmlDocument();
            StorageDB.Load(filePath);
            XmlNode root = StorageDB.DocumentElement;
            foreach(XmlElement node in root.ChildNodes )
            {
                string key = node.GetAttribute("id");
                string value = node.GetAttribute("status");

                AvailSpots.Add(key, value);
            }
        }

        public void PublishManifest()
        {
            Console.Clear();
            Helpers.PublishHeader("|| Manifest List || ", Header);
            foreach (KeyValuePair<string, string> slot in AvailSpots)
            {
                if (slot.Value == "filled")
                {
                    Console.ForegroundColor = Occupied;
                }
                else if (slot.Value == "empty") 
                {
                    Console.ForegroundColor = Available;
                }
                Console.WriteLine("Storage Slot #" + slot.Key+ ": " + slot.Value);
                Console.ResetColor();
            }
            Helpers.PublishPrompt("\nPress any key to continue. . .", UserPrompt);
            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();
        }

        public void AddManifestElement(string id, string file)
        {
            StorageDB = null;
            StorageDB = new XmlDocument();
            StorageDB.Load(file);
            XmlElement slotAdder = StorageDB.CreateElement("Slot");
            slotAdder.SetAttribute("id", id);
            slotAdder.SetAttribute("status", "empty");
            slotAdder.SetAttribute("owner", "");
            slotAdder.SetAttribute("room", "");
            slotAdder.SetAttribute("itemTotal", "");
            slotAdder.SetAttribute("paid", "");
            XmlNode storageData = StorageDB.SelectSingleNode("/DormStorage");
            storageData.AppendChild(slotAdder);
            StorageDB.Save(file);
            LoadStorageManifest(file);
        }

        public void AddManifestElement(string id, string owner, string room, string itemTotal, string paid, string file)
        {
            StorageDB = null;
            StorageDB = new XmlDocument();
            StorageDB.Load(file);
            XmlElement slotAdder = StorageDB.CreateElement("Slot");
            slotAdder.SetAttribute("id", id);
            slotAdder.SetAttribute("status", "filled");
            slotAdder.SetAttribute("owner", owner);
            slotAdder.SetAttribute("room", room);
            slotAdder.SetAttribute("itemTotal", itemTotal);
            slotAdder.SetAttribute("paid", paid);
            XmlNode storageData = StorageDB.SelectSingleNode("/DormStorage");
            storageData.AppendChild(slotAdder);
            StorageDB.Save(file);
            LoadStorageManifest(file);
        }

        public void RemoveManifestElement(string id, string file)
        {
            while (true)
            {
                Helpers.PublishWarning("Are you sure? All data will be unrecoverable.", Warning);
                Helpers.PublishPrompt("Y or N\n> ", UserPrompt);
                string reply = Console.ReadLine().ToLowerInvariant();
                if (reply.Equals("y"))
                {
                    StorageDB = null;
                    StorageDB = new XmlDocument();
                    StorageDB.Load(file);
                    string nodeLocation = "/DormStorage/Slot[@id='" + id + "']";
                    XmlNode selectedSlot = StorageDB.SelectSingleNode(nodeLocation);
                    selectedSlot.ParentNode.RemoveChild(selectedSlot);
                    StorageDB.Save(file);
                    LoadStorageManifest(file);
                    Helpers.PublishNotice("Storage Slot #" + id + " has been removed.", Information);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                }
                else if (reply.Equals("n"))
                {
                    Console.Clear();
                    break;
                }
            }
        }

        public void ClearManifestEntry(string id, string file)
        {
            string[] attrList = { "owner", "room", "itemTotal", "paid" };
            StorageDB = null;
            StorageDB = new XmlDocument();
            StorageDB.Load(file);
            string nodeLocation = "/DormStorage/Slot[@id='" + id + "']";
            XmlElement selectedSlot = (XmlElement)StorageDB.SelectSingleNode(nodeLocation);
            foreach (string attr in attrList)
            {
                selectedSlot.SetAttribute($"{attr}", "");
            }
            selectedSlot.SetAttribute("status", "empty");
            StorageDB.Save(file);
            LoadStorageManifest(file);
        }

        public void DetailedSpot(string id, string file)
        {
            StorageDB = null;
            StorageDB = new XmlDocument();
            SpotDetail = new Dictionary<string, string>();
            StorageDB.Load(file);
            string nodeLocation = "/DormStorage/Slot[@id='" + id + "']";
            XmlNode selectedSlot = StorageDB.SelectSingleNode(nodeLocation);
            foreach (XmlAttribute attr in selectedSlot.Attributes)
            {
                string key = attr.Name;
                string value = attr.Value;

                SpotDetail.Add(key, value);
            }
            DetailedSpotPublish();
        }

        public void DetailedSpotPublish()
        {
            string[] DictInventory = { "owner", "room", "itemTotal", "paid" };
            SpotDetail.TryGetValue("id", out string id);
            Helpers.PublishHeader("|| Detailed Look at Slot " + id + " ||", Header);
            Console.ResetColor();
            foreach(string key in DictInventory)
            {
                SpotDetail.TryGetValue($"{key}", out string value);
                if (key == "owner")
                {
                    Helpers.PublishDetail(key.ToUpper(), UserPrompt, Helpers.CapitalizeName(value));
                }
                else 
                { 
                    Helpers.PublishDetail(key.ToUpper(), UserPrompt, value);
                }
            }
            Console.ReadKey();
            Console.Clear();
        }

        public void EditSelectedRegistry(string id, string file)
        {
            StorageDB = null;
            StorageDB = new XmlDocument();
            StorageDB.Load(file);
        }
    }
}
