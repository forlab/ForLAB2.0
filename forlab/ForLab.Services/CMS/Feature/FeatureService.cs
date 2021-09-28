using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.CMS.Feature;
using ForLab.Repositories.CMS.Feature;
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

namespace ForLab.Services.CMS.Feature
{
    public class FeatureService : GService<FeatureDto, Data.DbModels.CMSSchema.Feature, IFeatureRepository>, IFeatureService
    {
        private readonly IFeatureRepository _featureRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IUploadFilesService _uploadFilesService;

        public FeatureService(IMapper mapper,
            IResponseDTO response,
            IFeatureRepository featureRepository,
             IUploadFilesService uploadFilesService,
            IUnitOfWork<AppDbContext> unitOfWork) : base(featureRepository, mapper)
        {
            _featureRepository = featureRepository;
            _response = response;
            _uploadFilesService = uploadFilesService;
            _unitOfWork = unitOfWork;
        }


        public IResponseDTO GetAll(string rootPath, int? pageIndex = null, int? pageSize = null, FeatureFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.CMSSchema.Feature> query = null;
            try
            {
                query = _featureRepository.GetAll()
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

                var dataList = _mapper.Map<List<FeatureDto>>(query.ToList());
                foreach (var item in dataList)
                {
                    if (!string.IsNullOrEmpty(item.LogoPath))
                    {
                        item.LogoPath = rootPath + item.LogoPath;
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
        public IResponseDTO GetAllAsDrp(FeatureFilterDto filterDto = null)
        {
            try
            {
                var query = _featureRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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
                }

                query = query.Select(i => new Data.DbModels.CMSSchema.Feature() { Id = i.Id });
                var dataList = _mapper.Map<List<FeatureDto>>(query.ToList());

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
        public async Task<IResponseDTO> GetFeatureDetails(string rootPath, int featureId)
        {
            try
            {
                var feature = await _featureRepository.GetAll()
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == featureId);
                if (feature == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }
                var featureDto = _mapper.Map<FeatureDto>(feature);
                _response.Data = featureDto;
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
        public async Task<IResponseDTO> CreateFeature(FeatureDto featureDto, IFormFile file = null)
        {
            try
            {
                var feature = _mapper.Map<Data.DbModels.CMSSchema.Feature>(featureDto);

                if (file != null)
                {
                    var path = $"\\Uploads\\Features";
                    await _uploadFilesService.UploadFile(path, file);
                    if (!feature.IsDeleted)
                    {
                        feature.LogoPath = $"\\{path}\\{file.FileName}";
                    }
                }

                // Add to the DB
                await _featureRepository.AddAsync(feature);

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
        public async Task<IResponseDTO> UpdateFeature(FeatureDto featureDto, IFormFile file = null)
        {
            try
            {
                var featureExist = await _featureRepository.GetFirstAsync(x => x.Id == featureDto.Id);
                if (featureExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var feature = _mapper.Map<Data.DbModels.CMSSchema.Feature>(featureDto);

                if (file != null)
                {
                    var path = $"\\Uploads\\Features";
                    await _uploadFilesService.UploadFile(path, file);
                    feature.LogoPath = $"\\{path}\\{file.FileName}";
                } else
                {
                    feature.LogoPath = featureExist.LogoPath;
                }

                _featureRepository.Update(feature);

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
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int featureId, bool IsActive)
        {
            try
            {
                var feature = await _featureRepository.GetFirstAsync(x => x.Id == featureId);
                if (feature == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                feature.IsActive = IsActive;
                feature.UpdatedBy = loggedInUserId;
                feature.UpdatedOn = DateTime.Now;

                // Update on the Database
                _featureRepository.Update(feature);

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
        public async Task<IResponseDTO> RemoveFeature(int featureId, int loggedInUserId)
        {
            try
            {
                var feature = await _featureRepository.GetFirstOrDefaultAsync(x => x.Id == featureId);
                if (feature == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                feature.IsDeleted = true;
                feature.UpdatedBy = loggedInUserId;
                feature.UpdatedOn = DateTime.Now;

                // Update on the Database
                _featureRepository.Update(feature);

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
        // Validators methods
        public bool IsTitleUnique(FeatureDto featureDto)
        {
            var searchResult = _featureRepository.GetAll(x =>
                                              !x.IsDeleted
                                              && x.Id != featureDto.Id
                                              && x.Title.ToLower().Trim() == featureDto.Title.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}
