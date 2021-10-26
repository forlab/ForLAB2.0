using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForLabApi.Models
{
    public class Conductforecast
    {
    }
    public class MLModel
    {
        public int SiteId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int TestId { get; set; }
        public List<MLModelResult> Forecasts { get; set; }
    }
    public class MLModelResult
    {

        public string Datetime { get; set; }
        public decimal Forecast { get; set; }
    }
    public class ConductforecastDasboard
    {
        public string Name { get; set; }
        public decimal[] Data { get; set; }
    }
    public class ConductDashboardchartdata
    {
        public string Name { get; set; }
        public decimal Y { get; set; }
    }
    public class Costclass
    {
        public string Totalcost { get; set; }
        public string Qccost { get; set; }
        public string Cccost { get; set; }


        public string Testcost { get; set; }
    }
<<<<<<< HEAD
=======

>>>>>>> Devops
    public class ProductSite
    {
        public List<int> productIds { get; set; }
        public int siteId { get; set; }
    }

<<<<<<< HEAD
    public class TestSite
    {
        public List<int> testIds { get; set; }
        public int siteId { get; set; }
    }

=======
>>>>>>> Devops
    public class ForecastCaculateModel
    {
        public int forecastPeriod { get; set; }
        public string frequency { get; set; }
        public List<ProductSite> productSite { get; set; }
<<<<<<< HEAD
        public List<TestSite> testSite { get; set; }
=======
>>>>>>> Devops
    }

    public class MLResponseModel
    {
        public int SiteId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int TestId { get; set; }
        public List<MLForecastModel> forecasts { get; set; }
    }
    public class MLForecastModel
    {

        public string datetime { get; set; }
        public decimal forecast { get; set; }
    }
<<<<<<< HEAD
=======

>>>>>>> Devops
}
