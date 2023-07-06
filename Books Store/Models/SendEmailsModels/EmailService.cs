using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
//using System.Net.Mail;

namespace Books_Store.Models.SendEmailsModels
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public EmailService(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }






        //1-
        //This method sends an email using the Simple Mail Transfer Protocol (SMTP) to a specified email address.
        //public async Task SendEmailAsync2(string emailAddressOfTheRecipient, string subjectOfThEmail, string htmlMessageBody)
        //{
        //    //2-
        //    //The method creates a new instance of the SmtpClient class, which is used to send email messages
        //    //using the SMTP protocol.
        //    using (var client = new System.Net.Mail.SmtpClient(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort))
        //    {
        //        // set to false, because we want to specify our own email credentials.
        //        client.UseDefaultCredentials = false;

        //        //The Credentials property is set to a new instance of the NetworkCredential class,
        //        //which represents the username and password that are used to authenticate with the SMTP server.
        //        client.Credentials = new NetworkCredential(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

        //        //The Host and Port properties of the SmtpClient instance are set to the values specified in
        //        //the EmailConfiguration object.These values determine the SMTP server that will be used to send the email.
        //        client.Host = _emailConfiguration.SmtpServer;
        //        client.Port = _emailConfiguration.SmtpPort;

        //        //we use SSL encryption to authenticate with the SMTP server.The EnableSsl property
        //        //of the SmtpClient object is set to true, which tells the SmtpClient object to use secure SSL/TLS  encryption when
        //        //connecting to the SMTP server.When SSL encryption is used, your email address and password are encrypted
        //        //before they are sent over the internet, making it much more difficult for hackers to intercept and read them.
        //        client.EnableSsl = true;//although it is ssl we us port of tls




        //        //3-
        //        //Next, the method creates a new instance of the MailMessage class, which represents the
        //        //email message that will be sent.

        //        using (var message = new MailMessage())
        //        {
        //            //Add the email address of the recipient.
        //            message.To.Add(emailAddressOfTheRecipient);
        //            message.Subject = subjectOfThEmail;
        //            message.Body = htmlMessageBody;

        //            //The IsBodyHtml property of the MailMessage instance is set to true, which indicates that the message body is in HTML format.
        //            message.IsBodyHtml = true;

        //            //The From property of the MailMessage instance is set to the email address and display
        //            //name of the sender, which are specified in the EmailConfiguration object.
        //            message.From = new MailAddress(_emailConfiguration.SenderEmail, _emailConfiguration.SenderName);

        //            //4-
        //            //Finally, the SendMailAsync method of the SmtpClient instance is called, passing in the
        //            //MailMessage instance as a parameter. This sends the email message to the SMTP server for
        //            //delivery to the recipient.
        //            //client.Send(message);
        //            await client.SendMailAsync(message);

        //        }
        //    }
        //}




        //using MailKit:
        public async Task SendEmailAsync(string emailAddressOfTheRecipient, string subjectOfThEmail, string htmlMessageBody)
        {
            var message = new MimeMessage();
            //message.From.Add(new MailboxAddress("Abdul-Hai", ""));
            //message.To.Add(new MailboxAddress(emailAddressOfTheRecipient, emailAddressOfTheRecipient));
            message.From.Add(MailboxAddress.Parse("Abdul-Hai"));
            message.To.Add(MailboxAddress.Parse(emailAddressOfTheRecipient));

            message.Subject = subjectOfThEmail;
            message.Body = new TextPart("html")
            {
                Text = htmlMessageBody
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                //client.ServerCertificateValidationCallback = (s, c, h, e) => true; // This line is added to bypass certificate validation

                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, SecureSocketOptions.StartTls);
                client.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
                client.Capabilities &= ~SmtpCapabilities.Chunking;
                //client.Send(message);
                await client.SendAsync(message);

                client.Disconnect(true);
            }
        }













    }
}




//Note:
//In C#, the using statement is used to define a scope at the end of which an object will be disposed.
//The MailMessage class in the .NET Framework implements the IDisposable interface, which requires that
//the object be explicitly disposed of when it is no longer needed.

//By using using (var message = new MailMessage()), we are creating a new instance of the MailMessage class
//and wrapping it in a using statement.This means that the MailMessage object will be disposed of automatically
//when the using block is exited, which helps to ensure that unmanaged resources associated with the
//object (such as network connections or file handles) are released in a timely manner and do not linger
//in memory, leading to performance and reliability issues.

//So in the context of the SendEmailAsync method, the using block creates a new MailMessage instance with
//the new keyword, which represents the email message that will be sent. The using statement ensures that
//the MailMessage object will be properly disposed of once the using block is exited, so we don't need
//to worry about manually calling the Dispose method on the object.