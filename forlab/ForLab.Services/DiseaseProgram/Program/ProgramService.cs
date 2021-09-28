
using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.DiseaseProgram.Program;
using ForLab.Repositories.DiseaseProgram.Program;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ForLab.Core.Common;
using ForLab.Services.Global.FileService;
using ForLab.DTO.Testing.TestingProtocolCalculationPeriodMonth;
using ForLab.Services.Global.General;

namespace ForLab.Services.DiseaseProgram.Program
{
    public class ProgramService : GService<ProgramDto, Data.DbModels.DiseaseProgramSchema.Program, IProgramRepository>, IProgramService
    {
        private readonly IProgramRepository _programRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportProgramDto> _fileService;
        private readonly IGeneralService _generalService;

        public ProgramService(IMapper mapper,
            IResponseDTO response,
            IProgramRepository programRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportProgramDto> fileService,
            IGeneralService generalService) : base(programRepository, mapper)
        {
            _programRepository = programRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ProgramFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.Program> query = null;
            try
            {
                query = _programRepository.GetAll()
                                    .Include(x => x.Disease)
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
                    if (filterDto.DiseaseId > 0)
                    {
                        query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.NumberOfYears > 0)
                    {
                        query = query.Where(x => x.NumberOfYears == filterDto.NumberOfYears);
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "DiseaseName".ToLower())
                    {
                        filterDto.SortProperty = "DiseaseId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ProgramDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(ProgramFilterDto filterDto = null)
        {
            try
            {
                var query = _programRepository.GetAll(x => !x.IsDeleted && x.IsActive);


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
                    if (filterDto.DiseaseId > 0)
                    {
                        query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.NumberOfYears > 0)
                    {
                        query = query.Where(x => x.NumberOfYears == filterDto.NumberOfYears);
                    }
                }

                query = query.Select(i => new Data.DbModels.DiseaseProgramSchema.Program() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<ProgramDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetProgramDetails(int programId)
        {
            try
            {
                var program = await _programRepository.GetAll()
                                        .Include(x => x.Disease)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == programId);
                if (program == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var programDto = _mapper.Map<ProgramDto>(program);

                _response.Data = programDto;
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
        public async Task<IResponseDTO> GetProgramDetailsForForcast(int programId)
        {
            try
            {
                var program = await _programRepository.GetAll()
                                        .Include(x => x.ProgramTests)
                                            .ThenInclude(x => x.Test)
                                        .Include(x => x.ProgramTests)
                                            .ThenInclude(x => x.TestingProtocol)
                                            .ThenInclude(x => x.TestingProtocolCalculationPeriodMonths)
                                            .ThenInclude(x => x.CalculationPeriodMonth)
                                        .FirstOrDefaultAsync(x => x.Id == programId);
                if (program == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var programTests = program.ProgramTests
                                            .Where(x => !x.IsDeleted && x.IsActive)
                                            .Select(x => new { Id = x.TestId, Name = x.Test?.Name })
                                            .Distinct()
                                            .ToList();
                var programTestingProtocols = program.ProgramTests
                                            .Where(x => !x.IsDeleted && x.IsActive)
                                            .Select(x => new 
                                            { 
                                                Id = x.TestingProtocolId, 
                                                TestId = x.TestId, 
                                                BaseLine = x.TestingProtocol?.BaseLine, 
                                                TestAfterFirstYear = x.TestingProtocol?.TestAfterFirstYear,
                                                TestingProtocolCalculationPeriodMonthDtos = _mapper.Map<List<TestingProtocolCalculationPeriodMonthDto>>(x.TestingProtocol?.TestingProtocolCalculationPeriodMonths.Where(y => !y.IsDeleted))
                                            })
                                            .Distinct()
                                            .ToList();

                _response.Data = new 
                {
                    Tests = programTests,
                    TestingProtocols = programTestingProtocols
                };
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
        public async Task<IResponseDTO> CreateProgram(ProgramDto programDto)
        {
            try
            {
                var program = _mapper.Map<Data.DbModels.DiseaseProgramSchema.Program>(programDto);

                // Set relation variables with null to avoid unexpected EF errors
                program.Disease = null;
                program.ProgramTests?.Select(x => 
                { 
                    x.Creator = null; 
                    x.Test = null; 
                    x.Program = null;
                    x.TestingProtocol.PatientGroup = null; 
                    x.TestingProtocol.Test = null;
                    x.TestingProtocol.CalculationPeriod = null;
                    x.TestingProtocol.TestingProtocolCalculationPeriodMonths.Select(y => 
                    { 
                        y.TestingProtocol = null;
                        y.CalculationPeriodMonth = null; 
                        return y; 
                    }).ToList();
                    return x; 
                }).ToList();
                program.TestingAssumptionParameters?.Select(x => { x.Creator = null; x.Program = null; return x; }).ToList();
                program.ProductAssumptionParameters?.Select(x => { x.Creator = null; x.Program = null; return x; }).ToList();
                program.PatientAssumptionParameters?.Select(x => { x.Creator = null; x.Program = null; return x; }).ToList();
                program.ForecastMorbidityPrograms?.Select(x => { x.Creator = null; x.ForecastInfo = null; x.Program = null; return x; }).ToList();
                // Add to the DB
                await _programRepository.AddAsync(program);

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
        public async Task<IResponseDTO> UpdateProgram(ProgramDto programDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var programExist = await _programRepository.GetFirstAsync(x => x.Id == programDto.Id);
                if (programExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && programExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var program = _mapper.Map<Data.DbModels.DiseaseProgramSchema.Program>(programDto);

                // Set relation variables with null to avoid unexpected EF errors
                program.Disease = null;
                program.Disease = null;
                program.ProgramTests?.Select(x => { x.Creator = null; x.Test = null; x.TestingProtocol = null; x.Program = null; return x; }).ToList();
                program.TestingAssumptionParameters?.Select(x => { x.Creator = null; x.Program = null; return x; }).ToList();
                program.ProductAssumptionParameters?.Select(x => { x.Creator = null; x.Program = null; return x; }).ToList();
                program.PatientAssumptionParameters?.Select(x => { x.Creator = null; x.Program = null; return x; }).ToList();
                program.ForecastMorbidityPrograms?.Select(x => { x.Creator = null; x.ForecastInfo = null; x.Program = null; return x; }).ToList();

                // Update on the database
                _programRepository.Update(program);

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
        public async Task<IResponseDTO> UpdateIsActive(int programId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var program = await _programRepository.GetFirstAsync(x => x.Id == programId);
                if (program == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && program.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                program.IsActive = IsActive;
                program.UpdatedBy = LoggedInUserId;
                program.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                program.PatientAssumptionParameters = null;
                program.ProductAssumptionParameters = null;
                program.TestingAssumptionParameters = null;
                program.ProgramTests = null;
                program.ForecastMorbidityPrograms = null;
                //program.ForecastMorbidityTargetBases = null;

                // Update on the Database
                _programRepository.Update(program);

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
        public async Task<IResponseDTO> RemoveProgram(int programId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var program = await _programRepository.GetFirstOrDefaultAsync(x => x.Id == programId);
                if (program == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && program.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                program.IsDeleted = true;
                program.UpdatedBy = LoggedInUserId;
                program.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                program.PatientAssumptionParameters = null;
                program.ProductAssumptionParameters = null;
                program.TestingAssumptionParameters = null;
                program.ProgramTests = null;
                program.ForecastMorbidityPrograms = null;
                //program.ForecastMorbidityTargetBases = null;

                // Update on the Database
                _programRepository.Update(program);

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
        public GeneratedFile ExportPrograms(int? pageIndex = null, int? pageSize = null, ProgramFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.Program> query = null;
            try
            {
                query = _programRepository.GetAll()
                                    .Include(x => x.Disease)
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
                    if (filterDto.DiseaseId > 0)
                    {
                        query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.NumberOfYears > 0)
                    {
                        query = query.Where(x => x.NumberOfYears == filterDto.NumberOfYears);
                    }
                }

                query = query.OrderByDescending(x => x.Name);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "DiseaseName".ToLower())
                    {
                        filterDto.SortProperty = "DiseaseId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportProgramDto>>(query.ToList());

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
        public async Task<IResponseDTO> ImportPrograms(List<ProgramDto> programDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var programs_database = _programRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var programs = _mapper.Map<List<Data.DbModels.DiseaseProgramSchema.Program>>(programDtos);

                // vars
                var newPrograms = new List<Data.DbModels.DiseaseProgramSchema.Program>();
                var updatedPrograms = new List<Data.DbModels.DiseaseProgramSchema.Program>();

                // Get new and updated programs
                foreach (var item in programs)
                {
                    var foundProgram = programs_database.FirstOrDefault(x => x.DiseaseId == item.DiseaseId 
                                                                           && x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                    if (foundProgram == null)
                    {
                        newPrograms.Add(item);
                    }
                    else
                    {
                        updatedPrograms.Add(item);
                    }
                }

                if (!IsSuperAdmin)
                {
                    updatedPrograms = updatedPrograms.Where(x => x.CreatedBy == LoggedInUserId).ToList();
                }

                // Add the new object to the database
                if (newPrograms.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newPrograms.Select(x =>
                    {
                        //x.ForecastMorbidityTargetBases = null;
                        x.PatientAssumptionParameters = null;
                        x.ProductAssumptionParameters = null;
                        x.TestingAssumptionParameters = null;
                        x.ProgramTests = null;
                        x.ForecastMorbidityPrograms = null;
                        x.ForecastMorbidityTestingProtocolMonths = null;
                        x.Disease = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _programRepository.AddRangeAsync(newPrograms);
                }

                // Update the existing objects with the new values
                if (updatedPrograms.Count() > 0)
                {
                    foreach (var item in updatedPrograms)
                    {
                        var fromDatabase = programs_database.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name?.ToLower()?.Trim()
                                                                                    && x.DiseaseId == item.DiseaseId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }
                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Name = item.Name;
                        fromDatabase.NumberOfYears = item.NumberOfYears;
                        fromDatabase.DiseaseId = item.DiseaseId;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.PatientAssumptionParameters = null;
                        fromDatabase.ProductAssumptionParameters = null;
                        fromDatabase.TestingAssumptionParameters = null;
                        fromDatabase.ProgramTests = null;
                        fromDatabase.ForecastMorbidityPrograms = null;
                        fromDatabase.ForecastMorbidityTestingProtocolMonths = null;
                        fromDatabase.Disease = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _programRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newPrograms.Count();
                var numberOfUpdated = updatedPrograms.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = programDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = programDtos.Count - (numberOfAdded + numberOfUpdated)
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
        public bool IsNameUnique(ProgramDto programDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _programRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != programDto.Id
                                                && x.Name.ToLower().Trim() == programDto.Name.ToLower().Trim()
                                                && x.DiseaseId == programDto.DiseaseId);

            // Security Filter
            if (!IsSuperAdmin)
            {
                var createdBy = _generalService.SuperAdminIds();
                createdBy.Add(LoggedInUserId);
                searchResult = searchResult.Where(x => createdBy.Contains(x.CreatedBy));
            }

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int programId)
        {
            try
            {
                var program = await _programRepository.GetAll()
                                        .Include(x => x.PatientAssumptionParameters )
                                        .Include(x => x.ProductAssumptionParameters )
                                        .Include(x => x.TestingAssumptionParameters )
                                        .Include(x => x.ProgramTests )
                                        .Include(x => x.ForecastMorbidityPrograms )
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == programId);
                if (program == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (program.PatientAssumptionParameters.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Program cannot be deleted or deactivated where it contains 'Patient Assumption Parameters'";
                    return _response;
                }
                if (program.ProductAssumptionParameters.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Program cannot be deleted or deactivated where it contains 'Product Assumption Parameters'";
                    return _response;
                }
                if (program.TestingAssumptionParameters.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Program cannot be deleted or deactivated where it contains 'Testing Assumption Parameters'";
                    return _response;
                }
                if (program.ForecastMorbidityPrograms.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Program cannot be deleted or deactivated where it contains 'Forecast Morbidity Programs'";
                    return _response;
                }
                if (program.ProgramTests.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Program cannot be deleted or deactivated where it contains 'Program Tests'";
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
