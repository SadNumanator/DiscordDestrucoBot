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
    public class Purge : ModuleBase<SocketCommandContext>
    {



        //RunMode = RunMode.Async is required for setting delays
        [Command("purge", RunMode = RunMode.Async)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeAsync(int purging = 0)
        {

            SocketGuildUser sender = (SocketGuildUser)Context.User;
            ChannelPermissions senderperms = sender.GetPermissions((IGuildChannel)Context.Channel);
            if (!senderperms.ManageMessages && sender.Id != 196475292888465410)//The id is TheSadNumanators Id (the person who made this bot)
            {
                await ReplyAsync("You require the manage messages permission in this channel.");
                return;
            }

            if (purging <= 0)
                return;

            if (purging > 200 && !senderperms.ManageChannel)
            {
                await ReplyAsync("You can not purge more then 200 messages at once unless you have the manage channel permission."); return;
            }

            if (purging > 1000) { await ReplyAsync("You can not purge more then 1000 messages at once."); return; }

            string purgemessage = "";

            await Context.Message.DeleteAsync(); //delete the message that sent this command
            var messages = await Context.Channel.GetMessagesAsync(purging).FlattenAsync(); //purging is the value
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            if (purging >= 500)
                purgemessage = @"```csharp
#̨᷇͊᷀͠ͅ𝐈̱̤̠̓̋͞͞𝐧́̇̈́̋᷆𝐧͖͕̓𝐨͙͉̯̖҆᷄̈́̎𝐜̩̗ͭ͗𝐞͎̫҃̃ͥͦ͛͜𝐧̷̟̫́̈𝐭ͥ҃̀᷁ͣ𝐬̖ͪ̑᷆ ͉̃ͭ͋҄͞𝐏̭͓ͣ̊̽͒𝐮̰ͮ𝐫̶͕̌͘ͅ𝐠̡̲̗᷄͢𝐞̸̲͊͌̀𝐝̖̩̥̣̈́ͮ!̨̉͠
```";//This will show up as "#Innocents Purged" in fancy bold zalgo^
            else if (purging >= 100)
                purgemessage = @"```csharp
#𝐈𝐧𝐧𝐨𝐜𝐞𝐧𝐭𝐬 𝐏𝐮𝐫𝐠𝐞𝐝!
```";

            else if (purging > 1)
                purgemessage = $"***Innocents Purged!***";

            else if (purging > 0)
                purgemessage = $"**Innocent Purged!**";

            RestUserMessage toDelete = await Context.Channel.SendMessageAsync(purgemessage);
            await Task.Delay(3000); //starting delay
            await toDelete.DeleteAsync();
        }

        //Purging for only commands sent by a user
        [Command("purge", RunMode = RunMode.Async)][Alias("purge user")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeAsync(int purging,[Remainder] IGuildUser userToPurge)
        {

            SocketGuildUser sender = (SocketGuildUser)Context.User;
            ChannelPermissions senderperms = sender.GetPermissions((IGuildChannel)Context.Channel);
            if (!senderperms.ManageMessages && sender.Id != 196475292888465410)//The id is TheSadNumanators Id (the person who made this bot)
            {
                await ReplyAsync("You require the manage messages permission in this channel.");
                return;
            }

            if (purging <= 0)
                return;

            if (purging > 100) { await ReplyAsync("You can not purge more then 100 messages at once from a user."); return; }

            string purgemessage = "";

            await Context.Message.DeleteAsync(); //delete the message that sent this command
            var messages = await Context.Channel.GetMessagesAsync(800).FlattenAsync(); //purging is the value
            messages = messages.Where(msg => msg.Author.Id == userToPurge.Id).Take(purging);
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
           
            if (purging >= 100)
                purgemessage = @"```csharp
#𝐈𝐧𝐧𝐨𝐜𝐞𝐧𝐭𝐬 𝐏𝐮𝐫𝐠𝐞𝐝!
```";

            else if (purging > 1)

                purgemessage = $"***Innocents Purged!***";
            else if (purging > 0)
                purgemessage = $"**Innocent Purged!**";

            RestUserMessage toDelete = await Context.Channel.SendMessageAsync(purgemessage);
            await Task.Delay(3000); //starting delay
            await toDelete.DeleteAsync();
        }

        //Purging command but only for a role
        [Command("purge", RunMode = RunMode.Async)]
        [Alias("purge role")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeAsync(int purging, [Remainder] SocketRole roleToPurge)
        {

            SocketGuildUser sender = (SocketGuildUser)Context.User;
            ChannelPermissions senderperms = sender.GetPermissions((IGuildChannel)Context.Channel);
            if (!senderperms.ManageMessages && sender.Id != 196475292888465410)//The id is TheSadNumanators Id (the person who made this bot)
            {
                await ReplyAsync("You require the manage messages permission in this channel.");
                return;
            }

            if (purging <= 0)
                return;
            if (purging > 200 && !senderperms.ManageChannel)
            {
                await ReplyAsync("You can not purge more then 200 messages at once unless you have the manage channel permission."); return;
            }
            if (purging > 500) { await ReplyAsync("You can not purge more then 500 messages at once from a role."); return; }

            await Context.Message.DeleteAsync(); //delete the message that sent this command
            var messages = await Context.Channel.GetMessagesAsync(800).FlattenAsync(); //purging is the value
            messages = messages.Where(msg => ((SocketGuildUser)msg.Author).Roles.Contains(roleToPurge)).Take(purging);
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            string purgemessage = "";
            if (purging >= 100)
                purgemessage = @"```csharp
#𝐈𝐧𝐧𝐨𝐜𝐞𝐧𝐭𝐬 𝐏𝐮𝐫𝐠𝐞𝐝!
```";

            else if (purging > 1)

                purgemessage = $"***Innocents Purged!***";
            else if (purging > 0)
                purgemessage = $"**Innocent Purged!**";

            RestUserMessage toDelete = await Context.Channel.SendMessageAsync(purgemessage);
            await Task.Delay(3000); //starting delay
            await toDelete.DeleteAsync();
        }


    }
}