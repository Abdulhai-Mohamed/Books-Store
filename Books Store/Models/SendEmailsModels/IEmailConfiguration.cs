namespace Books_Store.Models.SendEmailsModels
{
    //1- 
    //Mail clients vs Mail server
    //Mail clients and mail servers are two different types of software that are used in the context of email communication.
    //A mail client(also called an email client or email program) is an application that is used by
    //an individual to manage their email messages.Some popular mail clients include Microsoft Outlook,
    //Gmail, Apple Mail, and Mozilla Thunderbird.A mail client is typically installed on a
    //personal computer or mobile device, and it allows the user to read, write, send, and receive email messages.

    //A mail server, on the other hand, is a software application that is responsible for routing email
    //messages between different mail clients and servers.When you send an email message from your
    //mail client, the message is sent to a mail server, which then forwards the message to the
    //recipient's mail server. The recipient's mail server then delivers the message to the
    //recipient's mail client. Mail servers can be configured to handle different types of
    //email traffic, such as incoming and outgoing messages, spam filtering, and virus scanning.

    //In general, mail clients are used by individual users to manage their own email messages,
    //while mail servers are used by organizations to manage email traffic for multiple users.
    //However, there are some mail clients (such as Microsoft Outlook) that can also function as
    //a mail server, and there are some mail servers(such as Gmail) that can also be accessed using
    //a web interface like a mail client.


    //2-
    //SMTP (Simple Mail Transfer Protocol)
    //SMTP(Simple Mail Transfer Protocol) is a protocol used to send email messages over the internet.
    //It is typically used by mail clients (such as Microsoft Outlook) or mail servers(such as Microsoft Exchange)
    //to send email messages to other mail servers, which then deliver the messages to their intended recipients.
    //When you configure your mail client or server to send email messages, you typically need to specify
    //the following SMTP settings:

    //SmtpServer: The hostname or IP address of the SMTP server that will be used to send email messages.

    //SmtpPort: The port number that the SMTP server listens on for incoming connections. The default port
    //for SMTP is 25, but other ports may be used depending on the email provider and the configuration of the SMTP server.

    //SmtpUsername: The username that will be used to authenticate with the SMTP server. This is usually an
    //email address or a username assigned by the email provider.

    //SmtpPassword: The password that will be used to authenticate with the SMTP server.

    //The SmtpUsername and SmtpPassword settings are used to authenticate with the SMTP server.When you
    //send an email message using SMTP, you are connecting to an SMTP server and telling it to send the message
    //on your behalf.The SMTP server needs to know who you are(i.e., your email address and password) so that
    //it can verify that you are authorized to send email messages.


    //should "SmtpUsername": "your-username", "SmtpPassword": "your-password" must be a valid email and pass words?
    //No, the SmtpUsername and SmtpPassword values are typically not the same as your email and password.
    //These are the credentials used to authenticate with the SMTP server that you are using to send the email.

    //You will need to provide the correct credentials for the SMTP server that you are using.
    //These credentials are usually provided by your email service provider or your network administrator
    //if you are using an internal SMTP server.
    //Make sure that you have the correct values for these fields before attempting to send an email.

    //more explain
    //When you want to send an email using an SMTP server, you need to provide credentials that will allow you
    //to authenticate with the server.These credentials typically consist of a username and password.
    //The SmtpUsername and SmtpPassword values in your email configuration represent these credentials.
    //However, they are not necessarily the same as the username and password for your email account.

    //For example, if you are using a Gmail account to send emails, you will need to use your Gmail username(email address ) and
    //password to authenticate with the Gmail SMTP server because gmail use your Gmail username and
    //password as valeus of credentials
    //However, if you are using a different SMTP server(e.g.provided by your company), you will need to use
    //the credentials provided by that server.

    //So For other SMTP servers, you need to obtain your credentials from the email service provider or the
    //administrator of your email account.The process of obtaining your SMTP credentials may vary depending on
    //the email service provider or the email client you are using.

    //Usually, you can obtain your SMTP credentials by logging into your email account and navigating to the
    //account settings or the SMTP settings page.From there, you can find your SMTP server address, port number,
    //and the username and password needed to authenticate with the server.

    //Alternatively, you can contact your email service provider's support team and ask for assistance
    //in obtaining your SMTP credentials. They may provide you with instructions or direct you to the right
    //resources to obtain your SMTP credentials.


    //It is important to ensure that you have the correct credentials for your SMTP server, otherwise you will
    //not be able to send emails.You can usually obtain these credentials from your email service provider or network administrator.

    //Once you have obtained the correct credentials, you can set the SmtpUsername and SmtpPassword values
    //in your email configuration to these values. This will allow you to authenticate with the SMTP server
    //and send emails. 

    //Authentication is typically performed using one of two methods: plain text authentication or encrypted authentication.
    //With plain text authentication, your email address and password are sent over the internet in plain text,
    //which can be intercepted and read by hackers. Encrypted authentication uses SSL or TLS encryption to
    //protect your email address and password from being intercepted.

    //In the example above, we used SSL encryption to authenticate with the SMTP server.The EnableSsl property
    //of the SmtpClient object is set to true, which tells the SmtpClient object to use secure SSL/TLS  encryption when
    //connecting to the SMTP server.When SSL encryption is used, your email address and password are encrypted
    //before they are sent over the internet, making it much more difficult for hackers to intercept and read them.


    //3-
    //Note that you will also need to enable the "less secure apps" option in your Gmail account settings to allow
    //external applications, like your email client or application, to access your account. You can do this by going
    //to your Google Account settings, navigating to the "Security" tab, and then turning on the "Less secure app access" option.

    //4-
    //You can use one Gmail account as the SmtpUsername to authenticate with the Gmail SMTP server,
    //and use another Gmail account as the SenderEmail to send emails from.This allows you to keep your
    //authentication credentials separate from the email address that is displayed to the recipient.




    //5-
    //Google stopped allowing apps logging in to Gmail using real password

    //After 30 May 2022 you can't login with your username and password alone to Gmail. Less secure app access is not available anymore unless you have Google Workspace or Google Cloud Identity.

    //Source: https://support.google.com/accounts/answer/6010255

    //You need to make a password for specific app

    //Step one: enable 2FA

    //https://myaccount.google.com/signinoptions/two-step-verification/enroll-welcome
    //Step two: create an app-specific password

    //https://myaccount.google.com/apppasswords
    //enter image description here

    //After this, use that 16 digit



    public interface IEmailConfiguration
    {
        //The IEmailConfiguration interface represents the settings needed to configure
        //an email service, such as the SMTP server address, port, username, password, etc.


        //info of server 
        string SmtpServer { get; }
        int SmtpPort { get; }

        //your credentials to sent over SMTP
        string SmtpUsername { get; }
        string SmtpPassword { get; }

        //info of sender
        string SenderName { get; }
        string SenderEmail { get; }
    }
}
