using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.ComponentModel;
using StardewUI.Framework;
using StardewUI;

namespace TextInput
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private IViewEngine? viewEngine;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
            viewEngine.RegisterViews("Mods/TestMod/Views", "assets/views"); // Adjust the path as needed
            viewEngine.EnableHotReloading();
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // Check if the player is free and the F1 key is pressed
            if (Context.IsPlayerFree && e.Button == SButton.F1)
            {
                //ShowTextInputExample();
                Game1.activeClickableMenu = viewEngine.CreateMenuFromAsset(
                    "Mods/TestMod/Views/TextInput");
                // print button presses to the console window
                this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
            }

        }

        private void ShowTextInputExample()
        {
            var viewModel = new TextInputTestViewModel
            {
                Text = "Type your question here"
            };

            Game1.activeClickableMenu = viewEngine.CreateMenuFromAsset(
                "Mods/focustense.StardewUITest/Views/TextInput",
                viewModel);
        }
    }

    // ViewModel for the TextInput
    public partial class TextInputTestViewModel : INotifyPropertyChanged
    {
        private string text = "";

        public string Text
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
