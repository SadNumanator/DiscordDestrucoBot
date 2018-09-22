using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDestrucoBot.Modules
{
    public class ChannelInfo : ModuleBase<SocketCommandContext>
    {
        [Command("channelinfo")]
        public async Task ChannelInfoAsync(ITextChannel _channel = null)
        {

            ITextChannel _Channel;

            if (_channel != null)
                _Channel = _channel;
            else
                _Channel = (ITextChannel)Context.Channel;

            StringBuilder correctPermissions = new StringBuilder();

            
            foreach(var permission in _Channel.PermissionOverwrites)
            {
                string _rolename = Context.Guild.GetRole(permission.TargetId).Name;
                if (permission.Permissions.ToDenyList().Count > 0)
                {
                    correctPermissions.Append($"**{_rolename} is not able to : **");
                    correctPermissions.AppendJoin(", ", permission.Permissions.ToDenyList());
                    correctPermissions.AppendLine();
                }
                if (permission.Permissions.ToAllowList().Count > 0)
                {
                    correctPermissions.Append($"**{_rolename} is able to : **");
                    correctPermissions.AppendJoin(", ", permission.Permissions.ToAllowList());
                    correctPermissions.AppendLine();
                }
            }
            if (correctPermissions.Length == 0)
                correctPermissions.Append("None");
             
            EmbedBuilder builder = new EmbedBuilder();//Make the embed builder that makes the embed
            builder.WithTitle($"Info About : {_Channel.Name}")
                .WithColor(Color.Blue)
                .AddField($"**ID**", $"```{_Channel.Id}```", true)
                .AddField($"**Creation Date**", $"```{_Channel.CreatedAt}```", true)
            .AddField($"**Perms**", $"{correctPermissions.ToString()}");
            await ReplyAsync("", false, builder.Build());//This here makes the bot print the embed
        }
    }
}
