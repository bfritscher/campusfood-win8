using CampusFood.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace CampusFood
{

    public sealed partial class MenuSelectionPage : CampusFood.Common.LayoutAwarePage
    {        

        public MenuSelectionPage()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += UpdateSelection;
            this.Unloaded += (sender, e) =>
            {
                FoodDataSource.XMLSerialize();
            };
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {

            progressBar.Visibility = Visibility.Visible;
            await FoodDataSource.LoadRemoteMenusAsync();
            progressBar.Visibility = Visibility.Collapsed;
            // Communicate results through the view model
            this.DefaultViewModel["Filters"] = FoodDataSource.Campus;
            this.DefaultViewModel["ShowFilters"] = true;
            if (FoodDataSource.Meals.Count == 0)
            {
                var resourceLoader = new ResourceLoader();
                var messageDialog = new MessageDialog(resourceLoader.GetString("helpEmptyMenu"));

                // Show the message dialog and wait
                await messageDialog.ShowAsync();
            }
        }

        private void UpdateSelection(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            resultsGridView.SelectionChanged -= resultsGridView_SelectionChanged;
            resultsListView.SelectionChanged -= resultsGridView_SelectionChanged;
            foreach (Meal meal in FoodDataSource.Meals)
            {
                Menu menu = FoodDataSource.Menus.First(m => m.id == meal.mid);
                resultsGridView.SelectedItems.Add(menu);
                resultsListView.SelectedItems.Add(menu);
            }
            resultsGridView.SelectionChanged += resultsGridView_SelectionChanged;
            resultsListView.SelectionChanged += resultsGridView_SelectionChanged;
        }

        /// <summary>
        /// Invoked when a filter is selected using the ComboBox in snapped view state.
        /// </summary>
        /// <param name="sender">The ComboBox instance.</param>
        /// <param name="e">Event data describing how the selected filter was changed.</param>
        void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as Campus;
            if (selectedFilter != null)
            {
                // Mirror the results into the corresponding Filter object to allow the
                // RadioButton representation used when not snapped to reflect the change
                selectedFilter.Active = true;

                // TODO: Respond to the change in active filter by setting this.DefaultViewModel["Results"]
                //       to a collection of items with bindable Image, Title, Subtitle, and Description properties

                resultsGridView.SelectionChanged -= resultsGridView_SelectionChanged;
                resultsListView.SelectionChanged -= resultsGridView_SelectionChanged;
                this.DefaultViewModel["Results"] = selectedFilter.locations;
                UpdateSelection(null, null);
                this.groupGridView.ItemsSource = this.resultsViewSource.View.CollectionGroups;
                

                // Ensure results are found
                object results;
                ICollection resultsCollection;
                if (this.DefaultViewModel.TryGetValue("Results", out results) &&
                    (resultsCollection = results as ICollection) != null &&
                    resultsCollection.Count != 0)
                {
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    return;
                }
            }

            // Display informational text when there are no search results.
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        /// <summary>
        /// Invoked when a filter is selected using a RadioButton when not snapped.
        /// </summary>
        /// <param name="sender">The selected RadioButton instance.</param>
        /// <param name="e">Event data describing how the RadioButton was selected.</param>
        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
            }
        }

        private void resultsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(Menu m in e.AddedItems){
                FoodDataSource.Meals.Add(m.createMeal());
            }
            foreach (Menu m in e.RemovedItems)
            {
                FoodDataSource.Meals.Remove(FoodDataSource.Meals.First(meal => meal.mid == m.id));
            }
        }

        private void Location_Header_Click(object sender, RoutedEventArgs e)
        {
            Location location = (Location) (sender as FrameworkElement).DataContext;
            var mids = FoodDataSource.Meals.Select(me => me.mid);
            var allSelected = location.menus.Select(m => m.id).All(id => mids.Contains(id));
            
            foreach (Menu menu in location.menus)
            {
                if (allSelected)
                {
                    resultsGridView.SelectedItems.Remove(menu);
                }
                else
                {
                    resultsGridView.SelectedItems.Add(menu);
                }
                    
            }
        }

    }
}
