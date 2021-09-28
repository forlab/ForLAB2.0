using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastInstrument;
using ForLab.Repositories.Forecasting.ForecastInstrument;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Forecasting.ForecastInstrument
{
    public class ForecastInstrumentService : GService<ForecastInstrumentDto, Data.DbModels.ForecastingSchema.ForecastInstrument, IForecastInstrumentRepository>, IForecastInstrumentService
    {
        private readonly IForecastInstrumentRepository _forecastInstrumentRepository;
        private readonly IResponseDTO _response;
        private readonly IFileService<ExportForecastInstrumentDto> _fileService;
        public ForecastInstrumentService(IMapper mapper,
            IResponseDTO response,
            IForecastInstrumentRepository forecastInstrumentRepository,
            IFileService<ExportForecastInstrumentDto> fileService) : base(forecastInstrumentRepository, mapper)
        {
            _forecastInstrumentRepository = forecastInstrumentRepository;
            _response = response;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastInstrumentFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastInstrument> query = null;
            try
            {
                query = _forecastInstrumentRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Instrument)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.InstrumentId > 0)
                    {
                        query = query.Where(x => x.InstrumentId == filterDto.InstrumentId);
                    }
                    if (filterDto.ForecastInfoId > 0)
                    {
                        query = query.Where(x => x.ForecastInfoId == filterDto.ForecastInfoId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Instrument.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ForecastInstrumentDto>>(query.ToList());

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
        public GeneratedFile ExportForecastInstrument(int? pageIndex = null, int? pageSize = null, ForecastInstrumentFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastInstrument> query = null;
            try
            {

                query = _forecastInstrumentRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Instrument)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.InstrumentId > 0)
                    {
                        query = query.Where(x => x.InstrumentId == filterDto.InstrumentId);
                    }
                    if (filterDto.ForecastInfoId > 0)
                    {
                        query = query.Where(x => x.ForecastInfoId == filterDto.ForecastInfoId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Instrument.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ExportForecastInstrumentDto>>(query.ToList());

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
