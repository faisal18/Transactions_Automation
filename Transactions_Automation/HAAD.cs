using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Transactions_Automation
{
    public static class HAAD_Start
    {

        #region Variables
        public static HAAD_WS.WebservicesSoapClient haad_ws = new HAAD_WS.WebservicesSoapClient();
        public static string app_data = ConfigurationManager.AppSettings["app_data"];
        public static string upload_file;
        #endregion

        public static bool GNT_Control()
        {
            try
            {
                bool result = false;
                Logger.Info("Process started for HAAD");
                if (WS_Credentials.get_credentials("HAAD", app_data))
                {
                    if (GetNewTransaction())
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(HAAD_GNT.GNT_xml_transactions);
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


                            Logger.Info("Process started for FileID:" + fileID + " FileName:" + HAAD_GNT.GNT_file_name);
                            if (DownloadTransaction(fileID, "HAAD_GNT"))
                            {
                                if (StoreTransaction(HAAD_GNT.GNT_file_content, HAAD_GNT.GNT_file_name, data))
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

        public static bool GNPAT_Control()
        {
            try
            {
                bool result = false;
                Logger.Info("Process started for HAAD");
                if(WS_Credentials.get_credentials("HAAD", app_data))
                {
                    if (GetNewTransaction())
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(HAAD_GNPAT.GNPAT_xml_transactions);
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

                            Logger.Info("Process started for FileID:" + fileID + " FileName:" + HAAD_GNPAT.GNPAT_file_name);
                            if (DownloadTransaction(fileID, "HAAD_GNPAT"))
                            {
                                if (StoreTransaction(HAAD_GNPAT.GNPAT_file_content, HAAD_GNPAT.GNPAT_file_name, data))
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

            catch(Exception ex)
            {
                Logger.Error("Exception Occured ! " + ex.Message);
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

                string[] files_path = Directory.GetFiles(app_data + "\\HAAD_Upload\\","*.xml");
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
                            int response = haad_ws.UploadTransaction(HAAD.username, HAAD.password, Convert.FromBase64String(upload_file), filename, out error_message, out error_report);
                            if (response == 0)
                            {
                                Logger.Info("File Uploaded Susccessfully ! FileName: " + filename);
                                result = true;
                            }
                            else
                            {
                                using (StreamWriter text = new StreamWriter(app_data + "\\HAAD_Upload\\Error.txt"))
                                {
                                    text.Write("\n\nError Occured Uploading File: " + filename + "\n\n");
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

        #region HAAD Methods

        private static bool GetNewTransaction()
        {
            try
            {
                bool result = false;
                string transaction_xml,error_message;

                int response = haad_ws.GetNewTransactions(HAAD.username, HAAD.password, out transaction_xml, out error_message);
                if(response == 0)
                {
                    HAAD_GNT.GNT_xml_transactions = transaction_xml;
                    result = true;
                }
                else
                {
                    Logger.Info("Error Occured ! Error Message:" + error_message);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        private static bool GetNewPAT()
        {
            try
            {
                bool result = false;
                string transaction_xml,error_message;

                int response = haad_ws.GetNewPriorAuthorizationTransactions(HAAD.username, HAAD.password, out transaction_xml, out error_message);
                if(response == 0 )
                {
                    HAAD_GNPAT.GNPAT_xml_transactions = transaction_xml;
                    result = true;
                }
                else
                {
                    Logger.Info("Error Occured ! Error Message:" + error_message);
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
                string loc_filename,error_message,output_file;
                byte[] loc_filebyte;

                int response = haad_ws.DownloadTransactionFile(HAAD.username, HAAD.password, FileID, out loc_filename, out loc_filebyte, out error_message);
                if(response == 0)
                {
                    if (Path.GetExtension(loc_filename).ToString().ToLower() == ".zip")
                    {
                        Logger.Info("File is in ZIP format FileID: " + FileID);
                        output_file = zip_control((Convert.ToBase64String(loc_filebyte)), loc_filename, loc_filebyte);
                        if (output_file != "0")//Exception is 0
                        {
                            Logger.Info("File Zip extraction was Successfull");
                            result = true;
                        }
                        else
                        {
                            Logger.Info("File Zip extraction Failed for FileID: " + FileID);
                        }
                    }
                    else
                    {
                        output_file = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(loc_filebyte)));
                        result = true;
                    }

                    if (process == "HAAD_GNT")
                    {
                        HAAD_GNT.GNT_file_content = output_file;
                        HAAD_GNT.GNT_file_name = loc_filename;
                    }
                    else if (process == "HAAD_GNPAT")
                    {
                        HAAD_GNPAT.GNPAT_file_content = output_file;
                        HAAD_GNPAT.GNPAT_file_name = loc_filename;
                    }
                }
                else
                {
                    Logger.Info("Error Occured ! Error Message: " + error_message);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        private static bool StoreTransaction(string filecontent, string filename,string[] data)
        {
            try
            {
                bool result = false;

                if (!Directory.Exists(app_data + "\\HAAD\\"))
                {
                    Directory.CreateDirectory(app_data + "\\HAAD\\");
                }
                using (StreamWriter writer = new StreamWriter(app_data + "\\HAAD\\" + filename + Path.GetExtension(filename)))
                {
                    writer.Write(filecontent);
                   
                }
                using (StreamWriter writer = new StreamWriter(app_data + "\\HAAD\\" + filename + ".csv"))
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < data.Count(); i++)
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

        private static bool SetTransactions(string FileID)
        {
            try
            {
                bool result = false;
                string error_message;
                int response = haad_ws.SetTransactionDownloaded(HAAD.username, HAAD.password, FileID, out error_message);
                if(response == 0)
                {
                    Logger.Info("File set as downloaded Successful ! FileID: " + FileID);
                    result = true;
                }
                else
                {
                    Logger.Info("Error Occured ! Error Message: " + error_message);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        #endregion

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
                Logger.Info("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        #endregion
    }
}
