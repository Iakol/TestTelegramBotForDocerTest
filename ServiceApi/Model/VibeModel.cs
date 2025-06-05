using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceApi.Model
{
    [PrimaryKey(nameof(Id))]
    public class VibeModel
    {
        public long Id { get; set; }
        public float vibeLevel { get; set; }
        public DateTime timeOfTest { get; set; } = DateTime.Now;

        public UserModel user { get; set; }

        [ForeignKey(nameof(user))]
        public long userID { get; set; }
    }
}
