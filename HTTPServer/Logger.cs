using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime
            //########
            //Print on the screen
            //Console.WriteLine("Datetime: "+ DateTime.Now.ToLongTimeString());//       ToLongTimeString:  10:30:15 AM //Format
            //Console.WriteLine("Message: " + ex.Message);
            //Console.WriteLine("################");
            //Print in log file
            sr.WriteLine("Datetime: " + DateTime.Now.ToLongTimeString());
            sr.WriteLine("Message: " + ex.Message);
            sr.WriteLine("################");



        }
    }
}
