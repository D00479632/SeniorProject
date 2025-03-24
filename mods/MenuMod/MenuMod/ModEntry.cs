using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewUI.Framework;
using StardewValley;

namespace MenuMod
{
    internal sealed class ModEntry : Mod
    {
        private IViewEngine? viewEngine;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
            viewEngine.RegisterViews("Mods/TestMod/Views", "assets/views");
            viewEngine.EnableHotReloading();
        }

        private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (Context.IsPlayerFree && e.Button == SButton.F8)
            {
                var context = MenuData.Edibles();
                Game1.activeClickableMenu = viewEngine.CreateMenuFromAsset(
                    "Mods/TestMod/Views/ScrollingItemGrid",
                    context);
            }
        }
    }
}