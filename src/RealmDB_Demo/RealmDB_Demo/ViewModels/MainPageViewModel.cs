using Plugin.LocalNotifications;
using RealmDB_Demo.Models;
using RealmDB_Demo.Settings;
using Realms;
using Realms.Sync;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RealmDB_Demo.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        Realm realm;
        public IQueryable<Store> Stores { get; private set; }

        public IQueryable<ShoppingListItem> ThingsWeDontHaveYet => SelectedStore?.ShoppingListItems.Where(sli => !sli.GotIt);

        ICommand _AddStoreCommand;
        public ICommand AddStoreCommand => _AddStoreCommand ?? (_AddStoreCommand = new Command(OnAddStoreCommand));

        ICommand _AddItemCommand;
        public ICommand AddItemCommand => _AddItemCommand ?? (_AddItemCommand = new Command(OnAddItemCommand));

        Store _SelectedStore;
        public Store SelectedStore
        {
            get
            {
                return _SelectedStore;
            }
            set
            {
                _SelectedStore = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ThingsWeDontHaveYet));
            }
        }

        ShoppingListItem _SelectedShoppingItem;
        public ShoppingListItem SelectedShoppingItem
        {
            get
            {
                return _SelectedShoppingItem;
            }
            set
            {
                _SelectedShoppingItem = value;
                if (value != null)
                {
                    Application.Current.MainPage.Navigation.PushAsync(new ShoppingItemDetailPage(value));
                    _SelectedShoppingItem = null;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set { _IsLoading = value; RaisePropertyChanged(); }
        }


        public MainPageViewModel()
        {
            Initialize();
        }

        private async Task Initialize()
        {
            IsLoading = true;
            var user = await LoginUser();

            var serverURL = new Uri(Connection.RealmServerUrl);

            var configuration = new SyncConfiguration(user, serverURL);

            realm = Realm.GetInstance(configuration);

            Stores = realm.All<Store>();

            RaisePropertyChanged(nameof(Stores));

            IDisposable token = Stores.SubscribeForNotifications((sender, changes, error) =>
            {
                if (changes?.InsertedIndices.Any() ?? false)
                    CrossLocalNotifications.Current.Show("Store added", "A new store has been added.");
            });
            IsLoading = false;
        }

        private async Task<User> LoginUser()
        {
            try
            {
                User user = null;
                User.ConfigurePersistence(UserPersistenceMode.Disabled);

                if (User.AllLoggedIn.Count() > 1)
                    foreach (var item in User.AllLoggedIn)
                    {
                        item.LogOut();
                    }

                user = User.Current;
                if (user == null)
                {
                    var credentials = Credentials.UsernamePassword(Authentication.DefaultUser,
                        Authentication.DefaultPassword, createUser: false);

                    var authURL = new Uri(Connection.AuthUrl);
                    user = await User.LoginAsync(credentials, authURL);
                }

                return user;
            }
            catch (Exception ex)
            {


            }
            return null;
        }

        private void OnAddStoreCommand(object obj)
        {
            Store s = new Store();
            s.Name = "[new]";
            realm.Write(() => realm.Add(s));
            Application.Current.MainPage.Navigation.PushAsync(new StoreDetailPage(s));
        }

        private void OnAddItemCommand(object obj)
        {
            if (SelectedStore != null)
            {
                var item = new ShoppingListItem()
                {
                    Store = SelectedStore
                };
                realm.Write(() => realm.Add(item));
                Application.Current.MainPage.Navigation.PushAsync(new ShoppingItemDetailPage(item));
            }
        }

        public void RaisePropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
