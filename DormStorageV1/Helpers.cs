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
            string[] firstLast = toCapitalize.Split('_');
            char[] firstChars = firstLast[0].ToCharArray();
            char[] lastChars = firstLast[1].ToCharArray();
            List<string> firstName = new List<string>();
            List<string> lastName = new List<string>();
            foreach (char c in firstChars)
            {
                firstName.Add(c.ToString());
            }
            foreach (char c in lastChars)
            {
                lastName.Add(c.ToString());
            }
            firstName[0] = firstName[0].ToUpperInvariant();
            lastName[0] = lastName[0].ToUpperInvariant();
            return string.Join("", firstName) + " " + string.Join("", lastName); 
        }

        public static string PaidValidator(string paid, string itemTotal)
        {
            int.TryParse(itemTotal, out int result);
            if (paid == "yes")
            {
                return "No payment needed.";
            }
            else if (paid == "no" && result < 4)
            {
                return "No payment needed.";
            }
            else if (paid == "no" && result > 4)
            {
                return "Student owes $" + ((result - 4) * 25) + ".";
            }
            else
            {
                return "Unable to validate payment.";
            }
        }

        public static string OwnerValidator(string owner)
        {
            while (true)
            {
                if (!owner.Contains("_"))
                {
                    Helpers.PublishWarning("Name needs to follow this syntax: \n{First}_{Last}.\n", ConsoleColor.Red);
                    return "error";
                }
                return owner;
            }
        }
    }
}
