namespace Lockshot.User.API.Class
{
    public class Gun
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string NameGun { get; set; }
        public string WeaponType { get; set; }
        public double MaxDistance { get; set; }
        public int Bulets { get; set; }
        public string FiringMode { get; set; }
        public string Calibre { get; set; }
    }
}
