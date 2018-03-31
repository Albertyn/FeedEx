using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FeedEx
{
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

                List<TwitterUser> TempList = new List<TwitterUser>();

                int index, length;
                string username, tweet;
                string[] following, uqnames;

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
                uqnames = UserList.Select(i => i.Name).Concat(UserList.SelectMany(u => u.Follows)).Distinct().ToArray();
                //var uqUsers = UserList.Select(i => i.Name);
                //var uqFollowers = UserList.SelectMany(u => u.Follows);//.Where(f => !uqUsers.Contains(f));
                //var allUsers = uqUsers.Concat(uqFollowers).Distinct()
                
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