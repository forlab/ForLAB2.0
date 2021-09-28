using FluentValidation;
using ForLab.DTO.CMS.InquiryQuestion;
using ForLab.Services.CMS.InquiryQuestion;

namespace ForLab.Validators.CMS
{
    public class InquiryQuestionValidator: AbstractValidator<InquiryQuestionDto>
    {
        readonly IInquiryQuestionService _inquesryQuestionService;
        public InquiryQuestionValidator(IInquiryQuestionService inquiryQuestionService)
        {
            _inquesryQuestionService = inquiryQuestionService;
            RuleFor(x => x.Name )
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Message)
                .NotEmpty()
                .NotNull();
        }
    }

  }
