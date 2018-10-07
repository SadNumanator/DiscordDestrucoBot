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
using System.Net;
using System.Linq;

namespace DiscordDestrucoBot.Modules
{
    public class Help : ModuleBase<SocketCommandContext>
    {


        [Command("newhelp", RunMode = RunMode.Async)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task HelpCommandAsync([Remainder]string menu)
        {
            //RestUserMessage helpMessage = null;
            //EmbedBuilder builder;

            string prefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());

            var menusystem = new MenuSystem();

            //Base menus
            menusystem.AddMenu("Help", null);
            menusystem.AddMenu("Help-Admin", "Help", Color.Red);
            menusystem.AddMenu("Help-Misc", "Help", Color.Green);
            menusystem.AddMenu("Help-Fun", "Help", new Color(66, 226, 244));
            menusystem.AddMenu("Help-Info", "Help", Color.Purple);

            //Starting help menu
            menusystem.AddFieldInMenu("Help", "Help-Info", "**Info-1**", "");
            menusystem.AddFieldInMenu("Help", "Help-Fun", "**Fun-2**", "");
            menusystem.AddFieldInMenu("Help", "Help-Misc", "**Misc-3**", "");
            menusystem.AddFieldInMenu("Help", "Help-Admin", "**Admin-4**", "");
            menusystem.AddFieldInMenu("Help", "", "", "Press one of the reaction numbers or send a number key to go to the numbered labeled menu.");
            //Info menu

            menusystem.AddFieldInMenu("Help-Info", "Info-serverinfo", "Server info-1", $"`{prefix}serverinfo`");
            menusystem.AddFieldInMenu("Help-Info", "Info-userinfo", "User info-2", $"`{prefix}userinfo <user>`");
            menusystem.AddFieldInMenu("Help-Info", "Info-roleinfo", "Role info-3", $"`{prefix}roleinfo <role>` ");
            menusystem.AddFieldInMenu("Help-Info", "Info-channelinfo", "Channel info-4", $"`{prefix}channelinfo (channel)`");
            menusystem.AddFieldInMenu("Help-Info", "Info-userpermissions", "User permissions-5", $"`{prefix}userperms <user>`");
            menusystem.AddFieldInMenu("Help-Info", "Info-membercount", "Member count-6", $"`{prefix}membercount`");
            menusystem.AddFieldInMenu("Help-Info", "Info-help", "Help-7", $"`{prefix}help`");
            //Fun menu
            menusystem.AddFieldInMenu("Help-Fun", "Fun-dog|cat", "Dog or Cat pictures-1", $"`{prefix}dog|{prefix}cat`");
                menusystem.AddFieldInMenu("Help-Fun", "Fun-birb", "Birds-2 `", $"`{prefix}birb`");
                menusystem.AddFieldInMenu("Help-Fun", "Fun-randomnumber", "Random number-3", $"`{prefix}randomnumber <number> || <minnumber> <maxnumber>`");
                menusystem.AddFieldInMenu("Help-Fun", "Fun-choose", "Choose-4", $"`{prefix}choose (value)(value)(value)(value)(value)(value)..`");
            //Misc menu
            menusystem.AddFieldInMenu("Help-Misc", "Misc-rename", "Rename Users-1", $"`{prefix}rename <user> <nickname>{prefix}rename <role> <nickname>`");
                menusystem.AddFieldInMenu("Help-Misc", "Misc-say", "Say-2 `", $"`{prefix}say <text>`");
                menusystem.AddFieldInMenu("Help-Misc", "Misc-embed", "Embed text-3 `", $"`{prefix}embed (color) <title> <text>`");
                menusystem.AddFieldInMenu("Help-Misc", "Misc-ping", "Ping&Pong-4", $"`{prefix}Ping|{prefix}Pong`");
            //Admin menu
            menusystem.AddFieldInMenu("Help-Admin", "Admin-kick", "Kick-1", $"`{prefix}kick <user> (reason)`");
                menusystem.AddFieldInMenu("Help-Admin", "Admin-ban", "Ban-2", $"`{prefix}ban <user> (reason)`");
                menusystem.AddFieldInMenu("Help-Admin", "Admin-purge", "Purge-3", $"`{prefix}purge <amount>`");
                menusystem.AddFieldInMenu("Help-Admin", "Admin-g&r_role", "Give and remove Roles-4", $"`{prefix}giverole <roletogive> <user>|<roletogive> <role> ||{prefix}removerole <roletoremove> <user>|<roletoremove> <role>`");
                menusystem.AddFieldInMenu("Help-Admin", "Admin-changeprefix", "Change the Prefix-5", $"`{prefix}changePrefix <newprefix>`");
                menusystem.AddFieldInMenu("Help-Admin", "Admin-defaultprefix", "Reset the Prefix-6", $"`{prefix}defaultprefix`");




            try
            {
                await menusystem.DoCurrentMenuStuff(Context);
            }
            catch(Exception ex)
            {
                await ReplyAsync(ex.Message);
            }

        }

