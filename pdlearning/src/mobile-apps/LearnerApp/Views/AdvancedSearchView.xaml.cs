using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Models.PdCatelogue;
using LearnerApp.Models.Search;
using LearnerApp.Services.Backend;
using LearnerApp.Services.DataManager.CatalogueFilter;
using LearnerApp.Services.Dialog;
using LearnerApp.ViewModels.Base;
using LearnerApp.ViewModels.Search;
using Rg.Plugins.Popup.Services;
using Telerik.XamarinForms.Input;
using Telerik.XamarinForms.Input.AutoComplete;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvancedSearchView
    {
        private readonly IDialogService _dialogService;
        private readonly string _notApplicable = "Not Applicable";
        private CatalogueFilterModel _catalogueFilterModel;
        private IPdCatelogueService _pdCatalogueService;
        private List<MetadataTag> _metadataTagging;
        private JobDesignation[] _jobDesignationSuggestions;

        public AdvancedSearchView()
        {
            InitializeComponent();

            _pdCatalogueService = BaseViewModel.CreateRestClientFor<IPdCatelogueService>(GlobalSettings.BackendPdCatelogueService);
            _dialogService = DependencyService.Resolve<IDialogService>();

            Task.Run(LoadData);
        }

        protected async Task LoadData()
        {
            using (_dialogService.DisplayLoadingIndicator())
            {
                var metadataTagging = App.Current.Properties.GetMetadataTagging();
                _jobDesignationSuggestions = await _pdCatalogueService.GetJobDesignation() ?? new JobDesignation[0];
                _metadataTagging = new List<MetadataTag>();

                if (!metadataTagging.IsNullOrEmpty())
                {
                    _metadataTagging = metadataTagging;

                    TypePDActivitySelect.ItemsSource = metadataTagging
                        .Where(p => p.GroupCode == MetadataTagGroupCourse.OPPORTUNITYTYPE && p.DisplayText != _notApplicable)
                        .OrderBy(p => p.DisplayText)
                        .ToList();

                    ModeOfLearningSelect.ItemsSource = metadataTagging
                        .Where(p => p.GroupCode == MetadataTagGroupCourse.PDOMODES && p.DisplayText != _notApplicable)
                        .OrderBy(p => p.DisplayText)
                        .ToList();

                    PdoCategorySelect.ItemsSource = metadataTagging
                        .Where(p => p.GroupCode == MetadataTagGroupCourse.PDOCATEGORIES && p.DisplayText != _notApplicable)
                        .OrderBy(p => p.DisplayText)
                        .ToList();

                    ServiceSchemesSelect.ItemsSource = metadataTagging
                        .Where(p => p.GroupCode == MetadataTagGroupCourse.SERVICESCHEMES && p.DisplayText != _notApplicable)
                        .OrderBy(p => p.DisplayText).ToList();

                    TeachingLevelSelect.ItemsSource = metadataTagging
                        .Where(p => p.GroupCode == MetadataTagGroupCourse.TEACHINGLEVEL && p.DisplayText != _notApplicable)
                        .OrderBy(p => p.DisplayText)
                        .ToList();

                    CourseLevelSelect.ItemsSource = metadataTagging
                        .Where(p => p.GroupCode == MetadataTagGroupCourse.COURSELEVELS && p.DisplayText != _notApplicable)
                        .OrderBy(p => p.DisplayText)
                        .ToList();

                    NatureCourseSelect.ItemsSource = metadataTagging
                        .Where(p => p.GroupCode == MetadataTagGroupCourse.PDONATURES && p.DisplayText != _notApplicable)
                        .OrderBy(p => p.DisplayText)
                        .ToList();

                    SubjectSelect.ItemsSource = _metadataTagging
                        .Where(p => p.GroupCode == MetadataTagGroupCourse.TEACHINGSUBJECT && p.DisplayText != _notApplicable)
                        .OrderBy(p => p.DisplayText)
                        .ToList();

                    CommunityTypeSelect.ItemsSource = new List<CommunityTypeSearchViewModel>()
                    {
                        new CommunityTypeSearchViewModel(CommunitySearchTypeEnum.Open),
                        new CommunityTypeSearchViewModel(CommunitySearchTypeEnum.Restricted),
                    };

                    AttachmentTypeSelect.ItemsSource = new List<AttachmentTypeSearchViewModel>()
                    {
                        new AttachmentTypeSearchViewModel(AttachmentTypeEnum.DocumentFiles),
                        new AttachmentTypeSearchViewModel(AttachmentTypeEnum.DigitalGraphics),
                        new AttachmentTypeSearchViewModel(AttachmentTypeEnum.Audio),
                        new AttachmentTypeSearchViewModel(AttachmentTypeEnum.Video),
                        new AttachmentTypeSearchViewModel(AttachmentTypeEnum.InteractiveObjects),
                        new AttachmentTypeSearchViewModel(AttachmentTypeEnum.LearningPackages)
                    };

                    JobDesignationSelect.ItemsSource = _jobDesignationSuggestions;
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _catalogueFilterModel = CatalogueFilterManager.Current.GetCatalogueFilterModel();

            TypePDActivitySelect.Text = _catalogueFilterModel.TypePDActivitySelect?.DisplayText;
            ModeOfLearningSelect.Text = _catalogueFilterModel.ModeOfLearningSelect?.DisplayText;

            foreach (var item in _catalogueFilterModel.PdoCategorySelect)
            {
                PdoCategorySelect.Tokens.Add(item);
            }

            foreach (var item in _catalogueFilterModel.ServiceSchemesSelect)
            {
                ServiceSchemesSelect.Tokens.Add(item);
            }

            foreach (var item in _catalogueFilterModel.DevelopmentalRolesSelect)
            {
                DevelopmentalRolesSelect.Tokens.Add(item);
            }

            foreach (var item in _catalogueFilterModel.TeachingLevelSelect)
            {
                TeachingLevelSelect.Tokens.Add(item);
            }

            CourseLevelSelect.Text = _catalogueFilterModel.CourseLevelSelect?.DisplayText;

            foreach (var item in _catalogueFilterModel.SubjectSelect)
            {
                SubjectSelect.Tokens.Add(item);
            }

            foreach (var item in _catalogueFilterModel.LearningFrameworkSelect)
            {
                LearningFrameworkSelect.Tokens.Add(item);
            }

            foreach (var item in _catalogueFilterModel.LearningAreaSelect)
            {
                LearningAreaSelect.Tokens.Add(item);
            }

            foreach (var item in _catalogueFilterModel.LearningDimensionSelect)
            {
                LearningDimensionSelect.Tokens.Add(item);
            }

            foreach (var item in _catalogueFilterModel.CommunityTypes)
            {
                CommunityTypeSelect.Tokens.Add(new CommunityTypeSearchViewModel(item));
            }

            CommunityTypeSelect.Text = _catalogueFilterModel.AttachmentType?.ToFriendlyString();
            LearningSubAreaSelect.Text = _catalogueFilterModel.LearningSubAreaSelect?.DisplayText;
            NatureCourseSelect.Text = _catalogueFilterModel.NatureCourseSelect?.DisplayText;
            JobDesignationSelect.Text = _catalogueFilterModel.JobDesignation?.DisplayText;
        }

        private async Task SaveCatalogueFilterViewModel()
        {
            _catalogueFilterModel.TypePDActivitySelect = _metadataTagging.FirstOrDefault(x => x.DisplayText == TypePDActivitySelect.Text);
            _catalogueFilterModel.ModeOfLearningSelect = _metadataTagging.FirstOrDefault(x => x.DisplayText == ModeOfLearningSelect.Text);
            _catalogueFilterModel.PdoCategorySelect = PdoCategorySelect.Tokens.Cast<MetadataTag>().ToList();
            _catalogueFilterModel.ServiceSchemesSelect = ServiceSchemesSelect.Tokens.Cast<MetadataTag>().ToList();
            _catalogueFilterModel.DevelopmentalRolesSelect = DevelopmentalRolesSelect.Tokens.Cast<MetadataTag>().ToList();
            _catalogueFilterModel.TeachingLevelSelect = TeachingLevelSelect.Tokens.Cast<MetadataTag>().ToList();
            _catalogueFilterModel.CourseLevelSelect = _metadataTagging.FirstOrDefault(x => x.DisplayText == CourseLevelSelect.Text);
            _catalogueFilterModel.SubjectSelect = SubjectSelect.Tokens.Cast<MetadataTag>().ToList();
            _catalogueFilterModel.LearningFrameworkSelect = LearningFrameworkSelect.Tokens.Cast<MetadataTag>().ToList();
            _catalogueFilterModel.LearningDimensionSelect = LearningDimensionSelect.Tokens.Cast<MetadataTag>().ToList();
            _catalogueFilterModel.LearningAreaSelect = LearningAreaSelect.Tokens.Cast<MetadataTag>().ToList();
            _catalogueFilterModel.LearningSubAreaSelect = _metadataTagging.FirstOrDefault(x => x.DisplayText == LearningSubAreaSelect.Text);
            _catalogueFilterModel.NatureCourseSelect = _metadataTagging.FirstOrDefault(x => x.DisplayText == NatureCourseSelect.Text);

            _catalogueFilterModel.CommunityTypes = CommunityTypeSelect.Tokens
                .OfType<CommunityTypeSearchViewModel>()
                .Select(x => x.Value)
                .ToList();

            _catalogueFilterModel.AttachmentType = AttachmentTypeSelect.Text.ToAttachmentTypeSearchTypeEnum();

            _catalogueFilterModel.JobDesignation = _jobDesignationSuggestions.FirstOrDefault(x => x.DisplayText == JobDesignationSelect.Text);

            await CatalogueFilterManager.Current.SaveCatalogueFilterModel(_catalogueFilterModel);
        }

        private void Selection_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            var selection = sender as RadAutoCompleteView;

            ShowAllSugesstion(selection);
        }

        private void RadAutoCompleteView_SuggestionItemSelected(object sender, SuggestionItemSelectedEventArgs e)
        {
            var view = sender as RadAutoCompleteView;

            view.Tokens.Remove(e.DataItem);
        }

        private void Showmore_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                bool status = ShowMoreStack.IsVisible;
                if (status)
                {
                    ShowmoreLbl.Text = "Show more";
                }
                else
                {
                    ShowmoreLbl.Text = "Show less";
                }

                ShowMoreStack.IsVisible = !status;
            });
        }

        private void ClearFilter_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                LearningAreaSelect.Tokens.Clear();
                LearningDimensionSelect.Tokens.Clear();
                LearningFrameworkSelect.Tokens.Clear();
                SubjectSelect.Tokens.Clear();
                TeachingLevelSelect.Tokens.Clear();
                DevelopmentalRolesSelect.Tokens.Clear();
                ServiceSchemesSelect.Tokens.Clear();
                PdoCategorySelect.Tokens.Clear();
                CourseLevelSelect.Text = string.Empty;
                ModeOfLearningSelect.Text = string.Empty;
                LearningSubAreaSelect.Text = string.Empty;
                TypePDActivitySelect.Text = string.Empty;
                NatureCourseSelect.Text = string.Empty;

                await CatalogueFilterManager.Current.SaveCatalogueFilterModel(null);
            });
        }

        private async void OnApplyFilter_Tapped(object sender, EventArgs e)
        {
            using (_dialogService.DisplayLoadingIndicator())
            {
                await SaveCatalogueFilterViewModel();

                MessagingCenter.Send<AdvancedSearchView, CatalogueFilterModel>(this, "advanced-search-filter-value", _catalogueFilterModel);

                await PopupNavigation.Instance.RemovePageAsync(this);
            }
        }

        private void DevelopmentalRolesSelect_Focused(object sender, FocusEventArgs e)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            var serviceSchemeIds = ServiceSchemesSelect.Tokens.Cast<MetadataTag>().Select(_ => _.TagId)?.ToArray();

            DevelopmentalRolesSelect.ItemsSource = _metadataTagging.AsParallel().Where(p => p.GroupCode == MetadataTagGroupCourse.DEVROLES && serviceSchemeIds.Contains(p.ParentTagId) && p.DisplayText != _notApplicable).OrderBy(p => p.DisplayText).ToList();

            ShowAllSugesstion(sender as RadAutoCompleteView);

            IsBusy = false;
        }

        private void LearningFrameworkSelect_Focused(object sender, FocusEventArgs e)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            var serviceSchemeIds = ServiceSchemesSelect.Tokens.Cast<MetadataTag>().Select(_ => _.TagId)?.ToArray();

            LearningFrameworkSelect.ItemsSource = _metadataTagging.AsParallel().Where(p => p.GroupCode == MetadataTagGroupCourse.LEARNINGFRAMEWORK && serviceSchemeIds.Contains(p.ParentTagId) && p.DisplayText != _notApplicable).OrderBy(p => p.DisplayText).ToList();

            ShowAllSugesstion(sender as RadAutoCompleteView);

            IsBusy = false;
        }

        private void LearningDimensionSelect_Focused(object sender, FocusEventArgs e)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            var learningFrameworkIds = LearningFrameworkSelect.Tokens.Cast<MetadataTag>().Select(_ => _.TagId)?.ToArray();

            LearningDimensionSelect.ItemsSource = _metadataTagging.AsParallel().Where(p => learningFrameworkIds.Contains(p.ParentTagId) && p.DisplayText != _notApplicable).OrderBy(p => p.DisplayText).ToList();

            ShowAllSugesstion(sender as RadAutoCompleteView);

            IsBusy = false;
        }

        private void LearningAreaSelect_Focused(object sender, FocusEventArgs e)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            var learningDimensionIds = LearningDimensionSelect.Tokens.Cast<MetadataTag>().Select(_ => _.TagId)?.ToArray();

            LearningAreaSelect.ItemsSource = _metadataTagging.AsParallel().Where(p => learningDimensionIds.Contains(p.ParentTagId) && p.DisplayText != _notApplicable).OrderBy(p => p.DisplayText).ToList();

            ShowAllSugesstion(sender as RadAutoCompleteView);

            IsBusy = false;
        }

        private void LearningSubAreaSelect_Focused(object sender, FocusEventArgs e)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            var learningAreaIds = LearningAreaSelect.Tokens.Cast<MetadataTag>().Select(_ => _.TagId)?.ToArray();

            LearningSubAreaSelect.ItemsSource = _metadataTagging.AsParallel().Where(p => learningAreaIds.Contains(p.ParentTagId) && p.DisplayText != _notApplicable).OrderBy(p => p.DisplayText).ToList();

            ShowAllSugesstion(sender as RadAutoCompleteView);

            IsBusy = false;
        }

        private void ShowAllSugesstion(RadAutoCompleteView view)
        {
            if (view.Text == null)
            {
                view.Text = string.Empty;
            }

            bool isAny = false;
            foreach (var obj in view.ItemsSource)
            {
                isAny = true;
                break;
            }

            if (isAny)
            {
                view.ShowSuggestions();
            }
        }

        private void Close_Tapped(object sender, EventArgs e)
        {
            PopupNavigation.Instance.RemovePageAsync(this);
        }

        private void ServiceSchemesSelect_Unfocused(object sender, FocusEventArgs e)
        {
            var serviceSchemeIds = ServiceSchemesSelect.Tokens.Cast<MetadataTag>().Select(_ => _.TagId)?.ToArray();

            if (serviceSchemeIds.Count() > 0)
            {
                DevelopmentalRolesSelect.IsEnabled = true;
                SubjectSelect.IsEnabled = true;
                LearningFrameworkSelect.IsEnabled = true;
            }
            else
            {
                DevelopmentalRolesSelect.IsEnabled = false;
                SubjectSelect.IsEnabled = false;
                LearningFrameworkSelect.IsEnabled = false;
                LearningDimensionSelect.IsEnabled = false;
                LearningAreaSelect.IsEnabled = false;

                LearningDimensionSelect.Tokens.Clear();
                DevelopmentalRolesSelect.Tokens.Clear();
                SubjectSelect.Tokens.Clear();
                LearningFrameworkSelect.Tokens.Clear();
                LearningAreaSelect.Tokens.Clear();
            }
        }

        private void LearningFrameworkSelect_Unfocused(object sender, FocusEventArgs e)
        {
            var learningFrameworkIds = LearningFrameworkSelect.Tokens.Cast<MetadataTag>().Select(_ => _.TagId)?.ToArray();

            if (learningFrameworkIds.Count() > 0)
            {
                LearningDimensionSelect.IsEnabled = true;
            }
            else
            {
                LearningDimensionSelect.IsEnabled = false;
                LearningAreaSelect.IsEnabled = false;

                LearningAreaSelect.Tokens.Clear();
                LearningDimensionSelect.Tokens.Clear();
            }
        }

        private void LearningDimensionSelect_Unfocused(object sender, FocusEventArgs e)
        {
            var learningDimensionIds = LearningDimensionSelect.Tokens.Cast<MetadataTag>().Select(_ => _.TagId)?.ToArray();

            if (learningDimensionIds.Count() > 0)
            {
                LearningAreaSelect.IsEnabled = true;
            }
            else
            {
                LearningAreaSelect.IsEnabled = false;

                LearningAreaSelect.Tokens.Clear();
            }
        }
    }
}
