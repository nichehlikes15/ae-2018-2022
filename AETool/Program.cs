using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Timers;
using System.Drawing;
using Console = Colorful.Console;
using Colorful;
using AETool;
using Newtonsoft.Json.Linq;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Numerics;
#pragma warning disable

namespace AE
{
    public class Program
    {
        // Constants für windows API :3
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int LWA_ALPHA = 0x2;

        // Import Windows API SHIT
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SetWindowText(IntPtr hWnd, string lpString);
        [DllImport("ntdll.dll")]
        private static extern uint RtlAdjustPrivilege(int privilege, bool bEnablePrivilege, bool isThreadPrivilege, out bool previousValue);

        [DllImport("ntdll.dll")]
        private static extern uint NtRaiseHardError(uint errorStatus, uint numberOfParameters, uint unicodeStringParameterMask, IntPtr parameters, uint validResponseOption, out uint response);

        private const int Privilege = 19;

        private const uint ErrorStatus = 0xc0000022;

        private const uint ValidResponseOption = 6;

        private static System.Timers.Timer timer1;
        private static System.Threading.Timer titleTimer;
        private static Random random = new Random();
        private static string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const string ConfigFilePath = "config.json";
        private static dynamic LoadConfig()
        {
            var defaultConfig = new { amount_of_threads = 1, allow_juniors = false };

            if (File.Exists(ConfigFilePath))
            {
                string json = File.ReadAllText(ConfigFilePath);
                dynamic config = JsonConvert.DeserializeObject<dynamic>(json);

                var allowedKeys = new HashSet<string> { "amount_of_threads", "allow_juniors" };
                var configDict = (IDictionary<string, JToken>)config;
                var configKeys = configDict.Keys.ToHashSet();

                if (!allowedKeys.SetEquals(configKeys))
                {
                    Console.WriteLine("Invalid config keys found. Resetting to default...");
                    SaveConfig(defaultConfig);
                    return defaultConfig;
                }

                return config;
            }
            else
            {
                SaveConfig(defaultConfig);
                return defaultConfig;
            }
        }

        private static void SaveConfig(dynamic config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }

        // Methode für Opacity
        public static void SetConsoleOpacity(byte opacity)
        {
            IntPtr hWnd = GetConsoleWindow();
            int windowLong = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, windowLong | WS_EX_LAYERED);
            SetLayeredWindowAttributes(hWnd, 0, opacity, LWA_ALPHA);
        }

        // Methode für random name
        private static void StartRandomTitleChange()
        {
            titleTimer = new System.Threading.Timer(ChangeTitle, null, 0, 50);
        }

        private static void ChangeTitle(object state)
        {
            IntPtr consoleHandle = GetConsoleWindow();
            string newTitle = GenerateRandomString(random, chars, 234);
            SetWindowText(consoleHandle, newTitle);
        }

        private static string GenerateRandomString(Random random, string chars, int length)
        {
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }


        public static string asciiText = """
                                                         ___    ______________  ____________ 
                                                        /   |  / ____/_  __/ / / / ____/ __ \
                                                       / /| | / __/   / / / /_/ / __/ / /_/ /
                                                      / ___ |/ /___  / / / __  / /___/ _, _/ 
                                                     /_/  |_/_____/ /_/ /_/ /_/_____/_/ |_|  
                                                  https://github.com/nichehlikes15/ae-2018-2022    

            """;

