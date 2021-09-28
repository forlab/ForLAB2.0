using FluentValidation;
using ForLab.DTO.CMS.FrequentlyAskedQuestion;
using ForLab.Services.CMS.FrequentlyAskedQuestion;

namespace ForLab.Validators.CMS
{
    public class FrequentlyAskedQuestionValidator : AbstractValidator<FrequentlyAskedQuestionDto>
    {
        readonly IFrequentlyAskedQuestionService _frequentlyAskedQuestionService;
        public FrequentlyAskedQuestionValidator(IFrequentlyAskedQuestionService frequentlyAskedQuestionService)
        {
            _frequentlyAskedQuestionService = frequentlyAskedQuestionService;

            RuleFor(x => x.Question)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueQuestion).WithMessage("The Question is already exist please try a new one");
            RuleFor(x => x.Answer)
               .NotEmpty()
               .NotNull();
        }

        private bool BeUniqueQuestion(FrequentlyAskedQuestionDto frequentlyAskedQuestionDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _frequentlyAskedQuestionService.IsQuestionUnique(frequentlyAskedQuestionDto);
        }
    }
}
