using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnelleWiel.Classes;
using SnelleWiel.api;

namespace SnelleWiel.Classes
{
    class UrlCalls
    {
        public static async Task<User> GetUser(string username, string password)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("username", username));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            return ApiFunctionClass.DeserializeJSon<User>(await ApiFunctionClass.PostDataAsync("https://stefvanderwel.nl/snellewiel/index.php/API/login", parameters));
        }
        public static async Task<dynamic> GetOrders(string user, string date)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("user", user));
            parameters.Add(new KeyValuePair<string, string>("date", date));
            dynamic lala =  ApiFunctionClass.DeserializeJSon<dynamic>(await ApiFunctionClass.PostDataAsync("https://stefvanderwel.nl/snellewiel/index.php/API/orders", parameters));
            return lala;
        }
        public static async Task<RootObject> GetOrder(string orderid)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("id", orderid));
            return ApiFunctionClass.DeserializeJSon<RootObject>(await ApiFunctionClass.PostDataAsync("https://stefvanderwel.nl/snellewiel/index.php/API/order", parameters));
        }

        public static async Task<dynamic> GetLangLong(string straat, string huisnr, string plaats, string postcode)
        {
            string test = "https://maps.googleapis.com/maps/api/geocode/json?address=" + straat + "+" + huisnr + "+" + plaats + "+" + postcode;
            return ApiFunctionClass.DeserializeJSon<dynamic>(await ApiFunctionClass.GetDataAsync(test));
        }

        public static async Task<User> GetUserName()
        {
            return ApiFunctionClass.DeserializeJSon<User>(await ApiFunctionClass.GetDataAsync("localhost/sw/user/user?UserName=gijs"));
        }
    }
}
