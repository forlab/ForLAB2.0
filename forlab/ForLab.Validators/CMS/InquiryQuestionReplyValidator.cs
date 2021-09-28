using FluentValidation;
using ForLab.DTO.CMS.InquiryQuestionReply;

namespace ForLab.Validators.CMS
{
    public class InquiryQuestionReplyValidator: AbstractValidator<InquiryQuestionReplyDto>
    {
        public InquiryQuestionReplyValidator()
        {
            RuleFor(x => x.InquiryQuestionId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Message)
                .NotEmpty()
                .NotNull();
        }
    }
  }
