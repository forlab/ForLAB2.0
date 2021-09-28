using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.CMS.UsefulResource;
using ForLab.Repositories.CMS.UsefulResource;
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

namespace ForLab.Services.CMS.UsefulResource
{
    public class UsefulResourceService : GService<UsefulResourceDto, Data.DbModels.CMSSchema.UsefulResource, IUsefulResourceRepository>, IUsefulResourceService
    {
        private readonly IUsefulResourceRepository _usefulResourceRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IUploadFilesService _uploadFilesService;

        public UsefulResourceService(IMapper mapper,
            IResponseDTO response,
            IUsefulResourceRepository usefulResourceRepository,
             IUploadFilesService uploadFilesService,
            IUnitOfWork<AppDbContext> unitOfWork) : base(usefulResourceRepository, mapper)
        {
            _usefulResourceRepository = usefulResourceRepository;
            _response = response;
            _uploadFilesService = uploadFilesService;
            _unitOfWork = unitOfWork;
        }


        public IResponseDTO GetAll(string rootPath, int? pageIndex = null, int? pageSize = null, UsefulResourceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.CMSSchema.UsefulResource> query = null;
            try
            {
                query = _usefulResourceRepository.GetAll()
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Title))
                    {
                        query = query.Where(x => x.Title.Trim().ToLower().Contains(filterDto.Title.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.AttachmentUrl))
                    {
                        query = query.Where(x => x.AttachmentUrl.Trim().ToLower().Contains(filterDto.AttachmentUrl.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.AttachmentName))
                    {
                        query = query.Where(x => x.AttachmentName.Trim().ToLower().Contains(filterDto.AttachmentName.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<UsefulResourceDto>>(query.ToList());


                foreach (var item in dataList.Where(x => !x.IsExternalResource))
                {
                    if (!string.IsNullOrEmpty(item.AttachmentUrl))
                    {
                        item.AttachmentUrl = rootPath + item.AttachmentUrl;
                    }
                }

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
        public IResponseDTO GetAllAsDrp(UsefulResourceFilterDto filterDto = null)
        {
            try
            {
                var query = _usefulResourceRepository.GetAll(x => !x.IsDeleted && x.IsActive);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Title))
                    {
                        query = query.Where(x => x.Title.Trim().ToLower().Contains(filterDto.Title.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.AttachmentUrl))
                    {
                        query = query.Where(x => x.AttachmentUrl.Trim().ToLower().Contains(filterDto.AttachmentUrl.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.AttachmentName))
                    {
                        query = query.Where(x => x.AttachmentName.Trim().ToLower().Contains(filterDto.AttachmentName.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.CMSSchema.UsefulResource() { Id = i.Id });
                var dataList = _mapper.Map<List<UsefulResourceDto>>(query.ToList());

                _response.Data = dataList;
                _response.IsPassed = true;
                _response.Message = "Done";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> GetUsefulResourceDetails(int usefulResourceId)
        {
            try
            {
                var usefulResource = await _usefulResourceRepository.GetAll()
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == usefulResourceId);
                if (usefulResource == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var usefulResourceDto = _mapper.Map<UsefulResourceDto>(usefulResource);

                _response.Data = usefulResourceDto;
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
        public async Task<IResponseDTO> CreateUsefulResources(List<UsefulResourceDto> usefulResourceDtos, List<IFormFile> files = null)
        {
            try
            {
                var usefulResource = _mapper.Map<List<Data.DbModels.CMSSchema.UsefulResource>>(usefulResourceDtos);

                // Add to the DB
                await _usefulResourceRepository.AddRangeAsync(usefulResource);

                if (files != null)
                {
                    var path = $"\\Uploads\\UsefulResource";
                    await _uploadFilesService.UploadFiles(path, files);
                    foreach (var item in usefulResourceDtos.Where(x => x.IsExternalResource && !x.IsDeleted))
                    {
                        item.AttachmentUrl = $"\\{path}\\{item.AttachmentName}";
                    }
                }

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
        public async Task<IResponseDTO> CreateUsefulResource(UsefulResourceDto usefulResourceDto, IFormFile file = null)
        {
            try
            {
                var usefulResource = _mapper.Map<Data.DbModels.CMSSchema.UsefulResource>(usefulResourceDto);

                if (file != null)
                {
                    var path = $"\\Uploads\\UsefulResources";
                    await _uploadFilesService.UploadFile(path, file);
                    if (!usefulResource.IsExternalResource && !usefulResource.IsDeleted)
                    {
                        usefulResource.AttachmentUrl = $"\\{path}\\{usefulResource.AttachmentName}";
                    }
                }

                // Add to the DB
                await _usefulResourceRepository.AddAsync(usefulResource);

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
        public async Task<IResponseDTO> UpdateUsefulResource(UsefulResourceDto usefulResourceDto, IFormFile file = null)
        {
            try
            {
                var usefulResourceExist = await _usefulResourceRepository.GetFirstAsync(x => x.Id == usefulResourceDto.Id);
                if (usefulResourceExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var usefulResource = _mapper.Map<Data.DbModels.CMSSchema.UsefulResource>(usefulResourceDto);

                _usefulResourceRepository.Update(usefulResource);

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
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int usefulResourceId, bool IsActive)
        {
            try
            {
                var usefulResource = await _usefulResourceRepository.GetFirstAsync(x => x.Id == usefulResourceId);
                if (usefulResource == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                usefulResource.IsActive = IsActive;
                usefulResource.UpdatedBy = loggedInUserId;
                usefulResource.UpdatedOn = DateTime.Now;

                // Update on the Database
                _usefulResourceRepository.Update(usefulResource);

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
        public async Task<IResponseDTO> RemoveUsefulResource(int usefulResourceId, int loggedInUserId)
        {
            try
            {
                var usefulResource = await _usefulResourceRepository.GetFirstOrDefaultAsync(x => x.Id == usefulResourceId);
                if (usefulResource == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                usefulResource.IsDeleted = true;
                usefulResource.UpdatedBy = loggedInUserId;
                usefulResource.UpdatedOn = DateTime.Now;

                // Update on the Database
                _usefulResourceRepository.Update(usefulResource);

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
        public async Task<IResponseDTO> IncrementDownloadCount(int usefulResourceId)
        {
            try
            {
                var usefulResource = await _usefulResourceRepository.GetFirstAsync(x => x.Id == usefulResourceId);
                if (usefulResource == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update downcount value
                usefulResource.DownloadCount++;

                // Update on the Database
                _usefulResourceRepository.Update(usefulResource);

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
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        // Validators methods
        public bool IsTitleUnique(UsefulResourceDto usefulResourceDto)
        {
            var searchResult = _usefulResourceRepository.GetAll(x =>
                                              !x.IsDeleted
                                              && x.Id != usefulResourceDto.Id
                                              && x.Title.ToLower().Trim() == usefulResourceDto.Title.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsAttachmentNameUnique(UsefulResourceDto usefulResourceDto)
        {
            var searchResult = _usefulResourceRepository.GetAll(x =>
                                              !x.IsDeleted
                                              && x.Id != usefulResourceDto.Id
                                              && x.AttachmentName.ToLower().Trim() == usefulResourceDto.AttachmentName.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsUrlUnique(UsefulResourceDto usefulResourceDto)
        {
            var searchResult = _usefulResourceRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != usefulResourceDto.Id
                                                && x.IsExternalResource == usefulResourceDto.IsExternalResource
                                                && x.AttachmentUrl.ToLower().Trim() == usefulResourceDto.AttachmentUrl.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}
