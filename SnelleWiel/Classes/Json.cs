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
        public List<Order> orders { get; set; }
        public List<Ophaaladres> ophaaladres { get; set; }
        public List<Afleveradres> afleveradres { get; set; }
        public List<Result> results { get; set; }

    }
    public class Result
    {
        public Geometry geometry { get; set; }
    }
    public class Geometry
    {
        public Location location { get; set; }
    }
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Order
    {
        public Order thisorder { get { return this; } }
        public string id { get; set; }
        public string order { get; set; }
        public string chauffeur { get; set; }
        public string date { get; set; }
    }

    public class Ophaaladres
    {
        public Ophaaladres thisOphaalAdress { get { return this; } }
        public string id { get; set; }
        public string nummer { get; set; }
        public string naam { get; set; }
        public string straat { get; set; }
        public string huisnr { get; set; }
        public string plaats { get; set; }
        public string postcode { get; set; }
        public string telefoonnr { get; set; }
        public string type { get; set; }
    }

    public class Afleveradres
    {
        public Afleveradres thisAfleverAdress { get { return this; } }
        public string id { get; set; }
        public string nummer { get; set; }
        public string naam { get; set; }
        public string straat { get; set; }
        public string huisnr { get; set; }
        public string plaats { get; set; }
        public string postcode { get; set; }
        public string telefoonnr { get; set; }
        public string type { get; set; }
    }

}
