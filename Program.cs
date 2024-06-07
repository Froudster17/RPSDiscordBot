using DSharpPlus.CommandsNext;
using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPSDiscordBot.config;
using RPSDiscordBot.commands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace RPSDiscordBot
{
    internal class Program
    {
        public static DiscordClient DiscordClient { get; set; }
        private static CommandsNextExtension CommandsNextExtension { get; set; }

        static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,              
            };

            DiscordClient = new DiscordClient(discordConfig);

            DiscordClient.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromSeconds(30),
            });

            DiscordClient.Ready += DiscordClient_Ready;
            DiscordClient.ComponentInteractionCreated += DiscordClient_ComponentInteractionCreated;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = true,     
            };

            CommandsNextExtension = DiscordClient.UseCommandsNext(commandsConfig);

            CommandsNextExtension.RegisterCommands<Commands>();

            await DiscordClient.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task DiscordClient_ComponentInteractionCreated(DiscordClient sender, DSharpPlus.EventArgs.ComponentInteractionCreateEventArgs args)
        {
            // Send immediate acknowledgment (pong)
            await args.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

            // Extract interaction data
            var customId = args.Interaction.Data.CustomId;
            var userId = args.Interaction.User.Id;

            // Handle the interaction based on the custom ID
            switch (customId)
            {
                case "rock":
                case "paper":
                case "scissors":
                    // Handle button click
                    // Example: Log the button click or perform additional actions
                    Console.WriteLine($"Button clicked: {customId} by user {userId}");
                    break;
                default:
                    break;
            }
        }

        private static Task DiscordClient_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
