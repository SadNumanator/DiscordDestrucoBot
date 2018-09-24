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
