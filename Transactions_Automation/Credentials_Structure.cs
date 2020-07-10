using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions_Automation
{
    public static class DHPO
    {
        public static string username { get; set; }
        public static string password { get; set; }
    }

    public static class HAAD
    {
        public static string username { get; set; }
        public static string password { get; set; }
    }

    public static class NE_Payer
    {
        public static string payer_license { get; set; }
        public static string payer_key { get; set; }
        public static string batchsize { get; set; }
        public static string region { get; set; }
    }
}
