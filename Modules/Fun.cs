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
