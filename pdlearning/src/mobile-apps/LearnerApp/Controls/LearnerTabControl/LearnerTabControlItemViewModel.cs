using LearnerApp.ViewModels.Base;

namespace LearnerApp.Controls.LearnerTabControl
{
    public class LearnerTabControlItemViewModel : BaseViewModel
    {
        private string _text;
        private bool _isSelected;
        private bool _isVisible;

        public LearnerTabControlItemViewModel(string id, string text)
        {
            Id = id;
            _text = text;
        }

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text)
                {
                    return;
                }

                _text = value;
                RaisePropertyChanged(() => Text);
            }
        }

        public string Id { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected)
                {
                    return;
                }

                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible)
                {
                    return;
                }

                _isVisible = value;
                RaisePropertyChanged(() => IsVisible);
            }
        }
    }
}
