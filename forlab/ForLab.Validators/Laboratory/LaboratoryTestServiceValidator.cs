using FluentValidation;
using ForLab.DTO.Laboratory.LaboratoryTestService;
using ForLab.Services.Laboratory.LaboratoryTestService;
using System;

namespace ForLab.Validators.LaboratoryTestService
{
    public class LaboratoryTestServiceValidator : AbstractValidator<LaboratoryTestServiceDto>
    {
        readonly ILaboratoryTestService _laboratoryTestService;
        public LaboratoryTestServiceValidator(ILaboratoryTestService laboratoryTestService)
        {
            _laboratoryTestService = laboratoryTestService;

            RuleFor(x => x.LaboratoryId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.TestId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.ServiceDuration)
               .NotEmpty()
               .NotNull()
               .Must(IsTestServiceUnique).WithMessage("This laboratory test service is already exist with this duration");
        }

        private bool IsTestServiceUnique(LaboratoryTestServiceDto laboratoryTestServiceDto, DateTime newValue)
        {
            return _laboratoryTestService.IsTestServiceUnique(laboratoryTestServiceDto);
        }
    }
    public class ImportLaboratoryTestServiceValidator : AbstractValidator<LaboratoryTestServiceDto>
    {
        public ImportLaboratoryTestServiceValidator()
        {
            RuleFor(x => x.LaboratoryId)
                 .NotEmpty()
                 .GreaterThan(0)
                 .NotNull();
            RuleFor(x => x.TestId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.ServiceDuration)
               .NotEmpty()
               .NotNull();
        }
    }
}
