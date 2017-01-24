using System;
using System.Diagnostics;
using Nancy.Hosting.Self;
using Nancy;

namespace ImperaturService
{
    namespace ImperaturService
    {


        public class ImperaturServiceHost
        {
            private NancyHost m_nancyHost;

            public void Start()
            {
                HostConfiguration oh = new HostConfiguration();
                m_nancyHost = new NancyHost(new Uri("http://localhost:8090"));
                m_nancyHost.Start();
            }

            public void Stop()
            {
                m_nancyHost.Stop();
                Console.WriteLine("Stopped. Good bye!");
            }
        }
    }


}
