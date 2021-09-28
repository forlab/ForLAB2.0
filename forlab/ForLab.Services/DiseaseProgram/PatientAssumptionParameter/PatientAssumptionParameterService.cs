using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.DiseaseProgram.PatientAssumptionParameter;
using ForLab.Repositories.DiseaseProgram.PatientAssumptionParameter;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.DiseaseProgram.PatientAssumptionParameter
{
    public class PatientAssumptionParameterService : GService<PatientAssumptionParameterDto, Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter, IPatientAssumptionParameterRepository>, IPatientAssumptionParameterService
    {
        private readonly IPatientAssumptionParameterRepository _patientAssumptionParameterRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportPatientAssumptionParameterDto> _fileService;
        private readonly IGeneralService _generalService;
        public PatientAssumptionParameterService(IMapper mapper,
            IResponseDTO response,
            IGeneralService generalService,
            IPatientAssumptionParameterRepository patientAssumptionParameterRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportPatientAssumptionParameterDto> fileService) : base(patientAssumptionParameterRepository, mapper)
        {
            _patientAssumptionParameterRepository = patientAssumptionParameterRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, PatientAssumptionParameterFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter> query = null;
            try
            {
                query = _patientAssumptionParameterRepository.GetAll()
                                    .Include(x => x.Program)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        var createdBy = _generalService.SuperAdminIds();
                        createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => createdBy.Contains(x.CreatedBy));
                    }

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.IsPercentage != null)
                    {
                        query = query.Where(x => x.IsPercentage == filterDto.IsPercentage);
                    }
                    if (filterDto.IsPositive != null)
                    {
                        query = query.Where(x => x.IsPositive == filterDto.IsPositive);
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "ProgramName".ToLower())
                    {
                        filterDto.SortProperty = "ProgramId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<PatientAssumptionParameterDto>>(query.ToList());

                _response.Data = new
                {
                    List = dataList,
                    Page = pageIndex ?? 0,
                    pageSize = pageSize ?? 0,
                    Total = total,
                    Pages = pageSize.HasValue && pageSize.Value > 0 ? total / pageSize : 1
                };

                _response.Message = "Ok";
                _response.IsPassed = true;

            }

            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public IResponseDTO GetAllAsDrp(PatientAssumptionParameterFilterDto filterDto = null)
        {
            try
            {
                var query = _patientAssumptionParameterRepository.GetAll(x => !x.IsDeleted && x.IsActive);


                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        var createdBy = _generalService.SuperAdminIds();
                        createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => createdBy.Contains(x.CreatedBy));
                    }

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.IsPercentage != null)
                    {
                        query = query.Where(x => x.IsPercentage == filterDto.IsPercentage);
                    }
                    if (filterDto.IsPositive != null)
                    {
                        query = query.Where(x => x.IsPositive == filterDto.IsPositive);
                    }
                }

                query = query.Select(i => new Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<PatientAssumptionParameterDrp>>(query.ToList());

                _response.Data = dataList;
                _response.IsPassed = true;
                _response.Message = "Done";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public IResponseDTO GetAllPatientAssumptionsForForcast(string programIds)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter> query = null;
            try
            {
                // Filter by program ids
                var progIds = programIds?.Split(',')?.Select(int.Parse)?.ToList();
                if (progIds == null || progIds.Count == 0 || !progIds.Any(x => x != 0))
                {
                    _response.Data = new List<GroupPatientAssumptionParameterDto>();
                    _response.IsPassed = true;
                    return _response;
                }

                query = _patientAssumptionParameterRepository.GetAll()
                                    .Include(x => x.Program)
                                    .Where(x => !x.IsDeleted);

                query = query.Where(x => progIds.Contains(x.ProgramId));
                query = query.OrderByDescending(x => x.Id);

                var dataList = _mapper.Map<List<PatientAssumptionParameterDto>>(query.ToList());
                var grouped = new List<GroupPatientAssumptionParameterDto>();
                var groupdByName = dataList.GroupBy(x => x.ProgramName);
                foreach(var item in groupdByName)
                {
                    grouped.Add(new GroupPatientAssumptionParameterDto
                    {
                        ProgramName = item.Key,
                        PatientAssumptionParameterDtos = item.ToList()
                    });
                }

                _response.Data = grouped;
                _response.Message = "Ok";
                _response.IsPassed = true;
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> GetPatientAssumptionParameterDetails(int patientAssumptionParameterId)
        {
            try
            {
                var patientAssumptionParameter = await _patientAssumptionParameterRepository.GetAll()
                                        .Include(x => x.ProgramId)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == patientAssumptionParameterId);
                if (patientAssumptionParameter == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var patientAssumptionParameterDto = _mapper.Map<PatientAssumptionParameterDto>(patientAssumptionParameter);

                _response.Data = patientAssumptionParameterDto;
                _response.Message = "Ok";
                _response.IsPassed = true;
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public async Task<IResponseDTO> CreatePatientAssumptionParameter(PatientAssumptionParameterDto patientAssumptionParameterDto)
        {
            try
            {
                var patientAssumptionParameter = _mapper.Map<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter>(patientAssumptionParameterDto);

                // Set relation variables with null to avoid unexpected EF errors
                patientAssumptionParameter.Program = null;

                // Add to the DB
                await _patientAssumptionParameterRepository.AddAsync(patientAssumptionParameter);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.Data = null;
                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> UpdatePatientAssumptionParameter(PatientAssumptionParameterDto patientAssumptionParameterDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var patientAssumptionParameterExist = await _patientAssumptionParameterRepository.GetFirstAsync(x => x.Id == patientAssumptionParameterDto.Id);
                if (patientAssumptionParameterExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && patientAssumptionParameterExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var patientAssumptionParameter = _mapper.Map<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter>(patientAssumptionParameterDto);

                // Set relation variables with null to avoid unexpected EF errors

                patientAssumptionParameter.Program = null;


                _patientAssumptionParameterRepository.Update(patientAssumptionParameter);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.Data = null;
                _response.IsPassed = true;
                _response.Message = "Ok";

            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> UpdateIsActive(int patientAssumptionParameterId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var patientAssumptionParameter = await _patientAssumptionParameterRepository.GetFirstAsync(x => x.Id == patientAssumptionParameterId);
                if (patientAssumptionParameter == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && patientAssumptionParameter.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                patientAssumptionParameter.IsActive = IsActive;
                patientAssumptionParameter.UpdatedBy = LoggedInUserId;
                patientAssumptionParameter.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                patientAssumptionParameter.ForecastPatientAssumptionValues = null;

                // Update on the Database
                _patientAssumptionParameterRepository.Update(patientAssumptionParameter);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }

            return _response;
        }
        public async Task<IResponseDTO> RemovePatientAssumptionParameter(int patientAssumptionParameterId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var patientAssumptionParameter = await _patientAssumptionParameterRepository.GetFirstOrDefaultAsync(x => x.Id == patientAssumptionParameterId);
                if (patientAssumptionParameter == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && patientAssumptionParameter.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                patientAssumptionParameter.IsDeleted = true;
                patientAssumptionParameter.UpdatedBy = LoggedInUserId;
                patientAssumptionParameter.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                patientAssumptionParameter.ForecastPatientAssumptionValues = null;

                // Update on the Database
                _patientAssumptionParameterRepository.Update(patientAssumptionParameter);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public GeneratedFile ExportPatientAssumptionParameters(int? pageIndex = null, int? pageSize = null, PatientAssumptionParameterFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter> query = null;
            try
            {
                query = _patientAssumptionParameterRepository.GetAll()
                                     .Include(x => x.Program)
                                     .Include(x => x.Creator)
                                     .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        var createdBy = _generalService.SuperAdminIds();
                        createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => createdBy.Contains(x.CreatedBy));
                    }

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.IsPercentage != null)
                    {
                        query = query.Where(x => x.IsPercentage == filterDto.IsPercentage);
                    }
                    if (filterDto.IsPositive != null)
                    {
                        query = query.Where(x => x.IsPositive == filterDto.IsPositive);
                    }
                }

                query = query.OrderBy(x => x.Program.Name);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "CountryName".ToLower())
                    {
                        filterDto.SortProperty = "CountryId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportPatientAssumptionParameterDto>>(query.ToList());
                return _fileService.ExportToExcel(dataList);
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return null;
        }
        public async Task<IResponseDTO> ImportPatientAssumptionParameters(List<PatientAssumptionParameterDto> patientAssumptionParameterDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var patientAssumptionParameters_database = _patientAssumptionParameterRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var patientAssumptionParameters = _mapper.Map<List<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter>>(patientAssumptionParameterDtos);

                // vars
                var newPatientAssumptionParameters = new List<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter>();
                var updatedPatientAssumptionParameters = new List<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter>();

                // Get new and updated patientAssumptionParameters
                foreach (var item in patientAssumptionParameters)
                {
                    var foundPatientAssumptionParameter = patientAssumptionParameters_database.FirstOrDefault(x => x.ProgramId == item.ProgramId 
                                                                                                                && x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                    if (foundPatientAssumptionParameter == null)
                    {
                        newPatientAssumptionParameters.Add(item);
                    }
                    else
                    {
                        updatedPatientAssumptionParameters.Add(item);
                    }
                }

                if (!IsSuperAdmin)
                {
                    updatedPatientAssumptionParameters = updatedPatientAssumptionParameters.Where(x => x.CreatedBy == LoggedInUserId).ToList();
                }

                // Add the new object to the database
                if (newPatientAssumptionParameters.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newPatientAssumptionParameters.Select(x =>
                    {
                        x.Program = null;
                        x.ForecastPatientAssumptionValues = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _patientAssumptionParameterRepository.AddRangeAsync(newPatientAssumptionParameters);
                }

                // Update the existing objects with the new values
                if (updatedPatientAssumptionParameters.Count() > 0)
                {
                    foreach (var item in updatedPatientAssumptionParameters)
                    {
                        var fromDatabase = patientAssumptionParameters_database.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name?.ToLower()?.Trim() 
                                                                                                    && x.ProgramId == item.ProgramId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Name = item.Name;
                        fromDatabase.ProgramId = item.ProgramId;
                        //boolen variables
                        fromDatabase.IsNumeric = item.IsNumeric;
                        fromDatabase.IsPercentage = item.IsPercentage;
                        fromDatabase.IsPositive = item.IsPositive;
                        fromDatabase.IsNegative = item.IsNegative;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Program = null;
                        fromDatabase.ForecastPatientAssumptionValues = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _patientAssumptionParameterRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newPatientAssumptionParameters.Count();
                var numberOfUpdated = updatedPatientAssumptionParameters.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = patientAssumptionParameterDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = patientAssumptionParameterDtos.Count - (numberOfAdded + numberOfUpdated)
                };
                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        // Validators methods
        public bool IsNameUnique(PatientAssumptionParameterDto patientAssumptionParameterDto)
        {
            var searchResult = _patientAssumptionParameterRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != patientAssumptionParameterDto.Id
                                                && x.Name.ToLower().Trim() == patientAssumptionParameterDto.Name.ToLower().Trim()
                                                && x.ProgramId == patientAssumptionParameterDto.ProgramId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int patientAssumptionParameterId)
        {
            try
            {
                var patientAssumptionParameter = await _patientAssumptionParameterRepository.GetAll()
                                        .Include(x => x.Program)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == patientAssumptionParameterId);
                if (patientAssumptionParameter == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (patientAssumptionParameter.ForecastPatientAssumptionValues.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Patient Assumption Parameter cannot be deleted or deactivated where it contains ' Forecast Patient Assumption Values'";
                    return _response;
                }

                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
    }
}
