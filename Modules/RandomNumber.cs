using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDestrucoBot.Modules
{
    public class RandomNumber : ModuleBase<SocketCommandContext>
    { 

        [Command("randomnumber")]
        public async Task RandomNumberAsync(string value1 = null)//This is a optional paramater that you type after !ping awawawawa
        {//Remainder puts all the things into one string

            if(value1 == null || !value1.All(char.IsDigit))
            {
                await ReplyAsync("The value must be a **number** and be above 0"); return;
            }

            long number = 0;

            try {
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
            if(number > 8000000000054775806)
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
            long result = rand.Next((Int32)(min >> 32), (Int32)(max+1 >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((Int32)min, (Int32)max+1);
            return result;
        }
    }
}
