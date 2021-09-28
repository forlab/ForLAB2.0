using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ProductSchema;
using ForLab.Data.DbModels.TestingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("TestingAreas", Schema = "Lookup")]
    public class TestingArea: DynamicLookup
    {
        public TestingArea()
        {
            Tests = new HashSet<Test>();
            Instruments = new HashSet<Instrument>();
        }
        public bool Shared { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
        public virtual ICollection<Instrument> Instruments { get; set; }
    }
}
