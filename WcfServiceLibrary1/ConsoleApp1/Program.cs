using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using WcfServiceLibrary1;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a URI to serve as the base address
            Uri httpUrl = new Uri("http://localhost:8010/");

            //Create ServiceHost
            ServiceHost host = new ServiceHost(typeof(Service1));
            host.Description.Endpoints.Clear();

            //Add a service endpoint
            host.AddServiceEndpoint(typeof(IService1), new WSHttpBinding(), httpUrl);

            //Start the Service
            host.Open();

            Console.WriteLine("Service is host at " + DateTime.Now.ToString());
            Console.WriteLine("Host is running... Press <Enter> key to stop");
            Console.ReadLine();
        }
    }
}
