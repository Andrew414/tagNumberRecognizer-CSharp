using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tagrec_S
{
    static class Program
    {
        [STAThread]
        static void Main(String [] args)
        {
           string filename = "";
           if (args != null)
           {
               filename = args.Count() > 1 ? args.First(a => a != "cmd") : "";
               if (!args.Contains("cmd"))
               {
                   Application.EnableVisualStyles();
                   Application.SetCompatibleTextRenderingDefault(false);
                   if (args != null && args.Length >= 1)
                   {
                       Application.Run(new TagrecSForm(args[0]));
                   }
                   else
                   {
                       Application.Run(new TagrecSForm(""));
                   }
               }
               else
               {
                   CaptureProcessor processor;
                   if (args != null && args.Length >= 2) {
                    processor = new CaptureProcessor(args[1]);
                   }
                   else  {
                    processor = new CaptureProcessor("");                   
                   }

                   while (true)
                   {
                       String result = processor.MakeCapture();
                       if (result == null)
                       {
                           return; 
                       }
                       if (result != "")
                       {
                           Console.WriteLine(result);
                       }
                   }
               }
           }
        }
    }
}
