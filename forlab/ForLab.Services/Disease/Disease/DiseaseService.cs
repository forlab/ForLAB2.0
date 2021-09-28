using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Disease.Disease;
using ForLab.Repositories.Disease.Disease;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ForLab.Core.Common;
using ForLab.Services.Global.FileService;

namespace ForLab.Services.Disease.Disease
{
    public class DiseaseService : GService<DiseaseDto, Data.DbModels.DiseaseSchema.Disease, IDiseaseRepository>, IDiseaseService
    {
        private readonly IDiseaseRepository _diseaseRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportDiseaseDto> _fileService;

        public DiseaseService(IMapper mapper,
            IResponseDTO response,
            IDiseaseRepository diseaseRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportDiseaseDto> fileService) : base(diseaseRepository, mapper)
        {
            _diseaseRepository = diseaseRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, DiseaseFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseSchema.Disease> query = null;
            try
            {
                query = _diseaseRepository.GetAll()
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<DiseaseDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(DiseaseFilterDto filterDto = null)
        {
            var query = _diseaseRepository.GetAll(x => !x.IsDeleted && x.IsActive);


            if (filterDto != null)
            {
                if (filterDto.IsActive != null)
                {
                    query = query.Where(x => x.IsActive == filterDto.IsActive);
                }
                if (!string.IsNullOrEmpty(filterDto.Name))
                {
                    query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                }
            }

            query = query.OrderBy(x => x.Id);
            query = query.Select(i => new Data.DbModels.DiseaseSchema.Disease()
            {
                Id = i.Id,
                Name = i.Name,
            });
            var dataList = _mapper.Map<List<DiseaseDrp>>(query.ToList());

            _response.Data = dataList;
            _response.IsPassed = true;
            _response.Message = "Done";
            return _response;
        }
        public async Task<IResponseDTO> GetDiseaseDetails(int diseaseId)
        {
            try
            {
                var disease = await _diseaseRepository.GetAll()
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == diseaseId);
                if (disease == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var diseaseDto = _mapper.Map<DiseaseDto>(disease);

                _response.Data = diseaseDto;
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
        public GeneratedFile ExportDiseases(int? pageIndex = null, int? pageSize = null, DiseaseFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseSchema.Disease> query = null;
            try
            {
                query = _diseaseRepository.GetAll()
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Description))
                    {
                        query = query.Where(x => x.Description.Trim().ToLower().Contains(filterDto.Description.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }
                query = query.OrderByDescending(x => x.Id);

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

                var dataList = _mapper.Map<List<ExportDiseaseDto>>(query.ToList());

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
        public async Task<IResponseDTO> CreateDisease(DiseaseDto diseaseDto)
        {
            try
            {
                var disease = _mapper.Map<Data.DbModels.DiseaseSchema.Disease>(diseaseDto);

                // Set relation variables with null to avoid unexpected EF errors
                disease.CountryDiseaseIncidents = null;
                disease.DiseaseTestingProtocols = null;
                disease.Programs = null;
                disease.ForecastMorbidityWhoBases = null;

                // Add to the DB
                await _diseaseRepository.AddAsync(disease);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

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
        public async Task<IResponseDTO> UpdateDisease(DiseaseDto diseaseDto)
        {
            try
            {
                var diseaseExist = await _diseaseRepository.GetFirstAsync(x => x.Id == diseaseDto.Id);
                if (diseaseExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var disease = _mapper.Map<Data.DbModels.DiseaseSchema.Disease>(diseaseDto);

                // Set relation variables with null to avoid unexpected EF errors
                disease.CountryDiseaseIncidents = null;
                disease.DiseaseTestingProtocols = null;
                disease.Programs = null;
                disease.ForecastMorbidityWhoBases = null;

                // Update on the database
                _diseaseRepository.Update(disease);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

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
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int diseaseId, bool IsActive)
        {
            try
            {
                var disease = await _diseaseRepository.GetFirstAsync(x => x.Id == diseaseId);
                if (disease == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                disease.IsActive = IsActive;
                disease.UpdatedBy = loggedInUserId;
                disease.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                disease.CountryDiseaseIncidents = null;
                disease.DiseaseTestingProtocols = null;
                disease.Programs = null;
                disease.ForecastMorbidityWhoBases = null;

                // Update on the Database
                _diseaseRepository.Update(disease);

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
        public async Task<IResponseDTO> UpdateIsActiveForSelected(int loggedInUserId, List<int> ids, bool isActive)
        {
            try
            {
                var items = _diseaseRepository.GetAll(x => ids.Contains(x.Id)).ToList();

                // Update IsActive value
                var objectsToUpdate = items?.Where(x => x.IsActive != isActive).Select(x =>
                {
                    x.IsActive = isActive;
                    x.UpdatedBy = loggedInUserId;
                    x.UpdatedOn = DateTime.Now;
                    return x;
                }).ToList();


                // Update on the Database
                foreach (var item in objectsToUpdate)
                {
                    _diseaseRepository.Update(item);
                }

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "There is no changes to save";
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
        public async Task<IResponseDTO> RemoveDisease(int diseaseId, int loggedInUserId)
        {
            try
            {
                var disease = await _diseaseRepository.GetFirstOrDefaultAsync(x => x.Id == diseaseId);
                if (disease == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                disease.IsDeleted = true;
                disease.UpdatedBy = loggedInUserId;
                disease.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                disease.CountryDiseaseIncidents = null;
                disease.DiseaseTestingProtocols = null;
                disease.Programs = null;
                disease.ForecastMorbidityWhoBases = null;

                // Update on the Database
                _diseaseRepository.Update(disease);

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
        public async Task<IResponseDTO> ImportDiseases(List<DiseaseDto> diseaseDtos)
        {
            try
            {
                // Get all not deleted from the database
                var diseases_database = _diseaseRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var diseases = _mapper.Map<List<Data.DbModels.DiseaseSchema.Disease>>(diseaseDtos);

                // vars
                var names_database = diseases_database.Select(x => x.Name.ToLower().Trim());
                var names_dto = diseases.Select(x => x.Name.ToLower().Trim());
                // Get the new ones that their names don't exist on the database
                var newDiseases = diseases.Where(x => !names_database.Contains(x.Name.ToLower().Trim()));
                // Select the objects that their names already exist in the database
                var updatedDiseases = diseases_database.Where(x => names_dto.Contains(x.Name.ToLower().Trim()));

                // Add the new object to the database
                if (newDiseases.Count() > 0)
                {
                    newDiseases.Select(x =>
                    {
                        x.CountryDiseaseIncidents = null;
                        x.DiseaseTestingProtocols = null;
                        x.Programs = null;
                        x.ForecastMorbidityWhoBases = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _diseaseRepository.AddRangeAsync(newDiseases);
                }

                // Update the existing objects with the new values
                if (updatedDiseases.Count() > 0)
                {
                    foreach (var item in updatedDiseases)
                    {
                        var dto = diseaseDtos.First(x => x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                        item.UpdatedOn = DateTime.Now;
                        item.UpdatedBy = dto.CreatedBy;
                        item.Name = dto.Name;
                        item.Description = dto.Description;
                        // Set relation variables with null to avoid unexpected EF errors
                        item.CountryDiseaseIncidents = null;
                        item.DiseaseTestingProtocols = null;
                        item.Programs = null;
                        item.ForecastMorbidityWhoBases = null;
                        item.Creator = null;
                        item.Updator = null;
                        _diseaseRepository.Update(item);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newDiseases.Count();
                var numberOfUpdated = updatedDiseases.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.Data = new
                {
                    NumberOfUploded = diseaseDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated
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
        // Validators methods
        public bool IsNameUnique(DiseaseDto diseaseDto)
        {
            var searchResult = _diseaseRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != diseaseDto.Id
                                                && x.Name.ToLower().Trim() == diseaseDto.Name.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int diseaseId)
        {
            try
            {
                var disease = await _diseaseRepository.GetAll()
                                        .Include(x => x.CountryDiseaseIncidents)
                                        .Include(x => x.DiseaseTestingProtocols)
                                        .Include(x => x.ForecastMorbidityWhoBases)
                                        .Include(x => x.Programs)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == diseaseId);
                if (disease == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (disease.CountryDiseaseIncidents.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Disease cannot be deleted or deactivated where it contains 'Country Disease Incidents'";
                    return _response;
                }
                if (disease.DiseaseTestingProtocols.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Disease cannot be deleted or deactivated where it contains 'Disease Testing Protocols'";
                    return _response;
                }
                if (disease.ForecastMorbidityWhoBases.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Disease cannot be deleted or deactivated where it contains 'Forecast Morbidity Who Bases'";
                    return _response;
                }
                if (disease.Programs.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Disease cannot be deleted or deactivated where it contains 'Programs'";
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
    }
}
