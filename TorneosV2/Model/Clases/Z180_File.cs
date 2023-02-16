using System.ComponentModel.DataAnnotations;

namespace TorneosV2.Model.Clases
{
    public class Z180_File
    {
        [Key]
        [StringLength(50)]
        public string FileId { get; set; } = Guid.NewGuid().ToString();
        public DateTime Fecha { get; set; } = DateTime.Now;
        [StringLength(50)]
        public string Fuente { get; set; } = null!;
        public string FuenteId { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Archivo { get; set; } = null!;
        [StringLength(50)]
        public string? Gpo { get; set; }
        [StringLength(50)]
        public string Org { get; set; } = null!;
        public int Estado { get; set; } = 2;
        public bool Status { get; set; } = true;
    }
}
