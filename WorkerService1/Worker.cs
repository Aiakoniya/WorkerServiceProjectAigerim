using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using EmailService;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmailSender _emailSender;
        private FileSystemWatcher watcher;
        private readonly string directory = @"C:\Users\aiger\Desktop\MyFolder";

        public Worker(ILogger<Worker> logger, IEmailSender emailSender = null)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = directory;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnChanged;
            watcher.Changed += OnChanged;
            return base.StartAsync(cancellationToken);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            SendEmail(e.FullPath);
        }

        private void SendEmail(string fullPath)
        {
            _logger.LogInformation("A new message is about to be sent at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] { "aigerima.nurlanova@gmail.com" }, "My to do list","There is my plans for the week", fullPath);
            _emailSender.SendEmail(message);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                watcher.EnableRaisingEvents = true;
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }


    }
}
