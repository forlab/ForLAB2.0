using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastPatientGroup;
using ForLab.Repositories.Forecasting.ForecastPatientGroup;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Forecasting.ForecastPatientGroup
{
    public class ForecastPatientGroupService : GService<ForecastPatientGroupDto, Data.DbModels.ForecastingSchema.ForecastPatientGroup, IForecastPatientGroupRepository>, IForecastPatientGroupService
    {
        private readonly IForecastPatientGroupRepository _forecastPatientGroupRepository;
        private readonly IResponseDTO _response;
        private readonly IFileService<ExportForecastPatientGroupDto> _fileService;

        public ForecastPatientGroupService(IMapper mapper,
            IResponseDTO response,
            IForecastPatientGroupRepository forecastPatientGroupRepository,
            IFileService<ExportForecastPatientGroupDto> fileService) : base(forecastPatientGroupRepository, mapper)
        {
            _forecastPatientGroupRepository = forecastPatientGroupRepository;
            _response = response;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastPatientGroupFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastPatientGroup> query = null;
            try
            {
                query = _forecastPatientGroupRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.PatientGroup)
                                    .Include(x => x.Program)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.PatientGroupId > 0)
                    {
                        query = query.Where(x => x.PatientGroupId == filterDto.PatientGroupId);
                    }
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (filterDto.ForecastInfoId > 0)
                    {
                        query = query.Where(x => x.ForecastInfoId == filterDto.ForecastInfoId);
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

                var dataList = _mapper.Map<List<ForecastPatientGroupDto>>(query.ToList());

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
        public GeneratedFile ExportForecastPatientGroup(int? pageIndex = null, int? pageSize = null, ForecastPatientGroupFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastPatientGroup> query = null;
            try
            {
                query = _forecastPatientGroupRepository.GetAll()
                                    .Include(x => x.ForecastInfo)
                                    .Include(x => x.PatientGroup)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.PatientGroupId > 0)
                    {
                        query = query.Where(x => x.PatientGroupId == filterDto.PatientGroupId);
                    }
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (filterDto.ForecastInfoId > 0)
                    {
                        query = query.Where(x => x.ForecastInfoId == filterDto.ForecastInfoId);
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

                var dataList = _mapper.Map<List<ExportForecastPatientGroupDto>>(query.ToList());

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
