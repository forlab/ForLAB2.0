using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.Product;
using ForLab.Services.Product.Product;
using ForLab.Validators.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;

        public ProductsController(
            IProductService productService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _productService = productService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ProductFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _productService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] ProductFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _productService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetProductDetails(int productId)
        {
            _response = await _productService.GetProductDetails(productId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateProduct([FromBody]ProductDto productDto)
        {
            //Validation
            var validationResult = await (new ProductValidator(_productService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(productDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            productDto.Creator = null;
            productDto.Updator = null;
            productDto.CreatedBy = LoggedInUserId;
            productDto.CreatedOn = DateTime.Now;
            productDto.Shared = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            productDto.VendorName = null;
            productDto.ProductTypeName = null;
            productDto.ProductBasicUnitName = null;

            _response = await _productService.CreateProduct(productDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateProduct([FromBody]ProductDto productDto)
        {
            // Validation
            var validationResult = await (new ProductValidator(_productService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(productDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            productDto.Creator = null;
            productDto.Updator = null;
            productDto.UpdatedBy = LoggedInUserId;
            productDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            productDto.VendorName = null;
            productDto.ProductTypeName = null;
            productDto.ProductBasicUnitName = null;

            _response = await _productService.UpdateProduct(productDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int productId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _productService.IsUsed(productId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _productService.UpdateIsActive(productId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }



        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _productService.UpdateIsActiveForSelected(ids, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateSharedForSelected([FromBody] List<int> ids, bool shared)
        {
            _response = await _productService.UpdateSharedForSelected(ids, shared, LoggedInUserId, IsSuperAdmin);
            return _response;
        }

        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveProduct(int productId)
        {
            // Validation
            var validationResult = await _productService.IsUsed(productId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _productService.RemoveProduct(productId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportProducts([FromBody]List<ProductDto> productDtos)
        {
            // Validation
            for (var i = 0; i < productDtos.Count; i++)
            {
                var validationResult = await (new ImportProductValidator()).ValidateAsync(productDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication
            var isDuplicated = productDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated)
            {
                productDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = productDtos.GroupBy(x => x.Name.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the name" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the name";
                return _response;
            }
            var isDuplicated2 = productDtos.GroupBy(x => x.CatalogNo.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated2)
            {
                productDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = productDtos.GroupBy(x => x.CatalogNo.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the CatalogNo" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the CatalogNo";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            productDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.VendorName = null;
                x.ProductTypeName = null;
                x.ProductBasicUnitName = null;
                return x;
            }).ToList();

            _response = await _productService.ImportProducts(productDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportProducts(int? pageIndex = null, int? pageSize = null, [FromQuery] ProductFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _productService.ExportProducts(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


    }
}
