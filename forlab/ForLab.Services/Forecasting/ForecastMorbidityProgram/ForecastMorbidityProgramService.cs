using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityProgram;
using ForLab.Repositories.Forecasting.ForecastMorbidityProgram;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Forecasting.ForecastMorbidityProgram
{
    public class ForecastMorbidityProgramService : GService<ForecastMorbidityProgramDto, Data.DbModels.ForecastingSchema.ForecastMorbidityProgram, IForecastMorbidityProgramRepository>, IForecastMorbidityProgramService
    {
        private readonly IForecastMorbidityProgramRepository _forecastMorbidityProgramRepository;
        private readonly IResponseDTO _response;
        private readonly IFileService<ExportForecastMorbidityProgramDto> _fileService;

        public ForecastMorbidityProgramService(IMapper mapper,
            IResponseDTO response,
            IForecastMorbidityProgramRepository forecastMorbidityProgramRepository,
            IFileService<ExportForecastMorbidityProgramDto> fileService) : base(forecastMorbidityProgramRepository, mapper)
        {
            _forecastMorbidityProgramRepository = forecastMorbidityProgramRepository;
            _response = response;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastMorbidityProgramFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastMorbidityProgram> query = null;
            try
            {
                query = _forecastMorbidityProgramRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Program)
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
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Program.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ForecastMorbidityProgramDto>>(query.ToList());

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
        public GeneratedFile ExportForecastMorbidityProgram(int? pageIndex = null, int? pageSize = null, ForecastMorbidityProgramFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastMorbidityProgram> query = null;
            try
            {
                query = _forecastMorbidityProgramRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.Program)
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
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Program.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ExportForecastMorbidityProgramDto>>(query.ToList());

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
