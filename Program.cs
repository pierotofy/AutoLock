using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoLock
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            int port = 4325;
            string token = "";

            for (int i = 0; i < args.Length; i+= 2)
            {
                string param = args[i].ToLower();
                if (i + 1 >= args.Length) break;
                string value = args[i + 1];

                if (param == "--port" || param == "-p") port = int.Parse(value);
                else if (param == "--token" || param == "-t") token = value;
            }
            
            // Show the system tray icon.
            using (ProcessIcon pi = new ProcessIcon(port))
            {
                using (Listener l = new Listener(port, token))
                {
                    l.Listen();
                    pi.Display();

                    // Make sure the application runs!
                    Application.Run();
                }
            }
        }
    }
}
