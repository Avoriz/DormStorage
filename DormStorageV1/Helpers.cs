using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DormStorageV1
{
    public static class Helpers
    {
        public static void PublishHeader(string header, ConsoleColor hColor)
        {
            Console.ForegroundColor = hColor;
            Console.WriteLine(header);
            Console.ResetColor();
        }

        public static void PublishHeader(string header, ConsoleColor hColor, string description, ConsoleColor dColor) 
        {
            PublishHeader(header, hColor);
            Console.ForegroundColor = dColor;
            Console.WriteLine(description);
            Console.ResetColor();
        }

        public static void PublishPrompt(string prompt, ConsoleColor pColor) 
        {
            Console.ForegroundColor = pColor;
            Console.Write(prompt);
            Console.ResetColor();
        }

        public static void PublishWarning(string warning, ConsoleColor wColor) 
        {
            Console.ForegroundColor = wColor;
            Console.WriteLine(warning);
            Console.ResetColor();
        }

        public static void PublishNotice(string msg, ConsoleColor nColor)
        {
            Console.ForegroundColor = nColor;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void PublishDetail(string intro, ConsoleColor introColor, string description)
        {
            Console.ForegroundColor = introColor;
            Console.Write(intro + ": ");
            Console.ResetColor();
            Console.WriteLine(description);
        }

        public static string CapitalizeName(string toCapitalize)
        {
            char[] nameBreak = toCapitalize.ToCharArray();
            string letter;
            List<string> name = new List<string>();
            foreach (char c in nameBreak)
            {
                letter = c.ToString();
                name.Add(letter);
            }
            if (name.Count == 0)
            {
                return toCapitalize;
            }
            string firstCap = name[0].ToUpperInvariant();
            name[0] = firstCap;
            return string.Join("", name);
        }
    }
}
