using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.TaskLibrary;
using TaskLibrary;
using System.ServiceModel;

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
        private static Client.TaskLibrary.IService1 m_Service;
        private static Client.TaskLibrary.BitcoinBlock currentBlock;
        static Service1Client client;

        static void Main(string[] args)
        {
            //create context and client
            var callback = new MyCallback();
            var context = new InstanceContext(callback);
            client = new Service1Client(context);

            //register events
            callback.OnTimeUpdateEvent += new MyCallback.TimeUpdateEvent(TimeUpdate);
            callback.OnBlockFoundEvent += new EventHandler(BlockFound);

            //register client on server
            client.RegisterMe();

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
            throw new NotImplementedException();
        }

        //Method for mining blocks
        static private void MineBlocks()
        {
            throw new NotImplementedException();
            currentBlock = client.GetCurrentBlock();

        }
    }
}
