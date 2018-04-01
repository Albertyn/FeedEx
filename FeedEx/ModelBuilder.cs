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
        }
        public class Tweet
        {
            public string Name { get; set; }
            public string Message { get; set; }
        }
        class Bouwer
        {
            private static TwitterUser _TempUser = new TwitterUser();
            string[] UserNames {
                get
                {
                    return (UserList.Select(i => i.Name).Concat(UserList.SelectMany(u => u.Follows))).Distinct().ToArray();
                }
            }
            List<TwitterUser> UserList { get; set; }
            List<Tweet> TweetList { get; set; }
            public void Build(ref string[] txtUsers, ref string[] txtTweets, bool Timer = false)
            {
                Stopwatch watch = Stopwatch.StartNew();

                List<TwitterUser> _UserList = new List<TwitterUser>();
                List<Tweet> _TweetList = new List<Tweet>();

                int _index, _length;
                string _username, _tweet;
                string[] _following;

                foreach (string line in txtUsers)
                {
                    _index = line.ToLower().IndexOf("follows");

                    // check for well formed line. 
                    // 1.Delimiter exists in string. (if it's index is a positive number)
                    // 2.Delimiter not at frist position in string. (If this was true, there would be no substring to read as name)
                    if (_index > 0)
                    {
                        _username = line.Substring(0, _index).Trim();
                        _following = line.Substring(_index + 7).Replace(" ", "").Split(",");

                        // determine if the user already appears in the master list.
                        try
                        {
                            // There should not be multiple instances of the same user in the array - Single() will throw an exeption!
                            _TempUser = _UserList.Where(u => u.Name.ToLower() == _username.ToLower()).Single();

                            // Union to determine the users they follow
                            _TempUser.Follows = _TempUser.Follows.Concat(_following).Distinct().ToArray();
                        }
                        catch
                        {
                            _UserList.Add(new TwitterUser { Name = _username, Follows = _following });
                        }
                    }
                }
                
                foreach (string line in txtTweets)
                {
                    try
                    {
                        _index = line.IndexOf(">");

                        // check for well formed line. 
                        if (_index > 0)
                        {
                            _username = line.Substring(0, _index).Trim();
                            _tweet = line.Substring(_index + 1); // thanks for the breadcrumbs ;)

                            // 2 determine length of string : prevent index out of range ex.
                            _length = (_tweet.Length > 140) ? 140 : _tweet.Length - 1;
                            // Assumes messages appear in sequence
                            _TweetList.Add(new Tweet{ Name = _username, Message = _tweet});
                        }
                        else throw new Exception($"No delimiter or user: Line({Array.IndexOf(txtTweets, line)}) ");
                    }
                    catch(Exception e)
                    { 
                        Console.WriteLine($"Error processing line in Tweets > {e.Message}");
                    }
                }

                this.UserList = _UserList;
                this.TweetList = _TweetList;

                if (Timer) 
                    Console.WriteLine($"Build() Time: {watch.ElapsedMilliseconds}ms | t:{watch.ElapsedTicks}\n");
            }
            public void Render(bool Timer = false)
            {
                Stopwatch watch = Stopwatch.StartNew();
                
                // Display the file contents by using a foreach loop.
                foreach (string user in UserNames.OrderBy(n => n))
                {
                    Console.WriteLine(user);

                    _TempUser = UserList.Where(u => u.Name == user).FirstOrDefault();
                    // Console.WriteLine($"\t follows {string.Join("|", _TempUser.Follows)}");

                    if (_TempUser != null)
                        foreach (var t in TweetList.Where(u => u.Name == user || _TempUser.Follows.Contains(u.Name)))
                        Console.WriteLine($"\t @{t.Name}: {t.Message}");
                }
                if (Timer) 
                    Console.WriteLine($"\nRender() Time: {watch.ElapsedMilliseconds}ms | t:{watch.ElapsedTicks}");
            }
        }
}