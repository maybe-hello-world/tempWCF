using System.Runtime.Serialization;
using System.ServiceModel;
using System;

namespace TaskLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    // Announce that we use callbacks of certain type
    [ServiceContract]
    public interface IService1
    {
        /// <summary>
        /// Register client to raise callbacks to him
        /// </summary>
        [OperationContract]
        Guid RegisterMe();

        /// <summary>
        /// Get current block to be mined
        /// </summary>
        /// <returns>Block that is to be mined. In Nonce field stored initial value for mining.</returns>
        [OperationContract]
        BitcoinBlock GetCurrentBlock(Guid id);

        /// <summary>
        /// Propose answer for current mining block
        /// </summary>
        /// <param name="time">Current timestamp for checking on server</param>
        /// <param name="nonce">Nonce of answer</param>
        [OperationContract]
        void ProposeAnswer(Guid id, UInt32 time, UInt32 nonce);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "WcfServiceLibrary1.ContractType".
    [DataContract]
    public class BitcoinBlock
    {
        //Block version number
        [DataMember]
        public UInt32 Version;

        //hashPrevBlock 
        [DataMember]
        public byte[] hashPrevBlock;

        //256-bit hash based on all of the transactions in the block
        [DataMember]
        public byte[] hashMerkleRoot;

        //Current timestamp as seconds since 1970-01-01T00:00 UTC (unix time)
        [DataMember]
        public UInt32 Time;

        //Current target in compact format 
        /*The compact format of target is a special kind of floating-point encoding
         * using 3 bytes mantissa, the leading byte as exponent (where only the 5
         * lowest bits are used) and its base is 256.*/
        [DataMember]
        public UInt32 Bits;

        //32-bit number (starts at 0)
        //An item to be found by client
        [DataMember]
        public UInt32 Nonce;
    }
}
