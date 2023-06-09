﻿using System;
using System.Collections.Generic;
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
        public string LocalVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Version localVersion = assembly.GetName().Version;
                return localVersion.ToString();
            }
        }
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
                string value = node.GetAttribute("owner");

                AvailSpots.Add(key, value);
            }
        }

        public void PublishManifest()
        {
            Console.Clear();
            Helpers.PublishHeader("|| Manifest List || ", Header);
            if (AvailSpots.Count == 0)
            {
                Helpers.PublishWarning("\nNo registeries found in the manifest.", Information);
                Console.ReadKey();
                Console.Clear();
                return;
            }
            foreach (KeyValuePair<string, string> slot in AvailSpots)
            {
                if (!slot.Value.Equals(""))
                {
                    Console.ForegroundColor = Occupied;
                    Console.WriteLine("Storage Slot #" + slot.Key + ": Occupied by " + Helpers.CapitalizeName(slot.Value));
                }
                else if (slot.Value == "") 
                {
                    Console.ForegroundColor = Available;
                    Console.WriteLine("Storage Slot #" + slot.Key + ": Empty");
                }
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
            slotAdder.SetAttribute("owner", Helpers.OwnerValidator(owner));
            slotAdder.SetAttribute("room", room);
            slotAdder.SetAttribute("itemTotal", itemTotal);
            slotAdder.SetAttribute("paid", Helpers.PaidValidator(paid, itemTotal));
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

        private void DetailedSpotPublish()
        {
            string[] DictInventory = { "owner", "room", "itemTotal", "paid" };
            SpotDetail.TryGetValue("id", out string id);
            Helpers.PublishHeader("|| Detailed Look at Slot " + id + " ||", Header);
            Console.ResetColor();
            foreach(string key in DictInventory)
            {
                SpotDetail.TryGetValue($"{key}", out string value);
                if (key == "owner" && value == "")
                {
                    Helpers.PublishDetail(key.ToUpper(), UserPrompt, "No owner.");
                    continue;
                }
                else if (key == "owner" && !value.Equals(""))
                {
                    Helpers.PublishDetail(key.ToUpper(), UserPrompt, Helpers.CapitalizeName(value));
                    continue;
                }
                if (key == "itemTotal")
                { 
                    Helpers.PublishDetail("ITEM TOTAL", UserPrompt, value);
                    continue;
                }
                if (key == "paid" && value.Contains("owes"))
                {
                    Helpers.PublishDetail("PAID", Warning, value);
                    continue;
                }
                else
                {
                    Helpers.PublishDetail(key.ToUpper(), UserPrompt, value);
                }
            }
            Console.ReadKey();
            Console.Clear();
        }

        public void EditSelectedRegistry(string id, string key, string replace, string file)
        {
            StorageDB = null;
            StorageDB = new XmlDocument();
            StorageDB.Load(file);
            XmlElement selectedNode = (XmlElement)StorageDB.SelectSingleNode("/DormStorage/Slot[@id='" + id + "']");
            if (key.Equals("items"))
            {
                key = "itemTotal";
            }
            else if (key.Equals("paid"))
            {
                selectedNode.SetAttribute(key, Helpers.PaidValidator(replace, selectedNode.GetAttribute("itemTotal")));
            }
            else if (key.Equals("owner"))
            {
                selectedNode.SetAttribute(key, Helpers.OwnerValidator(replace));
            }
            else
            {
                selectedNode.SetAttribute(key, replace);
            }
            StorageDB.Save(file);
            LoadStorageManifest(StorageFilePath);
        }

        /// <summary>
        /// This variation functions for the "editall" command. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <param name="room"></param>
        /// <param name="items"></param>
        /// <param name="paid"></param>
        /// <param name="file"></param>
        /// <exception cref="InvalidCastException"/>
        public void EditSelectedRegistry(string id, string owner, string room, string items, string paid, string file)
        {
            StorageDB = null;
            StorageDB = new XmlDocument();
            StorageDB.Load(file);
            XmlElement selectedNode = (XmlElement)StorageDB.SelectSingleNode("/DormStorage/Slot[@id='" + id + "']");
            selectedNode.SetAttribute("owner", owner);
            selectedNode.SetAttribute("room", room);
            selectedNode.SetAttribute("itemTotal", items);
            selectedNode.SetAttribute("paid", Helpers.PaidValidator(paid, items));
            StorageDB.Save(file);
            LoadStorageManifest(StorageFilePath);
        }
    }
}
