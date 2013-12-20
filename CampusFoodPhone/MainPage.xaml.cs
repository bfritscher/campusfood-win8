using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CampusFoodPhone.Resources;

namespace CampusFoodPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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
    }
}