using Microsoft.EntityFrameworkCore;

namespace TorneosV2.Model.Clases
{
    [Index(nameof(OrgId), IsUnique = false)]
    public class Z190_Bitacora
    {
        [Key]
        public string BitacoraId { get; set; } = Guid.NewGuid().ToString();
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string UserId { get; set; } = "";
        public string Desc { get; set; } = "";
        public bool Sistema { get; set; } = false;
        public string OrgId { get; set; } = "";
        public string Gpo { get; set; } = string.Empty;
    }
}
