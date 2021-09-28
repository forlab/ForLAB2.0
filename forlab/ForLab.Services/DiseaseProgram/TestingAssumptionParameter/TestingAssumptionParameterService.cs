using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.DiseaseProgram.TestingAssumptionParameter;
using ForLab.Repositories.DiseaseProgram.TestingAssumptionParameter;
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

namespace ForLab.Services.DiseaseProgram.TestingAssumptionParameter
{
    public class TestingAssumptionParameterService : GService<TestingAssumptionParameterDto, Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter, ITestingAssumptionParameterRepository>, ITestingAssumptionParameterService
    {
        private readonly ITestingAssumptionParameterRepository _testingAssumptionParameterRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportTestingAssumptionParameterDto> _fileService;
        private readonly IGeneralService _generalService;
        public TestingAssumptionParameterService(IMapper mapper,
            IResponseDTO response,
            ITestingAssumptionParameterRepository testingAssumptionParameterRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportTestingAssumptionParameterDto> fileService,
            IGeneralService generalService) : base(testingAssumptionParameterRepository, mapper)
        {
            _testingAssumptionParameterRepository = testingAssumptionParameterRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, TestingAssumptionParameterFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter> query = null;
            try
            {
                query = _testingAssumptionParameterRepository.GetAll()
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

                var dataList = _mapper.Map<List<TestingAssumptionParameterDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(TestingAssumptionParameterFilterDto filterDto = null)
        {
            try
            {
                var query = _testingAssumptionParameterRepository.GetAll(x => !x.IsDeleted && x.IsActive);


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

                query = query.Select(i => new Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<TestingAssumptionParameterDrp>>(query.ToList());

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
        public IResponseDTO GetAllTestingAssumptionsForForcast(string programIds)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter> query = null;
            try
            {
                // Filter by program ids
                var progIds = programIds?.Split(',')?.Select(int.Parse)?.ToList();
                if (progIds == null || progIds.Count == 0 || !progIds.Any(x => x != 0))
                {
                    _response.Data = new List<GroupTestingAssumptionParameterDto>();
                    _response.IsPassed = true;
                    return _response;
                }

                query = _testingAssumptionParameterRepository.GetAll()
                                    .Include(x => x.Program)
                                    .Where(x => !x.IsDeleted);

                query = query.Where(x => progIds.Contains(x.ProgramId));
                query = query.OrderByDescending(x => x.Id);

                var dataList = _mapper.Map<List<TestingAssumptionParameterDto>>(query.ToList());
                var grouped = new List<GroupTestingAssumptionParameterDto>();
                var groupdByName = dataList.GroupBy(x => x.ProgramName);
                foreach (var item in groupdByName)
                {
                    grouped.Add(new GroupTestingAssumptionParameterDto
                    {
                        ProgramName = item.Key,
                        TestingAssumptionParameterDtos = item.ToList()
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
        public async Task<IResponseDTO> GetTestingAssumptionParameterDetails(int testingAssumptionParameterId)
        {
            try
            {
                var testingAssumptionParameter = await _testingAssumptionParameterRepository.GetAll()
                                        .Include(x => x.ProgramId)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == testingAssumptionParameterId);
                if (testingAssumptionParameter == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var testingAssumptionParameterDto = _mapper.Map<TestingAssumptionParameterDto>(testingAssumptionParameter);

                _response.Data = testingAssumptionParameterDto;
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
        public async Task<IResponseDTO> CreateTestingAssumptionParameter(TestingAssumptionParameterDto testingAssumptionParameterDto)
        {
            try
            {
                var testingAssumptionParameter = _mapper.Map<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter>(testingAssumptionParameterDto);

                // Set relation variables with null to avoid unexpected EF errors
                testingAssumptionParameter.Program = null;

                // Add to the DB
                await _testingAssumptionParameterRepository.AddAsync(testingAssumptionParameter);

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
        public async Task<IResponseDTO> UpdateTestingAssumptionParameter(TestingAssumptionParameterDto testingAssumptionParameterDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var testingAssumptionParameterExist = await _testingAssumptionParameterRepository.GetFirstAsync(x => x.Id == testingAssumptionParameterDto.Id);
                if (testingAssumptionParameterExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && testingAssumptionParameterExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var testingAssumptionParameter = _mapper.Map<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter>(testingAssumptionParameterDto);

                // Set relation variables with null to avoid unexpected EF errors

                testingAssumptionParameter.Program = null;


                _testingAssumptionParameterRepository.Update(testingAssumptionParameter);

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
        public async Task<IResponseDTO> UpdateIsActive(int testingAssumptionParameterId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var testingAssumptionParameter = await _testingAssumptionParameterRepository.GetFirstAsync(x => x.Id == testingAssumptionParameterId);
                if (testingAssumptionParameter == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && testingAssumptionParameter.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                testingAssumptionParameter.IsActive = IsActive;
                testingAssumptionParameter.UpdatedBy = LoggedInUserId;
                testingAssumptionParameter.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                testingAssumptionParameter.ForecastTestingAssumptionValues = null;

                // Update on the Database
                _testingAssumptionParameterRepository.Update(testingAssumptionParameter);

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
        public async Task<IResponseDTO> RemoveTestingAssumptionParameter(int testingAssumptionParameterId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var testingAssumptionParameter = await _testingAssumptionParameterRepository.GetFirstOrDefaultAsync(x => x.Id == testingAssumptionParameterId);
                if (testingAssumptionParameter == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && testingAssumptionParameter.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                testingAssumptionParameter.IsDeleted = true;
                testingAssumptionParameter.UpdatedBy = LoggedInUserId;
                testingAssumptionParameter.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                testingAssumptionParameter.ForecastTestingAssumptionValues = null;

                // Update on the Database
                _testingAssumptionParameterRepository.Update(testingAssumptionParameter);

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
        public GeneratedFile ExportTestingAssumptionParameters(int? pageIndex = null, int? pageSize = null, TestingAssumptionParameterFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter> query = null;
            try
            {
                query = _testingAssumptionParameterRepository.GetAll()
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

                var dataList = _mapper.Map<List<ExportTestingAssumptionParameterDto>>(query.ToList());
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
        public async Task<IResponseDTO> ImportTestingAssumptionParameters(List<TestingAssumptionParameterDto> testingAssumptionParameterDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var testingAssumptionParameters_database = _testingAssumptionParameterRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var testingAssumptionParameters = _mapper.Map<List<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter>>(testingAssumptionParameterDtos);

                // vars
                var newTestingAssumptionParameters = new List<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter>();
                var updatedTestingAssumptionParameters = new List<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter>();

                // Get new and updated testingAssumptionParameters
                foreach (var item in testingAssumptionParameters)
                {
                    var foundTestingAssumptionParameter = testingAssumptionParameters_database.FirstOrDefault(x => x.ProgramId == item.ProgramId
                                                                         && x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                    if (foundTestingAssumptionParameter == null)
                    {
                        newTestingAssumptionParameters.Add(item);
                    }
                    else
                    {
                        updatedTestingAssumptionParameters.Add(item);
                    }
                }

                if (!IsSuperAdmin)
                {
                    updatedTestingAssumptionParameters = updatedTestingAssumptionParameters.Where(x => x.CreatedBy == LoggedInUserId).ToList();
                }

                // Add the new object to the database
                if (newTestingAssumptionParameters.Count() > 0)
                {  
                    // Set relation variables with null to avoid unexpected EF errors
                    newTestingAssumptionParameters.Select(x =>
                    {
                        x.ForecastTestingAssumptionValues = null;
                        x.Program = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _testingAssumptionParameterRepository.AddRangeAsync(newTestingAssumptionParameters);
                }

                // Update the existing objects with the new values
                if (updatedTestingAssumptionParameters.Count() > 0)
                {
                    foreach (var item in updatedTestingAssumptionParameters)
                    {
                        var fromDatabase = testingAssumptionParameters_database.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name?.ToLower()?.Trim()
                                                        && x.ProgramId == item.ProgramId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Name = item.Name;
                        fromDatabase.ProgramId = item.ProgramId;
                        // boolen variables
                        fromDatabase.IsNumeric = item.IsNumeric;
                        fromDatabase.IsPercentage = item.IsPercentage;
                        fromDatabase.IsPositive = item.IsPositive;
                        fromDatabase.IsNegative = item.IsNegative;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.ForecastTestingAssumptionValues = null;
                        fromDatabase.Program = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _testingAssumptionParameterRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newTestingAssumptionParameters.Count();
                var numberOfUpdated = updatedTestingAssumptionParameters.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = testingAssumptionParameterDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = testingAssumptionParameterDtos.Count - (numberOfAdded + numberOfUpdated)
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
        public bool IsNameUnique(TestingAssumptionParameterDto testingAssumptionParameterDto)
        {
            var searchResult = _testingAssumptionParameterRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != testingAssumptionParameterDto.Id
                                                && x.Name.ToLower().Trim() == testingAssumptionParameterDto.Name.ToLower().Trim()
                                                && x.ProgramId == testingAssumptionParameterDto.ProgramId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int testingAssumptionParameterId)
        {
            try
            {
                var testingAssumptionParameter = await _testingAssumptionParameterRepository.GetAll()
                                        .Include(x => x.Program)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == testingAssumptionParameterId);
                if (testingAssumptionParameter == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (testingAssumptionParameter.ForecastTestingAssumptionValues.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Testing Assumption Parameter cannot be deleted or deactivated where it contains 'Forecast Testing Assumption Values'";
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
