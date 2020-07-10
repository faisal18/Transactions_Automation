using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Transactions_Automation
{
    public static class DHPO_Start
    {

        #region Variables
        public static DHPO_WS.ValidateTransactionsSoapClient dhpo_ws = new DHPO_WS.ValidateTransactionsSoapClient();
        public static string app_data = ConfigurationManager.AppSettings["app_data"];
        public static string upload_file;
        public static List<string> FileID_list_local = new List<string>();
        #endregion

        public static bool GNPAT_Control()
        {
            try
            {
                bool result = false;
                Logger.Info("Process started for DHPO");
                if (WS_Credentials.get_credentials("DHPO", app_data))
                {
                    if (GetNewPAT())
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(DHPO_GNPAT.GNPAT_xml_transactions);
                        XmlNodeList downloaded_files = xdoc.GetElementsByTagName("File");
                        foreach (XmlNode files in downloaded_files)
                        {
                            string fileID = files.Attributes["FileID"].Value.ToString();
                            string FileName = files.Attributes["FileName"].Value.ToString();
                            string SenderID = files.Attributes["SenderID"].Value.ToString();
                            string ReceiverID = files.Attributes["ReceiverID"].Value.ToString();
                            string TransactionDate = files.Attributes["TransactionDate"].Value.ToString();
                            string RecordCount = files.Attributes["RecordCount"].Value.ToString();
                            string IsDownloaded = files.Attributes["IsDownloaded"].Value.ToString();

                            string[] data = { fileID, FileName, SenderID, ReceiverID, TransactionDate, RecordCount, IsDownloaded };

                            Logger.Info("Process started for FileID:" + fileID + " FileName:" + DHPO_GNPAT.GNPAT_file_name);
                            if (DownloadTransaction(fileID, "GNPAT"))
                            {
                                if (StoreTransaction(DHPO_GNPAT.GNPAT_file_content, DHPO_GNPAT.GNPAT_file_name, data))
                                {
                                    if (SetTransactions(fileID))
                                    {
                                        Logger.Info("Success");
                                    }
                                    else
                                    {
                                        Logger.Info("Failiure Occured ! Updating Download Status of Transaction+" + fileID);
                                    }
                                }
                                else
                                {
                                    Logger.Info("Failiure Occured ! Storing Transaction+" + fileID);
                                }
                            }
                            else
                            {
                                Logger.Info("Failiure Occured ! Downloading Transaction:" + fileID);
                            }
                        }
                    }
                    else
                    {
                        Logger.Info("Failiure Occured ! Get New Transaction Function Failed");
                    }
                }
                else
                {
                    Logger.Info("Failiure Occured ! Fetching Credentials Failed ");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured !" + ex.Message);
                return false;
            }
        }

        public static bool GNT_Control()
        {
            try
            {
                bool result = false;
                Logger.Info("Process started for DHPO");
                if (WS_Credentials.get_credentials("DHPO", app_data))
                {
                    if (GetNewTransaction())
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(DHPO_GNT.GNT_xml_transactions);
                        XmlNodeList downloaded_files = xdoc.GetElementsByTagName("File");
                        foreach (XmlNode files in downloaded_files)
                        {
                            string fileID = files.Attributes["FileID"].Value.ToString();
                            string FileName = files.Attributes["FileName"].Value.ToString();
                            string SenderID = files.Attributes["SenderID"].Value.ToString();
                            string ReceiverID = files.Attributes["ReceiverID"].Value.ToString();
                            string TransactionDate = files.Attributes["TransactionDate"].Value.ToString();
                            string RecordCount = files.Attributes["RecordCount"].Value.ToString();
                            string IsDownloaded = files.Attributes["IsDownloaded"].Value.ToString();

                            string[] data = { fileID, FileName, SenderID, ReceiverID, TransactionDate, RecordCount, IsDownloaded };

                            Logger.Info("Process started for FileID:" + fileID + " FileName:" + DHPO_GNT.GNT_file_name);
                            if (DownloadTransaction(fileID, "GNT"))
                            {
                                if (StoreTransaction(DHPO_GNT.GNT_file_content, DHPO_GNT.GNT_file_name, data))
                                {
                                    if (SetTransactions(fileID))
                                    {
                                        Logger.Info("Success");
                                    }
                                    else
                                    {
                                        Logger.Info("Failiure Occured ! Updating Download Status of Transaction+" + fileID);
                                    }
                                }
                                else
                                {
                                    Logger.Info("Failiure Occured ! Storing Transaction+" + fileID);
                                }
                            }
                            else
                            {
                                Logger.Info("Failiure Occured ! Downloading Transaction:" + fileID);
                            }
                        }
                    }
                    else
                    {
                        Logger.Info("Failiure Occured ! Get New Transaction Function Failed");
                    }
                }
                else
                {
                    Logger.Info("Failiure Occured ! Fetching Credentials Failed ");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured !" + ex.Message);
                return false;
            }
        }

        public static bool UT_Control()
        {
            try
            {
                bool result = false;
                string filepath, error_message, filename;
                byte[] error_report;

                string[] files_path = Directory.GetFiles(app_data + "\\DHPO_Upload\\", "*.xml");
                if (files_path.Count() > 0)
                {
                    Logger.Info("Number of files to be uploaded " + files_path.Count());
                    foreach (string file in files_path)
                    {
                        filename = Path.GetFileName(file);
                        filepath = Path.GetFullPath(file);
                        Logger.Info("Uploading File: " + filename);
                        if (encodebase64(filepath))
                        {
                            int response = dhpo_ws.UploadTransaction(DHPO.username, DHPO.password, Convert.FromBase64String(upload_file), filename, out error_message, out error_report);
                            if (response == 0)
                            {
                                Logger.Info("File Uploaded Susccessfully ! FileName: " + filename);
                                result = true;
                            }
                            else
                            {
                                using (StreamWriter text = new StreamWriter(app_data + "\\DHPO_Upload\\Error.txt"))
                                {
                                    text.Write("\n\nError Occured Uploading File: " + filename+"\n\n");
                                    text.Write(Convert.ToBase64String(error_report));
                                }
                                Logger.Info("Error Occured ! Upload file Failed for FileName: " + filename + " Error Report: " + Convert.ToBase64String(error_report));
                            }
                        }
                    }
                }
                else
                {
                    Logger.Info("The directory has no files to be uploaded");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        public static bool GNT_Threaded_Control()
        {
            try
            {
                bool result = false;

                if(GetNewTransaction())
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(DHPO_GNT.GNT_xml_transactions);
                    XmlNodeList downloaded_files = xdoc.GetElementsByTagName("File");
                    foreach(XmlNode node in downloaded_files)
                    {
                        FileID_list_local.Add(node.Attributes["FileID"].Value);
                    }

                    Parallel.ForEach(FileID_list_local, fileID =>
                    {
                        DownloadTransaction(fileID, "GNT");
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Info("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        #region DHPO Methods

        private static bool GetNewTransaction()
        {
            try
            {
                bool result = false;
                string GNT_error_message;
                string transactions_xml;
                Logger.Info("Started Get New Transaction");

                int response = dhpo_ws.GetNewTransactions(DHPO.username, DHPO.password, out transactions_xml, out GNT_error_message);
                if (response == 0)
                {
                    Logger.Info("Found Transactions");
                    DHPO_GNT.GNT_xml_transactions = transactions_xml;
                    result = true;
                }
                else
                {
                    Logger.Info("Error occured ! Error Message:" + GNT_error_message);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured !" + ex.Message);
                return false;
            }
            
        }

        private static bool GetNewPAT()
        {
            try
            {
                bool result = false;
                string GNPAT_error_message;
                string transactions_xml;
                Logger.Info("Started Get New Prior Authourization Transaction");

                int response = dhpo_ws.GetNewPriorAuthorizationTransactions(DHPO.username, DHPO.password, out transactions_xml, out GNPAT_error_message);
                if(response == 1)
                {
                    Logger.Info("Found Transactions");
                    DHPO_GNPAT.GNPAT_xml_transactions = transactions_xml;
                    result = true;
                }
                else
                {
                    Logger.Info("Error occured ! Error Message:" + GNPAT_error_message);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured !" + ex.Message);
                return false;
            }
        }

        private static bool SetTransactions(string FileID)
        {
            try
            {
                bool result = false;
                string STD_error_message;
                //int response = dhpo_ws.SetTransactionDownloaded(DHPO.username, DHPO.password, FileID, out STD_error_message);
                //if(response == 0)
                //{
                //    Logger.Info("File set as downloaded Successful ! FileID: " + FileID);  
                //    result = true;
                //}
                //else
                //{
                //    Logger.Info("Error occured ! Error Message:" + STD_error_message);
                //}
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured !" + ex.Message);
                return false;
            }
        }

        private static bool StoreTransaction(string filecontent,string filename,string[] data)
        {
            try
            {
                bool result = false;
                if (!Directory.Exists(app_data + "\\DHPO\\"))
                {
                    Directory.CreateDirectory(app_data + "\\DHPO\\");
                }
                using (StreamWriter writer = new StreamWriter(app_data + "\\DHPO\\" + filename + Path.GetExtension(filename)))
                {
                    writer.Write(filecontent);
                }
                using (StreamWriter writer = new StreamWriter(app_data + "\\DHPO\\" + filename + ".csv"))
                {
                    StringBuilder sb = new StringBuilder();
                    for(int i =0;i<data.Count();i++)
                    {
                        sb.Append(data[i] + ",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    writer.Write(sb);
                    result = true;
                }
                    return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        private static bool DownloadTransaction(string FileID,string process)
        {
            try
            {
                bool result = false;
                string error_message;
                string loc_filename;
                byte[] loc_filebyte;
                string output_file;

                int response = dhpo_ws.DownloadTransactionFile(DHPO.username, DHPO.password, FileID, out loc_filename, out loc_filebyte, out error_message);
                if (response == 0)
                {
                    if (Path.GetExtension(loc_filename).ToString().ToLower() == ".zip")
                    {
                        Logger.Info("File is in ZIP format FileID:" + FileID);
                        output_file = zip_control((Convert.ToBase64String(loc_filebyte)), loc_filename, loc_filebyte);
                        if (output_file != "0")//Exception is 0
                        {
                            Logger.Info("File Zip extraction was Successfull");
                            result = true;
                        }
                        else
                        {
                            Logger.Info("File Zip extraction Failed for FileID:" + FileID);
                        }
                    }
                    else
                    {
                        output_file = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(loc_filebyte)));
                        result = true;
                    }

                    if(process == "GNT")
                    {
                        DHPO_GNT.GNT_file_content = output_file;
                        DHPO_GNT.GNT_file_name = loc_filename;
                    }
                    else if(process == "GNPAT")
                    {
                        DHPO_GNPAT.GNPAT_file_content = output_file;
                        DHPO_GNPAT.GNPAT_file_name = loc_filename;
                    }
                }
                else
                {
                    Logger.Info("Error Occured ! Error Message:" + error_message);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured !" + ex.Message);
                return false;
            }
        }


        #region Custom Control

        private static string zip_control(string filecontent, string filename, byte[] byte_file)
        {
            try
            {
               
                string zip_path = @"C:\tmp\" + Path.GetFileName(filename);
                string file;
                System.IO.File.WriteAllBytes(zip_path, byte_file);
                string extract_path = zip_path + "_" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                using (var zip = ZipFile.Read(zip_path))
                {
                    zip.ExtractAll(extract_path, ExtractExistingFileAction.OverwriteSilently);
                    zip.Dispose();
                }

                string[] filearray = Directory.GetFiles(extract_path);
                string new_path = "";
                foreach (string s in filearray)
                {
                    new_path = "C:\\tmp\\" + Path.GetFileName(s);
                    System.IO.File.Copy(s, "C:\\tmp\\" + Path.GetFileName(s));
                }

                using (StreamReader read = new StreamReader(new_path))
                {
                    StringBuilder sb = new StringBuilder();
                    string text;
                    while ((text = read.ReadLine()) != null)
                    {
                        sb.Append(text);
                    }
                    file = sb.ToString();
                }
                File.Delete(new_path);
                File.Delete(zip_path);
                Directory.Delete(extract_path, true);

                return file;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured while extracting zip file !" + ex.Message);
                return "0";
            }
        }

        private static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        private static bool encodebase64(string filepath)
        {
            try
            {
                Byte[] bytes = File.ReadAllBytes(filepath);
                upload_file = Convert.ToBase64String(bytes);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Info("Exception Occured ! "+ ex.Message);
                return false;
            }
        }

        #endregion

        #endregion
    }
}
