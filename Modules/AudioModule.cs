using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

public class AudioModule : ModuleBase<ICommandContext>
{
    private readonly AudioService _service;

    public AudioModule(AudioService service)
    {
        _service = service;
    }

    // You *MUST* mark these commands with 'RunMode.Async'
    // otherwise the bot will not respond until the Task times out.
    [Command("join", RunMode = RunMode.Async)]
    public async Task JoinCmd(IVoiceChannel channel = null)
    {
        if (!File.Exists("ffmpeg.exe"))
        {
            await Context.Channel.SendMessageAsync("This feature is only enabled for the test build of this bot"); return;
        }
        await LeaveCmd();
        await _service.JoinAudio(Context.Guild, channel ?? (Context.User as IVoiceState).VoiceChannel);
    }

    // Remiander, add preconditions to these commands.
    [Command("leave", RunMode = RunMode.Async)]
    public async Task LeaveCmd()
    {
        await _service.LeaveAudio(Context.Guild);
    }

    [Command("play", RunMode = RunMode.Async)]
    public async Task PlayCmd([Remainder] string song)
    {

        await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
    }
}