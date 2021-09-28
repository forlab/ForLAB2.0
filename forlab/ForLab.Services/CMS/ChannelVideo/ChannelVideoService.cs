using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.CMS.ChannelVideo;
using ForLab.Repositories.CMS.ChannelVideo;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.UploadFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.CMS.ChannelVideo
{
    public class ChannelVideoService : GService<ChannelVideoDto, Data.DbModels.CMSSchema.ChannelVideo, IChannelVideoRepository>, IChannelVideoService
    {
        private readonly IChannelVideoRepository _channelVideoRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IUploadFilesService _uploadFilesService;
        public ChannelVideoService(IMapper mapper,
            IResponseDTO response,
            IUploadFilesService uploadFilesService,
            IChannelVideoRepository channelVideoRepository,
            IUnitOfWork<AppDbContext> unitOfWork) : base(channelVideoRepository, mapper)
        {
            _channelVideoRepository = channelVideoRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _uploadFilesService = uploadFilesService;


        }


        public IResponseDTO GetAll(string rootPath, int? pageIndex = null, int? pageSize = null, ChannelVideoFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.CMSSchema.ChannelVideo> query = null;
            try
            {
                query = _channelVideoRepository.GetAll()
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.AttachmentSize > 0)
                    {
                        query = query.Where(x => x.AttachmentSize == filterDto.AttachmentSize);
                    }
                    if (!string.IsNullOrEmpty(filterDto.AttachmentName))
                    {
                        query = query.Where(x => x.AttachmentName.Trim().ToLower().Contains(filterDto.AttachmentName.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.AttachmentUrl))
                    {
                        query = query.Where(x => x.AttachmentUrl.Trim().ToLower().Contains(filterDto.AttachmentUrl.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Title))
                    {
                        query = query.Where(x => x.Title.Trim().ToLower().Contains(filterDto.Title.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ExtensionFormat))
                    {
                        query = query.Where(x => x.ExtensionFormat.Trim().ToLower().Contains(filterDto.ExtensionFormat.Trim().ToLower()));
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ChannelVideoDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(ChannelVideoFilterDto filterDto = null)
        {
            try
            {
                var query = _channelVideoRepository.GetAll(x => !x.IsDeleted && x.IsActive);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.AttachmentSize > 0)
                    {
                        query = query.Where(x => x.AttachmentSize == filterDto.AttachmentSize);
                    }
                    if (!string.IsNullOrEmpty(filterDto.AttachmentName))
                    {
                        query = query.Where(x => x.AttachmentName.Trim().ToLower().Contains(filterDto.AttachmentName.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.AttachmentUrl))
                    {
                        query = query.Where(x => x.AttachmentUrl.Trim().ToLower().Contains(filterDto.AttachmentUrl.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Title))
                    {
                        query = query.Where(x => x.Title.Trim().ToLower().Contains(filterDto.Title.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ExtensionFormat))
                    {
                        query = query.Where(x => x.ExtensionFormat.Trim().ToLower().Contains(filterDto.ExtensionFormat.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.CMSSchema.ChannelVideo() { Id = i.Id});
                query = query.OrderBy(x => x.AttachmentName);
                var dataList = _mapper.Map<List<ChannelVideoDto>>(query.ToList());

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
        public async Task<IResponseDTO> GetChannelVideoDetails(int channelVideoId)
        {
            try
            {
                var channelVideo = await _channelVideoRepository.GetAll()
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == channelVideoId);
                if (channelVideo == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var channelVideoDto = _mapper.Map<ChannelVideoDto>(channelVideo);

                _response.Data = channelVideoDto;
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
        public async Task<IResponseDTO> CreateChannelVideo(ChannelVideoDto channelVideoDto, IFormFile file = null)
        {
            try
            {
                var channelVideo = _mapper.Map<Data.DbModels.CMSSchema.ChannelVideo>(channelVideoDto);

                if (file != null)
                {
                    var path = $"\\Uploads\\ChannelVideos";
                    await _uploadFilesService.UploadFile(path, file);
                    if (!channelVideo.IsExternalResource && !channelVideo.IsDeleted)
                    {
                        channelVideo.AttachmentUrl = $"\\{path}\\{channelVideo.AttachmentName}";
                    }
                }

                // Add to the DB
                await _channelVideoRepository.AddAsync(channelVideo);

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
        public async Task<IResponseDTO> UpdateChannelVideo(ChannelVideoDto channelVideoDto, IFormFile file = null)
        {
            try
            {
                var usefulResourceExist = await _channelVideoRepository.GetFirstAsync(x => x.Id == channelVideoDto.Id);
                if (usefulResourceExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var channelVideo = _mapper.Map<Data.DbModels.CMSSchema.ChannelVideo>(channelVideoDto);

                _channelVideoRepository.Update(channelVideo);

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
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int channelVideoId, bool IsActive)
        {
            try
            {
                var channelVideo = await _channelVideoRepository.GetFirstAsync(x => x.Id == channelVideoId);
                if (channelVideo == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                channelVideo.IsActive = IsActive;
                channelVideo.UpdatedBy = loggedInUserId;
                channelVideo.UpdatedOn = DateTime.Now;

                // Update on the Database
                _channelVideoRepository.Update(channelVideo);

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
        public async Task<IResponseDTO> RemoveChannelVideo(int channelVideoId, int loggedInUserId)
        {
            try
            {
                var channelVideo = await _channelVideoRepository.GetFirstOrDefaultAsync(x => x.Id == channelVideoId);
                if (channelVideo == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                channelVideo.IsDeleted = true;
                channelVideo.UpdatedBy = loggedInUserId;
                channelVideo.UpdatedOn = DateTime.Now;

                // Update on the Database
                _channelVideoRepository.Update(channelVideo);

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
        public bool IsTitleUnique(ChannelVideoDto channelVideoDto)
        {
            var searchResult = _channelVideoRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != channelVideoDto.Id
                                                && x.Title.ToLower().Trim() == channelVideoDto.Title.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsUrlUnique(ChannelVideoDto channelVideoDto)
        {
            var searchResult = _channelVideoRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != channelVideoDto.Id
                                                && x.IsExternalResource == channelVideoDto.IsExternalResource
                                                && x.AttachmentUrl.ToLower().Trim() == channelVideoDto.AttachmentUrl.ToLower().Trim());


            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}
