using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System.Threading;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Chat.Jeri.Console
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(context, webSocket, loggerFactory.CreateLogger("Echo"));
                }
                else
                {
                    await next();
                }
            });

            app.UseFileServer();
        }

        private async Task Echo(HttpContext context, WebSocket webSocket, Microsoft.Extensions.Logging.ILogger logger)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                // If the client send "ServerClose", then they want a server-originated close to occur
                string content = "<<binary>>";
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    content = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (content.Equals("ServerClose"))
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing from Server", CancellationToken.None);
                        logger.LogDebug($"Sent Frame Close: {WebSocketCloseStatus.NormalClosure} Closing from Server");
                        return;
                    }
                    else if (content.Equals("ServerAbort"))
                    {
                        context.Abort();
                    }
                }

                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                logger.LogDebug($"Sent Frame {result.MessageType}: Len={result.Count}, Fin={result.EndOfMessage}: {content}");

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}