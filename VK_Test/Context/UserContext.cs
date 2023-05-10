using MessagePack.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Npgsql;
using VK_Test.DataTypes;
using VK_Test.Models;

namespace VK_Test.Context
{
    public class UserContext : DbContext
    {
        private readonly string connection = System.Configuration.ConfigurationManager.AppSettings["ConnectionUsers"]!;
        public UserContext(DbContextOptions<UserContext> options)
        : base(options)
        {
        }

        public DbSet<User> UserItems { get; set; } = null!;
        private Dictionary<long, TimeOnly> UserCreation = new Dictionary<long, TimeOnly>();
        public DbSet<User_group> UserGroups { get; set; } = null!;
        public DbSet<User_state> UserStates { get; set; } = null!;

        protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                await using var dataSource = NpgsqlDataSource.Create(connection);
            }
            else
            {
                optionsBuilder.UseNpgsql(connection);
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseLazyLoadingProxies();
            }//*/
        }

        public async void DeleteUser(long id)
        {
            if (UserStates == null || UserItems == null)
                return;

            var state = await UserStates.FindAsync(UserItems.FindAsync(id).Result!.user_state_id.Id);
            state!.code = State_Code.Blocked;
            state!.description = "deleted";
        }

        public bool ExistsAdmin()
        {
            return UserGroups.Count(x => x.code == Group_Code.Admin) > 0;
        }

        public bool AddUser(User user)
        {
            if (user.user_group_id.code != Group_Code.Admin || 
                (user.user_group_id.code == Group_Code.Admin && !ExistsAdmin()))
            {
                user.user_group_id.description = user.user_group_id.code == Group_Code.Admin ? "admin" : "user";
                user.user_state_id.description = "active";
                //пользователь и время его создания
                UserCreation.Add(user.Id, TimeOnly.FromDateTime(DateTime.UtcNow));
                UserItems.Add(user);
                UserGroups.Add(user.user_group_id);
                UserStates.Add(user.user_state_id);
                return true;
            }
            return false;
        }

        public bool Validation(string login, string password)
        {
            var user = UserItems.FirstOrDefault(x => x.login == login);
            TimeOnly time;
            if (user != null && UserCreation.TryGetValue(user.Id, out time) && (TimeOnly.FromDateTime(DateTime.UtcNow) - time).TotalSeconds < 5.1)
                return false;
            return password != "" && login != "";// пока простейшая проверка на непустые пароль и логин
        }
    }
}
