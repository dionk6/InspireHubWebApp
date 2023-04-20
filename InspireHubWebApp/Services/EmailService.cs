using InspireHubWebApp.DTOs;
using InspireHubWebApp.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace InspireHubWebApp.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEmailAsync(Application contact)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("Inspire Hub", "hello@inspirehub.info"));

            message.To.Add(MailboxAddress.Parse("dion.kuka@ubt-uni.net"));

            //message.ReplyTo.Add(MailboxAddress.Parse(contact.Email));

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
    }
}
