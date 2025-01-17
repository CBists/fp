﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudContainer
{
    public class DefaultDrawer : IDrawer
    {
        private readonly DefaultDrawerSettings defaultDrawerSettings;
        private readonly IRectangleArranger rectangleArranger;
        private readonly IInputTextProvider inputTextProvider;

        public DefaultDrawer(IInputTextProvider inputTextProvider, DefaultDrawerSettingsProvider settingsProvider,
            IRectangleArranger rectangleArranger)
        {
            this.inputTextProvider = inputTextProvider;
            this.defaultDrawerSettings = settingsProvider.DefaultDrawerSettings;
            this.rectangleArranger = rectangleArranger;
        }

        public Result<Bitmap> DrawImage(string text)
        {
            var textContainersResult =
                Result.Of(() =>
                    rectangleArranger.GetContainers(inputTextProvider.GetWords(text), defaultDrawerSettings.Font));
            if (!textContainersResult.IsSuccess)
                return Result.Fail<Bitmap>(textContainersResult.Error);
            var textContainers = textContainersResult.Value;
            var radius = GetRadius(textContainers) + 100;
            var image = new Bitmap(radius * 2, radius * 2);
            var graphics = Graphics.FromImage(image);
            graphics.Clear(defaultDrawerSettings.BackgroundColor);
            foreach (var container in textContainers)
            {
                var x = container.Rectangle.X + radius;
                var y = container.Rectangle.Y + radius;
                graphics.DrawString(container.Text, container.Font, new SolidBrush(defaultDrawerSettings.FontColor), x,
                    y);
            }

            return Result.Ok(image);
        }

        private int GetRadius(List<TextContainer> containers)
        {
            int radius = 0;
            foreach (var container in containers)
            {
                radius = Math.Max(radius, Math.Abs(container.Rectangle.Bottom));
                radius = Math.Max(radius, Math.Abs(container.Rectangle.Top));
                radius = Math.Max(radius, Math.Abs(container.Rectangle.Right));
                radius = Math.Max(radius, Math.Abs(container.Rectangle.Left));
            }

            return radius;
        }
    }
}