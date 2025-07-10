namespace HospitalManagementSystem.Helpers
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int LifetimeInMinutes { get; set; }
        public string SigningKey { get; set; }





    }
}
