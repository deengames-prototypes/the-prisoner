using DeenGames.ThePrisoner.Model;
using DeenGames.ThePrisoner.Helpers;
using System.Collections.Generic;
using System;
using CSharpCity.Helpers;
using RadiantWrench.Engine.View;
using RadiantWrench.Engine.Controller;
using System.Text;
using DeenGames.Utils.Silverlight;

namespace DeenGames.ThePrisoner.Screens
{
    public class CoreGameScreen : Screen
    {
        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 600;

        private const int TITLE_POSITION_Y = 0;
        private const int TITLE_SIZE = 24;

        private const int CHOICES_TITLE_POSITION_Y = 500;
        private const int CHOICES_BUTTONS_OFFSET = 8;

        private const int NUM_CHOICES_PER_ROW = 4;
        private const int TEXT_TITLE_OFFSET = 8;
        private const int CONTENT_FADE_RATE = 2;

        private const int TEXT_PANEL_PADDING = 16;

        private string[] _contentPanels;
        private int _currentPanel = 1;

        private float CHOICES_BUTTON_FADE_RATE = 2;
        private const int PADDING_BETWEEN_BUTTONS = 16;
        private int TEXT_PANEL_READABLE_AREA_WIDTH;

        private Sprite _background;
        private Sprite _titleHeader;
        private Sprite _choicesHeader;
        private Text _nodeTitle;
        private Text _choicesTitle;
        private Text _contentText;

        private IList<Button> _choiceButtons = new List<Button>();
        private IList<string> _choicesTaken = new List<string>();

        private Sprite _textPanel;

        private Button _theEndButton = null;

        private Node _nextNode = null;
        private Node _currentNode = null;


        public override void Initialize()
        {
            base.Initialize();  

            this.FadeOutImmediately();
            _choicesTaken.Clear();

            string content = Resources.Nodes;

            Node startNode = NodeHelper.GetStartNode(content);

            this._nodeTitle = this.AddText("");
            this._nodeTitle.FontSize = TITLE_SIZE;
            this._nodeTitle.Font = Font.GetFont("Fonts/Orbitron");

            this._nodeTitle.FadeOutComplete += () =>
            {
                // Done; flip content
                showContentFor(this._nextNode);

                foreach (Button b in this._choiceButtons)
                {
                    b.Alpha = 0;
                    b.AlphaRate = CONTENT_FADE_RATE;
                }

                if (this._theEndButton != null)
                {
                    this._theEndButton.Alpha = 0;
                    this._theEndButton.AlphaRate = CONTENT_FADE_RATE;
                }

                this._nodeTitle.AlphaRate = CONTENT_FADE_RATE;
                this._contentText.AlphaRate = CONTENT_FADE_RATE;
                this._choicesTitle.AlphaRate = CONTENT_FADE_RATE;
                this._nextNode = null;
            };

            this._contentText = this.AddText("");
            this._contentText.Z = 1; // Always above (changing) images.                  

            this._choicesTitle = this.AddText("Actions");
            this._choicesTitle.FontSize = TITLE_SIZE;
            this._choicesTitle.X = TEXT_TITLE_OFFSET;
            this._choicesTitle.Y = CHOICES_TITLE_POSITION_Y;
            this._choicesTitle.Font = Font.GetFont("Fonts/Orbitron");

            this._textPanel = this.AddSprite("text-panel");

            this._textPanel.Click += () =>
            {
                if (this._currentPanel < this._contentPanels.Length - 1)
                {
                    this._contentText.FadeOut();
                }
            };
            
            this._contentText.FadeOutComplete += () =>
            {
                if (this._currentPanel < this._contentPanels.Length - 1)
                {
                    this._currentPanel++;
                    showCurrentContentPanel();
                }
            };

            this._textPanel.Z = 100; // float above all.
            this._contentText.Z = this._textPanel.Z + 1;

            // 2*TEXT_PANEL_PADDING suffices, but make space for the MORE button and stuff.
            TEXT_PANEL_READABLE_AREA_WIDTH = this._textPanel.Width - (3 * TEXT_PANEL_PADDING);

            showContentFor(startNode);            

            CoreModel.Instance.StartTime = DateTime.Now;

            this.FadeIn();
        }

