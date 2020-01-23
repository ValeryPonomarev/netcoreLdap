using Newtonsoft.Json.Linq;
using System;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using Novell.Directory.Ldap;

namespace netLdap
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Input LDAP url:");
                AdService adService = new AdService(Console.ReadLine());
                adService.HandleCommand();

                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("FATAL ERROR:");
                Console.WriteLine(e.ToString());
                Console.WriteLine("Press enter to exit");
            }
            Console.ReadLine();

        }
    }

    class AdService
    {
        private String url;
        private LdapConnection connect;

        public AdService(string url)
        {
            this.url = url;
        }

        public void HandleCommand()
        {
            WriteHelp();
            while (true)
            {
                Console.WriteLine("Input your command:");
                var cmd = Console.ReadLine();

                if(cmd.Equals("0"))
                {
                    Logout();
                    Console.WriteLine("Goodbye!");
                    break;
                }

                switch (cmd)
                {
                    case "1":
                        Login();
                        break;
                    case "2":
                        Logout();
                        break;
                    case "3":
                        Query();
                        break;
                    case "9":
                        WriteHelp();
                        break;
                    default:
                        Console.WriteLine("Incorrect command");
                        WriteHelp();
                        break;
                }
            }
        }

        private void Login()
        {
            Console.WriteLine("Input login:");
            var login = Console.ReadLine();

            Console.WriteLine("Input password:");
            var password = Console.ReadLine();

            connect.Connect(url, 389);
            connect.Bind(login, password);
            Console.WriteLine("Authorization success");
        }

        private void Logout()
        {
            if(connect != null)
            {
                connect.Dispose();
            }
            Console.WriteLine("Logout");
        }

        private void Query()
        {
            try
            {
                Console.WriteLine("Input path:");
                var path = Console.ReadLine();

                Console.WriteLine("Input your query:");
                var query = Console.ReadLine();

                Console.WriteLine("Recursive search? (y/n):");
                var searchStrategy = LdapConnection.SCOPE_ONE;
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    searchStrategy = LdapConnection.SCOPE_SUB;
                }
                Console.WriteLine();

                LdapSearchConstraints c = new LdapSearchConstraints();
                c.MaxResults = 50;

                LdapSearchResults searchResults = this.connect.Search(path, searchStrategy, query, null, false, c);
                while (searchResults.hasMore())
                {
                    LdapEntry entry = null;
                    try
                    {
                        entry = searchResults.next();
                        //Console.WriteLine(entry.getAttribute("name"));
                        Console.WriteLine(entry.ToString());
                        Console.WriteLine();
                        Console.WriteLine("===============");
                        Console.WriteLine();
                    }
                    catch (LdapException e)
                    {
                        Console.WriteLine("Error: " + e.LdapErrorMessage);
                    }

                    /*
                    // Get the attribute set of the entry
                    LdapAttributeSet attributeSet = entry.getAttributeSet();
                    System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();

                    // Parse through the attribute set to get the attributes and the corresponding values
                    while (ienum.MoveNext())
                    {
                        LdapAttribute attribute = (LdapAttribute)ienum.Current;
                        string attributeName = attribute.Name;
                        string attributeVal = attribute.StringValue;
                        Console.WriteLine(attributeName + "value:" + attributeVal);
                    }
                    */
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Query ERROR:");
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }

        private void WriteHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("[1] - Login");
            Console.WriteLine("[2] - Logout");
            Console.WriteLine("[3] - Query");
            Console.WriteLine("[9] - Help");
            Console.WriteLine("[0] - Exit");
        }
    }
}
