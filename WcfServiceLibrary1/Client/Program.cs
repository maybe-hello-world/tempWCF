using System;
using System.Collections.Generic;
using System.Linq;
using Client.ServiceReference;
using System.Security.Cryptography;
using System.Timers;

namespace Client
{
    class Program
    {
        private static BitcoinBlock currentBlock;
        static Service1Client client;
        static Timer mainTimer;
        private static int _int = 10;
        private volatile static bool newBlockAv;
        private static Guid myID;

        //For simplicity now (instead of bits counting)
        private static UInt32 diff = 3;

        static void Main(string[] args)
        {
            //ask for new block every 10 seconds
            mainTimer = new Timer();
            mainTimer.Elapsed += new ElapsedEventHandler(TimerTick);
            mainTimer.Interval = _int * 1000;
            mainTimer.Start();

            client = new Service1Client();

            //register client on server
            myID = client.RegisterMe();

            //start mining new block
            newBlockAv = true;
            MineBlocks();
        }

        //Method for mining blocks
        private static void MineBlocks()
        {
            //iterations of sha256(sha256(block))
            while (true)
            {
                if (newBlockAv)
                {
                    currentBlock = client.GetCurrentBlock(myID);
                    
                    //if occasionally server didn't answer to us
                    if (currentBlock == null)
                    {
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }

                    Console.WriteLine("New block received: ");
                    writeBlock();
                    
                    newBlockAv = false;
                }

                if (!checkBlock(currentBlock))
                {
                    currentBlock.Nonce++;
                }
                else
                {
                    Console.WriteLine("Block found!");
                    writeBlock();
                    client.ProposeAnswer(myID, currentBlock.Time, currentBlock.Nonce);
                    newBlockAv = true;

                    //debug
                    Console.WriteLine("Time: " + currentBlock.Time + ", Nonce: " + currentBlock.Nonce);
                }
            }
        }

        //Check if block have enough leading zeros
        private static bool checkBlock(BitcoinBlock block)
        {
            int i = 0;
            var arr = getHash(block);
            while (arr.ElementAt(i) == 0) i++;
            return i >= diff;
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
            hex.AddRange(BitConverter.GetBytes(block.Nonce).Reverse().ToList());

            var x = new SHA256Managed();

            //compute double hash
            return x.ComputeHash(x.ComputeHash(hex.ToArray())).Reverse().ToArray();
        }
        
        // Handler for timer ticking that use callbacks to announce new time to clients
        private static void TimerTick(object source, ElapsedEventArgs e)
        {
            newBlockAv = true;
        }

        private static void writeBlock()
        {
            Console.WriteLine();
            Console.WriteLine("Version: " + currentBlock.Version);
            Console.WriteLine("Hash of Merkle root: " + String.Join("", currentBlock.hashMerkleRoot.Select(p => p.ToString()).ToArray()));
            Console.WriteLine("Hash of previous block: " + String.Join("", currentBlock.hashPrevBlock.Select(p => p.ToString()).ToArray()));
            Console.WriteLine("Bits: " + currentBlock.Bits);
            Console.WriteLine("Time: " + currentBlock.Time);
            Console.WriteLine("Nonce: " + currentBlock.Nonce);
            Console.WriteLine();
        }
    }
}
