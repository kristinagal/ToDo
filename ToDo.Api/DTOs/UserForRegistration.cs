using Microsoft.AspNetCore.Identity;

namespace ToDo.Api.DTOs
{
    public class UserForRegistration
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public UserForRegistration()
        {

        }

        public UserForRegistration(IdentityUser identity)
        { 
            UserName = identity.UserName;
            Email = identity.Email;
        }
    }
}
