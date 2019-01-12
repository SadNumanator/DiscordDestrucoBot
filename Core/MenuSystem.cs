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
using System.Drawing;

namespace DiscordDestrucoBot
{
    public class MenuSystem
    {

        private readonly Dictionary<string, string[]> Menus = new Dictionary<string, string[]>();
        private readonly Dictionary<string, string[]> MenuFields = new Dictionary<string, string[]>();
        private readonly Dictionary<string, int> menuLevelCount = new Dictionary<string, int>();
        public string CurrentMenu { get; private set; } = null;
        public string StartingMenu { get; private set; } = null;
        private int maxtimer;
        private int constantDelayTime;


        public MenuSystem(int _maxtimer = 50, int _constantdelaytime = 500)
        {
            maxtimer = _maxtimer;
            constantDelayTime = _constantdelaytime;
        }

        public void AddMenu(string menutitle, string previousMenu = null, bool keepPreviousMenuOptions = false, System.Drawing.Color? color = null)
        {//Adds the menu title to the Dictionary of menus as the value and title.
            if (Menus.ContainsKey(menutitle))
                CustomError($"Exception in AddMenu(); the menu already contains : {menutitle} the title you are adding");

            if (color == null)
                color = System.Drawing.Color.FromArgb(49, 79, 79);
            Menus.Add(menutitle, new string[] { previousMenu, (keepPreviousMenuOptions ? 1 : 0).ToString(), color.ToString() });
            if (StartingMenu == null)
            {
                CurrentMenu = menutitle;
                StartingMenu = menutitle;
            }
        }//Don't ask me why i made it a Dictionary ...



        public void AddFieldInMenu(string parent, string menuToOpen, string title, string description)
        {
            //Get the amount of levels in specified menu "parent", if it has none make it equal none.
            if (!menuLevelCount.TryGetValue(parent, out int menufieldcount))
                menufieldcount = 0;


            if (MenuFields.ContainsKey(parent + menufieldcount))
                CustomError("The menu already contains a key of " + parent + menufieldcount);

            //First is the menu it opens, Second is the title, Third is the description.
            MenuFields.Add(parent + menufieldcount, new string[] { menuToOpen, '\u200B' + title, '\u200B' + description });

            //Keep track of how many fields that are added in the specified menu "parent"
            menuLevelCount[parent] = ++menufieldcount;
        }


        //Returns the current menus, title, fields, and field count
        public void ReturnCurrentMenu(out string menutitle, out string[][] menufields, out int menufieldcount)
        {
            if (!Menus.ContainsKey(CurrentMenu))
                CustomError($"Tried to get menu key {CurrentMenu} which did not exist");

            //Get the amount of fields in the current menu
            if (!menuLevelCount.TryGetValue(CurrentMenu, out menufieldcount))
                menufieldcount = 0;

            //Create the menu fields as a jagged array (think of it as a list of all the menufield arrays).
            menufields = new string[menufieldcount][];

            //Add every fields array of values seperately into the menufields list to be exported out. 
            for (int i = 0; i < menufieldcount; i++)
            {
                MenuFields.TryGetValue(CurrentMenu + i, out string[] menufield);
                menufields[i] = menufield;
            }
            menutitle = CurrentMenu;
        }

        //Gets the amount of fields in the current menu
        public int CurrentMenuFieldCount()
        {
            if (!Menus.ContainsKey(CurrentMenu))
                CustomError($"Tried to get menu key {CurrentMenu} which did not exist");

            //Get the amount of fields in the current menu
            if (!menuLevelCount.TryGetValue(CurrentMenu, out int menufieldcount))
                menufieldcount = 0;

            return menufieldcount;
        }

        //This will set the current menu to the string to provide
        public void SetCurrentMenu(string menutitle)
        {
            if (Menus.ContainsKey(menutitle))
                CurrentMenu = menutitle;
            else
                CustomError($"In SetCurrentMenu() the menu : {menutitle} does not exist");
        }

