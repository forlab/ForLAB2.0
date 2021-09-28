using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.DiseaseProgram.ProgramTest;
using ForLab.Repositories.DiseaseProgram.Program;
using ForLab.Repositories.DiseaseProgram.ProgramTest;
using ForLab.Repositories.Testing.TestingProtocol;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.General;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.DiseaseProgram.ProgramTest
{
    public class ProgramTestService : GService<ProgramTestDto, Data.DbModels.DiseaseProgramSchema.ProgramTest, IProgramTestRepository>, IProgramTestService
    {
        private readonly IProgramTestRepository _programTestRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ITestingProtocolRepository _testingProtocolRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportProgramTestDto> _fileService;
        private readonly IGeneralService _generalService;
        public ProgramTestService(IMapper mapper,
            IResponseDTO response,
            IProgramTestRepository programTestRepository,
            IProgramRepository programRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            ITestingProtocolRepository testingProtocolRepository,
            IFileService<ExportProgramTestDto> fileService,
            IGeneralService generalService) : base(programTestRepository, mapper)
        {
            _programTestRepository = programTestRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _testingProtocolRepository = testingProtocolRepository;
            _programRepository = programRepository;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ProgramTestFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.ProgramTest> query = null;
            try
            {
                query = _programTestRepository.GetAll()
                                    .Include(x => x.Test)
                                    .Include(x => x.TestingProtocol).ThenInclude(x => x.PatientGroup)
                                    .Include(x => x.TestingProtocol).ThenInclude(x => x.CalculationPeriod)
                                    .Include(x => x.TestingProtocol).ThenInclude(x => x.TestingProtocolCalculationPeriodMonths).ThenInclude(x => x.CalculationPeriodMonth)
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
                    if (filterDto.TestingProtocolId > 0)
                    {
                        query = query.Where(x => x.TestingProtocolId == filterDto.TestingProtocolId);
                    }
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Test.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "TestingProtocolName".ToLower())
                    {
                        filterDto.SortProperty = "TestingProtocolId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "TestName".ToLower())
                    {
                        filterDto.SortProperty = "TestId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ProgramName".ToLower())
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

                var dataList = _mapper.Map<List<ProgramTestDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(ProgramTestFilterDto filterDto = null)
        {
            try
            {
                // Filter by program ids
                var progIds = filterDto.ProgramIds?.Split(',')?.Select(int.Parse)?.ToList();
                if (filterDto.FilterByProgramIds == true)
                {
                    if (progIds == null || progIds.Count == 0 || !progIds.Any(x => x != 0))
                    {
                        _response.Data = new List<ProgramTestDto>();
                        _response.IsPassed = true;
                        return _response;
                    }
                }

                var query = _programTestRepository.GetAll()
                                    .Include(x => x.TestingProtocol).ThenInclude(x => x.PatientGroup)
                                    .Include(x => x.Program)
                                    .Include(x => x.Test)
                                    .Include(x => x.TestingProtocol).ThenInclude(x => x.TestingProtocolCalculationPeriodMonths).ThenInclude(x => x.CalculationPeriodMonth)
                                    .Where(x => !x.IsDeleted && x.IsActive);

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
                    if (filterDto.TestingProtocolId > 0)
                    {
                        query = query.Where(x => x.TestingProtocolId == filterDto.TestingProtocolId);
                    }
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (filterDto.FilterByProgramIds == true)
                    {
                        if (progIds == null || progIds.Count == 0 || !progIds.Any(x => x != 0))
                        {
                            _response.Data = new List<ProgramTestDto>();
                            _response.IsPassed = true;
                            return _response;
                        }
                        query = query.Where(x => progIds.Contains(x.ProgramId));
                    }
                }
        
                var dataList = _mapper.Map<List<ProgramTestDto>>(query.ToList());

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
        public async Task<IResponseDTO> GetProgramTestDetails(int programTestId)
        {
            try
            {
                var programTest = await _programTestRepository.GetAll()
                                        .Include(x => x.Test)
                                        .Include(x => x.TestingProtocol)
                                        .Include(x => x.Program)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == programTestId);
                if (programTest == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var programTestDto = _mapper.Map<ProgramTestDto>(programTest);

                _response.Data = programTestDto;
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
        public async Task<IResponseDTO> CreateProgramTest(ProgramTestDto programTestDto)
        {
            try
            {
                var programTest = _mapper.Map<Data.DbModels.DiseaseProgramSchema.ProgramTest>(programTestDto);

                // Set relation variables with null to avoid unexpected EF errors
                programTest.Program = null;
                programTest.Test = null;
                programTest.Creator = null;
                programTest.Test = null;
                programTest.Program = null;
                programTest.TestingProtocol.PatientGroup = null;
                programTest.TestingProtocol.Test = null;
                programTest.TestingProtocol.CalculationPeriod = null;
                programTest.TestingProtocol.TestingProtocolCalculationPeriodMonths.Select(y =>
                {
                    y.TestingProtocol = null;
                    y.CalculationPeriodMonth = null;
                    return y;
                }).ToList();

                // Add to the DB
                await _programTestRepository.AddAsync(programTest);

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
        public async Task<IResponseDTO> UpdateProgramTest(ProgramTestDto programTestDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var programTestExist = await _programTestRepository.GetAll()
                                                .Include(x => x.TestingProtocol).ThenInclude(x => x.TestingProtocolCalculationPeriodMonths)
                                                .FirstOrDefaultAsync(x => x.Id == programTestDto.Id);
                if (programTestExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && programTestExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var programTest = _mapper.Map<Data.DbModels.DiseaseProgramSchema.ProgramTest>(programTestDto);

                // Handle Months
                if (programTest.TestingProtocol.CalculationPeriodId != programTestExist.TestingProtocol.CalculationPeriodId)
                {
                    programTest.TestingProtocol.TestingProtocolCalculationPeriodMonths = programTestExist.TestingProtocol.TestingProtocolCalculationPeriodMonths
                                                                                            .Select(x => { x.IsDeleted = true; return x; })
                                                                                            .ToList();
                }
                else
                {
                    programTest.TestingProtocol.TestingProtocolCalculationPeriodMonths = programTestExist.TestingProtocol.TestingProtocolCalculationPeriodMonths.ToList();
                    programTest.TestingProtocol.TestingProtocolCalculationPeriodMonths
                            .Select(x => { x.Value = programTestDto.TestingProtocolDto.TestingProtocolCalculationPeriodMonthDtos.First(y => y.CalculationPeriodMonthId == x.CalculationPeriodMonthId).Value; return x; })
                            .ToList();
                }

                // Set relation variables with null to avoid unexpected EF errors
                programTest.ProgramId = programTestExist.ProgramId;
                programTest.TestingProtocol.Id = programTestExist.TestingProtocolId;
                programTest.Test = null;
                // Set relation variables with null to avoid unexpected EF errors
                programTest.Program = null;
                programTest.Creator = null;
                programTest.Program = null;
                programTest.TestingProtocol.PatientGroup = null;
                programTest.TestingProtocol.Test = null;
                programTest.TestingProtocol.CalculationPeriod = null;
                programTest.TestingProtocol.TestingProtocolCalculationPeriodMonths.Select(y =>
                {
                    y.TestingProtocolId = programTestExist.TestingProtocolId;
                    y.TestingProtocol = null;
                    y.CalculationPeriodMonth = null;
                    return y;
                }).ToList();

                _programTestRepository.Update(programTest);

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
        public async Task<IResponseDTO> UpdateIsActive(int programTestId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var programTest = await _programTestRepository.GetFirstAsync(x => x.Id == programTestId);
                if (programTest == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && programTest.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                programTest.IsActive = IsActive;
                programTest.UpdatedBy = LoggedInUserId;
                programTest.UpdatedOn = DateTime.Now;

                // Update on the Database
                _programTestRepository.Update(programTest);

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
        public async Task<IResponseDTO> RemoveProgramTest(int programTestId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var programTest = await _programTestRepository.GetFirstOrDefaultAsync(x => x.Id == programTestId);
                if (programTest == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && programTest.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                programTest.IsDeleted = true;
                programTest.UpdatedBy = LoggedInUserId;
                programTest.UpdatedOn = DateTime.Now;

                // Update on the Database
                _programTestRepository.Update(programTest);

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
        public GeneratedFile ExportProgramTests(int? pageIndex = null, int? pageSize = null, ProgramTestFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.ProgramTest> query = null;
            try
            {
                query = _programTestRepository.GetAll()
                      .Include(x => x.Test)
                      .Include(x => x.TestingProtocol)
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
                    if (filterDto.TestingProtocolId > 0)
                    {
                        query = query.Where(x => x.TestingProtocolId == filterDto.TestingProtocolId);
                    }
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Test.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ExportProgramTestDto>>(query.ToList());
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
        public async Task<IResponseDTO> ImportProgramTests(List<ProgramTestDto> programTestDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var programTests_database = _programTestRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var programTests = _mapper.Map<List<Data.DbModels.DiseaseProgramSchema.ProgramTest>>(programTestDtos);

                // vars
                var newProgramTests = new List<Data.DbModels.DiseaseProgramSchema.ProgramTest>();
                var updatedProgramTests = new List<Data.DbModels.DiseaseProgramSchema.ProgramTest>();

                // Get new and updated programTests
                foreach (var item in programTests)
                {
                    var foundProgramTest = programTests_database.FirstOrDefault(x => x.ProgramId == item.ProgramId && x.TestingProtocolId == item.TestingProtocolId);
                    if (foundProgramTest == null)
                    {
                        newProgramTests.Add(item);
                    }
                    else
                    {
                        updatedProgramTests.Add(item);
                    }
                }

                if (!IsSuperAdmin)
                {
                    updatedProgramTests = updatedProgramTests.Where(x => x.CreatedBy == LoggedInUserId).ToList();
                }

                // Add the new object to the database
                if (newProgramTests.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newProgramTests.Select(x => 
                    {
                        x.Program = null;
                        x.TestingProtocol = null;
                        x.Test = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    });
                    await _programTestRepository.AddRangeAsync(newProgramTests);
                }

                // Update the existing objects with the new values
                if (updatedProgramTests.Count() > 0)
                {
                    foreach (var item in updatedProgramTests)
                    {
                        var fromDatabase = programTests_database.FirstOrDefault(x => x.ProgramId == item.ProgramId 
                                                                    && x.TestingProtocolId == item.TestingProtocolId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.ProgramId = item.ProgramId;
                        fromDatabase.TestingProtocolId = item.TestingProtocolId;
                        fromDatabase.TestId = item.TestId;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Program = null;
                        fromDatabase.TestingProtocol = null;
                        fromDatabase.Test = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _programTestRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newProgramTests.Count();
                var numberOfUpdated = updatedProgramTests.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = programTestDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = programTestDtos.Count - (numberOfAdded + numberOfUpdated)
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
        public bool IsProgramTestPatientGroupUnique(ProgramTestDto programTestDto)
        {
            var searchResult = _programTestRepository.GetAll().Include(x => x.TestingProtocol)
                                            .Where(x =>
                                                !x.IsDeleted
                                                && x.Id != programTestDto.Id
                                                && x.ProgramId == programTestDto.ProgramId
                                                && x.TestingProtocolId == programTestDto.TestingProtocolId
                                                && x.TestingProtocol.PatientGroupId == programTestDto.TestingProtocolDto.PatientGroupId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsProgramTestTestingProtocolUnique(ProgramTestDto programTestDto)
        {
            var searchResult = _programTestRepository.GetAll().Include(x => x.TestingProtocol)
                                            .Where(x =>
                                                !x.IsDeleted
                                                && x.Id != programTestDto.Id
                                                && x.ProgramId == programTestDto.ProgramId
                                                && x.TestingProtocol.Name.Trim().ToLower() == programTestDto.TestingProtocolDto.Name.Trim().ToLower());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}

