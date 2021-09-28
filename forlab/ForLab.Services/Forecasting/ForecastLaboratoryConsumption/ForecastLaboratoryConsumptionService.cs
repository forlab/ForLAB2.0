using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastLaboratoryConsumption;
using ForLab.Repositories.Forecasting.ForecastLaboratoryConsumption;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Forecasting.ForecastLaboratoryConsumption
{
    public class ForecastLaboratoryConsumptionService : GService<ForecastLaboratoryConsumptionDto, Data.DbModels.ForecastingSchema.ForecastLaboratoryConsumption, IForecastLaboratoryConsumptionRepository>, IForecastLaboratoryConsumptionService
    {
        private readonly IForecastLaboratoryConsumptionRepository _forecastLaboratoryConsumptionRepository;
        private readonly IResponseDTO _response;
        private readonly IFileService<ExportForecastLaboratoryConsumptionDto> _fileService;

        public ForecastLaboratoryConsumptionService(IMapper mapper,
            IResponseDTO response,
            IForecastLaboratoryConsumptionRepository forecastLaboratoryConsumptionRepository,
            IFileService<ExportForecastLaboratoryConsumptionDto> fileService) : base(forecastLaboratoryConsumptionRepository, mapper)
        {
            _forecastLaboratoryConsumptionRepository = forecastLaboratoryConsumptionRepository;
            _response = response;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryConsumptionFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastLaboratoryConsumption> query = null;
            try
            {
                query = _forecastLaboratoryConsumptionRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Laboratory)
                                    .Include(x => x.Product)
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
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Period))
                    {
                        query = query.Where(x => x.Period.Trim().ToLower().Contains(filterDto.Period.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Product.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ForecastLaboratoryConsumptionDto>>(query.ToList());

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
        public GeneratedFile ExportForecastLaboratoryConsumption(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryConsumptionFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastLaboratoryConsumption> query = null;
            try
            {
                query = _forecastLaboratoryConsumptionRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Laboratory)
                                    .Include(x => x.Product)
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
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Period))
                    {
                        query = query.Where(x => x.Period.Trim().ToLower().Contains(filterDto.Period.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Product.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ExportForecastLaboratoryConsumptionDto>>(query.ToList());

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
        public IResponseDTO ForecastLaboratoryConsumptionsChart(int forecastInfoId, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastLaboratoryConsumption> query = null;
            try
            {
                query = _forecastLaboratoryConsumptionRepository.GetAll(x => !x.IsDeleted && x.IsActive && x.ForecastInfoId == forecastInfoId);
                query = query.Select(i => new Data.DbModels.ForecastingSchema.ForecastLaboratoryConsumption() { Period = i.Period, AmountForecasted = i.AmountForecasted });

                query = query.OrderByDescending(x => x.Period);
                var total = query.Count();

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ForecastLaboratoryConsumptionDto>>(query.ToList());
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
