using System.Threading.Tasks;
using EPlast.BLL.DTO.Admin;

namespace EPlast.BLL.Interfaces.Admin
{
    public interface IAdminTypeService
    {
        Task<AdminTypeDTO> GetAdminTypeByNameAsync(string name);
        Task<AdminTypeDTO> GetAdminTypeByIdAsync(int adminTypeId);
        Task<AdminTypeDTO> CreateByNameAsync(string adminTypeName);
        Task<AdminTypeDTO> CreateAsync(AdminTypeDTO adminTypeDto);
    }
}