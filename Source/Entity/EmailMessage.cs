using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class EmailMessage
    {
        private string recipient;
        private string subject;
        private string body;
        private string[] attachments;


        public EmailMessage(string recipient, string subject, string body, string[] attachments)
        {
            this.recipient = recipient;
            this.subject = subject;
            this.body = body;
            this.attachments = attachments;
        }


        public string getRecipient()
        {
            return this.recipient;
        }


        public string getSubject()
        {
            return this.subject;
        }


        public string getBody()
        {
            return this.body;
        }


        public string[] getAttachments()
        {
            return this.attachments;
        }


    }
}