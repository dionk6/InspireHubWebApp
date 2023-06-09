using InspireHubWebApp.DTOs;
using InspireHubWebApp.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Mime;

namespace InspireHubWebApp.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEmailAsync(Application contact)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("Inspire Hub", "hello@inspirehub.info"));

            message.To.Add(MailboxAddress.Parse("hello@inspirehub.info"));

            message.ReplyTo.Add(MailboxAddress.Parse(contact.Email));

            message.Subject = "Inspire Hub - "+contact.CourseTitle;
            //message.HtmlBody = contact.Message;

            message.Body = new TextPart("html")
            {
                Text = contact.Message
            };

            SmtpClient client = new SmtpClient();

            try
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("hello@inspirehub.info", "vyypdulfolbenxfc");
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }

            return true;
        }

        public async Task<bool> SendMessageAsync(string emails, string msg, IFormFile attach)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("Inspire Hub", "hello@inspirehub.info"));

            var listTo = new List<MailboxAddress>();
            var objEmails = emails.Split(",");
            foreach (var item in objEmails)
            {
                var obj = MailboxAddress.Parse(item.Trim());
                listTo.Add(obj);
            }

            message.To.AddRange(listTo);

            message.ReplyTo.Add(MailboxAddress.Parse("hello@inspirehub.info"));

            message.Subject = "Inspire Hub - Message";
            //message.HtmlBody = contact.Message;

            /*message.Body = new TextPart("html")
            {
                Text = msg
            };*/

            var builder = new BodyBuilder();
            byte[] fileBytes;
            if(attach != null)
            {
                var file = attach;
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    builder.Attachments.Add(file.FileName, fileBytes, MimeKit.ContentType.Parse(attach.ContentType));
                }
            }
            builder.HtmlBody = msg;
            message.Body = builder.ToMessageBody();


            SmtpClient client = new SmtpClient();

            try
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("hello@inspirehub.info", "vyypdulfolbenxfc");
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }

            return true;
        }
    }
}
