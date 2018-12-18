using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace WizardOfPaws
{
    public class Core : ModuleBase<SocketCommandContext>
    {
        readonly Random rnd = new Random();
        Color RandomColor() => new Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));

        [Command("cat")]
        [Alias("kat", "kitten", "kitty", "meow", "purr", "pussy")]
        public async Task Cat(string idc = "")
        {
            dynamic json = JsonConvert.DeserializeObject(new StreamReader(WebRequest.Create("http://aws.random.cat/meow").GetResponse().GetResponseStream()).ReadToEnd());
            string link = (string)(json.file);
            await Context.Channel.SendMessageAsync("", false, new EmbedBuilder() { Title = char.ConvertFromUtf32(0x1F431), Color = RandomColor(), ImageUrl = link }.Build());
        }

        [Command("dog")]
        [Alias("bark", "bitch", "bork", "boye", "doggie", "doggo", "doggy", "pupper", "puppy", "ruff", "woof")]
        public async Task Dog(string idc = "")
        {
            dynamic json = JsonConvert.DeserializeObject(new StreamReader(WebRequest.Create("https://dog.ceo/api/breeds/image/random").GetResponse().GetResponseStream()).ReadToEnd());
            string link = (string)(json.message);
            await Context.Channel.SendMessageAsync("", false, new EmbedBuilder() { Title = char.ConvertFromUtf32(0x1F436), Color = RandomColor(), ImageUrl = link }.Build());
        }

        [Group("role")]
        public class ConfigModule : ModuleBase<SocketCommandContext>
        {
            [Command("")]
            [Alias("", "")]
            public async Task Role()
            {
                string response = Context.User.Mention;
                await Context.Channel.SendMessageAsync(response);
            }
        }
    }
}