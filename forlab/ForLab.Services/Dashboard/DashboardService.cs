using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DbModels.SecuritySchema;
using ForLab.Data.Enums;
using ForLab.DTO.Dashboard;
using ForLab.Repositories.CMS.InquiryQuestion;
using ForLab.Repositories.Disease.CountryDiseaseIncident;
using ForLab.Repositories.Forecasting.ForecastInfo;
using ForLab.Repositories.Forecasting.ForecastLaboratory;
using ForLab.Repositories.Lookup.Laboratory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IInquiryQuestionRepository _inquiryQuestionRepository;
        private readonly ILaboratoryRepository _laboratoryRepository;
        private readonly IForecastInfoRepository _forecastInfoRepository;
        private readonly ICountryDiseaseIncidentRepository _countryDiseaseIncidentRepository;
        private readonly IForecastLaboratoryRepository _forecastLaboratoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IResponseDTO _response;
        private readonly IMapper _mapper;

        public DashboardService(IMapper mapper,
            IResponseDTO response,
            IInquiryQuestionRepository inquiryQuestionRepository,
            ILaboratoryRepository laboratoryRepository,
            UserManager<ApplicationUser> userManager,
            IForecastInfoRepository forecastInfoRepository,
            ICountryDiseaseIncidentRepository countryDiseaseIncidentRepository,
            IForecastLaboratoryRepository forecastLaboratoryRepository)
        {
            _inquiryQuestionRepository = inquiryQuestionRepository;
            _laboratoryRepository = laboratoryRepository;
            _forecastInfoRepository = forecastInfoRepository;
            _countryDiseaseIncidentRepository = countryDiseaseIncidentRepository;
            _forecastLaboratoryRepository = forecastLaboratoryRepository;
            _userManager = userManager;
            _response = response;
            _mapper = mapper;
        }


        #region Main Dashboard
        public IResponseDTO MainCardCounts()
        {
            try
            {
                var laboratories = _laboratoryRepository.GetAll(x => !x.IsDeleted).ToList();
                var inquiryQuestions = _inquiryQuestionRepository.GetAll(x => !x.IsDeleted).ToList();
                var users = _userManager.Users;

                _response.Data = new
                {
                    ActiveLaboratories = laboratories.Where(x => x.IsActive).Count(),
                    LaboratoriesAddedToday = laboratories.Where(x => x.CreatedOn.Date == DateTime.Now.Date).Count(),
                    InquiryQuestionsAddedToday = inquiryQuestions.Where(x => x.CreatedOn.Date == DateTime.Now.Date).Count(),
                    NotAnsweredInquiryQuestions = inquiryQuestions.Where(x => !x.ReplyProvided).Count(),
                    ActiveUsers = users.Where(x => x.Status == UserStatusEnum.Active.ToString()).Count(),
                    UsersAddedToday = users.Where(x => x.CreatedOn.Date == DateTime.Now.Date).Count(),
                    UnactiveUsers = users.Where(x => x.Status != UserStatusEnum.Active.ToString()).Count(),
                    UsersDeactivatedToday = users.Where(x => x.Status != UserStatusEnum.Active.ToString() && x.UpdatedOn.Value.Date == DateTime.Now.Date).Count(),
                };
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
        public IResponseDTO NumberOfLaboratories(int? countryId = null, int? regionId = null)
        {
            try
            {
                var laboratories = _laboratoryRepository.GetAll()
                                            .Include(x => x.Region)
                                            .Where(x => !x.IsDeleted)
                                            .ToList();

                if(countryId != null)
                {
                    laboratories = laboratories.Where(x => x.Region.CountryId == countryId).ToList();
                }
                if (regionId != null)
                {
                    laboratories = laboratories.Where(x => x.RegionId == regionId).ToList();
                }

                _response.Data = laboratories.Count();
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
        public IResponseDTO NumberOfDiseases(int? countryId = null)
        {
            try
            {
                var countryDiseases = _countryDiseaseIncidentRepository.GetAll(x => !x.IsDeleted).ToList();

                if (countryId != null)
                {
                    countryDiseases = countryDiseases.Where(x => x.CountryId == countryId).ToList();
                }

                var diseasesCount = countryDiseases.Select(x => x.DiseaseId).Distinct().Count();

                _response.Data = diseasesCount;
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
        public IResponseDTO InquiryQuestionsChart(int numOfMonths)
        {
            try
            {
                // Controls
                numOfMonths = numOfMonths == 0 ? 8 : numOfMonths;

                // Data
                var lables = new List<string>();
                var answeredData = new List<int>();
                var notAnsweredData = new List<int>();

                // Query
                var validDates = GetLastXDates(numOfMonths);
                var inquiryQuestions = _inquiryQuestionRepository.GetAll(x => !x.IsDeleted).ToList();

                // Set Result
                lables = validDates.ConvertAll(x => x.Date.ToString("MMM-yy"));
                foreach(var item in validDates)
                {
                    var answeredCount = inquiryQuestions.Where(x => x.ReplyProvided && x.CreatedOn.Year == item.Year && x.CreatedOn.Month == item.Month).Count();
                    answeredData.Add(answeredCount);

                    var notAnsweredCount = inquiryQuestions.Where(x => !x.ReplyProvided && x.CreatedOn.Year == item.Year && x.CreatedOn.Month == item.Month).Count();
                    notAnsweredData.Add(notAnsweredCount);
                }

                _response.Data = new 
                {
                    Lables = lables,
                    AnsweredData = answeredData,
                    NotAnsweredData = notAnsweredData
                };
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
        public IResponseDTO UsersChart(int numOfMonths)
        {
            try
            {
                // Controls
                numOfMonths = numOfMonths == 0 ? 8 : numOfMonths;

                // Data
                var lables = new List<string>();
                var usersData = new List<int>();

                // Query
                var validDates = GetLastXDates(numOfMonths);
                var users = _userManager.Users;

                // Set Result
                lables = validDates.ConvertAll(x => x.Date.ToString("MMM-yy"));
                foreach (var item in validDates)
                {
                    var usersCount = users.Where(x => x.CreatedOn.Year == item.Year && x.CreatedOn.Month == item.Month).Count();
                    usersData.Add(usersCount);
                }

                _response.Data = new
                {
                    Lables = lables,
                    UsersData = usersData
                };
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
        public IResponseDTO LaboratoriesChart(int numOfMonths, int? countryId = null, int? regionId = null)
        {
            try
            {
                // Controls
                numOfMonths = numOfMonths == 0 ? 8 : numOfMonths;

                // Data
                var lables = new List<string>();
                var laboratoriesData = new List<int>();

                // Query
                var validDates = GetLastXDates(numOfMonths);
                var laboratories = _laboratoryRepository.GetAll(x => !x.IsDeleted).ToList();
                if (countryId != null)
                {
                    laboratories = laboratories.Where(x => x.Region.CountryId == countryId).ToList();
                }
                if (regionId != null)
                {
                    laboratories = laboratories.Where(x => x.RegionId == regionId).ToList();
                }

                // Set Result
                lables = validDates.ConvertAll(x => x.Date.ToString("MMM-yy"));
                foreach (var item in validDates)
                {
                    var laboratoriesCount = laboratories.Where(x => x.CreatedOn.Year == item.Year && x.CreatedOn.Month == item.Month).Count();
                    laboratoriesData.Add(laboratoriesCount);
                }

                _response.Data = new
                {
                    Lables = lables,
                    LaboratoriesData = laboratoriesData
                };
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
        #endregion


        #region Forecast Dashboard
        public async Task<IResponseDTO> ForecastCardCounts()
        {
            try
            {
                var forecasts = await _forecastInfoRepository.GetAllAsync(x => !x.IsDeleted);

                // Counts
                var service = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.Service).Count();
                var serviceAddedToday = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.Service 
                                                    && x.CreatedOn.Date == DateTime.Now.Date).Count();
                var consumption = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.Consumption).Count();
                var consumptionAddedToday = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.Consumption 
                                                    && x.CreatedOn.Date == DateTime.Now.Date).Count();
                var targetBase = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && x.IsTargetBased).Count();
                var targetBaseAddedToday = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity 
                                                    && x.CreatedOn.Date == DateTime.Now.Date
                                                    && x.IsTargetBased).Count();
                var whoBase = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity && x.IsWorldHealthOrganization).Count();
                var whoBaseAddedToday = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity
                                                    && x.CreatedOn.Date == DateTime.Now.Date
                                                    && x.IsWorldHealthOrganization).Count();

                _response.Data = new
                {
                    Service = service,
                    ServiceAddedToday = serviceAddedToday,
                    Consumption = consumption,
                    ConsumptionAddedToday = consumptionAddedToday,
                    TargetBase = targetBase,
                    TargetBaseAddedToday = targetBaseAddedToday,
                    WhoBase = whoBase,
                    WhoBaseAddedToday = whoBaseAddedToday,
                };
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
        public IResponseDTO ForecastsChart(int numOfMonths, int? countryPeriodId = null)
        {
            try
            {
                // Controls
                numOfMonths = numOfMonths == 0 ? 8 : numOfMonths;

                // Data
                var lables = new List<string>();
                var serviceData = new List<int>();
                var consumptionData = new List<int>();
                var targetBaseData = new List<int>();
                var whoBaseData = new List<int>();


                // Query
                var validDates = GetLastXDates(numOfMonths);
                var forecasts = _forecastInfoRepository.GetAll().Include(x => x.Country).Where(x => !x.IsDeleted).ToList();
                if(countryPeriodId != null)
                {
                    forecasts = forecasts.Where(x => x.Country.CountryPeriodId == countryPeriodId).ToList();
                }

                // Set Result
                lables = validDates.ConvertAll(x => x.Date.ToString("MMM-yy"));
                foreach (var item in validDates)
                {
                    var serviceCount = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.Service 
                                                && x.CreatedOn.Year == item.Year 
                                                && x.CreatedOn.Month == item.Month).Count();
                                                serviceData.Add(serviceCount);

                    var consumptionCount = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.Consumption 
                                                && x.CreatedOn.Year == item.Year 
                                                && x.CreatedOn.Month == item.Month).Count();
                                                consumptionData.Add(consumptionCount);

                    var targetBaseCount = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity
                                                && x.IsTargetBased
                                                && x.CreatedOn.Year == item.Year 
                                                && x.CreatedOn.Month == item.Month).Count();
                                                targetBaseData.Add(targetBaseCount);

                    var whoBaseCount = forecasts.Where(x => x.ForecastMethodologyId == (int)ForecastMethodologyEnum.DempgraphicMorbidity 
                                                && x.IsWorldHealthOrganization
                                                && x.CreatedOn.Year == item.Year 
                                                && x.CreatedOn.Month == item.Month).Count();
                                                whoBaseData.Add(whoBaseCount);
                }

                _response.Data = new
                {
                    Lables = lables,
                    ServiceData = serviceData,
                    ConsumptionData = consumptionData,
                    TargetBaseData = targetBaseData,
                    WhoBaseData = whoBaseData
                };
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
        public IResponseDTO NumberOfForecasts(DashboardNumberOfForecastsFilterDto filterDto)
        {
            try
            {
                var dataList = new List<DashboardNumberOfForecastsDto>();

                var query = _forecastLaboratoryRepository.GetAll()
                                     .Include(x => x.ForecastInfo)
                                     .Include(x => x.Laboratory).ThenInclude(x => x.Region)
                                     .Where(x => !x.IsDeleted && !x.ForecastInfo.IsDeleted  && x.ForecastInfo.IsActive)
                                     .ToList();

                if (filterDto != null)
                {
                    if (filterDto.CountryId == null && filterDto.RegionId == null)
                    {
                        var grouped = query.GroupBy(x => x.Laboratory.Region.Name);
                        foreach(var item in grouped)
                        {
                            dataList.Add(new DashboardNumberOfForecastsDto
                            {
                                Name = item.Key,
                                NumberOfForecasts = item.Select(x => x.ForecastInfoId).Distinct().Count()
                            });
                        }
                    }
                    else if (filterDto.CountryId != null && filterDto.RegionId == null)
                    {
                        var grouped = query.Where(x => x.Laboratory.Region.CountryId == filterDto.CountryId.Value).GroupBy(x => x.Laboratory.Region.Name);
                        foreach (var item in grouped)
                        {
                            dataList.Add(new DashboardNumberOfForecastsDto
                            {
                                Name = item.Key,
                                NumberOfForecasts = item.Select(x => x.ForecastInfoId).Distinct().Count()
                            });
                        }
                    }
                    else if (filterDto.CountryId != null && filterDto.RegionId != null)
                    {
                        var grouped = query.Where(x => x.Laboratory.RegionId == filterDto.RegionId.Value).GroupBy(x => x.Laboratory.Name);
                        foreach (var item in grouped)
                        {
                            dataList.Add(new DashboardNumberOfForecastsDto
                            {
                                Name = item.Key,
                                NumberOfForecasts = item.Select(x => x.ForecastInfoId).Distinct().Count()
                            });
                        }
                    }
                }

                var total = dataList.Count();

                // Filter
                if (!string.IsNullOrEmpty(filterDto.Name))
                {
                    dataList = dataList.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower())).ToList();
                }

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    dataList = dataList.AsQueryable().OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC")).ToList();
                }

                //Apply Pagination
                if (filterDto.PageIndex.HasValue && filterDto.PageSize.HasValue)
                {
                    dataList = dataList.Skip((filterDto.PageIndex.Value - 1) * filterDto.PageSize.Value).Take(filterDto.PageSize.Value).ToList();
                }

                _response.Data = new
                {
                    List = dataList,
                    Page = filterDto.PageIndex ?? 0,
                    pageSize = filterDto.PageSize ?? 0,
                    Total = total,
                    Pages = filterDto.PageSize.HasValue && filterDto.PageSize.Value > 0 ? total / filterDto.PageSize : 1
                };
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
        #endregion


        // helper methods
        private List<DateTime> GetLastXDates(int lastMonth)
        {
            var today = DateTime.Now.Date;
            var result = new List<DateTime>();

            result.Add(today);
            for (var i = 1; i < lastMonth; i++)
            {
                result.Add(today.AddMonths(-1 * i));
            }

            result = result.Select(x => { x = x.Date; return x; }).OrderBy(x => x.Date).ToList();
            result.Reverse();

            return result;
        }
    }
}
