using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_1.ServiceReference;

namespace Client_1
{
    class Program
    {
        static async Task MainAsync(string[] args)
        {
            Service1Client client = new Service1Client();
            var x = await client.RegisterMeAsync();
        }
    }
}
