using EventSystem.Data.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Data.Services
{
    public interface IMessageSender
    {
        MessageProvider GetMessageProvider();

        Dictionary<string,string> GetMessageTemplateParams();

        void Send();
    }
}
