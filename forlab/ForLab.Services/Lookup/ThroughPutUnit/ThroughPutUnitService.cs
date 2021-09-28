using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Lookup.ThroughPutUnit;
using ForLab.Repositories.Lookup.ThroughPutUnit;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForLab.Services.Lookup.ThroughPutUnit
{
    public class ThroughPutUnitService : GService<ThroughPutUnitDto, Data.DbModels.LookupSchema.ThroughPutUnit, IThroughPutUnitRepository>, IThroughPutUnitService
    {
        private readonly IThroughPutUnitRepository _throughPutUnitRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        public ThroughPutUnitService(IMapper mapper,
            IResponseDTO response,
            IThroughPutUnitRepository throughPutUnitRepository,
            IUnitOfWork<AppDbContext> unitOfWork) : base(throughPutUnitRepository, mapper)
        {
            _throughPutUnitRepository = throughPutUnitRepository;
            _response = response;
            _unitOfWork = unitOfWork;
        }


        public IResponseDTO GetThroughPutUnits()
        {
            IQueryable<Data.DbModels.LookupSchema.ThroughPutUnit> query = null;
            try
            {
                query = _throughPutUnitRepository.GetAll(x => !x.IsDeleted);

                var dataList = _mapper.Map<List<ThroughPutUnitDto>>(query.ToList());

                _response.Data = dataList;
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
