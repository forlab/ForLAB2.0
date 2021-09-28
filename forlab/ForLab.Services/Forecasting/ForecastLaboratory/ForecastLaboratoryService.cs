using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastLaboratory;
using ForLab.Repositories.Forecasting.ForecastLaboratory;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Forecasting.ForecastLaboratory
{
    public class ForecastLaboratoryService : GService<ForecastLaboratoryDto, Data.DbModels.ForecastingSchema.ForecastLaboratory, IForecastLaboratoryRepository>, IForecastLaboratoryService
    {
        private readonly IForecastLaboratoryRepository _forecastLaboratoryRepository;
        private readonly IResponseDTO _response;
        private readonly IFileService<ExportForecastLaboratoryDto> _fileService;

        public ForecastLaboratoryService(IMapper mapper,
            IResponseDTO response,
            IForecastLaboratoryRepository forecastLaboratoryRepository,
            IFileService<ExportForecastLaboratoryDto> fileService) : base(forecastLaboratoryRepository, mapper)
        {
            _forecastLaboratoryRepository = forecastLaboratoryRepository;
            _response = response;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastLaboratory> query = null;
            try
            {
                query = _forecastLaboratoryRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.ForecastCategory)
                                    .Include(x => x.Laboratory)
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
                    if (filterDto.ForecastCategoryId > 0)
                    {
                        query = query.Where(x => x.ForecastCategoryId == filterDto.ForecastCategoryId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Laboratory.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ForecastLaboratoryDto>>(query.ToList());

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
        public GeneratedFile ExportForecastLaboratory(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastLaboratory> query = null;
            try
            {
                query = _forecastLaboratoryRepository.GetAll()
                                     .Include(x => x.ForecastInfo)
                                     .Include(x => x.ForecastCategory)
                                     .Include(x => x.Laboratory)
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
                    if (filterDto.ForecastCategoryId > 0)
                    {
                        query = query.Where(x => x.ForecastCategoryId == filterDto.ForecastCategoryId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Laboratory.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ExportForecastLaboratoryDto>>(query.ToList());

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
