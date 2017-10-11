using Plugin.LocalNotifications;
using RealmDB_Demo.Models;
using Realms;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace RealmDB_Demo.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        readonly Realm realm;
        public IQueryable<Store> Stores { get; }

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

        public MainPageViewModel()
        {
            byte[] encryptionKey = new byte[64];
            Encoding.UTF8.GetBytes("My super dooper encryption key").CopyTo(encryptionKey, 0);
            var config = new RealmConfiguration("MyEncryptedRealm.db"); 
            config.EncryptionKey = encryptionKey;

            realm = Realm.GetInstance(config);
            Stores = realm.All<Store>();

            IDisposable token = Stores.SubscribeForNotifications((sender, changes, error) =>
            {
                if (changes?.InsertedIndices.Any() ?? false)
                    CrossLocalNotifications.Current.Show("Store added", "A new store has been added.");
            });
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
            if(SelectedStore != null)
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
