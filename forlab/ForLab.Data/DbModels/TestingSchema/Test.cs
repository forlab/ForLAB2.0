using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.LaboratorySchema;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.ProductSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.TestingSchema
{
    [Table("Tests", Schema = "Testing")]
    public class Test : BaseEntity
    {
        public Test()
        {
            ProductUsages = new HashSet<ProductUsage>();
            TestingProtocols = new HashSet<TestingProtocol>();
            LaboratoryTestServices = new HashSet<LaboratoryTestService>();
            ForecastTests = new HashSet<ForecastTest>();
            ForecastMorbidityTestingProtocolMonths = new HashSet<ForecastMorbidityTestingProtocolMonth>();
            ForecastLaboratoryTestServices = new HashSet<ForecastLaboratoryTestService>();
            ForecastResults = new HashSet<ForecastResult>();
        }
        public int TestingAreaId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool Shared { get; set; }

        public virtual TestingArea TestingArea { get; set; }
        public virtual ICollection<ProductUsage> ProductUsages { get; set; }
        public virtual ICollection<TestingProtocol> TestingProtocols { get; set; }
        public virtual ICollection<LaboratoryTestService> LaboratoryTestServices { get; set; }
        public virtual ICollection<ForecastTest> ForecastTests { get; set; }
        public virtual ICollection<ForecastMorbidityTestingProtocolMonth> ForecastMorbidityTestingProtocolMonths { get; set; }
        public virtual ICollection<ForecastLaboratoryTestService> ForecastLaboratoryTestServices { get; set; }
        public virtual ICollection<ForecastResult> ForecastResults { get; set; }
    }
}
