using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using fetchtoapi.Models;
// using Newtonsoft.Json.Linq;


namespace fetchtoapi.Controllers
{
    public class UMSlistController : ApiController
    {
        //
        // GET: /UMSlist/
        string strConnectionString = Properties.Settings.Default.LocalDB.ToString();

        [HttpGet]
        [Route("api/getentries")]
        [ActionName("getentries")]
        public string getentries()
        {
            ArrayList entlist = new ArrayList();
            using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[usp_GetallEntries]", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader sqlDataReader = cmd.ExecuteReader();
                    entrydetail obj;
                    while (sqlDataReader.Read())
                    {
                        obj = new entrydetail();
                        obj.entryno = sqlDataReader.GetValue(0).ToString();
                        obj.entrydate = sqlDataReader.GetValue(1).ToString();
                        obj.customcode = sqlDataReader.GetValue(2).ToString();                        
                        obj.customername = sqlDataReader.GetValue(4).ToString();
                        obj.JobType = sqlDataReader.GetValue(3).ToString();
                        obj.PaDcNo = sqlDataReader.GetValue(5).ToString();
                        obj.PaDcDate = sqlDataReader.GetValue(6).ToString();
                        obj.PaStatus = sqlDataReader.GetValue(7).ToString();
                        entlist.Add(obj);
                    }
                    sqlConn.Close();
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }

            return JsonConvert.SerializeObject(entlist);
        }


        [Route("api/updatejobdetails")]
        [ActionName("updatejobdetails")]
        public string UpdateJobDetails(List<JobDetailDto> collection)
        {   
            DataTable dataTable = new DataTable("JobDetail");

            dataTable.Columns.Add("PaCode", typeof(string));
            dataTable.Columns.Add("PaStatus", typeof(string));

            foreach (JobDetailDto detail in collection)
            {
                dataTable.Rows.Add(detail.JobNo, detail.Status);
            }

            using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[usp_UpdateJobStatus]", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter sqlParam = cmd.Parameters.AddWithValue("@inputdata", dataTable);
                    sqlParam.SqlDbType = SqlDbType.Structured;
                    int updatedRows = cmd.ExecuteNonQuery();
                    //SqlDataReader sqlDataReader = cmd.ExecuteReader();
                    // sqlDataReader.GetValue(0);
                    sqlConn.Close();
                    if (updatedRows > 0)
                    {
                        return "Records Updated Successfully";
                    }

                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }

