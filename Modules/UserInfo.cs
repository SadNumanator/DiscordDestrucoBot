using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDestrucoBot.Modules
{
    [Group("userinfo")]
    [Alias("whois")]
    public class UserInfo : ModuleBase<SocketCommandContext>
    {

        [Command("perms")]
        public async Task UserPermissionsAsync([Remainder]SocketGuildUser userArg)
        {

            string _nickname = "";//make the variable

            if (!string.IsNullOrWhiteSpace(userArg.Nickname))//If the nickname of the discord user is not nothing
                _nickname = $"with the nickname of { userArg.Nickname}";//change _nickname to show the nickname


            string _permissions = string.Join(", ", userArg.GuildPermissions.ToList());

            Color roleColor = GetColor(userArg);

            EmbedBuilder builder = new EmbedBuilder();//Make the embed builder that makes the embed

            builder.WithTitle($"Info About {userArg.Username} {_nickname}")
                .WithColor(roleColor)
            .AddField($"**Permissions:**", $"```{_permissions}```", true);

            await ReplyAsync("", false, builder.Build());//This here makes the bot print the embed
        }


        [Command]
        public async Task UserInfoAsync([Remainder]SocketGuildUser userArg)
        {//Remainder puts all the things into one string

            string _nickname = "";//make the variable

            if (!string.IsNullOrWhiteSpace(userArg.Nickname))//If the nickname of the discord user is not nothing
                _nickname = $"with the nickname of { userArg.Nickname}";//change _nickname to show the nickname

            //Puts all the roles into one string
            string _roles = string.Join(", ", userArg.Roles);

            if (_roles.Length > 10)//If there are more roles then just @everyone
                _roles = _roles.Replace("@everyone, ", "");
            else
                _roles = _roles.Replace("@everyone", "None");//Otherwise everyone equals none

            string _permissions = string.Join(", ", userArg.GuildPermissions.ToList());

            Color roleColor = GetColor(userArg);

            EmbedBuilder builder = new EmbedBuilder();//Make the embed builder that makes the embed

            builder.WithTitle($"Info About {userArg.Username}" + _nickname)
                .WithColor(roleColor)
                .WithThumbnailUrl(userArg.GetAvatarUrl())
            .AddField($"**ID:**", $"```{userArg.Id}```")
            .AddField($"**Server Join Date:**", $"```{userArg.JoinedAt}```", true)
            .AddField($"**Account Creation Date:**", $"```{userArg.CreatedAt}```", true)
            .AddField($"**Status:**", $"```{userArg.Status}```", true)
            .AddField($"**Roles:**", $"```{_roles}```")
            .AddField($"**Permissions:**", $"```Please use {DataStorage.GetPrefixValue("Prefix" + Context.Guild.Id)}userinfo perms @user```", true);

            await ReplyAsync("", false, builder.Build());//This here makes the bot print the embed
        }

        private static Color GetColor(SocketGuildUser userArg)
        {
            Color roleColor = Color.LighterGrey;
            int _rolePosition = 0;

            //For each role that the user has check if its position is higher then another role and find the highest.
            //Then assign the color from the highest role
            foreach (var _role in userArg.Roles)
            {
                if (_role.Position > _rolePosition)
                {
                    _rolePosition = _role.Position;
                    roleColor = _role.Color;
                }
            }

            return roleColor;
        }
    }
}
