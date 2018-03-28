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
        class FeedBuilder
        {
            // Step 1 : Read Data into structures that are enumerable/queryable and stable
            public List<Tuple<string, string[]>> Users { get; private set; }
            public List<Tuple<string, string>> Tweets { get; private set; }

            public long Build(ref string[] txtUsers, ref string[] txtTweets)
            {
                Stopwatch watch = Stopwatch.StartNew();

                Users = new List<Tuple<string, string[]>>();
                Tweets = new List<Tuple<string, string>>();

                int index, length;
                string line, username, tweet;
                string[] follows;

                for (int i = 0; i < txtUsers.Length; i++)
                {
                    line = txtUsers[i];
                    index = line.ToLower().IndexOf("follows");

                    // check for well formed line. 
                    // 1. If "follows" exists in string, then index is a positive number.
                    // 2. Must not be at frist position in string or 0, else there would be no substring to read as name.
                    if (index > 0)
                    {
                        username = line.Substring(0, index).Trim();
                        follows = line.Substring(index + 7).Replace(" ", "").Split(",");

                        Users.Add(new Tuple<string, string[]>(username, follows));
                        
                        /*
                        try
                        {
                            // determine if the user already appears in the master list.
                            // There should not be multiple instances of the same user in the array - Single() will throw an exeption!
                            var Temp = Users.Where(u => u.Item1.ToLower() == username.ToLower()).Single();

                            // Union to determine the users they follow
                            Temp = new Tuple<string, string[]>(username, Temp.Item2.Concat(follows).Distinct().ToArray());
                        }
                        catch
                        {
                            Users.Add(new Tuple<string, string[]>(username, follows));
                        }
                        */
                    }
                }
                for (int i = 0; i < txtTweets.Length; i++)
                {
                    line = txtTweets[i];
                    index = line.ToLower().IndexOf(">");

                    // check for well formed line. 
                    if (index > 0)
                    {
                        username = line.Substring(0, index).Trim();
                        tweet = line.Substring(index + 1); // thanks for the breadcrumbs ;)

                        // 2 determine length of string : prevent index out of range ex.
                        length = (tweet.Length > 140) ? 140 : tweet.Length - 1;

                        Tweets.Add(new Tuple<string, string>(username, tweet.Substring(0, length)));
                    }
                }

//                var Followers = Users.SelectMany(i => i.Item2).Distinct().ToArray();
//                var onlyFollowers = Followers.Where(f => !Users.Select(u => u.Item1).Contains(f));

                return watch.ElapsedMilliseconds;

            }

            public long Render()
            {
                Stopwatch watch = Stopwatch.StartNew();

                // Display the file contents by using a foreach loop.
                foreach (var tu in Users.OrderBy(tu => tu.Item1))
                {
                    Console.Write($"@{tu.Item1}");
                    Console.WriteLine($"\t follows {string.Join("|", tu.Item2)}");

                    foreach (var t in Tweets.Where(t => t.Item1 == tu.Item1 || tu.Item2.Contains(t.Item1)))
                        Console.WriteLine($"\t @{tu.Item1}: {t.Item2}");
                }

                return watch.ElapsedMilliseconds;
            }
        }

        static void Main(string[] args)
        {            
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine($"Welcome to a twitter-feed examlple!\nPress any key to continue...");
            Console.ReadKey(true);
            
            string[] txtUsers, txtTweets;
            long BuildTime, RenderTime;

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
                Console.WriteLine($"Error: incorrect loading of input data,\nMessage: {e.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("----------------------1981----------------------");
            FeedBuilder builder = new FeedBuilder();

            BuildTime = builder.Build(ref txtUsers, ref txtTweets);
            RenderTime = builder.Render();

            Console.WriteLine($"BuildTime: {BuildTime}ms");
            Console.WriteLine($"BuildTime: {RenderTime}ms");


            Console.WriteLine("----------------------2005----------------------");
            FB fb = new FB();
            BuildTime = builder.Build(ref txtUsers, ref txtTweets);
            RenderTime = builder.Render();

            Console.WriteLine($"BuildTime: {BuildTime}ms");
            Console.WriteLine($"BuildTime: {RenderTime}ms");

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

        public class TwitterUser
        {
            public string Name { get; set; }
            public string[] Follows { get; set; }
            public List<string> Tweets { get; set; }
        }
        class FB
        {
            public TwitterUser TempUser { get; private set; }
            public List<TwitterUser> UserList { get; private set; }

            public long Build(ref string[] txtUsers, ref string[] txtTweets)
            {
                Stopwatch watch = Stopwatch.StartNew();

                int index, length;
                string username, tweet;
                string[] following;

                foreach (string line in txtUsers)
                {
                    index = line.ToLower().IndexOf("follows");

                    // check for well formed line. 
                    // 1.Delimiter exists in string. (if it's index is a positive number)
                    // 2.Delimiter not at frist position in string. (If this was true, there would be no substring to read as name)
                    if (index > 0)
                    {
                        username = line.Substring(0, index).Trim();
                        following = line.Substring(index + 7).Replace(" ", "").Split(",");

                        // determine if the user already appears in the master list.
                        try
                        {
                            // There should not be multiple instances of the same user in the array - Single() will throw an exeption!
                            TempUser = UserList.Where(u => u.Name.ToLower() == username.ToLower()).Single();

                            // Union to determine the users they follow
                            TempUser.Follows = TempUser.Follows.Concat(following).Distinct().ToArray();
                        }
                        catch
                        {
                            UserList.Add(new TwitterUser { Name = username, Follows = following });
                        }
                    }
                }
                
                foreach (string line in txtTweets)
                {
                    index = line.ToLower().IndexOf(">");

                    // check for well formed line. 
                    if (index > 0)
                    {
                        username = line.Substring(0, index).Trim();
                        tweet = line.Substring(index + 1); // thanks for the breadcrumbs ;)

                        // 2 determine length of string : prevent index out of range ex.
                        length = (tweet.Length > 140) ? 140 : tweet.Length - 1;
                        var x = UserList.Where(u => u.Name == username).Single().Tweets;
                        if(x == null) x = new List<string>();
                        x.Add(tweet.Substring(0, length));

                        // Tweets.Add(new Tuple<string, string>(username, tweet.Substring(0, length)));
                    }
                }
                return watch.ElapsedMilliseconds;
            }
            public long Render()
            {
                Stopwatch watch = Stopwatch.StartNew();

                // Display the file contents by using a foreach loop.
                foreach (var tu in UserList.OrderBy(tu => tu.Name))
                {
                    Console.Write($"@{tu.Name}");
                    Console.WriteLine($"\t follows {string.Join("|", tu.Follows)}");

                    foreach (var t in tu.Tweets)
                        Console.WriteLine($"\t @{tu.Name}: {t}");
                }

                return watch.ElapsedMilliseconds;
            }
        }

    }
}

/* ASSUMPTIONS 

    == User names ==
    
    Are unique strings.
    Contain NO spaces.
    Minimum length is one character.

    <Users.txt> the word 'follows' appears even after a username even if the user does not follow anyone.



    == Tweets ==
    Do not have to trim() whitespace for tweet. 140 char.

    <Tweets.txt> 
        1. there are no empty tweets.
        2. All tweets are by users found in <Users.txt>

    Users.txt serves as master, for only tweets by users found therein will be written to the console.
    If Tweets.txt contain messages by other Users not found in Users.txt they will be ignored.

    On the use of Tuples vs. POCO classes : Tuples for performance / poco's for readable code...
    Also use the var keyword sparingly
*/