        //This will set the current menu to the string to provide
        public string GetPreviousMenu()
        {
            Menus.TryGetValue(CurrentMenu, out string[] menuData);
            return menuData[0];
        }
        public bool GetKeepPreviousMenu()
        {
            Menus.TryGetValue(CurrentMenu, out string[] menuData);
            return menuData[1] == "1";
        }
        public int[] GetCurrentMenuColor()
        {
            Menus.TryGetValue(CurrentMenu, out string[] menuData);

            var p = menuData[2].Split(new char[] { ',', ']' });

            return new int[] { Convert.ToInt32(p[1].Substring(p[1].IndexOf('=') + 1)),
            Convert.ToInt32(p[2].Substring(p[2].IndexOf('=') + 1)),
            Convert.ToInt32(p[3].Substring(p[3].IndexOf('=') + 1))
            };
        }


        public EmbedBuilder CurrentMenuEmbedBuilder(EmbedBuilder builder, out int menufieldcount)
        {//Get the values for the current menu
            ReturnCurrentMenu(out string menutitle, out string[][] menufields, out menufieldcount);
            //Add the title and color to the builder
            int[] rgb = GetCurrentMenuColor();
            builder.WithTitle(menutitle).WithColor(rgb[0], rgb[1], rgb[2]);
            //for each field in menufields add it as a field
            for (int i = 0; i < menufieldcount; i++)
                builder.AddField(menufields[i][1], menufields[i][2], false);

            return builder;
        }



