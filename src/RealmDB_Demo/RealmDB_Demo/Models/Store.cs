using Realms;
using System.Linq;

namespace RealmDB_Demo.Models
{
    public class Store : RealmObject
    {
        public string Name { get; set; }

        [Backlink(nameof(ShoppingListItem.Store))]
        public IQueryable<ShoppingListItem> ShoppingListItems { get; }
    }
}
