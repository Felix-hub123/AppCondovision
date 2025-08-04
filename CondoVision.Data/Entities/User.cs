using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CondoVision.Data.Entities
{
    public enum UserType
    {
        CompanyAdministrator,
        CondominiumManager,
        Resident,
        Employee
    }

    public class User : IdentityUser 
    {
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(20)]
        public string? TaxId { get; set; }

       
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

       
        public int? UnitId { get; set; }
        [ForeignKey("UnitId")]
        public Unit? Unit { get; set; }

        public UserType UserType { get; set; }
    }
}
