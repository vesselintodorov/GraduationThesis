using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Data.Services
{
    public class EmailSender : IMessageSender
    {
        public Models.Messages.MessageProvider GetMessageProvider()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetMessageTemplateParams()
        {
            throw new NotImplementedException();
        }

        public void Send()
        {
            throw new NotImplementedException();
        }
    }
}
