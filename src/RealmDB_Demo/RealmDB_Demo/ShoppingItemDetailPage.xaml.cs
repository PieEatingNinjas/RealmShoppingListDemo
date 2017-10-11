using RealmDB_Demo.Models;
using RealmDB_Demo.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealmDB_Demo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShoppingItemDetailPage : ContentPage
    {
        public ShoppingItemDetailPage(ShoppingListItem item)
        {
            BindingContext = new ShoppingItemDetailPageViewModel(item);
            InitializeComponent();
        }
    }
}