        private void loadSpritesForCurrentNode(string template)
        {
            if (this._background != null)
            {
                this.RemoveSprite(this._background);
                this.RemoveSprite(this._titleHeader);
                this.RemoveSprite(this._choicesHeader);
            }

            this._background = this.AddSprite("Scenes/" + template + "-background");
            this._background.PositionFromTopLeftCorner(0, 0);

            this._titleHeader = this.AddSprite("Scenes/" + template + "-header");
            this._titleHeader.PositionFromTopLeftCorner(0, 0);

            this._choicesHeader = this.AddSprite("Scenes/" + template + "-header");
            this._choicesHeader.PositionFromTopLeftCorner(0, CHOICES_TITLE_POSITION_Y);            
        }

        private void showContentFor(Node currentNode)
        {
            this._currentNode = currentNode;

            loadSpritesForCurrentNode(currentNode.Template);
            this._nodeTitle.Content = currentNode.Title;
            this._nodeTitle.X = TEXT_TITLE_OFFSET;
            this._nodeTitle.Y = TITLE_POSITION_Y;

            centerTextPanel();

            this._currentPanel = 0;
            
            this._contentPanels = getTextInPanels(currentNode);
            
            clearLinks();

            showCurrentContentPanel();

            // Badge~!
            if (currentNode.Title.Equals("Martyred")) {
                new BadgeManager().GrantBadge("IZ_UTEBXkBeOe3pcKx8KC82dZ2jgLraG_smE313LCQWkHsIBodEZ1RzJ2n8MHJ6H");
            }
        }

        private void centerTextPanel()
        {
            // Center vertically in the free space between the bottom of the title band and the top of the "choices" band.
            // Uses real Y values and image sizes, including the text-panel height.
            this._textPanel.X = this.Width / 2;
            this._textPanel.Y = ((this._choicesHeader.Y - this._titleHeader.Height) / 2) + this._titleHeader.Height;

            this._contentText.X = this._textPanel.X - (this._textPanel.Width / 2) + TEXT_PANEL_PADDING;
            this._contentText.Y = this._textPanel.Y - (this._textPanel.Height / 2) + TEXT_PANEL_PADDING;
        }

        private void showCurrentContentPanel() {

            this._contentText.Content = this._contentPanels[this._currentPanel];

            if (this._contentPanels.Length == 1 || this._currentPanel == this._contentPanels.Length - 1)
            {
                showLinksFor(this._currentNode);
            }

            this.centerTextPanel();

            this._contentText.FadeIn();

        }

        private string[] getTextInPanels(Node currentNode)
        {
            List<string> toReturn = new List<string>();

            // Get the broken-up text. Add a little buffer for the "[more]" part.
            string rawText = Text.StringFittedToWidth(currentNode.Content, TEXT_PANEL_READABLE_AREA_WIDTH).Trim();

            int textPanelReadableAreaHeight = this._textPanel.Height - (2 * TEXT_PANEL_PADDING);

            // Manual override, I hate you. WHYYY?!??!
            //int linesPerPanel = (int)Math.Floor(textPanelReadableAreaHeight * 1.0f / this._contentText.Font.Size);
            int linesPerPanel = 13;

            string[] textInLines = rawText.Split('\n');

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < textInLines.Length; i++)
            {
                if (i % linesPerPanel == linesPerPanel - 1)
                {
                    // This was the last line in a panel.

                    // Corner case: adding to a blank line. Remove last \n
                    if (string.IsNullOrWhiteSpace(textInLines[i]))
                    {
                        builder.Remove(builder.Length - 1, 1);
                    }

                    builder.Append(textInLines[i]);
                    // Corner case: don't add [more] on the last panel. Happens sometimes.
                    if (i < textInLines.Length - 1)
                    {
                        builder.Append(" [more]").Append('\n');
                    }

                    toReturn.Add(builder.ToString().Trim());
                    builder.Clear();
                }
                else
                {
                    builder.Append(textInLines[i]).Append('\n');
                }
            }

