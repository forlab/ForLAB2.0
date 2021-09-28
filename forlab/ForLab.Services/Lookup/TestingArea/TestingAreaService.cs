using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Lookup.TestingArea;
using ForLab.Repositories.Lookup.TestingArea;
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

namespace ForLab.Services.Lookup.TestingArea
{
    public class TestingAreaService : GService<TestingAreaDto, Data.DbModels.LookupSchema.TestingArea, ITestingAreaRepository>, ITestingAreaService
    {
        private readonly ITestingAreaRepository _testingAreaRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportTestingAreaDto> _fileService;
        private readonly IGeneralService _generalService;
        public TestingAreaService(IMapper mapper,
            IResponseDTO response,
            ITestingAreaRepository testingAreaRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportTestingAreaDto> fileService,
            IGeneralService generalService) : base(testingAreaRepository, mapper)
        {
            _testingAreaRepository = testingAreaRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, TestingAreaFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.TestingArea> query = null;
            try
            {
                query = _testingAreaRepository.GetAll()
                                    .Include(x => x.Instruments)
                                    .Include(x => x.Tests)
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
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<TestingAreaDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(TestingAreaFilterDto filterDto = null)
        {
            try
            {
                var query = _testingAreaRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.LookupSchema.TestingArea() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<TestingAreaDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetTestingAreaDetails(int testingAreaId)
        {
            try
            {
                var testingArea = await _testingAreaRepository.GetAll()
                                        .Include(x => x.Instruments)
                                        .Include(x => x.Name)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == testingAreaId);
                if (testingArea == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var testingAreaDto = _mapper.Map<TestingAreaDto>(testingArea);

                _response.Data = testingAreaDto;
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
        public async Task<IResponseDTO> CreateTestingArea(TestingAreaDto testingAreaDto)
        {
            try
            {
                var testingArea = _mapper.Map<Data.DbModels.LookupSchema.TestingArea>(testingAreaDto);

                // Set relation variables with null to avoid unexpected EF errors
                // testingArea.TestingProtocols = null;
                testingArea.Instruments = null;
                testingArea.Tests = null;


                // Add to the DB
                await _testingAreaRepository.AddAsync(testingArea);

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
        public async Task<IResponseDTO> UpdateTestingArea(TestingAreaDto testingAreaDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var testingAreaExist = await _testingAreaRepository.GetFirstAsync(x => x.Id == testingAreaDto.Id);
                if (testingAreaExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && testingAreaExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var testingArea = _mapper.Map<Data.DbModels.LookupSchema.TestingArea>(testingAreaDto);

                // Set relation variables with null to avoid unexpected EF errors
                testingArea.Tests = null;
                testingArea.Instruments = null;


                _testingAreaRepository.Update(testingArea);

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
        public async Task<IResponseDTO> UpdateIsActive(int testingAreaId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var testingArea = await _testingAreaRepository.GetFirstAsync(x => x.Id == testingAreaId);
                if (testingArea == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && testingArea.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                testingArea.IsActive = IsActive;
                testingArea.UpdatedBy = LoggedInUserId;
                testingArea.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                testingArea.Instruments = null;
                testingArea.Tests = null;

                // Update on the Database
                _testingAreaRepository.Update(testingArea);

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
                var items = _testingAreaRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _testingAreaRepository.Update(item);
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
                var items = _testingAreaRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _testingAreaRepository.Update(item);
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
        public async Task<IResponseDTO> RemoveTestingArea(int testingAreaId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var testingArea = await _testingAreaRepository.GetFirstOrDefaultAsync(x => x.Id == testingAreaId);
                if (testingArea == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && testingArea.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                testingArea.IsDeleted = true;
                testingArea.UpdatedBy = LoggedInUserId;
                testingArea.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                testingArea.Instruments = null;
                testingArea.Tests = null;

                // Update on the Database
                _testingAreaRepository.Update(testingArea);

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
        public async Task<IResponseDTO> ImportTestingAreas(List<TestingAreaDto> testingAreaDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var testingAreas_database = _testingAreaRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var testingAreas = _mapper.Map<List<Data.DbModels.LookupSchema.TestingArea>>(testingAreaDtos);

                // vars
                var names_database = testingAreas_database.Select(x => x.Name.ToLower().Trim());
                var names_dto = testingAreas.Select(x => x.Name.ToLower().Trim());
                // Get the new ones that their names don't exist on the database
                var newTestingAreas = testingAreas.Where(x => !names_database.Contains(x.Name.ToLower().Trim()));
                // Select the objects that their names already exist in the database
                var updatedTestingAreas = testingAreas_database.Where(x => names_dto.Contains(x.Name.ToLower().Trim()));
                if (!IsSuperAdmin)
                {
                    updatedTestingAreas = updatedTestingAreas.Where(x => x.CreatedBy == LoggedInUserId);
                }

                // Set relation variables with null to avoid unexpected EF errors
                newTestingAreas.Select(x =>
                {
                    x.Instruments = null;
                    x.Tests = null;
                    x.Creator = null;
                    x.Updator = null;
                    return x;
                }).ToList();

                // Add the new object to the database
                if (newTestingAreas.Count() > 0)
                {
                    await _testingAreaRepository.AddRangeAsync(newTestingAreas);
                }

                // Update the existing objects with the new values
                if (updatedTestingAreas.Count() > 0)
                {
                    foreach (var item in updatedTestingAreas)
                    {
                        var dto = testingAreaDtos.First(x => x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                        item.UpdatedOn = DateTime.Now;
                        item.UpdatedBy = dto.CreatedBy;
                        item.Name = dto.Name;
                        // Set relation variables with null to avoid unexpected EF errors
                        item.Instruments = null;
                        item.Tests = null;
                        item.Creator = null;
                        item.Updator = null;
                        _testingAreaRepository.Update(item);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newTestingAreas.Count();
                var numberOfUpdated = updatedTestingAreas.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.Data = new
                {
                    NumberOfUploded = testingAreaDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = testingAreaDtos.Count - (numberOfAdded + numberOfUpdated)
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
        public GeneratedFile ExportTestingAreas(int? pageIndex = null, int? pageSize = null, TestingAreaFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.TestingArea> query = null;
            try
            {
                query = _testingAreaRepository.GetAll()
                                    .Include(x => x.Instruments)
                                    .Include(x => x.Tests)
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
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Name);
                var total = query.Count();


                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportTestingAreaDto>>(query.ToList());
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
        // Validators methods
        public bool IsNameUnique(TestingAreaDto testingAreaDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _testingAreaRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != testingAreaDto.Id
                                                && x.Name.ToLower().Trim() == testingAreaDto.Name.ToLower().Trim());
            // Security Filter
            if (!IsSuperAdmin)
            {
                var createdBy = _generalService.SuperAdminIds();
                createdBy.Add(LoggedInUserId);
                searchResult = searchResult.Where(x => x.CreatedBy == null || createdBy.Contains(x.CreatedBy.Value));
            }

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int testingAreaId)
        {
            try
            {
                var testingArea = await _testingAreaRepository.GetAll()
                                        .Include(x => x.Instruments)
                                        .Include(x => x.Tests)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == testingAreaId);
                if (testingArea == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (testingArea.Instruments.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Testing Area cannot be deleted or deactivated where it contains 'Instruments'";
                    return _response;
                }
                if (testingArea.Tests.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Testing Area cannot be deleted or deactivated where it contains 'Tests'";
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
