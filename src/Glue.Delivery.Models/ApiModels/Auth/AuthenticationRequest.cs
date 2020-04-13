namespace Glue.Delivery.Models.ApiModels.Auth
{
    public class AuthenticationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }
}