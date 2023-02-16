namespace TorneosV2.Model.Clases
{
    public class AddUser
    {
        public string Mail { get; set; } = null!;
        public string Pass { get; set; } = null!;
        public string? ConfirmPass { get; set; }
        public string? ActualPass { get; set; }
        public string? OrgId { get; set; }
        public int Nivel { get; set; } = 1;
        public bool Positivo { get; set; } = false;
        public string? UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? UsuarioPaterno { get; set; }
        public string? UsuarioMaterno { get; set; }
        public string? UsuarioMail { get; set; }
        public string? UsuarioOrg { get; set; }
        public string? UsuarioOrgName { get; set; }
        public string? ConfirmacionCode { get; set; }
        public int UsuarioNivel { get; set; } = 1;
        public bool FirmaIn { get; set; } = false;
        public bool RecordarMe { get; set; } = false;
        public bool NewPass { get; set; } = false;
    }
}
