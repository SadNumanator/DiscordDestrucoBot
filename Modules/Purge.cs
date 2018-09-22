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
    public class Purge : ModuleBase<SocketCommandContext>
    {

        //RunMode = RunMode.Async is required for setting delays
        [Command("purge", RunMode = RunMode.Async)][RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeAsync(int purging = 0)
        {
            SocketGuildUser sender = (SocketGuildUser)Context.User;
            ChannelPermissions senderperms = sender.GetPermissions((IGuildChannel)Context.Channel);
            if (senderperms.ManageMessages != true && sender.Id != 196475292888465410)//The id is TheSadNumanators Id (the person who made this bot)
            {
                await ReplyAsync("You require the manage messages permission in this channel.");
                return;
            }


            if (purging <= 0)
                return;

            RestUserMessage toDelete;

            string purgemessage = "";

            await Context.Message.DeleteAsync(); //delete the message that sent this command
            var messages = await Context.Channel.GetMessagesAsync(purging).FlattenAsync(); //purging is the value
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);


            if (purging >= 100)
                purgemessage = @"```csharp
#𝐈𝐧𝐧𝐨𝐜𝐞𝐧𝐭𝐬 𝐏𝐮𝐫𝐠𝐞𝐝!
```";

            else if (purging > 1)
                purgemessage = $"***Innocents Purged!***";

            else if (purging > 0)
                purgemessage = $"**Innocent Purged!**";


            toDelete = await Context.Channel.SendMessageAsync(purgemessage);
                await Task.Delay(3000); //starting delay
                await toDelete.DeleteAsync();


        }
    }
}
