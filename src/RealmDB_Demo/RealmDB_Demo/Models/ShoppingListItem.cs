using Realms;

namespace RealmDB_Demo.Models
{
    public class ShoppingListItem : RealmObject
    { 
        public string Name { get; set; }

        public Store Store { get; set; }

        public string Description { get; set; }

        public bool GotIt { get; set; }
    }
}
