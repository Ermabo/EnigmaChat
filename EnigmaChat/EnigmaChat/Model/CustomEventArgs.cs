using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaChat.Model
{
    public class CustomEventArgs : EventArgs
    {
        public string ChatMessage { get; set; }
        public string SenderNickname { get; set; }

        public CustomEventArgs(string _recievedMessage)
        {
            ChatMessage = _recievedMessage;
        }
    }
}
