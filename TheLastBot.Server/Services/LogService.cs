using System;
using System.Threading.Tasks;
using Discord;

namespace TheLastBot.Server.Services
{
    public class LogService
    {
        public static Task Write(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}