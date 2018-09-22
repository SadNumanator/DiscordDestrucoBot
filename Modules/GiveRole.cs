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

namespace DiscordDestrucoBot.Modules
{
    [Group]
    [RequireUserPermission(ChannelPermission.ManageRoles)][RequireBotPermission(ChannelPermission.ManageRoles)]
    public class ChangeRoles : ModuleBase<SocketCommandContext>
    {
        //giverole must be said to use this command
        [Command("giverole")]
        public async Task GiveRoleAsync(IRole roletogive, [Remainder]SocketGuildUser Arguser)//The arguments are the role to give and the user to give it to
        {//Make sure the person assigning the role has a higher role then the assigned role
            if (roletogive.Position >= AuthorRolePos((SocketGuildUser)Context.User) && Context.User.Id != Context.Guild.OwnerId)
            {
                await ReplyAsync("Your highest role needs to be above the role your giving"); return;
            }
            //Make sure the bot assigning the role has a higher role then the assigned role
            if (roletogive.Position >= AuthorRolePos(Context.Guild.GetUser(Context.Client.CurrentUser.Id)))
            {
                await ReplyAsync("The bots highest role needs to be above the one you are giving"); return;
            }


            await Arguser.AddRoleAsync(roletogive);//Add the role

            await ReplyAsync($"{roletogive.Name} has been given to {Arguser.Username} by {Context.Message.Author.Mention}");
        }


        [Command("giverole")]//Same thing as above but giving a role to every member of a role instead
        public async Task GiveRoleAsync(IRole roletogive, [Remainder]SocketRole roletoadd)
        {
            if (roletogive.Position >= AuthorRolePos((SocketGuildUser)Context.User))
            {
                await ReplyAsync("Your highest role needs to be above the role your giving"); return;

            }
            if (roletogive.Position >= AuthorRolePos(Context.Guild.GetUser(Context.Client.CurrentUser.Id)))
            {
                await ReplyAsync("The bots highest role needs to be above the one you are giving"); return;
            }

            foreach (var member in roletoadd.Members)
                await member.AddRoleAsync(roletogive);

            await ReplyAsync($"{roletogive.Name} has been given to all of {roletoadd.Name} by {Context.Message.Author.Mention}");
        }


        
        [Command("removerole")]//Same thing as above but the oppisite, it removes the role instead
        public async Task RemoveRoleAsync(IRole roletoremove, [Remainder]SocketGuildUser Arguser)
        {
            if (roletoremove.Position >= AuthorRolePos((SocketGuildUser)Context.User))
            {
                await ReplyAsync("Your highest role needs to be above the role your removing"); return;
            }
            if (roletoremove.Position >= AuthorRolePos(Context.Guild.GetUser(Context.Client.CurrentUser.Id)))
            {
                await ReplyAsync("The bots highest role needs to be above the one you are removing"); return;
            }

            await Arguser.AddRoleAsync(roletoremove);

            await ReplyAsync($"{roletoremove.Name} has been removed from {Arguser.Username} by {Context.Message.Author.Mention}");
        }


        [Command("removerole")]
        public async Task RemoveRoleAsync(IRole roletotake, [Remainder]SocketRole roletoremove)
        {
            if (roletoremove.Position >= AuthorRolePos((SocketGuildUser)Context.User))
            {
                await ReplyAsync("Your highest role needs to be above the role your removing"); return;
            }
            if (roletoremove.Position >= AuthorRolePos(Context.Guild.GetUser(Context.Client.CurrentUser.Id)))
            {
                await ReplyAsync("The bots highest role needs to be above the one you are removing"); return;
            }

            foreach (var member in roletoremove.Members)
                await member.RemoveRoleAsync(roletotake);

            await ReplyAsync($"{roletotake.Name} has been removed from all of {roletoremove.Name} by {Context.Message.Author.Mention}");
        }









        private static int AuthorRolePos(SocketGuildUser Arguser)
        {
            int _rolePosition = 0;
            foreach (var _role in Arguser.Roles)
            {
                if (_role.Position > _rolePosition)
                {
                    _rolePosition = _role.Position;
                }
            }
            return _rolePosition;
        }
    }
}
