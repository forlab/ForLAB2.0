using ForLab.DTO.Common;

namespace ForLab.DTO.Disease.DiseaseTestingProtocol
{
    public class DiseaseTestingProtocolFilterDto : BaseFilterDto
    {
        public int DiseaseId { get; set; }
        public int TestingProtocolId { get; set; }
    }
}
