﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDestrucoBot.Modules
{
    public class Info : ModuleBase<SocketCommandContext>
    {
        [Command("serverinfo")]
        [Alias("guildinfo")]
        public async Task InfoAsync()
        {


            string _nickname = "";//make the variable

            if (!string.IsNullOrWhiteSpace(Context.Guild.Owner.Nickname))
                _nickname = $"with the nickname of { Context.Guild.Owner.Nickname}";

            //Puts all the roles into one string
            string _roles = string.Join(", ", Context.Guild.Roles);

            if (_roles.Length > 11)//If there are more roles then just @everyone
                _roles = _roles.Replace("@everyone, ", "");
            else
                _roles = _roles.Replace("@everyone", "None");//Otherwise everyone equals none

            string _categorycount = Context.Guild.CategoryChannels.Count.ToString();
            string _textcount = Context.Guild.TextChannels.Count.ToString();
            string _voicecount = Context.Guild.VoiceChannels.Count.ToString();

            EmbedBuilder builder = new EmbedBuilder();//Make the embed builder that makes the embed

            builder.WithTitle($"Info About : {Context.Guild.Name}")
                .WithColor(Color.Blue)
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .AddField($"**ID**", $"```{Context.Guild.Id}```",true)
                .AddField($"**Owner**", $"```{Context.Guild.Owner.Username} {_nickname}```", true)
                .AddField($"**Role Count : {Context.Guild.Roles.Count - 1} **", $"```{_roles}```")
                .AddField($"**Channels**", $"```{_categorycount} Categories\n{_textcount} Text\n{_voicecount} Voice```", true)
                .AddField($"**Member Count**", $"```{Context.Guild.MemberCount}```",true)
                .AddField($"**Creation Date**", $"```{Context.Guild.CreatedAt}```", true)
                .AddField($"**Verification level**", $"```{Context.Guild.VerificationLevel.ToString()}```", true);
            await ReplyAsync("", false, builder.Build());//This here makes the bot print the embed
        }

        [Command("roleinfo")]
        public async Task RoleInfoAsync([Remainder]SocketRole roleArg)
        {//Remainder puts all the things into one string

            string _mentionable;
            if (roleArg.IsMentionable == true)
                _mentionable = "Yes";
            else
                _mentionable = "No";


            string _permissions = string.Join(", ", roleArg.Permissions.ToList());

            EmbedBuilder builder = new EmbedBuilder();//Make the embed builder that makes the embed

            builder.WithTitle($"Info About {roleArg.Name}")
                .WithColor(roleArg.Color)
                .AddField($"**Permissions**", $"```{_permissions}```")
                .AddField($"**Position**", $"```{roleArg.Position}```", true)
                .AddField($"**Mentionable**", $"```{_mentionable}```", true)
                .AddField($"**Creation Date**", $"```{roleArg.CreatedAt}```", true);

            await ReplyAsync("", false, builder.Build());//This here makes the bot print the embed
        }

        [Command("membercount")]
        public async Task MemberCountAsync()
        {
            int _botcount = 0;
            int _membercount = 0;
            int _humancount = 0;
            int _onlinecount = 0;

            foreach (var member in Context.Guild.Users)
            {
                _membercount++;
                if (member.IsBot == true)
                    _botcount++;
                else
                    _humancount++;

                if (member.Status == UserStatus.Online && member.IsBot == false)
                    _onlinecount++;
            }

            EmbedBuilder builder = new EmbedBuilder();//Make the embed builder that makes the embed
            builder.AddField("**Members**", _membercount, true)
                .AddField("**Online**", _onlinecount, true)
                .AddField("**Humans**", _humancount, true)
                .AddField("**Bots**", _botcount, true)
                .WithColor(Color.Blue);

            await ReplyAsync("", false, builder.Build());
        }


    }
}