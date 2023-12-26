using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using SUBDLab;

namespace SUBDForm
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var host = new ServiceHost(typeof(SUBDLab.DBMenu)))
            {
                host.Open();
                Console.WriteLine(">Host opened");
                Console.ReadLine();
            }
        }
    }
}
