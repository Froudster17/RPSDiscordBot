using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RPSDiscordBot.commands
{
    internal class Commands : BaseCommandModule
    {

        private static readonly string[] Options = { "rock", "paper", "scissors" };
        private static readonly string[] Emojis = { "🪨", "📄", "✂️" };

        [Command("play")]
        public async Task PlayCommand(CommandContext ctx)
        {
            // Send the initial message with buttons
            var message = await SendButtonMessage(ctx.Client, ctx.Channel);

            // Wait for button interaction
            var interactivity = ctx.Client.GetInteractivity();
            var result = await interactivity.WaitForButtonAsync(message, TimeSpan.FromSeconds(5));

            // Handle interaction result
            if (result.TimedOut)
            {
                await ctx.Channel.SendMessageAsync("No selection was made in time. Please try again.");
                return;
            }

            // Extract user's choice and proceed
            var userChoice = result.Result.Id;
            await HandleUserChoice(ctx, userChoice);
        }

        private async Task<DiscordMessage> SendButtonMessage(DiscordClient client, DiscordChannel channel)
        {
            // Create embed message with buttons
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Rock, Paper, Scissors",
                Description = "React with 🪨 for Rock, 📄 for Paper, or ✂️ for Scissors.",
                Color = DiscordColor.Red,
                Timestamp = DateTimeOffset.UtcNow
            };

            var rockButton = new DiscordButtonComponent(ButtonStyle.Primary, "rock", "🪨 Rock");
            var paperButton = new DiscordButtonComponent(ButtonStyle.Primary, "paper", "📄 Paper");
            var scissorsButton = new DiscordButtonComponent(ButtonStyle.Primary, "scissors", "✂️ Scissors");

            var builder = new DiscordMessageBuilder()
                .WithEmbed(embed)
                .AddComponents(rockButton, paperButton, scissorsButton);

            // Send message with buttons
            return await channel.SendMessageAsync(builder);
        }

        private async Task HandleUserChoice(CommandContext ctx, string userChoice)
        {
            // Generate bot's choice
            var botChoice = Options[new Random().Next(Options.Length)];
            var botEmoji = Emojis[Array.IndexOf(Options, botChoice)];

            // Determine result
            var resultText = DetermineResult(userChoice, botChoice);

            // Construct result message
            var resultEmbed = new DiscordEmbedBuilder()
            {
                Title = "Rock, Paper, Scissors - Result",
                Description = $"You chose {Emojis[Array.IndexOf(Options, userChoice)]}, I chose {botEmoji}.\n\n**{resultText}**",
                Color = (resultText == "You Win!" ? DiscordColor.Green : (resultText == "It's a Draw!" ? DiscordColor.Orange : DiscordColor.Red))
            };

            // Send result message
            await ctx.Channel.SendMessageAsync(embed: resultEmbed);
        }

        private string DetermineResult(string userChoice, string botChoice)
        {
            if (userChoice == botChoice)
                return "It's a Draw!";

            if ((userChoice == "rock" && botChoice == "scissors") ||
                (userChoice == "paper" && botChoice == "rock") ||
                (userChoice == "scissors" && botChoice == "paper"))
                return "You Win!";

            return "You Lose!";
        }

    }
}
