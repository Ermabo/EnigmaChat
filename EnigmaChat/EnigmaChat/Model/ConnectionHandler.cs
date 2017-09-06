using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EnigmaChat.Model
{
    public class ConnectionHandler
    {
        public Socket ConnectionSocket;
        public byte[] RecievedBuffer;
        public IPEndPoint ServerIP;
        public NetworkStream NetStream;
        public string RecievedMessege;
        public string SenderNickname;
        public event EventHandler<CustomEventArgs> MessageWasRecieved;

        public ConnectionHandler()
        {
            ConnectionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void BeginListening()
        {
            try
            {
                byte[] tempBuffer = new byte[ConnectionSocket.ReceiveBufferSize];
                int recievedBytes;

                do
                {
                    recievedBytes = ConnectionSocket.Receive(tempBuffer);

                    byte[] buffer = new byte[recievedBytes];
                    Array.Copy(tempBuffer, buffer, recievedBytes);

                    string encodedMesg = Encoding.ASCII.GetString(buffer);

                    if (encodedMesg.Contains("#Nick#"))
                    {
                        int index = encodedMesg.IndexOf("#");
                        SenderNickname = encodedMesg.Remove(index);
                    }

                    else if (encodedMesg.Contains("#Invalid#"))
                    {
                        MessageBox.Show("Username already taken, please choose another", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    else
                    {
                        if (encodedMesg != string.Empty)
                        {
                            encodedMesg = CryptoString(encodedMesg);
                            RecievedMessege = SenderNickname + ": " + encodedMesg; //+ Encoding.ASCII.GetString(buffer);
                            OnMessageWasRecieved(RecievedMessege);
                        }
                    }

                } while (recievedBytes > 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            finally
            {
                ConnectionSocket.Close();
            }
        }

        public void ConnectToServer(string _nickname)
        {
            try
            {
                ConnectionSocket.Connect(ServerIP);
                byte[] nicknameBuffer = new byte[Encoding.ASCII.GetByteCount(_nickname)];
                nicknameBuffer = Encoding.ASCII.GetBytes(_nickname);
                ConnectionSocket.Send(nicknameBuffer);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        public void SendMessage(string _messageToSend)
        {
            if (_messageToSend != null && ConnectionSocket.Connected)
            {
                _messageToSend = CryptoString(_messageToSend);

                byte[] messageBuffer = new byte[Encoding.ASCII.GetByteCount(_messageToSend)];

                messageBuffer = Encoding.ASCII.GetBytes(_messageToSend);
                ConnectionSocket.Send(messageBuffer);
            }
        }

        public string CryptoString (string _stringToCrypto)
        {
            char[] array = _stringToCrypto.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        protected virtual void OnMessageWasRecieved(string _recievedMsg)
        {
            MessageWasRecieved?.Invoke(this, new CustomEventArgs(_recievedMsg));
        }
    }
}
