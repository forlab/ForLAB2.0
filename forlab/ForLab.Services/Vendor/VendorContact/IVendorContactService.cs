using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Vendor.VendorContact;
using ForLab.Repositories.Vendor.VendorContact;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Vendor.VendorContact
{
    public interface IVendorContactService : IGService<VendorContactDto, Data.DbModels.VendorSchema.VendorContact, IVendorContactRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, VendorContactFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(VendorContactFilterDto filterDto = null);
        Task<IResponseDTO> GetVendorContactDetails(int vendorContactId);
        Task<IResponseDTO> CreateVendorContact(VendorContactDto vendorContactDto);
        Task<IResponseDTO> UpdateVendorContact(VendorContactDto vendorContactDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int vendorContactId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveVendorContact(int vendorContactId, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> ImportVendorContacts(List<VendorContactDto> vendorContactDtos, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportVendorContacts(int? pageIndex = null, int? pageSize = null, VendorContactFilterDto filterDto = null);

        // Validators methods
        bool IsNameUnique(VendorContactDto vendorContactDto);
        bool IsEmailUnique(VendorContactDto vendorContactDto);
    }
}
