using CTFD.Global.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CTFD.Model.Base
{
    public delegate void DelReceiveData(AsyncSocketClient sender, byte[] buf);
    public delegate void DelDisConnect(AsyncSocketClient sclient);
    public delegate void DelConnected(AsyncSocketClient sender);
    public class AsyncSocketClient
    {
        private Socket _sock = null;
        public event DelReceiveData ReceiveHandle;
        public event DelDisConnect DisconnectedHandle;
        public event DelConnected ConnectHandle;

        private Thread _revThread = null;
        private Thread _sendThread = null;
        private Thread _connecteThread = null;
        public bool IsRunning = true;

        private const int _rsize = 1024 * 1024;
        private const int _wsize = 1024 * 1024;
        private byte[] _RevBuf = new byte[_rsize];
        private byte[] _SendBuf = new byte[_wsize];
        private List<byte> _dataBuffer = new List<byte>();
        private Queue<byte[]> _msgQqueue = new Queue<byte[]>();

        private const int _maxPerPack = 1024 * 1024;

        //是否重连
        private bool IsReconnect = false;
        private string _remoteIp = "127.0.0.1";
        private int _remotePort = 50000;

        private const int _maxPackSize = 1024 * 1024 * 8;
        protected AutoResetEvent _autoevent = new AutoResetEvent(false);
        public AsyncSocketClient(string ip, int port, bool isReconn=true)
        {
            _remoteIp = ip;
            _remotePort = port;
            IsReconnect = isReconn;

            _revThread = new Thread(ReceiveThread);
            _revThread.IsBackground = true;
            _revThread.Start();

            _sendThread = new Thread(SendThread);
            _sendThread.IsBackground = true;
            _sendThread.Start();

            if (IsReconnect)//开启重连接线程
            {
                _connecteThread = new Thread(ReconnectThread);
                _connecteThread.IsBackground = true;
                _connecteThread.Start();
            }
        }

        private void ReconnectThread()
        {
            while (IsRunning)
            {
                try
                {
                    Connecte();
                }
                catch (System.Exception ex)
                {
                    Thread.Sleep(1000);
                }

                Thread.Sleep(1000);
            }
        }

        private void Connecte()
        {
            try
            {
                if (_sock == null || !_sock.Connected)
                {
                    _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _sock.ReceiveBufferSize = _rsize;
                    _sock.SendBufferSize = _wsize;
                    try
                    {
                        _sock.Connect(_remoteIp, _remotePort);

                        if (ConnectHandle != null)
                        {
                            ConnectHandle(this);
                        }
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public bool StartConnecte()
        {
            bool b = false;
            if (!IsReconnect && (_sock == null || !_sock.Connected))
            {
                _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _sock.ReceiveBufferSize = _rsize;
                _sock.SendBufferSize = _wsize;
                try
                {
                    _sock.Connect(_remoteIp, _remotePort);

                    if (ConnectHandle != null)
                    {
                        ConnectHandle(this);
                    }

                    b = true;
                }
                catch (Exception)
                {
                    Thread.Sleep(2000);
                }
            }

            return b;
        }

        public AsyncSocketClient(Socket s)
        {
            try
            {
                _sock = s;
                _sock.ReceiveBufferSize = _rsize;
                _sock.SendBufferSize = _wsize;

                _revThread = new Thread(ReceiveThread);
                _revThread.IsBackground = true;
                _revThread.Start();

                _sendThread = new Thread(SendThread);
                _sendThread.IsBackground = true;
                _sendThread.Start();
            }
            catch (Exception ex)
            {
                //General.Log.Info(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public void Close()
        {
            try
            {
                if (_sock != null && _sock.Connected)
                {
                    _sock.Close();
                }
            }
            catch (Exception ex)
            {
                //General.Log.Info(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public string GetAddress()
        {
            string addr = "none";
            try
            {
                if (_sock != null && _sock.Connected)
                {
                    addr = _sock.RemoteEndPoint.ToString();
                }
            }
            catch (Exception ex)
            {
                //General.Log.Info(ex.Message + "\r\n" + ex.StackTrace);
            }

            return addr;
        }

        public string GetAddres()
        {
            string addr = "none";
            try
            {
                if (_sock != null)
                {
                    addr = _sock.RemoteEndPoint.ToString();
                }
            }
            catch (Exception ex)
            {
                //General.Log.Info(ex.Message + "\r\n" + ex.StackTrace);
            }
            return addr;
        }

        public void SendMsg(byte[] buf)
        {
            // 先打包
            byte[] pack = PackData(buf);
            try
            {
                if (pack != null)
                {
                    if (_sock != null && _sock.Connected)
                    {
                        for (int i = 0; i < pack.Length; i += _maxPerPack)
                        {
                            int len = Math.Min(_maxPerPack, pack.Length - i);
                            if (len > 0)
                            {
                                byte[] perpack = new byte[len];
                                Array.Copy(pack, i, perpack, 0, len);
                                lock (_msgQqueue)
                                {
                                    if (_msgQqueue.Count > 1024)
                                    {
                                        _msgQqueue.Clear();
                                    }
                                    _msgQqueue.Enqueue(perpack);

                                }
                                _autoevent.Set();
                            }
                        }
                    }
                    else
                    {
                        DisconnectInner();
                    }
                }
            }
            catch (Exception ex)
            {
                DisconnectInner();
                //General.Log.Info(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public void SendByte(byte[] buf)
        {
            try
            {
                if (buf != null)
                {
                    if (_sock != null && _sock.Connected)
                    {
                        lock (_msgQqueue)
                        {
                            if (_msgQqueue.Count > 1024)
                            {
                                _msgQqueue.Clear();
                            }
                            _msgQqueue.Enqueue(buf);

                        }
                        _autoevent.Set();
                    }
                }
            }
            catch (Exception ex)
            {
                DisconnectInner();
                //General.Log.Info(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void ReceiveThread()
        {
            int len = 0;
            while (IsRunning)
            {
                try
                {
                    if (_sock != null && _sock.Connected)
                    {

                        len = _sock.Receive(_RevBuf, _rsize, SocketFlags.None);
                        if (len > 0)
                        {
                            byte[] buf = new byte[len];
                            Array.Copy(_RevBuf, 0, buf, 0, len);

                            for (int i = 0; i < buf.Length; i++)
                            {
                                if (buf[i] == 0x02)
                                {
                                    if (_dataBuffer.Count > 2 && _dataBuffer[0] == 0x02 && _dataBuffer[_dataBuffer.Count - 1] == 0x03)
                                    {
                                        byte[] pack = _dataBuffer.ToArray();
                                        byte[] srcData = DepackData(_dataBuffer.ToArray());
                                        if (srcData != null && ReceiveHandle != null)
                                        {
                                            ReceiveHandle(this, srcData);
                                        }
                                    }
                                    _dataBuffer.Clear();
                                    _dataBuffer.Add(buf[i]);
                                }
                                else if (buf[i] == 0x03)  // 
                                {
                                    _dataBuffer.Add(buf[i]);
                                    if (_dataBuffer.Count > 2 && _dataBuffer[0] == 0x02 && _dataBuffer[_dataBuffer.Count - 1] == 0x03)
                                    {
                                        byte[] pack = _dataBuffer.ToArray();
                                        byte[] srcData = DepackData(_dataBuffer.ToArray());
                                        if (srcData != null && ReceiveHandle != null)
                                        {
                                            ReceiveHandle(this, srcData);
                                        }
                                    }

                                    _dataBuffer.Clear();
                                }
                                else
                                {
                                    if (_dataBuffer.Count > 0 && _dataBuffer[0] == 0x02)
                                    {
                                        _dataBuffer.Add(buf[i]);
                                    }

                                    if (_dataBuffer.Count > _maxPackSize)
                                    {
                                        _dataBuffer.Clear();
                                    }
                                }
                            }
                        }
                        else
                        {
                            DisconnectInner();
                        }
                    }
                    else
                    {
                        DisconnectInner();
                        Thread.Sleep(1000);
                    }
                }
                catch (System.Exception ex)
                {
                    //Console.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show($"{ex.Message}--{ex.StackTrace}");
                    DisconnectInner();
                }
            }
        }

        private void DisconnectInner()
        {
            try
            {
                _dataBuffer.Clear();

                if (DisconnectedHandle != null)
                {
                    DisconnectedHandle(this);
                }

                if (!IsReconnect)
                {
                    IsRunning = false;
                    _autoevent.Set();
                }

                _sock?.Close();
                _sock = null;
            }
            catch (Exception ex)
            {
               // General.Log.Info(ex.Message + "\r\n" + ex.StackTrace);
            }

            Thread.Sleep(1000);
        }

        private void SendThread()
        {
            while (IsRunning)
            {
                try
                {
                    if (_sock != null && _sock.Connected)
                    {
                        byte[] buf = null;
                        lock (_msgQqueue)
                        {
                            if (_msgQqueue.Count > 0)
                            {
                                buf = _msgQqueue.Dequeue();
                            }
                        }

                        if (buf != null)
                        {
                            _sock.Send(buf, buf.Length, SocketFlags.None);
                        }
                        else
                        {
                            _autoevent.WaitOne();
                            continue;
                        }
                    }
                    else
                    {
                        _autoevent.WaitOne();
                    }
                }
                catch (System.Exception ex)
                {
                    //General.Log.Info(ex.Message + "\r\n" + ex.StackTrace);
                    Thread.Sleep(100);
                }

                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 打包  每个消息包都是 0x02 开头  0x03结尾 包格式  0x02    data  0x03
        /// 0x02 转成 0x1b 0xe7   0x03转成 0x1b 0xe8  0x1b转成 0x1b 0x00
        /// </summary>
        /// <param name="buf">原数据</param>
        /// <returns></returns>
        private byte[] PackData(byte[] data)
        {

            List<byte> bytelst = new List<byte>();

            try
            {
                // 包头
                bytelst.Add(0x02);

                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == 0x02)
                    {
                        bytelst.Add(0x1b);
                        bytelst.Add(0xe7);
                    }
                    else if (data[i] == 0x03)
                    {
                        bytelst.Add(0x1b);
                        bytelst.Add(0xe8);
                    }
                    else if (data[i] == 0x1b)
                    {
                        bytelst.Add(0x1b);
                        bytelst.Add(0x00);
                    }
                    else
                    {
                        bytelst.Add(data[i]);
                    }
                }
                bytelst.Add(0x03);
            }
            catch (System.Exception ex)
            {
                //General.Log.Info(ex.Message + "\r\n" + ex.StackTrace);
                return null;
            }

            return bytelst.ToArray();
        }

        /// <summary>
        /// 解包 去掉 0x02   0x03  已经反转义
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] DepackData(byte[] data)
        {
            List<byte> bytelist = new List<byte>();
            try
            {
                for (int i = 1; i < data.Length - 1; i++)
                {
                    if (data[i] == 0x1b)
                    {
                        if (data[i + 1] == 0xe7)
                        {
                            bytelist.Add(0x02);
                        }
                        else if (data[i + 1] == 0xe8)
                        {
                            bytelist.Add(0x03);
                        }
                        else if (data[i + 1] == 0x00)
                        {
                            bytelist.Add(0x1b);
                        }

                        i++;
                    }
                    else
                    {
                        bytelist.Add(data[i]);
                    }
                }
            }
            catch (System.Exception ex)
            {
                //General.Log.Info(ex.Message + "\r\n" + ex.StackTrace);
                return null;
            }

            return bytelist.ToArray();
        }
    }
}
