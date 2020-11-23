using System;

namespace RealTimeMessaging.Orchestrator.Interfaces
{
    public interface IMessagingQueue
    {
        void Send(IMessage message);
        void Listen(Action<string> eventHandler, Action<string> errorHandler);
    }
}
