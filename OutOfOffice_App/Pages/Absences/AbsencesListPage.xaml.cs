using CommunityToolkit.Maui.Core.Extensions;
using OutOfOffice.Models;
using OutOfOffice.ViewModels;
using System.Text.RegularExpressions;

namespace OutOfOffice.Pages;

public partial class AbsencesListPage : ContentPage
{
    private readonly AbsencesListViewModel viewmodel;
    public AbsencesListPage(AbsencesListViewModel vm)
    {
        BindingContext = vm;
        viewmodel = vm;
        InitializeComponent();
    }

    private void CollectionView_Loaded(object sender, EventArgs e)
    {
        viewmodel.LeaveRequests = Globals.User.GetLeaveRequests().ToObservableCollection();
        viewmodel.FilteredLeaveRequests = viewmodel.LeaveRequests;
        viewmodel.CanSubmitLeaveRequest = Globals.User.PermissionList.CanSubmitRequests;
        viewmodel.CanViewReview = Globals.User.PermissionList.CanViewReviews;

        if (viewmodel.CanViewReview)
        {
            viewmodel.Reviews = Globals.User.GetReviews().ToObservableCollection();

            foreach (Review Review in viewmodel.Reviews)
            {
                LeaveRequest? ReviewedRequest = viewmodel.LeaveRequests.FirstOrDefault(l => l.Id == Review.LeaveRequest_Id);

                if (ReviewedRequest != null)
                    viewmodel.LeaveRequests[viewmodel.LeaveRequests.IndexOf(ReviewedRequest)].Review = Review;
            }

            if (!viewmodel.SortList.Contains("Reviewer"))
            {
                string[] AdditionalSortOptions = ["Review Status", "Reviewer"];
                foreach (string Option in AdditionalSortOptions)
                    viewmodel.SortList.Add(Option);
            }
        }

        ApplyFilters(FindByName("RequestFilter"), null);
        UpdateSort();
    }

    private void SortPicker_SelectedIndexChanged(object sender, EventArgs e) => UpdateSort();

    public void UpdateSort()
    {
        string SelectedSort = Regex.Replace(viewmodel.SelectedSort, @"\s+", "");

        if (SelectedSort == "")
            return;

        bool Reviewed = viewmodel.SelectedSortIndex > 5;

        string[] TranslatedSort = ["Status", "Approver"];

        if (Reviewed)
        {
            SelectedSort = TranslatedSort[viewmodel.SelectedSortIndex - 6];
            viewmodel.FilteredLeaveRequests = viewmodel.FilteredLeaveRequests.Where(r => r.Review != null).ToObservableCollection();
        }

        if (SelectedSort == "ReviewStatus")
            SelectedSort = "Status";

        viewmodel.SortedLeaveRequests = viewmodel.FilteredLeaveRequests
            .OrderBy(l => Reviewed ? l.Review?.GetType().GetProperty(SelectedSort)?.GetValue(l.Review) : l.GetType().GetProperty(SelectedSort)?.GetValue(l))
            .ToObservableCollection();

        if (viewmodel.Descending)
            viewmodel.SortedLeaveRequests = viewmodel.SortedLeaveRequests.Reverse().ToObservableCollection();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void OrderButtonClicked(object sender, EventArgs e)
    {
        viewmodel.Descending = !viewmodel.Descending;
        UpdateSort();
    }

    private void ApplyFilters(object sender, TextChangedEventArgs e)
    {
        string[] Filters = [];

        if (sender is Entry FilterEntry && FilterEntry.Text is string)
            Filters = FilterEntry.Text.Split(",");

        Dictionary<string, string> FilterDictionary = [];

        foreach (string Filter in Filters)
        {
            string[] SplitFilter = Filter.Split('=');

            if (SplitFilter.Length > 1 && SplitFilter[1].Trim() != "")
                FilterDictionary.Add(Regex.Replace(SplitFilter[0], @"\s+", ""), SplitFilter[1].Trim().ToLower());
        }


        viewmodel.FilteredLeaveRequests = viewmodel.LeaveRequests.Where(l =>
            (!FilterDictionary.ContainsKey("name") || l.Owner.Contains(FilterDictionary["name"], StringComparison.OrdinalIgnoreCase)) &&
            (!FilterDictionary.ContainsKey("status") || l.StatusString.Contains(FilterDictionary["status"], StringComparison.OrdinalIgnoreCase)) &&
            (!FilterDictionary.ContainsKey("startdate") || l.StartDateOnly.Split(' ')[1].Contains(FilterDictionary["startdate"])) &&
            (!FilterDictionary.ContainsKey("enddate") || l.EndDateOnly.Split(' ')[1].Contains(FilterDictionary["enddate"])) &&
            (!FilterDictionary.ContainsKey("reviewer") || (l.Review != null && l.Review.Approver.Contains(FilterDictionary["reviewer"], StringComparison.OrdinalIgnoreCase))) &&
            (!FilterDictionary.ContainsKey("reviewstatus") || (l.Review != null && l.Review.StatusString.Contains(FilterDictionary["reviewstatus"], StringComparison.OrdinalIgnoreCase))) &&
            (!FilterDictionary.ContainsKey("reason") || l.Reason.Contains(FilterDictionary["reason"], StringComparison.OrdinalIgnoreCase)))
            .ToObservableCollection();

        UpdateSort();
    }
}