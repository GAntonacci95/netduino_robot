using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace ComputerSide
{
    static class Startup
    {
        [STAThread]
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>        
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Loading());
            //Application.Run(new NUI());
        }
    }
}
