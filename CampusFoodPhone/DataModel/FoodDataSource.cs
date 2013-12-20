using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;

namespace CampusFood.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class FoodCommon : CampusFood.Common.BindableBase
    {
        private double _id = 0;
         
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

        public Campus()
        {
            _locations.CollectionChanged += NotifyCollectionChangedEventHandler;
        }

        private void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Description");
        }
        
        public ObservableCollection<Location> locations
        {
            get { return this._locations; }
        }

        private bool _active;

        public int Count
        {
            get { return _locations.Count; }
        }

        public bool Active
        {
            get { return _active; }
            set { this.SetProperty(ref _active, value); }
        }

        public String Description
        {
            get { return String.Format("{0} ({1})", this.name, _locations.Count); }
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


        public Meal createMeal()
        {
            Meal meal = new Meal();
            meal.mid = this.id;
            meal.name = this.ToString();
            return meal;
        }
    }

    public class Meal : FoodCommon
    {
        private double _mid;
        
        public double mid
        {
            get { return this._mid; }
            set { this.SetProperty(ref this._mid, value); }
        }

        private string _content = string.Empty;

        public string content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }


        public void UpdateWith(Meal meal)
        {
            this.content = meal.content;
            this.id = meal.id;
        }
    }

    public sealed class FoodDataSource
    {

        private ObservableCollection<Meal> _meals = new ObservableCollection<Meal>();
        public static ObservableCollection<Meal> Meals
        {
            get
            {
                return _foodDataSource._meals;
            }
            set
            {
                _foodDataSource._meals.Clear();
                foreach(Meal m in value){
                _foodDataSource._meals.Add(m);
                }
            }
        }

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
                me.content = "Jus d'orange\nPoitrine de poulet (BRA)\naux herbettes\nRiz créole\nSalade ou pomme\nBoulangerie";
                me.name = m1.ToString();
                m1.meal = me;

                meals.Add(me);
                meals.Add(me);
                meals.Add(me);
                meals.Add(me);
                meals.Add(me);

                return meals;
            }
        }
        public ObservableCollection<Campus> FakeCampus
        {
            get
            {
                var campus = new ObservableCollection<Campus>();
                Campus c = new Campus();
                c.id = 1;
                c.name = "UNIL";
                Location l = new Location();
                c.locations.Add(l);
                c.locations.Add(l);
                campus.Add(c);
                c = new Campus();
                c.id = 2;
                c.name = "EPFL";
                l = new Location();
                c.locations.Add(l);
                c.locations.Add(l);
                campus.Add(c);

                return campus;
            }
        }

        public ObservableCollection<Location> FakeLocations
        {
            get
            {
                Random rnd = new Random();
                var ls = new ObservableCollection<Location>();
                for (var j = 1; j < rnd.Next(3, 5); j++)
                {
                    Location l = new Location();
                    l.name = "Batiment " + j;
                    for (var i = 1; i < rnd.Next(3, 7); i++)
                    {
                        Menu m = new Menu();
                        m.name = "Assiette " + i;
                        l.menus.Add(m);
                    }
                    ls.Add(l);
                }

                return ls;
            }
        }

        const string filename = "data.xml";

        public FoodDataSource(){
            _meals.CollectionChanged += (a, e) =>
            {
                FoodDataSource.XMLSerialize();
            };
        }

        public static async Task XMLSerialize()
        {
            StorageFile newFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            Stream newFileStream = await newFile.OpenStreamForWriteAsync();
            DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<Meal>));
            ser.WriteObject(newFileStream, FoodDataSource.Meals);
            newFileStream.Dispose();
            var test = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        }

        public static async Task XMLDeserialize()
        {
            try
            {
                StorageFile newFile = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                Stream newFileStream = await newFile.OpenStreamForReadAsync();
                DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<Meal>));
                FoodDataSource.Meals = (ObservableCollection<Meal>)ser.ReadObject(newFileStream);
                newFileStream.Dispose();
                
            }
            catch(Exception)
            {
                FoodDataSource.Meals = new ObservableCollection<Meal>();
            }
        }

        private static FoodDataSource _foodDataSource = new FoodDataSource();

        private ObservableCollection<Campus> _campus = new ObservableCollection<Campus>();
        public static ObservableCollection<Campus> Campus
        {
            get { return _foodDataSource._campus; }
        }
               
        public static IEnumerable<Location> Locations
        {
            get { return FoodDataSource.Campus.SelectMany(campus => campus.locations); }
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

        private static String MidList
        {
            get
            {
                return String.Join(",", FoodDataSource.Meals.Select<Meal, Int32>(meal => Convert.ToInt32(meal.mid)));
            }
        }

        public static async Task LoadRemoteMealsAsync()
        {
            //TODO: handle date
            //TODO: handle cachedate
            try
            {
                var date = DateTime.Today.ToString("yyyy-MM-dd");

                // Retrieve recipe data from Azure
                var client = new HttpClient();
                client.MaxResponseContentBufferSize = 1024 * 1024; // Read up to 1 MB of data
                var response = await client.GetAsync(new Uri("https://isisvn.unil.ch/campusfood/api/meals/" + date + "/?m=" + FoodDataSource.MidList));
                var result = await response.Content.ReadAsStringAsync();

                // Parse the JSON recipe data
                UpdateMeals(JsonConvert.DeserializeObject(result));
                await FoodDataSource.XMLSerialize();
            }
            catch (Exception)
            {

            }
        }

        private static void UpdateMeals(JsonArray array)
        {
            foreach (var item in array)
            {
                Meal meal = new Meal();
                try
                {
                    var obj = item.GetObject();
                    foreach (var key in obj.Keys)
                    {
                        IJsonValue val;
                        if (!obj.TryGetValue(key, out val))
                            continue;
                        switch (key)
                        {
                            case "id":
                                try
                                {
                                    meal.id = val.GetNumber();
                                }
                                catch (Exception e)
                                {

                                }
                                break;
                            case "content":
                                meal.content = val.GetString();
                                break;
                            case "mid":
                                meal.mid = val.GetNumber();
                                break;
                        }
                    }
                    FoodDataSource.Meals.First(m => m.mid == meal.mid).UpdateWith(meal);
                }
                catch (Exception e)
                {

                }
            }
        }

        private static void CreateCampus(JsonArray array)
        {
            FoodDataSource.Campus.Clear();
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
                FoodDataSource.Campus.Add(campus);
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
