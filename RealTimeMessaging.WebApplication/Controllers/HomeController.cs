using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealTimeMessaging.Orchestrator.Interfaces;
using RealTimeMessaging.WebApplication.Models;
using SignalRChat.Hubs;

namespace RealTimeMessaging.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMessagingQueue _messagingQueue;
        private readonly ChatHub _chatHub;

        public HomeController(ILogger<HomeController> logger, IMessagingQueue messagingQueue, ChatHub chatHub)
        {
            _logger = logger;
            _messagingQueue = messagingQueue;
            _chatHub = chatHub;
        }

        public IActionResult Index()
        {
            _messagingQueue.Listen(EventHandler1, ErrorHandler1);
            return View();
        }

        private void ErrorHandler1(string obj)
        {
            throw new NotImplementedException();
        }

        private async void EventHandler1(string obj)
        {
            await _chatHub.SendMessage("t1", obj.ToString());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
