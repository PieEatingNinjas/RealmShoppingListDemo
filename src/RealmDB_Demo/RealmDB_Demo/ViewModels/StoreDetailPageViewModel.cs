using RealmDB_Demo.Models;

namespace RealmDB_Demo.ViewModels
{
    public class StoreDetailPageViewModel
    {
        public Store Store { get; }

        public StoreDetailPageViewModel(Store store)
        {
            Store = store;
        }
    }
}
