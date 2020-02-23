using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.Net;
using Discord.WebSocket;
using Discord.Webhook;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace DiscordDestrucoBot.Modules
{
    [Group]
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync($"***Pong***");
        }
        [Command("pong")]
        public async Task PongAsync()
        {
            await ReplyAsync($"***Ping***");
        }

        [Command("rename", RunMode = RunMode.Async)] [RequireUserPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [Alias("changename", "changenick", "nickname")]
        public async Task ChangeNickAsync(SocketGuildUser userArg, [Remainder] string name)
        {
            if (name.Length > 32)
            {
                await ReplyAsync("Nickname must be equal to or under 32 characters"); return;
            }
            SocketGuildUser sender = (SocketGuildUser)Context.User;
            if (userArg.Id == Context.Guild.OwnerId)
            {
                await ReplyAsync("You can't change the owners nickname"); return;
            }
            if (userArg.Hierarchy >= sender.Hierarchy && !sender.GuildPermissions.Administrator)
            {
                await ReplyAsync("Your role must be equal to or higher to assign the nickname"); return;
            }

            if(name == ".default.")//For removing nicknames
            {
                name = "";
            }
            await userArg.ModifyAsync(x => { x.Nickname = name; });

            string sentMessage;
            if (name != "")
            {
                sentMessage = $"{userArg.Username}s Nickname has changed to {name}";

                if (sentMessage.Contains("@everyone"))
                {
                    sentMessage.Replace("@everyone", "@​everyone");
                }
                if (sentMessage.Contains("@here"))
                {
                    sentMessage.Replace("@here", "@​here");
                }
            }
            else
            {
                sentMessage = $"{userArg.Username}s Nickname has been removed";
            }

            RestUserMessage toDelete = await Context.Channel.SendMessageAsync(sentMessage);
            await Task.Delay(4000); //starting delay
            await toDelete.DeleteAsync();
        }

        [Command("rename")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [Alias("changename", "changenick", "nickname")]
        public async Task ChangeNickAsync(SocketRole roleArg, [Remainder] string name)
        {
            if (name.Length > 32)
            {
                await ReplyAsync("Nickname must be equal to or under 32 characters"); return;
            }

            if (name == ".default.")//For removing nicknames
            {
                name = "";
            }

            int amountchanged = 0;
            var botuser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            foreach (SocketGuildUser userArg in roleArg.Members) {
                if (userArg.Hierarchy >= botuser.Hierarchy)
                    continue;

                await userArg.ModifyAsync(x => { x.Nickname = name; });
                amountchanged++;
            }
            string sentMessage;
            if (name != "")
            {
                sentMessage = $"{amountchanged} users have had their nicknames changed to {name} by {Context.User.Mention}";
                if (sentMessage.Contains("@everyone"))
                {
                    sentMessage.Replace("@everyone", "@ everyone");
                }
                if (sentMessage.Contains("@here"))
                {
                    sentMessage.Replace("@here", "@ here");
                }
            }
            else
            {
                sentMessage = $"{amountchanged} users have had their nicknames removed by {Context.User.Mention}";
            }
            await ReplyAsync(sentMessage);
        }

        [Command("rename_regex")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task ChangeNick_RegexAsync(SocketRole roleArg, [Remainder] string name)
        {
            if (name.Length > 32)
            {
                await ReplyAsync("Nickname must be equal to or under 32 characters"); return;
            }

            int amountchanged = 0;
            var botuser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            foreach (SocketGuildUser userArg in roleArg.Members)
            {
                if (userArg.Hierarchy >= botuser.Hierarchy)
                    continue;

                await userArg.ModifyAsync(x => { x.Nickname = name; });
                amountchanged++;
            }
            string sentMessage;
            sentMessage = $"{amountchanged} users have had their nicknames changed to {name} by {Context.User.Mention}";
            if (sentMessage.Contains("@everyone"))
            {
                sentMessage.Replace("@everyone", "@ everyone");
            }
            if (sentMessage.Contains("@here"))
            {
                sentMessage.Replace("@here", "@ here");
            }
            await ReplyAsync(sentMessage);
        }


        [Command("embed")]
        public async Task CreateEmbedAsync(string argStringColor, string _title = "", [Remainder] string _text = "")
        {
            string stringColor = argStringColor.ToLowerInvariant();
            Color color;

            switch (stringColor)
            {
                case "red":
                    color = Color.Red;
                    break;
                case "blue":
                    color = Color.Blue;
                    break;
                case "green":
                    color = Color.Green;
                    break;
                case "orange":
                    color = Color.Orange;
                    break;
                case "purple":
                    color = Color.Purple;
                    break;
                case "magenta":
                    color = Color.Magenta;
                    break;
                case "gold":
                    color = Color.Gold;
                    break;
                case "teal":
                    color = Color.Teal;
                    break;
                case "black":
                    color = new Color(0, 0, 0);
                    break;
                case "grey":
                    color = Color.LightGrey;
                    break;
                case "yellow":
                    color = new Color(255, 255, 0);
                    break;
                case "cyan":
                    color = new Color(0, 255, 255);
                    break;
                case "white":
                    color = new Color(0, 0, 0);
                    break;
                case "darkgrey":
                    color = Color.DarkerGrey;
                    break;
                default:
                    color = Color.LighterGrey;
                    _text = $"{_title} {_text}";
                    _title = argStringColor;
                    break;
            }

            EmbedBuilder builder = new EmbedBuilder();//Make the embed builder that makes the embed

            builder.WithTitle(_title)
                .WithColor(color)
                .WithDescription(_text);


            await ReplyAsync("", false, builder.Build());//This here makes the bot print the embed
        }


        /*
        [Command("emoji")]
        [Alias("emojitopicture", "picture", "emote")]
        public async Task EmojiToPictureAsync(Emote _emote)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($":{_emote.Name}:");
            await ReplyAsync("",false, builder.Build());
        }
        */


    }
}
