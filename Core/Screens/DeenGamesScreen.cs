using RadiantWrench.Engine.View;
using RadiantWrench.Engine.Controller;
using System;
using RadiantWrench.Engine.Utils;

namespace DeenGames.ThePrisoner.Screens
{
    public class DeenGamesScreen : Screen
    {
        Sprite _swish;
        Sprite _text;
        DateTime _waitStart = DateTime.Now;

        private const float NUM_SECONDS_TO_FLY_ACROSS_SCREEN = 3f;
        private const float NUM_ROTATIONS = 3f;

        private readonly Point2d TEXT_REST = new Point2d(360, 233);
        private const int TEXT_VERTICAL_OFFSET = 30;

        private AnimationState _currentState = AnimationState.LogoSwishing;

        private enum AnimationState
        {
            LogoSwishing,
            FirstWait,
            TextAppearing,
            Waiting,
            Fading,
            FinalWait
        }

        public override void Initialize()
        {
            base.Initialize();

            _swish = this.AddSprite("Content/swish");
            _swish.PositionFromTopLeftCorner(800 + _swish.Width, 199);
            _swish.Velocity.X = (int)((199f + (_swish.Width / 2) - _swish.X) / NUM_SECONDS_TO_FLY_ACROSS_SCREEN); // travel in 3s to 199
            _swish.RotationRate = -(int)((360 * NUM_ROTATIONS) / NUM_SECONDS_TO_FLY_ACROSS_SCREEN);

            _text = this.AddSprite("Content/text");
            _text.PositionFromTopLeftCorner(TEXT_REST.X, (TEXT_REST.Y + TEXT_VERTICAL_OFFSET));
            _text.Alpha = 0;
        }

        public override void Update(double elapsedSeconds)
        {
            base.Update(elapsedSeconds);
            if (_currentState == AnimationState.LogoSwishing && _swish.X <= 199 + (_swish.Width / 2) && _swish.Velocity.X != 0)
            {
                _swish.X = 199 + (_swish.Width / 2);
                _swish.Velocity.X = 0;
                _swish.RotationAngle = 0;
                _swish.RotationRate = 0;
                _currentState = AnimationState.FirstWait;                
                _waitStart = DateTime.Now;
            } else if (_currentState == AnimationState.FirstWait && (DateTime.Now - _waitStart).TotalSeconds >= 1) {
                // Make text appear
                _text.AlphaRate = 1f;
                _text.Velocity.Y = -TEXT_VERTICAL_OFFSET;
                _currentState = AnimationState.TextAppearing;
            }
            else if (_currentState == AnimationState.TextAppearing && _text.Alpha == 1)
            {
                _text.Y = TEXT_REST.Y + (_text.Height / 2);
                _text.Velocity.Y = 0;
                _currentState = AnimationState.Waiting;
                _waitStart = DateTime.Now;
            }
            else if (_currentState == AnimationState.Waiting && (DateTime.Now - _waitStart).TotalSeconds >= 3)
            {
                _text.AlphaRate = -1;
                _currentState = AnimationState.Fading;
                _waitStart = DateTime.Now;
            }
            else if (_currentState == AnimationState.Fading && (DateTime.Now - _waitStart).TotalSeconds >= 1)
            {
                this.FadeOut();
                _waitStart = DateTime.Now;
                _currentState = AnimationState.FinalWait;
            }
            else if (_currentState == AnimationState.FinalWait && (DateTime.Now - _waitStart).TotalSeconds >= 1)
            {
                ScreenController.ShowScreen(new TitleScreen());
            }
        }
    }
}
