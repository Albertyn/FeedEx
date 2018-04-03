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

            // Console.WriteLine("----------------------1981----------------------");
            FeedBuilder builder = new FeedBuilder();
            builder.Build(ref txtUsers, ref txtTweets, true);
            builder.Render(true);         


            // Console.WriteLine("----------------------2005----------------------");
            // Bouwer bou = new Bouwer();
            // bou.Build(ref txtUsers, ref txtTweets, true);
            // bou.Render(true);


            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }


    }
}

/* Also see ASSUMPTIONS */
