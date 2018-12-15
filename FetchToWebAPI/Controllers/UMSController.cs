using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace fetchtoapi.Controllers
{
    public class UMSController : ApiController
    {
        // Global Variables..
        string strConnectionString = Properties.Settings.Default.LocalDB.ToString();

        //[HttpGet]
        //public string getString()
        //{
        //    return "Hello world buddy";
        //}

        [HttpGet]
        [Route("api/getContactDetails")]
        [ActionName("getContactDetails")]  
        public string getContactDetails()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[usp_GetPartyContacts]", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader sqlDataReader = cmd.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        dict.Add(sqlDataReader.GetValue(0).ToString(), sqlDataReader.GetValue(2).ToString());
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
        [Route("api/getContactDetail/{strPartyCode}")]
        [ActionName("getContactDetail")]  
        public string getContactDetail(string strPartyCode)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[usp_GetPartyContact]", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@strPaCode", SqlDbType.VarChar, 50).Value = strPartyCode;
                    SqlDataReader sqlDataReader = cmd.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        dict.Add(sqlDataReader.GetValue(2).ToString(), sqlDataReader.GetValue(7).ToString());
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
        [Route("api/getcustomerautocomplete/{prefix}")]
        [ActionName("getcustomerautocomplete")]
        public string getcustomerautocomplete(string prefix)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (SqlConnection sqlConn = new SqlConnection(strConnectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[usp_GetCustomerByPrefix]", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@prefix", SqlDbType.VarChar, 50).Value = prefix;
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

    }
}
