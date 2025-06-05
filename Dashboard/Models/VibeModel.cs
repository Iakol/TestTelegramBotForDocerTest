namespace Dashboard.Models
{
    public class VibeModel
    {
        public long Id { get; set; }
        public float vibeLevel { get; set; }
        public DateTime timeOfTest { get; set; } = DateTime.Now;
        public int userID { get; set; }
    }
}
