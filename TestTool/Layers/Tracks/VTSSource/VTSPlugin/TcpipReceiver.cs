using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VTSPlugin
{
    /// <summary>
    /// 通过Tcpip协议接收数据的抽象基类
    /// 派生类在配置单中有如下定义：
    /// Server:远程服务器地址
    /// Port:远程服务器端口
    /// MaxDataInterval:最长无数据重连时间，单位秒，0表示不进行无数据重连
    /// </summary>
    public abstract class TcpipReceiver
    {
        /// <summary>
        /// Socket实例
        /// </summary>
        protected Socket _socket = null;

        /// <summary>
        /// 系统退出标志
        /// </summary>
        private ManualResetEvent _eventQuit = new ManualResetEvent(false);

        /// <summary>
        /// 工作线程实例
        /// </summary>
        private Thread _thread = null;

        /// <summary>
        /// 当前是否在重连
        /// </summary>
        private bool _reconnecting = false;

        /// <summary>
        /// 互斥锁，保证Reconnect和Shutdown不冲突
        /// </summary>
        private object _lock = new object();

        /// <summary>
        /// 实例序号，每Shutdown一次增加1
        /// </summary>
        private int _flag = 0;

        private string _ip = null;
        private string _port = null;
        private string _maxDataInterval = null;

        public void Startup(string ip, string port, string maxDataInterval)
        {
            _ip = ip;
            _port = port;
            _maxDataInterval = maxDataInterval;

            lock (_lock)
            {
                _eventQuit.Reset();

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _thread = new Thread(new ParameterizedThreadStart(workThread));
                _thread.IsBackground = true;
                _thread.Start(_flag);
            }
        }

        public void Shutdown()
        {
            lock (_lock)
            {
                _flag++;

                _eventQuit.Set();

                if (_thread != null)
                {
                    _thread.Join();
                    _thread = null;
                }

                if (_socket != null)
                {
                    if (_socket.Connected)
                        _socket.Disconnect(false);
                    _socket.Close();
                    _socket = null;
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param radarName="data"></param>
        protected void send(byte[] data)
        {
            try
            {
                if (_socket != null && _socket.Connected)
                    _socket.Send(data);
            }
            catch (SocketException)
            {
                //TestTool.Common.LogService.Log.Error(_ip + " socket error");
                //TestTool.Common.LogService.Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 工作线程
        /// </summary>
        private void workThread(object state)
        {
            //DateTime last = DateTime.Now;
            int lt = Environment.TickCount;

            int interval = 0;
            Int32.TryParse(_maxDataInterval, out interval);
            bool connected = false;
            try
            {
                //step1 : try to connect server
                IPAddress[] ips = Dns.GetHostAddresses(_ip);
                _socket.Connect(new IPEndPoint(ips[0], Int32.Parse(_port)));
                connected = true;
                _reconnecting = false;
                //Log.Add("与" + _ip + ":" + _port + "的网络连接成功");
                onConnected();

                //step2 : receive data loop
                byte[] buf = new byte[102400];
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, buf.Length);
                while (!_eventQuit.WaitOne(10))
                {
                    if (_socket.Poll(0, SelectMode.SelectRead))
                    {
                        //last = DateTime.Now;
                        lt = Environment.TickCount;
                        int recv = _socket.Receive(buf);
                        if (recv > 0)
                            onRecv(buf, recv);
                        else
                        {
                            //Log.Add("与" + _ip + ":" + _port + "的网络连接被对方主动关闭，将在10秒后重新连接");
                            break;
                        }
                    }
                    //检查是否要进行无数据重连
                    if (interval > 0)
                    {
                        //TimeSpan ts = DateTime.Now - last;
                        //if (ts.TotalSeconds > interval)
                        if (Environment.TickCount - lt > interval * 1000)
                        {
                            //Log.Add("与" + _ip + ":" + _port + "的网络连接已超时，将在10秒后重新连接");
                            break;
                        }
                    }
                }
            }
            catch (SocketException)
            {
                //如果是第一次失败，则记录LOG
                if (!_reconnecting)
                {
                    _reconnecting = true;
                    //Log.Add("与" + _ip + ":" + _port + "的网络连接发生异常，将在10秒后重新连接");
                }
            }
            finally
            {
                //释放网络资源
                _socket.Close();
                _socket = null;
            }
            //通知断线
            if (connected)
                onDisconnected();
            //如果不是Shutdown，则启动重新连接
            if (!_eventQuit.WaitOne(0))
                ThreadPool.QueueUserWorkItem(doReconnect, state);
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <param radarName="state"></param>
        private void doReconnect(object state)
        {
            //重新连接的时间间隔是10秒
            Thread.Sleep(10000);
            lock (_lock)
            {
                //检查是否已经被Shutdown
                if ((int)state == _flag)
                {
                    Shutdown();
                    Startup(_ip, _port, _maxDataInterval);
                }
                //else
                //	Log.Add("与" + _ip + ":" + _port + "的网络连接已经被Shutdown，无需Reconnect");
            }
        }

        /// <summary>
        /// 成功连接远程服务器
        /// </summary>
        protected virtual void onConnected() { }

        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param radarName="buf"></param>
        /// <param radarName="len"></param>
        protected abstract void onRecv(byte[] buf, int len);

        /// <summary>
        /// 与远程服务器的连接被断开
        /// </summary>
        protected virtual void onDisconnected() { }
    }

}
