using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordDestrucoBot.Modules
{
    public class Unlisted : ModuleBase<SocketCommandContext>
    {
        [Command("hello")]
        [Alias("hi")]
        public async Task SayHelloAsync()
        {
            string[] helloarray = new string[] { "Heya", "Hi", "Hello","Howdy","Howdydo","Hey","Hiya","Hail",
                "Aloha","G'day","Erro!","Ahoy","Heeey","Ello","Greetings","Shalom","Bonjour","Ciao","Buongiorno",
                "*Salutes Plurimas Dico*","How’s tricks?","Breaker, breaker",
                "Salutations","Sup","Yo","Hullo","Wassup","Hallo","Halloo!","Yello","Heyo","Gidday" };
            await ReplyAsync(helloarray[Program.rnd.Next(helloarray.Length)]);
        }
        
    }
}
