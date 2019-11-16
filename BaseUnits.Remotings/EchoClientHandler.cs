using System;
using DotNetty.Transport.Channels;

namespace BaseUnits.Remotings
{
    internal class EchoClientHandler : ChannelHandlerAdapter
    {
        private readonly Connector _connector;
        public EchoClientHandler(Connector connector)
        {
            _connector = connector;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _connector.Connected = true;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            _connector.Received(message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}
