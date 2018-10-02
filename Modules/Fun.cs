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
    [Group]
    public class Fun : ModuleBase<SocketCommandContext>
    {

        #region Pictures
        [Command("cat")]
        public async Task RndCatAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();//Initializes the embed builder

            builder.WithImageUrl(WebPageLinkFinder("https://api.thecatapi.com/v1/images/search?"));

            await ReplyAsync("", false, builder.Build());//Print the embed
        }

        [Command("dog")]
        public async Task RndDogAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();//Initializes the embed builder

            builder.WithImageUrl(WebPageLinkFinder("https://api.thedogapi.com/v1/images/search?"));

            await ReplyAsync("", false, builder.Build());//Print the embed
        }

        [Command("bird")]
        [Alias("birb")]
        public async Task RndBirdAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            string id = GetWebPageText("http://random.birb.pw/tweet/");

            builder.WithImageUrl("https://random.birb.pw/img/" + id);

            await ReplyAsync("", false, builder.Build());

        }


        #endregion

        #region 2048Game

        [Command("2048", RunMode = RunMode.Async)][RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Game2048Async(int _xvalue = 4,int _yvalue = 4)
        {
            if(_xvalue > 19 || _yvalue > 19)
            {
                await ReplyAsync("The x or y values can not go over 19");
                    return;
            }

            int maxX = _xvalue;
            int maxY = _yvalue;
            int timer;
            int maxtimer = 70;

            RestUserMessage clientMessage;
            StringBuilder stringMessage = new StringBuilder();

            int[,] gameArray = new int[maxX, maxY];

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    gameArray[x, y] = 0;
                }
            }
            int fullamount;
            PlaceRandom2048Value(maxX, maxY, gameArray);
            Update2048String(maxX, maxY, stringMessage, gameArray, out fullamount);

            

            
            clientMessage = await Context.Channel.SendMessageAsync($"Score : {fullamount}" + stringMessage.ToString());


            //Emojis
            Emoji leftarrow = new Emoji("\u2B05");
            Emoji rightarrow = new Emoji("\u27A1");
            Emoji downarrow = new Emoji("\u2B07");
            Emoji uparrow = new Emoji("\u2B06");
        
            await clientMessage.UpdateAsync();

            await clientMessage.AddReactionAsync(downarrow);
            await clientMessage.AddReactionAsync(uparrow);
            await clientMessage.AddReactionAsync(leftarrow);
            await clientMessage.AddReactionAsync(rightarrow);

            if (CheckForLoss2048(maxX, maxY, gameArray) == true)
            {
                await clientMessage.ModifyAsync(msg => msg.Content = stringMessage.ToString() + "**Game Over**\nYou Lose with a score of : " + fullamount + "\nand before you even had your first turn :(");
                return;
            }

            try
            {
                timer = maxtimer;
                while (true)
                {
                    await clientMessage.UpdateAsync();

                    int delaytime = 500;

                    if (clientMessage.Reactions.GetValueOrDefault(downarrow).ReactionCount > 1)
                    {
                        bool somethingMoved = false;
                        for (int x = 0; x < maxX; x++)
                        {
                            for (int y = maxY - 1; y >= 0; y--)
                            {
                                if (gameArray[x, y] != 0)
                                {
                                    int ypoint = y;
                                    while (ypoint + 1 < maxY)
                                    {
                                        if (gameArray[x, ypoint + 1] == gameArray[x, ypoint])
                                        {
                                            gameArray[x, ypoint + 1] += gameArray[x, ypoint];
                                            gameArray[x, ypoint] = 0;
                                            somethingMoved = true;
                                            break;
                                        }
                                        else if (gameArray[x, ypoint + 1] == 0)
                                        {
                                            gameArray[x, ypoint + 1] = gameArray[x, ypoint];
                                            gameArray[x, ypoint] = 0;
                                            somethingMoved = true;
                                            ypoint++;
                                        }
                                        else
                                            break;
                                    }
                                }
                            }
                        }
                        if (somethingMoved == false)
                        {
                            await RemoveAllButOneEmote(downarrow, clientMessage);
                            continue;
                        }

                        PlaceRandom2048Value(maxX, maxY, gameArray);

                        Update2048String(maxX, maxY, stringMessage, gameArray, out fullamount);

                        await clientMessage.ModifyAsync(msg => msg.Content = $"Score : {fullamount}" + stringMessage.ToString());
                        await RemoveAllButOneEmote(downarrow, clientMessage);
                        if (CheckForLoss2048(maxX, maxY, gameArray) == true)
                        {
                            await clientMessage.ModifyAsync(msg => msg.Content = stringMessage.ToString() + "**Game Over**\nYou Lose with a score of : " + fullamount);
                            break;
                        }
                        timer = maxtimer;
                    }

                    else if (clientMessage.Reactions.GetValueOrDefault(uparrow).ReactionCount > 1)
                    {
                        bool somethingMoved = false;
                        for (int x = 0; x < maxX; x++)
                        {
                            for (int y = 0; y < maxY; y++)
                            {
                                if (gameArray[x, y] != 0)
                                {
                                    int ypoint = y;
                                    while (ypoint - 1 >= 0)
                                    {
                                        if (gameArray[x, ypoint - 1] == gameArray[x, ypoint])
                                        {
                                            gameArray[x, ypoint - 1] += gameArray[x, ypoint];
                                            gameArray[x, ypoint] = 0;
                                            somethingMoved = true;
                                            break;
                                        }
                                        else if (gameArray[x, ypoint - 1] == 0)
                                        {
                                            gameArray[x, ypoint - 1] = gameArray[x, ypoint];
                                            gameArray[x, ypoint] = 0;
                                            somethingMoved = true;
                                            ypoint--;
                                        }
                                        else
                                            break;
                                    }
                                }
                            }
                        }
                        if (somethingMoved == false)
                        {
                            await RemoveAllButOneEmote(uparrow, clientMessage);
                            continue;
                        }


                        PlaceRandom2048Value(maxX, maxY, gameArray);

                        Update2048String(maxX, maxY, stringMessage, gameArray, out fullamount);

                        await clientMessage.ModifyAsync(msg => msg.Content = $"Score : {fullamount}" + stringMessage.ToString());
                        await RemoveAllButOneEmote(uparrow, clientMessage);
                        if (CheckForLoss2048(maxX, maxY, gameArray) == true)
                        {
                            await clientMessage.ModifyAsync(msg => msg.Content = stringMessage.ToString() + "**Game Over**\nYou Lose with a score of : " + fullamount);
                            break;
                        }
                        timer = maxtimer;
                    }

                    else if (clientMessage.Reactions.GetValueOrDefault(leftarrow).ReactionCount > 1)
                    {
                        bool somethingMoved = false;
                        for (int y = 0; y < maxY; y++)
                        {
                            for (int x = 0; x < maxX; x++)
                            {
                                if (gameArray[x, y] != 0)
                                {
                                    int xpoint = x;
                                    while (xpoint - 1 >= 0)
                                    {
                                        if (gameArray[xpoint - 1, y] == gameArray[xpoint, y])
                                        {
                                            gameArray[xpoint - 1, y] += gameArray[xpoint, y];
                                            gameArray[xpoint, y] = 0;
                                            somethingMoved = true;
                                            break;
                                        }
                                        else if (gameArray[xpoint - 1, y] == 0)
                                        {
                                            gameArray[xpoint - 1, y] = gameArray[xpoint, y];
                                            gameArray[xpoint, y] = 0;
                                            somethingMoved = true;
                                            xpoint--;
                                        }
                                        else
                                            break;
                                    }
                                }
                            }
                        }

                        if (somethingMoved == false)
                        {
                            await RemoveAllButOneEmote(leftarrow, clientMessage);
                            continue;
                        }

                        PlaceRandom2048Value(maxX, maxY, gameArray);

                        Update2048String(maxX, maxY, stringMessage, gameArray, out fullamount);

                        await clientMessage.ModifyAsync(msg => msg.Content = $"Score : {fullamount}" + stringMessage.ToString());
                        await RemoveAllButOneEmote(leftarrow, clientMessage);
                        if (CheckForLoss2048(maxX, maxY, gameArray) == true)
                        {
                            await clientMessage.ModifyAsync(msg => msg.Content = stringMessage.ToString() + "**Game Over**\nYou Lose with a score of : " + fullamount);
                            break;
                        }
                        timer = maxtimer;
                    }

                    else if (clientMessage.Reactions.GetValueOrDefault(rightarrow).ReactionCount > 1)
                    {
                        bool somethingMoved = false;
                        for (int y = 0; y < maxY; y++)
                        {
                            for (int x = maxX - 1; x >= 0; x--)
                            {
                                if (gameArray[x, y] != 0)
                                {
                                    int xpoint = x;
                                    while (xpoint + 1 < maxX)
                                    {
                                        if (gameArray[xpoint + 1, y] == gameArray[xpoint, y])
                                        {
                                            gameArray[xpoint + 1, y] += gameArray[xpoint, y];
                                            gameArray[xpoint, y] = 0;
                                            somethingMoved = true;
                                            break;
                                        }
                                        else if (gameArray[xpoint + 1, y] == 0)
                                        {
                                            gameArray[xpoint + 1, y] = gameArray[xpoint, y];
                                            gameArray[xpoint, y] = 0;
                                            somethingMoved = true;
                                            xpoint++;
                                        }
                                        else
                                            break;
                                    }
                                }
                            }
                        }

                        if (somethingMoved == false)
                        {
                            await RemoveAllButOneEmote(rightarrow, clientMessage);
                            continue;
                        }

                        PlaceRandom2048Value(maxX, maxY, gameArray);

                        Update2048String(maxX, maxY, stringMessage, gameArray, out fullamount);

                        await clientMessage.ModifyAsync(msg => msg.Content = $"Score : {fullamount}" + stringMessage.ToString());
                        await RemoveAllButOneEmote(rightarrow, clientMessage);
                        if (CheckForLoss2048(maxX, maxY, gameArray) == true)
                        {
                            await clientMessage.ModifyAsync(msg => msg.Content = stringMessage.ToString() + "**Game Over**\nYou Lose with a score of : " + fullamount);
                            break;
                        }
                        timer = maxtimer;
                    }


                    if (timer <= 0)
                    {
                        break;
                    }
                    else if (timer - maxtimer / 4 <= 0)
                    {
                        delaytime *= 5;
                    }
                    else if (timer - maxtimer / 2 <= 0)
                    {
                        delaytime *= 2;
                    }
                    timer--;
                    await Task.Delay(delaytime);
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message + "\nshow numan this");
            }
            await clientMessage.RemoveAllReactionsAsync();
        }

        private static bool CheckForLoss2048(int maxX, int maxY, int[,] gameArray)
        {
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (gameArray[x, y] != 0)
                    {
                        if (y + 1 < maxY)
                        {
                            if (gameArray[x, y + 1] == gameArray[x, y])
                            {
                                return false;
                            }
                            else if (gameArray[x, y + 1] == 0)
                            {
                                return false;
                            }
                        }
                        if (y - 1 >= 0)
                        {
                            if (gameArray[x, y - 1] == gameArray[x, y])
                            {
                                return false;
                            }
                            else if (gameArray[x, y - 1] == 0)
                            {
                                return false;
                            }
                        }
                        if (x + 1 < maxX)
                        {
                            if (gameArray[x + 1, y] == gameArray[x, y])
                            {
                                return false;
                            }
                            else if (gameArray[x + 1, y] == 0)
                            {
                                return false;
                            }
                        }
                        if (x - 1 >= 0)
                        {
                            if (gameArray[x - 1, y] == gameArray[x, y])
                            {
                                return false;
                            }
                            else if (gameArray[x - 1, y] == 0)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static int[,] PlaceRandom2048Value(int maxX, int maxY, int[,] gameArray)
        {
            int count = 0;
            while (count < Math.Ceiling(((maxX * maxY)/ 16d)))
            {
                int rndX;
                int rndY;
                do
                {
                    rndX = Program.rnd.Next(maxX);
                    rndY = Program.rnd.Next(maxY);
                }
                while (gameArray[rndX, rndY] != 0);
                int rndValue;
                if (Program.rnd.Next(2) == 0)
                    rndValue = 2;
                else
                    rndValue = 4;
                gameArray[rndX, rndY] = rndValue;
                count++;
            }
            return gameArray;
        }

        private static StringBuilder Update2048String(int maxX, int maxY, StringBuilder stringMessage, int[,] gameArray, out int fullamount)
        {
            fullamount = Get2048FullAmount(maxX, maxY, gameArray);
            stringMessage.Clear();
            string color = "";
            if (fullamount >= 1000)
            {
                string[] colors = (new string[] { "swift\n.", "fix\n.", "ini\n[" });
                color = colors[Program.rnd.Next(0, colors.Length)];
            }
                stringMessage.AppendLine("```" + color);
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    stringMessage.Append("|" + gameArray[x, y].ToString().PadRight(4));
                }
                stringMessage.AppendLine();
            }
            stringMessage.Append("```");
            return stringMessage;
        }
        private static int Get2048FullAmount(int maxX, int maxY, int[,] gameArray)
        {
            int fullamount = 0;
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    fullamount += gameArray[x, y];
                }
            }
            return fullamount;
        }

        #endregion

        #region RandomPicking

        [Command("coinflip")]
        [Alias("flipcoin", "coin flip", "flip coin")]
        public async Task CoinFlip()
        {
            int coinflipnum = Program.rnd.Next(1001);

            if (coinflipnum > 500)
                await ReplyAsync("**Tails**");
            else if (coinflipnum < 500)
                await ReplyAsync("**Heads**");
            else
                await ReplyAsync("**Edge**");


        }


        [Command("randomnumber")]
        public async Task RandomNumberAsync(string value1 = null)//This is a optional paramater that you type after !ping awawawawa
        {//Remainder puts all the things into one string

            if (value1 == null || !value1.All(char.IsDigit))
            {
                await ReplyAsync("The value must be a **number** and be above 0"); return;
            }

            long number = 0;

            try
            {
                number = long.Parse(value1);
            }
            catch (Exception ex)
            {
                if (ex is OverflowException)
                    await ReplyAsync("Why would you ever need a number this big");
                else
                    await ReplyAsync("this is a error, please report this to TheSadNumanator#1662");
                return;
            }
            if (number > 8000000000054775806)
            {
                await ReplyAsync("The value is too much"); return;
            }

            number = LongRandom(0, number, Program.rnd);

            await ReplyAsync(number.ToString());
        }

        [Command("randomnumber")]
        public async Task RandomNumberAsync(string value1, string value2)//This is a optional paramater that you type after !ping awawawawa
        {//Remainder puts all the things into one string

            if (!value1.All(char.IsDigit) || !value2.All(char.IsDigit))
            {
                await ReplyAsync("Both values must be a **number** and be above 0"); return;
            }


            long maxnumber = 0;
            long minnumber = 0;
            try
            {
                minnumber = long.Parse(value1);
            }
            catch (Exception ex)
            {
                if (ex is OverflowException)
                    await ReplyAsync("Why would you ever need a number this big");
                else
                    await ReplyAsync("this is a error, please report this to TheSadNumanator#1662");
                return;
            }
            try
            {
                maxnumber = long.Parse(value2);
            }
            catch (Exception ex)
            {
                if (ex is OverflowException)
                    await ReplyAsync("Why would you ever need a number this big");
                else
                    await ReplyAsync("this is a error, please report this to TheSadNumanator#1662");
                return;
            }
            if (maxnumber > 8000000000054775806)
            {
                await ReplyAsync("The max value is too much"); return;
            }

            maxnumber = LongRandom(minnumber, maxnumber, Program.rnd);

            await ReplyAsync(maxnumber.ToString());
        }


        [Command("pick")]
        [Alias("choose")]
        public async Task PickBetweenAsync(params string[] options)
        {
            int picked = Program.rnd.Next(options.Count());

            await ReplyAsync(options[picked]);
        }

        long LongRandom(long min, long max, Random rand)
        {
            long result = rand.Next((Int32)(min >> 32), (Int32)(max + 1 >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((Int32)min, (Int32)max + 1);
            return result;
        }
        #endregion



        private static string WebPageLinkFinder(string webpage)
        {
            string text;
            text = GetWebPageText(webpage);
            int _texthttps = text.IndexOf("http");//Gets where the http is

            //The image url is the first letter of https and the right before the " after that only getting the link
            text = text.Substring(_texthttps, text.IndexOf('"', _texthttps) - _texthttps);
            return text;
        }

        private static string GetWebPageText(string webpage)
        {
            string text;
            WebClient web = new WebClient();

            System.IO.Stream stream = web.OpenRead(webpage);//The webpage that generates cat pictures

            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();//Reads the whole webpage
            }

            return text;
        }

        private byte[] GetImage(string iconPath)
        {
            using (WebClient client = new WebClient())
            {
                byte[] pic = client.DownloadData(iconPath);
                //string checkPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +@"\1.png";
                //File.WriteAllBytes(checkPath, pic);
                return pic;
            }
        }


        private async Task RemoveAllButOneEmote(Emoji emote, RestUserMessage message)
        {
            foreach (var sender in await message.GetReactionUsersAsync(emote, 20).FlattenAsync())
            {
                if (sender.Id != Context.Client.CurrentUser.Id)
                    await message.RemoveReactionAsync(emote, sender);
            }
        }
    }
}
