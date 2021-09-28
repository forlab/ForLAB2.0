using System;

namespace ForLab.DTO.Testing.TestingProtocol
{
    public class ExportTestingProtocolDto
    {
        public string Name { get; set; }
        public string TestName { get; set; }
        public string PatientGroupName { get; set; }
        public string CalculationPeriodName { get; set; }
        public int BaseLine { get; set; }
        public int TestAfterFirstYear { get; set; }

        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
