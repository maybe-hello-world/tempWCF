using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Client.ServiceReference1;

namespace Client
{
    class Program
    {
        static Guid myGuid;
        static void Main(string[] args)
        {
            //create client
            Service1Client client = new Service1Client();

            myGuid = client.RegisterMe();

            while (true)
            {
            }
            
        }
    }
}
