using System;
using System.Collections.Generic;
using WebApp.Models;

namespace WebApp
{
    /// <summary>
    /// A dummy Database Layer
    /// </summary>
    public class DBL
    {
        public static List<Project> GetProjects()
        {
            return dummyProjects;
        }

        static DBL()
        {
            dummyProjects = new List<Project>
            {
                new Project
                {
                    Id = 1,
                    Name = "console",
                    Description = "Console Application",
                    PublishDateTime = new DateTime(1992, 1, 2)
                },
                new Project
                {
                    Id = 2,
                    Name = "classlib",
                    Description = "Class library",
                    PublishDateTime = new DateTime(2003, 2, 4)
                },
                new Project
                {
                    Id = 3,
                    Name = "wpf",
                    Description = "WPF Application",
                    PublishDateTime = new DateTime(2008, 10, 4)
                },
                new Project
                {
                    Id = 4,
                    Name = "winforms",
                    Description = "Windows Forms Application",
                    PublishDateTime = new DateTime(2005, 5, 7)
                },
                new Project
                {
                    Id = 5,
                    Name = "webapp",
                    Description = "ASP.NET Core Web App",
                    PublishDateTime = new DateTime(2010, 4, 25)
                },
            };
        }

        private static List<Project> dummyProjects;
    }
}