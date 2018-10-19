using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;

namespace OpenDrive_Folder_Size
{
    class Program
    {
        static void Main(string[] args)
        {
            string SessionID = "11111111";
            if (!CheckLogin(SessionID))
            {
                while (!Login(ref SessionID)) { }
            }
            while(true)
            {
                Console.WriteLine("\nEnter Folder ID (0 for root folder): ");
                string folderID = Console.ReadLine();
                Console.WriteLine("Scanning Folders...\n");
                decimal totalFolderSize = GetFolder(folderID, 0, ref SessionID);
                Console.WriteLine("Folder Size: " + string.Format("{0:F2}", totalFolderSize) + " MB");
            }
        }
        private static bool Login(ref string sessionID)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://dev.opendrive.com/api/v1/session/login.json";
                Console.WriteLine("Login With OpenDrive Account");
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
        private static decimal GetFolder(string folderID, int indent, ref string sessionID)
        {
            decimal size = 0m;
            if(!CheckLogin(sessionID))
            {
                Console.WriteLine("ERROR: Session Expired. Please Re-Enter Credentials");
                Login(ref sessionID);
            }
            using (HttpClient client = new HttpClient())
            {
                string url = "https://dev.opendrive.com/api/v1/folder/list.json/" + sessionID + "/" + folderID;
                var result = client.GetAsync(url).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = result.Content.ReadAsStringAsync().Result;
                    OpenDriveFolderList responseFolder = JsonConvert.DeserializeObject<OpenDriveFolderList>(response);
                    Console.WriteLine(GetIndent(indent) + responseFolder.Name);
                    foreach (File file in responseFolder.Files)
                    {
                        decimal fileSize = decimal.Parse(file.Size);
                        fileSize /= 1048576;
                        size += fileSize;
                    }
                    foreach (Folder folder in responseFolder.Folders)
                    {
                        size += GetFolder(folder.FolderID, indent+1, ref sessionID);
                    }
                    return size;
                }
                Console.WriteLine("ERROR: Server could not be reached.");
                return 0;
            }
        }
        private static string GetIndent(int indent)
        {
            string returnString = indent > 1 ? "|" : "";
            for(int i = 1; i<indent; i++)
            {
                returnString += "  ";
            }
            if (indent > 0)
            {
                returnString += "|-- ";
            }
            return returnString;
        }
    }
}
