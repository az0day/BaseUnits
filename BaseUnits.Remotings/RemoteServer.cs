using System;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace BaseUnits.Remotings
{
    public class RemoteServer
    {

        
        public event EventHandler OnRequestEvent;
        public event EventHandler OnPostEvent;
        
        private void Listen(int port)
        {
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();

            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                //.Handler(new LoggingHandler("LSTN"))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    //pipeline.AddLast(new LoggingHandler("CONN"));
                    pipeline.AddLast(new EchoServerHandler());
                }));

            var bootstrapChannel = bootstrap.BindAsync(port);

        }
    }
}
