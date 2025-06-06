﻿using System;
using System.Collections.Generic;
using System.Net;

namespace SSRS_Permission
{
    class Program
    {
        static void Main(string[] args)
        {
            // تنظیمات اتصال به سرور
            //var reportServiceUrl = "http://bi/ReportServer/ReportService2010.asmx";
            //var username = "hamid.doostparvar"; // نام کاربری برای احراز هویت
            
            //var password = "Pass"; // رمز عبور برای احراز هویت
            //var domain = "DomainName"; // دامنه (در صورت نیاز)

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Insert the reportServiceUrl:"); //like "http://bi/ReportServer/ReportService2010.asmx";
            Console.ForegroundColor = ConsoleColor.White;
            var reportServiceUrl = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Insert username:");
            Console.ForegroundColor = ConsoleColor.White;
            var username = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Insert password:");
            Console.ForegroundColor = ConsoleColor.White;
            var password = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Insert domain:");
            Console.ForegroundColor = ConsoleColor.White;
            var domain = Console.ReadLine();

            // ایجاد یک نمونه از سرویس گزارش‌دهی
            var reportService = new ReportingService2010
            {
                Url = reportServiceUrl,
                Credentials = new NetworkCredential(username, password, domain)
            };


            // لیست کاربرانی که می‌خواهید به آن‌ها دسترسی دهید
            var users = new List<string>();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Insert the users with \",\" seperators:"); // like :DomainName\Zeynab.Takallou,DomainName\Maryam.Sheikh,DomainName\Alireza.Ghorbani,DomainName\nikki.changizi,DomainName\fatemeh.golshani
            Console.ForegroundColor = ConsoleColor.White;

            string input = Console.ReadLine(); // دریافت ورودی از کاربر
            string[] userArray = input.Split(',');
            foreach (var user in userArray)
            {
                users.Add(user.Trim()); // Trim() برای حذف فاصله‌های اضافی
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Specify the Path:"); // Like: /Development/Optime
            Console.ForegroundColor = ConsoleColor.White;

            var itemPath = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Specify the Role:"); // Like : Browser
            Console.ForegroundColor = ConsoleColor.White;

            var Role = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Gray;

            // اعطای دسترسی "Browser" به کاربران
            foreach (var user in users)
            {
                try
                {
                    // دریافت سیاست‌های دسترسی فعلی
                    var policies = reportService.GetPolicies(itemPath, out bool inheritParent);

                    bool userAlreadyHasAccess = false;
                    foreach (var policy in policies)
                    {
                        if (policy.GroupUserName.Equals(user, StringComparison.OrdinalIgnoreCase))
                        {
                            userAlreadyHasAccess = true;
                            break;
                        }
                    }

                    if (userAlreadyHasAccess)
                    {
                        Console.WriteLine($"User {user} already has access. Skipping...");
                    }
                    else
                    {
                        var newPolicy = new Policy
                        {
                            GroupUserName = user,
                            Roles = new Role[] { new Role { Name = Role } }
                        };

                        var newPolicies = new Policy[policies.Length + 1];
                        policies.CopyTo(newPolicies, 0);
                        newPolicies[policies.Length] = newPolicy;

                        reportService.SetPolicies(itemPath, newPolicies);

                        Console.WriteLine($"Access granted to user: {user}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error granting access to user {user}: {ex.Message}");
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Access management completed.");
            Console.ReadLine();
        }
    }
}