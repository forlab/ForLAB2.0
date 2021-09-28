using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Testing.Test;
using ForLab.Repositories.Testing.Test;
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
using ForLab.DTO.Testing.TestingProtocol;
using ForLab.Services.Global.General;

namespace ForLab.Services.Testing.Test
{
    public class TestService : GService<TestDto, Data.DbModels.TestingSchema.Test, ITestRepository>, ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportTestDto> _fileService;
        private readonly IGeneralService _generalService;
        public TestService(IMapper mapper,
            IResponseDTO response,
            ITestRepository testRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportTestDto> fileService,
            IGeneralService generalService) : base(testRepository, mapper)
        {
            _testRepository = testRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, TestFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.TestingSchema.Test> query = null;
            try
            {
                query = _testRepository.GetAll()
                                    .Include(x => x.TestingArea)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        //var createdBy = _generalService.SuperAdminIds();
                        //createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId || x.Shared);
                    }

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.TestingAreaId > 0)
                    {
                        query = query.Where(x => x.TestingAreaId == filterDto.TestingAreaId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ShortName))
                    {
                        query = query.Where(x => x.ShortName.Trim().ToLower().Contains(filterDto.ShortName.Trim().ToLower()));
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "TestingAreaName".ToLower())
                    {
                        filterDto.SortProperty = "TestingAreaId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<TestDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(TestFilterDto filterDto = null)
        {
            try
            {
                var query = _testRepository.GetAll(x => !x.IsDeleted && x.IsActive);


                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        //var createdBy = _generalService.SuperAdminIds();
                        //createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId || x.Shared);
                    }

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.TestingAreaId > 0)
                    {
                        query = query.Where(x => x.TestingAreaId == filterDto.TestingAreaId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ShortName))
                    {
                        query = query.Where(x => x.ShortName.Trim().ToLower().Contains(filterDto.ShortName.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.TestingSchema.Test() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<TestDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetTestDetails(int testId)
        {
            try
            {
                var test = await _testRepository.GetAll()
                                        .Include(x => x.TestingArea)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == testId);
                if (test == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var testDto = _mapper.Map<TestDto>(test);

                _response.Data = testDto;
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
        public async Task<IResponseDTO> CreateTest(TestDto testDto)
        {
            try
            {
                var test = _mapper.Map<Data.DbModels.TestingSchema.Test>(testDto);

                // Set relation variables with null to avoid unexpected EF errors
                test.TestingArea = null;

                // Add to the DB
                await _testRepository.AddAsync(test);

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
        public async Task<IResponseDTO> UpdateTest(TestDto testDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var testExist = await _testRepository.GetFirstAsync(x => x.Id == testDto.Id);
                if (testExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && testExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var test = _mapper.Map<Data.DbModels.TestingSchema.Test>(testDto);

                // Set relation variables with null to avoid unexpected EF errors

                test.TestingArea = null;


                _testRepository.Update(test);

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
        public async Task<IResponseDTO> UpdateIsActive(int testId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var test = await _testRepository.GetFirstAsync(x => x.Id == testId);
                if (test == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && test.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                test.IsActive = IsActive;
                test.UpdatedBy = LoggedInUserId;
                test.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                test.ProductUsages = null;
                test.TestingProtocols = null;
                test.LaboratoryTestServices = null;
                test.ForecastTests = null;

                // Update on the Database
                _testRepository.Update(test);

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
        public async Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var items = _testRepository.GetAll(x => ids.Contains(x.Id)).ToList();

                if (!IsSuperAdmin && items.Select(x => x.CreatedBy).Any(x => x != LoggedInUserId))
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                var objectsToUpdate = items?.Where(x => x.IsActive != isActive).Select(x =>
                {
                    x.IsActive = isActive;
                    x.UpdatedBy = LoggedInUserId;
                    x.UpdatedOn = DateTime.Now;
                    return x;
                }).ToList();


                // Update on the Database
                foreach (var item in objectsToUpdate)
                {
                    _testRepository.Update(item);
                }

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "There is no changes to save";
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
        public async Task<IResponseDTO> UpdateSharedForSelected(List<int> ids, bool shared, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var items = _testRepository.GetAll(x => ids.Contains(x.Id)).ToList();

                // Update IsActive value
                var objectsToUpdate = items?.Where(x => x.Shared != shared).Select(x =>
                {
                    x.Shared = shared;
                    x.UpdatedBy = LoggedInUserId;
                    x.UpdatedOn = DateTime.Now;
                    return x;
                }).ToList();


                // Update on the Database
                foreach (var item in objectsToUpdate)
                {
                    _testRepository.Update(item);
                }

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "There is no changes to save";
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
        public async Task<IResponseDTO> RemoveTest(int testId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var test = await _testRepository.GetFirstOrDefaultAsync(x => x.Id == testId);
                if (test == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && test.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                test.IsDeleted = true;
                test.UpdatedBy = LoggedInUserId;
                test.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                test.ProductUsages = null;
                test.TestingProtocols = null;
                test.LaboratoryTestServices = null;
                test.ForecastTests = null;

                // Update on the Database
                _testRepository.Update(test);

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
        public GeneratedFile ExportTests(int? pageIndex = null, int? pageSize = null, TestFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.TestingSchema.Test> query = null;
            try
            {
                query = _testRepository.GetAll()
                                    .Include(x => x.TestingArea)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        //var createdBy = _generalService.SuperAdminIds();
                        //createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId || x.Shared);
                    }

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.TestingAreaId > 0)
                    {
                        query = query.Where(x => x.TestingAreaId == filterDto.TestingAreaId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ShortName))
                    {
                        query = query.Where(x => x.ShortName.Trim().ToLower().Contains(filterDto.ShortName.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Name);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "TestingAreaName".ToLower())
                    {
                        filterDto.SortProperty = "TestingAreaId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportTestDto>>(query.ToList());

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
        public async Task<IResponseDTO> ImportTests(List<TestDto> testDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var tests_database = _testRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var tests = _mapper.Map<List<Data.DbModels.TestingSchema.Test>>(testDtos);

                // vars
                var newTests = new List<Data.DbModels.TestingSchema.Test>();
                var updatedTests = new List<Data.DbModels.TestingSchema.Test>();

                // Get new and updated tests
                foreach (var item in tests)
                {
                    var foundTest = tests_database.FirstOrDefault(x => x.TestingAreaId == item.TestingAreaId
                                                                    && x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                    if (foundTest == null)
                    {
                        newTests.Add(item);
                    }
                    else
                    {
                        updatedTests.Add(item);
                    }
                }

                if (!IsSuperAdmin)
                {
                    updatedTests = updatedTests.Where(x => x.CreatedBy == LoggedInUserId).ToList();
                }

                // Add the new object to the database
                if (newTests.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newTests.Select(x =>
                    {
                        x.ProductUsages = null;
                        x.TestingProtocols = null;
                        x.LaboratoryTestServices = null;
                        x.ForecastTests = null;
                        x.ForecastMorbidityTestingProtocolMonths = null;
                        x.TestingArea = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _testRepository.AddRangeAsync(newTests);
                }

                // Update the existing objects with the new values
                if (updatedTests.Count() > 0)
                {
                    foreach (var item in updatedTests)
                    {
                        var fromDatabase = tests_database.FirstOrDefault(x => x.TestingAreaId == item.TestingAreaId
                                                                    && x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Name = item.Name;
                        fromDatabase.ShortName = item.ShortName;
                        fromDatabase.TestingAreaId = item.TestingAreaId;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.ProductUsages = null;
                        fromDatabase.TestingProtocols = null;
                        fromDatabase.LaboratoryTestServices = null;
                        fromDatabase.ForecastTests = null;
                        fromDatabase.ForecastMorbidityTestingProtocolMonths = null;
                        fromDatabase.TestingArea = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _testRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newTests.Count();
                var numberOfUpdated = updatedTests.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = testDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = testDtos.Count - (numberOfAdded + numberOfUpdated)
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
        public bool IsNameUnique(TestDto testDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _testRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != testDto.Id
                                                && x.Name.ToLower().Trim() == testDto.Name.ToLower().Trim()
                                                && x.TestingAreaId == testDto.TestingAreaId);
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
        public async Task<IResponseDTO> IsUsed(int testId)
        {
            try
            {
                var test = await _testRepository.GetAll()
                                        .Include(x => x.TestingArea)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == testId);
                if (test == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (test.ProductUsages.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Test cannot be deleted or deactivated where it contains 'Days of Product Usages'";
                    return _response;
                }
                if (test.TestingProtocols.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Test cannot be deleted or deactivated where it contains 'Testing Protocols'";
                    return _response;
                }
                if (test.LaboratoryTestServices.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Test cannot be deleted or deactivated where it contains 'Laboratory Test Services'";
                    return _response;
                }
                if (test.ForecastTests.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Test cannot be deleted or deactivated where it contains 'Forecast Tests '";
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
