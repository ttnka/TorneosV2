using TorneosV2.Model.Clases;

namespace TorneosV2.Model.Interface
{
    public interface IAddUser
    {
        Task<ApiRespuesta<AddUser>> FirmaIn(AddUser addUsuario);
        Task<ApiRespuesta<AddUser>> InsertNew(AddUser NewUser);
    }
}