        public async Task DoCurrentMenuStuff(SocketCommandContext Context)
        {   //Initialize the EmbedBuilder
            EmbedBuilder builder = new EmbedBuilder();
            //Create the embed
            builder = CurrentMenuEmbedBuilder(builder, out int menufieldcount);
            //Send the message
            RestUserMessage helpMessage = await Context.Channel.SendMessageAsync("", false, builder.Build());


            //Add all the emojis and make a array that will later keep track of the amount of each emoji
            Emoji[] digits = new Emoji[] {
                new Emoji("\U00000030\U000020e3"), new Emoji("\U00000031\U000020e3"),
                new Emoji("\U00000032\U000020e3"), new Emoji("\U00000033\U000020e3"),
                new Emoji("\U00000034\U000020e3"), new Emoji("\U00000035\U000020e3")
            , new Emoji("\U00000036\U000020e3"), new Emoji("\U00000037\U000020e3")
            , new Emoji("\U00000038\U000020e3"), new Emoji("\U00000039\U000020e3")};
            int[] digitCounts = new int[digits.Length];
            Emoji back = new Emoji("🔄");


            await helpMessage.UpdateAsync();

            ReturnCurrentMenu(out string menutitle, out string[][] menufields, out menufieldcount);

            //For every field in the CurrentMenu add a digit
            for (int i = 1; i < menufieldcount + 1; i++)
            {
                //if(!string.IsNullOrEmpty(menufields[i][0]))
                if (!helpMessage.Reactions.ContainsKey(digits[i]))
                    await helpMessage.AddReactionAsync(digits[i]);
            }



            int timer = maxtimer;
            //For checking if the menu has changed
            string oldCurrentMenu = CurrentMenu;

            string[] gotoFields = new string[menufieldcount];
            for (int i = 0; i < menufieldcount; i++)
                gotoFields[i] = menufields[i][0];

            while (true)
            {
                int delaytime = constantDelayTime;//The update time. The bigger it is the slower this updates

                await helpMessage.UpdateAsync();//Update the message for most importantly the amount of reactions on it
                //Keep track of all the reaction counts in a array
                for (int i = 1; i < helpMessage.Reactions.Count; i++)
                    digitCounts[i] = helpMessage.Reactions.GetValueOrDefault(digits[i]).ReactionCount;
                /*
                //Collect 5 channel messages and put them in a list
                var channelmessages = await Context.Channel.GetMessagesAsync(5).FlattenAsync();
                //For every channel message that we got ^
                foreach (var message in channelmessages)
                {
                    //If a message contains a number and that number is a menu field
                    //if the fields menuGoto is not empty CurrentMenu = menuGoto of the field, then delete the message.
                    for (int i = 1; i < menufieldcount + 1; i++)
                        if (message.Content == (i.ToString()) && !string.IsNullOrWhiteSpace(menufields[i - 1][0]))
                        {
                            SetCurrentMenu(menufields[i - 1][0]);
                            await message.DeleteAsync();
                        }
                    //If the 0 key was sent remove the message, remove any additional reactions, and set CurrentMenu to the previous menu
                    if (message.Content == ("0"))
                    {
                        await message.DeleteAsync();
                        foreach (var sender in await helpMessage.GetReactionUsersAsync(back, 30).FlattenAsync())
                            await helpMessage.RemoveReactionAsync(back, sender);

                        string previousMenu = GetPreviousMenu();
                        if (previousMenu != null)
                            SetCurrentMenu(previousMenu);
                    }
                }
                */

                //for every menu field if the emoji digit is bigger then 1
                //and the menuGoto string is not null (the string that tells you what menu the field sends you to)
                //Set the CurrentMenu to the menuGoto of the field
                for (int i = 1; i < menufieldcount + 1; i++)
                    if (digitCounts[i] > 1 && !string.IsNullOrWhiteSpace(gotoFields[i - 1]))
                        SetCurrentMenu(gotoFields[i - 1]);


                //If the back reaction count is more then 1 reset it back to 1 and assign the previous menu as the CurrentMenu
                if (helpMessage.Reactions.GetValueOrDefault(back).ReactionCount > 1)
                {
                    foreach (var sender in await helpMessage.GetReactionUsersAsync(back, 20).FlattenAsync())
                    {
                        if (sender.Id != Context.Client.CurrentUser.Id)
                            await helpMessage.RemoveReactionAsync(back, sender);
                    }

                    string previousMenu = GetPreviousMenu();
                    if (previousMenu != null)
                        SetCurrentMenu(previousMenu);
                }


                //Assign the new menu
                if (oldCurrentMenu != CurrentMenu)
                {
                    bool keepPrevious = GetKeepPreviousMenu();

                    //Update the local menu information
                    ReturnCurrentMenu(out menutitle, out menufields, out int tempmenufieldcount);

                    //Create the embedbuilder
                    builder = new EmbedBuilder();
                    CurrentMenuEmbedBuilder(builder, out tempmenufieldcount);
                    //Edit the message to show the new embed
                    await helpMessage.ModifyAsync(msg => msg.Embed = builder.Build());
                    timer = maxtimer;//Reset the timer

                    //If we don't keep the previous menu options update to the current menu options
                    //This is here so you can set it to false and it will keep the options from the previous menu.
                    if (!keepPrevious)
                    {
                        menufieldcount = tempmenufieldcount; 

                        gotoFields = new string[menufieldcount];
                        for (int i = 0; i < menufieldcount; i++)
                            gotoFields[i] = menufields[i][0];
                    }

                    //For each reaction make the reaction count 1 and or add missing reactions
                    for (int i = 1; i < menufieldcount + 1; i++)
                    {
                        if (!helpMessage.Reactions.ContainsKey(digits[i]))
                            await helpMessage.AddReactionAsync(digits[i]);

                        if (digitCounts[i] > 1)
                            foreach (var sender in await helpMessage.GetReactionUsersAsync(digits[i], 20).FlattenAsync())
                            {
                                if (sender.Id != Context.Client.CurrentUser.Id)
                                    await helpMessage.RemoveReactionAsync(digits[i], sender);
                            }
                    }

                    //If it doesn't equal the menu we started on add the back button so you can go back
                    if (CurrentMenu == StartingMenu)
                        foreach (var sender in await helpMessage.GetReactionUsersAsync(back, 30).FlattenAsync())
                            await helpMessage.RemoveReactionAsync(back, sender);
                    else if(helpMessage.Reactions.GetValueOrDefault(back).ReactionCount == 0)
                        await helpMessage.AddReactionAsync(back);


                    //Make sure this process doesn't happen anymore then needed
                    oldCurrentMenu = CurrentMenu;
                }

                //if the timer is 0 stop, if the timer is less then half, half the update rate, if its less then 1/4th, lower the update rate by a lot more
                if (timer <= 0)
                    break;
                else if (timer - maxtimer / 4 <= 0)
                    delaytime *= 4;
                //delaytime = (int)(delaytime / ((float)timer / maxtimer));
                else if (timer - maxtimer / 2 <= 0)
                    delaytime *= 2;

                //One less before independing doom
                timer--;
                await Task.Delay(delaytime);
            }


            await helpMessage.RemoveAllReactionsAsync();
        }




        private static void CustomError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ForegroundColor = ConsoleColor.Gray; Console.Title = "Error Encountered";
            throw new Exception();
        }
    }
}