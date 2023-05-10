using Client.DataTypes;

namespace Client.Models
{
    public class User_group
    {
        public long Id { get; set; }
        public Group_Code code { get; set; } = Group_Code.User;
        public string? description { get; set; } = null;

        public override string ToString()
        {
            return String.Format(@"group_id: {0}, code: {1}, description: {2}", Id, code.ToString(), description);
        }
    }
}
