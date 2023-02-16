using Radzen;

namespace TorneosV2.Model.Interface
{
    public class ApiRespuestas<TEntity> where TEntity : class
    {
        public bool Exito { get; set; }
        public List<string> MsnError { get; set; } = new List<string>();
        public IEnumerable<TEntity> Data { get; set; }
    }
}
