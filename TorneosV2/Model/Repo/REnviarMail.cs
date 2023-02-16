using MailKit.Security;
using Microsoft.AspNetCore.Components;
using MimeKit;
using System.Text.RegularExpressions;
using TorneosV2.Data;
using TorneosV2.Model.Clases;
using TorneosV2.Model.Interface;

namespace TorneosV2.Model.Repo
{
    public class REnviarMail : IEnviarMail
    {
        private readonly ApplicationDbContext _appDbContext;
        public REnviarMail(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }
        [Inject]
        public Repo<Z190_Bitacora, ApplicationDbContext> bitacoraRepo { get; set; } = default!;
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
                apiRespuesta.Data = mailCampos;
                await WriteBitacora("vacio", "vacio", "No hay datos para enviar mail", true);
                return apiRespuesta;
            }
            if (string.IsNullOrEmpty(mailCampos.SenderEmail))
                apiRespuesta.MsnError.Add("No hay direccion de envio Sender");

            
            
            if (string.IsNullOrEmpty(mailCampos.Titulo)
                )
                apiRespuesta.MsnError.Add("No hay titulo del mail!");

            if (string.IsNullOrEmpty(mailCampos.Cuerpo))
                apiRespuesta.MsnError.Add("No hay cuerpo del mail");

            if (apiRespuesta.MsnError.Count > 0)
            {
                var texto = "";
                foreach (var me in apiRespuesta.MsnError)
                {
                    texto += me.ToString() + " ";
                }
                await WriteBitacora(mailCampos.UserId, mailCampos.OrgId, texto, true);
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

        public MyFunc MyFunc { get; set; } = new MyFunc();
        protected async Task WriteBitacora(string userId, string orgId, string desc, bool sistema)
        {
            var bitaTemp = MyFunc.MakeBitacora(userId, orgId, desc, sistema);
            await bitacoraRepo.Insert(bitaTemp);
        }
    }
}
