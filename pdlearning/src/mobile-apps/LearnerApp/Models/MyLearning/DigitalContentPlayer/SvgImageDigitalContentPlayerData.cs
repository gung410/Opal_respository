using System;
using System.Collections.Generic;
using FFImageLoading.Cache;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class SvgImageDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        private string _source;

        public SvgImageDigitalContentPlayerData(string source)
        {
            _source = source;
        }

        public override View GetContentView()
        {
            return new SvgCachedImage
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
