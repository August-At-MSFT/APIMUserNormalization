﻿using System;
using System.Collections.Generic;
using System.Text;
using APIMUserNormalization.Services;
using System.Threading;
using System.Threading.Tasks;
using APIMUserNormalization.Models;
using System.Collections;
using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace APIMUserNormalization.ConsoleView
{
    public class UserConsole
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        MigrationService migrationService;
        static AppSettings config;
        bool userSetup = false;

        public UserConsole()
        {
            migrationService = new MigrationService();

            // Load configuration
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        public async Task<bool> PrintMainMenu()
        {


            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("API Management and AD B2C Environments list:");
            Console.WriteLine("====================");

            await migrationService.ServiceBootstraping();
            userSetup = true;

            Console.WriteLine("Command  Description");
            Console.WriteLine("====================");


            Console.ForegroundColor = userSetup ? Console.ForegroundColor = ConsoleColor.Gray : Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[A]      Create List of Users");
            if (userSetup)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[B]      Manage Users");
                Console.WriteLine("[C]      Normalize all Users");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[E]      Print Config Attributes");
            Console.WriteLine("[X]      Exit Program Console");
            Console.WriteLine("-------------------------");
            string option = Console.ReadLine();
            bool leaveLoop = false;
            option = option.ToUpper();

            switch (option)
            {
                case "A":
                    Console.WriteLine("This process will take some time, please wait...");
                    await migrationService.SetupUserCollections();
                    //migrationService.PrintUserNormalizationStatus();
                    userSetup = true;
                    Console.WriteLine("Done...");
                    break;
                case "B":
                    await ManageUsersAsync();
                    Console.WriteLine("Done...");
                    break;
                case "C":
                    await migrationService.NormalizeAllUsersAsync();
                    Console.WriteLine("Done...");
                    break;
                case "E":
                    PrintConfig();
                    Console.WriteLine("Done...");
                    break;
                case "X":
                    leaveLoop = true;
                    break;
                default:
                    Console.WriteLine("Option not recognized");
                    break;
            }
            Thread.Sleep(2000);
            if (!leaveLoop)
            {
                Console.Write("Press any key to continue...");
                Console.ReadLine();
            }

            return leaveLoop;

        }

        private async Task ManageUsersAsync()
        {
            Console.WriteLine("");
            Console.WriteLine("====================");
            Console.WriteLine("Choose your user #");
            string idx = Console.ReadLine().ToUpper();
            int userIdx = int.Parse(idx);


            ArrayList userNormalizationList = migrationService.getUserNormalizationList();
            var un = (UserNormalization) userNormalizationList[userIdx - 1];

            log.Info("User Email: " + un.Email);

            Console.Write("    Is user normalized?: ");
            if (!un.IsNormilized)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            log.Info(un.IsNormilized);
            Console.ForegroundColor = ConsoleColor.White;
            log.Info("    Found in " + un.ApimsFound + " APIM Services");
            log.Info("    User with " + un.UniqueIdS + " unique Object IdS");
            log.Info("");
            log.Info("Normalization Plan:");

            string s4 = string.Empty;
            string s5 = string.Empty;
            string s6 = string.Empty;
            foreach (UserNormalizationStatus uns in un.UsersStatus)
            {
                Console.WriteLine("    APIM: " + uns.APIMName + ":");

                if (!uns.ExistsInAPIM) Console.WriteLine("        [APIM] - Create User");
                if (!uns.IsFoundInADB2C) Console.WriteLine("        [ADB2C]- Create User");
                if (!uns.HasADB2C) Console.WriteLine("        [APIM] - Add ADB2C Identity");
                if (!uns.IsEmailFoundInADB2C) Console.WriteLine("        [ADB2C] - Update Properties");
            }

            Console.WriteLine();
            Console.WriteLine("Normalize User? (Y/N)");
            string yn = Console.ReadLine().ToUpper();
            if (yn.ToUpper().Equals("Y"))
            {
                log.Info("Normalize User selected!");

                //ALB: dummy function to create fake users 
                //await migrationService.Create100APIMUsers(un);

                await migrationService.NormalizeUserAsync(un);
            }

        }



        public void PrintConfig()
        {
            log.Info("");
            log.Info("API Mgmt Tenant Id   : " + config.APIMTenantId);

            log.Info("API Instances        : " + config.APIMApiManagementNames.Split(";").Length);
            log.Info("Resource Groups      : " + config.APIMResourceGroups.Split(";").Length);
            log.Info("API Mgmt Client Id   : " + config.APIMClientID);
            log.Info("Azure AD B2C Tenant  : " + config.AADB2CTenantId);
            log.Info("Azure AD Client Id   : " + config.AADB2CAppId);
        }

    }

}




