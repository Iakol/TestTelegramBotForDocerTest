using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceApi.Model
{
    public class UserModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public List<VibeModel> vibeModels { get; set; }
    }
}