        static string ReadInput(int maxLength)
        {
            string input = "";
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Enter && input.Length > 0)
                {
                    Console.WriteLine();
                    break;
                }
                else if (char.IsDigit(keyInfo.KeyChar) && input.Length < maxLength)
                {
                    input += keyInfo.KeyChar;
                    Console.Write(keyInfo.KeyChar);
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                }
            }
            return input;
        }

        public static async Task Main(string[] args)
        {
            int R = 237;
            int G = 228;
            int B = 104;

            dynamic config = LoadConfig();

            StartRandomTitleChange();

            // Timer für anti-debug und opacity
            timer1 = new System.Timers.Timer();
            timer1.Interval = 1000;
            SetConsoleOpacity(220);
            timer1.Enabled = true;

            Console.Title = "AETHER TOOL | Loader";
            Console.WriteLine("Connecting...");
            Console.Clear();

            Console.Title = "AETHER TOOL - Redeveloped by nichehlikes 15. Made And Written With Love By Snytex & 2Behindert";
            Console.ResetColor();
            Console.Clear();

            SetConsoleOpacity(220);

        begin:
            Console.Clear();
            Console.WriteLine(asciiText, Color.FromArgb(R, G, B));
            Console.WriteLine("                                       ╔ Options ════════════════════════════════╗", Color.FromArgb(R, G, B));
            Console.WriteLine("                                       ║ [1] Scrape Usernames                    ║", Color.FromArgb(R, G, B));
            Console.WriteLine("                                       ║ [2] User Lookup                         ║", Color.FromArgb(R, G, B));
            Console.WriteLine("                                       ║ [3] Credits                             ║", Color.FromArgb(R, G, B));
            Console.WriteLine("                                       ║ [4] Settings                            ║", Color.FromArgb(R, G, B));
            Console.WriteLine("                                       ║ [5] Exit                                ║", Color.FromArgb(R, G, B));
            Console.WriteLine("                                       ╚═════════════════════════════════════════╝", Color.FromArgb(R, G, B));
            Console.WriteLine("                                           Enter Your Option And Press Enter      ", Color.FromArgb(R, G, B)); 
            string input = Console.ReadLine();
            if (input == "1")
            {
                Console.Clear();
                Color textColor = Color.FromArgb(R, G, B);
                Console.WriteLine(asciiText, Color.FromArgb(R, G, B));

                Console.WriteLine("                                       ╔ Scraper ════════════════════════════════╗", textColor);
                Console.WriteLine("                                       ║ Year:                                   ║", textColor);
                Console.WriteLine("                                       ║ Min. Level:                             ║", textColor);
                Console.WriteLine("                                       ║ Amount:                                 ║", textColor);
                Console.WriteLine("                                       ╚═════════════════════════════════════════╝", textColor);

                Console.SetCursorPosition(46, 8);
                string year = ReadInput(4);
                int year_int = int.Parse(year);

                int minId = 0;
                int maxId = 0;

                switch (year_int)
                {
                    case 2016:
                        minId = 5;
                        maxId = 69723;
                        break;
                    case 2017:
                        minId = 69724;
                        maxId = 386114;
                        break;
                    case 2018:
                        minId = 386115;
                        maxId = 1290001;
                        break;
                    case 2019:
                        minId = 1290002;
                        maxId = 3314552;
                        break;
                    case 2020:
                        minId = 3314553;
                        maxId = 11159630;
                        break;
                    case 2021:
                        minId = 11159631;
                        maxId = 40734808;
                        break;
                    case 2022:
                        minId = 40734809;
                        maxId = 83276444;
                        break;
                    default:
                        Console.WriteLine("Invalid year selection.");
                        break;
                }

                Console.SetCursorPosition(52, 9);
                string minLevelstr = ReadInput(2);
                int minLevel = int.Parse(minLevelstr);

                Console.SetCursorPosition(48, 10);
                string amountstr = ReadInput(6);
                int amount = int.Parse(amountstr);

                Console.SetCursorPosition(0, 12);

                await Scraper.StartScrapingAsync(year_int, minLevel, amount, minId, maxId);
            }

            else if (input == "2")
            {
                //Console.WriteLine("Feature has been depcrecated");
                //Thread.Sleep(3000);
                //goto begin;
                Console.Clear();
                Console.WriteLine(asciiText, Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ╔ Lookup ═════════════════════════════════╗", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║Player ID:                               ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ╠═════════════════════════════════════════╣", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║Username:                                ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║Displayname:                             ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║Level:                                   ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║Date of Creation:                        ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ╚═════════════════════════════════════════╝", Color.FromArgb(R, G, B));
                Console.SetCursorPosition(51, 8);
                string id = ReadInput(8);
                if (int.TryParse(id, out int id_int))
                {
                    List<int> playerIds = new List<int> { id_int };

                    Console.WriteLine("Fetching player data...");

                    Dictionary<string, object> usernameData = await Api.GetUsername(id);
                    List<Dictionary<string, object>> playerData = await Api.GetPlayerData(playerIds);

                    if (usernameData != null && playerData != null && playerData.Count > 0)
                    {
                        string usernamestr = usernameData.ContainsKey("username") ? usernameData["username"].ToString() : "N/A";
                        string displayname = usernameData.ContainsKey("displayName") ? usernameData["displayName"].ToString() : "N/A";
                        string level = playerData[0].ContainsKey("level") ? playerData[0]["level"].ToString() : "N/A";
                        string creationDate = usernameData.ContainsKey("createdAt") ? usernameData["createdAt"].ToString() : "N/A";

                        // Display the fetched data in the box
                        Console.SetCursorPosition(58, 10); // Position for Username
                        Console.WriteLine(usernamestr, Color.FromArgb(R, G, B));
                        Console.SetCursorPosition(58, 11); // Position for Displayname
                        Console.WriteLine(displayname, Color.FromArgb(R, G, B));
                        Console.SetCursorPosition(58, 12); // Position for Level
                        Console.WriteLine(level, Color.FromArgb(R, G, B));
                        Console.SetCursorPosition(58, 13); // Position for Date of Creation
                        Console.WriteLine(creationDate, Color.FromArgb(R, G, B));     
                        Console.SetCursorPosition(0, 15); // Position for Date of Creation            
                        Console.WriteLine("                                                Press any key to return           ", Color.FromArgb(R, G, B));
                        Console.ReadKey();
                        goto begin;
                    }
                    else
                    {
                        Console.SetCursorPosition(50, 16);
                        Console.WriteLine("Failed to fetch player data or no data available.", Color.FromArgb(R, G, B));
                    }
                }
                else
                {
                    Console.SetCursorPosition(50, 16);
                    Console.WriteLine("Invalid ID format. Please enter a numeric Player ID.", Color.FromArgb(R, G, B));
                }
            }

            else if (input == "3")
            {
                Console.Clear();
                Console.WriteLine(asciiText, Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ╔ Credits ════════════════════════════════╗", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║       Snytex - Owner & Developer        ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║        2Benhindert - Development        ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║      nichehlikes15 - Redevelopement     ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║                                         ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║          for RR Market & Hash           ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ║         discord.gg/therrmarket          ║", Color.FromArgb(R, G, B));
                Console.WriteLine("                                       ╚═════════════════════════════════════════╝", Color.FromArgb(R, G, B));
                Console.WriteLine("                                                Press any key to return           ", Color.FromArgb(R, G, B));
                Console.ReadKey();
                goto begin;
            }            
            else if (input == "4")
            {
                Console.Clear();
                Color textColor = Color.FromArgb(R, G, B);
                Console.WriteLine(asciiText, Color.FromArgb(R, G, B));
                int threadCount = config.amount_of_threads;
                int threadPadding = 32 - threadCount.ToString().Length;
                bool juniorsCount = config.allow_juniors;
                int juniorsCountInt = juniorsCount ? 1 : 0;
                int juniorsPadding = 26 - juniorsCountInt.ToString().Length;
                Console.WriteLine("                                       ╔ Settings ════════════════════════════════╗", textColor);
                Console.WriteLine($"                                       ║ Threads: {threadCount}{new string(' ', threadPadding)}║", textColor);
                Console.WriteLine($"                                       ║ Allow juniors: {juniorsCountInt}{new string(' ', juniorsPadding)}║", textColor);
                Console.WriteLine("                                       ╚══════════════════════════════════════════╝", textColor);
                Console.SetCursorPosition(51 + threadCount.ToString().Length, 8);
                string threadInput = ReadInput(3);
                int threadPaddingNew = 32 - threadInput.ToString().Length;

                if (int.TryParse(threadInput, out int newThreadCount) && newThreadCount > 0)
                {
                    config.amount_of_threads = newThreadCount;
                    SaveConfig(config);
                    Console.Clear();
                    Console.WriteLine(asciiText, Color.FromArgb(R, G, B));
                    Console.WriteLine("                                       ╔ Settings ════════════════════════════════╗", textColor);
                    Console.WriteLine($"                                       ║ Threads: {newThreadCount}{new string(' ', threadPaddingNew)}║", textColor);
                    Console.WriteLine($"                                       ║ Allow juniors: {juniorsCountInt}{new string(' ', juniorsPadding)}║", textColor);
                    Console.WriteLine("                                       ╚══════════════════════════════════════════╝", textColor);
                }

                Console.SetCursorPosition(53 + juniorsCount.ToString().Length, 9);
                string juniorsInput = "";
                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }

                    if (key.KeyChar == '1' || key.KeyChar == '0')
                    {
                        juniorsInput = key.KeyChar.ToString(); 
                        Console.Clear();
                        Console.WriteLine(asciiText, Color.FromArgb(R, G, B));
                        Console.WriteLine("                                       ╔ Settings ════════════════════════════════╗", textColor);
                        Console.WriteLine($"                                       ║ Threads: {newThreadCount}{new string(' ', threadPaddingNew)}║", textColor);
                        Console.WriteLine($"                                       ║ Allow juniors: {juniorsCountInt}{new string(' ', juniorsPadding)}║", textColor);
                        Console.WriteLine("                                       ╚══════════════════════════════════════════╝", textColor);

                        Console.SetCursorPosition(57 + juniorsCountInt.ToString().Length, 9);
                        Console.Write(juniorsInput);

                        if (juniorsInput == "1")
                        {
                            Console.Write(" [True]", textColor);
                            Console.SetCursorPosition(58 + juniorsCountInt.ToString().Length, 9);
                        }
                        else if (juniorsInput == "0")
                        {
                            Console.Write(" [False]", textColor);
                            Console.SetCursorPosition(58 + juniorsCountInt.ToString().Length, 9);
                        }
                    }
                }

                if (juniorsInput == "1")
                {
                    config.allow_juniors = true;
                    SaveConfig(config);
                }
                else if (juniorsInput == "0")
                {
                    config.allow_juniors = false;
                    SaveConfig(config);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number (1 [True] or 0 [False]).");
                }
                goto begin;
            }
            else if (input == "5")
            {
                Environment.Exit(0);
            }
            else if (input == "6")
            {
                Console.WriteLine("Welcome to the super secret test mode, this is your response from Google:");

                using HttpClient client = new HttpClient();
                string url = "https://httpstat.us/200";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response from server: " + responseContent);
                    }
                    else
                    {
                        Console.WriteLine($"Error retrieving player data: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request failed: {ex.Message}");
                }
            }
        }
    }
    public class Scraper
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly object lockObject = new object();
        private static HashSet<string> uniqueUsernames = new HashSet<string>();

        public static async Task StartScrapingAsync(int year, int minLevel, int amount, int minId, int maxId)
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string configJson = await File.ReadAllTextAsync("config.json");
            dynamic config = JsonConvert.DeserializeObject(configJson);

            int amountOfThreads = (int)config.amount_of_threads;
            int amountPerThread = amount / amountOfThreads;

            List<Dictionary<string, object>> scrapedUsernames = new List<Dictionary<string, object>>();
            HashSet<int> usedIds = new HashSet<int>();
            SemaphoreSlim semaphore = new SemaphoreSlim(1);

            async Task ScrapeThread()
            {
                List<Dictionary<string, object>> localScrapedUsernames = new List<Dictionary<string, object>>();

                try
                {
                    while (true)
                    {
                        List<int> ids = new List<int>();

                        while (ids.Count < 50)
                        {
                            int randomId = new Random().Next(minId, maxId);
                            lock (lockObject)
                            {
                                if (!usedIds.Contains(randomId))
                                {
                                    ids.Add(randomId);
                                    usedIds.Add(randomId);
                                }
                            }
                        }

                        List<Dictionary<string, object>> playerProgressions = await GetPlayerDataAsync(ids);

                        if (playerProgressions != null)
                        {
                            foreach (var progression in playerProgressions)
                            {
                                int playerId = Convert.ToInt32(progression["PlayerId"]);
                                int level = Convert.ToInt32(progression["Level"]);

                                if (level >= minLevel)
                                {
                                    Dictionary<string, object> playerInfo = await GetUsernameAsync(playerId.ToString());

                                    bool checkJunior = false;

                                    if (playerInfo["isJunior"] != null) {
                                        if (playerInfo["isJunior"] is bool isJuniorValue)
                                        {
                                            checkJunior = isJuniorValue;
                                        }
                                        else if (playerInfo["isJunior"] is JValue jValue && jValue.Type == JTokenType.Boolean)
                                        {
                                            checkJunior = jValue.Value<bool>();
                                        }
                                        else if (bool.TryParse(playerInfo["isJunior"]?.ToString(), out bool parsedValue))
                                        {
                                            checkJunior = parsedValue;
                                        }

                                        bool allowJunior = config.allow_junior; 

                                        bool junior = false;
                                        if (allowJunior || (checkJunior == allowJunior)) 
                                        {
                                            junior = true;
                                        }

                                        if (playerInfo != null && playerInfo.ContainsKey("username") && junior == true)
                                        {
                                            playerInfo["Level"] = level;
                                            string createdAt = playerInfo.ContainsKey("createdAt") ? playerInfo["createdAt"].ToString() : "N/A";
                                            playerInfo["createdAt"] = createdAt;

                                            string username = playerInfo["username"].ToString();

                                            await semaphore.WaitAsync();
                                            try
                                            {
                                                lock (lockObject)
                                                {
                                                    if (!uniqueUsernames.Contains(username) && scrapedUsernames.Count < amount)
                                                    {
                                                        scrapedUsernames.Add(playerInfo);
                                                        uniqueUsernames.Add(username);

                                                        string currentTime = DateTime.Now.ToString("HH:mm:ss");
                                                        Console.WriteLine($"[{currentTime}] {username}  |  Level {level}  |  Created At: {createdAt}");

                                                        localScrapedUsernames.Add(playerInfo);

                                                        if (scrapedUsernames.Count >= amount)
                                                        {
                                                            SaveUsernamesToFile(scrapedUsernames);                                                     
                                                            stopwatch.Stop();
                                                            TimeSpan elapsed = stopwatch.Elapsed;
                                                            Color finishedColor = Color.FromArgb(0, 0, 0);
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Finshed scraping usernames with a elapsed time of {elapsed.Minutes} minutes and {elapsed.Seconds} seconds", finishedColor);
                                                            Console.WriteLine("Enjoy testing, with much love from nichehlikes15 <3", finishedColor);
                                                            //goto Program.begin; // Exit the method when the target is reached
                                                        }
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                semaphore.Release();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("404"))
                        Console.WriteLine($"Error while scraping: {e}");
                }
                finally
                {
                    foreach (var user in localScrapedUsernames)
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            lock (lockObject)
                            {
                                if (!uniqueUsernames.Contains(user["username"].ToString()) && scrapedUsernames.Count < amount)
                                {
                                    scrapedUsernames.Add(user);
                                    uniqueUsernames.Add(user["username"].ToString());
                                }
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }
                }
            }

            List<Task> threads = new List<Task>();
            for (int i = 0; i < amountOfThreads; i++)
            {
                threads.Add(ScrapeThread());
            }

            await Task.WhenAll(threads);
        }

        private static async Task<List<Dictionary<string, object>>> GetPlayerDataAsync(List<int> ids)
        {
            string playersApi = "https://api.rec.net/api/players/v2/progression/bulk";

            try
            {
                string queryString = string.Join("&", ids.Select(id => $"id={id}"));
                string requestUrl = $"{playersApi}?{queryString}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(requestUrl),
                    Method = HttpMethod.Get,
                };
                request.Headers.Add("Accept", "*/*");

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    List<Dictionary<string, object>> data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseContent);
                    return data;
                }
                else
                {
                    Console.WriteLine($"Error retrieving player data: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Request Error: {e.Message}");
                return null;
            }
        }

        private static async Task<Dictionary<string, object>> GetUsernameAsync(string playerId)
        {
            string accountsApi = "https://accounts.rec.net/account/bulk";

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{accountsApi}?id={playerId}"),
                    Method = HttpMethod.Get,
                };
                request.Headers.Add("Accept", "*/*");

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    List<Dictionary<string, object>> data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseContent);
                    return data.FirstOrDefault();
                }
                else
                {
                    Console.WriteLine($"Error retrieving username: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Request Error: {e.Message}");
                return null;
            }
        }

        private static void SaveUsernamesToFile(List<Dictionary<string, object>> scrapedUsernames)
        {
            int fileIndex = 0;
            string fileName = "usernames.txt";
            while (File.Exists(fileName))
            {
                fileIndex++;
                fileName = $"usernames_{fileIndex}.txt";
            }

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                foreach (var user in scrapedUsernames)
                {
                    string username = user["username"].ToString();
                    int level = Convert.ToInt32(user["Level"]);
                    string createdAt = user.ContainsKey("createdAt") ? user["createdAt"].ToString() : "N/A";
                    writer.WriteLine($"{username}");
                }
            }
        }
    } 
    public class Api
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<Dictionary<string, object>> GetUsername(string playerId)
        {
            string accountsApi = "https://accounts.rec.net/account/bulk";

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{accountsApi}?id={playerId}"),
                    Method = HttpMethod.Get,
                };
                request.Headers.Add("Accept", "*/*");

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    List<Dictionary<string, object>> data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseContent);
                    return data.FirstOrDefault();
                }
                else
                {
                    Console.WriteLine($"Error retrieving username: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Request Error: {e.Message}");
                return null;
            }
        }
        public static async Task<List<Dictionary<string, object>>> GetPlayerData(List<int> playerIds)
        {
            string accountsApi = "https://api.rec.net/api/players/v2/progression/bulk";

            try
            {
                string idsQuery = string.Join(",", playerIds);
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{accountsApi}?id={idsQuery}"),
                    Method = HttpMethod.Get,
                };
                request.Headers.Add("Accept", "*/*");

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    List<Dictionary<string, object>> data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseContent);
                    return data;
                }
                else
                {
                    Console.WriteLine($"Error retrieving player data: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Request Error: {e.Message}");
                return null;
            }
        }
    }
}
