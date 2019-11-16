using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DotNetty.Transport.Channels;

namespace BaseUnits.Remotings
{
    public class EchoServerHandler : ChannelHandlerAdapter
    {
        int receivedCount;
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Interlocked.Increment(ref receivedCount);
            //var buffer = message as IByteBuffer;
            //if (buffer != null)
            //{
            //    Console.WriteLine("Received from client: " + buffer.ToString(Encoding.UTF8));
            //}

            if (receivedCount % 100000 == 0)
            {
                Console.WriteLine($"current - {receivedCount}");
            }


            context.WriteAsync(message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}
