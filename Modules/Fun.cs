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




        #region RandomPicking
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
    }
}
