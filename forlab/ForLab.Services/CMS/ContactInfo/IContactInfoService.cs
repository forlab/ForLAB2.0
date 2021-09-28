using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.ContactInfo;
using ForLab.Repositories.CMS.ContactInfo;
using ForLab.Services.Generics;
using System.Threading.Tasks;

namespace ForLab.Services.CMS.ContactInfo
{
    public interface IContactInfoService : IGService<ContactInfoDto, Data.DbModels.CMSSchema.ContactInfo, IContactInfoRepository>
    {
        Task<IResponseDTO> GetContactInfoDetails();
        Task<IResponseDTO> UpdateContactInfo(ContactInfoDto contactInfoDto);

        // Validators methods
         bool IsUniqueLatlng(ContactInfoDto contactInfoDto);
        bool IsEmailUnique(ContactInfoDto contactInfoDto);
    }
}
