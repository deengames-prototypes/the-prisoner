    using DeenGames.ThePrisoner.Model;
using System.Collections.Generic;
using System;
using System.Text;
using RadiantWrench.Engine.View;
using RadiantWrench.Engine.Controller;

namespace DeenGames.ThePrisoner.Screens
{
    public class SummaryScreen : Screen
    {
        private const int TEXT_TITLE_OFFSET = 8;
        private const int TITLE_SIZE = 24;
        private const int TITLE_Y = 250;
        private const int GAME_TIME_OFFSET = 50;
        private const int CHOICES_TITLE_OFFSET = 50;
        private const int CHOICES_OFFSET = 32;
        private const int CONTENT_INDENTATION_OFFSET = 32;
        private const int BUTTONS_Y = 450;

        public override void Initialize()
        {
            base.Initialize();

            this.FadeOutImmediately();

            this.AddSprite("summary");

            Text title = this.AddText("Game Summary");
            title.FontSize = TITLE_SIZE;
            title.Font = Font.GetFont("Fonts/Orbitron");
            this.AddSprite("scenes/default-header");//.PositionFromTopLeftCorner(0, 0);
            title.X = TEXT_TITLE_OFFSET;

            TimeSpan gameTime = DateTime.Now - CoreModel.Instance.StartTime;
            StringBuilder builder = new StringBuilder();
            builder.Append("Game time: ");

            if (Math.Floor(gameTime.TotalHours) > 0)
            {
                builder.Append(gameTime.TotalHours).Append(" hours, ");
            }
            builder.Append(gameTime.Minutes).Append(" minutes, ").Append(gameTime.Seconds).Append(" seconds.");

            Text gameTimeText = this.AddText(builder.ToString());
            gameTimeText.FontSize = TITLE_SIZE - 4;
            gameTimeText.X = CONTENT_INDENTATION_OFFSET;
            gameTimeText.Y = title.Y + GAME_TIME_OFFSET;

            Text choicesLabel = this.AddText("Choices Taken");
            choicesLabel.X = TEXT_TITLE_OFFSET;
            choicesLabel.Font = Font.GetFont("Fonts/Orbitron");
            choicesLabel.FontSize = TITLE_SIZE;
            choicesLabel.Y = gameTimeText.Y + CHOICES_TITLE_OFFSET;
            Sprite choicesBackground = this.AddSprite("scenes/default-header");
            choicesBackground.Y = choicesLabel.Y + (choicesBackground.Height / 2);

            Text choicesTakenText = this.AddText("");
            IList<string> choicesTaken = CoreModel.Instance.ChoicesTaken;

            for (int i = 0; i < choicesTaken.Count; i++)
            {
                choicesTakenText.Content += (i + 1) + ") " + choicesTaken[i] + "\n";
            }
            choicesTakenText.Y = choicesLabel.Y + choicesLabel.Height + CHOICES_OFFSET;
            choicesTakenText.X = CONTENT_INDENTATION_OFFSET;

            //choicesTakenText.VerticalAlignment = VerticalAlignment.Top;

            Button startOverButton = this.AddButton("Start Over");
            startOverButton.Skin = "gold";
            startOverButton.FontSize = 18;

            startOverButton.X = ((this.Width - startOverButton.Width) / 2) - 75;
            startOverButton.Y = BUTTONS_Y;
            startOverButton.Click += () =>
            {
                this.FadeOut();
                this.FadeOutComplete += () =>
                {
                    ScreenController.ShowScreen(new CoreGameScreen());
                };
            };

            Button mainMenuButton = this.AddButton("Main Menu");
            mainMenuButton.Skin = "Gold";
            mainMenuButton.FontSize = 18;


            mainMenuButton.X = startOverButton.X + 175;
            mainMenuButton.Y = BUTTONS_Y;
            mainMenuButton.Click += () =>
            {
                this.FadeOut();
                this.FadeOutComplete += () =>
                {
                    ScreenController.ShowScreen(new TitleScreen());
                };
            };

            this.FadeIn();
        }
    }
}
