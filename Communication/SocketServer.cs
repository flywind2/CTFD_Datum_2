using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Communication
{
    public class SocketServer
    {
        private Socket _listener ;
        private List<SocketClient> _clientList = new List<SocketClient>();
        private string _ip = "127.0.0.1";
        private readonly int _port;
        private bool _isRuning = true;
        private Thread _listenThread;
        public event DelConnected ConnectedHandle;
        public event DelDisConnect DisConnectedHandle;
        public event DelReceiveData ReceiveHandle;

        public SocketServer(string addr)
        {
            string[] s = addr.Split(new char[] { ':' });
            _ip = s[0];
            _port = Convert.ToInt32(s[1]);
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public SocketServer(string ip,int port)
        {
            this._ip = ip;
            this._port = port;
            this._listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            _isRuning = true;
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _listener.Bind(iep);
            _listener.Listen(20);
            _listenThread = new Thread(ListenThread) { IsBackground = true };
            _listenThread.Start();
        }

        private void ListenThread()
        {
            while (_isRuning)
            {
                try
                {
                    Socket s = _listener.Accept();
                    SocketClient client = new SocketClient(s);
                    lock (_clientList)
                    {
                        _clientList.Add(client);
                    }
                    ConnectedHandle?.Invoke(client);
                    client.DisconnectedHandle += new DelDisConnect(DisconnectProc);
                    client.ReceiveHandle += new DelReceiveData(ReceiveDataProc);
                    Thread.Sleep(2000);
                }
                catch { }
            }
        }

        public void Stop()
        {
            try
            {
                _isRuning = false;
                _listener.Close();
                foreach (var item in _clientList)
                {
                    item.IsRunning = false;
                    item.Close();
                }
            }
            catch { }
        }

        public string GetClientInfo()
        {
            string str = "";
            try
            {
                lock (_clientList)
                {
                    for (int i = 0; i < _clientList.Count; i++)
                    {
                        str += _clientList[i].GetAddress() + "  ";
                    }
                }
            }
            catch { }
            return str;
        }

        public SocketClient GetSocketClient(string ipadd)
        {
            SocketClient sock = null;
            try
            {
                string[] ipadder = ipadd.Split(new char[] { ':' });
                for (int i = 0; i < _clientList.Count; i++)
                {
                    string[] str = _clientList[i].GetAddress().Split(new char[] { ':' });
                    if (ipadder[0] == str[0])
                    {
                        sock = _clientList[i];
                    }
                }
            }
            catch
            {
                throw new Exception();
            }
            return sock;
        }

        public List<ClientInfo> GetClientInfoLst()
        {
            List<ClientInfo> lst = new List<ClientInfo>();
            try
            {
                lock (_clientList)
                {
                    for (int i = 0; i < _clientList.Count; i++)
                    {
                        string[] str = _clientList[i].GetAddress().Split(new char[] { ':' });

                        if (str != null)
                        {
                            ClientInfo info = new ClientInfo();
                            info.IP = str[0];
                            info.Number = Convert.ToInt32(str[1]);
                            info.Param = _clientList[i].GetAddress();
                            lst.Add(info);
                        }
                    }
                }
            }
            catch { }

            return lst;
        }

        public void SendToClient(byte[] data)
        {
            try
            {
                lock (_clientList)
                {
                    for (int i = 0; i < _clientList.Count; i++)
                    {
                        _clientList[i].SendMsg(data);
                    }
                }
            }
            catch { }
        }

        public void SendToClient(SocketClient dest, byte[] data)
        {
            try
            {
                dest.SendMsg(data);
            }
            catch { }
        }

        public void CloseClient(SocketClient client)
        {
            try
            {
                lock (_clientList)
                {
                    if (_clientList.Contains(client))
                    {
                        _clientList.Remove(client);
                    }

                    client.Close();
                }
            }
            catch { }
        }

        public int GetClientCount()
        {
            int num = 0;
            try
            {
                lock (_clientList)
                {
                    num = _clientList.Count;
                }
            }
            catch { }
            return num;
        }

        private void DisconnectProc(SocketClient client)
        {
            try
            {
                lock (_clientList)
                {
                    if (_clientList.Contains(client))
                    {
                        _clientList.Remove(client);
                    }
                }
                DisConnectedHandle?.Invoke(client);
            }
            catch { }
        }

        private void ReceiveDataProc(SocketClient client, byte[] buf)
        {
            try
            {
                if (buf != null && ReceiveHandle != null)
                {
                    ReceiveHandle(client, buf);
                }
            }
            catch { }
        }
    }

    public class ClientInfo
    {
        public string IP { set; get; }
        public int Number { set; get; }
        public string Param { set; get; }
    }
}
