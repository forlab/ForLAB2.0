import { BaseURL } from '../config';

export const ChannelVideosController = {
    GetAll: BaseURL + `/api/ChannelVideos/GetAll`,
    GetAllAsDrp: BaseURL + `/api/ChannelVideos/GetAllAsDrp`,
    GetChannelVideoDetails: BaseURL + `/api/ChannelVideos/GetChannelVideoDetails`,
    CreateChannelVideo: BaseURL + `/api/ChannelVideos/CreateChannelVideo`,
    UpdateChannelVideo: BaseURL + `/api/ChannelVideos/UpdateChannelVideo`,
    UpdateIsActive: BaseURL + `/api/ChannelVideos/UpdateIsActive`,
    RemoveChannelVideo: BaseURL + `/api/ChannelVideos/RemoveChannelVideo`,
}