using System;
using System.Collections.Generic;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class ImageDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        private string _source;

        public ImageDigitalContentPlayerData(string uri)
        {
            _source = uri;
        }

        public override View GetContentView()
        {
            return new CachedImage
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Aspect = Aspect.AspectFit,
                CacheDuration = TimeSpan.FromDays(2),
                CacheType = CacheType.Disk,
                DownsampleToViewSize = true,
                ErrorPlaceholder = "image_place_holder_h150.png",
                Source = _source
            };
        }

        protected override List<string> InnerGetBrokenLink()
        {
            return new List<string>()
            {
                _source
            };
        }
    }
}
