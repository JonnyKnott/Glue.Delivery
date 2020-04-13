using System.Collections.Generic;

namespace Glue.Delivery.Models.ApiModels.Auth
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public ICollection<string> Claims { get; set; } 
    }
}