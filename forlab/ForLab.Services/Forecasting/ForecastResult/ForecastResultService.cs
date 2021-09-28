using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastResult;
using ForLab.Repositories.Forecasting.ForecastResult;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Forecasting.ForecastResult
{
    public class ForecastResultService : GService<ForecastResultDto, Data.DbModels.ForecastingSchema.ForecastResult, IForecastResultRepository>, IForecastResultService
    {
        private readonly IForecastResultRepository _forecastResultRepository;
        private readonly IResponseDTO _response;
        private readonly IFileService<ExportForecastResultDto> _fileService;

        public ForecastResultService(IMapper mapper,
            IResponseDTO response,
            IForecastResultRepository forecastResultRepository,
            IFileService<ExportForecastResultDto> fileService) : base(forecastResultRepository, mapper)
        {
            _forecastResultRepository = forecastResultRepository;
            _response = response;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastResultFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastResult> query = null;
            try
            {
                query = _forecastResultRepository.GetAll()
                                     .Include(x => x.ForecastInfo)
                                     .Include(x => x.Test)
                                     .Include(x => x.Product)
                                     .Include(x => x.Laboratory)
                                     .Include(x => x.ProductType)
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
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.ProductTypeId > 0)
                    {
                        query = query.Where(x => x.ProductTypeId == filterDto.ProductTypeId);
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

                var dataList = _mapper.Map<List<ForecastResultDto>>(query.ToList());

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
        public GeneratedFile ExportForecastResult(int? pageIndex = null, int? pageSize = null, ForecastResultFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastResult> query = null;
            try
            {
                query = _forecastResultRepository.GetAll()
                                     .Include(x => x.ForecastInfo)
                                     .Include(x => x.Test)
                                     .Include(x => x.Product)
                                     .Include(x => x.Laboratory)
                                     .Include(x => x.ProductType)
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
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.ProductTypeId > 0)
                    {
                        query = query.Where(x => x.ProductTypeId == filterDto.ProductTypeId);
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

                var dataList = _mapper.Map<List<ExportForecastResultDto>>(query.ToList());

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
        public IResponseDTO ForecastResultsChart(int forecastInfoId, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastResult> query = null;
            try
            {
                query = _forecastResultRepository.GetAll(x => !x.IsDeleted && x.IsActive && x.ForecastInfoId == forecastInfoId);
                query = query.Select(i => new Data.DbModels.ForecastingSchema.ForecastResult() 
                { 
                    Period = i.Period,
                    TotalPrice = i.TotalPrice,
                    TotalValue = i.TotalValue,
                    AmountForecasted = i.AmountForecasted
                });

                query = query.OrderByDescending(x => x.Period);

                var dataList = _mapper.Map<List<ForecastResultDto>>(query.ToList());
                var groupped = dataList.GroupBy(x => x.Period).ToList();
                var total = groupped.Count();

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    groupped = groupped.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
                }

                var lables = groupped.Select(x => x.Key);
                var totalPriceData = groupped.Select(x => x.Sum(y => y.TotalPrice));
                var totalValueData = groupped.Select(x => x.Sum(y => y.TotalValue));
                var amountForecastedData = groupped.Select(x => x.Sum(y => y.AmountForecasted));


                _response.Data = new
                {
                    List = new
                    {
                        Lables = lables,
                        TotalPriceData = totalPriceData,
                        TotalValueData = totalValueData,
                        AamountForecastedData = amountForecastedData,
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
