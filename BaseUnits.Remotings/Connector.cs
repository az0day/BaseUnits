using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace BaseUnits.Remotings
{
    class Connector : ChannelHandlerAdapter
    {
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Connected = true;
            OnConnectEvent?.Invoke(this, EventArgs.Empty);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Connected = false;
            OnDisconnectedEvent?.Invoke(this, EventArgs.Empty);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Received(message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }

        private static Bootstrap CreateBootstrap()
        {
            var group = new MultithreadEventLoopGroup();
            var bootstrap = new Bootstrap();
            return bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true);
        }

        public long Id { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }

        public bool Connected { get; internal set; }

        public event EventHandler OnConnectEvent;
        public event EventHandler OnDisconnectedEvent;

        private readonly AutoResetEvent _receviedEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _sendingEvent = new AutoResetEvent(false);
        private object _receviedMessage;

        private IChannel _channel;

        public async void Connect()
        {
            if (!Connected)
            {
                var bootstrap = CreateBootstrap();
                var address = IPAddress.Parse(Address);

                bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new EchoClientHandler(this));
                }));
                _channel = await bootstrap.ConnectAsync(new IPEndPoint(address, Port));
            }
        }

        public void Received(object message)
        {
            _receviedMessage = message;

            _sendingEvent.Set();
            _receviedEvent.Set();
        }

        public Connector(long id, string address, int port)
        {
            Id = id;
            Address = address;
            Port = port;
        }

        public byte[] Request(byte[] data)
        {
            if (Connected && _channel != null)
            {
                _sendingEvent.WaitOne(20000);

                _channel.WriteAndFlushAsync(data);
                _receviedEvent.WaitOne(20000);

                return (byte[]) _receviedMessage;
            }

            throw new ConnectionPendingException();
        }

        public void Post(byte[] data)
        {
            
        }
    }
}
