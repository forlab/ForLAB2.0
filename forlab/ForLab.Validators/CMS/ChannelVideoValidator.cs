using FluentValidation;
using ForLab.DTO.CMS.ChannelVideo;
using ForLab.Services.CMS.ChannelVideo;

namespace ForLab.Validators.CMS
{
    public class ChannelVideoValidator: AbstractValidator<ChannelVideoDto>
    {
        readonly IChannelVideoService _channelServiceService;
        public ChannelVideoValidator(IChannelVideoService channelServiceService)
        {
            _channelServiceService = channelServiceService;
            RuleFor(x => x.Title)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("Title of This Channel video Exists");
            RuleFor(x => x.AttachmentUrl)
                //.Must(BeValidUrl).WithMessage("Type a valid URL")
                .Must(BeUniqueUrl).WithMessage("The URL is already exist please try a new one");
        }
        private bool BeUniqueName(ChannelVideoDto channelVideoDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _channelServiceService.IsTitleUnique(channelVideoDto);
        }
        private bool BeUniqueUrl(ChannelVideoDto channelVideoDto, string newValue)
        {
            if (!channelVideoDto.IsExternalResource)
            {
                return true;
            }
            return _channelServiceService.IsUrlUnique(channelVideoDto);
        }
        private bool BeValidUrl(ChannelVideoDto channelVideoDto, string newValue)
        {
            if (!channelVideoDto.IsExternalResource)
            {
                return true;
            }

            return (new GeneralValidators()).IsValidUrl(newValue);
        }
    }
}
