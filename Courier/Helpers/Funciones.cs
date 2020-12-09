using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace Courier.Helpers
{
    public class Funciones
    {
     
        public int EnviarCorreo(string oTo, string oAsunto, string oMensaje)
        {
            var fromAddress = new MailAddress("francisco.rojas@celsouth.cl", "Francisco Rojas");
            var toAddress = new MailAddress(oTo, oTo);
            const string fromPassword = "hornblower";
            string  subject;
            string body = oMensaje;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,                
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };



            subject = oMensaje;

            


            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,               
                IsBodyHtml=true,
                Body= subject
            })

            {
                try
                {
                    smtp.Send(message);
                    return 1;//OK
                }
                catch
                {
                    return 0;//ERROR
                }
            }
            
        }
    }
}