using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Mail;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Web.UI;
using System.Text;
using System.Web;
using System.Web.Http.Results;

namespace fetchtoapi.Controllers
{
    public class UMSMachineController : ApiController
    {
        string strConnectionString = Properties.Settings.Default.LocalDB.ToString();

        [HttpGet]
        [Route("api/getMachineDetails")]
        [ActionName("getMachineDetails")]
        public string getMachineDetails()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[usp_GetMachineDetails]", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader sqlDataReader = cmd.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        dict.Add(sqlDataReader.GetValue(0).ToString(), sqlDataReader.GetValue(1).ToString());
                    }
                    sqlConn.Close();

                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }

            return JsonConvert.SerializeObject(dict);
        }
        
        [HttpGet]
        [Route("api/getJobDetails/{frmdata}")]
        [ActionName("getJobDetails")]
        public string getJobDetails(string frmdata)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            JobInfo job = new JobInfo();
            Utilities utils = new Utilities();
            using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
            {
                try
                {
                    
                    foreach(String str in frmdata.Split('$'))
                    {
                        String[] strarr = str.Split('=');
                        if (strarr.Length > 0)
                        {
                            if (strarr[0].Trim().ToLower() == "entryno")
                            {
                                job.JobNo = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "work_progress")
                            {
                                job.JobType = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "entry_date")
                            {
                                job.JobDate = DMYTOMDY(strarr[1] == null ? "" : strarr[1].Trim());                                
                            }
                            else if (strarr[0].Trim().ToLower() == "dc_1_no")
                            {
                                job.PaDcNo = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "dc_date")
                            {
                                job.PaDcDate = DMYTOMDY(strarr[1] == null ? "" : strarr[1].Trim());
                                
                            }
                            else if (strarr[0].Trim().ToLower() == "customer_name1")
                            {
                                string[] code = new string[10];
                                code = strarr[1].Split('~');
                                job.Pacode = strarr[1] == null ? "" : code[0];
                            }
                            else if (strarr[0].Trim().ToLower() == "customer_mail_id")
                            {
                                job.PaMailID = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "gst_no")
                            {
                                job.GSTNo = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "prepared_by")
                            {
                                job.Preparedby = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "completed_by")
                            {
                                job.Completedby = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "remarks")
                            {
                                job.Remarks = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "photo_upload_status")
                            {
                                job.Imagestatus = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "inspection_status")
                            {
                                job.InsStatus = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "macdata")
                            {
                                job.MacDetail = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                            else if (strarr[0].Trim().ToLower() == "imgpath")
                            {
                                job.Image = strarr[1] == null ? "" : strarr[1].Trim();
                            }
                        }

                    }
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[usp_InsertMainJob]", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@JobNo", SqlDbType.VarChar, 50).Value = job.JobNo;
                    cmd.Parameters.Add("@JobDate", SqlDbType.VarChar, 50).Value = job.JobDate;
                    cmd.Parameters.Add("@JobType", SqlDbType.VarChar, 50).Value = job.JobType;
                    cmd.Parameters.Add("@Pacode", SqlDbType.VarChar, 50).Value = job.Pacode;
                    cmd.Parameters.Add("@PaDcNo", SqlDbType.VarChar, 50).Value = job.PaDcNo;
                    cmd.Parameters.Add("@PaDcDate", SqlDbType.VarChar, 50).Value = job.PaDcDate;
                    cmd.Parameters.Add("@PaMailID", SqlDbType.VarChar, 50).Value = job.PaMailID;
                    cmd.Parameters.Add("@Imagestatus", SqlDbType.VarChar, 50).Value = job.Imagestatus;
                    cmd.Parameters.Add("@Image", SqlDbType.VarChar, 256).Value = job.Image;
                    cmd.Parameters.Add("@Remarks", SqlDbType.VarChar, 50).Value = job.Remarks;
                    cmd.Parameters.Add("@GSTNo", SqlDbType.VarChar, 50).Value = job.GSTNo;
                    cmd.Parameters.Add("@Preparedby", SqlDbType.VarChar, 50).Value = job.Preparedby;
                    cmd.Parameters.Add("@Completedby", SqlDbType.VarChar, 50).Value = job.Completedby;
                    cmd.Parameters.Add("@InsStatus", SqlDbType.VarChar, 50).Value = job.InsStatus;

                    SqlDataReader sqlDataReader = cmd.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        if(job.JobNo == "")
                        job.JobNo = sqlDataReader.GetValue(0).ToString();
                    }
                    sqlDataReader.Close();
                    bool isIdExist = true;
                    int id = 1;
                    while (isIdExist)
                    {
                        String Prcode = "";
                        String PrName = "";
                        String PrDrawing = "";
                        String Narration = "";
                        String JobStartTime = "";
                        String JobEndTime = "";
                        
                        String strjobname = "job_" + id + "_no";
                        if (frmdata.IndexOf(strjobname) >= 0)
                        {
                            foreach (String str in frmdata.Split('$'))
                            {
                                String[] strarr = str.Split('=');
                                if (strarr.Length > 0)
                                {
                                    if (strarr[0].Trim().ToLower() == strjobname)
                                    {
                                        Prcode = strarr[1] == null ? "" : strarr[1].Trim();
                                    }
                                    else if (strarr[0].Trim().ToLower() == ("job_" + id + "_name"))
                                    {
                                        PrName = strarr[1] == null ? "" : strarr[1].Trim();
                                    }
                                    else if (strarr[0].Trim().ToLower() == ("drawing_" + id + "_details"))
                                    {
                                        PrDrawing = strarr[1] == null ? "" : strarr[1].Trim();
                                    }
                                    else if (strarr[0].Trim().ToLower() == ("work_" + id + "_details"))
                                    {
                                        Narration = strarr[1] == null ? "" : strarr[1].Trim();
                                    }
                                    else if (strarr[0].Trim().ToLower() == ("job_" + id + "_starttime"))
                                    {
                                        JobStartTime = strarr[1] == null ? "" : strarr[1].Trim();
                                    }
                                    else if (strarr[0].Trim().ToLower() == ("job_" + id + "_endtime"))
                                    {
                                        JobEndTime = strarr[1] == null ? "" : strarr[1].Trim();
                                    }
                                }
                            }
                            cmd = new SqlCommand("[dbo].[usp_InsertSubJob]", sqlConn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@JobNo", SqlDbType.Int, 50).Value = Convert.ToInt16(job.JobNo);
                            cmd.Parameters.Add("@Prcode", SqlDbType.VarChar, 150).Value = Prcode;
                            cmd.Parameters.Add("@PrName", SqlDbType.VarChar, 150).Value = PrName;
                            cmd.Parameters.Add("@PrDrawing", SqlDbType.VarChar, 250).Value = PrDrawing;
                            cmd.Parameters.Add("@Narration", SqlDbType.VarChar, 250).Value = Narration;
                            cmd.Parameters.Add("@InsStatus", SqlDbType.VarChar, 50).Value = job.InsStatus;
                            cmd.Parameters.Add("@JobStarttime", SqlDbType.VarChar, 50).Value = JobStartTime;
                            cmd.Parameters.Add("@JobEndtime", SqlDbType.VarChar, 50).Value = JobEndTime;
                            sqlDataReader = cmd.ExecuteReader();
                            sqlDataReader.Close();
                            id++;
                        }
                        else
                        {
                            isIdExist = false;
                        }
                    }
                    if (job.MacDetail != null && job.MacDetail != "")
                    {
                        foreach (String str in job.MacDetail.Split('^'))
                        {
                            if (str != null && str != "")
                            {

                                if (str.Split('~').Length > 0)
                                {
                                    cmd = new SqlCommand("[dbo].[usp_InsertMachineDetails]", sqlConn);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add("@JobNo", SqlDbType.Int, 50).Value = Convert.ToInt16(job.JobNo);
                                    cmd.Parameters.Add("@Prcode", SqlDbType.VarChar, 150).Value = str.Split('~')[0];
                                    cmd.Parameters.Add("@Machine", SqlDbType.VarChar, 150).Value = str.Split('~')[1];
                                    sqlDataReader = cmd.ExecuteReader();
                                    sqlDataReader.Close();
                                }
                            }
                        }
                    }
                    sqlConn.Close();

                    try {
                        // utils.SendPDFEmail(job);
                        sendmail1(job);
                    }
                    catch (Exception ex) { 
                        throw; }                   
                    
                    dict.Add("key", "Job Successfully Submitted. JobId:" + job.JobNo);

                }
                catch (Exception ex)
                {
                    dict.Add("Error", "Error:" + ex.InnerException.Message.ToString());
                    return JsonConvert.SerializeObject(dict);
                }
            }

            return JsonConvert.SerializeObject(dict);
        }

        private string DMYTOMDY(string data_dt)
        {
            string strTemp = "";
            if (data_dt!= null && data_dt != "")
            {
                strTemp = data_dt.Split('/')[1] + "/" + data_dt.Split('/')[0] + "/" + data_dt.Split('/')[2];
            }
            return strTemp;
        }

        private void sendmail1(JobInfo job)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
            sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Job Info:" + job.JobNo + "</b></td></tr>");

            sb.Append("<tr><td><b>Job Type</b> ");
            sb.Append(job.JobType);
            sb.Append("</td><td><b>Job Date</b>");          
            sb.Append(job.JobDate);
            sb.Append("</td></tr>");


            sb.Append("<tr><td><b>DC No</b> ");
            sb.Append(job.PaDcNo);
            sb.Append("</td><td><b>DC Date</b>");
            sb.Append(job.PaDcDate);
            sb.Append("</td></tr>");

            sb.Append("<tr><td><b>Inspection Status</b> ");
            sb.Append(job.InsStatus);
            sb.Append("</td><td><b>Remarks</b>");
            sb.Append(job.Remarks);
            sb.Append("</td></tr>");
            sb.Append("</table>");

            MailMessage mm = new MailMessage();
            mm.From = new MailAddress("supremesofts@gmail.com");            
            mm.To.Add(new MailAddress(job.PaMailID));
            mm.Subject = "UMS Job Details: JobNo - " + job.JobNo;
            mm.Body = sb.ToString();
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            // smtp.Host = Properties.Settings.Default.smtp.ToString();
            //smtp.EnableSsl = true;
            //smtp.Port = 587;
            //// smtp.Port = 465;
            //smtp.UseDefaultCredentials = false;
            //NetworkCredential NetworkCred = new NetworkCredential();
            //NetworkCred.UserName = Properties.Settings.Default.networkusername;
            //NetworkCred.Password = Properties.Settings.Default.networkpassword;
            //smtp.Credentials = NetworkCred;

            smtp.Send(mm);
        }

        [HttpPost]
        [Route("api/SaveImage/")]
        [ActionName("SaveImage")]
        public string SaveImage()
        {
            string savedFilePath = string.Empty;
            try
            {
                var uploadImage = HttpContext.Current.Request.Files[0];
                // var fileName = Path.GetFileName(uploadImage.FileName);
                var ext = HttpContext.Current.Request.Files[0].FileName.ToString();
                var arr1 = ext.Split('.');
                if (arr1.Length > 0)
                    ext = arr1[(arr1.Length - 1)];                
                var fileName = HttpContext.Current.Request.Params["ImageName"] + "." + ext.ToString();                
                bool folderExists = Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadedImages"));                
                if (!folderExists)
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadedImages"));                
                string path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/UploadedImages"), fileName);
                (HttpContext.Current.Request.Files[0]).SaveAs(path);
                
                savedFilePath = path;
            }
            catch (Exception ex)
            {                
                //throw;
            }

            return savedFilePath;
        }
    }
}
