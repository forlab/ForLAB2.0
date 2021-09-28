using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityTestingProtocolMonth;
using ForLab.Repositories.Forecasting.ForecastMorbidityTestingProtocolMonth;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Forecasting.ForecastMorbidityTestingProtocolMonth
{
    public class ForecastMorbidityTestingProtocolMonthService : GService<ForecastMorbidityTestingProtocolMonthDto, Data.DbModels.ForecastingSchema.ForecastMorbidityTestingProtocolMonth, IForecastMorbidityTestingProtocolMonthRepository>, IForecastMorbidityTestingProtocolMonthService
    {
        private readonly IForecastMorbidityTestingProtocolMonthRepository _forecastMorbidityTestingProtocolMonthRepository;
        private readonly IResponseDTO _response;
        private readonly IFileService<ExportForecastMorbidityTestingProtocolMonthDto> _fileService;

        public ForecastMorbidityTestingProtocolMonthService(IMapper mapper,
            IResponseDTO response,
            IForecastMorbidityTestingProtocolMonthRepository forecastMorbidityTestingProtocolMonthRepository,
            IFileService<ExportForecastMorbidityTestingProtocolMonthDto> fileService) : base(forecastMorbidityTestingProtocolMonthRepository, mapper)
        {
            _forecastMorbidityTestingProtocolMonthRepository = forecastMorbidityTestingProtocolMonthRepository;
            _response = response;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastMorbidityTestingProtocolMonthFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastMorbidityTestingProtocolMonth> query = null;
            try
            {
                query = _forecastMorbidityTestingProtocolMonthRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Test)
                                    .Include(x => x.TestingProtocol)
                                    .Include(x => x.Program)
                                    .Include(x => x.PatientGroup)
                                    .Include(x=> x.CalculationPeriodMonth)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.ForecastInfoId > 0)
                    {
                        query = query.Where(x => x.ForecastInfoId == filterDto.ForecastInfoId);
                    }
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.TestingProtocolId > 0)
                    {
                        query = query.Where(x => x.TestingProtocolId == filterDto.TestingProtocolId);
                    }
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (filterDto.PatientGroupId > 0)
                    {
                        query = query.Where(x => x.PatientGroupId == filterDto.PatientGroupId);
                    }
                    if (filterDto.CalculationPeriodMonthId > 0)
                    {
                        query = query.Where(x => x.CalculationPeriodMonthId == filterDto.CalculationPeriodMonthId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.PatientGroup.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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
                    else if (filterDto.SortProperty.ToLower() == "TestName".ToLower())
                    {
                        filterDto.SortProperty = "TestId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "PatientGroupName".ToLower())
                    {
                        filterDto.SortProperty = "PatientGroupId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "CalculationPeriodMonthName".ToLower())
                    {
                        filterDto.SortProperty = "CalculationPeriodMonthId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ForecastMorbidityTestingProtocolMonthDto>>(query.ToList());

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
        public GeneratedFile ExportForecastMorbidityTestingProtocolMonth(int? pageIndex = null, int? pageSize = null, ForecastMorbidityTestingProtocolMonthFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastMorbidityTestingProtocolMonth> query = null;
            try
            {
                query = _forecastMorbidityTestingProtocolMonthRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Test)
                                    .Include(x => x.TestingProtocol)
                                    .Include(x => x.Program)
                                    .Include(x => x.PatientGroup)
                                    .Include(x => x.CalculationPeriodMonth)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.ForecastInfoId > 0)
                    {
                        query = query.Where(x => x.ForecastInfoId == filterDto.ForecastInfoId);
                    }
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.TestingProtocolId > 0)
                    {
                        query = query.Where(x => x.TestingProtocolId == filterDto.TestingProtocolId);
                    }
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (filterDto.PatientGroupId > 0)
                    {
                        query = query.Where(x => x.PatientGroupId == filterDto.PatientGroupId);
                    }
                    if (filterDto.CalculationPeriodMonthId > 0)
                    {
                        query = query.Where(x => x.CalculationPeriodMonthId == filterDto.CalculationPeriodMonthId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.PatientGroup.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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
                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportForecastMorbidityTestingProtocolMonthDto>>(query.ToList());

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
