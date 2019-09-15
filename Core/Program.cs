using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text; 
using System.Threading.Tasks;

namespace DiscordDestrucoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Config.bot.defaultcmdPrefix == null)
            {
                Console.WriteLine("A default bot prefix is required");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Default Bot Prefix: " + Config.bot.defaultcmdPrefix);
            new Program().RunBotAsync().GetAwaiter().GetResult();
        }

        public static Random rnd = new Random();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private AudioService _audioservice;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            if (Config.bot.token == "" || Config.bot.token == null)
            {
                Console.WriteLine("A bot token is required");
                Console.ReadLine();
                return;
            }


            _client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Verbose });
            _commands = new CommandService();
            _audioservice = new AudioService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands).AddSingleton(_audioservice)
                .BuildServiceProvider();

            //Event subscriptions
            _client.Log += Log;
            //_client.UserJoined += AnnoucmentUserJoined;

            await RegisterCommandAsync();

            await _client.LoginAsync(TokenType.Bot, Config.bot.token);

            await _client.StartAsync();

            await _client.SetGameAsync("with your secret info");

            await Task.Delay(-1);

        }

        private async Task AnnoucmentUserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var channel = guild.DefaultChannel;
            await channel.SendMessageAsync($"Welcome, {user.Mention}");
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;


            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            //If the message is null or the author is the bot return so it doesn't respond to itself
            if (message == null || message.Author.IsBot)
                return;

            int argPos = 0;

            var contex = new SocketCommandContext(_client, message);

            if (message.HasStringPrefix(DataStorage.GetPrefixValue(contex.Guild.Id.ToString()), ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {

                var result = await _commands.ExecuteAsync(contex, argPos, _services);//execute command if it exists

                if (result.IsSuccess)
                    return;
                else
                {
                    if (result.Error != CommandError.UnknownCommand)
                        await contex.Channel.SendMessageAsync(result.ErrorReason);
                }


            }
            /* Tutorial:

            await contex.Channel.SendMessageAsync("Hello World."); //  This allows you to send a string as a message into discord.
            //you can check if the messagestring is == to whatever command you want such as "!cat".
            //using other normal C# code works fine, all you need to know is the send message thing.

            //But of course here is a little more information.

            //Here you create the class that creates embeded builders
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"I AM EMBED TITLE")
                .WithColor(Color.Blue)
                .AddField("I AM TITLE", "I AM DESCRIPTION");

            //You can just add .AddField to add another field, remove the color, or do some other stuff by just adding . to the end of the builder.
            //After you have built the embed you like use this to message it into discord.

            await ReplyAsync("", false, builder.Build());//This will build the embed and put it in discord as a message.


            //If you want contex about who where why and how the message was sent.
            //Use contex.Whatever
            //For example //contex.User.Username; //will get the username of the person who sent the message.
            //contex.Message.Reactions;//will get all the reactions that the message has (although you'd be checking a message as it was created so it'd get nothing)
             //just try contex.    then look
             * * */


            string messagestring = message.Content.ToLowerInvariant();//convert the message to lowercase and put it in a string

            if (contex.Channel.Name == "testing")
            {
                //await contex.Channel.SendMessageAsync(isThisHaiku(messagestring) ? "true" : "false");
                if (!isThisHaiku(messagestring))
                {
                    await contex.Message.DeleteAsync();
                    return;
                }
            }

            if (messagestring == "999")
            {
                await contex.Channel.SendMessageAsync("1000");
            }
            else if (messagestring.Contains($"{contex.Client.CurrentUser.Id}"))
            {
                await contex.Channel.SendMessageAsync("**No**");
            }
        }






        bool isThisHaiku(string possible_haiku)
        {
            if (possible_haiku.Length > 1)//if the message is more than one character
            {
                //SyllableCount

                string[] strarray = possible_haiku.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);//Split message by nextline

                if (strarray.Length != 3)//if this is not three lines
                {
                    return false;//This is not a proper haiku
                }

                for (uint i = 0; i < strarray.Length; i++)//for every sentence in the possible haiku
                {
                    char[] arr = strarray[i].Where(c => (char.IsLetter(c) ||
                             char.IsWhiteSpace(c))).ToArray();

                    strarray[i] = new string(arr);
                    int Syllablecount = 0;


                    string[] wordarray = strarray[i].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);//seperate each word into array

                    for (uint j = 0; j < wordarray.Length; j++)//Count every syllable in sentence
                    {
                        int syllableword = 0;//How many syllables this word is

                        WebClient wc = new WebClient();
                        string webData = wc.DownloadString("http://api.datamuse.com/words?sp=" + wordarray[j] + "&md=s&max=1");

                        //Console.WriteLine(webData);
                        syllableword = webData.LastIndexOf(':');//Stores index of string number

                        if (syllableword != -1)
                        {
                            webData = webData.Substring(syllableword);

                            arr = webData.Where(c => char.IsDigit(c)).ToArray();
                            webData = new string(arr);

                            syllableword = int.Parse(webData);

                            Syllablecount += syllableword;
                        }
                        else
                        {
                            syllableword += SyllableCount(wordarray[j]);

                            Syllablecount += syllableword;
                        }
                        //Console.WriteLine(wordarray[j] + "=" + syllableword);
                    }
                    


                    if (i == 1)//If middle line
                    {
                        if (Syllablecount != 7)//Must be 7
                        {
                            return false;
                        }
                    }
                    else//If first or last line
                    {
                        if (Syllablecount != 5)//Must be 5
                        {
                            return false;
                        }
                    }

                }
            }
            else//message not more than one character is of course not a haiku
            {
                return false;
            }

            return true;
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


        private static int SyllableCount(string word)
        {

            word = word.ToLower().Trim();
            bool lastWasVowel = false;
            var vowels = new[] { 'a', 'e', 'i', 'o', 'u', 'y' };
            int count = 0;

            //a string is an IEnumerable<char>; convenient.
            foreach (var c in word)
            {
                if (vowels.Contains(c))
                {
                    if (!lastWasVowel)
                        count++;
                    lastWasVowel = true;
                }
                else
                    lastWasVowel = false;
            }

            if ((word.EndsWith("e") || (word.EndsWith("es") || word.EndsWith("ed")))
                  && !word.EndsWith("le"))
                count--;

            if (count == 0)//If the word is somehow 0 syllables (be and ree are) 
                count = 1;//make sure it has at least one

            return count;
        }
    }
}
