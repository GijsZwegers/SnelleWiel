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

        public static async Task<User> GetUserName()
        {
            return ApiFunctionClass.DeserializeJSon<User>(await ApiFunctionClass.GetDataAsync("localhost/sw/user/user?UserName=gijs"));
        }
    }
}
