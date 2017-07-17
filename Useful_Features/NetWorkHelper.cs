using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Useful_Features
{
    class NetWorkHelper
    {
        public string IP { get; set; }
        public int Port { get; set; }

        private IClientHandleData ClientHandleData;
        private IServerHandleData ServerHandleData;

        public void CreateClient()
        {
            Thread ClientThread = new Thread(new ThreadStart(Update));
            ClientThread.IsBackground = true;
            ClientThread.Start();
        }
        #region Client
        public void SetClientHandleData(IClientHandleData data)
        {
            this.ClientHandleData = data;
        }

        private void Update()
        {
            TcpClient client = new TcpClient();
            client.Connect(IP, Port);
            NetworkStream stream = client.GetStream();

            if (ClientHandleData != null)
                ClientHandleData.Handle(stream);

            stream.Close();
            client.Close();
        }

        public interface IClientHandleData
        {
            void Handle(NetworkStream stream);
        }
        #endregion

        #region Server
        public void CreateServer()
        {
            Thread thread = new Thread(new ThreadStart(TcpServerRun));
            thread.IsBackground = true;
            thread.Start();
        }

        private void TcpServerRun()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();

            while(true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                Thread tcpHandlerThread = new Thread(new ParameterizedThreadStart(tcpHandler));
                tcpHandlerThread.IsBackground = true;
                tcpHandlerThread.Start(client);
            }
        }

        private void tcpHandler(object obj)
        {
            TcpClient mClient = (TcpClient)obj;
            NetworkStream stream = mClient.GetStream();

            if (ServerHandleData != null)
                ServerHandleData.Handle(stream);

        }

        private void SetServerHandleData(IServerHandleData data)
        {
            this.ServerHandleData = data;
        }

        public interface IServerHandleData
        {
            void Handle(NetworkStream stream);
        }
        #endregion
    }
}
