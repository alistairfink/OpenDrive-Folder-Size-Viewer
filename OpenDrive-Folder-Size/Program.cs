using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenDrive_Folder_Size
{
    class Program
    {
        private static string SessionID;
        static void Main(string[] args)
        {
            SessionID = "11111111";
            if (!CheckLogin())
            {
                while (!Login()) { }
            }
            while(true)
            {
                
            }
        }
        private static bool Login()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://dev.opendrive.com/api/v1/session/login.json";
                Console.WriteLine("\nLogin With OpenDrive Account");
                Console.Write("Username: ");
                string username = System.Console.ReadLine();
                Console.Write("Password: ");
                string pass = GetHiddenConsoleInput();
                string jsonString = "{\"username\":\"" + username + "\",\"passwd\":\"" + pass + "\"}";
                JObject json = JObject.Parse(jsonString);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;
                string resultString = result.Content.ReadAsStringAsync().Result;
                JObject jsonResponse = JObject.Parse(resultString);

            }
            return false;
        }
        private static bool CheckLogin()
        {
            using(HttpClient client = new HttpClient())
            {
                Console.WriteLine("Checking Login...");
                string url = "https://dev.opendrive.com/api/v1/session/exists.json";
                string jsonString = "{\"session_id\":\"" + SessionID + "\"}";
                JObject json = JObject.Parse(jsonString);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;
                string resultString = result.Content.ReadAsStringAsync().Result;
                JObject jsonResponse = JObject.Parse(resultString);
                if (jsonResponse.TryGetValue("result", out JToken response))
                {
                    return bool.Parse(response.ToString());
                }
            }
            return false;
        }
        private static string GetHiddenConsoleInput()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
            }
            return input.ToString();
        }
    }
}
