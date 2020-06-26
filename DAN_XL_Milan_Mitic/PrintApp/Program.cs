using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n\tPRINT\n");

            string color = "";

            while (color !="#")
            {
                Console.WriteLine("\nPlease enter a color to save to the palet. Press '#' to quit");
                color = Console.ReadLine();
                if (!string.IsNullOrEmpty(color) && color != "#")
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(@"..\..\Palet.txt", true))
                        {
                            sw.WriteLine(color);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
