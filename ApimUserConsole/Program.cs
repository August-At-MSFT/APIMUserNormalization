using APIMUserNormalization.ConsoleView;
using APIMUserNormalization.Models;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace ApimUserConsole
{
    class Program
    {
        
        static async Task Main(string[] args)
        {

            UserConsole userConsole = new UserConsole();

            bool breakFlag = false;

            while (!breakFlag)
            {
                breakFlag = await userConsole.PrintMainMenu();
                //var subs = await subsMigrationService.ListAPIMSubscriptions();
                //await subsMigrationService.NormalizeAllSubscriptionsAsync();
                //breakFlag = true;
            }

        }

    }
}
