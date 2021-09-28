using ForLab.Core.Interfaces;
using ForLab.DTO.Testing.TestingProtocol;
using ForLab.Services.Testing.TestingProtocol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class TestingProtocolsController : BaseController
    {
        private readonly ITestingProtocolService _testingProtocolService;

        public TestingProtocolsController(
            ITestingProtocolService testingProtocolService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _testingProtocolService = testingProtocolService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] TestingProtocolFilterDto filterDto)
        {
            _response = _testingProtocolService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] TestingProtocolFilterDto filterDto)
        {
            _response = _testingProtocolService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetTestingProtocolDetails(int testingProtocolId)
        {
            _response = await _testingProtocolService.GetTestingProtocolDetails(testingProtocolId);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportTestingProtocols(int? pageIndex = null, int? pageSize = null, [FromQuery] TestingProtocolFilterDto filterDto = null)
        {
            var file = _testingProtocolService.ExportTestingProtocols(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
