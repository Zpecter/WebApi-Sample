﻿using System;
using System.Configuration;
using Infrastructure;
using Microsoft.Owin.Hosting;
using Serilog;

namespace ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // Specify the URI to use for the local host:
            string baseUri = "http://*:8081/";           

            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .CreateLogger();

            Console.WriteLine("Starting web Server...");
            WebApp.Start<Startup>(baseUri);
            Console.WriteLine("Server running at {0} - press Enter to quit. ", baseUri);
            Console.ReadLine();
        }
    }
}