            return "Update Failed";
        }

        [HttpGet]
        [Route("api/deletejobdetail")]
        [ActionName("deletejobdetail")]
        public string DeleteJobDetail(int jobNo)
        {
            using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[usp_DeleteJob]", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter sqlParam = cmd.Parameters.AddWithValue("@jobno", jobNo);
                    sqlParam.SqlDbType = SqlDbType.Int;
                    int updatedRows = cmd.ExecuteNonQuery();
                    sqlConn.Close();
                    if (updatedRows > 0)
                    {
                        return "Job Deleted Successfully";
                    }

                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }

            return "Update Failed";
        }

        //    [HttpPost]
        //    [Route("api/UpdateJobDetails")]
        //    [ActionName("UpdateJobDetails")]
        //    public string UpdateJobDetails()
        //    {
        //        // dynamic stuff = JsonConvert.DeserializeObject(frmdata);
        //        //string hi = System.Web.HttpContext.Current.Request.ToString();
        //        // JObject bud =  JObject.Parse(hi);

        //        // var dict = Request.Form["ListViewTable"].ToDictionary(x => x.Key, x => x.Value.ToString());
        //        jsonObj myobj = new jsonObj();
        //        myobj = System.Web.HttpContext.Current.Request.Form["ListViewTable"];


        //        JobInfo job = new JobInfo();
        //        Utilities utils = new Utilities();
        //        using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
        //        {
        //            try
        //            {
        //                //foreach (String str in frmdata.Split('$'))
        //                //{
        //                //    String[] strarr = str.Split('=');
        //                //    if (strarr.Length > 0)
        //                //    {
        //                //        if (strarr[0].Trim().ToLower() == "entryno")
        //                //        {
        //                //            job.JobNo = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "work_progress")
        //                //        {
        //                //            job.JobType = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "entry_date")
        //                //        {
        //                //            //job.JobDate = DMYTOMDY(strarr[1] == null ? "" : strarr[1].Trim());
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "dc_1_no")
        //                //        {
        //                //            job.PaDcNo = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "dc_date")
        //                //        {
        //                //            // job.PaDcDate = DMYTOMDY(strarr[1] == null ? "" : strarr[1].Trim());

        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "customer_name1")
        //                //        {
        //                //            job.Pacode = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "customer_mail_id")
        //                //        {
        //                //            job.PaMailID = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "gst_no")
        //                //        {
        //                //            job.GSTNo = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "prepared_by")
        //                //        {
        //                //            job.Preparedby = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "remarks")
        //                //        {
        //                //            job.Remarks = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "photo_upload_status")
        //                //        {
        //                //            job.Imagestatus = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "inspection_status")
        //                //        {
        //                //            job.InsStatus = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //        else if (strarr[0].Trim().ToLower() == "macdata")
        //                //        {
        //                //            job.MacDetail = strarr[1] == null ? "" : strarr[1].Trim();
        //                //        }
        //                //    }

        //                //}
        //                sqlConn.Open();
        //                SqlCommand cmd = new SqlCommand("[dbo].[usp_InsertMainJob]", sqlConn);
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add("@JobNo", SqlDbType.VarChar, 50).Value = job.JobNo;
        //                cmd.Parameters.Add("@JobDate", SqlDbType.VarChar, 50).Value = job.JobDate;
        //                cmd.Parameters.Add("@JobType", SqlDbType.VarChar, 50).Value = job.JobType;
        //                cmd.Parameters.Add("@Pacode", SqlDbType.VarChar, 50).Value = job.Pacode;
        //                cmd.Parameters.Add("@PaDcNo", SqlDbType.VarChar, 50).Value = job.PaDcNo;
        //                cmd.Parameters.Add("@PaDcDate", SqlDbType.VarChar, 50).Value = job.PaDcDate;
        //                cmd.Parameters.Add("@PaMailID", SqlDbType.VarChar, 50).Value = job.PaMailID;
        //                cmd.Parameters.Add("@Imagestatus", SqlDbType.VarChar, 50).Value = job.Imagestatus;
        //                cmd.Parameters.Add("@Image", SqlDbType.VarChar, 50).Value = job.Image;
        //                cmd.Parameters.Add("@Remarks", SqlDbType.VarChar, 50).Value = job.Remarks;
        //                cmd.Parameters.Add("@GSTNo", SqlDbType.VarChar, 50).Value = job.GSTNo;
        //                cmd.Parameters.Add("@Preparedby", SqlDbType.VarChar, 50).Value = job.Preparedby;

        //                SqlDataReader sqlDataReader = cmd.ExecuteReader();
        //                while (sqlDataReader.Read())
        //                {
        //                    if (job.JobNo == "")
        //                        job.JobNo = sqlDataReader.GetValue(0).ToString();
        //                }
        //                sqlDataReader.Close();
        //                sqlConn.Close();
        //                // dict.Add("key", "Job Successfully Submitted. JobId:" + job.JobNo);

        //            }
        //            catch (Exception ex)
        //            {
        //                // dict.Add("Error", "Error:" + ex.InnerException.Message.ToString());
        //                return JsonConvert.SerializeObject("");
        //            }
        //        }

        //        return JsonConvert.SerializeObject("");
        //    }
        //}

        //public class jsonObj
        //{
        //    public string EntryNo { get; set; }
        //    public string Status { get; set; }
        //}
    }
}