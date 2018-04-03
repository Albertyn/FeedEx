using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FeedEx
{
    class Program
    {
        public class FeedExConfiguration
        {
            public string UsersPath { get; set; }
            public string TweetsPath { get; set; }
        }
        static void Main(string[] args)
        {            
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine($"Welcome to a twitter-feed examlple!\nPress any key to continue...");
            Console.ReadKey(true);
            
            string[] txtUsers, txtTweets;

            try
            {
                // Configuration
                var JSON = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/appsettings.json");
                var Configuration = JsonConvert.DeserializeObject<FeedExConfiguration>(JSON);

                // Read each line of the file into a string array. 
                txtUsers = File.ReadAllLines(Configuration.UsersPath);

                // Each element of the array is one line of the file.
                txtTweets = File.ReadAllLines(Configuration.TweetsPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: loading input data.\n{e.Message}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }


            try 
            {            
                FeedBuilder builder = new FeedBuilder();
                builder.Build(ref txtUsers, ref txtTweets);
                builder.Render();

                /*
                // tuple
                FeedBuilder builder = new FeedBuilder();

                // poco
                Bouwer bou = new Bouwer();

                for(int i = 0; i < 10; i++)
                {
                    
                builder.Build(ref txtUsers, ref txtTweets, true); // uncomment to print duration 
                bou.Build(ref txtUsers, ref txtTweets, true);
                
                }

                builder.Render(true); // uncomment to print duration
                bou.Render(true);
                 */
                

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: processing feed data.\n{e.Message}");
            }

            // Keep the console window open
            Console.WriteLine("Press any key to exit...");
            System.Console.ReadKey();
        }


    }
}

/* Also see ASSUMPTIONS */
