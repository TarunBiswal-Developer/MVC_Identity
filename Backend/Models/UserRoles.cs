using System.ComponentModel.DataAnnotations.Schema;
namespace Backend.Models
{
    [Table("UserRoles")]
    public class UserRoles
    {
       public string UserId { get; set; }
        public string RoleId { get; set; }
        public string IdentityUser_Id { get; set; }
    }
}