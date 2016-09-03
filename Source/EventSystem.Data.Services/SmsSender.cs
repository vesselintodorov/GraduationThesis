using EventSystem.Data.Models.Messages;
using System;
using System.Collections.Generic;

namespace EventSystem.Data.Services
{
    public class SmsSender : IMessageSender
    {

        public MessageProvider GetMessageProvider()
        {
            // Fetch SMS provider details


            return new MessageProvider();
        }

        public Dictionary<string, string> GetMessageTemplateParams()
        {
            // Get Templates from DB

            return new Dictionary<string, string>();
        }

        public void Send()
        {
            var provider = GetMessageProvider();
            var templateParams = GetMessageTemplateParams();

            // Send message
        }
    }
}
