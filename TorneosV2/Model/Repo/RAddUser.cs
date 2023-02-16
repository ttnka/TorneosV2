using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using TorneosV2.Data;
using TorneosV2.Model.Clases;
using TorneosV2.Model.Interface;
using MailKit.Security;
using MimeKit;

namespace TorneosV2.Model.Repo
{
    public class RAddUser : IAddUser
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<Z110_Usuario> _userManager;
        private readonly IUserStore<Z110_Usuario> _userStore;
        private readonly SignInManager<Z110_Usuario> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IUserEmailStore<Z110_Usuario> _emailStore;
        private readonly NavigationManager _navigationManager;

        public RAddUser(ApplicationDbContext appDbContext,
            UserManager<Z110_Usuario> userManager,
            IUserStore<Z110_Usuario> userStore,
            SignInManager<Z110_Usuario> signInManager,
            IEmailSender emailSender,
            NavigationManager navigationManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _emailSender = emailSender;
            _navigationManager = navigationManager;
        }

        public async Task<ApiRespuesta<AddUser>> FirmaIn(AddUser addUsuario)
        {
            ApiRespuesta<AddUser> apiRespuesta = new()
            { Exito = false, Data = addUsuario };
            string texto = "";

            {
                try
                {

                    var resultado = await _signInManager.PasswordSignInAsync(
                        addUsuario.Mail, addUsuario.Pass, addUsuario.RecordarMe,
                        lockoutOnFailure: false);

                    addUsuario.Positivo = resultado.Succeeded;

                    apiRespuesta.Exito = true;
                    texto = $"Firmo con exito {addUsuario.Mail}";
                    await WriteBitacora("Vacio", "Vacio", texto, true);
                    return apiRespuesta;
                }
                catch (Exception ex)
                {
                    apiRespuesta.MsnError = new List<string> { ex.Message };
                    texto = $"Intento ingresar {addUsuario.Mail} pass XXX ";
                    await WriteBitacora("Vacio", "Vacio", texto, false);
                    return apiRespuesta;
                }
            }

        }

