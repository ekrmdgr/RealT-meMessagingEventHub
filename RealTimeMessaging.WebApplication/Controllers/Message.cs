using RealTimeMessaging.Orchestrator.Interfaces;

namespace RealTimeMessaging.WebApplication.Controllers
{
    internal class Message : IMessage
    {
        public string Body { get; set; }
    }
}