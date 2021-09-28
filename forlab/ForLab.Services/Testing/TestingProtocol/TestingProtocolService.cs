using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Testing.TestingProtocol;
using ForLab.Repositories.Testing.TestingProtocol;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.Testing.TestingProtocol
{
    public class TestingProtocolService : GService<TestingProtocolDto, Data.DbModels.TestingSchema.TestingProtocol, ITestingProtocolRepository>, ITestingProtocolService
    {
        private readonly ITestingProtocolRepository _testingProtocolRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportTestingProtocolDto> _fileService;

        public TestingProtocolService(IMapper mapper,
            IResponseDTO response,
            ITestingProtocolRepository testingProtocolRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportTestingProtocolDto> fileService) : base(testingProtocolRepository, mapper)
        {
            _testingProtocolRepository = testingProtocolRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, TestingProtocolFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.TestingSchema.TestingProtocol> query = null;
            try
            {
                query = _testingProtocolRepository.GetAll()
                                    .Include(x => x.Test)
                                    .Include(x => x.PatientGroup)
                                    .Include(x => x.CalculationPeriod)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId);
                    }

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.PatientGroupId > 0)
                    {
                        query = query.Where(x => x.PatientGroupId == filterDto.PatientGroupId);
                    }
                    if (filterDto.BaseLine > 0)
                    {
                        query = query.Where(x => x.BaseLine == filterDto.BaseLine);
                    }
                    if (filterDto.CalculationPeriodId > 0)
                    {
                        query = query.Where(x => x.CalculationPeriodId == filterDto.CalculationPeriodId);
                    }
                    if (filterDto.TestAfterFirstYear > 0)
                    {
                        query = query.Where(x => x.TestAfterFirstYear == filterDto.TestAfterFirstYear);
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "TestName".ToLower())
                    {
                        filterDto.SortProperty = "TestId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "PatientGroupName".ToLower())
                    {
                        filterDto.SortProperty = "PatientGroupId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<TestingProtocolDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(TestingProtocolFilterDto filterDto = null)
        {
            try
            {
                var query = _testingProtocolRepository.GetAll().Where(x => !x.IsDeleted && x.IsActive);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId);
                    }

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.PatientGroupId > 0)
                    {
                        query = query.Where(x => x.PatientGroupId == filterDto.PatientGroupId);
                    }
                    if (filterDto.BaseLine > 0)
                    {
                        query = query.Where(x => x.BaseLine == filterDto.BaseLine);
                    }
                    if (filterDto.CalculationPeriodId > 0)
                    {
                        query = query.Where(x => x.CalculationPeriodId == filterDto.CalculationPeriodId);
                    }
                    if (filterDto.TestAfterFirstYear > 0)
                    {
                        query = query.Where(x => x.TestAfterFirstYear == filterDto.TestAfterFirstYear);
                    }
                }
                query = query.OrderBy(x => x.TestId);
                query = query.Select(i => new Data.DbModels.TestingSchema.TestingProtocol()
                {
                    Id = i.Id,
                    Name = i.Name,
                    BaseLine = i.BaseLine,
                    CalculationPeriod = i.CalculationPeriod,
                    Test = i.Test
                });
                var dataList = _mapper.Map<List<TestingProtocolDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetTestingProtocolDetails(int testingProtocolId)
        {
            try
            {
                var testingProtocol = await _testingProtocolRepository.GetAll()
                                        .Include(x => x.TestingProtocolCalculationPeriodMonths).ThenInclude(x => x.CalculationPeriodMonth)
                                        .Include(x => x.Test)
                                        .Include(x => x.CalculationPeriod)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == testingProtocolId);
                if (testingProtocol == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                testingProtocol.TestingProtocolCalculationPeriodMonths = testingProtocol.TestingProtocolCalculationPeriodMonths.Where(x => !x.IsDeleted).ToList();

                var testingProtocolDto = _mapper.Map<TestingProtocolDto>(testingProtocol);

                _response.Data = testingProtocolDto;
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
        public GeneratedFile ExportTestingProtocols(int? pageIndex = null, int? pageSize = null, TestingProtocolFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.TestingSchema.TestingProtocol> query = null;
            try
            {
                query = _testingProtocolRepository.GetAll()
                                    .Include(x => x.Test)
                                    .Include(x => x.PatientGroup)
                                    .Include(x => x.CalculationPeriod)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId);
                    }

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.PatientGroupId > 0)
                    {
                        query = query.Where(x => x.PatientGroupId == filterDto.PatientGroupId);
                    }
                    if (filterDto.BaseLine > 0)
                    {
                        query = query.Where(x => x.BaseLine == filterDto.BaseLine);
                    }
                    if (filterDto.CalculationPeriodId > 0)
                    {
                        query = query.Where(x => x.CalculationPeriodId == filterDto.CalculationPeriodId);
                    }
                    if (filterDto.TestAfterFirstYear > 0)
                    {
                        query = query.Where(x => x.TestAfterFirstYear == filterDto.TestAfterFirstYear);
                    }
                }

                query = query.OrderByDescending(x => x.CreatedOn);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "TestName".ToLower())
                    {
                        filterDto.SortProperty = "TestId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "PatientGroupName".ToLower())
                    {
                        filterDto.SortProperty = "PatientGroupId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportTestingProtocolDto>>(query.ToList());

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
    }
}
