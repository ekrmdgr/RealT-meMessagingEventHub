using RealTimeMessaging.Orchestrator.Interfaces;
using RealTimeMessaging.Orchestrator.MessagingQueues;
using System;

namespace RealTimeMessaging.WebApplication
{
    public class MessagingQueue
    {
        public void Send()
        {

        }

        public void Listen(Action<string> eventHandler, Action<string> errorHandler)
        {
            //var messagingQueue = new EventHubQueue(eventHandler, errorHandler);
            //messagingQueue.Listen();
        }
    }
}
