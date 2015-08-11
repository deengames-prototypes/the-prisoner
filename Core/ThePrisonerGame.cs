using RadiantWrench.Engine.Core;
using RadiantWrench.Engine.Controller;
using DeenGames.ThePrisoner.Screens;
using RadiantWrench.Engine.View;

namespace DeenGames.ThePrisoner
{
    public class ThePrisonerGame : Game
    {
        protected override void Initialize()
        {
            base.Initialize();
            this.Name = "Engine Test";
            Font.LoadEmbeddedFont("Fonts/orbitron-medium.otf", "Orbitron");

            ScreenController.ShowScreen(new DeenGamesScreen());
        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
