using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FeedEx
{        class FeedBuilder
        {
            // Step 1 : Read Data into structures that are enumerable/queryable and stable            
            // public List<Tuple<string, string[]>> Users { get; private set; }
            public Tuple<string, string[]> [] Users { get; private set; }

            // public List<Tuple<string, string>> Tweets { get; private set; }
            public Tuple<string, string> [] Tweets { get; private set; }
            public string[] Problems { get; private set; }

            public void Build(ref string[] txtUsers, ref string[] txtTweets, bool Timer = false)
            {
                Stopwatch watch = Stopwatch.StartNew();
                
                Users = new Tuple<string, string[]>[txtUsers.Length]; // username / username[] (followers) 
                Tweets = new Tuple<string, string>[txtTweets.Length]; // username / tweet

                var _feed = new List<Tuple<string, string[]>>();
                var _problems = new List<string>();
                
                int index, length;
                string line, username, tweet;
                string[] follows;

                try{

                    for (int i = 0; i < txtTweets.Length; i++) // Index Data 
                    {
                        line = txtTweets[i];
                        index = line.ToLower().IndexOf(">");

                        // Check for well formed line. 
                        // 1. If ">" exists in string, then index is 0 or a positive number.
                        // 2. Must not be at frist position in string or 0, else there would be no substring to read as username.
                        if (index > 0) // not -1
                        {
                            username = line.Substring(0, index).Trim();
                            tweet = line.Substring(index + 2); // one space " "  + 1 ;)

                            // sanitize input 
                            if (username.IndexOf(" ") > -1) throw new InvalidCastException(); 
                            if (String.IsNullOrEmpty(tweet)) throw new InvalidOperationException();

                            // determine length of string : prevent index out of range ex.
                            length = (tweet.Length > 140) ? 140 : tweet.Length - 1;

                            Tweets[i] = new Tuple<string, string>(username, tweet.Substring(0, length));
                        }
                    }

                    for (int i = 0; i < txtUsers.Length; i++) // Build Master Index for Output
                    {
                        line = txtUsers[i];
                        index = line.ToLower().IndexOf("follows");

                        // Check for well formed line. 
                        // 1. If "follows" exists in string, then index is a positive number.
                        // 2. Must not be at frist position in string or 0, else there would be no substring to read as name.
                        if (index > 0)
                        {
                            username = line.Substring(0, index).Trim();
                            follows = line.Substring(index + 7).Replace(" ", "").Split(",");

                            Users[i] = new Tuple<string, string[]>(username, follows);
                        }
                    }
                    foreach (var uqname in unicorns())
                        _feed.Add(new Tuple<string, string[]>(uqname,zombies(uqname)));
                    
                    // Asign the reduced/aggrigated Collection to Array member var
                    Users = _feed.ToArray();

                }
                catch (Exception e)
                {
                    _problems.Add($"Type:{e.GetType().Name} > Message:{e.Message}");
                }

                this.Problems = _problems.ToArray();
                if (Timer) 
                    Console.WriteLine($"Build() Time: {watch.ElapsedMilliseconds}ms | t:{watch.ElapsedTicks}\n");
            }
            private string[] unicorns() => (Users.Select(i => i.Item1).Concat(Users.SelectMany(u => u.Item2))).Distinct().ToArray();

            private string[] zombies(string uqname) => (Users.Where(u => u.Item1 == uqname)).SelectMany(u => u.Item2).Distinct().ToArray();

            public void Render(bool Timer = false)
            {
                Stopwatch watch = Stopwatch.StartNew();

                // Init & Get List of unique usernames / foreach loop.
                foreach (var u in Users.OrderBy(u => u.Item1))
                {
                    Console.WriteLine(u.Item1);
                    
                    foreach (var t in Tweets.Where(t => t.Item1 == u.Item1 || u.Item2.Contains(t.Item1)))
                        Console.WriteLine($"\t @{t.Item1}: {t.Item2}");
                }
                if (Timer) 
                    Console.WriteLine($"\nRender() Time: {watch.ElapsedMilliseconds}ms | t:{watch.ElapsedTicks}");
            }
        }


}