            string lastPanel = builder.ToString().Trim();
            if (!string.IsNullOrWhiteSpace(lastPanel))
            {
                // Corner case: don't give us an empty panel at the end.
                toReturn.Add(lastPanel);
            }

            return toReturn.ToArray();
        }

        private void clearLinks()
        {
            foreach (Button choiceButton in this._choiceButtons)
            {
                choiceButton.Remove();
            }

            this._choiceButtons.Clear();
        }

        private void showLinksFor(Node currentNode)
        {
            showContentLinks(currentNode);

            if (currentNode.Links.Count == 0)
            {
                this._theEndButton = this.AddButton("The End");
                this._theEndButton.Skin = "Gold";

                this._theEndButton.FontSize = 18;

                this._theEndButton.Click += () =>
                {
                    this.FadeOut();
                    this.FadeOutComplete += () =>
                    {
                        CoreModel.Instance.ChoicesTaken = this._choicesTaken;
                        ScreenController.ShowScreen(new SummaryScreen());
                    };
                };

                this._theEndButton.X = (SCREEN_WIDTH - this._theEndButton.Width) / 2;
                this._theEndButton.Y = calculateYForButton(this._theEndButton);

                if (this._currentPanel == this._contentPanels.Length - 1)
                {
                    this._theEndButton.Alpha = 0;
                    this._theEndButton.AlphaRate = CHOICES_BUTTON_FADE_RATE;
                }
            }

            positionButtons();
        }

        private void positionButtons()
        {
            // All the buttons are added. So position them (they have a width now).
            int totalWidth = 0;

            foreach (Button b in this._choiceButtons)
            {
                totalWidth += b.Width;
                totalWidth += PADDING_BETWEEN_BUTTONS;
            }

            // Remove extra padding
            totalWidth -= PADDING_BETWEEN_BUTTONS;

            for (int i = 0; i < this._choiceButtons.Count; i++)
            {
                Button b = this._choiceButtons[i];
                if (i == 0)
                {
                    b.X = (this.Width - totalWidth) / 2;
                }
                else
                {
                    Button previousButton = this._choiceButtons[i - 1];
                    b.X = previousButton.X + previousButton.Width + PADDING_BETWEEN_BUTTONS;
                }

                // center horizontally
                //b.X += b.Width / 2;
                b.Y = calculateYForButton(b);
            }
        }

        private int calculateYForButton(Button b)
        {
            return this._choicesHeader.Y + this._choicesHeader.Height + CHOICES_BUTTONS_OFFSET;
        }

        private void showContentLinks(Node currentNode)
        {
            IList<NodeLink> links = currentNode.Links;
                
            for (int i = 0; i < links.Count; i++)
            {
                Button choiceButton = this.AddButton(links[i].Caption);
                choiceButton.Skin = currentNode.Template;

                Node target = links[i].Target;
                string choice = links[i].Caption;

                choiceButton.Click += () =>
                {
                    if (target.Template == currentNode.Template)
                    {
                        fadeOutContentControls();
                        this._nextNode = target;
                    }
                    else
                    {
                        this.FadeOut();
                        // FadeOutComplete already has the right event.
                    }

                    this._choicesTaken.Add(choice);
                };

                choiceButton.FontSize = 16;

                if (this._currentPanel == this._contentPanels.Length - 1)
                {
                    choiceButton.Alpha = 0;
                    choiceButton.AlphaRate = CHOICES_BUTTON_FADE_RATE;
                }

                this._choiceButtons.Add(choiceButton);
            }
        }

        private void fadeOutContentControls()
        {            
            this._nodeTitle.FadeOut();
            this._contentText.FadeOut();
            this._choicesTitle.FadeOut();
            
            foreach (Button b in this._choiceButtons)
            {
                b.FadeOut();
            }
        }
    }
}
