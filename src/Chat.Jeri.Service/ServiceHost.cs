using Chat.Jeri.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting.Internal;
using System.Net.WebSockets;
using System.Threading;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Http;

namespace Chat.Jeri.Service
{
    public class ServiceHost : IServiceHost
    {
        private readonly IWebHost webHost;
        private readonly WebSocket webSocket;
        public ServiceHost(IWebHost webHost)
        {
            this.webHost = webHost;
            //this.webSocket = webSocket;
        }

        public void Run()
        {
            webHost.Run();
        }

        public static HostBuilder Create(string[] args)
        {
            Console.Title = typeof(Startup).Namespace;
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var webHostBuilder = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseStartup(typeof(Startup));

            return new HostBuilder(webHostBuilder.Build());
        }
    }

    public abstract class BuilderBase
    {
        public abstract ServiceHost Build();
    }

    public class HostBuilder : BuilderBase
    {
        private readonly IWebHost webHost;
        private readonly WebSocket webSocket;
        public HostBuilder(IWebHost webHost)
        {
            this.webHost = webHost;
            //this.webSocket = webSocket;
        }

        public override ServiceHost Build()
        {
            return new ServiceHost(webHost);
        }
    }
}