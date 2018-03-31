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
            
            public List<Tuple<string, string[], string[]>> Feed { get; private set; }
            public List<Tuple<string, string[]>> Users { get; private set; }
            public List<Tuple<string, string>> Tweets { get; private set; }

            public long Build(ref string[] txtUsers, ref string[] txtTweets)
            {
                Stopwatch watch = Stopwatch.StartNew();

                Feed = new List<Tuple<string, string[], string[]>>(); // username / username[] (followers)  / tweet[]
                Users = new List<Tuple<string, string[]>>(); // username / username[] (followers) 
                Tweets = new List<Tuple<string, string>>(); // username / tweet

                int index, length;
                string line, username, tweet;
                string[] follows, uqnames;

                for (int i = 0; i < txtTweets.Length; i++) // Index Data 
                {
                    line = txtTweets[i];
                    index = line.ToLower().IndexOf(">");

                    // Check for well formed line. 
                    // 1. If ">" exists in string, then index is a positive number.
                    // 2. Must not be at frist position in string or 0, else there would be no substring to read as username.
                    if (index > 0)
                    {
                        username = line.Substring(0, index).Trim();
                        tweet = line.Substring(index + 1); // one space " ", thanks for the tip ;)

                        // 2. determine length of string : prevent index out of range ex.
                        length = (tweet.Length > 140) ? 140 : tweet.Length - 1;

                        Tweets.Add(new Tuple<string, string>(username, tweet.Substring(0, length)));
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

                        Users.Add(new Tuple<string, string[]>(username, follows));
                    }
                }
                // 3. Get List of unique usernames
                uqnames = (Users.Select(i => i.Item1).Concat(Users.SelectMany(u => u.Item2))).Distinct().ToArray();
                // 4. Initialize Feed
                for(int i = 0; i < uqnames.Length; i++)
                {
                    follows = Users.Where(u => u.Item1 == uqnames[i]).SelectMany(u => u.Item2).Distinct().ToArray();
                    
                    Feed.Add(
                        new Tuple<string, string[], string[]>(
                            uqnames[i], 
                            follows,
                            Tweets.Where(t => t.Item1 == uqnames[i] || follows.Contains(t.Item1))
                                .Select(t => t.Item2).ToArray()
                            )
                        );
                }
                return watch.ElapsedMilliseconds;
            }

            public long Render()
            {
                Stopwatch watch = Stopwatch.StartNew();

                // Display the file contents by using a foreach loop.
                foreach (var tu in Feed.OrderBy(tu => tu.Item1))
                {
                    Console.Write($"@{tu.Item1}");
                    Console.WriteLine($"\t follows {string.Join("|", tu.Item2)}");

                    foreach (var t in tu.Item3)
                        Console.WriteLine($"\t @{tu.Item1}: {t}");
                }

                return watch.ElapsedMilliseconds;
            }
        }


}