using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Useful_Features
{
  
    public class EmailHelper
    {
        public static Dictionary<string, int> CommonSmtp = new Dictionary<string, int>()
        {
            { "smtp.google.com", 587},
            { "smtp.live.com", 25},
            { "smtp.mail.yahoo.com.tw", 465},
        };

        private Encoding EmailEncoding { get; set; }
        private string EmailFrom { get; set; }
        private string Title { get; set; }

        public string SmtpAddress { get; set; }
        public int SmtpPort { get; set; }

        public MailMessage CustomEmail { get; set; }


        public EmailHelper(string emailFrom, string Title, Encoding encod)
        {
            this.EmailEncoding = encod;
            this.EmailFrom = emailFrom;
            this.Title = Title;
        }

        public void CreateNewEmail(string sendToPath, string subject, bool hasAttachment, IEmailAttachment attachment = null)
        {
            CustomEmail = new MailMessage();
            CustomEmail.From = new MailAddress(EmailFrom, Title, EmailEncoding);
            CustomEmail.To.Add(sendToPath);
            CustomEmail.Subject = subject;
            CustomEmail.SubjectEncoding = EmailEncoding;
            CustomEmail.BodyEncoding = EmailEncoding;
            CustomEmail.IsBodyHtml = hasAttachment;

            if(hasAttachment && attachment != null)
            {
                attachment.SetAttachment(CustomEmail);
            }
        }

        public void SendEmail() 
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient(SmtpAddress))
                {
                    smtp.Port = SmtpPort;
                    smtp.EnableSsl = true;
                    smtp.Timeout = 100000;
                    smtp.Send(CustomEmail);
                }
            }catch(Exception)
            {
                throw new MailException();
            }
            finally
            {
                if(CustomEmail.Attachments.Count > 0)
                {
                    foreach (Attachment a in CustomEmail.Attachments)
                        a.Dispose();
                }
                CustomEmail.Dispose();
            }
        }


        public interface IEmailAttachment
        {
             void SetAttachment(MailMessage mail);
        }

        #region
//        Attachment Example

//        Uri uri = new Uri(path, UriKind.RelativeOrAbsolute);
//        BitmapImage bitmap = new BitmapImage();
//        Attachment attachment;
//            using (FileStream stream = new FileStream(uri.AbsolutePath, FileMode.Open))
//            {
//                bitmap.BeginInit();
//                bitmap.StreamSource = stream;
//                bitmap.CacheOption = BitmapCacheOption.OnLoad;
//                bitmap.EndInit();
//            }

//          var ms = new MemoryStream();

//          JpegBitmapEncoder encoder = new JpegBitmapEncoder();
//          encoder.Frames.Add(BitmapFrame.Create(bitmap));
//            encoder.Save(ms);
//            ms.Seek(0, SeekOrigin.Begin);

//            attachment = new Attachment(ms, "image/jpg");

//          attachment.Name = name;
//            attachment.NameEncoding = encode;
//            attachment.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;

//            attachment.ContentDisposition.Inline = true;
//            attachment.ContentDisposition.DispositionType = "inline";

//            mail.Attachments.Add(attachment);
        #endregion


        [Serializable]
        public class MailException : Exception
        {
            public MailException() { }
            public MailException(string message) : base(message) { }
            public MailException(string message, Exception inner) : base(message, inner) { }
            protected MailException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

            public override string Message
            {
                get
                {
                    return base.Message + "Please Checkout SmtpAddress:SmtpPort \n if has attachment check it";
                }
            }
        }
    }
}
