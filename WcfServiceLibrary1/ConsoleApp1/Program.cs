using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using TaskLibrary;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a URI to serve as the base address
            Uri httpUrl = new Uri("http://localhost:8733/Design_Time_Addresses/WcfServiceLibrary1/Service1/");

            //Create ServiceHost
            using (ServiceHost host = new ServiceHost(typeof(Service1), httpUrl))
            {
                //create and add behavior
                var smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                host.AddDefaultEndpoints();

                foreach (var ep in host.Description.Endpoints)
                {
                    ep.Binding = new BasicHttpBinding()
                    {
                        CloseTimeout = new TimeSpan(0, 1, 0),
                        MessageEncoding = WSMessageEncoding.Text,
                        TextEncoding = System.Text.Encoding.UTF8,
                        OpenTimeout = new TimeSpan(0, 3, 0),
                        ReceiveTimeout = new TimeSpan(0, 1, 0)
                    };
                }

                host.Open();

                Console.WriteLine("Service is host at " + DateTime.Now.ToString());
                PrintServiceInfo(host);
                Console.WriteLine("Host is running... Press <Enter> key to stop");
                Console.ReadLine();

                host.Close();
            }
        }

        static void PrintServiceInfo(ServiceHost host)
        {
            Console.WriteLine("{0} is up and running with following endpoint(s)-", host.Description.ServiceType);

            foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
                Console.WriteLine("A-> {0}, B-> {1}, C-> {2}", endpoint.Address, endpoint.Binding.Name, endpoint.Contract.Name);
            Console.WriteLine();
        }
    }
}
