using Chat.Jeri.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Jeri.Test.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var service = ServiceHost.Create(args);
            service.Build().Run();
        }
    }
}
