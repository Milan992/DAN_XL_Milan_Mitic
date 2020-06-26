using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace PrintApp
{
    class Program
    {
        // create 2 printer threads.
        static Thread printerOne = new Thread(() => PrintOne(document, currentPcName));
        static Thread printerTwo = new Thread(() => PrintTwo(document, currentPcName));
        static Thread pc;

        static bool AllPrinted = false;

        static Random random = new Random();

        static CountdownEvent ce = new CountdownEvent(10);
        static EventWaitHandle requestOne = new AutoResetEvent(false);
        static EventWaitHandle requestTwo = new AutoResetEvent(false);
        static readonly object l = new object();
        static readonly object l2 = new object();
        static readonly object l3 = new object();

        static Document document = new Document();

        static string currentPcName = "";

        static string[] formats = { "A3", "A4" };
        static string[] orientations = { "portrait", "landscape" };
        // list to add all the colors from the txt file
        static List<string> colors = new List<string>();
        // list to add all distinct PCs that printed at least one document
        static List<string> pcsPrintedAtLeastOne = new List<string>();

        static void Main(string[] args)
        {
            Console.WriteLine("\n\tPRINT\n");

            string color = "";

            // clear the file from previous program run insert.
            File.WriteAllText(@"..\..\Palet.txt", "white");

            while (color != "#")
            {
                Console.WriteLine("\nPlease enter a color to save to the palet. Press '#' when you finish entering colors to start the program.");
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
            Thread stopper = new Thread(() => Stopper());
            stopper.Start();
            Thread getColors = new Thread(() => FillColorList());
            getColors.Start();
            getColors.Join();

            // create 10 pc threads.
            for (int i = 0; i < 10; i++)
            {
                pc = new Thread(() => SendRequest());
                pc.Name = "PC_" + i;
                pc.Start();
            }

            printerOne.Name = "PRINTER_1";
            printerTwo.Name = "PRINTER_2";
            printerOne.Start();
            printerTwo.Start();
        }

        /// <summary>
        /// Writes out on the console when the document is printed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pcName"></param>
        public static void PrintOne(Document document, string pcName)
        {
            requestOne.WaitOne();
            lock (l2)
            {
                while (!AllPrinted)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("\n{0} can take {1} format document from " + Thread.CurrentThread.Name, currentPcName, document.Format);
                    Console.WriteLine("");

                    if (!pcsPrintedAtLeastOne.Contains(currentPcName))
                    {
                        pcsPrintedAtLeastOne.Add(currentPcName);
                        ce.Signal();
                    }
                }
            }
        }

        /// <summary>
        /// Writes out on the console when the document is printed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pcName"></param>
        public static void PrintTwo(Document document, string pcName)
        {
            requestTwo.WaitOne();

            lock (l3)
            {
                while (!AllPrinted)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("\n{0} can take {1} format document from " + Thread.CurrentThread.Name, currentPcName, document.Format);
                    Console.WriteLine("");

                    if (!pcsPrintedAtLeastOne.Contains(currentPcName))
                    {
                        pcsPrintedAtLeastOne.Add(currentPcName);
                        ce.Signal();
                    }
                }
            }
        }

        /// <summary>
        /// Chooses document data and sends a request to printer to print.
        /// </summary>
        public static void SendRequest()
        {
            while (!AllPrinted)
            {
                lock (l)
                {
                    Thread.Sleep(100);

                    // choose format
                    document.Format = formats[random.Next(0, 2)];

                    // choose color
                    document.Color = colors[random.Next(0, colors.Count)];

                    //choose orientation
                    document.Orientation = orientations[random.Next(0, 2)];

                    currentPcName = Thread.CurrentThread.Name;

                    Console.WriteLine("{0} sent a print request for a {1} format document, color: {2}, orientation: {3}.",
                        Thread.CurrentThread.Name, document.Format, document.Color, document.Orientation);

                    int printerNumber = random.Next(1, 3);
                    if (printerNumber == 1)
                    {
                        requestOne.Set();
                    }
                    else
                    {
                        requestTwo.Set();
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the lines from a file and adds them to a list.
        /// </summary>
        public static void FillColorList()
        {
            try
            {
                using (StreamReader sr = new StreamReader(@"..\..\Palet.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        colors.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Waits coundown event to signal and stops printing and sending requests
        /// </summary>
        public static void Stopper()
        {
            ce.Wait();
            AllPrinted = true;
            printerOne.Abort();
            printerTwo.Abort();
            pc.Abort();
            Console.WriteLine("\n\tALL PRINTED. . . press any key to exit\n");
            Console.ReadLine();
        }
    }
}