        [Command("Help", RunMode = RunMode.Async)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task HelpCommandAsync()
        {

            Emoji[] digits = new Emoji[] {
                new Emoji("\U00000030\U000020e3"), new Emoji("\U00000031\U000020e3"),
                new Emoji("\U00000032\U000020e3"), new Emoji("\U00000033\U000020e3"),
                new Emoji("\U00000034\U000020e3"), new Emoji("\U00000035\U000020e3")
            , new Emoji("\U00000036\U000020e3"), new Emoji("\U00000037\U000020e3")
            , new Emoji("\U00000038\U000020e3"), new Emoji("\U00000039\U000020e3")};

            Emoji back = new Emoji("🔄");
            RestUserMessage helpMessage = null;
            EmbedBuilder builder;

            int[] digitCounts = new int[digits.Length];
            const int fulltimer = 70;


        Start:

            int looping = 0;

            string prefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());

            builder = new EmbedBuilder();
            builder.WithTitle("Help-" + looping).WithColor(Color.Blue)
                .WithDescription("**Info** 1\n\n**Fun** 2\n\n**Misc** 3\n\n**Admin** 4 \n\n" +
                "Press one of the reaction numbers to go to the labeled menu.");

            if (helpMessage == null)
                helpMessage = await Context.Channel.SendMessageAsync("", false, builder.Build());
            else
                await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());




            await helpMessage.UpdateAsync();

            if (!helpMessage.Reactions.ContainsKey(digits[1]))
                await helpMessage.AddReactionAsync(digits[1]);
            if (!helpMessage.Reactions.ContainsKey(digits[2]))
                await helpMessage.AddReactionAsync(digits[2]);
            if (!helpMessage.Reactions.ContainsKey(digits[3]))
                await helpMessage.AddReactionAsync(digits[3]);
            if (!helpMessage.Reactions.ContainsKey(digits[4]))
                await helpMessage.AddReactionAsync(digits[4]);
            if (!helpMessage.Reactions.ContainsKey(digits[1]))
                await helpMessage.AddReactionAsync(digits[5]);


            Middle:

            int timer = fulltimer;
            while (true)
            {

                await helpMessage.UpdateAsync();
                for (int i = 1; i < helpMessage.Reactions.Count; i++)
                    digitCounts[i] = helpMessage.Reactions.GetValueOrDefault(digits[i]).ReactionCount;

                //INFO
                if (digitCounts[1] > 1 || looping == 1)
                {
                    looping = 1;

                    prefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());
                    builder = new EmbedBuilder();
                    builder.WithTitle("Help-Info").WithColor(Color.Purple)
                .AddField("Server info-1", $"`{prefix}serverinfo`")
                .AddField("User info-2", $"`{prefix}userinfo <user>`")
                .AddField("Role info-3", $"`{prefix}roleinfo <role>` ")
                .AddField("Channel info-4", $"`{prefix}channelinfo (channel)`")
                .AddField("User permissions-5", $"`{prefix}userperms <user>`")
                .AddField("Member count-6", $"`{prefix}membercount`")
                .AddField("Help-7", $"`{prefix}help`");


                    await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());

                    timer = fulltimer;

                    await RemoveAllButOneEmote(digits, helpMessage, 1);

