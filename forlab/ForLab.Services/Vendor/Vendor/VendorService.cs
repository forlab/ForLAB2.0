using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.Repositories.Vendor.Vendor;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ForLab.DTO.Vendor.Vendor;
using Microsoft.EntityFrameworkCore;
using ForLab.Core.Common;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.General;

namespace ForLab.Services.Vendor.Vendor
{
    public class VendorService : GService<VendorDto, Data.DbModels.VendorSchema.Vendor, IVendorRepository>, IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportVendorDto> _fileService;
        private readonly IGeneralService _generalService;
        public VendorService(IMapper mapper,
            IResponseDTO response,
            IVendorRepository vendorRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportVendorDto> fileService,
            IGeneralService generalService) : base(vendorRepository, mapper)
        {
            _vendorRepository = vendorRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, VendorFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.VendorSchema.Vendor> query = null;
            try
            {
                query = _vendorRepository.GetAll()
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
                    if (!string.IsNullOrEmpty(filterDto.Address))
                    {
                        query = query.Where(x => x.Address.Trim().ToLower().Contains(filterDto.Address.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<VendorDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(VendorFilterDto filterDto = null)
        {
            try
            {
                var query = _vendorRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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

                query = query.Select(i => new Data.DbModels.VendorSchema.Vendor()
                {
                    Id = i.Id,
                    Name = i.Name
                });
                query = query.OrderBy(x => x.Name);

                var dataList = _mapper.Map<List<VendorDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetVendorDetails(int vendorId)
        {
            try
            {
                var vendor = await _vendorRepository.GetAll()
                                                .Include(x => x.Creator)
                                                .Include(x => x.Updator)
                                                .Include(x => x.VendorContacts).ThenInclude(x => x.Creator)
                                                .FirstOrDefaultAsync(x => x.Id == vendorId);
                if (vendor == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                vendor.VendorContacts = vendor.VendorContacts.Where(x => !x.IsDeleted).ToList();
                var vendorDto = _mapper.Map<VendorDto>(vendor);

                _response.Data = vendorDto;
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
        public async Task<IResponseDTO> CreateVendor(VendorDto vendorDto)
        {
            try
            {
                var vendor = _mapper.Map<Data.DbModels.VendorSchema.Vendor>(vendorDto);

                // Add to the Database
                await _vendorRepository.AddAsync(vendor);

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
        public async Task<IResponseDTO> UpdateVendor(VendorDto vendorDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var vendorExist = await _vendorRepository.GetFirstAsync(x => x.Id == vendorDto.Id);
                if (vendorExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && vendorExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var vendor = _mapper.Map<Data.DbModels.VendorSchema.Vendor>(vendorDto);

                // Update on the Database
                _vendorRepository.Update(vendor);

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
        public async Task<IResponseDTO> UpdateIsActive(int vendorId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var vendor = await _vendorRepository.GetFirstAsync(x => x.Id == vendorId);
                if (vendor == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && vendor.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                vendor.IsActive = IsActive;
                vendor.UpdatedBy = LoggedInUserId;
                vendor.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                vendor.VendorContacts = null;
                vendor.Products = null;
                vendor.Instruments = null;

                // Update on the Database
                _vendorRepository.Update(vendor);

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
        public async Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var items = _vendorRepository.GetAll(x => ids.Contains(x.Id)).ToList();

                if (!IsSuperAdmin && items.Select(x => x.CreatedBy).Any(x => x != LoggedInUserId))
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                var objectsToUpdate = items?.Where(x => x.IsActive != isActive).Select(x =>
                {
                    x.IsActive = isActive;
                    x.UpdatedBy = LoggedInUserId;
                    x.UpdatedOn = DateTime.Now;
                    return x;
                }).ToList();


                // Update on the Database
                foreach (var item in objectsToUpdate)
                {
                    _vendorRepository.Update(item);
                }

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "There is no changes to save";
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
        public async Task<IResponseDTO> RemoveVendor(int vendorId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var vendor = await _vendorRepository.GetFirstAsync(x => x.Id == vendorId);
                if (vendor == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && vendor.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }


                // Update IsDeleted value
                vendor.IsDeleted = true;
                vendor.UpdatedBy = LoggedInUserId;
                vendor.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                vendor.VendorContacts = null;
                vendor.Products = null;
                vendor.Instruments = null;

                // Update on the Database
                _vendorRepository.Update(vendor);

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
        public GeneratedFile ExportVendors(int? pageIndex = null, int? pageSize = null, VendorFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.VendorSchema.Vendor> query = null;
            try
            {
                query = _vendorRepository.GetAll()
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
                    if (!string.IsNullOrEmpty(filterDto.Address))
                    {
                        query = query.Where(x => x.Address.Trim().ToLower().Contains(filterDto.Address.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ExportVendorDto>>(query.ToList());

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
        public async Task<IResponseDTO> ImportVendors(List<VendorDto> vendorDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var vendors_database = _vendorRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var vendors = _mapper.Map<List<Data.DbModels.VendorSchema.Vendor>>(vendorDtos);

                // vars
                var emails_database = vendors_database.Select(x => x.Email.ToLower().Trim());
                var names_database = vendors_database.Select(x => x.Name.ToLower().Trim());

                // Get the new ones that their names don't exist on the database
                var newVendors = vendors.Where(x => !names_database.Contains(x.Name.ToLower().Trim())
                                                     && !emails_database.Contains(x.Email.ToLower().Trim()));

                // Get Skipped
                var skipped = vendors.Where(x => ImportSkipped(vendors_database, x)).ToList();

                // Select the objects that their names already exist in the database
                var updatedVendors = vendors.Except(skipped).Except(newVendors);
                if (!IsSuperAdmin)
                {
                    updatedVendors = updatedVendors.Where(x => x.CreatedBy == LoggedInUserId);
                }

                // Add the new object to the database
                if (newVendors.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newVendors.Select(x =>
                    {
                        x.VendorContacts = null;
                        x.Products = null;
                        x.Instruments = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _vendorRepository.AddRangeAsync(newVendors);
                }

                // Update the existing objects with the new values
                if (updatedVendors.Count() > 0)
                {
                    foreach (var item in updatedVendors)
                    {
                        var fromDatabase = vendors_database.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name?.ToLower()?.Trim()
                                                                              || x.Email.ToLower().Trim() == item.Email?.ToLower()?.Trim());
                        if (fromDatabase == null)
                        {
                            continue;
                        }
                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Name = item.Name;
                        fromDatabase.Address = item.Address;
                        fromDatabase.Telephone = item.Telephone;
                        fromDatabase.Email = item.Email;
                        fromDatabase.Url = item.Url;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.VendorContacts = null;
                        fromDatabase.Products = null;
                        fromDatabase.Instruments = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _vendorRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newVendors.Count();
                var numberOfUpdated = updatedVendors.Count();
                var numberOfSkipped = skipped.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = vendorDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = numberOfSkipped,
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
        public bool IsNameUnique(VendorDto vendorDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _vendorRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != vendorDto.Id
                                                && x.Name.ToLower().Trim() == vendorDto.Name.ToLower().Trim());
            // Security Filter
            if (!IsSuperAdmin)
            {
                var createdBy = _generalService.SuperAdminIds();
                createdBy.Add(LoggedInUserId);
                searchResult = searchResult.Where(x => createdBy.Contains(x.CreatedBy));
            }

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsEmailUnique(VendorDto vendorDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _vendorRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != vendorDto.Id
                                                && x.Email.ToLower().Trim() == vendorDto.Email.ToLower().Trim());
            // Security Filter
            if (!IsSuperAdmin)
            {
                var createdBy = _generalService.SuperAdminIds();
                createdBy.Add(LoggedInUserId);
                searchResult = searchResult.Where(x => createdBy.Contains(x.CreatedBy));
            }

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int vendorId)
        {
            try
            {
                var vendor = await _vendorRepository.GetAll()
                                        .Include(x => x.VendorContacts)
                                        .Include(x => x.Products)
                                        .Include(x => x.Instruments)
                                        .FirstOrDefaultAsync(x => x.Id == vendorId);
                if (vendor == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (vendor.VendorContacts.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Vendor cannot be deleted or deactivated where it contains 'Contacts'";
                    return _response;
                }
                if (vendor.Products.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Vendor cannot be deleted or deactivated where it contains 'Products'";
                    return _response;
                }
                if (vendor.Instruments.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Vendor cannot be deleted or deactivated where it contains 'Instruments'";
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

        private bool ImportSkipped(List<Data.DbModels.VendorSchema.Vendor> vendors, Data.DbModels.VendorSchema.Vendor vendor)
        {
            var foundByName = vendors.FirstOrDefault(x => x.Name.Trim().ToLower() == vendor.Name.Trim().ToLower());
            var foundByEmail = vendors.FirstOrDefault(x => x.Email.Trim().ToLower() == vendor.Email.Trim().ToLower());

            if (foundByName != null && foundByEmail != null && foundByName.Id != foundByEmail.Id)
            {
                return true;
            }

            return false;
        }
    }
}
