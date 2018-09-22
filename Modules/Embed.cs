using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDestrucoBot.Modules
{
    public class Embed : ModuleBase<SocketCommandContext>
    {
        [Command("embed")]
        public async Task PingAsync(string stringColor = "", string _title = "",[Remainder] string _text = "")
        {
            stringColor = stringColor.ToLowerInvariant();
            Color color;

            if (stringColor == "")
            { color = Color.LighterGrey; }
            else if (stringColor == "red")
                color = Color.Red;
            else if (stringColor == "blue")
                color = Color.Blue;
            else if (stringColor == "green")
                color = Color.Green;
            else if (stringColor == "orange")
                color = Color.Orange;
            else if (stringColor == "purple")
                color = Color.Purple;
            else if (stringColor == "magenta")
                color = Color.Magenta;
            else if (stringColor == "gold")
                color = Color.Gold;
            else if (stringColor == "teal")
                color = Color.Teal;
            else if (stringColor == "darkgrey")
                color = Color.DarkerGrey;
            else
            {
                color = Color.LighterGrey;
            }

            EmbedBuilder builder = new EmbedBuilder();//Make the embed builder that makes the embed

            builder.WithTitle(_title)
                .WithColor(color)
                .WithDescription($"{_text}");


                await ReplyAsync("", false, builder.Build());//This here makes the bot print the embed
        }
    }
}
