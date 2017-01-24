using ImperaturService.ImperaturService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Imperatur_v2;

namespace ImperaturService
{


    class Program
    {

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ImperaturServiceHost>(isc =>
                {
                    isc.ConstructUsing(name => new ImperaturServiceHost());
                    isc.WhenStarted(tc => tc.Start());
                    isc.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Service for Imperatur Market");
                x.SetDisplayName("ImperaturService");
                x.SetServiceName("Imperatur");
            });
        }
    }
      
}
