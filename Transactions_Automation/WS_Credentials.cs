using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Transactions_Automation
{
    static class WS_Credentials
    {
        public static bool get_credentials(string account,string path)
        {
            return (fetch(account, path));   
        }

        private static bool fetch(string account,string path)
        {
            bool result = false;
            XmlDocument xdoc = new XmlDocument();
            try
            {
                xdoc.Load(path + "credentials_automation.xml");
                string query = string.Format("//*[@name='{0}']", account);
                Logger.Info("Fetching account details for " + account);
                switch (account)
                {
                    case "DHPO":
                        DHPO.username = xdoc.SelectSingleNode(query).ChildNodes[0].InnerText.ToString();
                        DHPO.password = xdoc.SelectSingleNode(query).ChildNodes[1].InnerText.ToString();
                        result = true;
                        break;
                    case "HAAD":
                        HAAD.username = xdoc.SelectSingleNode(query).ChildNodes[0].InnerText.ToString();
                        HAAD.password = xdoc.SelectSingleNode(query).ChildNodes[1].InnerText.ToString();
                        result = true;
                        break;

                    case "NE_Payer":
                        NE_Payer.payer_license = xdoc.SelectSingleNode(query).ChildNodes[0].InnerText.ToString();
                        NE_Payer.payer_key = xdoc.SelectSingleNode(query).ChildNodes[1].InnerText.ToString();
                        NE_Payer.batchsize = xdoc.SelectSingleNode(query).ChildNodes[2].InnerText.ToString();
                        NE_Payer.region = xdoc.SelectSingleNode(query).ChildNodes[3].InnerText.ToString();
                        result = true;
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured" + ex.Message);
                return false;
            }
        }
    }
}
