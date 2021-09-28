using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.Data.Enums;
using ForLab.DTO.Forecasting.ForecastCategory;
using ForLab.DTO.Forecasting.ForecastInfo;
using ForLab.DTO.Forecasting.ForecastLaboratory;
using ForLab.DTO.Forecasting.ForecastLaboratoryConsumption;
using ForLab.DTO.Forecasting.ForecastLaboratoryTestService;
using ForLab.DTO.Forecasting.ForecastMorbidityTargetBase;
using ForLab.DTO.Forecasting.ForecastMorbidityWhoBase;
using ForLab.Repositories.Disease.CountryDiseaseIncident;
using ForLab.Repositories.DiseaseProgram.PatientAssumptionParameter;
using ForLab.Repositories.DiseaseProgram.TestingAssumptionParameter;
using ForLab.Repositories.Forecasting.ForecastCategory;
using ForLab.Repositories.Forecasting.ForecastInfo;
using ForLab.Repositories.Forecasting.ForecastMorbidityTargetBase;
using ForLab.Repositories.Forecasting.ForecastResult;
using ForLab.Repositories.Laboratory.LaboratoryConsumption;
using ForLab.Repositories.Laboratory.LaboratoryInstrument;
using ForLab.Repositories.Laboratory.LaboratoryTestService;
using ForLab.Repositories.Product.Instrument;
using ForLab.Repositories.Product.Product;
using ForLab.Repositories.Product.ProductUsage;
using ForLab.Repositories.Testing.TestingProtocol;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ForLab.Services.Forecasting.ForecastInfo
{
    public class ForecastInfoService : GService<ForecastInfoDto, Data.DbModels.ForecastingSchema.ForecastInfo, IForecastInfoRepository>, IForecastInfoService
    {
        private readonly IForecastInfoRepository _forecastInfoRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportForecastInfoDto> _fileService;
        private readonly IConfiguration _configuration;
        private readonly ILaboratoryTestServiceRepository _laboratoryTestServiceRepository;
        private readonly ILaboratoryConsumptionRepository _laboratoryConsumptionRepository;
        private readonly IForecastCategoryRepository _forecastCategoryRepository;
        private readonly IForecastMorbidityTargetBaseRepository _forecastMorbidityTargetBaseRepository;
        private readonly ICountryDiseaseIncidentRepository _countryDiseaseIncidentRepository;
        private readonly IProductUsageRepository _productUsageRepository;
        private readonly ILaboratoryInstrumentRepository _laboratoryInstrumentRepository;
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPatientAssumptionParameterRepository _patientAssumptionParameterRepository;
        private readonly ITestingAssumptionParameterRepository _testingAssumptionParameterRepository;
        private readonly ITestingProtocolRepository _testingProtocolRepository;
        private readonly IGeneralService _generalService;
        private readonly IForecastResultRepository _forecastResultRepository;
        public ForecastInfoService(IMapper mapper,
            IGeneralService generalService,
            IResponseDTO response,
            IForecastInfoRepository forecastInfoRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportForecastInfoDto> fileService,
            IConfiguration configuration,
            ILaboratoryTestServiceRepository laboratoryTestServiceRepository,
            ILaboratoryConsumptionRepository laboratoryConsumptionRepository,
            IForecastCategoryRepository forecastCategoryRepository,
            IForecastMorbidityTargetBaseRepository forecastMorbidityTargetBaseRepository,
            ICountryDiseaseIncidentRepository countryDiseaseIncidentRepository,
            IProductUsageRepository productUsageRepository,
            ILaboratoryInstrumentRepository laboratoryInstrumentRepository,
            IInstrumentRepository instrumentRepository,
            IProductRepository productRepository,
            IPatientAssumptionParameterRepository patientAssumptionParameterRepository,
            ITestingAssumptionParameterRepository testingAssumptionParameterRepository,
            ITestingProtocolRepository testingProtocolRepository,
            IForecastResultRepository forecastResultRepository) : base(forecastInfoRepository, mapper)
        {
            _generalService = generalService;
            _forecastInfoRepository = forecastInfoRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _configuration = configuration;
            _laboratoryTestServiceRepository = laboratoryTestServiceRepository;
            _laboratoryConsumptionRepository = laboratoryConsumptionRepository;
            _forecastCategoryRepository = forecastCategoryRepository;
            _forecastMorbidityTargetBaseRepository = forecastMorbidityTargetBaseRepository;
            _countryDiseaseIncidentRepository = countryDiseaseIncidentRepository;
            _productUsageRepository = productUsageRepository;
            _laboratoryInstrumentRepository = laboratoryInstrumentRepository;
            _instrumentRepository = instrumentRepository;
            _productRepository = productRepository;
            _patientAssumptionParameterRepository = patientAssumptionParameterRepository;
            _testingAssumptionParameterRepository = testingAssumptionParameterRepository;
            _testingProtocolRepository = testingProtocolRepository;
            _forecastResultRepository = forecastResultRepository;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastInfoFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastInfo> query = null;
            try
            {
                query = _forecastInfoRepository.GetAll()
                                    .Include(x => x.ForecastInfoLevel)
                                    .Include(x => x.ScopeOfTheForecast)
                                    .Include(x => x.Country)
                                    .Include(x => x.ForecastMethodology)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId);
                    }


                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.IsAggregate != null)
                    {
                        query = query.Where(x => x.IsAggregate == filterDto.IsAggregate);
                    }
                    if (filterDto.IsTargetBased != null && filterDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity)
                    {
                        query = query.Where(x => x.IsTargetBased == filterDto.IsTargetBased);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.ForecastInfoLevelId > 0)
                    {
                        query = query.Where(x => x.ForecastInfoLevelId == filterDto.ForecastInfoLevelId);
                    }
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }
                    if (filterDto.ForecastMethodologyId > 0)
                    {
                        query = query.Where(x => x.ForecastMethodologyId == filterDto.ForecastMethodologyId);
                    }
                    if (filterDto.ScopeOfTheForecastId > 0)
                    {
                        query = query.Where(x => x.ScopeOfTheForecastId == filterDto.ScopeOfTheForecastId);
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

                var dataList = _mapper.Map<List<ForecastInfoDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(ForecastInfoFilterDto filterDto = null)
        {
            var query = _forecastInfoRepository.GetAll(x => !x.IsDeleted && x.IsActive);


            if (filterDto != null)
            {
                // Security Filter
                if (!filterDto.IsSuperAdmin)
                {
                    query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId);
                }


                if (filterDto.IsActive != null)
                {
                    query = query.Where(x => x.IsActive == filterDto.IsActive);
                }
                if (filterDto.IsAggregate != null)
                {
                    query = query.Where(x => x.IsAggregate == filterDto.IsAggregate);
                }
                if (filterDto.IsTargetBased != null && filterDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity)
                {
                    query = query.Where(x => x.IsTargetBased == filterDto.IsTargetBased);
                }
                if (!string.IsNullOrEmpty(filterDto.Name))
                {
                    query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                }
                if (filterDto.ForecastInfoLevelId > 0)
                {
                    query = query.Where(x => x.ForecastInfoLevelId == filterDto.ForecastInfoLevelId);
                }
                if (filterDto.CountryId > 0)
                {
                    query = query.Where(x => x.CountryId == filterDto.CountryId);
                }
                if (filterDto.ForecastMethodologyId > 0)
                {
                    query = query.Where(x => x.ForecastMethodologyId == filterDto.ForecastMethodologyId);
                }
                if (filterDto.ScopeOfTheForecastId > 0)
                {
                    query = query.Where(x => x.ScopeOfTheForecastId == filterDto.ScopeOfTheForecastId);
                }
            }

            query = query.OrderBy(x => x.Id);
            query = query.Select(i => new Data.DbModels.ForecastingSchema.ForecastInfo() { Id = i.Id, Name = i.Name });
            var dataList = _mapper.Map<List<ForecastInfoDrp>>(query.ToList());

            _response.Data = dataList;
            _response.IsPassed = true;
            _response.Message = "Done";
            return _response;
        }
        public async Task<IResponseDTO> GetForecastInfoDetails(int forecastInfoId)
        {
            try
            {
                var forecastInfo = await _forecastInfoRepository.GetAll()
                                        .Include(x => x.ForecastInfoLevel)
                                        .Include(x => x.ScopeOfTheForecast)
                                        .Include(x => x.Country)
                                        .Include(x => x.ForecastMethodology)
                                        .Include(x => x.Creator)
                                        .Include(x => x.Updator)
                                        .FirstOrDefaultAsync(x => x.Id == forecastInfoId);
                if (forecastInfo == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var forecastInfoDto = _mapper.Map<ForecastInfoDto>(forecastInfo);

                _response.Data = forecastInfoDto;
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
        public async Task<IResponseDTO> GetForecastInfoDetailsForUpdate(int forecastInfoId, int loggedInUserId)
        {
            try
            {
                var forecastInfo = await _forecastInfoRepository.GetAll()
                                    .Include(x => x.ForecastInstruments)
                                        .ThenInclude(x => x.Instrument)
                                        .ThenInclude(x => x.TestingArea)
                                    .Include(x => x.ForecastPatientGroups)
                                        .ThenInclude(x => x.PatientGroup)
                                    .Include(x => x.ForecastPatientGroups)
                                        .ThenInclude(x => x.Program)
                                    .Include(x => x.ForecastTests)
                                        .ThenInclude(x => x.Test)
                                        .ThenInclude(x => x.TestingArea)
                                    .Include(x => x.ForecastPatientAssumptionValues)
                                    .Include(x => x.ForecastProductAssumptionValues)
                                    .Include(x => x.ForecastTestingAssumptionValues)
                                    .Include(x => x.ForecastMorbidityPrograms)
                                    .Include(x => x.ForecastMorbidityTestingProtocolMonths)
                                    .Include(x => x.ForecastMorbidityTargetBases)
                                        .ThenInclude(x => x.ForecastLaboratory)
                                        .ThenInclude(x => x.Laboratory)
                                    .Include(x => x.ForecastMorbidityTargetBases)
                                        .ThenInclude(x => x.ForecastMorbidityProgram)
                                     .Include(x => x.ForecastMorbidityWhoBases)
                                        .ThenInclude(x => x.Country)
                                     .Include(x => x.ForecastMorbidityWhoBases)
                                        .ThenInclude(x => x.Disease)
                                    .Include(x => x.ForecastLaboratoryConsumptions)
                                        .ThenInclude(x => x.Laboratory)
                                    .Include(x => x.ForecastLaboratoryTestServices)
                                        .ThenInclude(x => x.Laboratory)
                                    .FirstOrDefaultAsync(x => x.Id == forecastInfoId);
                if (forecastInfo == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }
                if (forecastInfo.CreatedBy != loggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You are not allowed to update the forecast";
                    return _response;
                }

                // Filter Children
                forecastInfo.ForecastInstruments = forecastInfo.ForecastInstruments.Where(x => !x.IsDeleted).ToList();
                forecastInfo.ForecastPatientGroups = forecastInfo.ForecastPatientGroups.Where(x => !x.IsDeleted).ToList();
                forecastInfo.ForecastTests = forecastInfo.ForecastTests.Where(x => !x.IsDeleted).ToList();
                forecastInfo.ForecastPatientAssumptionValues = forecastInfo.ForecastPatientAssumptionValues.Where(x => !x.IsDeleted).ToList();
                forecastInfo.ForecastProductAssumptionValues = forecastInfo.ForecastProductAssumptionValues.Where(x => !x.IsDeleted).ToList();
                forecastInfo.ForecastMorbidityPrograms = forecastInfo.ForecastMorbidityPrograms.Where(x => !x.IsDeleted).ToList();
                forecastInfo.ForecastMorbidityTestingProtocolMonths = forecastInfo.ForecastMorbidityTestingProtocolMonths.Where(x => !x.IsDeleted).ToList();
                forecastInfo.ForecastMorbidityWhoBases = forecastInfo.ForecastMorbidityWhoBases.Where(x => !x.IsDeleted).ToList();
                forecastInfo.ForecastMorbidityTargetBases = forecastInfo.ForecastMorbidityTargetBases.Where(x => !x.IsDeleted).ToList();
                forecastInfo.ForecastLaboratoryConsumptions = forecastInfo.ForecastLaboratoryConsumptions.Where(x => !x.IsDeleted).ToList();
                forecastInfo.ForecastLaboratoryTestServices = forecastInfo.ForecastLaboratoryTestServices.Where(x => !x.IsDeleted).ToList();


                // Map the result
                var forecastInfoDto = _mapper.Map<ForecastInfoDto>(forecastInfo);
                // Histoical Data
                forecastInfoDto.ForecastLaboratoryConsumptionDtos = null;
                forecastInfoDto.HistoicalConsumptionDtos = _mapper.Map<List<HistoicalConsumptionDto>>(forecastInfo.ForecastLaboratoryConsumptions);
                forecastInfoDto.HistoicalConsumptionDtos = forecastInfoDto.HistoicalConsumptionDtos.GroupBy(x => new
                {
                    LaboratoryId = x.LaboratoryId,
                    RegionId = x.RegionId,
                    ProductId = x.ProductId
                }, y => y).Select(x => x.First()).ToList();
                // Histoical Service
                forecastInfoDto.ForecastLaboratoryTestServiceDtos = null;
                forecastInfoDto.HistoicalServiceDataDtos = _mapper.Map<List<HistoicalServiceDataDto>>(forecastInfo.ForecastLaboratoryTestServices);
                forecastInfoDto.HistoicalServiceDataDtos = forecastInfoDto.HistoicalServiceDataDtos.GroupBy(x => new
                {
                    LaboratoryId = x.LaboratoryId,
                    RegionId = x.RegionId,
                    TestId = x.TestId
                }, y => y).Select(x => x.First()).ToList();
                // Target Base
                forecastInfoDto.ForecastMorbidityTargetBaseDtos = null;
                forecastInfoDto.HistoicalTargetBaseDtos = _mapper.Map<List<HistoicalTargetBaseDto>>(forecastInfo.ForecastMorbidityTargetBases);
                forecastInfoDto.HistoicalTargetBaseDtos = forecastInfoDto.HistoicalTargetBaseDtos.GroupBy(x => new
                {
                    CurrentPatient = x.CurrentPatient,
                    TargetPatient = x.TargetPatient,
                    LaboratoryId = x.LaboratoryId,
                    RegionId = x.RegionId,
                    ProgramId = x.ProgramId,
                }, y => y).Select(x => x.First()).ToList();
                // WHO
                forecastInfoDto.ForecastMorbidityWhoBaseDtos = null;
                forecastInfoDto.HistoicalWhoBaseDtos = _mapper.Map<List<HistoicalWhoBaseDto>>(forecastInfo.ForecastMorbidityWhoBases);
                forecastInfoDto.HistoicalWhoBaseDtos = forecastInfoDto.HistoicalWhoBaseDtos.GroupBy(x => new
                {
                    CountryId = x.CountryId,
                    CountryName = x.CountryName,
                    DiseaseId = x.DiseaseId,
                    DiseaseName = x.DiseaseName
                }, y => y).Select(x => x.First()).ToList();

                _response.Data = forecastInfoDto;
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
        public async Task<IResponseDTO> CreateForecastInfo(ForecastInfoDto forecastInfoDto)
        {
            try
            {
                // Communicate with the ML
                if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.Service
                    || forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.Consumption
                    || (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfoDto.IsWorldHealthOrganization)
                 )
                {
                    forecastInfoDto = await CommunicateWithML(forecastInfoDto);
                    if (forecastInfoDto == null)
                    {
                        _response.IsPassed = false;
                        _response.Message = "Something went wrong with the machine learning";
                        return _response;
                    }
                }
                else if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfoDto.IsTargetBased)
                {
                    forecastInfoDto.ForecastMorbidityTargetBaseDtos = _mapper.Map<List<ForecastMorbidityTargetBaseDto>>(forecastInfoDto.HistoicalTargetBaseDtos);
                }

                // Get Forecast Laboratories from Excel File
                forecastInfoDto = GetForecastLaboratories(forecastInfoDto);

                // Map DTO to Database
                var forecastInfo = _mapper.Map<Data.DbModels.ForecastingSchema.ForecastInfo>(forecastInfoDto);
                // Move forecast categories to a variable to add it to the DB after getting the forecastId
                var forecastCategories = forecastInfo.ForecastCategories;
                forecastInfo.ForecastCategories = null;
                // Move forecast target bases to a variable to add it to the DB after getting the forecastId
                var targetBases = forecastInfo.ForecastMorbidityTargetBases;
                forecastInfo.ForecastMorbidityTargetBases = null;
               

                #region Calculation Module  
                // Init the result value
                var ForecastResults = new List<Data.DbModels.ForecastingSchema.ForecastResult>();

                if (forecastInfo.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfo.IsTargetBased)
                {
                    // Assumtions Ids
                    var patientAssumptionIds = forecastInfo.ForecastPatientAssumptionValues.Select(x => x.PatientAssumptionParameterId).ToList();
                    var testingAssumptionIds = forecastInfo.ForecastTestingAssumptionValues.Select(x => x.TestingAssumptionParameterId).ToList();
                    // Assumtions Values
                    var patientAssumptions = _patientAssumptionParameterRepository.GetAll(x => !x.IsDeleted && x.IsActive && patientAssumptionIds.Contains(x.Id)).ToList();
                    var testingAssumptions = _testingAssumptionParameterRepository.GetAll(x => !x.IsDeleted && x.IsActive && testingAssumptionIds.Contains(x.Id)).ToList();
                    // Testing Protocols
                    var testingProtocolIds = forecastInfo.ForecastMorbidityTestingProtocolMonths.Select(x => x.TestingProtocolId).ToList();
                    var testingProtocolsData = _testingProtocolRepository.GetAll(x => !x.IsDeleted && x.IsActive && testingProtocolIds.Contains(x.Id)).ToList();

                    foreach (var targetBase in forecastInfoDto.HistoicalTargetBaseDtos)
                    {
                        decimal CurrentPatient = targetBase.CurrentPatient;
                        decimal NewPatient = targetBase.TargetPatient - CurrentPatient;
                        decimal AttritionRate = 0;      //AttritionRate > 0 : Positive, < 0 : Negative
                        foreach (var FPAV in forecastInfo.ForecastPatientAssumptionValues)
                        {
                            var patientAssumptionParameter = patientAssumptions.FirstOrDefault(x => x.Id == FPAV.PatientAssumptionParameterId);
                            if (patientAssumptionParameter != null)
                            {
                                if (patientAssumptionParameter.IsPositive)
                                {
                                    AttritionRate += Convert.ToDecimal(FPAV.Value);
                                }
                                if (patientAssumptionParameter.IsNegative)
                                {
                                    AttritionRate -= Convert.ToDecimal(FPAV.Value);
                                }
                            }
                        }
                        decimal RepeatRate = 0;             //RepeatRate > 0 : Positive, < 0 : Negative
                        foreach (var FTAV in forecastInfo.ForecastTestingAssumptionValues)
                        {
                            var testingAssumptionParameter = testingAssumptions.FirstOrDefault(x => x.Id == FTAV.TestingAssumptionParameterId);
                            if (testingAssumptionParameter != null)
                            {
                                if (testingAssumptionParameter.IsPositive)
                                {
                                    RepeatRate += Convert.ToDecimal(FTAV.Value);
                                }
                                if (testingAssumptionParameter.IsNegative)
                                {
                                    RepeatRate -= Convert.ToDecimal(FTAV.Value);
                                }
                            }
                        }

                        var tests = forecastInfo.ForecastMorbidityTestingProtocolMonths.GroupBy(g => new { g.TestId }).Select(s => new { s.Key.TestId }).ToList();
                        var groups = forecastInfo.ForecastMorbidityTestingProtocolMonths.GroupBy(g => new { g.PatientGroupId }).Select(s => new { s.Key.PatientGroupId }).ToList();
                        for (int i = 0; i < tests.Count; i++)
                        {
                            float TotalTestNum = 0;   //TotalTest# Per Site, Test, Cycle
                            List<TestNumCycleForecastByGroup> TNCFByGroupList = new List<TestNumCycleForecastByGroup>();

                            for (int j = 0; j < groups.Count; j++)
                            {
                                var testingProtocols = forecastInfo.ForecastMorbidityTestingProtocolMonths
                                    .Where(x => !x.IsDeleted && tests[i].TestId == x.TestId && groups[j].PatientGroupId == x.PatientGroupId)
                                    .OrderBy(x => x.CalculationPeriodMonthId)
                                    .ToList();

                                decimal GroupPercentage = forecastInfo.ForecastPatientGroups.Where(x => x.PatientGroupId == groups[j].PatientGroupId).Select(s => s.Percentage).FirstOrDefault();
                                int testingProtocolId = testingProtocols.Select(x => x.TestingProtocolId).FirstOrDefault();
                                int totalTestPerYear = testingProtocolsData.Where(x => x.Id == testingProtocolId).Select(x => x.TestAfterFirstYear).FirstOrDefault();
                                float CurrentTestNumAfterCycle = (float)totalTestPerYear / testingProtocols.Count;

                                for (int k = 0; k < forecastInfo.Duration; k++)
                                {
                                    float GroupCurrentPatient = 0.0f;
                                    float GroupNewPatient = 0.0f;
                                    float stepPatients = (float)NewPatient / forecastInfo.Duration;
                                    decimal NewTestNumPerMonth = testingProtocols[k % testingProtocols.Count].Value ?? 0;
                                    if (stepPatients > 0)
                                    {
                                        GroupCurrentPatient = (float)targetBase.CurrentPatient + stepPatients * k;
                                        GroupNewPatient = stepPatients;
                                    }
                                    else
                                    {
                                        GroupCurrentPatient = (float)targetBase.TargetPatient + stepPatients * (k + 1);
                                        GroupNewPatient = 0;
                                    }
                                    GroupCurrentPatient *= (float)(GroupPercentage / 100) * (float)(1 + AttritionRate / 100);
                                    GroupNewPatient *= (float)(GroupPercentage / 100) * (float)(1 + AttritionRate / 100);
                                    TotalTestNum = GroupCurrentPatient * CurrentTestNumAfterCycle + GroupNewPatient * (float)NewTestNumPerMonth;
                                    TotalTestNum *= (float)(1 + RepeatRate / 100);

                                    TestNumCycleForecastByGroup TNCFByGroup = new TestNumCycleForecastByGroup();
                                    TNCFByGroup.GroupId = testingProtocolId;
                                    TNCFByGroup.TimeStamp = forecastInfo.StartDate.AddMonths(k + 1);
                                    TNCFByGroup.Forecast = Convert.ToDecimal(TotalTestNum);
                                    TNCFByGroupList.Add(TNCFByGroup);
                                }
                            }
                            if (TNCFByGroupList.Count > 0)
                            {
                                var TNCFList = TNCFByGroupList.GroupBy(g => g.TimeStamp).Select(s => new TestNumCycleForecast
                                {
                                    TimeStamp = s.Key,
                                    Forecast = s.Sum(x => x.Forecast)
                                }).ToList();

                                for (int idx = 0; idx < TNCFList.Count; idx++)
                                {
                                    var TS = new Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService();
                                    TS.LaboratoryId = targetBase.LaboratoryId;
                                    TS.TestId = tests[i].TestId;
                                    TS.AmountForecasted = TNCFList[idx].Forecast;
                                    forecastInfo.ForecastLaboratoryTestServices.Add(TS);
                                }
                            }
                        }
                    }

                    // Call ML after creating test services
                    forecastInfo.ForecastLaboratoryTestServices = await ML_TragetBase(forecastInfo);
                }

                if (forecastInfo.ForecastMethodologyId == (int)ForecastMethodologyEnum.Service ||
                    (forecastInfo.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfo.IsTargetBased)
                    )
                {
                    // Vars
                    var testIds = forecastInfo.ForecastLaboratoryTestServices.Select(x => x.TestId).ToList();
                    var labIds = forecastInfo.ForecastLaboratoryTestServices.Select(x => x.LaboratoryId).ToList();
                    // Get Data
                    var ProductUsagesData = _productUsageRepository.GetAll(x => !x.IsDeleted && x.IsActive && testIds.Contains(x.TestId.Value))
                                                                .Include(x => x.Product)
                                                                .Include(x => x.Instrument)
                                                                .ToList();
                    var instrumentIds = ProductUsagesData.Select(x => x.InstrumentId).ToList();
                    var LaboratoryInstrumentData = _laboratoryInstrumentRepository.GetAll(x => !x.IsDeleted && x.IsActive && labIds.Contains(x.LaboratoryId) && instrumentIds.Contains(x.InstrumentId)).ToList();

                    foreach (var testService in forecastInfo.ForecastLaboratoryTestServices)
                    {
                        /*** Table Operation Comment Format : Get Rows from XXX.XXX(table name) table by XXX(Where fields) ***/
                        //Get Rows from Product.ProductUsage table by Test 
                        var ProductUsages = ProductUsagesData.Where(x => x.TestId == testService.TestId).ToList();

                        //For loop Product Usages to get Product# from Test#
                        foreach (var productUsage in ProductUsages)
                        {
                            decimal Qty = 0;
                            if (testService.LaboratoryId > 0)
                            {
                                decimal workingDays = 22;
                                //Get Rows from Laboratory.LaboratoryInstruments table by Laboratory & Instrument
                                var LaboratoryInstrument = LaboratoryInstrumentData.FirstOrDefault(x => x.LaboratoryId == testService.LaboratoryId && x.InstrumentId == productUsage.InstrumentId);

                                if (productUsage.PerPeriodPerInstrument || productUsage.PerPeriod)
                                {
                                    #region Consumable Usage Rate
                                    if (productUsage.PerPeriodPerInstrument)
                                    {
                                        if (LaboratoryInstrument != null)
                                        {
                                            switch (productUsage.CountryPeriodId)
                                            {
                                                case (int)CountryPeriodEnum.Weekly:
                                                    Qty = productUsage.Amount * (workingDays / 4) * LaboratoryInstrument.Quantity;
                                                    break;
                                                case (int)CountryPeriodEnum.Monthly:
                                                    Qty = productUsage.Amount * 1 * LaboratoryInstrument.Quantity;
                                                    break;
                                                case (int)CountryPeriodEnum.Quarterly:
                                                    Qty = productUsage.Amount * (1 / 3) * LaboratoryInstrument.Quantity;
                                                    break;
                                                case (int)CountryPeriodEnum.Annualy:
                                                    Qty = productUsage.Amount * (1 / 12) * LaboratoryInstrument.Quantity;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    if (productUsage.PerPeriod)
                                    {
                                        switch (productUsage.CountryPeriodId)
                                        {
                                            case (int)CountryPeriodEnum.Weekly:
                                                Qty = productUsage.Amount * workingDays;
                                                break;
                                            case (int)CountryPeriodEnum.Monthly:
                                                Qty = productUsage.Amount * (workingDays / 4);
                                                break;
                                            case (int)CountryPeriodEnum.Quarterly:
                                                Qty = productUsage.Amount * (1 / 3);
                                                break;
                                            case (int)CountryPeriodEnum.Annualy:
                                                Qty = productUsage.Amount * (1 / 12);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    //if (productUsage.PerTest)
                                    //{
                                    //    Qty = testService.AmountForecasted * productUsage.QuantityNeededPerTest;
                                    //}
                                    #endregion
                                }
                                else
                                {
                                    if (!productUsage.IsForControl)
                                    {
                                        #region Test Product Usage Rate
                                        decimal percentage = 1;
                                        if (LaboratoryInstrument != null)
                                        {
                                            if (LaboratoryInstrument.TestRunPercentage > 1 && LaboratoryInstrument.TestRunPercentage < 100)
                                            {
                                                percentage = LaboratoryInstrument.TestRunPercentage / 100;

                                            }
                                        }
                                        Qty = productUsage.Amount * testService.AmountForecasted * percentage;
                                        #endregion
                                    }
                                    else
                                    {
                                        #region QC Usage Rate
                                        //Get Rows from Product.Instrument table by Id
                                        var Instrument = productUsage.Instrument;
                                        switch (Instrument.ControlRequirementUnitId)
                                        {
                                            case (int)ControlRequirementUnitEnum.Daily:
                                                Qty = productUsage.Amount * workingDays * Instrument.ControlRequirement;
                                                break;
                                            case (int)ControlRequirementUnitEnum.Weekly:
                                                Qty = productUsage.Amount * (workingDays / 4) * Instrument.ControlRequirement;
                                                break;
                                            case (int)ControlRequirementUnitEnum.Monthly:
                                                Qty = productUsage.Amount * 1 * Instrument.ControlRequirement;
                                                break;
                                            default:
                                                break;
                                        }
                                        #endregion
                                    }
                                }
                            }
                            var packSize = productUsage.Product.PackSize;
                            var ForecastResult = new Data.DbModels.ForecastingSchema.ForecastResult
                            {
                                LaboratoryId = testService.LaboratoryId,
                                TestId = testService.TestId,
                                ProductId = productUsage.ProductId,
                                ProductTypeId = productUsage.Product.ProductTypeId,
                                AmountForecasted = testService.AmountForecasted,
                                TotalValue = Qty,
                                Period = testService.Period,
                                DurationDateTime = Convert.ToDateTime(testService.Period),
                                PackSize = packSize,
                                PackQty = int.Parse(decimal.Round(packSize == 0 ? 0 : (Qty / packSize), 0).ToString()),
                                PackPrice = productUsage.Product.ManufacturerPrice  //Need to update with Real Price for Level
                            };

                            ForecastResults.Add(ForecastResult);
                        }
                    }
                }
                else if (forecastInfo.ForecastMethodologyId == (int)ForecastMethodologyEnum.Consumption)
                {
                    // Consumptions
                    var ProductIds = forecastInfo.ForecastLaboratoryConsumptions.Select(x => x.ProductId).ToList();
                    var Products = _productRepository.GetAll(x => !x.IsDeleted && x.IsActive && ProductIds.Contains(x.Id)).ToList();

                    foreach (var consumption in forecastInfo.ForecastLaboratoryConsumptions)
                    {
                        if (consumption.ProductId > 0)
                        {
                            var product = Products.First(x => x.Id == consumption.ProductId);
                            var ForecastResult = new Data.DbModels.ForecastingSchema.ForecastResult
                            {
                                LaboratoryId = consumption.LaboratoryId,
                                ProductId = consumption.ProductId,
                                ProductTypeId = product.ProductTypeId,
                                AmountForecasted = consumption.AmountForecasted,
                                TotalValue = consumption.AmountForecasted,
                                Period = consumption.Period,
                                DurationDateTime = Convert.ToDateTime(consumption.Period),
                                PackSize = product.PackSize,
                                PackQty = int.Parse(decimal.Round(product.PackSize == 0 ? 0 : (consumption.AmountForecasted / product.PackSize), 0).ToString()),
                                PackPrice = product.ManufacturerPrice  //Need to update with Real Price with Price Level
                            };
                            ForecastResults.Add(ForecastResult);
                        }
                    }
                }

                // Set results
                forecastInfo.ForecastResults = ForecastResults.GroupBy(g => new { g.LaboratoryId, g.ProductTypeId, g.ProductId, g.Period }).Select(s => new Data.DbModels.ForecastingSchema.ForecastResult
                {
                    CreatedBy = forecastInfo.CreatedBy,
                    CreatedOn = DateTime.Now,
                    LaboratoryId = s.Key.LaboratoryId,
                    TestId = s.Max(x => x.TestId) == 0 ? null : s.Max(x => x.TestId),
                    ProductId = s.Key.ProductId,
                    ProductTypeId = s.Key.ProductTypeId,
                    AmountForecasted = s.Sum(x => x.AmountForecasted),
                    TotalValue = s.Sum(x => x.TotalValue),
                    Period = s.Max(x => x.Period),
                    DurationDateTime = s.Max(x => x.DurationDateTime),
                    PackSize = s.Max(x => x.PackSize),
                    PackQty = s.Sum(x => x.PackQty),
                    PackPrice = s.Max(x => x.PackPrice),
                    IsActive = true,
                    TotalPrice = s.Sum(x => x.PackQty) * s.Max(x => x.PackPrice) * (1 + forecastInfo.WastageRate)
                }).ToList();

                #endregion

                // Set relation variables with null to avoid unexpected EF errors
                forecastInfo = SetNullable(forecastInfo);

                // Add to the DB
                await _forecastInfoRepository.AddAsync(forecastInfo);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                #region After Creating the Forecast
                // Add forecast categories to the DB
                if (forecastInfo.IsAggregate && forecastCategories.Count() > 0)
                {
                    forecastCategories?.Select(x =>
                    {
                        x.ForecastInfoId = forecastInfo.Id;
                        x.ForecastLaboratories?.Select(y => { y.ForecastInfoId = forecastInfo.Id; return y; }).ToList();
                        return x;
                    }).ToList();

                    await _forecastCategoryRepository.AddRangeAsync(forecastCategories);
                    save = await _unitOfWork.CommitAsync();
                    if (save == 0)
                    {
                        _response.Data = null;
                        _response.IsPassed = false;
                        _response.Message = "Not saved";
                        return _response;
                    }
                }
                // Add forecast target bases to the DB
                if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfoDto.IsTargetBased)
                {
                    targetBases?.Select(x =>
                    {
                        x.ForecastInfoId = forecastInfo.Id;
                        x.ForecastLaboratoryId = forecastInfo.ForecastLaboratories.First(x => x.LaboratoryId == x.LaboratoryId).Id;
                        x.ForecastMorbidityProgramId = forecastInfo.ForecastMorbidityPrograms.First(x => x.ProgramId == x.ProgramId).Id;
                        x.IsActive = true;
                        x.ForecastMorbidityProgram = null;
                        x.ForecastLaboratory = null;
                        x.CreatedBy = forecastInfo.CreatedBy;
                        x.CreatedOn = forecastInfo.CreatedOn;
                        return x;
                    }).ToList();

                    await _forecastMorbidityTargetBaseRepository.AddRangeAsync(targetBases);
                    save = await _unitOfWork.CommitAsync();
                    if (save == 0)
                    {
                        _response.Data = null;
                        _response.IsPassed = false;
                        _response.Message = "Not saved";
                        return _response;
                    }
                }
                #endregion

                _response.Data = null;
                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> UpdateForecastInfo(ForecastInfoDto forecastInfoDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                if (forecastInfoDto.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You are not allowed to update the forecast";
                    return _response;
                }

                // Communicate with the ML
                if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.Service
                    || forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.Consumption
                    || (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfoDto.IsWorldHealthOrganization)
                 )
                {
                    forecastInfoDto = await CommunicateWithML(forecastInfoDto);
                    if (forecastInfoDto == null)
                    {
                        _response.IsPassed = false;
                        _response.Message = "The data entered is not valid";
                        return _response;
                    }
                }
                else if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfoDto.IsTargetBased)
                {
                    forecastInfoDto.ForecastMorbidityTargetBaseDtos = _mapper.Map<List<ForecastMorbidityTargetBaseDto>>(forecastInfoDto.HistoicalTargetBaseDtos);
                }

                // Get Forecast Laboratories from Excel File
                forecastInfoDto = GetForecastLaboratories(forecastInfoDto);

                // Set relation variables with null to avoid unexpected EF errors
                forecastInfoDto = SetNullableForDto(forecastInfoDto);

                // Map DTO to Database
                var forecastInfo = _mapper.Map<Data.DbModels.ForecastingSchema.ForecastInfo>(forecastInfoDto);

                // Set is deleted with true for deleted objects
                forecastInfo = SetIsDeleted(forecastInfo);

                // Set relation variables with null to avoid unexpected EF errors
                forecastInfo = SetNullable(forecastInfo);

                // Move forecast target bases to a variable to add it to the DB after getting the forecastId
                var targetBases = forecastInfo.ForecastMorbidityTargetBases;
                forecastInfo.ForecastMorbidityTargetBases = null;

                // Update to the DB
                _forecastInfoRepository.Update(forecastInfo);

                // Remove all results
                var results = _forecastResultRepository.GetAll(x => x.ForecastInfoId == forecastInfo.Id).ToList();
                _forecastResultRepository.RemoveRange(results);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                // Add forecast target bases to the DB
                if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfoDto.IsTargetBased)
                {
                    targetBases?.Select(x =>
                    {
                        x.ForecastInfo = null;
                        x.ForecastLaboratory = null;
                        x.ForecastMorbidityProgram = null;
                        x.ForecastInfoId = forecastInfo.Id;
                        x.ForecastLaboratoryId = forecastInfo.ForecastLaboratories.First(x => x.LaboratoryId == x.LaboratoryId).Id;
                        x.ForecastMorbidityProgramId = forecastInfo.ForecastMorbidityPrograms.First(x => x.ProgramId == x.ProgramId).Id;
                        x.IsActive = true;
                        return x;
                    }).ToList();

                    await _forecastMorbidityTargetBaseRepository.AddRangeAsync(targetBases.Where(x => x.Id == 0));
                    foreach(var item in targetBases.Where(x => x.Id > 0))
                    {
                        _forecastMorbidityTargetBaseRepository.Update(item);
                    }
                }

                // Add new results
                var newResults = await GetNewCalc(forecastInfo, forecastInfoDto);
                if(newResults.Count > 0)
                {
                    await _forecastResultRepository.AddRangeAsync(newResults);
                }

                // Commit
                save = await _unitOfWork.CommitAsync();
    

                _response.Data = null;
                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> UpdateIsActive(int forecastInfoId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var forecast = await _forecastInfoRepository.GetFirstAsync(x => x.Id == forecastInfoId);
                if (forecast == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (forecast.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You are not allowed to close the forecast";
                    return _response;
                }

                // Update IsActive value
                forecast.IsActive = IsActive;
                forecast.UpdatedBy = LoggedInUserId;
                forecast.UpdatedOn = DateTime.Now;

                // Update on the Database
                _forecastInfoRepository.Update(forecast);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }

            return _response;
        }
        public GeneratedFile ExportForecastInfo(int? pageIndex = null, int? pageSize = null, ForecastInfoFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ForecastingSchema.ForecastInfo> query = null;
            try
            {
                query = _forecastInfoRepository.GetAll()
                                    .Include(x => x.Country)
                                    .Include(x => x.ForecastInfoLevel)
                                    .Include(x => x.ForecastMethodology)
                                    .Include(x => x.ScopeOfTheForecast)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);
                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId);
                    }


                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.IsAggregate != null)
                    {
                        query = query.Where(x => x.IsAggregate == filterDto.IsAggregate);
                    }
                    if (filterDto.IsTargetBased != null && filterDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity)
                    {
                        query = query.Where(x => x.IsTargetBased == filterDto.IsTargetBased);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.ForecastInfoLevelId > 0)
                    {
                        query = query.Where(x => x.ForecastInfoLevelId == filterDto.ForecastInfoLevelId);
                    }
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }
                    if (filterDto.ForecastMethodologyId > 0)
                    {
                        query = query.Where(x => x.ForecastMethodologyId == filterDto.ForecastMethodologyId);
                    }
                    if (filterDto.ScopeOfTheForecastId > 0)
                    {
                        query = query.Where(x => x.ScopeOfTheForecastId == filterDto.ScopeOfTheForecastId);
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

                var dataList = _mapper.Map<List<ExportForecastInfoDto>>(query.ToList());

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
        // Validators methods
        public bool IsNameUnique(ForecastInfoDto forecastInfoDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _forecastInfoRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != forecastInfoDto.Id
                                                && x.Name.ToLower().Trim() == forecastInfoDto.Name.ToLower().Trim());

            // Security Filter
            searchResult = searchResult.Where(x => x.CreatedBy == LoggedInUserId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        // Helper methods
        private ForecastInfoDto SetNullableForDto(ForecastInfoDto forecast)
        {
            // Forcast info
            forecast.Creator = null;
            forecast.Updator = null;
            // Children
            forecast.ForecastInstrumentDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastPatientGroupDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastTestDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastCategoryDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastLaboratoryDtos?.Select(y =>
                {
                    y.Creator = null;
                    y.Updator = null;
                    return y;
                }).ToList();

                return x;
            }).ToList();
            forecast.ForecastLaboratoryDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastMorbidityTargetBaseDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastMorbidityWhoBaseDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastMorbidityTestingProtocolMonthDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastLaboratoryConsumptionDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastLaboratoryTestServiceDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastPatientAssumptionValueDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastProductAssumptionValueDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastTestingAssumptionValueDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            forecast.ForecastMorbidityProgramDtos?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                return x;
            }).ToList();
            // Return
            return forecast;
        }
        private Data.DbModels.ForecastingSchema.ForecastInfo SetNullable(Data.DbModels.ForecastingSchema.ForecastInfo forecast)
        {
            // Forcast info
            forecast.ForecastInfoLevel = null;
            forecast.Country = null;
            forecast.ForecastMethodology = null;
            forecast.ScopeOfTheForecast = null;
            // Children
            forecast.ForecastInstruments?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.Instrument = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastPatientGroups?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.PatientGroup = null;
                x.Program = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastTests?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.Test = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastCategories?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                x.ForecastLaboratories?.Select(y =>
                {
                    y.Creator = null;
                    y.Updator = null;
                    y.ForecastInfo = null;
                    y.Laboratory = null;
                    y.ForecastCategory = null;
                    y.CreatedBy = forecast.CreatedBy;
                    y.CreatedOn = DateTime.Now;
                    y.ForecastInfoId = forecast.Id;
                    return y;
                }).ToList();

                return x;
            }).ToList();
            forecast.ForecastLaboratories?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.Laboratory = null;
                x.ForecastCategory = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastMorbidityTargetBases?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastLaboratory = null;
                x.ForecastMorbidityProgram = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastMorbidityWhoBases?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.Disease = null;
                x.Country = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastMorbidityTestingProtocolMonths?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.CalculationPeriodMonth = null;
                x.PatientGroup = null;
                x.Test = null;
                x.TestingProtocol = null;
                x.Program = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastLaboratoryConsumptions?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.Product = null;
                x.Laboratory = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastLaboratoryTestServices?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.Test = null;
                x.Laboratory = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastPatientAssumptionValues?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.PatientAssumptionParameter = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastProductAssumptionValues?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.ProductAssumptionParameter = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastTestingAssumptionValues?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.TestingAssumptionParameter = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            forecast.ForecastMorbidityPrograms?.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.ForecastInfo = null;
                x.Program = null;
                x.CreatedBy = forecast.CreatedBy;
                x.CreatedOn = DateTime.Now;
                x.ForecastInfoId = forecast.Id;
                return x;
            }).ToList();
            // Return
            return forecast;
        }
        private async Task<ForecastInfoDto> CommunicateWithML(ForecastInfoDto forecastInfoDto)
        {
            // Get the request body
            var requestBody = GetRequestBody(forecastInfoDto);

            // HTTP Request
            string url = _configuration["ML:PredictURL"];
            HttpClient http = new HttpClient();
            var json = JsonConvert.SerializeObject(requestBody);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var data = await http.PostAsync(url, body);
            if (!data.IsSuccessStatusCode)
            {
                return null;
            }
            var readAsString = await data.Content.ReadAsStringAsync();
            var responseFromML = JsonConvert.DeserializeObject<List<MLResponseDto>>(readAsString);

            // Handle response
            forecastInfoDto = GetRequestResponse(forecastInfoDto, responseFromML);

            // Return
            return forecastInfoDto;
        }
        private List<MLBodyDto> GetRequestBody(ForecastInfoDto forecastInfoDto)
        {
            var startDate = forecastInfoDto.StartDate.AddDays(forecastInfoDto.StartDate.Day * -1).Date;

            if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.Service)
            {
                var labIds = forecastInfoDto.HistoicalServiceDataDtos.Select(x => x.LaboratoryId);
                var testIds = forecastInfoDto.HistoicalServiceDataDtos.Select(x => x.TestId);
                var laboratoryTestServices = _laboratoryTestServiceRepository.GetAll(x =>
                                                   labIds.Contains(x.LaboratoryId)
                                                   && testIds.Contains(x.TestId)
                                                   && !x.IsDeleted && x.IsActive
                                                   && x.ServiceDuration.AddDays(x.ServiceDuration.Day * -1).Date < startDate).ToList();

                var result = _mapper.Map<List<MLBodyDto>>(forecastInfoDto.HistoicalServiceDataDtos);
                foreach (var item in result)
                {
                    item.from = forecastInfoDto.StartDate.ToString("M/d/yy");
                    item.to = forecastInfoDto.EndDate.ToString("M/d/yy");
                    item.historyData = _mapper.Map<List<MLBodyHistoryDataDto>>(laboratoryTestServices.Where(x => x.TestId == item.test && x.LaboratoryId == item.site));
                }
                return result;
            }
            else if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.Consumption)
            {
                var labIds = forecastInfoDto.HistoicalConsumptionDtos.Select(x => x.LaboratoryId);
                var productIds = forecastInfoDto.HistoicalConsumptionDtos.Select(x => x.ProductId);
                var laboratoryConsumptions = _laboratoryConsumptionRepository.GetAll(x =>
                                                    labIds.Contains(x.LaboratoryId)
                                                    && productIds.Contains(x.ProductId)
                                                    && !x.IsDeleted && x.IsActive
                                                    && x.ConsumptionDuration.AddDays(x.ConsumptionDuration.Day * -1).Date < startDate).ToList();

                var result = _mapper.Map<List<MLBodyDto>>(forecastInfoDto.HistoicalConsumptionDtos);
                foreach (var item in result)
                {
                    item.from = forecastInfoDto.StartDate.ToString("M/d/yy");
                    item.to = forecastInfoDto.EndDate.ToString("M/d/yy");
                    item.historyData = _mapper.Map<List<MLBodyHistoryDataDto>>(laboratoryConsumptions.Where(x => x.ProductId == item.product && x.LaboratoryId == item.site));
                }
                return result;
            }
            else if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfoDto.IsWorldHealthOrganization)
            {
                var countryIds = forecastInfoDto.HistoicalWhoBaseDtos.Select(x => x.CountryId);
                var diseaseIds = forecastInfoDto.HistoicalWhoBaseDtos.Select(x => x.DiseaseId);

                var incidents = _countryDiseaseIncidentRepository.GetAll(x =>
                                                    countryIds.Contains(x.CountryId)
                                                    && diseaseIds.Contains(x.DiseaseId)
                                                    && !x.IsDeleted && x.IsActive
                                                    && x.Year < (DateTime.Now.Year)
                                                    && x.Year >= (DateTime.Now.Year - 6)).ToList();

                var result = _mapper.Map<List<MLBodyDto>>(forecastInfoDto.HistoicalWhoBaseDtos);
                foreach (var item in result)
                {
                    item.from = forecastInfoDto.StartDate.ToString("M/d/yy");
                    item.to = forecastInfoDto.EndDate.ToString("M/d/yy");
                    item.historyData = _mapper.Map<List<MLBodyHistoryDataDto>>(incidents.Where(x => x.CountryId == item.country && x.DiseaseId == item.disease));
                }
                return result;
            }

            // Return
            return null;
        }
        private ForecastInfoDto GetRequestResponse(ForecastInfoDto forecastInfoDto, List<MLResponseDto> response)
        {
            if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.Service)
            {
                forecastInfoDto.ForecastLaboratoryTestServiceDtos = new List<ForecastLaboratoryTestServiceDto>();
                foreach (var item in response)
                {
                    var testServices = _mapper.Map<List<ForecastLaboratoryTestServiceDto>>(item.PredictionData);
                    testServices.Select(x => { x.TestId = item.Test; x.LaboratoryId = item.Site; x.IsActive = true; return x; }).ToList();
                    forecastInfoDto.ForecastLaboratoryTestServiceDtos.AddRange(testServices);
                }
            }
            else if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.Consumption)
            {
                forecastInfoDto.ForecastLaboratoryConsumptionDtos = new List<ForecastLaboratoryConsumptionDto>();
                foreach (var item in response)
                {
                    var consumptions = _mapper.Map<List<ForecastLaboratoryConsumptionDto>>(item.PredictionData);
                    consumptions.Select(x => { x.ProductId = item.Product; x.LaboratoryId = item.Site; x.IsActive = true; return x; }).ToList();
                    forecastInfoDto.ForecastLaboratoryConsumptionDtos.AddRange(consumptions);
                }
            }
            else if (forecastInfoDto.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfoDto.IsWorldHealthOrganization)
            {
                forecastInfoDto.ForecastMorbidityWhoBaseDtos = new List<ForecastMorbidityWhoBaseDto>();
                foreach (var item in response)
                {
                    var who = _mapper.Map<List<ForecastMorbidityWhoBaseDto>>(item.PredictionData);
                    who.Select(x => { x.CountryId = item.Country; x.DiseaseId = item.Disease; x.IsActive = true; return x; }).ToList();
                    forecastInfoDto.ForecastMorbidityWhoBaseDtos.AddRange(who);
                }
            }

            // Return
            return forecastInfoDto;
        }
        private ForecastInfoDto GetForecastLaboratories(ForecastInfoDto forecastInfoDto)
        {
            switch (forecastInfoDto.ForecastMethodologyId)
            {
                case (int)ForecastMethodologyEnum.Service:
                    forecastInfoDto.ForecastLaboratoryDtos = _mapper.Map<List<ForecastLaboratoryDto>>(forecastInfoDto.HistoicalServiceDataDtos);
                    break;
                case (int)ForecastMethodologyEnum.Consumption:
                    forecastInfoDto.ForecastLaboratoryDtos = _mapper.Map<List<ForecastLaboratoryDto>>(forecastInfoDto.HistoicalConsumptionDtos);
                    break;
                case (int)ForecastMethodologyEnum.DempgraphicMorbidity:
                    if (forecastInfoDto.IsTargetBased)
                    {
                        forecastInfoDto.ForecastLaboratoryDtos = _mapper.Map<List<ForecastLaboratoryDto>>(forecastInfoDto.HistoicalTargetBaseDtos);
                    }
                    else
                    {

                    }
                    break;
                default:
                    break;
            }
            forecastInfoDto.ForecastLaboratoryDtos = forecastInfoDto.ForecastLaboratoryDtos.Distinct().ToList();
            if (forecastInfoDto.IsAggregate) // Will contain the category
            {
                forecastInfoDto.ForecastCategoryDtos = new List<ForecastCategoryDto>();
                var grouped = forecastInfoDto.ForecastLaboratoryDtos.GroupBy(x => x.ForecastCategoryName);
                foreach (var item in grouped)
                {
                    forecastInfoDto.ForecastCategoryDtos.Add(new ForecastCategoryDto
                    {
                        IsActive = true,
                        Name = item.Key,
                        ForecastLaboratoryDtos = item.Select(x => { x.ForecastCategoryId = 0; x.IsActive = true; return x; }).ToList()
                    });
                }
                forecastInfoDto.ForecastLaboratoryDtos = null;
            }
            else
            {
                forecastInfoDto.ForecastLaboratoryDtos.Select(x => { x.ForecastCategoryId = null; x.IsActive = true; return x; }).ToList();
            }

            // Return
            return forecastInfoDto;
        }
        private async Task<List<Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService>> ML_TragetBase(Data.DbModels.ForecastingSchema.ForecastInfo forecastInfo)
        {
            var result = new List<Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService>();
            var startDate = forecastInfo.StartDate.AddDays(forecastInfo.StartDate.Day * -1).Date;

            var labIds = forecastInfo.ForecastLaboratoryTestServices.Select(x => x.LaboratoryId);
            var testIds = forecastInfo.ForecastLaboratoryTestServices.Select(x => x.TestId);
            var laboratoryTestServices = _laboratoryTestServiceRepository.GetAll(x =>
                                               labIds.Contains(x.LaboratoryId)
                                               && testIds.Contains(x.TestId)
                                               && !x.IsDeleted && x.IsActive
                                               && x.ServiceDuration.AddDays(x.ServiceDuration.Day * -1).Date < startDate).ToList();

            var reqBody = _mapper.Map<List<MLBodyDto>>(forecastInfo.ForecastLaboratoryTestServices);
            foreach (var item in reqBody)
            {
                item.from = forecastInfo.StartDate.ToString("M/d/yy");
                item.to = forecastInfo.EndDate.ToString("M/d/yy");
                item.historyData = _mapper.Map<List<MLBodyHistoryDataDto>>(laboratoryTestServices.Where(x => x.TestId == item.test && x.LaboratoryId == item.site));
            }


            // HTTP Request
            string url = _configuration["ML:PredictURL"];
            HttpClient http = new HttpClient();
            var json = JsonConvert.SerializeObject(reqBody);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var data = await http.PostAsync(url, body);
            if (!data.IsSuccessStatusCode)
            {
                return null;
            }
            var readAsString = await data.Content.ReadAsStringAsync();
            var responseFromML = JsonConvert.DeserializeObject<List<MLResponseDto>>(readAsString);

            // Handle response
            forecastInfo.ForecastLaboratoryTestServices = new List<Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService>();
            foreach (var item in responseFromML)
            {
                var testServices = _mapper.Map<List<Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService>>(item.PredictionData);
                testServices.Select(x => { x.TestId = item.Test; x.LaboratoryId = item.Site; x.IsActive = true; return x; }).ToList();
                result.AddRange(testServices);
            }

            // Return
            return result;
        }
        // Update
        private Data.DbModels.ForecastingSchema.ForecastInfo SetIsDeleted(Data.DbModels.ForecastingSchema.ForecastInfo forecast)
        {
            var oldForecast = _forecastInfoRepository.GetAll()
                                    .Include(x => x.ForecastInstruments)
                                    .Include(x => x.ForecastPatientGroups)
                                    .Include(x => x.ForecastLaboratories)
                                    .Include(x => x.ForecastTests)
                                    .Include(x => x.ForecastPatientAssumptionValues)
                                    .Include(x => x.ForecastProductAssumptionValues)
                                    .Include(x => x.ForecastTestingAssumptionValues)
                                    .Include(x => x.ForecastMorbidityPrograms)
                                    .Include(x => x.ForecastMorbidityTestingProtocolMonths)
                                    .Include(x => x.ForecastMorbidityTargetBases)
                                        .ThenInclude(x => x.ForecastLaboratory)
                                    .Include(x => x.ForecastMorbidityTargetBases)
                                        .ThenInclude(x => x.ForecastMorbidityProgram)
                                    .Include(x => x.ForecastLaboratoryConsumptions)
                                    .Include(x => x.ForecastLaboratoryTestServices)
                                    .Include(x => x.ForecastMorbidityWhoBases)
                                    .FirstOrDefault(x => x.Id == forecast.Id);

            // Filter Children
            oldForecast.ForecastInstruments = oldForecast.ForecastInstruments.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastPatientGroups = oldForecast.ForecastPatientGroups.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastTests = oldForecast.ForecastTests.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastPatientAssumptionValues = oldForecast.ForecastPatientAssumptionValues.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastProductAssumptionValues = oldForecast.ForecastProductAssumptionValues.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastMorbidityPrograms = oldForecast.ForecastMorbidityPrograms.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastMorbidityTestingProtocolMonths = oldForecast.ForecastMorbidityTestingProtocolMonths.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastMorbidityTargetBases = oldForecast.ForecastMorbidityTargetBases.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastLaboratoryConsumptions = oldForecast.ForecastLaboratoryConsumptions.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastLaboratoryTestServices = oldForecast.ForecastLaboratoryTestServices.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastLaboratories = oldForecast.ForecastLaboratories.Where(x => !x.IsDeleted).ToList();
            oldForecast.ForecastMorbidityWhoBases = oldForecast.ForecastMorbidityWhoBases.Where(x => !x.IsDeleted).ToList();


            // Instruments
            foreach (var item in forecast.ForecastInstruments)
            {
                var found = oldForecast.ForecastInstruments.FirstOrDefault(x => x.InstrumentId == item.InstrumentId);
                if (found != null)
                {
                    item.Id = found.Id;
                }
            }
            var newInstrumentIds = forecast.ForecastInstruments.Select(x => x.Id);
            var instrumentsToDelete = oldForecast.ForecastInstruments.Where(x => !newInstrumentIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastInstruments = forecast.ForecastInstruments.Concat(instrumentsToDelete).ToList();

            // Category & Lab
            if (forecast.IsAggregate) // Aggregate
            {
                forecast.ForecastLaboratories = new List<Data.DbModels.ForecastingSchema.ForecastLaboratory>();

                foreach (var item in forecast.ForecastCategories)
                {
                    var found = oldForecast.ForecastCategories.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                    if (found != null)
                    {
                        item.Id = found.Id;
                    }

                    // Deleted Labs
                    var newLaboratoryIds = item.ForecastLaboratories.Select(x => x.Id);
                    var laboratoriesToDelete = oldForecast.ForecastLaboratories.Where(x => !newLaboratoryIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
                    forecast.ForecastLaboratories = forecast.ForecastLaboratories.Concat(laboratoriesToDelete).ToList();
                }
                var newCatIds = forecast.ForecastCategories.Select(x => x.Id);
                var catsToDelete = oldForecast.ForecastCategories.Where(x => !newCatIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
                forecast.ForecastCategories = forecast.ForecastCategories.Concat(catsToDelete).ToList();
            }
            else // Site by Site
            {
                forecast.ForecastCategories = null;
                foreach (var item in forecast.ForecastLaboratories)
                {
                    var found = oldForecast.ForecastLaboratories.FirstOrDefault(x => x.LaboratoryId == item.LaboratoryId);
                    if (found != null)
                    {
                        item.Id = found.Id;
                    }
                }
                var newLaboratoryIds = forecast.ForecastLaboratories.Select(x => x.Id);
                var laboratoriesToDelete = oldForecast.ForecastLaboratories.Where(x => !newLaboratoryIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
                forecast.ForecastLaboratories = forecast.ForecastLaboratories.Concat(laboratoriesToDelete).ToList();
            }

            // Test
            foreach (var item in forecast.ForecastTests)
            {
                var found = oldForecast.ForecastTests.FirstOrDefault(x => x.TestId == item.TestId);
                if (found != null)
                {
                    item.Id = found.Id;
                }
            }
            var newTestIds = forecast.ForecastTests.Select(x => x.TestId);
            var testsToDelete = oldForecast.ForecastTests.Where(x => !newTestIds.Contains(x.TestId)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastTests = forecast.ForecastTests.Concat(testsToDelete).ToList();

            // Program
            foreach (var item in forecast.ForecastMorbidityPrograms)
            {
                var found = oldForecast.ForecastMorbidityPrograms.FirstOrDefault(x => x.ProgramId == item.ProgramId);
                if (found != null)
                {
                    item.Id = found.Id;
                }
            }
            var newProgramIds = forecast.ForecastMorbidityPrograms.Select(x => x.Id);
            var programsToDelete = oldForecast.ForecastMorbidityPrograms.Where(x => !newProgramIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastMorbidityPrograms = forecast.ForecastMorbidityPrograms.Concat(programsToDelete).ToList();

            // PatientGroup
            foreach (var item in forecast.ForecastPatientGroups)
            {
                var found = oldForecast.ForecastPatientGroups.FirstOrDefault(x => x.PatientGroupId == item.PatientGroupId && x.ProgramId == item.ProgramId);
                if (found != null)
                {
                    item.Id = found.Id;
                }
            }
            var newPatientGroupIds = forecast.ForecastPatientGroups.Select(x => x.Id);
            var patientGroupsToDelete = oldForecast.ForecastPatientGroups.Where(x => !newPatientGroupIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastPatientGroups = forecast.ForecastPatientGroups.Concat(patientGroupsToDelete).ToList();

            // Patient Assumption
            foreach (var item in forecast.ForecastPatientAssumptionValues)
            {
                var found = oldForecast.ForecastPatientAssumptionValues.FirstOrDefault(x => x.PatientAssumptionParameterId == item.PatientAssumptionParameterId);
                if (found != null)
                {
                    item.Id = found.Id;
                }
            }
            var newPatientAssIds = forecast.ForecastPatientAssumptionValues.Select(x => x.Id);
            var patientAssToDelete = oldForecast.ForecastPatientAssumptionValues.Where(x => !newPatientAssIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastPatientAssumptionValues = forecast.ForecastPatientAssumptionValues.Concat(patientAssToDelete).ToList();

            // Product Assumption
            foreach (var item in forecast.ForecastProductAssumptionValues)
            {
                var found = oldForecast.ForecastProductAssumptionValues.FirstOrDefault(x => x.ProductAssumptionParameterId == item.ProductAssumptionParameterId);
                if (found != null)
                {
                    item.Id = found.Id;
                }
            }
            var newProductAssIds = forecast.ForecastProductAssumptionValues.Select(x => x.Id);
            var productAssToDelete = oldForecast.ForecastProductAssumptionValues.Where(x => !newProductAssIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastProductAssumptionValues = forecast.ForecastProductAssumptionValues.Concat(productAssToDelete).ToList();

            // Testing Assumption
            foreach (var item in forecast.ForecastTestingAssumptionValues)
            {
                var found = oldForecast.ForecastProductAssumptionValues.FirstOrDefault(x => x.ProductAssumptionParameterId == item.TestingAssumptionParameterId);
                if (found != null)
                {
                    item.Id = found.Id;
                }
            }
            var newTestAssIds = forecast.ForecastTestingAssumptionValues.Select(x => x.Id);
            var testAssToDelete = oldForecast.ForecastTestingAssumptionValues.Where(x => !newTestAssIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastTestingAssumptionValues = forecast.ForecastTestingAssumptionValues.Concat(testAssToDelete).ToList();

            // Months
            var newMonthIds = forecast.ForecastMorbidityTestingProtocolMonths.Select(x => x.Id);
            var monthsToDelete = oldForecast.ForecastMorbidityTestingProtocolMonths.Where(x => !newMonthIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastMorbidityTestingProtocolMonths = forecast.ForecastMorbidityTestingProtocolMonths.Concat(monthsToDelete).ToList();

            // Target Base
            var newTargetBaseIds = forecast.ForecastMorbidityTargetBases.Select(x => x.Id);
            var targetBaseToDelete = oldForecast.ForecastMorbidityTargetBases.Where(x => !newTargetBaseIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastMorbidityTargetBases = forecast.ForecastMorbidityTargetBases.Concat(targetBaseToDelete).ToList();

            // WHO Base
            var newWhoBaseIds = forecast.ForecastMorbidityWhoBases.Select(x => x.Id);
            var whoBaseToDelete = oldForecast.ForecastMorbidityWhoBases.Where(x => !newWhoBaseIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastMorbidityWhoBases = forecast.ForecastMorbidityWhoBases.Concat(whoBaseToDelete).ToList();

            // Consumptions
            var newConsumtionIds = forecast.ForecastLaboratoryConsumptions.Select(x => x.Id);
            var consumtionToDelete = oldForecast.ForecastLaboratoryConsumptions.Where(x => !newConsumtionIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastLaboratoryConsumptions = forecast.ForecastLaboratoryConsumptions.Concat(consumtionToDelete).ToList();

            // Test Service
            var newServiceIds = forecast.ForecastLaboratoryTestServices.Select(x => x.Id);
            var serviceToDelete = oldForecast.ForecastLaboratoryTestServices.Where(x => !newServiceIds.Contains(x.Id)).Select(x => { x.IsDeleted = true; return x; });
            forecast.ForecastLaboratoryTestServices = forecast.ForecastLaboratoryTestServices.Concat(serviceToDelete).ToList();

            // Return
            return forecast;
        }
        private async Task<List<Data.DbModels.ForecastingSchema.ForecastResult>> GetNewCalc(Data.DbModels.ForecastingSchema.ForecastInfo forecastInfo, ForecastInfoDto forecastInfoDto)
        {
            #region Calculation Module  
            // Init the result value
            var ForecastResults = new List<Data.DbModels.ForecastingSchema.ForecastResult>();

            if (forecastInfo.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfo.IsTargetBased)
            {
                // Assumtions Ids
                var patientAssumptionIds = forecastInfo.ForecastPatientAssumptionValues.Select(x => x.PatientAssumptionParameterId).ToList();
                var testingAssumptionIds = forecastInfo.ForecastTestingAssumptionValues.Select(x => x.TestingAssumptionParameterId).ToList();
                // Assumtions Values
                var patientAssumptions = _patientAssumptionParameterRepository.GetAll(x => !x.IsDeleted && x.IsActive && patientAssumptionIds.Contains(x.Id)).ToList();
                var testingAssumptions = _testingAssumptionParameterRepository.GetAll(x => !x.IsDeleted && x.IsActive && testingAssumptionIds.Contains(x.Id)).ToList();
                // Testing Protocols
                var testingProtocolIds = forecastInfo.ForecastMorbidityTestingProtocolMonths.Select(x => x.TestingProtocolId).ToList();
                var testingProtocolsData = _testingProtocolRepository.GetAll(x => !x.IsDeleted && x.IsActive && testingProtocolIds.Contains(x.Id)).ToList();

                foreach (var targetBase in forecastInfoDto.HistoicalTargetBaseDtos)
                {
                    decimal CurrentPatient = targetBase.CurrentPatient;
                    decimal NewPatient = targetBase.TargetPatient - CurrentPatient;
                    decimal AttritionRate = 0;      //AttritionRate > 0 : Positive, < 0 : Negative
                    foreach (var FPAV in forecastInfo.ForecastPatientAssumptionValues)
                    {
                        var patientAssumptionParameter = patientAssumptions.FirstOrDefault(x => x.Id == FPAV.PatientAssumptionParameterId);
                        if (patientAssumptionParameter != null)
                        {
                            if (patientAssumptionParameter.IsPositive)
                            {
                                AttritionRate += Convert.ToDecimal(FPAV.Value);
                            }
                            if (patientAssumptionParameter.IsNegative)
                            {
                                AttritionRate -= Convert.ToDecimal(FPAV.Value);
                            }
                        }
                    }
                    decimal RepeatRate = 0;             //RepeatRate > 0 : Positive, < 0 : Negative
                    foreach (var FTAV in forecastInfo.ForecastTestingAssumptionValues)
                    {
                        var testingAssumptionParameter = testingAssumptions.FirstOrDefault(x => x.Id == FTAV.TestingAssumptionParameterId);
                        if (testingAssumptionParameter != null)
                        {
                            if (testingAssumptionParameter.IsPositive)
                            {
                                RepeatRate += Convert.ToDecimal(FTAV.Value);
                            }
                            if (testingAssumptionParameter.IsNegative)
                            {
                                RepeatRate -= Convert.ToDecimal(FTAV.Value);
                            }
                        }
                    }

                    var tests = forecastInfo.ForecastMorbidityTestingProtocolMonths.GroupBy(g => new { g.TestId }).Select(s => new { s.Key.TestId }).ToList();
                    var groups = forecastInfo.ForecastMorbidityTestingProtocolMonths.GroupBy(g => new { g.PatientGroupId }).Select(s => new { s.Key.PatientGroupId }).ToList();
                    for (int i = 0; i < tests.Count; i++)
                    {
                        float TotalTestNum = 0;   //TotalTest# Per Site, Test, Cycle
                        List<TestNumCycleForecastByGroup> TNCFByGroupList = new List<TestNumCycleForecastByGroup>();

                        for (int j = 0; j < groups.Count; j++)
                        {
                            var testingProtocols = forecastInfo.ForecastMorbidityTestingProtocolMonths
                                .Where(x => !x.IsDeleted && tests[i].TestId == x.TestId && groups[j].PatientGroupId == x.PatientGroupId)
                                .OrderBy(x => x.CalculationPeriodMonthId)
                                .ToList();

                            decimal GroupPercentage = forecastInfo.ForecastPatientGroups.Where(x => x.PatientGroupId == groups[j].PatientGroupId).Select(s => s.Percentage).FirstOrDefault();
                            int testingProtocolId = testingProtocols.Select(x => x.TestingProtocolId).FirstOrDefault();
                            int totalTestPerYear = testingProtocolsData.Where(x => x.Id == testingProtocolId).Select(x => x.TestAfterFirstYear).FirstOrDefault();
                            float CurrentTestNumAfterCycle = (float)totalTestPerYear / testingProtocols.Count;

                            for (int k = 0; k < forecastInfo.Duration; k++)
                            {
                                float GroupCurrentPatient = 0.0f;
                                float GroupNewPatient = 0.0f;
                                float stepPatients = (float)NewPatient / forecastInfo.Duration;
                                decimal NewTestNumPerMonth = testingProtocols[k % testingProtocols.Count].Value ?? 0;
                                if (stepPatients > 0)
                                {
                                    GroupCurrentPatient = (float)targetBase.CurrentPatient + stepPatients * k;
                                    GroupNewPatient = stepPatients;
                                }
                                else
                                {
                                    GroupCurrentPatient = (float)targetBase.TargetPatient + stepPatients * (k + 1);
                                    GroupNewPatient = 0;
                                }
                                GroupCurrentPatient *= (float)(GroupPercentage / 100) * (float)(1 + AttritionRate / 100);
                                GroupNewPatient *= (float)(GroupPercentage / 100) * (float)(1 + AttritionRate / 100);
                                TotalTestNum = GroupCurrentPatient * CurrentTestNumAfterCycle + GroupNewPatient * (float)NewTestNumPerMonth;
                                TotalTestNum *= (float)(1 + RepeatRate / 100);

                                TestNumCycleForecastByGroup TNCFByGroup = new TestNumCycleForecastByGroup();
                                TNCFByGroup.GroupId = testingProtocolId;
                                TNCFByGroup.TimeStamp = forecastInfo.StartDate.AddMonths(k + 1);
                                TNCFByGroup.Forecast = Convert.ToDecimal(TotalTestNum);
                                TNCFByGroupList.Add(TNCFByGroup);
                            }
                        }
                        if (TNCFByGroupList.Count > 0)
                        {
                            var TNCFList = TNCFByGroupList.GroupBy(g => g.TimeStamp).Select(s => new TestNumCycleForecast
                            {
                                TimeStamp = s.Key,
                                Forecast = s.Sum(x => x.Forecast)
                            }).ToList();

                            for (int idx = 0; idx < TNCFList.Count; idx++)
                            {
                                var TS = new Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService();
                                TS.LaboratoryId = targetBase.LaboratoryId;
                                TS.TestId = tests[i].TestId;
                                TS.AmountForecasted = TNCFList[idx].Forecast;
                                forecastInfo.ForecastLaboratoryTestServices.Add(TS);
                            }
                        }
                    }
                }

                // Call ML after creating test services
                forecastInfo.ForecastLaboratoryTestServices = await ML_TragetBase(forecastInfo);
            }

            if (forecastInfo.ForecastMethodologyId == (int)ForecastMethodologyEnum.Service ||
                (forecastInfo.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && forecastInfo.IsTargetBased)
                )
            {
                // Vars
                var testIds = forecastInfo.ForecastLaboratoryTestServices.Select(x => x.TestId).ToList();
                var labIds = forecastInfo.ForecastLaboratoryTestServices.Select(x => x.LaboratoryId).ToList();
                // Get Data
                var ProductUsagesData = _productUsageRepository.GetAll(x => !x.IsDeleted && x.IsActive && testIds.Contains(x.TestId.Value))
                                                            .Include(x => x.Product)
                                                            .Include(x => x.Instrument)
                                                            .ToList();
                var instrumentIds = ProductUsagesData.Select(x => x.InstrumentId).ToList();
                var LaboratoryInstrumentData = _laboratoryInstrumentRepository.GetAll(x => !x.IsDeleted && x.IsActive && labIds.Contains(x.LaboratoryId) && instrumentIds.Contains(x.InstrumentId)).ToList();

                foreach (var testService in forecastInfo.ForecastLaboratoryTestServices)
                {
                    /*** Table Operation Comment Format : Get Rows from XXX.XXX(table name) table by XXX(Where fields) ***/
                    //Get Rows from Product.ProductUsage table by Test 
                    var ProductUsages = ProductUsagesData.Where(x => x.TestId == testService.TestId).ToList();

                    //For loop Product Usages to get Product# from Test#
                    foreach (var productUsage in ProductUsages)
                    {
                        decimal Qty = 0;
                        if (testService.LaboratoryId > 0)
                        {
                            decimal workingDays = 22;
                            //Get Rows from Laboratory.LaboratoryInstruments table by Laboratory & Instrument
                            var LaboratoryInstrument = LaboratoryInstrumentData.FirstOrDefault(x => x.LaboratoryId == testService.LaboratoryId && x.InstrumentId == productUsage.InstrumentId);

                            if (productUsage.PerPeriodPerInstrument || productUsage.PerPeriod)
                            {
                                #region Consumable Usage Rate
                                if (productUsage.PerPeriodPerInstrument)
                                {
                                    if (LaboratoryInstrument != null)
                                    {
                                        switch (productUsage.CountryPeriodId)
                                        {
                                            case (int)CountryPeriodEnum.Weekly:
                                                Qty = productUsage.Amount * (workingDays / 4) * LaboratoryInstrument.Quantity;
                                                break;
                                            case (int)CountryPeriodEnum.Monthly:
                                                Qty = productUsage.Amount * 1 * LaboratoryInstrument.Quantity;
                                                break;
                                            case (int)CountryPeriodEnum.Quarterly:
                                                Qty = productUsage.Amount * (1 / 3) * LaboratoryInstrument.Quantity;
                                                break;
                                            case (int)CountryPeriodEnum.Annualy:
                                                Qty = productUsage.Amount * (1 / 12) * LaboratoryInstrument.Quantity;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                if (productUsage.PerPeriod)
                                {
                                    switch (productUsage.CountryPeriodId)
                                    {
                                        case (int)CountryPeriodEnum.Weekly:
                                            Qty = productUsage.Amount * workingDays;
                                            break;
                                        case (int)CountryPeriodEnum.Monthly:
                                            Qty = productUsage.Amount * (workingDays / 4);
                                            break;
                                        case (int)CountryPeriodEnum.Quarterly:
                                            Qty = productUsage.Amount * (1 / 3);
                                            break;
                                        case (int)CountryPeriodEnum.Annualy:
                                            Qty = productUsage.Amount * (1 / 12);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                //if (productUsage.PerTest)
                                //{
                                //    Qty = testService.AmountForecasted * productUsage.QuantityNeededPerTest;
                                //}
                                #endregion
                            }
                            else
                            {
                                if (!productUsage.IsForControl)
                                {
                                    #region Test Product Usage Rate
                                    decimal percentage = 1;
                                    if (LaboratoryInstrument != null)
                                    {
                                        if (LaboratoryInstrument.TestRunPercentage > 1 && LaboratoryInstrument.TestRunPercentage < 100)
                                        {
                                            percentage = LaboratoryInstrument.TestRunPercentage / 100;

                                        }
                                    }
                                    Qty = productUsage.Amount * testService.AmountForecasted * percentage;
                                    #endregion
                                }
                                else
                                {
                                    #region QC Usage Rate
                                    //Get Rows from Product.Instrument table by Id
                                    var Instrument = productUsage.Instrument;
                                    switch (Instrument.ControlRequirementUnitId)
                                    {
                                        case (int)ControlRequirementUnitEnum.Daily:
                                            Qty = productUsage.Amount * workingDays * Instrument.ControlRequirement;
                                            break;
                                        case (int)ControlRequirementUnitEnum.Weekly:
                                            Qty = productUsage.Amount * (workingDays / 4) * Instrument.ControlRequirement;
                                            break;
                                        case (int)ControlRequirementUnitEnum.Monthly:
                                            Qty = productUsage.Amount * 1 * Instrument.ControlRequirement;
                                            break;
                                        default:
                                            break;
                                    }
                                    #endregion
                                }
                            }
                        }
                        var packSize = productUsage.Product.PackSize;
                        var ForecastResult = new Data.DbModels.ForecastingSchema.ForecastResult
                        {
                            LaboratoryId = testService.LaboratoryId,
                            TestId = testService.TestId,
                            ProductId = productUsage.ProductId,
                            ProductTypeId = productUsage.Product.ProductTypeId,
                            AmountForecasted = testService.AmountForecasted,
                            TotalValue = Qty,
                            Period = testService.Period,
                            DurationDateTime = Convert.ToDateTime(testService.Period),
                            PackSize = packSize,
                            PackQty = int.Parse(decimal.Round(packSize == 0 ? 0 : (Qty / packSize), 0).ToString()),
                            PackPrice = productUsage.Product.ManufacturerPrice  //Need to update with Real Price for Level
                        };

                        ForecastResults.Add(ForecastResult);
                    }
                }
            }
            else if (forecastInfo.ForecastMethodologyId == (int)ForecastMethodologyEnum.Consumption)
            {
                // Consumptions
                var ProductIds = forecastInfo.ForecastLaboratoryConsumptions.Select(x => x.ProductId).ToList();
                var Products = _productRepository.GetAll(x => !x.IsDeleted && x.IsActive && ProductIds.Contains(x.Id)).ToList();

                foreach (var consumption in forecastInfo.ForecastLaboratoryConsumptions)
                {
                    if (consumption.ProductId > 0)
                    {
                        var product = Products.First(x => x.Id == consumption.ProductId);
                        var ForecastResult = new Data.DbModels.ForecastingSchema.ForecastResult
                        {
                            LaboratoryId = consumption.LaboratoryId,
                            ProductId = consumption.ProductId,
                            ProductTypeId = product.ProductTypeId,
                            AmountForecasted = consumption.AmountForecasted,
                            TotalValue = consumption.AmountForecasted,
                            Period = consumption.Period,
                            DurationDateTime = Convert.ToDateTime(consumption.Period),
                            PackSize = product.PackSize,
                            PackQty = int.Parse(decimal.Round(product.PackSize == 0 ? 0 : (consumption.AmountForecasted / product.PackSize), 0).ToString()),
                            PackPrice = product.ManufacturerPrice  //Need to update with Real Price with Price Level
                        };
                        ForecastResults.Add(ForecastResult);
                    }
                }
            }

            // Set results
            ForecastResults = ForecastResults.GroupBy(g => new { g.LaboratoryId, g.ProductTypeId, g.ProductId, g.Period }).Select(s => new Data.DbModels.ForecastingSchema.ForecastResult
            {
                CreatedBy = forecastInfo.CreatedBy,
                CreatedOn = DateTime.Now,
                ForecastInfoId = forecastInfo.Id,
                LaboratoryId = s.Key.LaboratoryId,
                TestId = s.Max(x => x.TestId) == 0 ? null : s.Max(x => x.TestId),
                ProductId = s.Key.ProductId,
                ProductTypeId = s.Key.ProductTypeId,
                AmountForecasted = s.Sum(x => x.AmountForecasted),
                TotalValue = s.Sum(x => x.TotalValue),
                Period = s.Max(x => x.Period),
                DurationDateTime = s.Max(x => x.DurationDateTime),
                PackSize = s.Max(x => x.PackSize),
                PackQty = s.Sum(x => x.PackQty),
                PackPrice = s.Max(x => x.PackPrice),
                IsActive = true,
                TotalPrice = s.Sum(x => x.PackQty) * s.Max(x => x.PackPrice) * (1 + forecastInfo.WastageRate)
            }).ToList();

            #endregion

            return ForecastResults;
        }
    }
}
