using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnelleWiel.Classes
{
    public class User
    {
        public bool success { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string functie { get; set; }
    }

    
    public class RootObject
    {
        public bool success { get; set; }
        public List<User> user { get; set; }
    }
}
