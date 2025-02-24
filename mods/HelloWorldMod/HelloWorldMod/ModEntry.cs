using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace HelloWorldMod
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
         ** Public methods
         *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Adds an 'event handler' when the button-pressed event happens.
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
         ** Private methods
         *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // Check if the right mouse button was pressed
            if (e.Button == SButton.MouseRight)
            {
                // Generate a random number
                Random random = new Random();
                int randomNumber = random.Next(1, 101); // Random number between 1 and 100

                // Display the random number in the HUD messages
                Game1.hudMessages.Add(new HUDMessage($"Your random number is {randomNumber}", HUDMessage.achievement_type));
            }

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }
    }
}