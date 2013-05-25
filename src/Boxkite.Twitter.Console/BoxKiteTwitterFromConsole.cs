﻿// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.IO;
using System.Text;
using System.Threading;
using BoxKite.Twitter.Console.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Console
{
    public class BoxKiteTwitterFromConsole
    {
        public static IUserStream userstream;
        public static ISearchStream searchstream;

        private static void Main(string[] args)
        {
            ConsoleOutput.PrintMessage("Welcome to BoxKite.Twitter Console");
            ConsoleOutput.PrintMessage("(control-c ends)");

            var twittercredentials = ManageTwitterCredentials.MakeConnection();

            if (twittercredentials.Valid)
            {
                System.Console.CancelKeyPress += new ConsoleCancelEventHandler(cancelStreamHandler);
                var session = new UserSession(twittercredentials, new DesktopPlatformAdaptor());
                var checkUser = session.GetVerifyCredentials().Result;
                if (!checkUser.twitterFaulted)
                {
                    ConsoleOutput.PrintMessage(twittercredentials.ScreenName + " is authorised to use BoxKite.Twitter.");

                    var accountSettings = session.GetAccountSettings().Result;
                    if (!accountSettings.twitterFaulted)
                    {

                        //var fileName = @"C:\Users\Nick\Pictures\My Online Avatars\666.jpg";
                        //if (File.Exists(fileName))
                        //{
                        //var newImage = File.ReadAllBytes(fileName);

                        var sr = FilesHelper.FromFile("sampleimage\\MaggieThatcherRules.jpg");

                        // var x = session.SendTweetWithImage("Testing Image Upload. You can Ignore", Path.GetFileName(fileName),newImage).Result;

                        using (var fs = new FileStream(sr, FileMode.Open, FileAccess.Read))
                        {                               
                            
                            //var x = session.ChangeAccountProfileImage("MaggieThatcherRules.jpg", fs).Result;

                            var x = session.SendTweetWithImage("Maggies Rules", "maggie.jpg", fs).Result;

                            if (x.twitterFaulted)
                            {
                                PrintTwitterErrors(x.twitterControlMessage);
                            }
                            else
                            {
                                ConsoleOutput.PrintTweet(x, ConsoleColor.Green);
                            }

                        }



                        /*userstream = session.GetUserStream();
                        userstream.Tweets.Subscribe(
                            t =>
                                System.Console.WriteLine(String.Format("ScreenName: {0}, Tweet: {1}", t.User.ScreenName,
                                    t.Text)));
                        userstream.Start();

                        while (userstream.IsActive)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(0.5));
                        }


                       userstream = session.GetUserStream();
                         userstream.Tweets.Subscribe(t => ConsoleOutput.PrintTweet(t, ConsoleColor.Green));
                         userstream.Events.Subscribe(e => ConsoleOutput.PrintEvent(e, ConsoleColor.Yellow));
                         userstream.DirectMessages.Subscribe(
                             d => ConsoleOutput.PrintDirect(d, ConsoleColor.Magenta, ConsoleColor.Black));
                         userstream.Start();

                         while (userstream.IsActive)
                         {
                             Thread.Sleep(TimeSpan.FromSeconds(0.5));
                         } */



                        /*
                         * 
                         * //var locations = new List<string> { "-34.081953", "150.700493", "-33.593316", "151.284828" };
                            //searchstream = session.StartSearchStream(locations: locations);
                            searchstream = session.StartSearchStream(track: "hazel");
                            searchstream.FoundTweets.Subscribe(t => ConsoleOutput.PrintTweet(t, ConsoleColor.Green));
                            searchstream.Start();

                            while (searchstream.IsActive)
                            {
                                Thread.Sleep(TimeSpan.FromMinutes(1));
                                var sr = new StreamSearchRequest();
                                sr.tracks.Add("xbox");
                                searchstream.SearchRequests.Publish(sr);
                            }
                         * 
                         */


                        /*
                        var x = session.GetMentions(count:100).Result;

                        foreach (var tweet in x)
                        {
                            ConsoleOutputPrintTweet(tweet);
                        }
                        
                    
                         session.GetFavourites(count: 10)
                            .Subscribe(t => ConsoleOutputPrintTweet(t, ConsoleColor.White, ConsoleColor.Black));
                        */

                     }
                }
                else
                {
                    ConsoleOutput.PrintMessage(String.Format("Credentials could not be verified: {0}", checkUser.twitterControlMessage.twitter_error_message), ConsoleColor.Red);
                }
            }
            else
            {
                ConsoleOutput.PrintMessage("Authenticator could not start. Do you have the correct Client/Consumer IDs and secrets?", ConsoleColor.Red);
            }
            System.Console.ReadLine();
        }

        public static void PrintTwitterErrors(TwitterControlMessage tcm)
        {
            ConsoleOutput.PrintMessage("START: TWITTER CONTROL MESSAGE");
            ConsoleOutput.PrintError(String.Format("http reason: {0}", tcm.http_reason));
            ConsoleOutput.PrintError(String.Format("http status code: {0}", tcm.http_status_code));
            ConsoleOutput.PrintError(String.Format("twitter error code: {0}", tcm.twitter_error_code));
            ConsoleOutput.PrintError(String.Format("twitter error message: {0}", tcm.twitter_error_message));
            ConsoleOutput.PrintError(String.Format("API rates: {0}/{1} Resets {2}",
                tcm.twitter_rate_limit_remaining,
                tcm.twitter_rate_limit_limit, tcm.twitter_rate_limit_reset));
            ConsoleOutput.PrintMessage("END: TWITTER CONTROL MESSAGE");
        }

        private static void cancelStreamHandler(object sender, ConsoleCancelEventArgs e)
        {
            if (userstream != null)
                userstream.Stop();
            ConsoleOutput.PrintMessage("All finished.", ConsoleColor.Blue);
            Thread.Sleep(TimeSpan.FromSeconds(1.3));
        }
    }
}
