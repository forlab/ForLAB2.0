using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.Repositories.Vendor.VendorContact;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ForLab.DTO.Vendor.VendorContact;
using Microsoft.EntityFrameworkCore;
using ForLab.Core.Common;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.General;

namespace ForLab.Services.Vendor.VendorContact
{
    public class VendorContactService : GService<VendorContactDto, Data.DbModels.VendorSchema.VendorContact, IVendorContactRepository>, IVendorContactService
    {
        private readonly IVendorContactRepository _vendorContactRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportVendorContactDto> _fileService;
        private readonly IGeneralService _generalService;

        public VendorContactService(IMapper mapper,
            IResponseDTO response,
            IVendorContactRepository vendorContactRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportVendorContactDto> fileService,
            IGeneralService generalService) : base(vendorContactRepository, mapper)
        {
            _vendorContactRepository = vendorContactRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, VendorContactFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.VendorSchema.VendorContact> query = null;
            try
            {
                query = _vendorContactRepository.GetAll()
                                                .Include(x => x.Vendor)
                                                .Include(x => x.Creator)
                                                .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        var createdBy = _generalService.SuperAdminIds();
                        createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => createdBy.Contains(x.CreatedBy));
                    }

                    if (filterDto.VendorId > 0)
                    {
                        query = query.Where(x => x.VendorId == filterDto.VendorId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Telephone))
                    {
                        query = query.Where(x => x.Telephone.Trim().ToLower().Contains(filterDto.Telephone.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Email))
                    {
                        query = query.Where(x => x.Email.Trim().ToLower().Contains(filterDto.Email.Trim().ToLower()));
                    }
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                }

                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<VendorContactDto>>(query.ToList());

                _response.Data = new
                {
                    List = dataList,
                    Page = pageIndex ?? 0,
                    pageSize = pageSize ?? 0,
                    Total = total,
                    Pages = pageSize.HasValue && pageSize.Value > 0 ? total / pageSize : 1
                };

