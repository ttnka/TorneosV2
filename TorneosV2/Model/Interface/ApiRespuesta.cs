using Radzen;

namespace TorneosV2.Model.Interface
{
    public class ApiRespuesta<TEntity> where TEntity : class
    {
        public bool Exito { get; set; }
        public List<string> MsnError { get; set; } = new List<string>();
        public TEntity Data { get; set; }
    }
}
