using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.SessionState;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Web;

namespace fetchtoapi.Controllers
{
    public class UMSuserController : ApiController
    {
        //
        // GET: /UMSuser/
        string strConnectionString = Properties.Settings.Default.LocalDB.ToString();

        [HttpGet]
        [Route("api/getuser")]
        [ActionName("getuser")]
        public String getuser()
        {
            try
            {
                if (HttpContext.Current.Session["UserId"] == null)
                    return "failed";
                else
                    return "succeed";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        [HttpPost]
        [Route("api/authenticateuser/{username}/{password}")]
        [ActionName("authenticateuser")]
        public String authenticateuser(String username, String password)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[usp_Authenticateuser]", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar, 50).Value = password;
                    SqlDataReader sqlDataReader = cmd.ExecuteReader();
                    String Status = "failed";
                    while (sqlDataReader.Read())
                    {
                        Status =  "succeed";
                        HttpContext.Current.Session["UserId"] = sqlDataReader.GetValue(0).ToString();
                    }
                    sqlConn.Close();
                    dict.Add("status", Status);
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
