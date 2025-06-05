using Microsoft.EntityFrameworkCore;

namespace ServiceApi.Model
{
    [PrimaryKey(nameof(Id))]
    public class UserModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public List<VibeModel> vibeModels { get; set; }
    }
}