                    await helpMessage.AddReactionAsync(digits[6]);
                    await helpMessage.AddReactionAsync(digits[7]);
                    await helpMessage.AddReactionAsync(back);
                    break;
                }


                //FUN
                else if (digitCounts[2] > 1 || looping == 2)
                {
                    looping = 2;

                    prefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());
                    builder = new EmbedBuilder();
                    builder.WithTitle("Help-Fun").WithColor(new Color(66, 226, 244))
                .AddField("Dog or Cat pictures-1", $"`{prefix}dog|{prefix}cat`")
                .AddField("Birds-2 `", $"`{prefix}birb`")
                .AddField("Random number-3", $"`{prefix}randomnumber <number> || <minnumber> <maxnumber>`")
                .AddField("Choose-4", $"`{prefix}choose (value)(value)(value)(value)(value)(value)..`");


                    await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());

                    timer = fulltimer;

                    await RemoveAllButOneEmote(digits, helpMessage, 2);
                    await helpMessage.AddReactionAsync(back);
                    break;
                }

                //MISC
                else if (digitCounts[3] > 1 || looping == 3)
                {
                    looping = 3;

                    prefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());
                    builder = new EmbedBuilder();
                    builder.WithTitle("Help-Misc").WithColor(Color.Green)
                .AddField("Rename Users-1", $"`{prefix}rename <user> <nickname>{prefix}rename <role> <nickname>`")
                .AddField("Say-2 `", $"`{prefix}say <text>`")
                .AddField("Embed text-3 `", $"`{prefix}embed (color) <title> <text>`")
                .AddField("Ping&Pong-4", $"`{prefix}Ping|{prefix}Pong`");


                    await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());

                    timer = fulltimer;

                    await RemoveAllButOneEmote(digits, helpMessage, 3);
                    await helpMessage.AddReactionAsync(back);
                    break;
                }

                //ADMIN
                else if (digitCounts[4] > 1 || looping == 4)
                {
                    looping = 4;

                    prefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());
                    builder = new EmbedBuilder();
                    builder.WithTitle("Help-Admin").WithColor(Color.Red)
                .AddField("Kick-1", $"`{prefix}kick <user> (reason)`")
                .AddField("Ban-2", $"`{prefix}ban <user> (reason)`")
                .AddField("Purge-3", $"`{prefix}purge <amount>`")
                .AddField("Give and remove Roles-4", $"`{prefix}giverole <roletogive> <user>|<roletogive> <role> ||{prefix}removerole <roletoremove> <user>|<roletoremove> <role>`")
                .AddField("Change the Prefix-5", $"`{prefix}changePrefix <newprefix>`")
                .AddField("Reset the Prefix-6", $"`{prefix}defaultprefix`");

                    await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());

                    timer = fulltimer;

                    await RemoveAllButOneEmote(digits, helpMessage, 4);

                    await helpMessage.AddReactionAsync(digits[6]);
                    await helpMessage.AddReactionAsync(back);
                    break;
                }



                if (timer <= 0)
                {
                    looping = -1;
                    break;
                }

                timer--;

                await Task.Delay(500);
            }


        End:
            while (true)
            {

                await helpMessage.UpdateAsync();
                for (int i = 1; i < helpMessage.Reactions.Count; i++)
                    digitCounts[i] = helpMessage.Reactions.GetValueOrDefault(digits[i]).ReactionCount;

                #region INFO
                //INFO
                if (looping == 1)
                {
                    prefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());


                    if (digitCounts[1] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Server info").WithColor(Color.Purple)
                            .AddField($"{prefix}serverinfo", "This will give info about the server/guild you are currently in.");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 1);
                        timer = fulltimer;
                        break;
                    }
                    else if (digitCounts[2] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("User info").WithColor(Color.Purple)
                            .AddField($"{prefix}userinfo <user>", "This will give info about the user you specify.");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 2);
                        timer = fulltimer;
                        break;
                    }
                    else if (digitCounts[3] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Role info").WithColor(Color.Purple)
                            .AddField($"{prefix}roleinfo <role>", "This will give info about the role you specify.");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 3);
                        timer = fulltimer;
                        break;
                    }
                    else if (digitCounts[4] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Channel info").WithColor(Color.Purple)
                            .AddField($"{prefix}channelinfo <channel>", "Will give info about the channel you specify.");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 4);
                        timer = fulltimer;
                        break;
                    }
                    else if (digitCounts[5] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Member Count").WithColor(Color.Purple)
                            .AddField($"{prefix}membercount", "This shows the amount of members and the amount of bots and humans and the amount of users online.");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 5);
                        timer = fulltimer;
                        break;
                    }
                    else if (digitCounts[6] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("User permissions").WithColor(Color.Purple)
                            .AddField($"{prefix}userperms <user>", "This will show all the available permission a user has in the channel its used in");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 6);
                        timer = fulltimer;
                        break;
                    }
                    else if (digitCounts[7] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Help").WithColor(Color.Purple)
                            .AddField($"{prefix}Help", "**Help Me!**");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 7);
                        timer = fulltimer;
                        break;
                    }
                }
                #endregion


                #region FUN
                //FUN
                else if (looping == 2)
                {
                    prefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());

                    if (digitCounts[1] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Dog and Cat pictures").WithColor(new Color(66, 226, 244))
                            .AddField($"{prefix}dog\n{prefix}cat", "This takes a random picture off who knows where of either a dog or cat and sends it here.");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 1);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[2] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Birbs").WithColor(new Color(66, 226, 244))
                            .AddField($"{prefix}birb", "This command sends memes of ~~birds~~ **birbs**.");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 2);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[3] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Random number").WithColor(new Color(66, 226, 244))
                            .AddField($"{prefix}randomnumber <MaxNumber> \n{prefix}randomnumber <MinNumber> <MaxNumber>", $"This can either send a number between 0 and whatever one number you pick `{prefix}randomnumber 10` this will return a number between 0 and 10.\n" +
                            $"If you input two numbers, it will get a number between them both `{prefix}randomnumber 50 100` this will return a number between 50 and 100.");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 3);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[4] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Choose").WithColor(new Color(66, 226, 244))
                            .AddField($"{prefix}choose", $"This will choose a value you give it, for example if you give it 10 43 and 71 `{prefix}choose 10 43 71` it will randomly pick one of those 3 values.");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 4);
                        timer = fulltimer;
                        break;
                    }
                }
                #endregion

                #region MISC
                //MISC
                else if (looping == 3)
                {
                    prefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());

                    if (digitCounts[1] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Rename Users").WithColor(Color.Green)
                            .AddField($"{prefix}rename users <user> <nickname> \n{prefix}rename <role> <nickname>", "This can either rename a single user \n" +
                            "or it can rename every user that has a specified role");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 1);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[2] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Say").WithColor(Color.Green)
                            .AddField($"{prefix}say", $" Did someone {prefix}say Thunderfury, Blessed Blade of the Windseeker? ");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 2);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[3] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Embed Text").WithColor(Color.Green)
                            .AddField($"{prefix}embed (color) <title> (text)", "This allows you to input a color, a title, and text\n" +
                            $"If you only input one argument it becomes just the title, if you input two and the first one is a color it becomes a title with a color \n" +
                            $"If you input 2 arguments without the first one being a color it becomes the title and some text \n" +
                            $"3 arguments would be the color the title and the text, all this creates a embed which the bot says");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 3);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[4] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Ping&Pong").WithColor(Color.Green)
                            .AddField($"{prefix}ping \n{prefix}pong", "The bot is not happy with what you serve, so it returns it right away");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 4);
                        timer = fulltimer;
                        break;
                    }

                }
                #endregion


                #region ADMIN
                //ADMIN
                else if (looping == 4)
                {
                    prefix = DataStorage.GetPrefixValue(Context.Guild.Id.ToString());

                    if (digitCounts[1] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Kick").WithColor(Color.Red)
                            .AddField($"{prefix}kick <user> (reason)", "For when your pal says @everyone");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 1);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[2] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Ban").WithColor(Color.Red)
                            .AddField($"{prefix}ban <user> (reason)", "For bringing down the ban hammer");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 2);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[3] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Purge").WithColor(Color.Red)
                            .AddField($"{prefix}purge <amount>", "when you want to PURGE THE INNOCENTS.  \n" +
                            "This command allows you to remove a specified amount of messages");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 3);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[4] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Give and remove Roles").WithColor(Color.Red)
                            .AddField($"{prefix}giverole <roletogive> <user>|<roletogive> <role> ||\n{prefix}removerole <roletoremove> <user>|<roletoremove> <role>",
                            "This allows you to give or remove a role from a specified user \n" +
                            "If you are a admin it also allows you to give or remove a role to every user in a specified role");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 4);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[5] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Change Prefix").WithColor(Color.Red)
                            .AddField($"{prefix}changeprefix <newprefix>", $"This changes the {prefix}prefix to the text you choose \n" +
                            $"Remember you can use {Context.Client.CurrentUser.Mention} as a prefix as well (mentioning the bot)");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 5);
                        timer = fulltimer;
                        break;
                    }
                    if (digitCounts[6] > 1)
                    {
                        builder = new EmbedBuilder();
                        builder.WithTitle("Default Prefix").WithColor(Color.Red)
                            .AddField($"{prefix}defaultprefix", $"Changes the prefix back to the default which is {Config.bot.defaultcmdPrefix}");

                        await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                        await RemoveAllButOneEmote(digits, helpMessage, 6);
                        timer = fulltimer;
                        break;
                    }



                }
                #endregion


                if (helpMessage.Reactions.GetValueOrDefault(back).ReactionCount > 1)
                {
                    foreach (var sender in await helpMessage.GetReactionUsersAsync(back, 10).FlattenAsync())
                        await helpMessage.RemoveReactionAsync(back, sender);
                    goto Start;
                }




                if (timer <= 0)
                {
                    looping = -1;

                    break;
                }
                timer--;

                await Task.Delay(500);



            }


            while (true)
            {
                await helpMessage.UpdateAsync();
                for (int i = 1; i < helpMessage.Reactions.Count; i++)
                    digitCounts[i] = helpMessage.Reactions.GetValueOrDefault(digits[i]).ReactionCount;


                for (int i = 0; i < digitCounts.Length; i++)
                {
                    if (digitCounts[i] > 1)
                    {
                        goto End;
                    }
                }
                if (helpMessage.Reactions.GetValueOrDefault(back).ReactionCount > 1)
                {
                    foreach (var sender in await helpMessage.GetReactionUsersAsync(back, 10).FlattenAsync())
                    {
                        if (sender.Id != Context.Client.CurrentUser.Id)
                            await helpMessage.RemoveReactionAsync(back, sender);
                    }
                    goto Middle;
                }




                if (timer <= 0)
                {
                    looping = -1;

                    break;
                }
                timer--;

                await Task.Delay(500);
            }


            //end
            await helpMessage.RemoveAllReactionsAsync();

        }

        private async Task RemoveAllButOneEmote(Emoji[] digits, RestUserMessage helpMessage, int digit)
        {
            foreach (var sender in await helpMessage.GetReactionUsersAsync(digits[digit], 20).FlattenAsync())
            {
                if (sender.Id != Context.Client.CurrentUser.Id)
                    await helpMessage.RemoveReactionAsync(digits[digit], sender);
            }
        }

    }
}

















