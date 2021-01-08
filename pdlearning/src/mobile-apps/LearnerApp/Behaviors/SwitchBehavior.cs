using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace LearnerApp.Behaviors
{
    public class SwitchBehavior : Behavior<Switch>
    {
        public static readonly BindableProperty ToggledCommandProperty = BindableProperty.Create(nameof(ToggledCommand), typeof(ICommand), typeof(SwitchBehavior), null);

        public ICommand ToggledCommand
        {
            get { return (ICommand)GetValue(ToggledCommandProperty); }
            set { SetValue(ToggledCommandProperty, value); }
        }

        public Switch Bindable { get; private set; }

        protected override void OnAttachedTo(Switch bindable)
        {
            base.OnAttachedTo(bindable);
            Bindable = bindable;
            Bindable.BindingContextChanged += OnBindingContextChanged;
            Bindable.Toggled += OnSwitchToggled;
        }

        protected override void OnDetachingFrom(Switch bindable)
        {
            base.OnDetachingFrom(bindable);
            Bindable.BindingContextChanged -= OnBindingContextChanged;
            Bindable.Toggled -= OnSwitchToggled;
            Bindable = null;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
            BindingContext = Bindable.BindingContext;
        }

        private void OnSwitchToggled(object sender, ToggledEventArgs e)
        {
            ToggledCommand?.Execute(e.Value);
        }
    }
}
