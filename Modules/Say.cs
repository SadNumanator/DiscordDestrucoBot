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
        public async Task SayAsync([Remainder] string argumentname)//This is a optional paramater that you type after !ping awawawawa
        {//Remainder puts all the things into one string


            await ReplyAsync(argumentname);
        }


        [Command]
        public async Task SayAsyncNoEveryone([Remainder] string argumentname)//This is a optional paramater that you type after !ping awawawawa
        {//Remainder puts all the things into one string


            argumentname = argumentname.Replace("@everyone", "everyone");
            argumentname = argumentname.Replace("@here", "here");

            await ReplyAsync(argumentname);
        }
        
    }
}
