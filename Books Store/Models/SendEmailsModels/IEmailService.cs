using NLog.Fluent;

namespace Books_Store.Models.SendEmailsModels
{
    public interface IEmailService
    {
        Task SendEmailAsync(string emailAddressOfTheRecipient, string subjectOfThEmail, string htmlMessageBody);

    }
//    mail body
//log to db
//coby db

}
