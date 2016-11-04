using Seecool.Common.Util;
using Seecool.Messaging.Core;
using Seecool.Messaging.Parser;
using Seecool.Messaging.Receiver;
using System.Text;
using Seecool.Common.Run;
using System;

namespace VTSCore.Layers.Tracks
{
    public class ReceiverBase : IConsumable<string>
    {
        bool IsRunning { get; set; }
        public string Ip { get; private set; }
        public int Port { get; private set; }

        private AutoRepeatRunner _autoRepeatRunner;

        public void SetConfig(string ip, int port)
        {
            if (Ip != ip || Port != port)
            {
                Stop();
                Ip = ip;
                Port = port;
                Start();
            }
        }

        public void Start()
        {
            if (IsRunning)
                Stop();
            if (!string.IsNullOrWhiteSpace(Ip))
            {
                var receiver = new TcpClientReceiver(Ip, Port);
                var binToStringConverter = new SpecificEndingsBinaryToStringConverter(new string[] { Environment.NewLine }, Encoding.Default);
                var convertParser = new ConvertParser<IData, string>(this, binToStringConverter);
                var recvRunner = new ReceiverSourceRunner<IData>(receiver, convertParser);
                _autoRepeatRunner = new AutoRepeatRunner(recvRunner);

                _autoRepeatRunner.Start();
                Runner.ThreadRun(_autoRepeatRunner);
            }
        }

        public void Stop()
        {
            if (_autoRepeatRunner != null)
                _autoRepeatRunner.Stop();
            _autoRepeatRunner = null;
            IsRunning = false;
        }

        public virtual void Consume(string line)
        {
        }
    }
}
