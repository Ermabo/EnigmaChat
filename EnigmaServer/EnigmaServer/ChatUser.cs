using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace EnigmaServer
{
    public class ChatUser
    {
        public string Nickname { get; set; }

        public Socket ClientSock { get; set; }

        public ChatUser()
        {
            
        }
    }
}
