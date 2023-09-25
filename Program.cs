using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;

namespace ArbitraryProcurement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string apiKey = "AIzaSyCqP--dX4KiEHR_xSgxarLbXjoxacxN4nM";
            Console.WriteLine("Welcome to jimxcds' Arbitrary Procurement terminal for youtube videos");
            EnterCommand:
            Console.Write("Enter a command:");
            string command = Console.ReadLine();
            if (command == null)
            {
                goto EnterCommand;
            }

            switch (command.ToLower())
            {
                case "checkid":
                    Console.WriteLine("Enter a youtube video ID");
                    Console.Write("ID:");
                    string videoIdToCheck = Console.ReadLine();
                    CheckIfVideoIdIsValid(apiKey, videoIdToCheck);
                    break;
                
                case "attemptrandom":
                    Console.WriteLine("Enter a number of time random video id should be generated and checked");
                    GetNumOfVideos:
                    Console.Write("num: ");
                    try
                    {
                        int numberOfTimesToSearch = Int32.Parse(Console.ReadLine() ?? string.Empty);
                        if (numberOfTimesToSearch == 0){goto GetNumOfVideos;}
                        for (int i = 0; i < numberOfTimesToSearch; i++)
                        {
                            if (!CheckIfVideoIdIsValid(apiKey, FindRandomVideo())) continue;
                            Console.WriteLine("You are the luckiest person in the world!");
                            break;
                        }
                    }
                    catch (FormatException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Enter integers only");
                        Console.ForegroundColor = ConsoleColor.White;
                        goto GetNumOfVideos;
                    }

                    break;
                case "procurerandom":
                    ProcureRandom(apiKey);
                    break;
                
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid command");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            goto EnterCommand;
        }

        static string FindRandomVideo()
        {
            Random rand = new Random();
            string videoId = "";
            for (int i = 0; i < 11; i++)
            {
                videoId += PickRandomChar();
            }

            return videoId;
            
            char PickRandomChar()
            {
                char[] possibleIdCharacters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_".ToCharArray();
                int idIndex = rand.Next(possibleIdCharacters.Length);
                return possibleIdCharacters[idIndex];
            }
        }

        static async Task<VideoListResponse> GetDataRequest(string part, string apiKey, string videoIdToCheck)
        {
            //creates a youtube service with the api key
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey
            });
                
            //Creates a snippet data request with video ID
            var videoRequest = youtubeService.Videos.List(part);
            videoRequest.Id = videoIdToCheck;
                
            //executes data request and awaits response
            var videoResponse = await videoRequest.ExecuteAsync();
            return videoResponse;
        }

        static void GetVideoData(string apiKey, string videoIdToCheck)
        {
            try
            {
                var videoSnippetDataRequest = GetDataRequest("snippet", apiKey, videoIdToCheck);
                var videoStatisticsDataRequest = GetDataRequest("Statistics", apiKey, videoIdToCheck);
                var videoSnippetResponse = videoSnippetDataRequest.Result;
                var videoStatisticsResponse = videoStatisticsDataRequest.Result;
                
                Console.WriteLine("Video Title: " + videoSnippetResponse.Items[0].Snippet.Title);
                //Console.WriteLine("Description: " + videoResponse.Items[0].Snippet.Description);
                Console.WriteLine("Channel Title: " + videoSnippetResponse.Items[0].Snippet.ChannelTitle);
                Console.WriteLine("View Count: " + videoStatisticsResponse.Items[0].Statistics.ViewCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static bool CheckIfVideoIdIsValid(string apiKey, string videoIdToCheck)
        {
            try
            {
                var videoSnippetDataRequest = GetDataRequest("snippet", apiKey, videoIdToCheck);
                var videoResponse = videoSnippetDataRequest.Result;

                if (videoResponse.Items.Count > 0)
                {
                    Console.WriteLine("Video '" + videoIdToCheck + "' is valid");
                    GetVideoData(apiKey, videoIdToCheck);
                    return true;
                }
                else
                {
                    Console.WriteLine($"Video '{videoIdToCheck}' does not exist");
                    return false;
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return false;
        }

        static async Task<SearchListResponse> GetSearchDataRequest(string part, string apiKey, string searchQuery, int maxResults)
        {
            //creates a youtube service with the api key
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey
            });
            
            //Creates a request to search for videos
            var searchRequest = youtubeService.Search.List(part);
            searchRequest.Q = searchQuery;
            searchRequest.MaxResults = maxResults;
            
            //applies filters 
            searchRequest.Type = "video";
            searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date;

            var searchResponse = await searchRequest.ExecuteAsync();
            return searchResponse;
        }

        private static void ProcureRandom(string apiKey)
        {
            Random rand = new Random();
            string currantSearchQuery = "";
            Console.WriteLine("Enter the category of video you want to procure:");
            VideoCategory:
            Console.Write("Category:");
            string category = Console.ReadLine();
            if (category == null)
            {
                Console.WriteLine("Enter a value");
                goto VideoCategory;
            }
            
            Console.WriteLine("Enter the number of crawls you want to preform");
            CrawlNumber:
            var crawlNumber = 0;
            Console.Write("Num:");
            try
            {
                crawlNumber = Int32.Parse(Console.ReadLine() ?? string.Empty);
                if (crawlNumber == 0){goto CrawlNumber;}
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Enter integers only");
                Console.ForegroundColor = ConsoleColor.White;
                goto CrawlNumber;
            }

            for (int currantCrawl = 0; currantCrawl < crawlNumber; currantCrawl++)
            {
                switch (category.ToLower())
                {
                    case "smartphone":
                        SmartPhone(false);
                        break;
                    case "smartphone date":
                        SmartPhone(true);
                        break;
                    case "webcam":
                        Webcam(false);
                        break;
                    case "webcam date":
                        Webcam(true);
                        break;
                    case "pa":
                        currantSearchQuery = "/Storage/Emulated/";
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case "misc":
                        Misc();
                        break;
                    case "pc":
                        Pc(false);
                        break;
                    case "pc filter":
                        Pc(true);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid command");
                        Console.ForegroundColor = ConsoleColor.White;
                        goto VideoCategory;
                }
            }

            string AddDateToQuery(string searchQuery, string type)
            {
                switch (type)
                {
                    case "smartphone":
                        searchQuery += " ";
                        searchQuery += DateTime.Now.Day;
                        searchQuery = DateChecker(DateTime.Now.Month);
                        searchQuery += DateTime.Now.Year;
                        break;
                    
                    case "smartphone whatsapp":
                        searchQuery += DateTime.Now.Year;
                        break;
                    
                    case "webcam":
                        searchQuery += " ";
                        searchQuery += DateTime.Now.Year;
                        searchQuery = DateChecker(DateTime.Now.Month);
                        searchQuery = DateChecker(DateTime.Now.Day);
                        break;
                }
                return searchQuery;
                
                string DateChecker(int dateValue)
                {
                    if (dateValue >= 10)
                    {
                        searchQuery += dateValue;
                    }
                    else
                    {
                        searchQuery += "0";
                        searchQuery += dateValue;
                    }

                    return searchQuery;
                }
            }
            
            void Pc(bool filterSpam)
            {
                var format = rand.Next(filterSpam ? 5 : 8);

                switch (format)
                {
                    case 0:
                        currantSearchQuery = ".MP4";
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 1:
                        currantSearchQuery = ".3GP";
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 2:
                        currantSearchQuery = ".MOV";
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 3:
                        currantSearchQuery = ".AVI";
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 4:
                        currantSearchQuery = ".WMV";
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 5:
                        currantSearchQuery = ".MKV";
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 6:
                        currantSearchQuery = ".MPEG";
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 7:
                        currantSearchQuery = ".FLV";
                        SearchForProcurement(currantSearchQuery);
                        break;
                }
            }
            
            void Misc()
            {
                int format = rand.Next(2);
                switch (format)
                {
                    case 0:
                        currantSearchQuery = "FullSizeRender";
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 1:
                        currantSearchQuery = "My Movie";
                        SearchForProcurement(currantSearchQuery);
                        break;
                }
            }

            void Webcam(bool date)
            {
                int format = rand.Next(2);
                switch (format)
                {
                    case 0:
                        currantSearchQuery = "WIN";
                        if (date){currantSearchQuery = AddDateToQuery(currantSearchQuery, "webcam");}
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 1:
                        currantSearchQuery = "VID";
                        if (date){currantSearchQuery = AddDateToQuery(currantSearchQuery, "webcam");}
                        SearchForProcurement(currantSearchQuery);
                        break;
                }
            }

            void SmartPhone(bool date)
            {
                int imgOrMvi = rand.Next(3);

                switch (imgOrMvi)
                {
                    case 0:
                        currantSearchQuery = "IMG";
                        if (date){currantSearchQuery = AddDateToQuery(currantSearchQuery, "smartphone");}
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 1:
                        currantSearchQuery = "MVI";
                        if (date){currantSearchQuery = AddDateToQuery(currantSearchQuery, "smartphone");}
                        SearchForProcurement(currantSearchQuery);
                        break;
                    case 2:
                        currantSearchQuery = "WhatsApp Video";
                        if (date){currantSearchQuery = AddDateToQuery(currantSearchQuery, "smartphone whatsapp");}
                        SearchForProcurement(currantSearchQuery);
                        break;
                }
            }

            void SearchForProcurement(string searchQuery)
            {
                try
                {
                    var searchRequest = GetSearchDataRequest("snippet", apiKey, searchQuery, 10);
                    var searchResponse = searchRequest.Result;

                    var searchResponseList = searchResponse.Items;
                    if (searchResponseList.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No results");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    int videoNumber = rand.Next(searchResponseList.Count);
                    var searchResult = searchResponseList[videoNumber];
                    Console.WriteLine("Link: https://youtu.be/" + searchResult.Id.VideoId);
                    GetVideoData(apiKey, searchResult.Id.VideoId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}