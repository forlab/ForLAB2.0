using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.ChannelVideo;
using ForLab.Repositories.CMS.ChannelVideo;
using ForLab.Services.Generics;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.CMS.ChannelVideo
{
    public interface IChannelVideoService : IGService<ChannelVideoDto, Data.DbModels.CMSSchema.ChannelVideo, IChannelVideoRepository>
    {
        IResponseDTO GetAll(string rootPath, int? pageIndex = null, int? pageSize = null, ChannelVideoFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(ChannelVideoFilterDto filterDto = null);
        Task<IResponseDTO> GetChannelVideoDetails(int channelVideoId);
        Task<IResponseDTO> CreateChannelVideo(ChannelVideoDto channelVideoDto, IFormFile file = null);
        Task<IResponseDTO> UpdateChannelVideo(ChannelVideoDto channelVideoDto, IFormFile file = null);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int channelVideoId, bool IsActive);
        Task<IResponseDTO> RemoveChannelVideo(int channelVideoId, int loggedInUserId);

        // Validators methods
        bool IsTitleUnique(ChannelVideoDto channelVideoDto);
        bool IsUrlUnique(ChannelVideoDto channelVideoDto);
    }
}
