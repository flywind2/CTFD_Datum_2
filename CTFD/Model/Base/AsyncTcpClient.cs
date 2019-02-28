using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.Model.Base
{
    public class AsyncTcpClient
    {
        private TcpClient tcpClient;

        private NetworkStream networkStream;

        private IPEndPoint remoteEndPoint;

        private Package package;

        public event EventHandler<NotifyEventArgs> Disconnected;

        public event EventHandler<NotifyEventArgs> DataReceived;

        public AsyncTcpClient() { this.package = new Package(); }

        public AsyncTcpClient(byte[] header, byte[] tail)
        {
            this.package = new Package(header, tail);
        }

        public bool GetConnectionState() => this.tcpClient.Connected;

        public void Connect(IPAddress remoteIPAddress, int remotePort)
        {
            if (this.tcpClient == null)
            {
                this.tcpClient = new TcpClient();
                this.tcpClient?.BeginConnect(remoteIPAddress, remotePort, new AsyncCallback(this.Connect), this.tcpClient);
                this.remoteEndPoint = new IPEndPoint(remoteIPAddress, remotePort);
            }
        }

        private void Connect(IAsyncResult asyncResult)
        {
            try
            {
                if (tcpClient.Connected)
                {
                    tcpClient.EndConnect(asyncResult);
                    this.networkStream = this.tcpClient.GetStream();
                    var size = tcpClient.ReceiveBufferSize;
                    var buffer = new byte[size];
                    this.networkStream.BeginRead(buffer, 0, size, this.ReceiveData, buffer);
                }
                else
                {

                }
            }
            catch
            {

            }
        }

        private void ReceiveData(IAsyncResult asyncResult)
        {
            var buffer = (byte[])asyncResult.AsyncState;
            int receivedLength = 0;
            try { receivedLength = this.networkStream.EndRead(asyncResult); }
            catch { receivedLength = 0; }
            if (receivedLength != 0)
            {
                var receivedBuffer = new byte[receivedLength];
                Buffer.BlockCopy(buffer, 0, receivedBuffer, 0, receivedLength);
                this.package.HandleData(receivedBuffer, () => this.RaiseDataReceived(this.tcpClient, this.package.CompleteData.ToArray()));
                this.networkStream.BeginRead(buffer, 0, buffer.Length, ReceiveData, buffer);
            }
        }

        public void SendData(byte[] data)
        {
            if (this.tcpClient.Connected == false)
            {
                //this.tcpClient.Dispose();
                this.tcpClient = new TcpClient();
                this.tcpClient?.BeginConnect(this.remoteEndPoint.Address, this.remoteEndPoint.Port, new AsyncCallback(this.Connect), this.tcpClient);
            }
            else
            {
                try
                {
                    //byte[] buffer = Encoding.Default.GetBytes(data);
                    this.networkStream.BeginWrite(data, 0, data.Length, new AsyncCallback(this.SendData), this.tcpClient);
                    this.networkStream.Flush();
                }
                catch { }
            }
        }

        private void SendData(IAsyncResult asyncResult)
        {
            try
            {
                this.networkStream.EndWrite(asyncResult);
            }
            catch { }
        }

        private void RaiseDisconnected(TcpClient TcpClient, string message)
        {
            this.Disconnected?.Invoke(this, new NotifyEventArgs(TcpClient, message));
        }

        private void RaiseDataReceived(TcpClient TcpClient, object message)
        {
            this.DataReceived?.Invoke(this, new NotifyEventArgs(TcpClient, message));
        }
    }

    public class AsyncTcpServer : IDisposable
    {
        private int currentClientCount;

        private TcpListener tcpListener;

        public List<TcpClient> tcpClients;

        private Package package;

        private bool disposed = false;

        private bool isServerRunning;

        public event EventHandler<NotifyEventArgs> ClientConnected;

        public event EventHandler<NotifyEventArgs> ClientDisconnected;

        public event EventHandler<NotifyEventArgs> DataReceived;

        public AsyncTcpServer(int listenPort, byte[] header, byte[] tail) : this(IPAddress.Any, listenPort, header, tail) { }

        public AsyncTcpServer(IPEndPoint localEP, byte[] header, byte[] tail) : this(localEP.Address, localEP.Port, header, tail) { }

        public AsyncTcpServer(IPAddress iPAddress, int port, byte[] header, byte[] tail)
        {
            tcpClients = new List<TcpClient>();
            this.tcpListener = new TcpListener(new IPEndPoint(iPAddress, port));
            this.tcpListener.AllowNatTraversal(true);
            this.package = new Package(header, tail);
        }

        public void Start(int maxClientCount = 0)
        {
            if (this.isServerRunning == false)
            {
                isServerRunning = true;
                if (maxClientCount == 0) tcpListener.Start();
                else tcpListener.Start(maxClientCount);
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientRequest), tcpListener);
            }
        }

        public void Stop()
        {
            if (isServerRunning)
            {
                isServerRunning = false;
                tcpListener.Stop();
                lock (tcpClients)
                {
                    CloseAllClient();
                }
            }
        }

        private void ClientRequest(IAsyncResult asyncResult)
        {
            if (isServerRunning)
            {
                var newClient = tcpListener.EndAcceptTcpClient(asyncResult);
                var newClientBuffer = new byte[newClient.ReceiveBufferSize];
                lock (tcpClients)
                {
                    tcpClients.Add(newClient);
                    RaiseClientConnected(newClient, "新客户端");
                }
                newClient.GetStream().BeginRead(newClientBuffer, 0, newClientBuffer.Length, ReceiveData, new Tuple<TcpClient, byte[]>(newClient, newClientBuffer));
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientRequest), asyncResult.AsyncState);
            }
        }

        private void ReceiveData(IAsyncResult asyncResult)
        {
            if (isServerRunning)
            {
                var newState = (Tuple<TcpClient, byte[]>)asyncResult.AsyncState;
                int receivedLength = 0;
                try { receivedLength = newState.Item1.GetStream().EndRead(asyncResult); }
                catch { receivedLength = 0; }

                if (receivedLength == 0)
                {
                    lock (tcpClients)
                    {
                        tcpClients.Remove(newState.Item1);
                        this.RaiseClientDisconnected(newState.Item1, "客户端离开");
                        return;
                    }
                }
                var receivedBuffer = new byte[receivedLength];
                Buffer.BlockCopy(newState.Item2, 0, receivedBuffer, 0, receivedLength);
                this.package.HandleData(receivedBuffer, new Action(() => this.RaiseDataReceived(newState.Item1, this.package.CompleteData.ToArray())));
                newState.Item1.GetStream().BeginRead(newState.Item2, 0, newState.Item2.Length, ReceiveData, new Tuple<TcpClient, byte[]>(newState.Item1, newState.Item2));
            }
        }

        public void SendData(TcpClient client, byte[] data)
        {
            try
            {
                //var buffer = Encoding.GetBytes(data);
                var networkStream = client.GetStream();
                if (isServerRunning && client != null && data != null) networkStream.BeginWrite(data, 0, data.Length, SendData, client);
                networkStream.Flush();
            }
            catch { }
        }

        private void SendData(IAsyncResult asyncResult)
        {
            ((TcpClient)asyncResult.AsyncState).GetStream().EndWrite(asyncResult);
        }

        private void RaiseClientConnected(TcpClient tcpClient, string message)
        {
            this.ClientConnected?.Invoke(this, new NotifyEventArgs(tcpClient, message));
        }

        private void RaiseClientDisconnected(TcpClient tcpClient, string message)
        {
            this.ClientDisconnected?.Invoke(this, new NotifyEventArgs(tcpClient, message));
        }

        private void RaiseDataReceived(TcpClient TcpClient, byte[] message)
        {
            this.DataReceived?.Invoke(this, new NotifyEventArgs(TcpClient, message));
        }

        public void Close(TcpClient tcpClient)
        {
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClients.Remove(tcpClient);
                currentClientCount--;
                //TODO 触发关闭事件  
            }
        }

        public void CloseAllClient()
        {
            foreach (var client in tcpClients)
            {
                Close(client);
            }
            currentClientCount = 0;
            tcpClients.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    try
                    {
                        Stop();
                        if (tcpListener != null)
                        {
                            tcpListener = null;
                        }
                    }
                    catch (SocketException)
                    {

                    }
                }
                disposed = true;
            }
        }
    }

    public class Package
    {
        private byte[] header;

        private byte[] tail;

        public List<byte> CompleteData { get; private set; } = new List<byte>();

        private List<byte> packetBuffer = new List<byte>();

        private Step step = Step.Start;

        private int dynamicDataLength;

        public Package()
        {
            this.header = new byte[] { 0x01, 0x02, 0x03 };
            this.tail = new byte[] { 0x03, 0x02, 0x01 };
        }

        public Package(byte[] header, byte[] tail)
        {
            this.header = header;
            this.tail = tail;
        }

        public void SetHeaderAndTail(byte[] header, byte[] tail)
        {
            this.header = header;
            this.tail = tail;
        }

        public void HandleData(byte[] receivedBuffer, Action completed)
        {
            this.dynamicDataLength = this.header.Length - 1;
            this.packetBuffer.AddRange(receivedBuffer);
            while (this.packetBuffer.Count > this.dynamicDataLength)
            {
                switch (this.step)
                {
                    case Step.Start:
                    {
                        if (this.packetBuffer[0] == this.header[0])
                        {
                            var isFindHeader = true;
                            for (int i = 0; i < this.header.Length; i++) if (this.packetBuffer[i] != this.header[i]) { isFindHeader = false; break; }

                            if (isFindHeader)
                            {
                                this.packetBuffer = this.packetBuffer.Skip(this.header.Length).ToList();
                                this.step = Step.End;
                                this.dynamicDataLength = this.tail.Length - 1;
                                break;
                            }
                        }
                        else this.packetBuffer.RemoveAt(0);
                        break;
                    }
                    case Step.End:
                    {
                        if (this.packetBuffer[0] == this.tail[0])
                        {
                            var isFindTail = true;
                            for (int i = 0; i < this.tail.Length; i++) if (this.packetBuffer[i] != this.tail[i]) { isFindTail = false; break; }
                            if (isFindTail)
                            {
                                this.packetBuffer.RemoveRange(0, this.tail.Length);
                                this.step = Step.Start;
                                completed();
                                this.CompleteData.Clear();
                                break;
                            }
                        }
                        else
                        {
                            this.CompleteData.Add(this.packetBuffer[0]);
                            this.packetBuffer.RemoveAt(0);
                        }
                        break;
                    }
                }
            }
        }
    }

    public class NotifyEventArgs : EventArgs
    {
        public TcpClient TcpClient { get; set; }
        public object Message { get; set; }
        public NotifyEventArgs(TcpClient TcpClient, object message)
        {
            this.TcpClient = TcpClient;
            this.Message = message;
        }
    }

    public enum Step
    {
        Start = 0,
        End = 1
    }
}
