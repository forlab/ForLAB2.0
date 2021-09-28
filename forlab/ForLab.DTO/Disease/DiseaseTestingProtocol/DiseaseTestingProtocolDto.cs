using ForLab.DTO.Common;

namespace ForLab.DTO.Disease.DiseaseTestingProtocol
{
    public class DiseaseTestingProtocolDto : BaseEntityDto
    {
        public int DiseaseId { get; set; }
        public int TestingProtocolId { get; set; }

        //UI
        public string DiseaseName { get; set; }
        public string TestingProtocolName { get; set; }
    }
    public class DiseaseTestingProtocolDrp : DropdownDrp
    {
        public string DiseaseName { get; set; }
        public string TestingProtocolName { get; set; }
    }

}
