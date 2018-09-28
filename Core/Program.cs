using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
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

        public static Random rnd = new Random(DateTime.Now.Second);

        private DiscordSocketClient _client;
        private CommandService _commands;
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

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            //Event subscriptions
            _client.Log += Log;
            //_client.UserJoined += AnnoucmentUserJoined;

            await RegisterCommandAsync();

            await _client.LoginAsync(TokenType.Bot, Config.bot.token);

            await _client.StartAsync();

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

            if (message.HasStringPrefix(DataStorage.GetPrefixValue("Prefix" + contex.Guild.Id), ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {

                var result = await _commands.ExecuteAsync(contex, argPos, _services);


                if (result.IsSuccess)
                    return;
                else
                {
                    if(result.Error != CommandError.UnknownCommand)
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

            if (messagestring.Contains("no u"))//if the message contains no u
                await contex.Channel.SendMessageAsync("no u");//send the message



            else if (messagestring == "!iamcat")
            {
                EmbedBuilder builder = new EmbedBuilder();//Make the embed builder that makes the embed

                builder.WithTitle("MEOW!")
                    .WithColor(Color.Green)
                    .WithDescription(">.><.<");

                await contex.Channel.SendMessageAsync("", false, builder.Build());//This here makes the bot print the embed
            }
            else if (messagestring.Contains($"{contex.Client.CurrentUser.Id}"))
            {
                await contex.Channel.SendMessageAsync("**No**");
            }

        }
    }
}
