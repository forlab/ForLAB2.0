using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityWhoBase;
using ForLab.Repositories.Forecasting.ForecastMorbidityWhoBase;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Forecasting.ForecastMorbidityWhoBase
{
    public class ForecastMorbidityWhoBaseService : GService<ForecastMorbidityWhoBaseDto, Data.DbModels.ForecastingSchema.ForecastMorbidityWhoBase, IForecastMorbidityWhoBaseRepository>, IForecastMorbidityWhoBaseService
    {
        private readonly IForecastMorbidityWhoBaseRepository _forecastMorbidityWhoBaseRepository;
        private readonly IResponseDTO _response;
        private readonly IFileService<ExportForecastMorbidityWhoBaseDto> _fileService;

        public ForecastMorbidityWhoBaseService(IMapper mapper,
            IResponseDTO response,
            IForecastMorbidityWhoBaseRepository forecastMorbidityWhoBaseRepository,
            IFileService<ExportForecastMorbidityWhoBaseDto> fileService) : base(forecastMorbidityWhoBaseRepository, mapper)
        {
            _forecastMorbidityWhoBaseRepository = forecastMorbidityWhoBaseRepository;
            _response = response;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastMorbidityWhoBaseFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastMorbidityWhoBase> query = null;
            try
            {
                query = _forecastMorbidityWhoBaseRepository.GetAll()
                                     .Include(x => x.ForecastInfo)
                                     .Include(x => x.Disease)
                                     .Include(x => x.Country)
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
                    if (filterDto.DiseaseId > 0)
                    {
                        query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                    }
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Period))
                    {
                        query = query.Where(x => x.Period.Trim().ToLower().Contains(filterDto.Period.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Disease.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ForecastMorbidityWhoBaseDto>>(query.ToList());

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
        public GeneratedFile ExportForecastMorbidityWhoBase(int? pageIndex = null, int? pageSize = null, ForecastMorbidityWhoBaseFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastMorbidityWhoBase> query = null;
            try
            {
                query = _forecastMorbidityWhoBaseRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Disease)
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
                    if (filterDto.DiseaseId > 0)
                    {
                        query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Period))
                    {
                        query = query.Where(x => x.Period.Trim().ToLower().Contains(filterDto.Period.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Disease.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ExportForecastMorbidityWhoBaseDto>>(query.ToList());

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
        public IResponseDTO ForecastMorbidityWhoBasesChart(int forecastInfoId, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastMorbidityWhoBase> query = null;
            try
            {
                query = _forecastMorbidityWhoBaseRepository.GetAll(x => !x.IsDeleted && x.IsActive && x.ForecastInfoId == forecastInfoId);
                query = query.Select(i => new Data.DbModels.ForecastingSchema.ForecastMorbidityWhoBase() { Period = i.Period, Count = i.Count });

                query = query.OrderByDescending(x => x.Period);
                var total = query.Count();

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ForecastMorbidityWhoBaseDto>>(query.ToList());
                var lables = dataList.Select(x => x.Period);
                var data = dataList.Select(x => x.Count);

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
