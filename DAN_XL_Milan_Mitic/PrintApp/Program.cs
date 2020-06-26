using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrintApp
{
    class Program
    {
        // at the star of the program no colors are entered
        static bool colorsEntered = false;

        static Random random = new Random();

        static EventWaitHandle request = new AutoResetEvent(false);

        static Document document = new Document();

        static void Main(string[] args)
        {
            Console.WriteLine("\n\tPRINT\n");

            string color = "";

            // if no colors are entered, ask the user to enter them.
            if (colorsEntered == false)
            {
                // clear the file from previous program run insert.
                File.WriteAllText(@"..\..\Palet.txt", "");

                while (color != "#")
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
            // colors are entered, so the user will not be asked to do it again
            colorsEntered = true;

            Thread pc;
            // create 10 pc threads.
            for (int i = 0; i < 10; i++)
            {
                pc = new Thread(() => SendRequest());
                pc.Name = "PC+" + i;
                pc.Start();
            }

            // create 2 printer threads.
            Thread printer;
            for (int i = 0; i < 2; i++)
            {
                printer = new Thread(() => Print(document));
                printer.Name = "PRINTER_" + i;
                printer.Start();
            }
        }

        public static void Print(Document document)
        {

        }

        public static void SendRequest()
        {

        }
    }
}
