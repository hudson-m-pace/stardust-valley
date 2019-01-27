using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace StardustValley
{
    public class ModEntry : Mod, IAssetEditor
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) // Return if player hasn't loaded a save yet.
            {
                return;
            }
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}."); // Print button press to the console.
            if (e.Button == SButton.Space)
            {
                if (Game1.player.currentLocation is StardewValley.Locations.FarmHouse)
                {
                    this.Monitor.Log($"{Game1.player.Name} is in the farmhouse. Adding item to inventory.");
                    Game1.player.addItemToInventory((Item)new StardewValley.Object(900, 1));
                }
            }
            if (e.Button == SButton.Q)
            {
                Game1.player.craftingRecipes.Add("Lorem Ipsum", 0);
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                return true;
            }
            else if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
                return true;
            }
            return false;
        }
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                data.Add(900, $"Roli's First Item/0/-300/Basic {StardewValley.Object.metalResources}/Roli's First Item/My first item.");
            }
            else if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data.Add("Lorem Ipsum", "900 5/Home/335/false/1 0");
            }
        }
    }
}
