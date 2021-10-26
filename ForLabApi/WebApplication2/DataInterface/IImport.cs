using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ForLabApi.DataInterface
{
    public interface IImport<receivereportdata,Reportobject, ForecastSiteInfonew, Matchrule>
    {
        string importexcel(IFormFile file,int userid,int countryid,string sheets,List<Matchrule> matchrules);
        Array getimporteddata(int id);
        DataTable importconsumption(List<string[]> jArray, int id,int userid);
        Array getimportedservicedata(int id);
        DataTable importservice(List<string[]> jArray, int id,int userid);
        string Importconsumptionnew(List<string[]> jArray, int id, int userid);
        string importservicenew(List<string[]> jArray, int id, int userid);
        string saveimportservice(Reportobject _rdata);
        string saveimportconsumption(List<receivereportdata> _rdata);
         void importdatafromwho();
        IEnumerable<ForecastSiteInfonew> Importpatient(IFormFile file,int id,int userid,int countryid);


    }
}
