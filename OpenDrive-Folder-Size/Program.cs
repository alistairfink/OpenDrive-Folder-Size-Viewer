using Newtonsoft.Json;
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
        static void Main(string[] args)
        {
            string SessionID = "11111111";
            if (!CheckLogin(SessionID))
            {
                while (!Login(out SessionID)) { }
            }
            while(true)
            {
                Console.WriteLine("\nEnter Folder ID (0 for root folder): ");
                string folderID = Console.ReadLine();
                int totalFolderSize = GetFolder(folderID, 0m, SessionID);
            }
        }
        private static bool Login(out string sessionID)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://dev.opendrive.com/api/v1/session/login.json";
                Console.WriteLine("\nLogin With OpenDrive Account");
                Console.Write("Username: ");
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string pass = GetHiddenConsoleInput();
                string jsonString = "{\"username\":\"" + username + "\",\"passwd\":\"" + pass + "\"}";
                JObject json = JObject.Parse(jsonString);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                Console.WriteLine("\nLogging in...");
                var result = client.PostAsync(url, content).Result;
                string resultString = result.Content.ReadAsStringAsync().Result;
                JObject jsonResponse = JObject.Parse(resultString);
                if (jsonResponse.TryGetValue("error", out JToken response))
                {
                    Console.WriteLine(response["message"]);
                }
                else if (jsonResponse.TryGetValue("SessionID", out JToken token))
                {
                    sessionID = token.ToString();
                    Console.WriteLine("Success!\n");
                    return true;
                }
            }
            sessionID = "11111111";
            return false;
        }
        private static bool CheckLogin(string SessionID)
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
        private static int GetFolder(string folderID, decimal size, string sessionID)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://dev.opendrive.com/api/v1/folder/list.json/" + sessionID + "/" + folderID;
                var result = client.GetAsync(url).Result;

                if (result.IsSuccessStatusCode)
                {
                    var response = result.Content.ReadAsStringAsync().Result;
                    OpenDriveFolderList responseFolder = JsonConvert.DeserializeObject<OpenDriveFolderList>(response);
                    foreach (File file in responseFolder.Files)
                    {
                        decimal fileSize = decimal.Parse(file.Size);
                        fileSize /= 1024;
                        //size += file.Size;
                    }
                }
                else
                {
                    return 0;
                }
            }
                return 0;
        }
    }
}
