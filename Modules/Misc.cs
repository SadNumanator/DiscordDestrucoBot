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

            await userArg.ModifyAsync(x => { x.Nickname = name; });

            RestUserMessage toDelete = await Context.Channel.SendMessageAsync($"{userArg.Username}s Nickname has changed to {name}");
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

            int _amountchanged = 0;
            var botuser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            foreach (SocketGuildUser userArg in roleArg.Members) {
                if (userArg.Hierarchy >= botuser.Hierarchy)
                    continue;

                await userArg.ModifyAsync(x => { x.Nickname = name; });
                _amountchanged++;
            }

            await ReplyAsync($"{_amountchanged} users have had their nicknames changed to {name} by {Context.User.Mention}");
        }


        [Command("embed")]
        public async Task CreateEmbedAsync(string argStringColor, string _title = "", [Remainder] string _text = "")
        {
            string stringColor = argStringColor.ToLowerInvariant();
            Color color;

            if (stringColor == "red")
                color = Color.Red;
            else if (stringColor == "blue")
                color = Color.Blue;
            else if (stringColor == "green")
                color = Color.Green;
            else if (stringColor == "orange")
                color = Color.Orange;
            else if (stringColor == "purple")
                color = Color.Purple;
            else if (stringColor == "magenta")
                color = Color.Magenta;
            else if (stringColor == "gold")
                color = Color.Gold;
            else if (stringColor == "teal")
                color = Color.Teal;
            else if (stringColor == "darkgrey")
                color = Color.DarkerGrey;
            else
            {
                color = Color.LighterGrey;
                _text = $"{_title} {_text}";
                _title = argStringColor;
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
        public async Task EmojiToPictureAsync(GuildEmote _emote)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"\\:{_emote.Name}:");
            await ReplyAsync(null ,false, builder.Build());
        }
        */


    }
}
