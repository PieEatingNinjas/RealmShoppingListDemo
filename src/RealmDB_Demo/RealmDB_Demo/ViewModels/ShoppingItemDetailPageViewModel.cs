using RealmDB_Demo.Models;

namespace RealmDB_Demo.ViewModels
{
    internal class ShoppingItemDetailPageViewModel
    {
        public ShoppingListItem Item { get; }

        public ShoppingItemDetailPageViewModel(ShoppingListItem item)
        {
            this.Item = item;
        }
    }
}