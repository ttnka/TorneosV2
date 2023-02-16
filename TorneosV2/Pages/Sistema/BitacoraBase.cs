using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Radzen.Blazor;
using Radzen;
using System.Xml.Linq;
using TorneosV2.Data;
using TorneosV2.Model.Clases;
using TorneosV2.Model.Repo;

namespace TorneosV2.Pages.Sistema
{
    public class BitacoraBase : ComponentBase
    {
        public const string TBita = "Bitacora";
        [Inject]
        public Repo<Z100_Org, ApplicationDbContext> OrgsRepo { get; set; } = default!;
        [Inject]
        public Repo<Z110_Usuario, ApplicationDbContext> UserRepo { get; set; } = default!;
        public List<Z110_Usuario> LosUsers { get; set; } = new List<Z110_Usuario>();

        public List<Z190_Bitacora> LasBitas { get; set; } = new List<Z190_Bitacora>();
        public RadzenDataGrid<Z190_Bitacora>? BitaGrid { get; set; } =
            new RadzenDataGrid<Z190_Bitacora>();
        public Z190_Bitacora SearchBita { get; set; } = new();

        public string LaOrgName { get; set; } = "";
        string ErrorMsn = "";
        bool PrimeraVez = true;
        protected override async Task OnInitializedAsync()
        {
            if (PrimeraVez)
            {
                PrimeraVez = false;
                await LeerElUser();
                await LeerBitacoras();
                await LeerUsers();
                await ReadOrg();
                var bitaTemp = MyFunc.MakeBitacora(ElUser.Id, ElUser.OrgId,
                    $"{TBita}, Consulto listado de bitacora", false);
                await BitaRepo.Insert(bitaTemp);
            }
        }
        public async Task ReadOrg()
        {
            try
            {
                var orgTemp = await OrgsRepo.Get(x => x.OrgId == ElUser.OrgId);
                if (orgTemp != null)
                {
                    LaOrgName = orgTemp.FirstOrDefault(x => x.Status)!.Comercial;
                }
                else
                {
                    LaOrgName = "No se encontro Nombre de la organizacion!";
                }
            }
            catch (Exception ex)
            {

                ErrorMsn = "Error en la lectura" + ex;
            }
        }

        public async Task LeerBitacoras()
        {
            ErrorMsn = "";
            IEnumerable<Z190_Bitacora> resultado = new List<Z190_Bitacora>();
            try
            {
                if (SearchBita.UserId == "Filtro")
                {
                    resultado = await BitaRepo.Get(x => x.OrgId == ElUser.OrgId &
                    x.Sistema == SearchBita.Sistema & x.Desc.Contains(SearchBita.Desc));
                }
                else
                {
                    resultado = await BitaRepo.GetAll();
                }
                if (resultado != null)
                {
                    resultado = resultado.OrderByDescending(x => x.Fecha);
                    LasBitas = resultado.ToList<Z190_Bitacora>();
                }
                else
                {
                    ErrorMsn = "No hay registros de bitacora";
                }
            }
            catch (Exception ex)
            {
                ErrorMsn = ex.Message;
                LasBitas = new();
            }
        }
        public async Task LeerUsers()
        {
            ErrorMsn = "";
            try
            {
                var resultado = await UserRepo.GetAll();
                if (resultado != null)
                {
                    LosUsers = resultado.ToList<Z110_Usuario>();
                }
                else
                {
                    ErrorMsn = "No hay registros de usuarios";
                }
            }
            catch (Exception ex)
            {
                ErrorMsn = ex.Message;
                LosUsers = new();
            }
        }

        #region Usuario y Bitacora
        [Inject]
        public UserManager<Z110_Usuario> UserMger { get; set; } = default!;
        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; } = default!;
        [CascadingParameter(Name = "ElUserAll")]
        public Z110_Usuario ElUser { get; set; } = new();
        public async Task LeerElUser()
        {
            try
            {
                var user = (await AuthStateTask).User;
                if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
                {
                    ElUser = await UserMger.GetUserAsync(user);
                }
                else
                {
                    NM.NavigateTo("Identity/Account/Login?ReturnUrl=/admin", true);
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
        [Inject]
        public Repo<Z190_Bitacora, ApplicationDbContext> BitaRepo { get; set; } = default!;
        public MyFunc MyFunc { get; set; } = new MyFunc();
        public NotificationMessage ElMsn(string tipo, string titulo, string mensaje, int duracion)
        {
            NotificationMessage respuesta = new();
            switch (tipo.ToLower())
            {
                case "Info":
                    respuesta.Severity = NotificationSeverity.Info;
                    break;
                case "Error":
                    respuesta.Severity = NotificationSeverity.Error;
                    break;
                case "Warning":
                    respuesta.Severity = NotificationSeverity.Warning;
                    break;
                default:
                    respuesta.Severity = NotificationSeverity.Success;
                    break;
            }
            respuesta.Summary = titulo;
            respuesta.Detail = mensaje;
            respuesta.Duration = 4000 + duracion;
            return respuesta;
        }
        [Inject]
        public NavigationManager NM { get; set; } = default!;

        public Z190_Bitacora LastBita { get; set; } = new();
        public async Task BitacoraAll(Z190_Bitacora bita)
        {
            if (bita.Fecha.Subtract(LastBita.Fecha).TotalSeconds > 15 ||
                LastBita.Desc != bita.Desc || LastBita.Sistema != bita.Sistema ||
                LastBita.UserId != bita.UserId || LastBita.OrgId != bita.OrgId)
            {
                LastBita = bita;
                await BitaRepo.Insert(bita);
            }
        }
        #endregion

    }
}

