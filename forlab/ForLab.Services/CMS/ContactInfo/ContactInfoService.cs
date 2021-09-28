using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.CMS.ContactInfo;
using ForLab.Repositories.CMS.ContactInfo;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ForLab.Services.Global.UploadFiles;

namespace ForLab.Services.CMS.ContactInfo
{
    public class ContactInfoService : GService<ContactInfoDto, Data.DbModels.CMSSchema.ContactInfo, IContactInfoRepository>, IContactInfoService
    {
        private readonly IContactInfoRepository _contactInfoRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IUploadFilesService _uploadFilesService;

        public ContactInfoService(IMapper mapper,
            IResponseDTO response,
            IContactInfoRepository contactInfoRepository,
             IUploadFilesService uploadFilesService,
            IUnitOfWork<AppDbContext> unitOfWork) : base(contactInfoRepository, mapper)
        {
            _contactInfoRepository = contactInfoRepository;
            _response = response;
            _uploadFilesService = uploadFilesService;
            _unitOfWork = unitOfWork;
        }

       
        public async Task<IResponseDTO> GetContactInfoDetails()
        {
            try
            {
                var contactInfo = await _contactInfoRepository.GetAll()
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync();
                if (contactInfo == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var contactInfoDto = _mapper.Map<ContactInfoDto>(contactInfo);

                _response.Data = contactInfoDto;
                _response.Message = "Ok";
                _response.IsPassed = true;
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public async Task<IResponseDTO> UpdateContactInfo(ContactInfoDto contactInfoDto)
        {
            try
            {
                var contactInfoExist = await _contactInfoRepository.GetFirstAsync(x => x.Id == contactInfoDto.Id);
                if (contactInfoExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var contactInfo = _mapper.Map<Data.DbModels.CMSSchema.ContactInfo>(contactInfoDto);
                _contactInfoRepository.Update(contactInfo);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.Data = null;
                _response.IsPassed = true;
                _response.Message = "Ok";

            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        
        // Validators methods
        public bool IsUniqueLatlng(ContactInfoDto contactInfoDto)
        {
            var searchResult = _contactInfoRepository.GetAll(x =>
                                              !x.IsDeleted
                                              && x.Id != contactInfoDto.Id
                                              && x.Longitude.ToLower().Trim() == contactInfoDto.Longitude.ToLower().Trim()
                                              && x.Latitude.ToLower().Trim() == contactInfoDto.Latitude.ToLower().Trim());


            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsEmailUnique(ContactInfoDto contactInfoDto)
        {
            var searchResult = _contactInfoRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != contactInfoDto.Id
                                                && x.Email != contactInfoDto.Email);


            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;

        }

    }
}
