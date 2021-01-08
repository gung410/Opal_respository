using System;
using System.Collections.Generic;
using LearnerApp.Services.Navigation;
using Telerik.XamarinForms.PdfViewer;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class PdfDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        private readonly string _source;

        public PdfDigitalContentPlayerData(string source)
        {
            _source = source;
        }

        public override View GetContentView()
        {
            RadPdfViewer pdfViewer = new RadPdfViewer
            {
                Source = new UriDocumentSource(new Uri(_source)),
                EnableHardwareAcceleration = true,
                MinZoomLevel = 0.7,
                MaxZoomLevel = 3.0,
                LayoutMode = LayoutMode.ContinuousScroll,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            pdfViewer.SourceException += PdfSource_Exception;
            return pdfViewer;
        }

        protected override List<string> InnerGetBrokenLink()
        {
            return new List<string>()
            {
                _source
            };
        }

        private void PdfSource_Exception(object sender, SourceExceptionEventArgs e)
        {
            DialogService.ShowAlertAsync("PDF source error");
        }
    }
}
