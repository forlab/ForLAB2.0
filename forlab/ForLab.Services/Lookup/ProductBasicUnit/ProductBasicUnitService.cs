using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Lookup.ProductBasicUnit;
using ForLab.Repositories.Lookup.ProductBasicUnit;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForLab.Services.Lookup.ProductBasicUnit
{
    public class ProductBasicUnitService : GService<ProductBasicUnitDto, Data.DbModels.LookupSchema.ProductBasicUnit, IProductBasicUnitRepository>, IProductBasicUnitService
    {
        private readonly IProductBasicUnitRepository _productBasicUnitRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        public ProductBasicUnitService(IMapper mapper,
            IResponseDTO response,
            IProductBasicUnitRepository productBasicUnitRepository,
            IUnitOfWork<AppDbContext> unitOfWork) : base(productBasicUnitRepository, mapper)
        {
            _productBasicUnitRepository = productBasicUnitRepository;
            _response = response;
            _unitOfWork = unitOfWork;
        }


        public IResponseDTO GetProductBasicUnits()
        {
            IQueryable<Data.DbModels.LookupSchema.ProductBasicUnit> query = null;
            try
            {
                query = _productBasicUnitRepository.GetAll(x => !x.IsDeleted);

                var dataList = _mapper.Map<List<ProductBasicUnitDto>>(query.ToList());

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
