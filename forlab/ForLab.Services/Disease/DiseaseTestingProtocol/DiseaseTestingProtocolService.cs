using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Disease.DiseaseTestingProtocol;
using ForLab.Repositories.Disease.DiseaseTestingProtocol;
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

namespace ForLab.Services.Disease.DiseaseTestingProtocol
{
    public class DiseaseTestingProtocolService : GService<DiseaseTestingProtocolDto, Data.DbModels.DiseaseSchema.DiseaseTestingProtocol, IDiseaseTestingProtocolRepository>, IDiseaseTestingProtocolService
    {
        private readonly IDiseaseTestingProtocolRepository _diseaseTestingProtocolRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportDiseaseTestingProtocolDto> _fileService;

        public DiseaseTestingProtocolService(IMapper mapper,
            IResponseDTO response,
            IDiseaseTestingProtocolRepository diseaseTestingProtocolRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportDiseaseTestingProtocolDto> fileService) : base(diseaseTestingProtocolRepository, mapper)
        {
            _diseaseTestingProtocolRepository = diseaseTestingProtocolRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, DiseaseTestingProtocolFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseSchema.DiseaseTestingProtocol> query = null;
            try
            {
                query = _diseaseTestingProtocolRepository.GetAll()
                                    .Include(x => x.TestingProtocol)
                                    .Include(x => x.Disease)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.TestingProtocolId > 0)
                    {
                        query = query.Where(x => x.TestingProtocolId == filterDto.TestingProtocolId);
                    }
                    if (filterDto.DiseaseId > 0)
                    {
                        query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "DiseaseName".ToLower())
                    {
                        filterDto.SortProperty = "DiseaseId";
                    }
                    if (filterDto.SortProperty.ToLower() == "TestingProtocolName".ToLower())
                    {
                        filterDto.SortProperty = "TestingProtocolId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<DiseaseTestingProtocolDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(DiseaseTestingProtocolFilterDto filterDto = null)
        {
            try
            {
                var query = _diseaseTestingProtocolRepository.GetAll(x => !x.IsDeleted && x.IsActive);


                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.TestingProtocolId > 0)
                    {
                        query = query.Where(x => x.TestingProtocolId == filterDto.TestingProtocolId);
                    }
                    if (filterDto.DiseaseId > 0)
                    {
                        query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                    }
                }

                query = query.OrderBy(x => x.Id);
                query = query.Select(i => new Data.DbModels.DiseaseSchema.DiseaseTestingProtocol()
                {
                    Id = i.Id,
                    Disease = i.Disease,
                    TestingProtocol = i.TestingProtocol
                });
                var dataList = _mapper.Map<List<DiseaseTestingProtocolDrp>>(query.ToList());

                _response.Data = dataList;
                _response.IsPassed = true;
                _response.Message = "Done";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> GetDiseaseTestingProtocolDetails(int diseaseTestingProtocolId)
        {
            try
            {
                var diseaseTestingProtocol = await _diseaseTestingProtocolRepository.GetAll()
                                        .Include(x => x.DiseaseId)
                                        .Include(x => x.TestingProtocolId)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == diseaseTestingProtocolId);
                if (diseaseTestingProtocol == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var diseaseTestingProtocolDto = _mapper.Map<DiseaseTestingProtocolDto>(diseaseTestingProtocol);

                _response.Data = diseaseTestingProtocolDto;
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
        public async Task<IResponseDTO> CreateDiseaseTestingProtocol(DiseaseTestingProtocolDto diseaseTestingProtocolDto)
        {
            try
            {
                var diseaseTestingProtocol = _mapper.Map<Data.DbModels.DiseaseSchema.DiseaseTestingProtocol>(diseaseTestingProtocolDto);

                // Set relation variables with null to avoid unexpected EF errors
                diseaseTestingProtocol.Disease = null;
                diseaseTestingProtocol.TestingProtocol = null;

                // Add to the DB
                await _diseaseTestingProtocolRepository.AddAsync(diseaseTestingProtocol);

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
        public async Task<IResponseDTO> UpdateDiseaseTestingProtocol(DiseaseTestingProtocolDto diseaseTestingProtocolDto)
        {
            try
            {
                var diseaseTestingProtocolExist = await _diseaseTestingProtocolRepository.GetFirstAsync(x => x.Id == diseaseTestingProtocolDto.Id);
                if (diseaseTestingProtocolExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var diseaseTestingProtocol = _mapper.Map<Data.DbModels.DiseaseSchema.DiseaseTestingProtocol>(diseaseTestingProtocolDto);

                // Set relation variables with null to avoid unexpected EF errors

                diseaseTestingProtocol.Disease = null;
                diseaseTestingProtocol.TestingProtocol = null;


                _diseaseTestingProtocolRepository.Update(diseaseTestingProtocol);

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
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int diseaseTestingProtocolId, bool IsActive)
        {
            try
            {
                var diseaseTestingProtocol = await _diseaseTestingProtocolRepository.GetFirstAsync(x => x.Id == diseaseTestingProtocolId);
                if (diseaseTestingProtocol == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                diseaseTestingProtocol.IsActive = IsActive;
                diseaseTestingProtocol.UpdatedBy = loggedInUserId;
                diseaseTestingProtocol.UpdatedOn = DateTime.Now;

                // Update on the Database
                _diseaseTestingProtocolRepository.Update(diseaseTestingProtocol);

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
        public async Task<IResponseDTO> RemoveDiseaseTestingProtocol(int diseaseTestingProtocolId, int loggedInUserId)
        {
            try
            {
                var diseaseTestingProtocol = await _diseaseTestingProtocolRepository.GetFirstOrDefaultAsync(x => x.Id == diseaseTestingProtocolId);
                if (diseaseTestingProtocol == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                diseaseTestingProtocol.IsDeleted = true;
                diseaseTestingProtocol.UpdatedBy = loggedInUserId;
                diseaseTestingProtocol.UpdatedOn = DateTime.Now;

                // Update on the Database
                _diseaseTestingProtocolRepository.Update(diseaseTestingProtocol);

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
        public GeneratedFile ExportDiseaseTestingProtocols(int? pageIndex = null, int? pageSize = null, DiseaseTestingProtocolFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseSchema.DiseaseTestingProtocol> query = null;
            try
            {
                query = _diseaseTestingProtocolRepository.GetAll()
                                    .Include(x => x.Disease)
                                    .Include(x => x.TestingProtocol)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.TestingProtocolId > 0)
                    {
                        query = query.Where(x => x.TestingProtocolId == filterDto.TestingProtocolId);
                    }
                    if (filterDto.DiseaseId > 0)
                    {
                        query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                    }
                }

                query = query.OrderByDescending(x => x.Id);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "DiseaseName".ToLower())
                    {
                        filterDto.SortProperty = "DiseaseId";
                    }
                    if (filterDto.SortProperty.ToLower() == "TestingProtocolName".ToLower())
                    {
                        filterDto.SortProperty = "TestingProtocolId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportDiseaseTestingProtocolDto>>(query.ToList());

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
        public async Task<IResponseDTO> ImportDiseaseTestingProtocols(List<DiseaseTestingProtocolDto> diseaseTestingProtocolDtos)
        {
            try
            {
                // Map DTO to data object
                var diseaseTestingProtocols = _mapper.Map<List<Data.DbModels.DiseaseSchema.DiseaseTestingProtocol>>(diseaseTestingProtocolDtos);

                // Add the new object to the database
                if (diseaseTestingProtocols.Count() > 0)
                { 
                    // Set relation variables with null to avoid unexpected EF errors
                    diseaseTestingProtocols.Select(x => 
                    {
                        x.Disease = null;
                        x.TestingProtocol = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    });
                    await _diseaseTestingProtocolRepository.AddRangeAsync(diseaseTestingProtocols);
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = diseaseTestingProtocols.Count();

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
                    NumberOfUploded = diseaseTestingProtocolDtos.Count,
                    NumberOfAdded = numberOfAdded
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
    }
}
