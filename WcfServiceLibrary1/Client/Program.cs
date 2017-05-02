using System;
using System.Collections.Generic;
using System.Linq;
using Client.TaskLibrary;
using TaskLibrary;
using System.ServiceModel;
using System.Security.Cryptography;

namespace Client
{
    class MyCallback : IClientCallback
    {
        public event EventHandler OnBlockFoundEvent;
        public event TimeUpdateEvent OnTimeUpdateEvent;

        public delegate void TimeUpdateEvent(UInt32 time);

        public void BlockFound()
        {
            OnBlockFoundEvent(this, EventArgs.Empty);
        }

        public void TimeUpdate(UInt32 time)
        {
            OnTimeUpdateEvent(time);
        }
    }
    class Program
    {
        private static Client.TaskLibrary.BitcoinBlock currentBlock;
        static Service1Client client;
        static volatile bool newBlockAvailable = false;

        //For simplicity now (instead of bits counting)
        private static UInt32 diff = 15;

        static void Main(string[] args)
        {
            //Client.TaskLibrary
            //create context and client
            var callback = new MyCallback();
            var context = new InstanceContext(callback);
            client = new Service1Client(context);

            //register events
            callback.OnTimeUpdateEvent += new MyCallback.TimeUpdateEvent(TimeUpdate);
            callback.OnBlockFoundEvent += new EventHandler(BlockFound);

            //register client on server
            client.RegisterMe();

            //set flag for requesting new block
            newBlockAvailable = true;

            //start mining new block
            MineBlocks();
        }

        //on time update set new time to current block
        static private void TimeUpdate(UInt32 time)
        {
            if (currentBlock != null) currentBlock.Time = time;
        }

        //stop mining current block, get new and start mining it
        static private void BlockFound(object sender, EventArgs e)
        {
            newBlockAvailable = true;
        }

        //Method for mining blocks
        private static void MineBlocks()
        {
            //iterations of sha256(sha256(block))
            while (true)
            {
                if (newBlockAvailable)
                {
                    currentBlock = client.GetCurrentBlock();
                    
                    //if occasionally server didn't answer to us
                    if (currentBlock == null)
                    {
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }

                    newBlockAvailable = false;
                }

                if (!checkBlock(currentBlock))
                {
                    currentBlock.Nonce++;
                }
                else
                {
                    client.ProposeAnswer(currentBlock.Time, currentBlock.Nonce);
                    newBlockAvailable = true;

                    //debug
                    Console.WriteLine("Time: " + currentBlock.Time + ", Nonce: " + currentBlock.Nonce);
                }
            }
        }

        //Check if block have enough leading zeros
        private static bool checkBlock(Client.TaskLibrary.BitcoinBlock block)
        {
            int i = 0;
            while (getHash(block).ElementAt(i) == 0) i++;
            return i > diff;
        }

        //calculate hash of block
        //do not forget to reverse each field because of little-endian
        private static byte[] getHash(Client.TaskLibrary.BitcoinBlock block)
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
