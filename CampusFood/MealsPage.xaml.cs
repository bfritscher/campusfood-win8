﻿using CampusFood.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace CampusFood
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class MealsPage : CampusFood.Common.LayoutAwarePage
    {

        public MealsPage()
        {
            this.InitializeComponent();
            /*
            this.Loaded += (sender, e) =>
            {
            
                if (FoodDataSource.Meals.Count == 0)
                {
                    //this.Frame.Navigate(typeof(MenuSelectionPage));
                    //add some defaults
                   
                }
            };
            */
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
            this.DefaultViewModel["Items"] = FoodDataSource.Meals;
            if (FoodDataSource.Meals.Count == 0)
            {

                 Meal m = new Meal();
                    m.mid = 3.0;
                    m.name = "Amphimax - Assiette 1";
                    FoodDataSource.Meals.Add(m);
                    m = new Meal();
                    m.mid = 4.0;
                    m.name = "Amphimax - Assiette 2";
                    FoodDataSource.Meals.Add(m);
                    m = new Meal();
                    m.mid = 17.0;
                    m.name = "Geopolis - Air";
                    FoodDataSource.Meals.Add(m);
                    m = new Meal();
                    m.mid = 18.0;
                    m.name = "Geopolis - Eau";
                    FoodDataSource.Meals.Add(m);
                    m = new Meal();
                    m.mid = 19.0;
                    m.name = "Geopolis - Feu";
                    FoodDataSource.Meals.Add(m);
                    m = new Meal();
                    m.mid = 16.0;
                    m.name = "Geopolis - Terre";
                    FoodDataSource.Meals.Add(m);
                    m = new Meal();
                    m.mid = 20.0;
                    m.name = "Geopolis - Univers";
                    FoodDataSource.Meals.Add(m);
                    m = new Meal();
                    m.mid = 5.0;
                    m.name = "Unithèque - Assiette 1";
                    FoodDataSource.Meals.Add(m);
                    m = new Meal();
                    m.mid = 6.0;
                    m.name = "Unithèque - Assiette 2";
                    FoodDataSource.Meals.Add(m);
                    m = new Meal();
                    m.mid = 7.0;
                    m.name = "Unithèque - Menu 1";
                    FoodDataSource.Meals.Add(m);
                    m = new Meal();
                    m.mid = 8.0;
                    m.name = "Unithèque - Menu 2";
                    FoodDataSource.Meals.Add(m);
            }

            progressBar.Visibility = Visibility.Visible;
            await FoodDataSource.LoadRemoteMealsAsync();
            progressBar.Visibility = Visibility.Collapsed;
            
            
        }
        
        private void itemGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemGridView.SelectedItem != null)
            {
                PageAppBar.IsOpen = true;
            }
            
            
        }
        

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            FoodDataSource.Meals.Remove((Meal)itemGridView.SelectedItem);
            PageAppBar.IsOpen = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MenuSelectionPage));
        }

        private void PageAppBar_Closed(object sender, object e)
        {
            DeleteButton.Visibility = Visibility.Collapsed;
        }

        private void PageAppBar_Opened(object sender, object e)
        {
            if (itemGridView.SelectedItem != null)
            {
                DeleteButton.Visibility = Visibility.Visible;
            }
        }
    }
}
