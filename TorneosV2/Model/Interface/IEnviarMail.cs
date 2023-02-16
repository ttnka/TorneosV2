using TorneosV2.Model.Clases;

namespace TorneosV2.Model.Interface
{
    public interface IEnviarMail
    {
        Task<ApiRespuesta<MailCampos>> EnviarMail(MailCampos mailCampos);
    }
}
