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
    public static class NE_Start
    {

        #region Variables
        public static PAYER_NE_WS.PayerIntegrationWSClient ne_ws = new PAYER_NE_WS.PayerIntegrationWSClient();
        public static string app_data = ConfigurationManager.AppSettings["app_data"];
        public static List<NE_GNC_list> NE_GNC_List = new List<NE_GNC_list>();
        public static List<NE_GNPR_list> NE_GNPR_List = new List<NE_GNPR_list>();
        #endregion

        public static bool GNC_Control()
        {
            try
            {
                bool result = false;
                Logger.Info("Process started for NE - Payer Service");
                if(WS_Credentials.get_credentials("NE_Payer", app_data))
                {
                    Logger.Info("Getting New Claims");
                    if (GetNewClaims())
                    {
                        List<bool> check = new List<bool>();
                        for (int i = 0; i < NE_GNC_list.NE_GNC_CS.Count(); i++)
                        {
                            XmlDocument xdoc = new XmlDocument();
                            string xml = NE_GNC_list.NE_GNC_CS[i].ToString();
                            string filename = NE_GNC_list.NE_GNC_Filename[i].ToString();
                            string[] data = { NE_GNC.GNC_BatchID, NE_GNC.GNC_BatchDateTime };
                            if (StoreTransaction(filename, xml, data))
                            {
                                check.Add(true);
                            }
                            else
                            {
                                check.Add(false);
                            }
                        }
                        if (!check.Contains(false))
                        {
                            //ConfirmBatchReceived(NE_GNC.GNC_BatchID, Convert.ToInt32(NE_GNC.GNC_TransactionType));
                        }
                    }
                    else
                    {
                        Logger.Info("Problem Occured while executing Function GetNewClaims ");
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
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        public static bool GNPR_Control()
        {
            try
            {
                bool result = false;
                
                Logger.Info("Process started for NE - Payer Service");
                if (WS_Credentials.get_credentials("NE_Payer", app_data))
                {
                    if (GetNewPR())
                    {
                        List<bool> check = new List<bool>();
                        for (int i = 0; i < NE_GNPR_list.NE_GNPR_CS.Count(); i++)
                        {
                            XmlDocument xdoc = new XmlDocument();
                            string xml = NE_GNPR_list.NE_GNPR_CS[i].ToString();
                            string filename = NE_GNPR_list.NE_GNPR_Filename[i].ToString();
                            string[] data = { NE_GNPR.GNPR_BatchID, NE_GNPR.GNPR_BatchDateTime };
                            if (StoreTransaction(filename, xml, data))
                            {
                                check.Add(true);
                            }
                            else
                            {
                                check.Add(false);
                            }
                        }
                        if (!check.Contains(false))
                        {
                            //ConfirmBatchReceived(NE_GNC.GNC_BatchID, Convert.ToInt32(NE_GNC.GNC_TransactionType));
                        }
                    }
                    else
                    {
                        Logger.Info("Problem Occured while executing Function GetNewClaims ");
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
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        public static bool UT_Control()
        {
            try
            {
                bool result = false;
                string filename, filepath,dummy;

                string[] files_path = Directory.GetFiles(app_data + "\\NEmirates_Upload\\", "*.xml");
                if (files_path.Count() > 0)
                {
                    PAYER_NE_WS.uploadClaimSubmissionClaimSubmissionHeader c_header = null;
                    PAYER_NE_WS.uploadClaimSubmissionClaimSubmissionClaim[] c_claim_array = null;
                    PAYER_NE_WS.uploadClaimSubmissionClaimSubmissionClaim c_claim = null;
                    PAYER_NE_WS.uploadClaimSubmissionClaimSubmission upload_claim = new PAYER_NE_WS.uploadClaimSubmissionClaimSubmission();
                    PAYER_NE_WS.feedbackResult fresult = new PAYER_NE_WS.feedbackResult();
                    Logger.Info("Number of files to be uploaded " + files_path.Count());
                    foreach (string file in files_path)
                    {
                        filename = Path.GetFileName(file);
                        filepath = Path.GetFullPath(file);
                        Logger.Info("Uploading File: " + filename);

                        upload_claim.Claim = c_claim_array;
                        upload_claim.Header = c_header;

                        fresult =  ne_ws.uploadClaimSubmission(upload_claim, filename, NE_Payer.payer_license, NE_Payer.payer_key);
                        if(fresult.value == 0)
                        {
                            dummy = fresult.errorDetails;
                            dummy = fresult.MSG;
                            dummy = Convert.ToString(fresult.value);
                            dummy = Convert.ToString(fresult.valueSpecified);
                            dummy = fresult.XmlFileName;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        #region NE Methods

        private static bool GetNewClaims()
        {
            try
            {
                bool result = false;
                PAYER_NE_WS.claimSubmissionBatch loc_Claim_Batch_Result = new PAYER_NE_WS.claimSubmissionBatch();
                PAYER_NE_WS.pbmClaimSubmission[] loc_Claim_Array = null;
                
                loc_Claim_Batch_Result =  ne_ws.getNewClaims(NE_Payer.payer_license, NE_Payer.payer_key, Convert.ToInt32(NE_Payer.batchsize), Convert.ToInt32(NE_Payer.region));
                NE_GNC.GNC_BatchID = loc_Claim_Batch_Result.batchID;
                NE_GNC.GNC_BatchDateTime = Convert.ToString(loc_Claim_Batch_Result.batchDateTime);
                loc_Claim_Array = loc_Claim_Batch_Result.pbmClaimsList;
                for (int i = 0; i < loc_Claim_Array.Count(); i++)
                {
                    NE_GNC_list.NE_GNC_CS = loc_Claim_Array[i].claimSubmission.ToString();
                    NE_GNC_list.NE_GNC_Filename = loc_Claim_Array[i].fileName.ToString();
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        private static bool GetNewPR()
        {
            try
            {
                bool result = false;
                PAYER_NE_WS.priorRequestBatch loc_PR_Batch_Result = new PAYER_NE_WS.priorRequestBatch();
                PAYER_NE_WS.pbmPriorRequest[] loc_batchArray = null;

                loc_PR_Batch_Result = ne_ws.getNewPriorRequests(NE_Payer.payer_license, NE_Payer.payer_key, Convert.ToInt32(NE_Payer.batchsize), Convert.ToInt32(NE_Payer.region));
                NE_GNPR.GNPR_BatchID = loc_PR_Batch_Result.batchID;
                loc_batchArray = loc_PR_Batch_Result.requestsList;
                NE_GNPR.GNPR_BatchDateTime = Convert.ToString(loc_PR_Batch_Result.batchDateTime);
                for (int i = 0; i < loc_batchArray.Count(); i++) 
                {
                    NE_GNPR_list.NE_GNPR_Filename = loc_batchArray[i].fileName.ToString();
                    NE_GNPR_list.NE_GNPR_CS = loc_batchArray[i].priorRequest.ToString();
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        private static bool ConfirmBatchReceived(string BatchID,int TransactionType)
        {
            try
            {
                bool result = false;

                if(ne_ws.confirmBatchReceived(NE_Payer.payer_license, NE_Payer.payer_key, BatchID, TransactionType))
                {
                    result = true;
                }
                else
                {
                    Logger.Info("Error Occured !");
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Occured ! " + ex.Message);
                return false;
            }
        }

        private static bool StoreTransaction(string filename,string filecontent,string[] data)
        {
            try
            {
                bool result = false;

                if (!Directory.Exists(app_data + "\\NEmirates\\"))
                {
                    Directory.CreateDirectory(app_data + "\\NEmirates\\");
                }
                using (StreamWriter writer = new StreamWriter(app_data + "\\NEmirates\\" + filename + Path.GetExtension(filename)))
                {
                    writer.Write(filecontent);
                }

                using (StreamWriter writer = new StreamWriter(app_data + "\\NEmirates\\" + filename + ".csv"))
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

        #endregion
    }

}
