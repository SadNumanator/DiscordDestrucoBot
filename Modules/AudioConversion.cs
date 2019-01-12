using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.Net;
using Discord.WebSocket;
using Discord.Webhook;
using System.Net;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace DiscordDestrucoBot.Modules
{
    ///Process.Start("oggenc2.exe", "wa.wav");
    public class AudioConversion : ModuleBase<SocketCommandContext>
    {
        [Command("convert", RunMode = RunMode.Async), RequireUserPermission(GuildPermission.Administrator)]
        public async Task AudioConversationAsync(string strquality = "4", string outputFileType = "ogg")
        {

            string attachmentID = Context.Message.Attachments.FirstOrDefault().Id.ToString();
            Directory.CreateDirectory(attachmentID);

            //allow the arguments to come in any order by checking which one is a number and assigning them accordingly
            if (!int.TryParse(strquality, out int quality))
            {
                outputFileType = strquality;
                if (!int.TryParse(outputFileType, out quality))
                {
                    if (outputFileType == "mp3")
                        quality = 128;
                    else
                        quality = 4;
                }
                }

            if (outputFileType != "mp3" &&(quality < -1 || quality > 10))
            {
                await ReplyAsync("The quality setting must be between -1 and 10 (if you were planning to make it a ogg file)");
                if (outputFileType != "ogg")
                    await ReplyAsync("Not that it matters since the output file isn't a ogg file, but i felt like it should mention it anyway");
                return;
            }
            if(Context.Message.Attachments.Count == 0)
            {
                await ReplyAsync("The message must have an audio file attached"); return;
            }
            switch (outputFileType)
            {
                case "ogg":
                case "wav":
                case "flac":
                case "mp3":

                    break;
                default:
                    await ReplyAsync("The output filetype you specified is either not implemented yet, or it doesn't exist");
                    return;
                    
            }



            int index;
            int counter;
            long fileLength;
            string _url = Context.Message.Attachments.FirstOrDefault().Url;
            string fileName = attachmentID + "/" + Context.Message.Attachments.FirstOrDefault().Filename;
            index = fileName.LastIndexOf(".");
            string fileType = fileName.Substring(index + 1).ToLowerInvariant();

            if(fileType == "png"|| fileType == "jpg")
            { await ReplyAsync("That isn't a audio file ..."); return;  }
      

            RestUserMessage message = await Context.Channel.SendMessageAsync($"Recived \"{fileName}\" with the filetype being {fileType}\n**Downloading**");

            //Create the WebClient that will download the file
            WebClient Client = new WebClient();
            //Download the file with the assigned random file name
            await Client.DownloadFileTaskAsync(_url, fileName);
            string outputFile;
            

            //If this is a module file convert it to wav/flac
            if (fileType == "xm"|| fileType == "it"|| fileType == "mod" || fileType == "s3m" || fileType == "mptm" || fileType == "stm" || fileType == "nst")
            {
                await message.ModifyAsync(msg => msg.Content = $"Recived \"{fileName}\" with the filetype being {fileType}\n**Converting module file to wav**");
                outputFile = ConvertStringFileTypeTo__(fileName, "wav");
                Process.Start("AudioStuff/openmpt123.exe", "--quiet --output " + outputFile + " " + fileName);
                await Task.Delay(1000);

                counter = 0;
                do
                {
                    if (counter == 25)
                    {
                        await ReplyAsync("Error `<MODULE TO WAV FAILED TO CONVERT |TIMEOUT|>`");
                        return;
                    }
                    counter++;
                    fileLength = new FileInfo(outputFile).Length;//Get the size of the file
                    await Task.Delay(1000);
                }
                while (fileLength != new FileInfo(outputFile).Length);
                //Delete the UNCONVERTED file
                File.Delete(fileName);

                fileName = outputFile;
            }

            outputFile = ConvertStringFileTypeTo__(fileName, outputFileType);

            //if (outputFileType != "wav")
            //{
                //In case the filetype is the same as the output file type add a ^ to the output file name to prevent them from overwriting each other
                if (fileType == outputFileType)
                    outputFile = outputFile.Insert(outputFile.LastIndexOf('.'), "!");

                    await message.ModifyAsync(msg => msg.Content = $"Recived \"{fileName}\" with the filetype being {fileType}\n**Converting to " + outputFileType + "**");

                //Convert the assigned random file name
                Process.Start("AudioStuff/sox.exe", $"{fileName} -C {quality} {outputFile}");
                await Task.Delay(1000);



                //This whole thing here waits for the conversion to finish by every half a second checking if the file size has changed
                //If the file size has not changed that means the file is done convertinng
                counter = 0;
                do
                {
                    if (counter == 60)
                    {
                        await ReplyAsync("Error `<AUDIO TO OGG FAILED TO CONVERT |TIMEOUT|>`");
                        return;
                    }
                    counter++;
                    fileLength = new FileInfo(outputFile).Length;//Get the size of the file
                    await Task.Delay(1000);
                }
                while (fileLength != new FileInfo(outputFile).Length);
                //Delete the UNCONVERTED file
                File.Delete(fileName);
          //  }







            if (File.Exists(outputFile))
            {
                await message.ModifyAsync(msg => msg.Content = $"Recived \"{fileName}\" with the filetype being {fileType}\n**Sending the message**");
                if (new FileInfo(outputFile).Length <= 8388608)
                    await Context.Channel.SendFileAsync(outputFile);
                else
                    await ReplyAsync("Reduce the quality, the ogg file is above 8 mb and the bot is unable to send it \nTo reduce the quality(or raise it) you put a number between -1 and 10 as the argument. \nThe lower it is the lower quality it will be, the higher it is the more quality you will have.");

                await message.DeleteAsync();
                File.Delete(outputFile);
                Directory.Delete(attachmentID);
            }
            else
            {
                await ReplyAsync("UnspecifiedError");
            }


            //await ReplyAsync("Passed");
        }

        private static string ConvertStringFileTypeTo__(string fileName, string filetype)
        {
            //Remove the file extension at the end
            int index = fileName.LastIndexOf(".");
            if (index > 0)
                fileName = fileName.Substring(0, index);
            //Add the .ogg file extension to the name
            return fileName += '.' + filetype;
        }
    }
}
