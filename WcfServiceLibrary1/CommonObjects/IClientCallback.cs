namespace CommonObjects
{
    using System;
    using System.ServiceModel;
    // Instead of events WCF use callbacks
    public interface IClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void TimeUpdate(UInt32 time);

        [OperationContract(IsOneWay = true)]
        void BlockFound();
    }
}
