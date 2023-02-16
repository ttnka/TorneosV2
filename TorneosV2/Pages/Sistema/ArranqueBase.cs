using Microsoft.AspNetCore.Components;
using TorneosV2.Data;
using TorneosV2.Model.Clases;
using TorneosV2.Model.Interface;
using TorneosV2.Model.Repo;

namespace TorneosV2.Pages.Sistema
{
    public class ArranqueBase : ComponentBase
    {
        public const string TBita = "Arranque";
        [Inject]
        public Repo<Z100_Org, ApplicationDbContext> OrgsRepo { get; set; } = default!;

        [Inject]
        public IAddUser AddUserRepo { get; set; } = default!;
        [Inject]
        public Repo<Z110_Usuario, ApplicationDbContext> UserRepo { get; set; } = default!;
        [Inject]
        public Repo<Z190_Bitacora, ApplicationDbContext> BitacoraRepo { get; set; } = default!;
        [Inject]
        public NavigationManager NM { get; set; } = default!;
        public bool Editando = false;
        protected override async Task OnInitializedAsync()
        {
            var emp1 = await OrgsRepo.Get(x => x.Rfc == Constantes.PgRfc);
            var emp2 = await OrgsRepo.Get(x => x.Rfc == Constantes.SyRfc);
            if ((emp1 != null && emp1.Count() > 0) && (emp2 != null && emp2.Count() > 0)) { NM.NavigateTo("/", true); }
        }

        protected async Task RunInicio()
        {
            Editando = true;
            if (Clave.Pass == Constantes.Arranque)
            {
                Editando = false;
                await Creacion();
            }
            Clave.Pass = "";
            NM.NavigateTo("/");
        }
        protected void PassW()
        {
            if (Clave.Pass == Constantes.Arranque)
                Editando = true;
        }
        protected async Task<bool> Creacion()
        {
            var resultado = await OrgsRepo.GetAll();
            if (resultado == null || resultado.Count() > 1) return false;
            try
            {
                // Genera una nueva organizacion con datos sistema 
                Z100_Org SysOrg = new();
                SysOrg.Rfc = Constantes.SyRfc;
                SysOrg.Comercial = Constantes.SyRazonSocial;
                SysOrg.RazonSocial = Constantes.SyRazonSocial;
                SysOrg.Estado = Constantes.SyEstado;
                SysOrg.Status = Constantes.SyStatus;
                SysOrg.Tipo = "Administracion";
                var newSysOrg = await OrgsRepo.Insert(SysOrg);



                // Genera un nuevo acceso al sistema con un usuario
                AddUser eAddUsuario = new();
                eAddUsuario.Mail = Constantes.SyMail;
                eAddUsuario.Pass = Constantes.SysPassword;
                eAddUsuario.OrgId = newSysOrg.OrgId;
                eAddUsuario.UsuarioNombre = "El WebMaster";
                eAddUsuario.UsuarioPaterno = "Zuver";
                eAddUsuario.UsuarioMaterno = "Inc";
                eAddUsuario.Nivel = 7;

                var userNew = await AddUserRepo.InsertNew(eAddUsuario);

                // Genera una organizacion nueva para publico en general
                Z100_Org PgOrg = new();
                PgOrg.Rfc = Constantes.PgRfc;
                PgOrg.Comercial = Constantes.PgRazonSocial;
                PgOrg.RazonSocial = Constantes.PgRazonSocial;
                PgOrg.Estado = Constantes.PgEstado;
                PgOrg.Status = Constantes.PgStatus;
                PgOrg.Tipo = "Administracion";
                var newPgOrg = await OrgsRepo.Insert(PgOrg);


                // Genera acceso para publico en general todos 
                AddUser eAddUsuarioPublico = new();
                eAddUsuarioPublico.Mail = Constantes.DeMailPublico;
                eAddUsuarioPublico.Pass = Constantes.PasswordMailPublico;
                eAddUsuarioPublico.OrgId = newPgOrg.OrgId;
                eAddUsuarioPublico.UsuarioNombre = "Publico";
                eAddUsuarioPublico.UsuarioPaterno = "General";
                eAddUsuarioPublico.UsuarioMaterno = "S/F";
                eAddUsuarioPublico.Nivel = Constantes.NivelPublico;
                var userNewPublico = await AddUserRepo.InsertNew(eAddUsuarioPublico);

                var bitaTemp = MyFunc.MakeBitacora(userNew.Data.UsuarioId!, userNew.Data.OrgId!,
                    $"{TBita}, Se las tablas por primera vez", true);
                await BitacoraRepo.Insert(bitaTemp);
                return true;
            }
            catch (Exception ex)
            {
                var msn = ex.Message;
                return false;

            }
        }

        public class LaClave
        {
            public string Pass { get; set; } = "";
        }
        public LaClave Clave { get; set; } = new();
        public MyFunc MyFunc { get; set; } = new();
    }
}
