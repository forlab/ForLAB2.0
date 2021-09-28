using FluentValidation;
using ForLab.DTO.Disease.DiseaseTestingProtocol;
using ForLab.Services.Disease.DiseaseTestingProtocol;

namespace ForLab.Validators.DiseaseTestingProtocol
{
    public class DiseaseTestingProtocolValidator : AbstractValidator<DiseaseTestingProtocolDto>
    {
        readonly IDiseaseTestingProtocolService _diseaseTestingProtocolService;
        public DiseaseTestingProtocolValidator(IDiseaseTestingProtocolService diseaseTestingProtocolService)
        {
            _diseaseTestingProtocolService = diseaseTestingProtocolService;
            RuleFor(x => x.DiseaseId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.TestingProtocolId)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
        }
    }

    public class ImportDiseaseTestingProtocolValidator : AbstractValidator<DiseaseTestingProtocolDto>
    {
        public ImportDiseaseTestingProtocolValidator()
        {
            RuleFor(x => x.DiseaseId)
                 .GreaterThan(0)
                 .NotEmpty()
                 .NotNull();
            RuleFor(x => x.TestingProtocolId)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
        }
    }

}
