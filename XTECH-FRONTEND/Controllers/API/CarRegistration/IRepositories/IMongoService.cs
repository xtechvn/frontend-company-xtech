using HuloToys_Service.Controllers.CarRegistration.Model;

namespace XTECH_FRONTEND.Controllers.API.CarRegistration.IRepositories
{
    public interface IMongoService
    {
        Task<long> Insert(RegistrationRecord model);
        Task<long> Insert116(RegistrationRecord model);
        List<RegistrationRecordMongo> GetList();
    }
 
}
