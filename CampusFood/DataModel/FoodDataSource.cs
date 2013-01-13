using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace CampusFood.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class FoodCommon : CampusFood.Common.BindableBase
    {
        private double _id;
        public double id
        {
            get { return this._id; }
            set { this.SetProperty(ref this._id, value); }
        }

        private string _name = string.Empty;
        public string name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        public override string ToString()
        {
            return this.name;
        }
    }

    public class Campus : FoodCommon
    {
        private ObservableCollection<Location> _locations = new ObservableCollection<Location>();
        public ObservableCollection<Location> locations
        {
            get { return this._locations; }
        }
    }

    public class Location : FoodCommon
    {
        private ObservableCollection<Menu> _menus = new ObservableCollection<Menu>();
        public ObservableCollection<Menu> menus
        {
            get { return this._menus; }
        }
        private Campus _campus;
        public Campus campus
        {
            get { return this._campus; }
            set { this.SetProperty(ref this._campus, value); }
        }
    }

    public class Menu : FoodCommon
    {

        private Meal _meal;
        public Meal meal
        {
            get { return this._meal; }
            set { this.SetProperty(ref this._meal, value); }
        }

        private Location _location;
        public Location location
        {
            get { return this._location; }
            set { this.SetProperty(ref this._location, value); }
        }

        public override string ToString()
        {
            return String.Join("", new String[]{this.location.name, " - ", this.name});
        }
        
    }

    public class Meal : FoodCommon
    {
        private Menu _menu;
        public Menu menu
        {
            get { return this._menu; }
            set { this.SetProperty(ref this._menu, value); }
        }
    }

    public sealed class FoodDataSource
    {

        public ObservableCollection<Meal> FakeMeals
        {
            get
            {
                var meals = new ObservableCollection<Meal>();
                Campus c = new Campus();
                c.id = 1;
                c.name = "UNIL";
                Location l = new Location();
                l.id = 1;
                l.name = "Geopolis";
                l.campus = c;
                c.locations.Add(l);
                Menu m1 = new Menu();
                m1.name = "Eau";
                m1.location = l;
                l.menus.Add(m1);
                Meal me = new Meal();
                me.id = 1;
                me.name = "Jus d'orange\nPoitrine de poulet (BRA)\naux herbettes\nRiz créole\nSalade ou pomme\nBoulangerie";
                me.menu = m1;
                m1.meal = me;

                meals.Add(me);
                meals.Add(me);
                meals.Add(me);
                meals.Add(me);
                meals.Add(me);

                return meals;
            }
        }

        private static FoodDataSource _foodDataSource = new FoodDataSource();

        private ObservableCollection<Campus> _campus = new ObservableCollection<Campus>();
        public ObservableCollection<Campus> Campus
        {
            get { return this._campus; }
        }

        private ObservableCollection<Meal> _meals = new ObservableCollection<Meal>();
        public static ObservableCollection<Meal> Meals
        {
            get { return _foodDataSource._meals; }
        }

        public static IEnumerable<Location> Locations
        {
            get { return _foodDataSource.Campus.SelectMany(campus => campus.locations); }
        }

        public static IEnumerable<Menu> Menus
        {
            get { return FoodDataSource.Locations.SelectMany(l => l.menus); }
        }

        public static async Task LoadRemoteMenusAsync()
        {
            // Retrieve recipe data from Azure
            var client = new HttpClient();
            client.MaxResponseContentBufferSize = 1024 * 1024; // Read up to 1 MB of data
            var response = await client.GetAsync(new Uri("https://isisvn.unil.ch/campusfood/api/menus"));
            var result = await response.Content.ReadAsStringAsync();

            // Parse the JSON recipe data
            CreateCampus(JsonArray.Parse(result));
        }

        public static async Task LoadRemoteMealsAsync()
        {
            //TODO: handle date
            //TODO: handle cache
            //TODO: handle selection of menus

            // Retrieve recipe data from Azure
            var client = new HttpClient();
            client.MaxResponseContentBufferSize = 1024 * 1024; // Read up to 1 MB of data
            var response = await client.GetAsync(new Uri("https://isisvn.unil.ch/campusfood/api/meals/2013-01-10"));
            var result = await response.Content.ReadAsStringAsync();

            // Parse the JSON recipe data
            CreateMeal(JsonArray.Parse(result));
        }

        private static void CreateMeal(JsonArray array)
        {
            foreach (var item in array)
            {
                Meal meal = new Meal();

                var obj = item.GetObject();
                foreach (var key in obj.Keys)
                {
                    IJsonValue val;
                    if (!obj.TryGetValue(key, out val))
                        continue;
                    switch (key)
                    {
                        case "id":
                            meal.id = val.GetNumber();
                            break;
                        case "content":
                            meal.name = val.GetString();
                            break;
                        case "mid":
                            Menu menu = FoodDataSource.Menus.FirstOrDefault(c => c.id == val.GetNumber());
                            if (menu != null)
                            {
                                meal.menu = menu;
                                menu.meal = meal;
                                _foodDataSource._meals.Add(meal);
                            }

                            break;
                    }
                }
            }
        }

        private static void CreateCampus(JsonArray array)
        {
            foreach (var item in array)
            {
                Campus campus = new Campus();

                var obj = item.GetObject();
                foreach (var key in obj.Keys)
                {
                    IJsonValue val;
                    if (!obj.TryGetValue(key, out val))
                        continue;
                    switch (key)
                    {
                        case "id":
                            campus.id = val.GetNumber();
                            break;
                        case "name":
                            campus.name = val.GetString();
                            break;
                        case "locations":
                            CreateLocations(val.GetArray(), campus);
                            break;
                    }
                }
                _foodDataSource.Campus.Add(campus);
            }
        }

        private static void CreateLocations(JsonArray array, Campus campus)
        {
            foreach (var item in array)
            {
                Location location = new Location();
                campus.locations.Add(location);
                location.campus = campus;

                var obj = item.GetObject();
                foreach (var key in obj.Keys)
                {
                    IJsonValue val;
                    if (!obj.TryGetValue(key, out val))
                        continue;
                    switch (key)
                    {
                        case "id":
                            location.id = val.GetNumber();
                            break;
                        case "name":
                            location.name = val.GetString();
                            break;
                        case "menus":
                            CreateMenus(val.GetArray(), location);
                            break;
                    }
                }
            }
        }

        private static void CreateMenus(JsonArray array, Location location)
        {
            foreach (var item in array)
            {
                Menu menu = new Menu();
                location.menus.Add(menu);
                menu.location = location;

                var obj = item.GetObject();
                foreach (var key in obj.Keys)
                {
                    IJsonValue val;
                    if (!obj.TryGetValue(key, out val))
                        continue;
                    switch (key)
                    {
                        case "id":
                            menu.id = val.GetNumber();
                            break;
                        case "name":
                            menu.name = val.GetString();
                            break;
                    }
                }
            }
        }
    }
}
