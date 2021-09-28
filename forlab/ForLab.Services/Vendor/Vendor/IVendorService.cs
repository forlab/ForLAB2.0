using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Vendor.Vendor;
using ForLab.Repositories.Vendor.Vendor;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Vendor.Vendor
{
    public interface IVendorService : IGService<VendorDto, Data.DbModels.VendorSchema.Vendor, IVendorRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, VendorFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(VendorFilterDto filterDto = null);
        Task<IResponseDTO> GetVendorDetails(int vendorId);
        Task<IResponseDTO> CreateVendor(VendorDto vendorDto);
        Task<IResponseDTO> UpdateVendor(VendorDto vendorDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int vendorId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveVendor(int vendorId, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> ImportVendors(List<VendorDto> vendorDtos, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportVendors(int? pageIndex = null, int? pageSize = null, VendorFilterDto filterDto = null);
        // Validators methods
        bool IsNameUnique(VendorDto vendorDto, int LoggedInUserId, bool IsSuperAdmin);
        bool IsEmailUnique(VendorDto vendorDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> IsUsed(int vendorId);
    }
}
