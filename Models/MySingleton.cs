using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui_Project_Uppgift.Models
{
    public sealed class MySingleton
    {
        private static readonly MySingleton instance = new MySingleton();

        private List<string> items = new List<string>();

        private MySingleton()
        {
        }

        public static MySingleton Instance
        {
            get
            {
                return instance;
            }
        }

        public void AddItem(string item)
        {
            items.Add(item);
        }
        public IEnumerable<string> GetItems()
        {
            return items;
        }
    }
}
