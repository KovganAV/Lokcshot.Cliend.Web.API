namespace Lockshot.User.API.Class
{
    public class Hit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string WeaponType { get; set; }
        public int Score { get; set; }
        public DateTime Timestamp { get; set; }
        public double Distance { get; set; }
        public double Metrics { get; set; } 
    }
}