        public async Task<ApiRespuesta<AddUser>> InsertNew(AddUser NewUser)
        {
            ApiRespuesta<AddUser> apirespuesta = new() { Exito = false };
            //string txt = "";

            try
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, NewUser.Mail, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, NewUser.Mail, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, NewUser.Pass);
                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);

                    user.Nombre = NewUser.UsuarioNombre ?? "";
                    user.Paterno = NewUser.UsuarioPaterno ?? "";
                    user.Materno = NewUser.UsuarioMaterno ?? "";

                    var userUpDate = _appDbContext.Usuarios.FirstOrDefault(
                        x => x.Id == userId);
                    userUpDate.Nombre = user.Nombre;
                    userUpDate.Paterno = user.Paterno;
                    userUpDate.Materno = user.Materno;
                    userUpDate.OldEmail = user.Email;
                    userUpDate.OrgId = NewUser.OrgId;
                    userUpDate.Nivel = NewUser.Nivel;
                    var sav1 = await _appDbContext.SaveChangesAsync();

                    NewUser.UsuarioId = NewUser.UsuarioId ?? userId;
                    NewUser.UsuarioOrg = NewUser.UsuarioOrg ?? NewUser.OrgId;

                    var txt = $"Se creo nuevo acceso {NewUser.Mail} y password, ";
                    txt += $"{user.Nombre} {user.Paterno} {NewUser.UsuarioOrgName}";

                    await WriteBitacora(NewUser.UsuarioId, NewUser.UsuarioOrg, txt, false);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var url = ($"https://uniformes.datason.com.mx/Account/ConfirmEmail/Id={userId}&code={code}");

                    MailCampos mailCampos = new MailCampos();
                    string elCuerpo = "<label>Hola !</label> <br /><br />";
                    elCuerpo += $"<label>Te escribimos de {Constantes.ElDominio} para enviarte un correo de confirmacion de cuenta </label><br />";
                    elCuerpo += $"<label>por favor confirma tu Cuenta de correo ingresando al siguiente enlace:</label><br />";
                    elCuerpo += $"<a href={url}>Confirma tu Cuenta</a> <br />";

                    MailCampos datos = new MailCampos();
                    datos = mailCampos.PoblarMail(NewUser.Mail, "Confirma Tu correo!", elCuerpo,
                        NewUser.Mail, userId, NewUser.OrgId, Constantes.DeNombreMail01,
                        Constantes.DeMail01, Constantes.ServerMail01, Constantes.PortMail01,
                        Constantes.UserNameMail01, Constantes.PasswordMail01);

                    var mails = await EnviarMail(datos);
                    if (!mails.Exito)
                        await WriteBitacora(NewUser.UsuarioId, NewUser.OrgId,
                            $"Nuevo User, No fue posible enviar un correo a {mails.MsnError}", true);
                }
                else
                {
                    await WriteBitacora(NewUser.UsuarioId, NewUser.UsuarioOrg,
                        $"No creo nuevo usuario!!! {NewUser.Mail}", true);
                }
                apirespuesta.Exito = true;
                apirespuesta.Data = NewUser;
                return apirespuesta;
            }
            catch (Exception ex)
            {
                apirespuesta.MsnError = new List<string>() { ex.Message };
                return apirespuesta;
            }
        }

        private Z110_Usuario CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Z110_Usuario>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"No pude creear un nuevo usuario de '{nameof(Z110_Usuario)}'. ");
            }
        }

        private IUserEmailStore<Z110_Usuario> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Se requiere soporte de correo electronico!.");
            }
            return (IUserEmailStore<Z110_Usuario>)_userStore;
        }

        public MyFunc MyFunc { get; set; } = new();
        public async Task WriteBitacora(string uId, string oId, string d, bool s)
        {
            var bitaTemp = MyFunc.MakeBitacora(uId, oId, d, s);
            await _appDbContext.Bitacora.AddAsync(bitaTemp);
            await _appDbContext.SaveChangesAsync();

        }

        public async Task<ApiRespuesta<MailCampos>> EnviarMail(MailCampos mailCampos)
        {
            ApiRespuesta<MailCampos> apiRespuesta = new()
            {
                Exito = false,
                MsnError = new List<string>(),
                Data = mailCampos
            };
            #region EVALUAR Info de envio y cuentas
            if (mailCampos == null)
            {
                apiRespuesta.Exito = false;
                apiRespuesta.MsnError.Add("No hay datos para enviar mail!");
                apiRespuesta.Data = new MailCampos();

                return apiRespuesta;
            }
            if (string.IsNullOrEmpty(mailCampos.SenderEmail))
                apiRespuesta.MsnError.Add("No hay direccion de envio Sender");

            
            if (string.IsNullOrEmpty(mailCampos.Titulo))
                apiRespuesta.MsnError.Add("No hay titulo del mail!");

            if (string.IsNullOrEmpty(mailCampos.Cuerpo))
                apiRespuesta.MsnError.Add("No hay cuerpo del mail");

            if (apiRespuesta.MsnError.Count > 0)
            {
                return apiRespuesta;
            }

            #endregion
            #region Evaluar si es correo de pruebas
            if (mailCampos.Para.EndsWith(".com1") ||
                mailCampos.Para == Constantes.DeMail01)
            {
                apiRespuesta.Exito = true;
                apiRespuesta.MsnError.Add("Email de prueba exitos!");
                await WriteBitacora(mailCampos.UserId, mailCampos.OrgId,
                    "Se supespendio el envio de mail ya que es un correo de prueba!", true);
                return apiRespuesta;
            }
            #endregion
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(mailCampos.SenderEmail));
                email.To.Add(MailboxAddress.Parse(mailCampos.Para));
                email.Subject = mailCampos.Titulo;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = mailCampos.Cuerpo };
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect(mailCampos.Server, mailCampos.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailCampos.UserName, mailCampos.Password);
                smtp.Send(email);
                smtp.Disconnect(true);

                await WriteBitacora(mailCampos.UserId, mailCampos.OrgId,
                    $"Se envio un Email a {mailCampos.Para} Titulo {mailCampos.Titulo}", true);
                apiRespuesta.Exito = true;
                return apiRespuesta;
            }
            catch (Exception ex)
            {
                apiRespuesta.MsnError.Add(ex.Message);
                var text = $"Hubo un error al enviar MAIL {ex.Message} Para {mailCampos.Para} ";
                text += $"Titulo {mailCampos.Titulo} ";
                await WriteBitacora(mailCampos.UserId, mailCampos.OrgId, text, true);
                return apiRespuesta;
            }
        }

    }
}
