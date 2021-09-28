using FluentValidation;
using ForLab.Data.Enums;
using ForLab.DTO.Laboratory.LaboratoryWorkingDay;
using ForLab.Services.Laboratory.LaboratoryWorkingDay;
using System;

namespace ForLab.Validators.LaboratoryWorkingDay
{
    public class LaboratoryWorkingDayValidator : AbstractValidator<LaboratoryWorkingDayDto>
    {

        readonly ILaboratoryWorkingDayService _laboratoryWorkingDayService;

        public LaboratoryWorkingDayValidator(ILaboratoryWorkingDayService laboratoryWorkingDayService)
        {
            _laboratoryWorkingDayService = laboratoryWorkingDayService;

            RuleFor(x => x.LaboratoryId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.FromTime)
                .NotEmpty()
                .NotNull()
                .Must(BeValidTime).WithMessage("From time & End time can't be the same time");
            RuleFor(x => x.ToTime)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Day)
               .NotEmpty()
               .NotNull()
               .IsEnumName(typeof(DaysOfWeek), caseSensitive: true).WithMessage("Please select a valid day of week")
               .Must(BeUniqueDay).WithMessage("Day is already exist please try a new one");
        }

        private bool BeUniqueDay(LaboratoryWorkingDayDto laboratoryWorkingDayDto, string NewValue)
        {
            if (string.IsNullOrEmpty(NewValue)) return true;
            return _laboratoryWorkingDayService.IsDayUnique(laboratoryWorkingDayDto);
        }

        private bool BeValidTime(LaboratoryWorkingDayDto laboratoryWorkingDayDto, TimeSpan NewValue)
        {
            if (laboratoryWorkingDayDto.FromTime == laboratoryWorkingDayDto.ToTime)
            {
                return false;
            }
            return true;
        }

    }
}