//Depreciated
/*
            builder = new EmbedBuilder();
            builder = menusystem.CurrentMenuEmbedBuilder(builder);

            helpMessage = await Context.Channel.SendMessageAsync("", false, builder.Build());

            Emoji[] digits = new Emoji[] {
                new Emoji("\U00000030\U000020e3"), new Emoji("\U00000031\U000020e3"),
                new Emoji("\U00000032\U000020e3"), new Emoji("\U00000033\U000020e3"),
                new Emoji("\U00000034\U000020e3"), new Emoji("\U00000035\U000020e3")
            , new Emoji("\U00000036\U000020e3"), new Emoji("\U00000037\U000020e3")
            , new Emoji("\U00000038\U000020e3"), new Emoji("\U00000039\U000020e3")};
            int[] digitCounts = new int[digits.Length];
            Emoji back = new Emoji("🔄");

            await helpMessage.UpdateAsync();

            if (!helpMessage.Reactions.ContainsKey(digits[1]))
                await helpMessage.AddReactionAsync(digits[1]);
            if (!helpMessage.Reactions.ContainsKey(digits[2]))
                await helpMessage.AddReactionAsync(digits[2]);
            if (!helpMessage.Reactions.ContainsKey(digits[3]))
                await helpMessage.AddReactionAsync(digits[3]);
            if (!helpMessage.Reactions.ContainsKey(digits[4]))
                await helpMessage.AddReactionAsync(digits[4]);




            menusystem.ReturnCurrentMenu(out string menutitle, out string[][] menufields, out int menufieldcount);
            int maxtimer = 50;

            int timer = maxtimer;
            string oldCurrentMenu = menusystem.CurrentMenu;
            while (true)
            {
                int delaytime = 500;
                await helpMessage.UpdateAsync();
                for (int i = 1; i < helpMessage.Reactions.Count; i++)
                    digitCounts[i] = helpMessage.Reactions.GetValueOrDefault(digits[i]).ReactionCount;


                var channelmessages = await Context.Channel.GetMessagesAsync(5).FlattenAsync();

                //If a channel message contains a number and the menu field which that number is
                //Does not have a menuGoto that is null set the menu to the menuGoto of the field
                foreach (var message in channelmessages)
                {
                    for (int i = 1; i < menufieldcount + 1; i++)
                        if (message.Content == (i.ToString()) && !string.IsNullOrWhiteSpace(menufields[i - 1][0]))
                        {
                            menusystem.SetCurrentMenu(menufields[i - 1][0]);
                            await message.DeleteAsync();
                        }
                    if (message.Content == ("0"))
                    {
                        foreach (var sender in await helpMessage.GetReactionUsersAsync(back, 30).FlattenAsync())
                            await helpMessage.RemoveReactionAsync(back, sender);

                        string previousMenu = menusystem.GetPreviousMenu();
                        if (previousMenu != null)
                            menusystem.SetCurrentMenu(previousMenu);
                        await message.DeleteAsync();
                    }
                }

                //for every menu field if the emoji digit is bigger then 1
                //and the menuGoto string is not null (the string that tells you what menu the field sends you to)
                //Set the menu to the menuGoto of the field
                for (int i = 1; i < menufieldcount + 1; i++)
                    if (digitCounts[i] > 1 && !string.IsNullOrWhiteSpace(menufields[i -1][0]))
                        menusystem.SetCurrentMenu(menufields[i - 1][0]);


                if (helpMessage.Reactions.GetValueOrDefault(back).ReactionCount > 1)
                {
                    foreach (var sender in await helpMessage.GetReactionUsersAsync(back, 30).FlattenAsync())
                        await helpMessage.RemoveReactionAsync(back, sender);

                    string previousMenu = menusystem.GetPreviousMenu();
                    if(previousMenu != null)
                        menusystem.SetCurrentMenu(previousMenu);
                }


                if (oldCurrentMenu != menusystem.CurrentMenu)
                {
                    menusystem.ReturnCurrentMenu(out menutitle, out menufields, out menufieldcount);

                    builder = new EmbedBuilder();
                    builder.WithTitle(menutitle);
                    for (int i = 0; i < menufieldcount; i++)
                        builder.AddField(menufields[i][1], menufields[i][2], false);

                    await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                    timer = maxtimer;//Reset timer

                    for(int i = 1; i < menufieldcount + 1; i++)
                    {
                        if (!helpMessage.Reactions.ContainsKey(digits[i]))
                            await helpMessage.AddReactionAsync(digits[i]);
                        else if (digitCounts[i] > 1)
                            await RemoveAllButOneEmote(digits, helpMessage, i);
                    }

                    if (menusystem.CurrentMenu != "Help")
                        await helpMessage.AddReactionAsync(back);

                    oldCurrentMenu = menusystem.CurrentMenu;
                }
                

                if (timer <= 0)
                    break;
                else if (timer - maxtimer / 4 <= 0)
                    delaytime = (int)(delaytime / ((float)timer / maxtimer));
                else if (timer - maxtimer / 2 <= 0)
                    delaytime *= 2;

                timer--;
                await Task.Delay(delaytime);
            }
            */
