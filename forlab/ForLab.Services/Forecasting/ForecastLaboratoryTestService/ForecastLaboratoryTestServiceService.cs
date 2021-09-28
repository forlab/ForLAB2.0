using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastLaboratoryTestService;
using ForLab.Repositories.Forecasting.ForecastLaboratoryTestService;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Forecasting.ForecastLaboratoryTestService
{
    public class ForecastLaboratoryTestServiceService : GService<ForecastLaboratoryTestServiceDto, Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService, IForecastLaboratoryTestServiceRepository>, IForecastLaboratoryTestServiceService
    {
        private readonly IForecastLaboratoryTestServiceRepository _forecastLaboratoryTestServiceRepository;
        private readonly IResponseDTO _response;
        private readonly IFileService<ExportForecastLaboratoryTestServiceDto> _fileService;

        public ForecastLaboratoryTestServiceService(IMapper mapper,
            IResponseDTO response,
            IForecastLaboratoryTestServiceRepository forecastLaboratoryTestServiceRepository,
            IFileService<ExportForecastLaboratoryTestServiceDto> fileService) : base(forecastLaboratoryTestServiceRepository, mapper)
        {
            _forecastLaboratoryTestServiceRepository = forecastLaboratoryTestServiceRepository;
            _response = response;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryTestServiceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService> query = null;
            try
            {
                query = _forecastLaboratoryTestServiceRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Laboratory)
                                    .Include(x => x.Test)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Period))
                    {
                        query = query.Where(x => x.Period.Trim().ToLower().Contains(filterDto.Period.Trim().ToLower()));
                    }
                    if (filterDto.ForecastInfoId > 0)
                    {
                        query = query.Where(x => x.ForecastInfoId == filterDto.ForecastInfoId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
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
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ForecastLaboratoryTestServiceDto>>(query.ToList());

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
        public GeneratedFile ExportForecastLaboratoryTestService(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryTestServiceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService> query = null;
            try
            {
                query = _forecastLaboratoryTestServiceRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Laboratory)
                                    .Include(x => x.Test)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Period))
                    {
                        query = query.Where(x => x.Period.Trim().ToLower().Contains(filterDto.Period.Trim().ToLower()));
                    }
                    if (filterDto.ForecastInfoId > 0)
                    {
                        query = query.Where(x => x.ForecastInfoId == filterDto.ForecastInfoId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
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
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }
                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportForecastLaboratoryTestServiceDto>>(query.ToList());

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
        public IResponseDTO ForecastLaboratoryTestServicesChart(int forecastInfoId, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService> query = null;
            try
            {
                query = _forecastLaboratoryTestServiceRepository.GetAll(x => !x.IsDeleted && x.IsActive && x.ForecastInfoId == forecastInfoId);
                query = query.Select(i => new Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService() { Period = i.Period, AmountForecasted = i.AmountForecasted });

                query = query.OrderByDescending(x => x.Period);
                var total = query.Count();

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ForecastLaboratoryTestServiceDto>>(query.ToList());
                var lables = dataList.Select(x => x.Period);
                var data = dataList.Select(x => x.AmountForecasted);

                _response.Data = new
                {
                    List = new
                    {
                        Lables = lables,
                        Data = data
                    },
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
    }
}
