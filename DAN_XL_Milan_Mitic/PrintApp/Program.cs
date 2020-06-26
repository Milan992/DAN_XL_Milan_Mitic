﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace PrintApp
{
    class Program
    {
        static bool AllPrinted = false;

        static Random random = new Random();

        static EventWaitHandle request = new AutoResetEvent(false);
        static readonly object l = new object();

        static Document document = new Document();

        static string[] formats = { "A3", "A4" };
        static string[] orientations = { "portrait", "landscape" };
        // list to add all the colors from the txt file
        static List<string> colors = new List<string>();

        static void Main(string[] args)
        {
            Console.WriteLine("\n\tPRINT\n");

            string color = "";

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
            Thread getColors = new Thread(() => FillColorList());
            getColors.Start();
            getColors.Join();

            // create 10 pc threads.
            Thread pc;
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

        /// <summary>
        /// Chooses document data and sends a request to printer to print.
        /// </summary>
        public static void SendRequest()
        {
            while (!AllPrinted)
            {
                Thread.Sleep(100);
                lock (l)
                {
                    // choose format
                    document.Format = formats[random.Next(0, 2)];

                    // choose color
                    document.Color = colors[random.Next(0, colors.Count)];

                    document.Orientation = orientations[random.Next(0, 2)];

                    Console.WriteLine("\n{0} sent a print request for a {1} format document, color: {2}, orientation: {3}.", 
                        Thread.CurrentThread.Name, document.Format, document.Color, document.Orientation);
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
    }
}
