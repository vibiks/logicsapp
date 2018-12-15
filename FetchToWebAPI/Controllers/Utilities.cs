using System.IO;
using System.Text;
using System.Data;
using System.Net;
using System.Net.Mail;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.UI;
using System;
using System.Web.Configuration;

namespace fetchtoapi.Controllers
{
    public class Utilities
    {
        public void SendPDFEmail(JobInfo job)
        {
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {

                    string companyName = "UMS";                   
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Order Sheet</b></td></tr>");
                    
                    sb.Append("<tr><td colspan = '2'><b>UMS Job Info</b> ");
                    sb.Append(companyName);
                    sb.Append("</td>" + companyName + "</tr>");

                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    
                    sb.Append("<tr><td><b>Job No:</b>");
                    sb.Append(job.JobNo);
                    sb.Append("</td><td><b>Job Date: </b>");
                    sb.Append(job.JobDate);
                    sb.Append(" </td></tr>");

                    sb.Append("<tr><td><b>Job Type:</b>");
                    sb.Append(job.JobType);
                    sb.Append("</td><td><b>CustomerCode: </b>");
                    sb.Append(job.Pacode);
                    sb.Append(" </td></tr>");

                    sb.Append("</table>");

                    StringReader sr = new StringReader(sb.ToString());

                    try
                    {
                        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    // Rectangle rec1 = new Rectangle(PageSize.A4);
                    // PdfDocument myPDF = new PdfDocument();
                    // myPDF.SetPageSize(rec1);
                    // myPDF.SetMargins(10,10,10,10);

                    
                    // HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

                    //using (MemoryStream memoryStream = new MemoryStream())
                    //{
                    //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    //    pdfDoc.Open();                        
                    //    htmlparser.Parse(sr);
                    //    pdfDoc.Close();
                    //    byte[] bytes = memoryStream.ToArray();
                    //    memoryStream.Close();

                    //    Attachment pdfAttachment = new Attachment(new MemoryStream(bytes), "UMS_Job_" + job.JobNo+ ".pdf");
                    //    pdfAttachment.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                        
                    //    MailMessage mm = new MailMessage();
                    //    mm.From = new MailAddress("kgs.sri@gmail.com");
                    //    mm.To.Add(new MailAddress("kgs.sri@gmail.com"));
                    //    mm.To.Add(new MailAddress(job.PaMailID));
                    //    mm.Subject = "UMS Job Details: JobNo - " + job.JobNo;
                    //    mm.Body = "Please find the attachment for the job:" + job.JobNo;


                    //    // old code...
                    //    /*
                    //    MailMessage mm = new MailMessage();
                    //    mm.From = new MailAddress("kgs.sri@gmail.com");
                    //    mm.To.Add(new MailAddress("kgs.sri@gmail.com"));
                    //    mm.Subject = "UMS Job Details: JobNo - " + job.JobNo;
                    //    mm.Body = "Please find the attachment for the job:" + job.JobNo;
                    //    mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "UMSJob_" + job.JobNo + ".pdf"));
                    //    //mm.IsBodyHtml = true;
                    //    SmtpClient smtp = new SmtpClient();
                    //    smtp.Host =  Properties.Settings.Default.smtp.ToString();
                    //    smtp.EnableSsl = true;
                    //    NetworkCredential NetworkCred = new NetworkCredential();
                    //    NetworkCred.UserName = Properties.Settings.Default.networkusername;
                    //    NetworkCred.Password = Properties.Settings.Default.networkpassword;
                    //    smtp.UseDefaultCredentials = true;
                    //    smtp.Credentials = NetworkCred;
                    //    smtp.Port = 587;
                    //    smtp.Send(mm);
                    //    */


                    //    //New code..
                    //    mm.Attachments.Add(pdfAttachment);
                    //    SmtpClient smtp = new SmtpClient();
                    //    smtp.Host =  Properties.Settings.Default.smtp.ToString();
                    //    smtp.EnableSsl = true;
                    //    smtp.Port = 587;
                    //    // smtp.Port = 465;
                    //    smtp.UseDefaultCredentials = false;
                    //    NetworkCredential NetworkCred = new NetworkCredential();
                    //    NetworkCred.UserName = Properties.Settings.Default.networkusername;
                    //    NetworkCred.Password = Properties.Settings.Default.networkpassword;                        
                    //    smtp.Credentials = NetworkCred;
                        
                    //    smtp.Send(mm);
                    //}
                }
            }
        }
    }
}