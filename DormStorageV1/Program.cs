using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DormStorageV1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Updater updater = new Updater();
            StorageHandler scla = new StorageHandler("StorageManifest.xml");
            while (true)
            {
                Helpers.PublishHeader("|| SCLA Dorm Storage System ||", ConsoleColor.Blue, "Welcome to the Dorm Storage System.\nPlease try: 'add', 'remove', 'detail', 'clear', 'help', or 'list'.", ConsoleColor.White);
                Helpers.PublishPrompt("> ", ConsoleColor.Yellow);
                string choice = Console.ReadLine().ToLowerInvariant();
                if (choice.Equals("quit"))
                {
                    break;
                }
                if (choice.Equals("list"))
                {
                    scla.PublishManifest();
                }
                else if (choice.StartsWith("add "))
                {
                    string[] add = choice.Split(' ');
                    if (add.Length == 2)
                    {
                        while (true)
                        {
                            string id = add[1];
                            if (scla.AvailSpots.ContainsKey(id))
                            {
                                Console.WriteLine("A slot with this ID already exists. Can't write to this spot.");
                                Console.ReadKey();
                                Console.Clear();
                                break;
                            }
                            scla.AddManifestElement(id, scla.StorageFilePath);
                            Console.WriteLine("Successfully created Storage Slot " + id + ". It is currently empty.\nNo editability implemented yet.");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                    }
                    else if (add.Length == 6)
                    {
                        string id = add[1];
                        string owner = add[2];
                        string room = add[3].ToUpperInvariant();
                        string itemTotal = add[4];
                        string paid = add[5];
                        while (true)
                        {
                            if (scla.AvailSpots.ContainsKey(id))
                            {
                                Console.WriteLine("A slot with this ID already exists. Can't write to this spot.");
                                Console.ReadKey();
                                Console.Clear();
                                break;
                            }
                            if (room.StartsWith("A") || room.StartsWith("B") || room.StartsWith("C") || room.StartsWith("D") || room.StartsWith("E") || room.StartsWith("F"))
                            {
                            }
                            else
                            {
                                Console.WriteLine("Room number formatting needs to include the Wing letter and room number.");
                                Console.ReadKey();
                                Console.Clear();
                                break;
                            }
                            if (!paid.Equals("yes") && !paid.Equals("no"))
                            {
                                Console.WriteLine("Paid status can only be 'yes' or 'no'.");
                                Console.ReadKey();
                                Console.Clear();
                                break;
                            }
                            scla.AddManifestElement(id, owner, room, itemTotal, paid, scla.StorageFilePath);
                            Console.WriteLine("Successfully created Storage #" + id + " owned by " + Helpers.CapitalizeName(owner) + ".\nThey have " + itemTotal + " item(s).");
                            Console.WriteLine("\nPress any key to continue. . .");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Insufficient amount of arguments.");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                else if (choice.StartsWith("remove "))
                {
                    string id = choice.Substring(7);
                    while (true)
                    {
                        if (!scla.AvailSpots.ContainsKey(id))
                        {
                            Console.WriteLine("Cannot find Slot #" + id + ". Please try again.\nPress any key to continue. . .");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                        else
                        {
                            scla.RemoveManifestElement(id, scla.StorageFilePath);
                            break;
                        }
                    }
                }
                else if (choice.StartsWith("detail "))
                {
                    Console.Clear();
                    string id = choice.Substring(7);
                    while (true)
                    {
                        if (!scla.AvailSpots.ContainsKey(id))
                        {
                            Console.WriteLine("Cannot find a slot with this Id: " + id + ".\nPress any key to continue. . .");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                        scla.DetailedSpot(id, scla.StorageFilePath);
                        break;
                    }
                }
                else if (choice.StartsWith("clear "))
                {
                    Console.Clear();
                    string id = choice.Substring(6);
                    while (true)
                    {
                        Helpers.PublishPrompt("Are you sure? This is irreversible.", ConsoleColor.Red);
                        Console.Write("\nY or N\n> ");
                        string confirm = Console.ReadLine().ToLowerInvariant();
                        if (confirm == "y")
                        {
                            scla.ClearManifestEntry(id, scla.StorageFilePath);
                            Console.WriteLine("Storage Slot #" + id + " successfully cleared.");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                        break;
                    }
                }
                else if (choice.Equals("edit"))
                {
                    Console.WriteLine("Not yet implemented.");
                    Console.ReadKey();
                    Console.Clear();
                }
                else if (choice.StartsWith("help"))
                {
                    string key = choice.Substring(4).ToLowerInvariant();
                    if (key.Equals(""))
                    {
                        Console.WriteLine("Help is able to tell you more about each command. Try adding them as a secondary argument: 'add', 'remove', or 'alter'.");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else if (key.Equals(" list"))
                    {
                        Console.WriteLine("The [LIST] command lists out all of the nodes within the db. This enables a quick view to see which are filled \nor emptied.");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else if (key.Equals(" add"))
                    {
                        Console.WriteLine("The [ADD] command allows you to generate a unique Storage Slot.\nSyntax 1: add <id> <status>\nSyntax 2: add <id> <status> <owner> <room> <itemTotal> <paid>");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else if (key.Equals(" remove"))
                    {
                        Helpers.PublishWarning("Caution! Can cause data loss!", ConsoleColor.Red);
                        Console.WriteLine("The [REMOVE] command is allows you to delete a storage entry from the db. Use this cautiously.\nSyntax: remove <id>");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else if (key.Equals(" detail"))
                    {
                        Console.WriteLine("The [DETAIL] command gives a deeper look at the specific slot.\nSyntax: detail <id>");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else if (key.Equals(" clear"))
                    {
                        Helpers.PublishWarning("Caution! Can cause data loss!", ConsoleColor.Red);
                        Console.WriteLine("The [CLEAR] command erases all saved data from a storage slot.\nSyntax: clear <id>");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else
                    {
                        Console.WriteLine("That command wasn't found. Sorry.");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Command not recognized. Try 'add', 'remove', 'alter', 'help', or 'quit.'");
                    Console.ResetColor();
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }
    }
}
