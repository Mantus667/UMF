using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ActionMailer.Net;
using ActionMailer.Net.Mvc;
using System.Configuration;
using Umbraco.Core.Models;

namespace UMF.Controllers
{
    public class MailController : MailerBase
    {
        public EmailResult SendNotificationEmail(string email, string subject, string from, IPublishedContent content, string culture = "DE")
        {
            To.Add(email);
            Subject = subject;
            From = from;

            return Email("NotificationNewAnswer" + culture, content);
        }
    }
}