                _response.Message = "Ok";
                _response.IsPassed = true;

            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public IResponseDTO GetAllAsDrp(VendorContactFilterDto filterDto = null)
        {
            try
            {
                var query = _vendorContactRepository.GetAll(x => !x.IsDeleted && x.IsActive && x.VendorId == filterDto.VendorId);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        var createdBy = _generalService.SuperAdminIds();
                        createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => createdBy.Contains(x.CreatedBy));
                    }

                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Telephone))
                    {
                        query = query.Where(x => x.Telephone.Trim().ToLower().Contains(filterDto.Telephone.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Email))
                    {
                        query = query.Where(x => x.Email.Trim().ToLower().Contains(filterDto.Email.Trim().ToLower()));
                    }
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                }

                query = query.Select(i => new Data.DbModels.VendorSchema.VendorContact()
                {
                    Id = i.Id,
                    Name = i.Name
                });
                query = query.OrderBy(x => x.Name);

                var dataList = _mapper.Map<List<VendorContactDrp>>(query.ToList());

                _response.Data = dataList;
                _response.IsPassed = true;
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }

            return _response;
        }
        public async Task<IResponseDTO> GetVendorContactDetails(int vendorContactId)
        {
            try
            {
                var vendorContact = await _vendorContactRepository.GetAll()
                                                .Include(x => x.Creator)
                                                .FirstOrDefaultAsync(x => x.Id == vendorContactId);
                if (vendorContact == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var vendorContactDto = _mapper.Map<VendorContactDto>(vendorContact);

                _response.Data = vendorContactDto;
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
        public async Task<IResponseDTO> CreateVendorContact(VendorContactDto vendorContactDto)
        {
            try
            {
                var vendorContact = _mapper.Map<Data.DbModels.VendorSchema.VendorContact>(vendorContactDto);

                // Set relation variables with null to avoid unexpected EF errors
                vendorContact.Vendor = null;

                // Add to the Database
                await _vendorContactRepository.AddAsync(vendorContact);

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
        public async Task<IResponseDTO> UpdateVendorContact(VendorContactDto vendorContactDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var vendorContactExist = await _vendorContactRepository.GetFirstAsync(x => x.Id == vendorContactDto.Id);
                if (vendorContactExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && vendorContactExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var vendorContact = _mapper.Map<Data.DbModels.VendorSchema.VendorContact>(vendorContactDto);

                // Set relation variables with null to avoid unexpected EF errors
                vendorContact.Vendor = null;

                // Update on the Database
                _vendorContactRepository.Update(vendorContact);

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
        public async Task<IResponseDTO> UpdateIsActive(int vendorContactId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var vendorContact = await _vendorContactRepository.GetFirstAsync(x => x.Id == vendorContactId);
                if (vendorContact == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && vendorContact.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                vendorContact.IsActive = IsActive;
                vendorContact.UpdatedBy = LoggedInUserId;
                vendorContact.UpdatedOn = DateTime.Now;

                // Update on the Database
                _vendorContactRepository.Update(vendorContact);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

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
        public async Task<IResponseDTO> RemoveVendorContact(int vendorContactId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var vendorContact = await _vendorContactRepository.GetFirstAsync(x => x.Id == vendorContactId);
                if (vendorContact == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && vendorContact.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }


                // Update IsDeleted value
                vendorContact.IsDeleted = true;
                vendorContact.UpdatedBy = LoggedInUserId;
                vendorContact.UpdatedOn = DateTime.Now;

                // Update on the Database
                _vendorContactRepository.Update(vendorContact);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

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
        public GeneratedFile ExportVendorContacts(int? pageIndex = null, int? pageSize = null, VendorContactFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.VendorSchema.VendorContact> query = null;
            try
            {
                query = _vendorContactRepository.GetAll()
                                    .Include(x => x.Vendor)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        var createdBy = _generalService.SuperAdminIds();
                        createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => createdBy.Contains(x.CreatedBy));
                    }

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.VendorId > 0)
                    {
                        query = query.Where(x => x.VendorId == filterDto.VendorId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Telephone))
                    {
                        query = query.Where(x => x.Telephone.Trim().ToLower().Contains(filterDto.Telephone.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Name);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "VendorName".ToLower())
                    {
                        filterDto.SortProperty = "VendorId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportVendorContactDto>>(query.ToList());

                return _fileService.ExportToExcel(dataList);

            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return null;
        }
        public async Task<IResponseDTO> ImportVendorContacts(List<VendorContactDto> vendorContactDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var vendorContacts_database = _vendorContactRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var vendorContacts = _mapper.Map<List<Data.DbModels.VendorSchema.VendorContact>>(vendorContactDtos);

                // Get the new ones that their names don't exist on the database
                var newVendorContacts = new List<Data.DbModels.VendorSchema.VendorContact>();
                foreach (var item in vendorContacts)
                {
                    // vars
                    var emails_database = vendorContacts_database.Where(x => x.VendorId == item.VendorId).Select(x => x.Email.ToLower().Trim());
                    var names_database = vendorContacts_database.Where(x => x.VendorId == item.VendorId).Select(x => x.Name.ToLower().Trim());

                    if (!names_database.Contains(item.Name.ToLower().Trim())
                         && !emails_database.Contains(item.Email.ToLower().Trim()))
                    {
                        newVendorContacts.Add(item);
                    }
                }

                // Get Skipped
                var skipped = vendorContacts.Where(x => ImportSkipped(vendorContacts_database, x)).ToList();

                // Select the objects that their names already exist in the database
                var updatedVendorContacts = vendorContacts.Except(skipped).Except(newVendorContacts);

                if (!IsSuperAdmin)
                {
                    updatedVendorContacts = updatedVendorContacts.Where(x => x.CreatedBy == LoggedInUserId).ToList();
                }

                // Add the new object to the database
                if (newVendorContacts.Count() > 0)
                { 
                    // Set relation variables with null to avoid unexpected EF errors
                    newVendorContacts.Select(x =>
                    {
                        x.Vendor = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _vendorContactRepository.AddRangeAsync(newVendorContacts);
                }

                // Update the existing objects with the new values
                if (updatedVendorContacts.Count() > 0)
                {
                    foreach (var item in updatedVendorContacts)
                    {
                        var fromDatabase = vendorContacts_database.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name?.ToLower()?.Trim()
                                                                              || x.Email.ToLower().Trim() == item.Email?.ToLower()?.Trim());
                        if (fromDatabase == null)
                        {
                            continue;
                        }
                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Name = item.Name;
                        fromDatabase.Email = item.Email;
                        fromDatabase.VendorId = item.VendorId;
                        fromDatabase.Telephone = item.Telephone;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Vendor = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _vendorContactRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newVendorContacts.Count();
                var numberOfUpdated = updatedVendorContacts.Count();
                var numberOfSkipped = skipped.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = vendorContactDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = numberOfSkipped
                };
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
        public bool IsNameUnique(VendorContactDto vendorContactDto)
        {
            var searchResult = _vendorContactRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != vendorContactDto.Id
                                                && x.VendorId == vendorContactDto.VendorId
                                                && x.Name.ToLower().Trim() == vendorContactDto.Name.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsEmailUnique(VendorContactDto vendorContactDto)
        {
            var searchResult = _vendorContactRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != vendorContactDto.Id
                                                && x.VendorId == vendorContactDto.VendorId
                                                && x.Email.ToLower().Trim() == vendorContactDto.Email.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        private bool ImportSkipped(List<Data.DbModels.VendorSchema.VendorContact> vendorContacts, Data.DbModels.VendorSchema.VendorContact vendorContact)
        {
            var foundByName = vendorContacts.FirstOrDefault(x => x.Name.Trim().ToLower() == vendorContact.Name.Trim().ToLower()
                                                              && x.VendorId == vendorContact.VendorId);
            var foundByEmail = vendorContacts.FirstOrDefault(x => x.Email.Trim().ToLower() == vendorContact.Email.Trim().ToLower()
                                                               && x.VendorId == vendorContact.VendorId);

            if (foundByName != null && foundByEmail != null && foundByName.Id != foundByEmail.Id)
            {
                return true;
            }

            return false;
        }
    }
}
