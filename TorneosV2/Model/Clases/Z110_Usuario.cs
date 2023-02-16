using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TorneosV2.Model.Clases
{
    [Index(nameof(OrgId), IsUnique = false)]
    public class Z110_Usuario : IdentityUser
    {
        [StringLength(25)]
        public string? Apodo { get; set; }
        [StringLength(50)]
        public string Nombre { get; set; } = "";
        [StringLength(50)]
        public string Paterno { get; set; } = "";
        [StringLength(50)]
        public string? Materno { get; set; }
        public int Nivel { get; set; } = 1;
        [StringLength(50)]
        public string OrgId { get; set; } = "";
        public string? OldEmail { get; set; }
        public int Estado { get; set; } = 2;
        public bool Status { get; set; } = true;
    }
}
