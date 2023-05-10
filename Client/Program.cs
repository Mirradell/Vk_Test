using Client;
using System.Net.Http.Headers;
using Client.Models;

static class Program
{
    static ClientCommands commands;
    static HttpClient client;

    static Program()
    {
        commands = new ClientCommands();
        client = commands.client;
    }

    static void Main()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    static async Task RunAsync()
    {
        // Update port # in the following line.
        client.BaseAddress = new Uri("https://localhost:7229/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            // Create a new product
            User? user = new User() { login = "111", password = "222" };

            var url = new Uri("https://localhost:7229/Users/22"); //await commands.CreateUserAsync(user); 
            Console.WriteLine($"Created at {url}");

            // Get the product
            user = await commands.GetUserAsync(long.Parse(url.Segments.Last()));
            //ShowProduct(user);

            // Update the product
            Console.WriteLine("Updating password...");
            user!.password = "new_password";
            await commands.UpdateUserAsync(user);

            // Get the updated product
            user = await commands.GetUserAsync(user.Id);

            // Delete the product
            var statusCode = await commands.DeleteUserAsync(user!.Id);
            Console.WriteLine($"Deleted: (HTTP Status = {(int)statusCode})");

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadKey();
    }
}