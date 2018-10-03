using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDestrucoBot.Modules
{
    [Group("say")]
    public class Say : ModuleBase<SocketCommandContext>
    {

        [Command("several"), RequireUserPermission(GuildPermission.Administrator)]
        [Alias("multible", "few")]
        public async Task SayMultible(int numberof, [Remainder] string argumentname)
        {
            if (numberof > 5)
            {
                await ReplyAsync("You can not spam more then 5 messages at once");
                return;
            }

            argumentname = argumentname.Replace("u", "us");
            argumentname = argumentname.Replace("m", "ms");
            argumentname = argumentname.Replace("s", "ss");
            argumentname = argumentname.Replace("a", "as");
            argumentname = argumentname.Replace("e", "es");

            for (int i = 0; i< numberof; i++)
                await ReplyAsync('\u200B' + argumentname);
            //the '\u200B' will prevent other bots from picking up on the message
        }

        [Command, RequireUserPermission(ChannelPermission.MentionEveryone)]
        public async Task SayAsync([Remainder] string argumentname)
        {

            await ReplyAsync('\u200B' + argumentname);
            //the '\u200B' will prevent other bots from picking up on the message
        }


        [Command]
        public async Task SayAsyncNoEveryone([Remainder] string argumentname)
        {


            argumentname = argumentname.Replace("@everyone", "everyone");
            argumentname = argumentname.Replace("@here", "here");

            await ReplyAsync('\u200B' + argumentname);
        }
        
    }
}
