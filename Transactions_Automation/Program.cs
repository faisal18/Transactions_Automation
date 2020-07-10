using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Threading;

namespace Transactions_Automation
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            //start_automation();
        }

        public static void start_automation()
        {
            ThreadPool.SetMaxThreads(10, 10);
            ThreadPool.QueueUserWorkItem(new WaitCallback(runit));
        }

        private static void runit(object x)
        {
            DHPO_Start.GNT_Control();
            DHPO_Start.GNPAT_Control();
            DHPO_Start.UT_Control();

            HAAD_Start.GNT_Control();
            HAAD_Start.GNPAT_Control();
            HAAD_Start.UT_Control();

            NE_Start.GNC_Control();
            NE_Start.GNPR_Control();
            NE_Start.UT_Control();
        }
    }
}
