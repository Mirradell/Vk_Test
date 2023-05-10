namespace Client.Models
{
    public class User
    {
        //id, login, password, created_date, user_group_id, user_state_id
        public long Id { get; set; }
        public string login { get; set; } = "";
        public string password { get; set; } = "";
        public DateOnly created_date { get; private set; }
        public virtual User_group user_group_id { get;  set; }
        public virtual User_state user_state_id { get;  set; }

        public User() 
        {
            created_date = DateOnly.FromDateTime(DateTime.UtcNow);
            user_group_id = new User_group();
            user_state_id = new User_state(); 
        }

        public override string ToString()
        {
            return String.Format(@"User #{0}, login: {1}, password: {2}, created_date: {3}.{4}.{5}, {6}, {7}", 
                Id, login, password, created_date.Day, created_date.Month, created_date.Year, user_group_id.ToString(), user_state_id.ToString());
        }
    }
}
