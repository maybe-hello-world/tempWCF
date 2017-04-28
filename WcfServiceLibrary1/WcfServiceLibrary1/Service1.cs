using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfServiceLibrary1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Single)]
    public class Service1 : IService1
    {
        List<Guid> clients = new List<Guid>();

        public Guid RegisterMe()
        {
            var x = (OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty);
            Console.WriteLine(x.Address);
            Console.WriteLine(x.Port);
            var ans = Guid.NewGuid();
            clients.Add(ans);
            return ans;
        }
    }       
}
