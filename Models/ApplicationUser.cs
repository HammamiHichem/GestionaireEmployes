#pragma warning disable CS8618
using Microsoft.AspNetCore.Identity;


namespace GestionaireEmployes.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
