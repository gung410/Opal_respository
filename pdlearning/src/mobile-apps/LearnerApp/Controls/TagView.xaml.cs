using System.Collections.Generic;
using Xamarin.Forms;

namespace LearnerApp.Controls
{
    public partial class TagView : ContentView
    {
        public static readonly BindableProperty ItemsProperty = BindableProperty.Create(nameof(Items), typeof(List<string>), typeof(TagView), null);

        public TagView()
        {
            InitializeComponent();
        }

        public List<string> Items
        {
            get { return (List<string>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
    }
}
