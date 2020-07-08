﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BDF.Lib.Entities;
using Discord;
using Discord.Commands;

namespace BDF.Bot.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        [Alias("about")]
        public async Task InfoAsync()
        {
            var app = await Context.Client.GetApplicationInfoAsync();

            await ReplyAsync(
                $"{Format.Bold("Info")}\n" +
                $"- Author: {app.Owner} ({app.Owner.Id})\n" +
                $"- Library: Discord.Net ({DiscordConfig.Version})\n" +
                $"- Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture} " +
                $"({RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture})\n" +
                $"- Uptime: {GetUptime()}\n\n" +

                $"{Format.Bold("Stats")}\n" +
                $"- Heap Size: {GetHeapSize()}MiB\n" +
                $"- Guilds: {Context.Client.Guilds.Count}\n" +
                $"- Channels: {Context.Client.Guilds.Sum(g => g.Channels.Count)}\n" +
                $"- Users: {Context.Client.Guilds.Sum(g => g.Users.Count)}\n");
        }

        [Command("help")]
        [Alias("commands")]
        public async Task HelpAsync()
        {
            await ReplyAsync(
                $"{Format.Bold("Available Commands:")}\n" +
                $"- !info: Displays bot info.\n" +
                $"- !help: Lists available commands.\n" +
                $"- !audio: Lists audio commands.\n" +
                $"- !image: Lists image commands.\n" +
                $"- !echo {Format.Code("msg")}: Makes the bot say something.\n" +
                $"- !mc {Format.Code("ip")}: Queries a Minecraft server.\n" +
                $"- !ping: Test latency.\n" +
                $"- !clean {Format.Code("amount")}: Deletes the specified amount of messages." +
                $"\n {Format.Quote($"{Format.Code("?")} available as alternative prefix.")}");
        }

        [Command("audio")]
        [Alias("help audio", "sound", "help sound")]
        public async Task HelpAudioAsync()
        {
            await ReplyAsync(
                $"{Format.Bold("Available Music Commands:")}\n" +
                $"- !play {Format.Code("url")}: Plays song from given link.\n" +
                $"- !disconnect: Disconnects the bot from the voice channel.\n" +
                $"- !Connect: Connects the bot to the voice channel.\n" +
                $"- !position: Shows elapsed track time.\n" +
                $"- !skip: Skips the current song in queue and plays the next.\n" +
                $"- !queue: Lists all songs in queue.\n" +
                $"- !stop: Stops playback.\n" +
                $"- !volume {Format.Code("percentage")}: Sets the playback volume (0 - 200).");
        }

        [Command("image")]
        [Alias("help image", "picture", "help picture")]
        public async Task HelpImageAsync()
        {
            await ReplyAsync(
                $"{Format.Bold("Available Image Commands:")}\n" +
                $"- !r34 {Format.Code("query")}: Returns a r34 image based on query.\n" +
                $"- !anime {Format.Code("query")}: Returns a anime image based on query.\n" +
                $"- !cat: Returns cat picture.");
        }

        [Command("minecraft")]
        [Alias("mc")]
        public async Task MinecraftInfoAsync([Remainder] string url)
        {
            var query = await QueryMc(url);
            
            if (!query.online) { 
                await ReplyAsync($"{query.hostname} is currently offline!");
                return;
            }

            await ReplyAsync(
                $"{Format.Bold($"{query.hostname} is online:")}\n" +
                $"- IP: {query.ip}:{query.port}\n" +
                $"- Players: {query.players.online}/{query.players.max}\n" +
                $"- Version: {query.version} ({query.software})\n" +
                $"- Plugins: {query.plugins.names.Count()}");
        }

        private static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
        private async Task<McQuery> QueryMc(string url)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://api.mcsrvstat.us/2/{url}");
            var result = await response.Content.ReadAsStringAsync();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<McQuery>(result);

        }
    }
}