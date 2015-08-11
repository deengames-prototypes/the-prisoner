using System;
using RadiantWrench.Engine.View;
using RadiantWrench.Engine.Controller;

namespace DeenGames.ThePrisoner.Screens
{
    public class TitleScreen : Screen
    {
        public override void Initialize()
        {
            base.Initialize();

            this.FadeOutImmediately();

            this.AddSprite("titlescreen");

            Button newGame = this.AddButton("Start");
            newGame.Skin = "Gold";

            newGame.FontSize = 18;

            newGame.Click += () =>
            {
                this.FadeOut();
                
                this.FadeOutComplete += () =>
                {
                    ScreenController.ShowScreen(new CoreGameScreen());
                };
                 
            };

            newGame.X = (800 - newGame.Width) / 2;
            newGame.Y = ((600 - newGame.Height) / 2) + 165;

            this.FadeIn();
        }
    }
}
