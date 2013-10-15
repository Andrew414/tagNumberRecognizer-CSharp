using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;


namespace Tagrec_S
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string [] args)
        {
            string filename = "";
            if (args != null) {
                filename = args.Count() > 1 ? args.First (a => a != "cmd") : "";
                if(!args.Contains("cmd")) {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new TagrecSForm(filename));
                } 
                else 
                {
                    while(true)
                    {
                        var cap = new CaptureProcessor (filename);
                        cap.MakeCapture ();
                        if(cap.lastNumberSaved != "")
                        {
                            Console.WriteLine (cap.lastNumberSaved);
                        }

                        Thread.Sleep (1000);
                    }
                }
            }

        }
    }
}
