using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Client
{
    public class ClientCommands
    {
        public HttpClient client = new HttpClient();

        private JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = true
        };

        public async Task<Uri> CreateUserAsync(User user)
        {
            HttpResponseMessage responce = await client.PostAsJsonAsync("Users/", user);
            if (responce.IsSuccessStatusCode)
                Console.WriteLine("Creation: Success!");
            else
                Console.WriteLine("Creation: Something went wrong. Error: " + responce.Content.ToString());

            return responce.Headers.Location!;
        }

        public async Task<User?> GetUserAsync(long id)
        {
            HttpResponseMessage response = await client.GetAsync($"Users/{id}");
            if (response.IsSuccessStatusCode)
            {                
                User user = JsonSerializer.DeserializeAsync<User>((response.Content.ReadAsStreamAsync().Result), options).Result!;
                Console.WriteLine("Get: Success. User:\n\t" + user.ToString());
                return user;
            }

            Console.WriteLine("Get: Something went wrong. " + response.Content.ToString());
            return null;
        }

        public async Task<User?> GetUsersAsync()
        {
            HttpResponseMessage responce = await client.GetAsync("Users/");
            if (responce.IsSuccessStatusCode)
            {
                List<User> users = await JsonSerializer.DeserializeAsync<List<User>>(responce.Content.ReadAsStreamAsync().Result);
                if (users == null || users.Count == 0)
                {
                    Console.WriteLine("Get: Success, 0 users");
                    return null;
                }
                else
                {
                    foreach (var user in users)
                    {
                        Console.WriteLine("\t" + user.ToString());
                        return user;
                    }
                }
            }

            Console.WriteLine("Get: Bad request: " + responce.Content.ToString());
            return null;
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(String.Format(@"Users/{0}", user.Id), user);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Update: Bad request. " + response.Content.ToString());
                return null;
            }

            Console.WriteLine("Update: Success!");
            return user;
        }

        public async Task<HttpStatusCode> DeleteUserAsync(long id)
        {
            HttpResponseMessage response = await client.DeleteAsync(String.Format(@"Users/{0}", id));
            return response.StatusCode;
        }
    }
}