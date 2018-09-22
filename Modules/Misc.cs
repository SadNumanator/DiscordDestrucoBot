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
            if (HighestRole(userArg) >= HighestRole(sender) && !sender.GuildPermissions.Administrator)
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
                if (userArg.Id == Context.Guild.OwnerId)
                    continue;
                if (HighestRole(userArg) >= HighestRole(botuser))
                    continue;

                await userArg.ModifyAsync(x => { x.Nickname = name; });
                _amountchanged++;
            }

            await ReplyAsync($"{_amountchanged} users have had their nicknames changed to {name} by {Context.User.Mention}");
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

        [Command("changeprefix")]
        [Alias("setprefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeGuildPrefixAsync(string prefix)
        {
            string previousprefix = DataStorage.GetPrefixValue("Prefix" + Context.Guild.Id);

            DataStorage.AddPair("Prefix" + Context.Guild.Id, prefix);
            await ReplyAsync($"The previous prefix was `{previousprefix}`. \nThe new prefix is now `{DataStorage.GetPrefixValue("Prefix" + Context.Guild.Id)}` it was set by {Context.User.Mention}. \nRemember you can use {Context.Client.CurrentUser.Mention} as a prefix.");
        }

        [Command("removeprefix")]
        [Alias("defaultprefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveGuildPrefixAsync()
        {
            if(!DataStorage.KeyExists("Prefix" + Context.Guild.Id))
            {
                await ReplyAsync("The prefix is already the default prefix."); return;
            }

            string previousprefix = DataStorage.GetPrefixValue("Prefix" + Context.Guild.Id);

            if (DataStorage.KeyExists("Prefix" + Context.Guild.Id))
                DataStorage.RemoveKey("Prefix" + Context.Guild.Id);

            await ReplyAsync($"The previous prefix was `{previousprefix}`. \nThe new prefix is now `{Config.bot.defaultcmdPrefix}` it was set by {Context.User.Mention}");
        }

        [Command("prefix")]
        public async Task GetPrefixAsync()
        {
            await ReplyAsync($"The prefix is `{DataStorage.GetPrefixValue("Prefix" + Context.Guild.Id)}`");
        }



        private int HighestRole(SocketGuildUser userArg)
        {
            return userArg.Roles.OrderBy(x => x.Position).Last().Position;
        }
    }
}
