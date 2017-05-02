using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using System.Security.Cryptography;

namespace TaskLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Single)]
    public class Service1 : IService1
    {
        //Interval of timer (in seconds) 
        private static int tInterval = 10;

        //difficulty (also change bits in block)
        private static int diff = 15;

        private static readonly List<IClientCallback> CallbackChannels = new List<IClientCallback>();
        private static BitcoinBlock currentBlock = InitializeBlock();
        private static bool initialized = init();
        private static Timer mainT;
        private static UInt32 _time;

        /// <summary>
        /// Client must call this method to be notificated by callbacks
        /// </summary>
        public void RegisterMe()
        {
            //Get callback channel from client
            var channel = OperationContext.Current.GetCallbackChannel<IClientCallback>();

            if (!CallbackChannels.Contains(channel)) CallbackChannels.Add(channel);
        }

        /// <summary>
        /// Get current block to start mining
        /// </summary>
        /// <returns>BitcoinBlock structure with startup nonce for mining</returns>
        public BitcoinBlock GetCurrentBlock()
        {
            //get callback and find a range of client in list
            var channel = OperationContext.Current.GetCallbackChannel<IClientCallback>();
            var i = CallbackChannels.IndexOf(channel);

            //clone block with modified nonce for him
            var cBlock = (BitcoinBlock)currentBlock.Clone();
            if (i > 0)
            {
                cBlock.Nonce = (UInt32)(UInt32.MaxValue / CallbackChannels.Count * i);
            }

            return cBlock;
        }

        public void ProposeAnswer(UInt32 time, UInt32 nonce)
        {
            if (time != _time) return;

            //check answer
            var cBlock = (BitcoinBlock)currentBlock.Clone();
            cBlock.Time = time;
            cBlock.Nonce = nonce;

            // on error do nothing
            if (!checkBlock(cBlock)) return;

            // if true - generate new block and announce about block finding
            BitcoinBlock newB = new BitcoinBlock();
            newB.Version = cBlock.Version;
            newB.hashPrevBlock = getHash(cBlock);
            newB.hashMerkleRoot = cBlock.hashMerkleRoot; //for simplicity do not implement real merkle root tree now
            newB.Time = _time;
            newB.Bits = cBlock.Bits; //target is also not raising
            newB.Nonce = 0;

            currentBlock = newB;
            foreach (var x in CallbackChannels)
            {
                try
                {
                    x.BlockFound();
                }
                catch (CommunicationException) { }
            }
        }

        // Initialize block at startup
        private static BitcoinBlock InitializeBlock()
        {
            BitcoinBlock cur = new BitcoinBlock();
            cur.Version = 1;
            cur.hashPrevBlock = StringToByteConvert("00000000000008df4269884f1d3bfc2aed3ea747292abb89be3dc3faa8c5d26f");
            cur.hashMerkleRoot = StringToByteConvert("be0b136f2f3db38d4f55f1963f0acac506d637b3c27a4c42f3504836a4ec52b1");
            cur.Time = _time;
            cur.Bits = 436956491;
            cur.Nonce = 0;
            return cur;
        }

        //initialization of service
        private static bool init()
        {
            _time = (UInt32)DateTimeOffset.Now.ToUnixTimeSeconds();
            mainT = new Timer();
            mainT.Elapsed += new ElapsedEventHandler(TimerTick);
            mainT.Interval = tInterval * 1000;
            mainT.Start();
            return true;
        }

        //convert string representation of bytes to bytes
        private static byte[] StringToByteConvert(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        // Handler for timer ticking that use callbacks to announce new time to clients
        private static void TimerTick(object source, ElapsedEventArgs e)
        {
            _time += (UInt32)mainT.Interval;
            foreach (var x in CallbackChannels)
            {
                try
                {
                    x.TimeUpdate(_time);
                }
                catch (CommunicationException) { }
            }
        }

        //Check if block have enough leading zeros
        private static bool checkBlock(BitcoinBlock block)
        {
            int i = 0;
            while (getHash(block).ElementAt(i) == 0) i++;
            return i > diff;
        }

        //calculate hash of block
        //do not forget to reverse each field because of little-endian
        private static byte[] getHash(BitcoinBlock block)
        {
            //add version number
            List<byte> hex = BitConverter.GetBytes(block.Version).Reverse().ToList();

            //add hash of previous block
            hex.AddRange(block.hashPrevBlock.Reverse().ToList());

            //add hash of merkle tree
            hex.AddRange(block.hashMerkleRoot.Reverse().ToList());

            //add timestamp
            hex.AddRange(BitConverter.GetBytes(block.Time).Reverse().ToList());

            //add bits
            hex.AddRange(BitConverter.GetBytes(block.Bits).Reverse().ToList());

            //add nonce
            hex.AddRange(BitConverter.GetBytes(block.Bits).Reverse().ToList());

            var x = new SHA256Managed();

            //compute double hash
            return x.ComputeHash(x.ComputeHash(hex.ToArray())).Reverse().ToArray();
        }
    }       
}
