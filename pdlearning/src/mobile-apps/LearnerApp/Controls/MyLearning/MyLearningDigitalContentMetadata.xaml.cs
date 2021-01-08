using System;
using System.Collections.Generic;
using System.Linq;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Models.MyLearning;
using Telerik.XamarinForms.DataControls;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningDigitalContentMetadata : ContentView
    {
        public static readonly BindableProperty DataProperty =
            BindableProperty.Create(
                nameof(Data),
                typeof(MyLearningDigitalContentMetadataTransfer),
                typeof(MyLearningDigitalContentMetadata),
                null,
                propertyChanged: OnDataChanged);

        public MyLearningDigitalContentMetadata()
        {
            InitializeComponent();
        }

        public MyLearningDigitalContentMetadataTransfer Data
        {
            get { return (MyLearningDigitalContentMetadataTransfer)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == null)
            {
                return;
            }

            var view = bindable as MyLearningDigitalContentMetadata;
            var data = newValue as MyLearningDigitalContentMetadataTransfer;
            var keyValuePairs = (newValue as MyLearningDigitalContentMetadataTransfer)?.KeyValuePairs;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (view == null)
                {
                    return;
                }

                if (data != null)
                {
                    // Information
                    view.DescriptionValue.Text = string.IsNullOrEmpty(data.DescriptionValue) ? GlobalSettings.NotAvailable : data.DescriptionValue;
                    view.TitleValue.Text = data.TitleValue;
                    view.TypeValue.Text = data.TypeValue;
                    view.TypeImageValue.Source = data.TypeImageValue;
                    view.UploadDateValue.Text = data.UploadDateValue;
                    view.NumberOfViews.Text = data.ViewsCount.ToString();
                    view.NumberOfDownloads.Text = data.DownloadsCount.ToString();
                    view.NumberOfShares.Text = data.SharesCount.ToString();

                    view.PreRequisitesValue.Text = data.PreRequisties;
                    view.ObjectivesOutcomesValue.Text = data.ObjectivesOutcomes ?? GlobalSettings.NotAvailable;

                    // Right
                    view.OwnershipValue.Text = data.Ownership;
                    view.LicenseTypeValue.Text = data.LicenseType;
                    view.PermissionsValue.Text = data.PermissionsValue;
                    view.LicenseTerritoryValue.Text = data.LicenseTerritory;
                    view.CopyrightOwnerValue.Text = data.CopyrightOwnerValue;
                }

                if (keyValuePairs == null || keyValuePairs.Count <= 0)
                {
                    view.PdOpportunityTypeValue.Text = GlobalSettings.NotAvailable;
                    view.ServiceSchemesValue.Text = GlobalSettings.NotAvailable;
                    view.MainSubjectAreaValue.Text = GlobalSettings.NotAvailable;
                    view.CourseLevelValue.Text = GlobalSettings.NotAvailable;
                    view.LearningframeworkValue.Text = GlobalSettings.NotAvailable;
                    return;
                }

                var serviceSchemes = new List<string>();
                var learningFrameworks = new List<string>();

                foreach (var item in keyValuePairs)
                {
                    switch (item.Value.GroupCode)
                    {
                        case MetadataTagGroupCourse.OPPORTUNITYTYPE:
                            view.PdOpportunityTypeValue.Text = item.Value.DisplayText;
                            break;
                        case MetadataTagGroupCourse.SERVICESCHEMES:
                            serviceSchemes.Add(item.Value.DisplayText);
                            break;
                        case MetadataTagGroupCourse.COURSELEVELS:
                            view.CourseLevelValue.Text = item.Value.DisplayText;
                            break;
                        case MetadataTagGroupCourse.LEARNINGFRAMEWORK:
                            learningFrameworks.Add(item.Value.DisplayText);
                            break;
                    }
                }

                view.PdOpportunityTypeValue.Text = string.IsNullOrEmpty(view.PdOpportunityTypeValue.Text) ? GlobalSettings.NotAvailable : view.PdOpportunityTypeValue.Text;
                view.CourseLevelValue.Text = string.IsNullOrEmpty(view.PdOpportunityTypeValue.Text) ? GlobalSettings.NotAvailable : view.CourseLevelValue.Text;
                view.ServiceSchemesValue.Text = StringExtension.GetInformationFromList(serviceSchemes) ?? GlobalSettings.NotAvailable;
                view.LearningframeworkValue.Text = StringExtension.GetInformationFromList(learningFrameworks) ?? GlobalSettings.NotAvailable;
                view.MainSubjectAreaValue.Text = data?.MainSubjectAreaValue;

                if (data != null && !data.IsDigitalLearningClosed)
                {
                    CreateTreeInView(view, keyValuePairs);
                }
            });
        }

        private static void CreateTreeInView(MyLearningDigitalContentMetadata view, Dictionary<string, MetadataTag> metadataTags)
        {
            var metadataTagging = Application.Current.Properties.GetMetadataTagging();

            if (metadataTagging.IsNullOrEmpty())
            {
                return;
            }

            var serviceSchemesMetadataTags = metadataTags.Values.Where(p => p.GroupCode == MetadataTagGroupCourse.SERVICESCHEMES).ToList();
            var learningFrameworkTags = metadataTags.Values.Where(p => p.GroupCode == MetadataTagGroupCourse.LEARNINGFRAMEWORK).ToList();
            var metadataTree = GetTreeRoot(serviceSchemesMetadataTags, metadataTagging);

            // Create tree Developmental Roles
            view.DevRoleTreeTags.ItemsSource = GetTreeByGroupCode(metadataTree, metadataTags, new List<string> { MetadataTagGroupCourse.DEVROLES });

            // Create tree Subject Areas and General Keywords
            view.MainSubjectAreaTreeTags.ItemsSource = GetTreeByGroupCode(metadataTree, metadataTags, new List<string> { MetadataTagGroupCourse.MAINSUBJECTAREA });

            // Create tree Dimensions and Learning Areas
            var learningFrameworkTree = GetTreeRoot(learningFrameworkTags, metadataTagging);
            var list = new List<string> { "Learning Dimension", "Learning Area", "Learning Area Private" };
            view.DimensionsLearningAreaTreeTags.ItemsSource = GetTreeByGroupCode(learningFrameworkTree, metadataTags, list);

            view.DevRoleTreeTags.CollapseAll();
            view.MainSubjectAreaTreeTags.CollapseAll();
            view.DimensionsLearningAreaTreeTags.CollapseAll();
        }

        private static List<MetadataTag> GetTreeByGroupCode(List<MetadataTag> metadataTree, Dictionary<string, MetadataTag> metadataTags, List<string> groupCode)
        {
            var tree = metadataTree.Clone();

            foreach (var node in tree)
            {
                if (groupCode.Count == 1)
                {
                    node.Children.RemoveAll(m => !groupCode.Contains(m.GroupCode) || !metadataTags.Keys.Contains(m.TagId));
                }
                else
                {
                    node.Children.RemoveAll(m => !groupCode.Contains(m.Type) || !metadataTags.Keys.Contains(m.TagId));
                }
            }

            return tree;
        }

        private static List<MetadataTag> GetTreeRoot(List<MetadataTag> rootTree, List<MetadataTag> metadataTagging)
        {
            var trees = new List<MetadataTag>();
            foreach (var metadata in rootTree)
            {
                metadata.Name = metadata.DisplayText;

                GetTreeChild(metadata, metadataTagging);

                trees.Add(metadata);
            }

            return trees;
        }

        private static void GetTreeChild(MetadataTag item, List<MetadataTag> metadataTagging)
        {
            foreach (var child in metadataTagging)
            {
                if (child.ParentTagId == item.TagId && !child.IsCollected)
                {
                    child.Name = child.DisplayText;

                    item.Children.Add(child);

                    child.IsCollected = true;

                    GetTreeChild(child, metadataTagging);
                }
            }
        }

        private void ShowMoreInformation_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                bool hiddenStatus = !HiddenInformationStack.IsVisible;

                ShowMoreLabel.Text = hiddenStatus ? "SHOW LESS" : "SHOW MORE";

                HiddenInformationStack.IsVisible = hiddenStatus;
            });
        }

        private void RadTreeView_ItemCollapsed(object sender, Telerik.XamarinForms.DataControls.TreeView.TreeViewItemEventArgs e)
        {
            ((RadTreeView)sender).InvalidateMeasureNonVirtual(InvalidationTrigger.MeasureChanged);
        }

        private void RadTreeView_ItemExpanded(object sender, Telerik.XamarinForms.DataControls.TreeView.TreeViewItemEventArgs e)
        {
            ((RadTreeView)sender).InvalidateMeasureNonVirtual(InvalidationTrigger.MeasureChanged);
        }
    }
}
