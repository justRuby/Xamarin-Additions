using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MoveAndZoom_Image.Extensions;
using Xamarin.Forms;

namespace MoveAndZoom_Image.Extensions
{
    public class MoveAndZoomContent : ContentView
    {
        double currentScale = 1;
        double startScale = 1;
        double xOffset = 0;
        double yOffset = 0;
        bool isBusy;

        public MoveAndZoomContent()
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.NumberOfTapsRequired = 2;
            tapGesture.Tapped += OnDoubleTapped;
            GestureRecognizers.Add(tapGesture);

            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            GestureRecognizers.Add(pinchGesture);

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);
        }

        private void OnDoubleTapped(object sender, EventArgs e)
        {
            currentScale = 1;
            xOffset = 0;
            yOffset = 0;

            double tranlsationX = Math.Max(Math.Min(0, xOffset), -Math.Abs(Content.Width - (Content.Width * currentScale)));
            double tranlsationY = Math.Max(Math.Min(0, yOffset), -Math.Abs(Content.Height - (Content.Height * currentScale)));
            Task.Run(() => Content.TranslateTo(tranlsationX, tranlsationY, 250, Easing.CubicOut));
            Task.Run(() => Content.ScaleTo(currentScale, 250, Easing.CubicOut));
        }

        private async void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
                isBusy = true;
            }
            if (e.Status == GestureStatus.Running)
            {
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);

                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
                Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

                Content.Scale = currentScale;
            }
            if (e.Status == GestureStatus.Completed)
            {
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;

                await Task.Delay(400);
                isBusy = false;
            }
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (!isBusy)
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        Content.TranslationX = Math.Max(Math.Min(0, xOffset + e.TotalX), -Math.Abs(Content.Width - (Content.Width * currentScale)));
                        Content.TranslationY = Math.Max(Math.Min(0, yOffset + e.TotalY), -Math.Abs(Content.Height - (Content.Height * currentScale)));
                        break;

                    case GestureStatus.Completed:
                        xOffset = Content.TranslationX;
                        yOffset = Content.TranslationY;
                        break;
                }
            }
        }

    }
}
