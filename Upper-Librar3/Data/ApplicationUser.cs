using Microsoft.AspNetCore.Identity;

namespace library.Data
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
    }
}
