using System;
using System.Collections.Generic;
using System.Linq;
using LearnerApp.Common;
using LearnerApp.Services.Dialog;
using Plugin.HybridWebView.Shared.Delegates;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public abstract class BaseDigitalContentPlayerData
    {
        public BaseDigitalContentPlayerData()
        {
            DialogService = DependencyService.Resolve<IDialogService>();
        }

        public string Description { get; set; }

        protected IDialogService DialogService { get; }

        /// <summary>
        /// This will be used to return the view which supposed to be added to the Player (Video, Webview, Audio...)
        /// </summary>
        /// <returns>The view which will be added to the player.</returns>
        public abstract View GetContentView();

        /// <summary>
        /// This is called after the player from GetContentView has been added.
        /// </summary>
        /// <param name="digitalContentPlayer">The current digital content player.</param>
        public virtual void LoadStyleForDigitalContentPlayer(Controls.DigitalContentPlayer.DigitalContentPlayer digitalContentPlayer)
        {
            digitalContentPlayer.Player.Padding = new Thickness(10, 0);
        }

        /// <summary>
        /// This is called in case you need to add some more data to the view.
        /// </summary>
        /// <param name="metadataViewGroup">The view group of the metadata used to add metadata views.</param>
        public virtual void AddMetadataView(StackLayout metadataViewGroup)
        {
            if (Description.IsNullOrEmpty())
            {
                metadataViewGroup.Children.Add(new Label { Text = Description });
            }
        }

        /// <summary>
        /// This is triggered when the old player is cleared or disappear.
        /// </summary>
        public virtual void OnClearPlayer()
        {
        }

        /// <summary>
        /// Call this when you want to manually clean up the player.
        /// </summary>
        public virtual void Close()
        {
            OnClearPlayer();
        }

        /// <summary>
        /// Get the broken links base on the content of the specific data. You can give more broken links by overriding InnerGetBrokenLink.
        /// </summary>
        /// <returns>The broken links which is used to report.</returns>
        public List<string> GetBrokenLink()
        {
            var brokenLinks = ExtractUrlFromContent.Extract(Description);
            var additionalBrokenLinks = InnerGetBrokenLink();
            if (!additionalBrokenLinks.IsNullOrEmpty())
            {
                brokenLinks.AddRange(additionalBrokenLinks);
            }

            return brokenLinks;
        }

        /// <summary>
        /// Get the broken links base on the content.
        /// </summary>
        /// <returns>he broken links which is used to report.</returns>
        protected virtual List<string> InnerGetBrokenLink()
        {
            return null;
        }

        /// <summary>
        /// Decide if we should open the URL by showing a confirm dialog or just.
        /// </summary>
        /// <param name="e">DecisionHandlerDelegate.</param>
        /// <param name="ignoreUrls">The urls which will be ignored (not being handled).</param>
        protected async void ShouldHandleUrlNavigation(DecisionHandlerDelegate e, params string[] ignoreUrls)
        {
            if (e == null || string.IsNullOrEmpty(e.Uri))
            {
                return;
            }

            bool isUrl = Uri.TryCreate(e.Uri, UriKind.Absolute, out var uriResult) &&
                         (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!isUrl)
            {
                return;
            }

            if (ignoreUrls.Any(ignoreUrl => e.Uri.Contains(ignoreUrl)))
            {
                return;
            }

            e.Cancel = true;

            await DialogService.ConfirmAsync("Do you want to navigate on this page?", onConfirmed: async (confirmed) =>
            {
                if (confirmed)
                {
                    await Browser.OpenAsync(e.Uri, BrowserLaunchMode.External);
                }
            });
        }
    }
}
