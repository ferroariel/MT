using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace MT.Models
{
    public class Mail
    {
        public int doMail(string eSubject, string eTo, string eBody)
        {
            int res = 0;
            MailAddress mailAddress = new MailAddress(eTo);
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(ConfigurationManager.AppSettings["MT_sitio"] + "<" + ConfigurationManager.AppSettings["MT_senderemail"] + ">"),
                Subject = eSubject,
                IsBodyHtml = false,
                Body = eBody
            };
            mailMessage.To.Add(mailAddress);
            SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["MT_smptServer"]);
            try
            {
                smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MT_smptUser"], ConfigurationManager.AppSettings["MT_smptPass"]);
                smtpClient.Send(mailMessage);
            }
            catch (SmtpException smtpExc)
            {
                string b = smtpExc.Message;
                res = -1;
            }
            catch (Exception ex)
            {
                string c = ex.Message;
                res = -2;
            }
            finally
            {
                smtpClient = null;
                mailAddress = null;
                mailMessage = null;
            }
            return res;
        }
    }
}