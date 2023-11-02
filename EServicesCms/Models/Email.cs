using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EServicesCms.Models
{
    public class MailAttachments
    {
        public byte[] FileData = null;

        public string FileType = string.Empty;

        public string FileName = string.Empty;

        public string FileSize = string.Empty;
        public MailAttachments()
        {
        }
    }
    public class EmailMessage
    {
        private string _receiver;

        private string _cc;

        private string _bcc;

        private string _sender;

        private string _subject;

        private string _message;

        private List<MailAttachments> _attachmentsList;

        public List<MailAttachments> Attachments
        {
            get
            {
                return this._attachmentsList;
            }
            set
            {
                this._attachmentsList = value;
            }
        }
        public string bcc
        {
            get
            {
                return this._bcc;
            }
            set
            {
                this._bcc = value;
            }
        }
        public string cc
        {
            get
            {
                return this._cc;
            }
            set
            {
                this._cc = value;
            }
        }
        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
            }
        }
        public string Receiver
        {
            get
            {
                return this._receiver;
            }
            set
            {
                this._receiver = value;
            }
        }
        public string Sender
        {
            get
            {
                return this._sender;
            }
            set
            {
                this._sender = value;
            }
        }
        public string Subject
        {
            get
            {
                return this._subject;
            }
            set
            {
                this._subject = value;
            }
        }
        public EmailMessage()
        {
        }
    }
}