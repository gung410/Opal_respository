using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard.VerticalItem
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerticalStandAloneFormItem : ContentView
    {
        public VerticalStandAloneFormItem()
        {
            InitializeComponent();

            TagView.Items = new List<string> { "Standalone form" };
        }
    }
}
