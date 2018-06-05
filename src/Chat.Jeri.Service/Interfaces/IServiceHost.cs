using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Chat.Jeri.Service.Interfaces
{
    public interface IServiceHost
    {
        void Run();
    }
}
