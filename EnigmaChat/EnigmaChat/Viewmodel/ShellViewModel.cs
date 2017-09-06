using Caliburn.Micro;
using EnigmaChat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;

namespace EnigmaChat
{
    public class ShellViewModel : PropertyChangedBase
    {
        private string _message;
        private string _selectedIP;
        private string _portNumber;
        private string _nickname;
        private ObservableCollection<string> _chatMessages;
        public string RecievedMessage;
        public string SenderNickname;

        public ConnectionHandler ServerConnection;

        public ShellViewModel()
        {
            ServerConnection = new ConnectionHandler();
            _chatMessages = new ObservableCollection<string>();
            ServerConnection.MessageWasRecieved += ServerConnection_MessageWasRecieved;
            SelectedIP = "127.0.0.1";
            PortNumber = "8888";
            Nickname = "SleepyTurtle";
        }

        private void ServerConnection_MessageWasRecieved(object sender, CustomEventArgs e)
        {
            RecievedMessage = e.ChatMessage;
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                ChatMessages.Add(RecievedMessage);
            });
        }

        public string SelectedIP
        {
            get { return _selectedIP; }
            set
            {
                _selectedIP = value;
                NotifyOfPropertyChange(() => SelectedIP);
            }
        }

        public string PortNumber
        {
            get { return _portNumber; }
            set
            {
                _portNumber = value;
                NotifyOfPropertyChange(() => PortNumber);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public string Nickname
        {
            get { return _nickname; }
            set
            {
                _nickname = value;
                NotifyOfPropertyChange(() => Nickname);
            }
        }

        public ObservableCollection<string> ChatMessages
        {
            get { return _chatMessages; }
            set
            {
                _chatMessages = value;
                NotifyOfPropertyChange(() => ChatMessages);
            }
        }

        public void ConnectButton()
        {
            int parsedPort = Int32.Parse(PortNumber);
            ServerConnection.ServerIP = new IPEndPoint(IPAddress.Parse(SelectedIP), parsedPort);

            ServerConnection.ConnectionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerConnection.ConnectToServer(Nickname);

            //NotifyOfPropertyChange(() => CanConnectButton);

            if (ServerConnection.ConnectionSocket.Connected)
            {
                Thread thread = new Thread(() => ServerConnection.BeginListening());
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public bool CanConnectButton
        {
            get { return ServerConnection.ConnectionSocket.Connected != true || false; }
        }

        public void SendButton()
        {
            ServerConnection.SendMessage(Message);
        }

    }
}


