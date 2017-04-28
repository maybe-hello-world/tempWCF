using System.Runtime.Serialization;
using System.ServiceModel;
using System;
using System.Collections.Generic;

namespace WcfServiceLibrary1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        // TODO: Add your service operations here
        /// <summary>
        /// Registers clients in database;
        /// </summary>
        /// <returns>Unique GUID for client, that must be used for next operations</returns>
        [OperationContract]
        Guid RegisterMe();
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "WcfServiceLibrary1.ContractType".
    [DataContract]
    public class BitcoinBlock
    {

    }
}
