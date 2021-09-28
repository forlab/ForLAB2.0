using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityTargetBase;
using ForLab.Repositories.Forecasting.ForecastMorbidityTargetBase;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Forecasting.ForecastMorbidityTargetBase
{
    public class ForecastMorbidityTargetBaseService : GService<ForecastMorbidityTargetBaseDto, Data.DbModels.ForecastingSchema.ForecastMorbidityTargetBase, IForecastMorbidityTargetBaseRepository>, IForecastMorbidityTargetBaseService
    {
        private readonly IForecastMorbidityTargetBaseRepository _forecastMorbidityTargetBaseRepository;
        private readonly IResponseDTO _response;
        private readonly IFileService<ExportForecastMorbidityTargetBaseDto> _fileService;

        public ForecastMorbidityTargetBaseService(IMapper mapper,
            IResponseDTO response,
            IForecastMorbidityTargetBaseRepository forecastMorbidityTargetBaseRepository,
            IFileService<ExportForecastMorbidityTargetBaseDto> fileService) : base(forecastMorbidityTargetBaseRepository, mapper)
        {
            _forecastMorbidityTargetBaseRepository = forecastMorbidityTargetBaseRepository;
            _response = response;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastMorbidityTargetBaseFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastMorbidityTargetBase> query = null;
            try
            {
                query = _forecastMorbidityTargetBaseRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.ForecastLaboratory).ThenInclude(x => x.Laboratory)
                                    .Include(x => x.ForecastMorbidityProgram).ThenInclude(x => x.Program)
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
                    if (filterDto.ForecastLaboratoryId > 0)
                    {
                        query = query.Where(x => x.ForecastLaboratoryId == filterDto.ForecastLaboratoryId);
                    }
                    if (filterDto.ForecastMorbidityProgramId > 0)
                    {
                        query = query.Where(x => x.ForecastMorbidityProgramId == filterDto.ForecastMorbidityProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.ForecastMorbidityProgram.Program.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ForecastMorbidityTargetBaseDto>>(query.ToList());

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
        public GeneratedFile ExportForecastMorbidityTargetBase(int? pageIndex = null, int? pageSize = null, ForecastMorbidityTargetBaseFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastMorbidityTargetBase> query = null;
            try
            {
                query = _forecastMorbidityTargetBaseRepository.GetAll()
                                    .Include(x => x.ForecastLaboratory)
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.ForecastMorbidityProgram)
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
                    if (filterDto.ForecastLaboratoryId > 0)
                    {
                        query = query.Where(x => x.ForecastLaboratoryId == filterDto.ForecastLaboratoryId);
                    }
                    if (filterDto.ForecastMorbidityProgramId > 0)
                    {
                        query = query.Where(x => x.ForecastMorbidityProgramId == filterDto.ForecastMorbidityProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.ForecastMorbidityProgram.Program.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<ExportForecastMorbidityTargetBaseDto>>(query.ToList());

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
        public IResponseDTO ForecastMorbidityTargetBasesChart(int forecastInfoId, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastMorbidityTargetBase> query = null;
            try
            {
                query = _forecastMorbidityTargetBaseRepository.GetAll()
                                    .Include(x => x.ForecastLaboratory).ThenInclude(x => x.Laboratory)
                                    .Include(x => x.ForecastMorbidityProgram).ThenInclude(x => x.Program)
                                    .Where(x => !x.IsDeleted && x.ForecastInfoId == forecastInfoId);

                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ForecastMorbidityTargetBaseDto>>(query.ToList());
                var lables = dataList.Select(x => $"{x.ForecastLaboratoryLaboratoryName}-{x.ForecastMorbidityProgramProgramName}");
                var currentPatientData = dataList.Select(x => x.CurrentPatient);
                var targetPatientData = dataList.Select(x => x.TargetPatient);

                _response.Data = new
                {
                    List = new
                    {
                        Lables = lables,
                        CurrentPatientData = currentPatientData,
                        TargetPatientData = targetPatientData
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
