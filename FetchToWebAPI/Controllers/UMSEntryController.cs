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

namespace fetchtoapi.Controllers
{
    public class UMSEntryController : ApiController
    {
        //
        // GET: /UMSEntry/

        [HttpGet]
        [Route("api/getentryinfo/{entryid}")]
        [ActionName("getentryinfo")]
        public string getentryinfo(string entryid)
        {
            string strConnectionString = Properties.Settings.Default.LocalDB.ToString();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[usp_Getentryinfo]", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@jobcode", SqlDbType.VarChar, 50).Value = entryid;
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            dict.Add(".entry_date", ds.Tables[0].Rows[0][0].ToString().Trim());
                            dict.Add(".radio-btn", ds.Tables[0].Rows[0][1].ToString().Trim());
                            dict.Add(".customerName", ds.Tables[0].Rows[0][2].ToString().Trim());
                            dict.Add(".dc_1_no", ds.Tables[0].Rows[0][3].ToString().Trim());
                            dict.Add(".dc_date", ds.Tables[0].Rows[0][4].ToString().Trim());
                            dict.Add(".customer_mail_id", ds.Tables[0].Rows[0][5].ToString().Trim());
                            dict.Add(".imagestatus", ds.Tables[0].Rows[0][6].ToString().Trim());
                            dict.Add(".remarks", ds.Tables[0].Rows[0][7].ToString().Trim());
                            dict.Add(".preparedby", ds.Tables[0].Rows[0][8].ToString().Trim());
                            dict.Add(".completedby", ds.Tables[0].Rows[0][9].ToString().Trim());
                        }
                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            dict.Add("jobcnt", ds.Tables[1].Rows.Count.ToString());
                            int i = 1;
                            foreach (DataRow row in ds.Tables[1].Rows)
                            {
                                dict.Add("prcode" + i, row[0].ToString().Trim());
                                dict.Add("prname" + i, row[1].ToString());
                                dict.Add("prdrawing" + i, row[2].ToString());
                                dict.Add("narration" + i, row[3].ToString());
                                dict.Add("insstatus" + i, row[4].ToString());
                                dict.Add("jobstarttime" + i, row[5].ToString());
                                dict.Add("jobendtime" + i, row[6].ToString());
                                String strmachinecode = "";
                                if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                                {
                                    foreach (DataRow row1 in ds.Tables[2].Select(String.Format("prcode='{0}' ", row[0].ToString())))
                                    {
                                        strmachinecode += row1[1].ToString() + ",";
                                    }
                                }
                                dict.Add("machinecode" + i, strmachinecode);
                                i++;
                            }
                        }
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


    }
}
