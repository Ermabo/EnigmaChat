using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EnigmaServer
{
    public class Server
    {
        private List<ChatUser> connectedClients = new List<ChatUser>();
        private bool nicknameChecked;
        private Socket servSocket;

        public Server()
        {
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 8888);
            servSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            servSocket.Bind(ipEnd);
        }

        public void InitClientConnecton()
        {

            while (true)
            {
                ChatUser user = new ChatUser();
                servSocket.Listen(0);
                Console.WriteLine("Started a new socket, waiting for connections..");

                user.ClientSock = servSocket.Accept();
                Console.WriteLine("New client was connected");

                GetClientNickname(user);

                // Makes sure the first client to connect gets added to the list
                if (connectedClients.Count == 0)
                {
                    connectedClients.Add(user);
                    nicknameChecked = true;

                    Thread clientThread = new Thread(() => HandleMessages(user));
                    clientThread.Start();
                }
                else
                {
                    nicknameChecked = false;
                    if (CheckNameAvaiability(user))
                    {
                        connectedClients.Add(user);
                        Thread clientThread = new Thread(() => HandleMessages(user));
                        clientThread.Start();
                    }
                }
            }
        }

        public void GetClientNickname(ChatUser _user)
        {
            byte[] tempNicknameBuffer = new byte[_user.ClientSock.ReceiveBufferSize];
            Console.WriteLine("Waiting for user to send desired nickname..");
            int recievedBytes = _user.ClientSock.Receive(tempNicknameBuffer);

            Console.WriteLine("Nickname buffer recieved! Encoding buffer..");
            byte[] nicknameBuffer = new byte[recievedBytes];

            Array.Copy(tempNicknameBuffer, nicknameBuffer, recievedBytes);
            _user.Nickname = Encoding.ASCII.GetString(nicknameBuffer);
        }

        public bool CheckNameAvaiability(ChatUser _user)
        {
            if (nicknameChecked == false)
            {
                for (int i = 0; i < connectedClients.Count; i++)
                {
                    if (connectedClients[i].Nickname == _user.Nickname)
                    {
                        // We end up here if user has an already taken username
                        string cmd = "#Invalid#";
                        byte[] cmdBuffer = new byte[Encoding.ASCII.GetByteCount(cmd)];
                        cmdBuffer = Encoding.ASCII.GetBytes(cmd);
                        _user.ClientSock.Send(cmdBuffer);

                        Console.WriteLine("User tried to connect with already taken username");

                        _user.ClientSock.Close();
                        nicknameChecked = true;
                        return false;
                    }
                }
            }
            return true;
        }

        private void HandleMessages(ChatUser _usr)
        {
            string clientNickname;

            clientNickname = _usr.Nickname + "#Nick#";
            try
            {
                byte[] tempBuffer = new byte[_usr.ClientSock.ReceiveBufferSize];
                int recievedBytes;

                do
                {
                    byte[] nicknameBuffer = new byte[Encoding.ASCII.GetByteCount(clientNickname)];
                    nicknameBuffer = Encoding.ASCII.GetBytes(clientNickname);

                    recievedBytes = _usr.ClientSock.Receive(tempBuffer);

                    byte[] buffer = new byte[recievedBytes];
                    Array.Copy(tempBuffer, buffer, recievedBytes);
                    string recievedMessege = Encoding.ASCII.GetString(buffer);

                    Console.WriteLine("Recieved message from client {0}: " + recievedMessege, _usr.Nickname);

                    foreach (var socket in connectedClients)
                    {
                        socket.ClientSock.Send(nicknameBuffer);
                        socket.ClientSock.Send(buffer);
                    }

                } while (recievedBytes > 0);
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Console.WriteLine(e.Message);
                connectedClients.Remove(_usr);
                InitClientConnecton();
            }
        }

    }
}

