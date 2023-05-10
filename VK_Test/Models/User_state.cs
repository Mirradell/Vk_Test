using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using VK_Test.DataTypes;

namespace VK_Test.Models
{
    public class User_state
    {
        public long Id { get; set; }
        public State_Code code { get; set; } = State_Code.Active;
        public string? description { get; set; } = null;

        public override string ToString()
        {
            return String.Format(@"state_id: {0}, code: {1}, description: {2}", Id, code.ToString(), description);
        }
    }
}
