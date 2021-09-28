using System;
using System.Threading.Tasks;
using ForLab.Repositories.Configuration.Configuration;
using ForLab.Core.Interfaces;
using ForLab.DTO.Configuration;
using ForLab.Services.Generics;
using AutoMapper;
using ForLab.Repositories.UOW;
using ForLab.Data.DataContext;
using System.Linq;
using ForLab.Repositories.Configuration.ConfigurationAudit;
using Microsoft.EntityFrameworkCore;
using ForLab.DTO.Configuration.ConfigurationAudit;
using System.Collections.Generic;
using ForLab.Core.Common;
using ForLab.Services.Global.FileService;
using ForLab.Data.Enums;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Configuration
{
   public  class ConfigurationService : GService<ConfigurationDto, Data.DbModels.ConfigurationSchema.Configuration, IConfigurationRepository>, IConfigurationService
    {

        private readonly IResponseDTO _response;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IConfigurationAuditRepository _configurationAuditRepository;
        private readonly IFileService<ExportConfigurationAuditDto> _fileService;
        public ConfigurationService(IMapper mapper, 
            IResponseDTO response,
            IUnitOfWork<AppDbContext> unitOfWork, 
            IConfigurationAuditRepository configurationAuditRepository,
            IConfigurationRepository configurationRepository,
            IFileService<ExportConfigurationAuditDto> fileService)
            : base(configurationRepository, mapper)
        {
            _response = response;
            _unitOfWork = unitOfWork;
            _configurationAuditRepository = configurationAuditRepository;
            _configurationRepository = configurationRepository;
            _fileService = fileService;
        }

        public async Task<IResponseDTO> GetConfigurationDetails()
        {
            try
            {
                var config = await _configurationRepository.GetFirstAsync();
                if (config == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "There is no data in the configuration table";
                    return _response;
                }

                _response.Data = _mapper.Map<ConfigurationDto>(config);
                _response.Message = "Ok";
                _response.IsPassed = true;
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public async Task<IResponseDTO> UpdateConfiguration(ConfigurationDto configurationDto, int loggedInUserId)
        {
            try
            {
                var ChangePassTime = await _configurationRepository.GetFirstAsync();
                if (ChangePassTime == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "There is no data in the configuration table";
                    return _response;
                }

                var configuration = _mapper.Map<Data.DbModels.ConfigurationSchema.Configuration>(configurationDto);

                // Add object audits
                var audit = _mapper.Map<Data.DbModels.ConfigurationSchema.ConfigurationAudit>(configuration);
                // set date of action and userId who do the action
                audit.DateOfAction = DateTime.Now;
                audit.CreatedBy = loggedInUserId;
                audit.Action = ActionOfAuditEnum.Update.ToString();
                configuration.ConfigurationAudits = new List<Data.DbModels.ConfigurationSchema.ConfigurationAudit>();
                configuration.ConfigurationAudits.Add(audit);

                _configurationRepository.Update(configuration);

                var finalResult = await _unitOfWork.CommitAsync();
                if (finalResult == 0)
                {
                    _response.IsPassed = false;
                    _response.Message = "can not update the configuration";
                    return _response;
                }
                    
                _response.IsPassed = true;
                _response.Message = "Updated successfully";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> GetTimeToChangePassword()
        {
            try
            {
                var ChangePassTime = await _configurationRepository.GetFirstAsync();
                if(ChangePassTime != null)
                {
                    _response.Data = ChangePassTime.NumOfDaysToChangePassword;
                    _response.IsPassed = true;
                    _response.Message = "Done";
                }
                else
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "There is no data in the configuration table";
                }
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> TimeToSessionTimeOut()
        {
            try
            {
                var TimeToSessionTimeOut = await _configurationRepository.GetFirstAsync();
                if (TimeToSessionTimeOut != null)
                {
                    _response.Data = TimeToSessionTimeOut.TimeToSessionTimeOut;
                    _response.IsPassed = true;
                    _response.Message = "Done";
                }

                else
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "There is no data in the configuration table";
                }
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public IResponseDTO GetAllConfigurationAudits(int? pageIndex = null, int? pageSize = null, ConfigurationAuditFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ConfigurationSchema.ConfigurationAudit> query = null;
            try
            {
                query = _configurationAuditRepository.GetAll()
                    .Include(x => x.Creator);

                if (filterDto != null)
                {
                    if (filterDto.CreatedBy > 0)
                    {
                        query = query.Where(x => x.CreatedBy == filterDto.CreatedBy);
                    }
                    if (filterDto.DateOfAction != null)
                    {
                        query = query.Where(x => x.DateOfAction.Date == filterDto.DateOfAction.Value.Date);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Action))
                    {
                        query = query.Where(x => x.Action.Trim().ToLower() == filterDto.Action.Trim().ToLower());
                    }
                }

                query = query.OrderByDescending(x => x.Id);

                var total = query.Count();

                // Check Sort Property
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
                var dataList = _mapper.Map<List<ConfigurationAuditDto>>(query.ToList());

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
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public GeneratedFile ExportConfigurationAudits(int? pageIndex = null, int? pageSize = null, ConfigurationAuditFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ConfigurationSchema.ConfigurationAudit> query = null;
            query = _configurationAuditRepository.GetAll()
                        .Include(x => x.Creator);

            // Apply filter
            if (filterDto != null)
            {
                if (filterDto.CreatedBy > 0)
                {
                    query = query.Where(x => x.CreatedBy == filterDto.CreatedBy);
                }
                if (filterDto.DateOfAction != null)
                {
                    query = query.Where(x => x.DateOfAction.Date == filterDto.DateOfAction.Value.Date);
                }
                if (!string.IsNullOrEmpty(filterDto.Action))
                {
                    query = query.Where(x => x.Action.Trim().ToLower() == filterDto.Action.Trim().ToLower());
                }
            }

            query = query.OrderByDescending(x => x.Id);

            var total = query.Count();

            // Check Sort Property
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

            var dataList = _mapper.Map<List<ExportConfigurationAuditDto>>(query.ToList());
            return _fileService.ExportToExcel(dataList);
        }
    }
}
