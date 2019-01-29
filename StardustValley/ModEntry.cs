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
                Texture2D spriteSheet = asset.AsImage().Data;
                Texture2D newTexture = Helper.Content.Load<Texture2D>("assets/test.png", ContentSource.ModFolder); // file extension?
                Texture2D newTexture2 = Helper.Content.Load<Texture2D>("assets/test2.png", ContentSource.ModFolder);

                spriteSheet = SwapTexture(spriteSheet, newTexture, 335);
                spriteSheet = SwapTexture(spriteSheet, newTexture2, 912);
                asset.ReplaceWith(spriteSheet);
            }
        }

        private Texture2D SwapTexture(Texture2D spriteSheet, Texture2D newTexture, int sheetIndex)
        {
            Rectangle oldTextureRect = Game1.getSourceRectForStandardTileSheet(spriteSheet, sheetIndex, 16, 16);
            Color[] spriteSheetData = new Color[spriteSheet.Width * spriteSheet.Height];
            Color[] newTextureData = new Color[newTexture.Width * newTexture.Height];
            spriteSheet.GetData<Color>(spriteSheetData);
            newTexture.GetData<Color>(newTextureData);

            int spriteSheetHeight = spriteSheet.Height;
            while (spriteSheetHeight * spriteSheet.Width / (16 * 16) <= sheetIndex)
            {
                spriteSheetHeight+= 16;
            }
            if (spriteSheetHeight != spriteSheet.Height)
            {
                Color[] temp = new Color[spriteSheet.Width * spriteSheetHeight];
                spriteSheetData.CopyTo(temp, 0);
                this.Monitor.Log($"{temp.Length}, {spriteSheetData.Length}");
                spriteSheetData = temp;
                this.Monitor.Log($"{spriteSheetData.Length}");
            }


            int i = 0;
            for (int j = 0; j < oldTextureRect.Height; j++)
            {
                for (int k = 0; k < oldTextureRect.Width; k++)
                {
                    this.Monitor.Log($"{((oldTextureRect.Y + j) * spriteSheet.Width) + oldTextureRect.X + k}");
                    spriteSheetData[((oldTextureRect.Y + j) * spriteSheet.Width) + oldTextureRect.X + k] = newTextureData[i];
                    i++;
                }
            }
            Texture2D tempe = new Texture2D(Game1.graphics.GraphicsDevice, spriteSheet.Width, spriteSheetHeight);
            tempe.SetData<Color>(spriteSheetData);
            return tempe;
        }
    }
}
