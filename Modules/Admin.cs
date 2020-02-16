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
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Command("kick")][RequireBotPermission(GuildPermission.KickMembers)][RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickUserAsync(SocketGuildUser user, [Remainder] string reason = "No reason provided.")
        {
            if (user.Id == Context.Client.CurrentUser.Id) { await ReplyAsync($"User {Context.User.Mention} is a bot abuser."); return; }
            if (user.Id == Context.User.Id) { await ReplyAsync("Why dont you just leave."); return; }
            if (user.Id == Context.Guild.OwnerId) { await ReplyAsync("You can't kick the owner of the server"); return; }
            if (user.Hierarchy >= ((SocketGuildUser)Context.User).Hierarchy)
            {
                await ReplyAsync("Your highest role must be above the highest role of the user you are kicking"); return;
            }

            if (user.Hierarchy >= (Context.Guild.GetUser(Context.Client.CurrentUser.Id)).Hierarchy)
            {
                await ReplyAsync("My highest role must be higher than the user I am kicking."); return;
            }

            await user.KickAsync(reason);

            await Context.Message.DeleteAsync();
            await ReplyAsync($"User {user.Mention} kicked by {Context.User.Mention}");
        }

        [Command("ban")][RequireBotPermission(GuildPermission.BanMembers)][RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync(SocketGuildUser user, [Remainder] string reason = "No reason provided.")
        {
            if (user.Id == Context.Client.CurrentUser.Id) { await ReplyAsync("Please don't ban me D:"); return; }
            if (user.Id == Context.User.Id) { await ReplyAsync("Try spam pinging an admin if you want to be banned."); return; }
            if (user.Id == Context.Guild.OwnerId) { await ReplyAsync("You can't ban the owner of this server."); return; }

            if (user.Hierarchy >= ((SocketGuildUser)Context.User).Hierarchy)
            {
                await ReplyAsync("Your highest role must be above the highest role of the user you are banning"); return;
            }

            if (user.Hierarchy >= (Context.Guild.GetUser(Context.Client.CurrentUser.Id)).Hierarchy)
            {
                await ReplyAsync("My highest role must be higher than the user I am banning."); return;
            }

            await user.BanAsync(0, reason);

            await Context.Message.DeleteAsync();
            await ReplyAsync($"User {user.Mention} banned by {Context.User.Mention} pruning {0} days of messages");
        }














        [Command("changeprefix")]
        [Alias("setprefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeGuildPrefixAsync(string prefix)
        {
            string previousprefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());

            DataStorage.AddPair("Prefix" + Context.Guild.Id, prefix);
            await ReplyAsync($"The previous prefix was `{previousprefix}`. \nThe new prefix is now `{DataStorage.GetPrefixValue(Context.Guild.Id.ToString())}` it was set by {Context.User.Mention}. \nRemember you can use {Context.Client.CurrentUser.Mention} as a prefix.");
        }

        [Command("removeprefix")]
        [Alias("defaultprefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveGuildPrefixAsync()
        {
            if (!DataStorage.KeyExists("Prefix" + Context.Guild.Id))
            {
                await ReplyAsync("The prefix is already the default prefix."); return;
            }

            string previousprefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());

            if (DataStorage.KeyExists("Prefix" + Context.Guild.Id))
                DataStorage.RemoveKey("Prefix" + Context.Guild.Id);

            await ReplyAsync($"The previous prefix was `{previousprefix}`. \nThe new prefix is now `{Config.bot.defaultcmdPrefix}` it was set by {Context.User.Mention}");
        }


    }
}
