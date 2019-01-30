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
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveCreated;
            //helper.Events.GameLoop.GameLaunched += this.Initialize;
        }

        private void OnSaveCreated(object sender, SaveLoadedEventArgs e) // Changes the player right after the save is created.
        {
            Game1.player.craftingRecipes.Add("Lorem Ipsum", 0);
            Game1.player.craftingRecipes.Add("Free Copper", 0);
            Game1.player.craftingRecipes.Add("Free Iron", 0);
            Game1.player.craftingRecipes.Add("Coke Oven", 0);
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
            else if (asset.AssetNameEquals("Data/BigCraftablesInformation"))
            {
                return true;
            }
            else if (asset.AssetNameEquals("TileSheets/Craftables"))
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
                data.Add(912, $"Roli's First Item/0/-300/Basic {StardewValley.Object.metalResources}/Roli's First Item/My first item.");
                data.Add(913, $"Black & Blue Circle/0/-300/Basic {StardewValley.Object.metalResources}/Roli's Second Item/My second item.");
                data.Add(914, $"Coke/15/-300/Basic -15/Coke/A fuel with few impurities high carbon content. Now we're cookin'.");
            }
            else if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data.Add("Lorem Ipsum", $"{912} 5/Home/335/false/1 0");
                data.Add("Free Copper", "335 0/Home/334/false/1 0");
                data.Add("Free Iron", "335 0/Home/335/false/1 0");
                data.Add("Coke Oven", "335 10/Home/210/true/1 2");
            }
            else if (asset.AssetNameEquals("Maps/springobjects"))
            {
                Texture2D spriteSheet = asset.AsImage().Data;
                //Texture2D newTexture = Helper.Content.Load<Texture2D>("assets/test.png", ContentSource.ModFolder); // file extension?
                Texture2D newTexture2 = Helper.Content.Load<Texture2D>("assets/test2.png", ContentSource.ModFolder);
                Texture2D newTexture3 = Helper.Content.Load<Texture2D>("assets/test3.png", ContentSource.ModFolder);
                Texture2D cokeTexture = Helper.Content.Load<Texture2D>("assets/coke.png", ContentSource.ModFolder);

                //spriteSheet = AddTexture(spriteSheet, newTexture, 335);
                spriteSheet = AddTexture(spriteSheet, newTexture2, 912);
                spriteSheet = AddTexture(spriteSheet, newTexture3, 913);
                spriteSheet = AddTexture(spriteSheet, cokeTexture, 914);
                asset.ReplaceWith(spriteSheet);
            }
            else if (asset.AssetNameEquals("Data/BigCraftablesInformation"))
            {
                // name/price/edibility/type & category/description/can be set outdoors/indoors/fragility/is lamp
                // Furnace/50/-300/Crafting -9/Turns ore and coal into metal bars./true/true/0

                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                data.Add(210, "Coke Oven/50/-300/Crafting -9/Turns coal into coke through destructive distillation./true/true/0");

            }
            else if (asset.AssetNameEquals("TileSheets/Craftables"))
            {
                Texture2D spriteSheet = asset.AsImage().Data;
                Texture2D cokeOvenTexture = Helper.Content.Load<Texture2D>("assets/cokeOven.png");
                this.Monitor.Log("yo");
                spriteSheet = AddTexture(spriteSheet, cokeOvenTexture, 210);
                asset.ReplaceWith(spriteSheet);
            }
        }

        private Texture2D AddTexture(Texture2D spriteSheet, Texture2D newTexture, int sheetIndex)
        {
            Rectangle oldTextureRect = Game1.getSourceRectForStandardTileSheet(spriteSheet, sheetIndex, newTexture.Width, newTexture.Height);
            Color[] spriteSheetData = new Color[spriteSheet.Width * spriteSheet.Height];
            Color[] newTextureData = new Color[newTexture.Width * newTexture.Height];
            spriteSheet.GetData<Color>(spriteSheetData);
            newTexture.GetData<Color>(newTextureData);

            int spriteSheetHeight = spriteSheet.Height;
            while (spriteSheetHeight * spriteSheet.Width / (newTexture.Width * newTexture.Height) <= sheetIndex)
            {
                spriteSheetHeight+= newTexture.Height;
            }
            if (spriteSheetHeight != spriteSheet.Height)
            {
                Color[] temp = new Color[spriteSheet.Width * spriteSheetHeight];
                spriteSheetData.CopyTo(temp, 0);
                spriteSheetData = temp;
            }


            int i = 0;
            for (int j = 0; j < oldTextureRect.Height; j++)
            {
                for (int k = 0; k < oldTextureRect.Width; k++)
                {
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

/*
[20:02:56 ERROR game] An error occured in the base update loop: System.FormatException: String was not recognized as a valid Boolean.
   at System.Boolean.Parse(String value)
   at StardewValley.Object..ctor(Vector2 tileLocation, Int32 parentSheetIndex, Boolean isRecipe) in C:\Users\gitlab-runner\gitlab-runner\builds\5c0f9387\0\chucklefish\stardewvalley\Farmer\Farmer\Objects\Object.cs:line 402
   at StardewValley.CraftingRecipe.createItem() in C:\Users\gitlab-runner\gitlab-runner\builds\5c0f9387\0\chucklefish\stardewvalley\Farmer\Farmer\CraftingRecipe.cs:line 115
   at StardewValley.Menus.CraftingPage.performHoverAction(Int32 x, Int32 y) in C:\Users\gitlab-runner\gitlab-runner\builds\5c0f9387\0\chucklefish\stardewvalley\Farmer\Farmer\Menus\CraftingPage.cs:line 433
   at StardewValley.Menus.GameMenu.performHoverAction(Int32 x, Int32 y) in C:\Users\gitlab-runner\gitlab-runner\builds\5c0f9387\0\chucklefish\stardewvalley\Farmer\Farmer\Menus\GameMenu.cs:line 236
   at StardewValley.Game1.updateActiveMenu(GameTime gameTime) in C:\Users\gitlab-runner\gitlab-runner\builds\5c0f9387\0\chucklefish\stardewvalley\Farmer\Farmer\Game1.cs:line 2785
   at StardewValley.Game1._update(GameTime gameTime) in C:\Users\gitlab-runner\gitlab-runner\builds\5c0f9387\0\chucklefish\stardewvalley\Farmer\Farmer\Game1.cs:line 2370
   at StardewModdingAPI.Framework.SGame.Update(GameTime gameTime) in C:\source\_Stardew\SMAPI\src\SMAPI\Framework\SGame.cs:line 900
   */