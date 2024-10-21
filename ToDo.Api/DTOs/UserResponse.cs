using Microsoft.AspNetCore.Identity;

namespace ToDo.Api.DTOs
{
    public class UserResponse : UserForRegistration
    {
        public string? Id { get; set; }

        public UserResponse()
        {
        }

        public UserResponse(IdentityUser identity): base(identity)
        {
            Id = identity.Id;
        }
    }
}
