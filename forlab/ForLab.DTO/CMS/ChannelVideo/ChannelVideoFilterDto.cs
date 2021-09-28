﻿using ForLab.DTO.Common;
namespace ForLab.DTO.CMS.ChannelVideo
{
  public  class ChannelVideoFilterDto:BaseFilterDto
    {
        public string Title { get; set; }
        public string AttachmentUrl { get; set; }
        public float? AttachmentSize { get; set; }
        public string AttachmentName { get; set; }
        public string ExtensionFormat { get; set; }
        public bool? IsExternalResource { get; set; }
    }
}