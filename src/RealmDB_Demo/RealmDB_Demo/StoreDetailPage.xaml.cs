using RealmDB_Demo.Models;
using RealmDB_Demo.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealmDB_Demo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreDetailPage : ContentPage
    {
        public StoreDetailPage(Store store)
        {
            this.BindingContext = new StoreDetailPageViewModel(store);
            InitializeComponent();
        }
    }
}