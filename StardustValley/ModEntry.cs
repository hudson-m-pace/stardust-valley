using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace StardustValley
{
    public class ModEntry : Mod, IAssetEditor
    {
        private Texture2D testTexture;
        const int startingID = 912;
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveCreated;
            //helper.Events.GameLoop.GameLaunched += this.Initialize;
        }

        private void OnSaveCreated(object sender, SaveLoadedEventArgs e) // Changes the player right after the save is created.
        {
            Game1.player.craftingRecipes.Add("Lorem Ipsum", 0);
            this.Monitor.Log("Save Created.");
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
                    Game1.player.addItemToInventory((Item)new StardewValley.Object(912, 1));
                }
            }
        }
        /*private void Initialize(object sender, GameLaunchedEventArgs e)
        {
            


            ChangeTexture(testTexture, 335);
        }

        private void ChangeTexture(Texture2D newTexture, int index)
        {
            
        }*/

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
            else if (asset.AssetNameEquals("Maps/springobjects"))
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
                data.Add(startingID, $"Roli's First Item/0/-300/Basic {StardewValley.Object.metalResources}/Roli's First Item/My first item.");
            }
            else if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data.Add("Lorem Ipsum", $"{startingID} 5/Home/335/false/1 0");
            }
            else if (asset.AssetNameEquals("Maps/springobjects"))
            {
                Texture2D data = asset.AsImage().Data;
                Texture2D newTexture = Helper.Content.Load<Texture2D>("assets/test.png", ContentSource.ModFolder); // file extension?
                Color[] newTextureData = new Color[newTexture.Width * newTexture.Height];
                newTexture.GetData<Color>(newTextureData);
                Rectangle rect = Game1.getSourceRectForStandardTileSheet(data, 335, 16, 16);
                Color[] tempData = new Color[data.Width * data.Height];
                data.GetData<Color>(tempData);
                int i = 0;
                for (int j = 0; j < rect.Height; j++)
                {
                    for (int k = 0; k < rect.Width; k++)
                    {
                        tempData[((rect.Y + j) * data.Width) + rect.X + k] = newTextureData[i];
                       // this.Monitor.Log($"Changed pixel at {rect.X + k}, {rect.Y + j}");
                        i++;
                    }
                }
                data.SetData<Color>(tempData);
            }
        }
    }
}
