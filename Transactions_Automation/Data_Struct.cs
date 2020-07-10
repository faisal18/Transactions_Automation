using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions_Automation
{
    public static class DHPO_GNT
    {
        public static string GNT_file_content { get; set; }
        public static string GNT_file_name { get; set; }
        public static string GNT_xml_transactions { get; set; }
    }

    public static class DHPO_GNPAT
    {
        public static string GNPAT_file_content { get; set; }
        public static string GNPAT_file_name { get; set; }
        public static string GNPAT_xml_transactions { get; set; }
    }


    public static class HAAD_GNT
    {
        public static string GNT_file_content { get; set; }
        public static string GNT_file_name { get; set; }
        public static string GNT_xml_transactions { get; set; }
    }

    public static class HAAD_GNPAT
    {
        public static string GNPAT_file_content { get; set; }
        public static string GNPAT_file_name { get; set; }
        public static string GNPAT_xml_transactions { get; set; }
    }

    
    public static class NE_GNC
    {

        public static string GNC_BatchID { get; set; }
        public static string GNC_TransactionType { get; set; }
        public static string GNC_BatchDateTime { get; set; }
        
    }

    public class NE_GNC_list
    {
        public static string NE_GNC_CS { get; set; }
        public static string NE_GNC_Filename { get; set; }
    }


    public static class NE_GNPR
    {
        public static string GNPR_BatchID { get; set; }
        public static string GNPR_TransactionType { get; set; }
        public static string GNPR_BatchDateTime { get; set; }
    }

    public class NE_GNPR_list
    {
        public static string NE_GNPR_CS { get; set; }
        public static string NE_GNPR_Filename { get; set; }
    }
}
