using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

public class AudioService
{
    private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

    public async Task JoinAudio(IGuild guild, IVoiceChannel target)
    {
        IAudioClient client;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
        {
            return;
        }
        if (target.Guild.Id != guild.Id)
        {
            return;
        }
        try
        {
            var audioClient = await target.ConnectAsync();
            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                // If you add a method to log happenings from this service,
                // you can uncomment these commented lines to make use of that.
                //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        
        
    }

    public async Task LeaveAudio(IGuild guild)
    {
        IAudioClient client;
        if (ConnectedChannels.TryRemove(guild.Id, out client))
        {
            await client.StopAsync();
            //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
        }
    }

    public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
    {
        if (!File.Exists("ffmpeg.exe"))
        {
            await channel.SendMessageAsync("This feature is only enabled for the test build of this bot"); return;
        }
        // Your task: Get a full path to the file if the value of 'path' is only a filename.
        if (!File.Exists(path))
        { 
            await channel.SendMessageAsync("File does not exist.");
            return;
        }
        IAudioClient client;
        try
        {
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                using (var ffmpeg = CreateProcess(path))
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }
        catch(Exception ex)
        {
            await channel.SendMessageAsync("Error : " + ex);
        }
    }

    private Process CreateProcess(string path)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg.exe",
            Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true
        });
    }
}