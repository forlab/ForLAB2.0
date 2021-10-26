using ForLabApi.DataInterface;
using ForLabApi.Models;
using ForLabApi.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
namespace ForLabApi.Repositories
{
    public class ImportAccessRepositories:IImport<receivereportdata,Reportobject, ForecastSiteInfonew, Matchrule>
    {
        private IHostingEnvironment _hostingEnvironment;

        ForecastInfo _forecastInfo = new ForecastInfo();
         ForLabContext ctx;
        DataTable ConsumptionDT = new DataTable();
        DataTable ServiceDT = new DataTable();
        public ImportAccessRepositories(ForLabContext c, IHostingEnvironment hostingEnvironment)
        {
            ctx = c;
            _hostingEnvironment = hostingEnvironment;
            //return ctx;
        }
        public async  void importdatafromwho()
        {
            List<GlobalRegion> GL = new List<GlobalRegion>();

            GlobalRegion GL1 = new GlobalRegion();
            string mainurl = "https://ghoapi.azureedge.net/api/";
            Regioncountryfromwholist lstcon = new Regioncountryfromwholist();
            Populationdatalist lstpop = new Populationdatalist();
            Annualgrowthratelist lstofannual = new Annualgrowthratelist();
            try
            {
                string xmlstring = "";
                HttpWebRequest request = default(HttpWebRequest);
                HttpWebResponse response = null;
                StreamReader reader = default(StreamReader);

                /////get global region 
                string url = mainurl+"DIMENSION/REGION/DimensionValues";
                request = (HttpWebRequest)WebRequest.Create(url);
              
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                xmlstring = reader.ReadToEnd();

                lstcon = JsonConvert.DeserializeObject<Regioncountryfromwholist>(xmlstring);

                for (int i = 0; i < lstcon.value.Count; i++)
                {
                    var isexist = ctx.GlobalRegion.Where(b => b.Shortcode == lstcon.value[i].Code).FirstOrDefault();
                    if (isexist == null)
                    {
                        GL1.Shortcode = lstcon.value[i].Code;
                        GL1.Name = lstcon.value[i].Title;
                        GL.Add(GL1);
                    }
                }

                ctx.GlobalRegion.AddRange(GL);
                ctx.SaveChanges();


                /////////////getcountry
                ///

                List<Country> CL = new List<Country>();
                Country CL1 = new Country();
                var getglobalregion = ctx.GlobalRegion.ToList();
                var getcountrylist = ctx.Country.ToList();
                url = mainurl + "DIMENSION/COUNTRY/DimensionValues";
                request = (HttpWebRequest)WebRequest.Create(url);

                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                xmlstring = reader.ReadToEnd();

                lstcon = JsonConvert.DeserializeObject<Regioncountryfromwholist>(xmlstring);


                ///https://ghoapi.azureedge.net/api/WHS9_86?$filter=SpatialDimType%20eq%20%27Country%27
                url = mainurl + "WHS9_86?$filter=SpatialDimType%20eq%20%27Country%27";
             request = (HttpWebRequest)WebRequest.Create(url);

                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                xmlstring = reader.ReadToEnd();
                lstpop = JsonConvert.DeserializeObject<Populationdatalist>(xmlstring);

                url = mainurl + "WHS9_97";
                request = (HttpWebRequest)WebRequest.Create(url);

                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                xmlstring = reader.ReadToEnd();
                lstofannual = JsonConvert.DeserializeObject<Annualgrowthratelist>(xmlstring);


                 int yy=   DateTime.Now.Year - Convert.ToInt32(lstpop.value[0].TimeDim);
                decimal annualgrowthrate = 0;
                decimal currentpopulation = 0;
                decimal population = 0;
                for (int i = 0; i < lstcon.value.Count; i++)
                {
                    annualgrowthrate = lstofannual.value.Where(b => b.SpatialDim == lstcon.value[i].ParentCode).FirstOrDefault().NumericValue;
                    population = lstpop.value.Where(b => b.SpatialDim == lstcon.value[i].Code).FirstOrDefault().NumericValue;
                    currentpopulation = population + (((population * annualgrowthrate) / 100) * yy);
                    var isexist = getcountrylist.Where(b => b.ShortCode == lstcon.value[i].Code && b.Region==lstcon.value[i].ParentTitle).FirstOrDefault();
                    if (isexist == null)
                    {
                        CL1.ShortCode = lstcon.value[i].Code;
                        CL1.Name = lstcon.value[i].Title;
                        CL1.Region = lstcon.value[i].ParentTitle;
                        CL1.Regionid = getglobalregion.Where(b => b.Shortcode == lstcon.value[i].ParentCode).FirstOrDefault().Id;
                        CL1.Population = currentpopulation*10000;
                         CL.Add(CL1);
                    }
                }


                ctx.Country.AddRange(CL);
               
            }
            catch (Exception x)
            {

            }
            
        }
    
        public string saveimportconsumption_old(Reportobject _rdata)
        {
            string res = "";
            try
            {
                ForecastSite fs = new ForecastSite();
                ForecastCategorySite fcatsite = new ForecastCategorySite();

                bool isduplicate = false;
                ForecastSite efs = new ForecastSite();//existing
                IList<ForecastSiteProduct> existingFsp = new List<ForecastSiteProduct>();
                _forecastInfo = ctx.ForecastInfo.Find(_rdata.receivereportdata[0].forecastid);
                for (int i = 0; i < _rdata.receivereportdata.Count; i++)

                {


                    if (!_rdata.receivereportdata[i].haserror && _forecastInfo.DataUsage != "DATA_USAGE3")
                    {

                        ForecastSiteProduct fp = new ForecastSiteProduct();
                        fs = ctx.ForecastSite.Where(b => b.SiteId == _rdata.receivereportdata[i].SiteID && b.ForecastInfoId==_forecastInfo.ForecastID).FirstOrDefault();

                        if (fs == null)
                        {
                            fs = new ForecastSite();
                            fs.SiteId = _rdata.receivereportdata[i].SiteID;
                            fs.ForecastInfoId = _forecastInfo.ForecastID;
                            ctx.ForecastSite.Add(fs);
                            ctx.SaveChanges();

                        }
                        fp.ForecastSiteID = fs.Id;
                        fp.ProductID = _rdata.receivereportdata[i].ProID;
                        if (!Utility.Utility.IsDateTime(_rdata.receivereportdata[i].Duration))
                        {
                            fp.CDuration = _rdata.receivereportdata[i].Duration;
                            fp.DurationDateTime = Utility.Utility.DurationToDateTime(_rdata.receivereportdata[i].Duration);

                        }
                        else
                        {
                            fp.DurationDateTime = DateTime.Parse(_rdata.receivereportdata[i].Duration);
                            fp.CDuration = Utility.Utility.DatetimeToDurationStr(_forecastInfo.Period, DateTime.Parse(_rdata.receivereportdata[i].Duration));
                        }
                        fp.AmountUsed = _rdata.receivereportdata[i].Amount;
                        fp.StockOut = _rdata.receivereportdata[i].StockOut;
                        fp.InstrumentDowntime = _rdata.receivereportdata[i].InstrumentDownTime;//b
                        if (fp.StockOut > 0)
                        {
                            int days = fp.StockOut;
                            decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

                            if (days >= workingday)
                            {
                                days = 0;
                                fp.StockOut = 0;
                            }
                            if (days >= 0)
                                fp.StockOut = days;
                        }


                        var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

                        if (isexist == null)
                        {
                            ctx.ForecastSiteProduct.Add(fp);
                            ctx.SaveChanges();


                            fp.Adjusted = fp.AmountUsed;
                            //if (fp.AmountUsed == 0)
                            //{
                            //    try
                            //    {
                            //        Consumption cs = GetConsumption(fp.Product.Id, fp);
                            //        fp.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
                            //    }
                            //    catch { fp.Adjusted = fp.AmountUsed; }
                            //}
                            if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, _forecastInfo.Period, ctx.Site.Where(b => b.SiteID == fs.SiteId).Select(b => b.WorkingDays).FirstOrDefault());//b

                            ctx.SaveChanges();
                        }







                    }

                    //end adding

                    //add by category
                    if (!_rdata.receivereportdata[i].haserror && _forecastInfo.DataUsage == "DATA_USAGE3")
                    {
                        ForecastCategory fcat = new ForecastCategory();
                        ForecastCategoryProduct fp = new ForecastCategoryProduct();
                        fcat = ctx.ForecastCategory.Where(b => b.CategoryName == _rdata.receivereportdata[i].CategoryName && b.ForecastId==_forecastInfo.ForecastID ).FirstOrDefault(); ;
                        if (fcat == null)
                        {
                            fcat = new ForecastCategory();
                            fcat.CategoryName = _rdata.receivereportdata[i].CategoryName;
                            fcat.ForecastId = _forecastInfo.ForecastID;
                            ctx.ForecastCategory.Add(fcat);
                            ctx.SaveChanges();

                        }


                        fp = ctx.ForecastCategoryProduct.Where(b => b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == _rdata.receivereportdata[i].Duration1).FirstOrDefault();
                        isduplicate = false;

                        if (fp == null)
                            fp = new ForecastCategoryProduct();
                        else
                            isduplicate = true;

                        fp.CategoryID = fcat.CategoryId;
                        fp.ProductID = _rdata.receivereportdata[i].ProID;

                        fp.AmountUsed = _rdata.receivereportdata[i].Amount;
                        fp.StockOut = _rdata.receivereportdata[i].StockOut;
                        fp.InstrumentDowntime = _rdata.receivereportdata[i].InstrumentDownTime;//b

                        if (!Utility.Utility.IsDateTime(_rdata.receivereportdata[i].Duration))
                        {
                            fp.DurationDateTime = Utility.Utility.DurationToDateTime(_rdata.receivereportdata[i].Duration);
                            fp.CDuration = _rdata.receivereportdata[i].Duration;
                        }
                        else
                        {
                            fp.DurationDateTime = DateTime.Parse(_rdata.receivereportdata[i].Duration);
                            fp.CDuration = Utility.Utility.DatetimeToDurationStr(_forecastInfo.Period, DateTime.Parse(_rdata.receivereportdata[i].Duration));
                        }

                        if (fp.StockOut > 0)
                        {
                            int days = fp.StockOut;
                            decimal workingday = 22;

                            if (days >= workingday)
                            {
                                days = 0;
                                fp.StockOut = 0;
                            }
                            if (days >= 0)
                                fp.StockOut = days;
                        }
                        if (!isduplicate)
                        {
                            ctx.ForecastCategoryProduct.Add(fp);
                            ctx.SaveChanges();

                            fp.Adjusted = fp.AmountUsed;
                            //if (fp.AmountUsed == 0)
                            //{
                            //    try
                            //    {
                            //        Consumption cs = GetConsumption(fp.Product.Id, fp);
                            //        fp.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
                            //    }
                            //    catch { fp.Adjusted = fp.AmountUsed; }
                            //}
                            if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, _forecastInfo.Period, 22);//b

                            ctx.SaveChanges();
                        }
                    }
                }
                res = "Data saved successfully";
                return res;
            }
            catch(Exception ex)
            {
                res = ex.Message;
                return res;
            }

           
        }



        public string saveimportconsumption(List<receivereportdata> _rdata)
        {
            string res = "";
            try
            {
                ForecastSite fs = new ForecastSite();
                ForecastCategory fcat = new ForecastCategory();
                ForecastCategorySite fcatsite = new ForecastCategorySite();

                bool isduplicate = false;
                ForecastSite efs = new ForecastSite();//existing
            
                _forecastInfo = ctx.ForecastInfo.Find(_rdata[0].forecastid);
               

                var countryid = ctx.User.Find(_forecastInfo.UserId).CountryId;
                //  var forecastsite = ctx.ForecastSite.Where(b => b.ForecastInfoId == _rdata[0].forecastid).ToList();
                List<Site> sites = new List<Site>();
                sites = ctx.Site.Where(b => b.CountryId == countryid || b.UserId == _forecastInfo.UserId).ToList();
                var country = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault();
                List<ForecastSite> forecastSites = new List<ForecastSite>();
                List<Site> sitelst = new List<Site>();
                List<HistoricalData> hisdata = new List<HistoricalData>();
                hisdata = ctx.HistoricalData.ToList();
                forecastSites = ctx.ForecastSite.Where(b => b.ForecastInfoId == _forecastInfo.ForecastID).ToList();
                //  ctx.ForecastSite.RemoveRange(forecastsite);
                if (_forecastInfo.DataUsage != "DATA_USAGE3")
                {
                    IList<HistoricalData> existingFsp = new List<HistoricalData>();
                    IList<ForecastSiteProduct> existingFsp1 = new List<ForecastSiteProduct>();
                    var getdistinctsiteids = _rdata.GroupBy(b => b.SiteID).Select(x => x.Key).ToList();
                    for (int i = 0; i < getdistinctsiteids.Count; i++)
                    {
                      

                        fs = forecastSites.Where(b => b.SiteId == getdistinctsiteids[i]).FirstOrDefault();
                        if (fs == null)
                        {
                            fs = new ForecastSite();
                            fs.SiteId = getdistinctsiteids[i];
                            fs.ForecastInfoId = _forecastInfo.ForecastID;
                            ctx.ForecastSite.Add(fs);
                            ctx.SaveChanges();

                        }
                        var dd = _rdata.Where(x => x.SiteID == getdistinctsiteids[i]).GroupBy(b=>b.ProID).Select(x=>x.Key).ToList();
                        decimal workingday = sites.Where(b => b.SiteID == _rdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

                        for (int j = 0; j < dd.Count; j++)
                        {

                          //  var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.Id && b.ProductID == dd[j]).ToList();
                      



                             var isexist1 = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.Id && b.ProductID == dd[j]).ToList();
                        //    var isexist1 = ctx.HistoricalData.Where(b => b.SiteId == getdistinctsiteids[i] && b.ProductId == dd[j]).ToList();
                            var data1 = _rdata.Where(x => x.SiteID == getdistinctsiteids[i] && x.ProID == dd[j]).ToList();
                            for (int k = 0; k < data1.Count; k++)
                            {
                                if (!data1[k].haserror)
                                {
                                    ForecastSiteProduct fp1 = new ForecastSiteProduct
                                    {
                                        ForecastSiteID = fs.Id,
                                        ProductID = data1[k].ProID
                                };
                                   // fp.ProductId = data[k].ProID;
                                    if (!Utility.Utility.IsDateTime(data1[k].Duration))
                                    {
                                        fp1.CDuration = data1[k].Duration;
                                        fp1.DurationDateTime = Utility.Utility.DurationToDateTime(data1[k].Duration);

                                    }
                                    else
                                    {
                                        fp1.DurationDateTime = DateTime.Parse(data1[k].Duration);
                                        fp1.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data1[k].Duration));
                                    }
                                    fp1.AmountUsed = data1[k].Amount;
                                    fp1.StockOut = data1[k].StockOut;
                                    fp1.InstrumentDowntime = data1[k].InstrumentDownTime;//b
                                    if (fp1.StockOut > 0)
                                    {
                                        int days = fp1.StockOut;
                                        // decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

                                        if (days >= workingday)
                                        {
                                            days = 0;
                                            fp1.StockOut = 0;
                                        }
                                        if (days >= 0)
                                            fp1.StockOut = days;
                                    }


                                    // var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

                                    if (isexist1.Count == 0)
                                    {



                                        fp1.Adjusted = fp1.AmountUsed;

                                        if ((fp1.InstrumentDowntime > 0 || fp1.StockOut > 0) && fp1.AmountUsed > 0)
                                            fp1.Adjusted = Utility.Utility.GetAdjustedVolume(fp1.AmountUsed, fp1.StockOut + fp1.InstrumentDowntime, country.Period, workingday);//b

                                        existingFsp1.Add(fp1);
                                    }
                                    else
                                    {
                                        var exist1 = isexist1.Where(x => x.CDuration == fp1.CDuration && x.DurationDateTime == fp1.DurationDateTime).FirstOrDefault();
                                        if (exist1 == null)
                                        {


                                            fp1.Adjusted = fp1.AmountUsed;

                                            if ((fp1.InstrumentDowntime > 0 || fp1.StockOut > 0) && fp1.AmountUsed > 0)
                                                fp1.Adjusted = Utility.Utility.GetAdjustedVolume(fp1.AmountUsed, fp1.StockOut + fp1.InstrumentDowntime, country.Period, workingday);//b


                                            existingFsp1.Add(fp1);
                                        }
                                        else
                                        {

                                            exist1.AmountUsed = data1[k].Amount;
                                            exist1.StockOut = data1[k].StockOut;
                                            exist1.InstrumentDowntime = data1[k].InstrumentDownTime;//b
                                            exist1.Adjusted = fp1.AmountUsed;
                                            if ((exist1.InstrumentDowntime > 0 || exist1.StockOut > 0) && exist1.AmountUsed > 0)
                                                exist1.Adjusted = Utility.Utility.GetAdjustedVolume(exist1.AmountUsed, exist1.StockOut + exist1.InstrumentDowntime, country.Period, workingday);//b

                                            ctx.SaveChanges();
                                        }
                                    }







                                }
                            }



                        }
                    }


                    for (int i = 0; i < getdistinctsiteids.Count; i++)
                    {
                        var dd = _rdata.Where(x => x.SiteID == getdistinctsiteids[i]).GroupBy(b => b.ProID).Select(x => x.Key).ToList();
                        decimal workingday =sites.Where(b => b.SiteID == _rdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();
                        for (int j = 0; j < dd.Count; j++)
                        {

                            var isexist = hisdata.Where(b => b.SiteId == getdistinctsiteids[i] && b.ProductId == dd[j]).ToList();
                            var data = _rdata.Where(x => x.SiteID == getdistinctsiteids[i] && x.ProID == dd[j]).ToList();
                            for (int k = 0; k < data.Count; k++)
                            {
                                if (!data[k].haserror)
                                {
                                    HistoricalData fp = new HistoricalData();

                                    fp.SiteId = getdistinctsiteids[i];
                                    fp.ProductId = data[k].ProID;
                                    if (!Utility.Utility.IsDateTime(data[k].Duration))
                                    {
                                        fp.CDuration = data[k].Duration;
                                        fp.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);

                                    }
                                    else
                                    {
                                        fp.DurationDateTime = DateTime.Parse(data[k].Duration);
                                        fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
                                    }
                                    fp.AmountUsed = data[k].Amount;
                                    fp.StockOut = data[k].StockOut;
                                    fp.InstrumentDowntime = data[k].InstrumentDownTime;//b
                                    if (fp.StockOut > 0)
                                    {
                                        int days = fp.StockOut;
                                        // decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

                                        if (days >= workingday)
                                        {
                                            days = 0;
                                            fp.StockOut = 0;
                                        }
                                        if (days >= 0)
                                            fp.StockOut = days;
                                    }


                                    // var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

                                    if (isexist.Count == 0)
                                    {



                                        fp.Adjusted = fp.AmountUsed;

                                        if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                            fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b

                                        existingFsp.Add(fp);
                                    }
                                    else
                                    {
                                        var exist1 = isexist.Where(x => x.CDuration == fp.CDuration && x.DurationDateTime == fp.DurationDateTime).FirstOrDefault();
                                        if (exist1 == null)
                                        {


                                            fp.Adjusted = fp.AmountUsed;

                                            if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                                fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b


                                            existingFsp.Add(fp);
                                        }
                                        else
                                        {

                                            exist1.AmountUsed = data[k].Amount;
                                            exist1.StockOut = data[k].StockOut;
                                            exist1.InstrumentDowntime = data[k].InstrumentDownTime;//b
                                            exist1.Adjusted = fp.AmountUsed;
                                            if ((exist1.InstrumentDowntime > 0 || exist1.StockOut > 0) && exist1.AmountUsed > 0)
                                                exist1.Adjusted = Utility.Utility.GetAdjustedVolume(exist1.AmountUsed, exist1.StockOut + exist1.InstrumentDowntime, country.Period,workingday);//b

                                            ctx.SaveChanges();
                                        }
                                    }







                                }
                            }
                        }
                    }
                    ctx.HistoricalData.AddRange(existingFsp);
                    ctx.SaveChanges();
                    ctx.ForecastSiteProduct.AddRange(existingFsp1);
                    ctx.SaveChanges();
                    res = "Data saved successfully";
                }
                else
                {
                    IList<HistoricalData> existingFcp = new List<HistoricalData>();
                    IList<ForecastCategoryProduct> existingFcp1 = new List<ForecastCategoryProduct>();
                    var getdistinctsiteids = _rdata.GroupBy(b => b.CategoryName).Select(x => x.Key).ToList();
                    for (int i = 0; i < getdistinctsiteids.Count; i++)
                    {
                        var dd = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i]).GroupBy(b => b.ProID).Select(x => x.Key).ToList(); 
                        fcat = ctx.ForecastCategory.Where(b => b.CategoryName == getdistinctsiteids[i] && b.ForecastId == _forecastInfo.ForecastID).FirstOrDefault(); ;
                        if (fcat == null)
                        {
                            fcat = new ForecastCategory();
                            fcat.CategoryName = getdistinctsiteids[i];
                            fcat.ForecastId = _forecastInfo.ForecastID;
                            ctx.ForecastCategory.Add(fcat);
                            ctx.SaveChanges();

                        }
                        decimal workingday = 22;

                        for (int j = 0; j < dd.Count; j++)
                        {
                            var isexist = ctx.HistoricalData.Where(b => b.CategoryId == fcat.CategoryId && b.ProductId == dd[j]).ToList();

                            var data = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i] && x.ProID == dd[j]).ToList();
                            for (int k = 0; k < data.Count; k++)
                          
                                {
                                HistoricalData fp = new HistoricalData();




                                isduplicate = false;

                                if (isexist.Count == 0)
                                    fp = new HistoricalData();
                                else
                                {
                                    var exist1 = isexist.Where(x => x.CDuration == data[k].Duration1 ).FirstOrDefault();
                                    if (exist1 != null)
                                    {
                                        isduplicate = true;
                                    }
                                }

                                fp.CategoryId = fcat.CategoryId;
                                fp.ProductId = data[k].ProID;


                                fp.AmountUsed = data[k].Amount;
                                fp.StockOut = data[k].StockOut;
                                fp.InstrumentDowntime = data[k].InstrumentDownTime;//b

                                if (!Utility.Utility.IsDateTime(data[k].Duration))
                                {
                                    fp.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);
                                    fp.CDuration = data[k].Duration;
                                }
                                else
                                {
                                    fp.DurationDateTime = DateTime.Parse(data[k].Duration);
                                    fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
                                }

                                if (fp.StockOut > 0)
                                {
                                    int days = fp.StockOut;
                                 

                                    if (days >= workingday)
                                    {
                                        days = 0;
                                        fp.StockOut = 0;
                                    }
                                    if (days >= 0)
                                        fp.StockOut = days;
                                }
                                if (!isduplicate)
                                {
                                    //ctx.ForecastCategoryProduct.Add(fp);
                                    //ctx.SaveChanges();

                                    //fp.Adjusted = fp.AmountUsed;
                                    //if (fp.AmountUsed == 0)
                                    //{
                                    //    try
                                    //    {
                                    //        Consumption cs = GetConsumption(fp.Product.Id, fp);
                                    //        fp.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
                                    //    }
                                    //    catch { fp.Adjusted = fp.AmountUsed; }
                                    //}
                                    if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                        fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, 22);//b
                                    existingFcp.Add(fp);
                                   // ctx.SaveChanges();
                                }






                            }



                            //////////forecastcategory
                            ///
                            var isexist1 = ctx.ForecastCategoryProduct.Where(b => b.CategoryID == fcat.CategoryId && b.ProductID == dd[j]).ToList();

                            var data1 = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i] && x.ProID == dd[j]).ToList();
                            for (int k = 0; k < data1.Count; k++)

                            {
                                ForecastCategoryProduct fp1 = new ForecastCategoryProduct();




                                isduplicate = false;

                                if (isexist.Count == 0)
                                    fp1 = new ForecastCategoryProduct();
                                else
                                {
                                    var exist1 = isexist1.Where(x => x.CDuration == data[k].Duration1).FirstOrDefault();
                                    if (exist1 != null)
                                    {
                                        isduplicate = true;
                                    }
                                }

                                fp1.CategoryID = fcat.CategoryId;
                                fp1.ProductID = data[k].ProID;


                                fp1.AmountUsed = data[k].Amount;
                                fp1.StockOut = data[k].StockOut;
                                fp1.InstrumentDowntime = data[k].InstrumentDownTime;//b

                                if (!Utility.Utility.IsDateTime(data[k].Duration))
                                {
                                    fp1.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);
                                    fp1.CDuration = data[k].Duration;
                                }
                                else
                                {
                                    fp1.DurationDateTime = DateTime.Parse(data[k].Duration);
                                    fp1.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
                                }

                                if (fp1.StockOut > 0)
                                {
                                    int days = fp1.StockOut;


                                    if (days >= workingday)
                                    {
                                        days = 0;
                                        fp1.StockOut = 0;
                                    }
                                    if (days >= 0)
                                        fp1.StockOut = days;
                                }
                                if (!isduplicate)
                                {
                                    //ctx.ForecastCategoryProduct.Add(fp1);
                                    //ctx.SaveChanges();

                                    //fp1.Adjusted = fp1.AmountUsed;
                                    //if (fp1.AmountUsed == 0)
                                    //{
                                    //    try
                                    //    {
                                    //        Consumption cs = GetConsumption(fp1.Product.Id, fp1);
                                    //        fp1.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
                                    //    }
                                    //    catch { fp1.Adjusted = fp1.AmountUsed; }
                                    //}
                                    if ((fp1.InstrumentDowntime > 0 || fp1.StockOut > 0) && fp1.AmountUsed > 0)
                                        fp1.Adjusted = Utility.Utility.GetAdjustedVolume(fp1.AmountUsed, fp1.StockOut + fp1.InstrumentDowntime, country.Period, 22);//b
                                    existingFcp1.Add(fp1);
                                    // ctx.SaveChanges();
                                }






                            }
                        }
                        
                    }

                    ctx.HistoricalData.AddRange(existingFcp);
                    ctx.ForecastCategoryProduct.AddRange(existingFcp1);
                    ctx.SaveChanges();
                    res = "Data saved successfully";
                }


                return res;
            }
            catch (Exception ex)
            {
                res = ex.Message;
                return res;
            }


        }
        public string saveimportservice(Reportobject _rdata)
        {
            string res = "";
            ForecastSite fs = new ForecastSite();
            ForecastCategorySite fcatsite = new ForecastCategorySite();
            ForecastCategory fcat = new ForecastCategory();
            bool isduplicate = false;
            ForecastSite efs = new ForecastSite();//existing
            string fail = "";
            _forecastInfo = ctx.ForecastInfo.Find(_rdata.receivereportdata[0].forecastid);

            var countryid = ctx.User.Find(_forecastInfo.UserId).CountryId;
            var country = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault();
            var testarray = ctx.ForecastTest.Where(b => b.forecastID == _forecastInfo.ForecastID).Select(b=>b.TestID).ToArray();
            var distincttest= _rdata.receivereportdata.GroupBy(b => b.testID).Select(x => x.Key).ToList();
            List<Site> sites = new List<Site>();
            sites = ctx.Site.Where(b => b.CountryId == countryid || b.UserId == _forecastInfo.UserId).ToList();
            for (int j = 0; j < testarray.Length; j++)
            {
                if (distincttest.Contains(testarray[j]) != true)
                {
                    fail = fail + "," + ctx.Test.Find(testarray[j]).TestName;
                }

            }



            if (_forecastInfo.DataUsage != "DATA_USAGE3")
            {
                // IList<ForecastSiteTest> existingFsp = new List<ForecastSiteTest>();
                IList<HistoricalData> existingFsp = new List<HistoricalData>();
                IList<ForecastSiteTest> existingFsp1 = new List<ForecastSiteTest>();
                var getdistinctsiteids = _rdata.receivereportdata.GroupBy(b => b.SiteID).Select(x => x.Key).ToList();
                for (int i = 0; i < getdistinctsiteids.Count; i++)
                {


                    fs = ctx.ForecastSite.Where(b => b.SiteId == getdistinctsiteids[i] && b.ForecastInfoId == _forecastInfo.ForecastID).FirstOrDefault();
                    if (fs == null)
                    {
                        fs = new ForecastSite();
                        fs.SiteId = getdistinctsiteids[i];
                        fs.ForecastInfoId = _forecastInfo.ForecastID;
                        ctx.ForecastSite.Add(fs);
                        ctx.SaveChanges();

                    }
                    var dd = _rdata.receivereportdata.Where(x => x.SiteID == getdistinctsiteids[i]).GroupBy(b => b.testID).Select(x => x.Key).ToList();
                    decimal workingday = sites.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();
                


                    for (int j = 0; j < dd.Count; j++)
                    {
                        if (testarray.Contains(dd[j]) == true)
                        {
                          //  var isexist = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == fs.Id && b.TestID == dd[j]).ToList();
                       

                            ////////forecasttest
                            ///


                            var isexist1 = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == fs.Id && b.TestID == dd[j]).ToList();
                            var data1 = _rdata.receivereportdata.Where(x => x.SiteID == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
                            for (int k = 0; k < data1.Count; k++)
                            {
                                if (!data1[k].haserror)
                                {
                                    ForecastSiteTest fp = new ForecastSiteTest
                                    {
                                        ForecastSiteID = fs.Id ,
                                        TestID = data1[k].testID
                                    };
                                    if (!Utility.Utility.IsDateTime(data1[k].Duration))
                                    {
                                        fp.CDuration = data1[k].Duration;
                                        fp.DurationDateTime = Utility.Utility.DurationToDateTime(data1[k].Duration);

                                    }
                                    else
                                    {
                                        fp.DurationDateTime = DateTime.Parse(data1[k].Duration);
                                        fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data1[k].Duration));
                                    }
                                    fp.AmountUsed = data1[k].Amount;
                                    fp.StockOut = data1[k].StockOut;
                                    fp.InstrumentDowntime = data1[k].InstrumentDownTime;//b
                                    if (fp.StockOut > 0)
                                    {
                                        int days = fp.StockOut;
                                        // decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

                                        if (days >= workingday)
                                        {
                                            days = 0;
                                            fp.StockOut = 0;
                                        }
                                        if (days >= 0)
                                            fp.StockOut = days;
                                    }


                                    // var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

                                    if (isexist1.Count == 0)
                                    {



                                        fp.Adjusted = fp.AmountUsed;

                                        if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                            fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period,workingday);//b

                                        existingFsp1.Add(fp);
                                    }
                                    else
                                    {
                                        var exist1 = isexist1.Where(x => x.CDuration == fp.CDuration && x.DurationDateTime == fp.DurationDateTime).FirstOrDefault();
                                        if (exist1 == null)
                                        {


                                            exist1.Adjusted = fp.AmountUsed;

                                            if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                                exist1.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b


                                            ctx.SaveChanges();
                                        }
                                    }







                                }
                            }
                        }


                    }
                }


                for (int i = 0; i < getdistinctsiteids.Count; i++)
                {
                    var dd = _rdata.receivereportdata.Where(x => x.SiteID == getdistinctsiteids[i]).GroupBy(b => b.testID).Select(x => x.Key).ToList();
                    decimal workingday = sites.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();



                    for (int j = 0; j < dd.Count; j++)
                    {
                        var isexist = ctx.HistoricalData.Where(b => b.SiteId == getdistinctsiteids[i] && b.TestId == dd[j]).ToList();
                        var data = _rdata.receivereportdata.Where(x => x.SiteID == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
                        for (int k = 0; k < data.Count; k++)
                        {
                            if (!data[k].haserror)
                            {
                                HistoricalData fp = new HistoricalData();

                                fp.SiteId = getdistinctsiteids[i];
                                fp.TestId = data[k].testID;
                                if (!Utility.Utility.IsDateTime(data[k].Duration))
                                {
                                    fp.CDuration = data[k].Duration;
                                    fp.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);

                                }
                                else
                                {
                                    fp.DurationDateTime = DateTime.Parse(data[k].Duration);
                                    fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
                                }
                                fp.AmountUsed = data[k].Amount;
                                fp.StockOut = data[k].StockOut;
                                fp.InstrumentDowntime = data[k].InstrumentDownTime;//b
                                if (fp.StockOut > 0)
                                {
                                    int days = fp.StockOut;
                                    // decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

                                    if (days >= workingday)
                                    {
                                        days = 0;
                                        fp.StockOut = 0;
                                    }
                                    if (days >= 0)
                                        fp.StockOut = days;
                                }


                                // var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

                                if (isexist.Count == 0)
                                {



                                    fp.Adjusted = fp.AmountUsed;

                                    if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                        fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period,workingday);//b

                                    existingFsp.Add(fp);
                                }
                                else
                                {
                                    var exist1 = isexist.Where(x => x.CDuration == fp.CDuration && x.DurationDateTime == fp.DurationDateTime).FirstOrDefault();
                                    if (exist1 == null)
                                    {


                                        exist1.Adjusted = fp.AmountUsed;

                                        if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                            exist1.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period,workingday);//b


                                        ctx.SaveChanges();
                                    }
                                    else
                                    {

                                        exist1.AmountUsed = data[k].Amount;
                                        exist1.StockOut = data[k].StockOut;
                                        exist1.InstrumentDowntime = data[k].InstrumentDownTime;//b
                                        exist1.Adjusted = fp.AmountUsed;
                                        if ((exist1.InstrumentDowntime > 0 || exist1.StockOut > 0) && exist1.AmountUsed > 0)
                                            exist1.Adjusted = Utility.Utility.GetAdjustedVolume(exist1.AmountUsed, exist1.StockOut + exist1.InstrumentDowntime, country.Period, workingday);//b

                                        ctx.SaveChanges();
                                    }
                                }







                            }
                        }
                    }
                }
                ctx.HistoricalData.AddRange(existingFsp);
                ctx.SaveChanges();
                ctx.ForecastSiteTest.AddRange(existingFsp1);
                ctx.SaveChanges();
                if (fail != "")
                {
                    res ="you are selected these test " +fail.TrimStart(',') + " but excel sheet doesn't have these";
                }
                else
                {
                    res = "Data saved successfully";
                }
            }

            else
            {
                IList<HistoricalData> existingFcp = new List<HistoricalData>();
                IList<ForecastCategoryTest> existingFcp1 = new List<ForecastCategoryTest>();
                var getdistinctsiteids = _rdata.receivereportdata.GroupBy(b => b.CategoryName).Select(x => x.Key).ToList();
                for (int i = 0; i < getdistinctsiteids.Count; i++)
                {
                    var dd = _rdata.receivereportdata.Where(x => x.CategoryName == getdistinctsiteids[i]).GroupBy(b => b.testID).Select(x => x.Key).ToList();
                    fcat = ctx.ForecastCategory.Where(b => b.CategoryName == getdistinctsiteids[i] && b.ForecastId == _forecastInfo.ForecastID).FirstOrDefault(); ;
                    if (fcat == null)
                    {
                        fcat = new ForecastCategory();
                        fcat.CategoryName = getdistinctsiteids[i];
                        fcat.ForecastId = _forecastInfo.ForecastID;
                        ctx.ForecastCategory.Add(fcat);
                        ctx.SaveChanges();

                    }
                    decimal workingday = 22;

                    for (int j = 0; j < dd.Count; j++)
                    {
                        //var isexist = ctx.ForecastCategoryTest.Where(b => b.CategoryID == fcat.CategoryId && b.TestID == dd[j]).ToList();
                        var isexist = ctx.HistoricalData.Where(b => b.CategoryId == fcat.CategoryId && b.TestId == dd[j]).ToList();
                        var data = _rdata.receivereportdata.Where(x => x.CategoryName == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
                        for (int k = 0; k < data.Count; k++)

                        {
                           // ForecastCategoryTest fp = new ForecastCategoryTest();

                            HistoricalData fp = new HistoricalData();


                            isduplicate = false;

                            if (isexist.Count == 0)
                                fp = new HistoricalData();
                            else
                            {
                                var exist1 = isexist.Where(x => x.CDuration == data[k].Duration1).FirstOrDefault();
                                if (exist1 != null)
                                {
                                    isduplicate = true;
                                }
                            }

                            fp.CategoryId = fcat.CategoryId;
                            fp.TestId = data[k].testID;

                            fp.AmountUsed = data[k].Amount;
                            fp.StockOut = data[k].StockOut;
                            fp.InstrumentDowntime = data[k].InstrumentDownTime;//b

                            if (!Utility.Utility.IsDateTime(data[k].Duration))
                            {
                                fp.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);
                                fp.CDuration = data[k].Duration;
                            }
                            else
                            {
                                fp.DurationDateTime = DateTime.Parse(data[k].Duration);
                                fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
                            }

                            if (fp.StockOut > 0)
                            {
                                int days = fp.StockOut;


                                if (days >= workingday)
                                {
                                    days = 0;
                                    fp.StockOut = 0;
                                }
                                if (days >= 0)
                                    fp.StockOut = days;
                            }
                            if (!isduplicate)
                            {
                                //ctx.ForecastCategoryProduct.Add(fp);
                                //ctx.SaveChanges();

                                //fp.Adjusted = fp.AmountUsed;
                                //if (fp.AmountUsed == 0)
                                //{
                                //    try
                                //    {
                                //        Consumption cs = GetConsumption(fp.Product.Id, fp);
                                //        fp.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
                                //    }
                                //    catch { fp.Adjusted = fp.AmountUsed; }
                                //}
                                if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                    fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, 22);//b
                                existingFcp.Add(fp);
                                // ctx.SaveChanges();
                            }






                        }



                        //forecastcategory

                        var isexist1 = ctx.HistoricalData.Where(b => b.CategoryId == fcat.CategoryId && b.TestId == dd[j]).ToList();
                        var data1 = _rdata.receivereportdata.Where(x => x.CategoryName == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
                        for (int k = 0; k < data1.Count; k++)

                        {
                            // ForecastCategoryTest fp = new ForecastCategoryTest();

                            ForecastCategoryTest fp = new ForecastCategoryTest();


                            isduplicate = false;

                            if (isexist.Count == 0)
                                fp = new ForecastCategoryTest();
                            else
                            {
                                var exist1 = isexist.Where(x => x.CDuration == data1[k].Duration1).FirstOrDefault();
                                if (exist1 != null)
                                {
                                    isduplicate = true;
                                }
                            }

                            fp.CategoryID = fcat.CategoryId;
                            fp.TestID = data1[k].testID;

                            fp.AmountUsed = data1[k].Amount;
                            fp.StockOut = data1[k].StockOut;
                            fp.InstrumentDowntime = data1[k].InstrumentDownTime;//b

                            if (!Utility.Utility.IsDateTime(data1[k].Duration))
                            {
                                fp.DurationDateTime = Utility.Utility.DurationToDateTime(data1[k].Duration);
                                fp.CDuration = data1[k].Duration;
                            }
                            else
                            {
                                fp.DurationDateTime = DateTime.Parse(data1[k].Duration);
                                fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data1[k].Duration));
                            }

                            if (fp.StockOut > 0)
                            {
                                int days = fp.StockOut;


                                if (days >= workingday)
                                {
                                    days = 0;
                                    fp.StockOut = 0;
                                }
                                if (days >= 0)
                                    fp.StockOut = days;
                            }
                            if (!isduplicate)
                            {
                                //ctx.ForecastCategoryProduct.Add(fp);
                                //ctx.SaveChanges();

                                //fp.Adjusted = fp.AmountUsed;
                                //if (fp.AmountUsed == 0)
                                //{
                                //    try
                                //    {
                                //        Consumption cs = GetConsumption(fp.Product.Id, fp);
                                //        fp.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
                                //    }
                                //    catch { fp.Adjusted = fp.AmountUsed; }
                                //}
                                if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                    fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, 22);//b
                                existingFcp1.Add(fp);
                                // ctx.SaveChanges();
                            }






                        }
                    }

                }

                ctx.HistoricalData.AddRange(existingFcp);
                ctx.ForecastCategoryTest.AddRange(existingFcp1);
                ctx.SaveChanges();
                res = "Data saved successfully";
            }




            return res;

        }
       public IEnumerable<ForecastSiteInfonew> Importpatient(IFormFile file,int id,int userid,int countryid)
        {


            string folderName = "Upload";

            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            StringBuilder sb = new StringBuilder();
            ISheet sheet;
            IWorkbook workbook = null;
            IFont Font;
            ICellStyle cellStyle;
            List<Region> Rgl = new List<Region>();
            List<ForecastSiteInfo> FS = new List<ForecastSiteInfo>();
            string str;

            var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>

   x.Id

).FirstOrDefault();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            if (file.Length > 0)
            {
                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                string fullPath = Path.Combine(newPath, fileName);
                // string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                //string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();

                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite))
                {

                    if (sFileExtension == ".xls")
                    {
                        file.CopyTo(stream);
                        stream.Position = 0;
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheet("Morbidity-PatientNumber"); //get first sheet from workbook  
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }
                    else
                    {

                        // file.CopyTo(stream);
                        stream.Position = 0;
                        //InputStream ExcelFileToRead = new FileInputStream("C:/Test.xlsx");
                        XSSFWorkbook hssfwb = new XSSFWorkbook((FileStream)stream); //This will read 2007 Excel format  

                        sheet = hssfwb.GetSheet("Morbidity-PatientNumber"); //get first sheet from workbook   
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;
                    if (4 > cellCount)
                    {
                        str = "Imported Morbidity-PatientNumber Sheet has less columns than needed.";
                       // return str;
                    }
                    if (4 < cellCount)
                    {
                        str = "Imported Morbidity-PatientNumber Sheet has too many columns.";
                     //   return str;
                    }
                    if (sheet.LastRowNum == 0)
                    {
                        str = "Imported Morbidity-PatientNumber Sheet is empty.";
                      //  return str;
                    }


                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //for (int j = row.FirstCellNum; j < cellCount; j++)
                        //{

                        if (row.GetCell(0) != null && row.GetCell(0).ToString() != "")
                        {
                            var region = ctx.Region.Where(b => b.RegionName == row.GetCell(0).ToString() && (b.UserId == adminuserid || b.UserId==userid)).FirstOrDefault();
                            if (region != null)
                            {

                                if (row.GetCell(1) != null && row.GetCell(1).ToString() != "")
                                {
                                    var site = ctx.Site.Where(b => b.SiteName == row.GetCell(1).ToString() && (b.UserId == adminuserid || b.UserId == userid)).FirstOrDefault();
                                    {
                                        var ForecastSiteInfo = ctx.ForecastSiteInfo.Where(c => c.ForecastinfoID == id && c.SiteID == site.SiteID).FirstOrDefault();
                                        if (ForecastSiteInfo == null && (Convert.ToInt64(row.GetCell(3).ToString())> Convert.ToInt64(row.GetCell(2).ToString())))
                                        {
                                            ForecastSiteInfo FS1 = new ForecastSiteInfo();
                                            FS1.ForecastinfoID = id;
                                            FS1.SiteID = site.SiteID;
                                            FS1.CurrentPatient = Convert.ToInt64(row.GetCell(2).ToString());
                                            FS1.TargetPatient = Convert.ToInt64(row.GetCell(3).ToString());
                                            FS.Add(FS1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            //str = str + "Error in Row No." + (i + 1) + " : Region Name Required,";
                            //error = error + 1;
                            //continue;
                        }
                     
                    }

                    //str = str + noofsave + " Regions/Districts/Provinces  are imported and saved successfully.,";
                    //if (error > 0)
                    //{
                    //    str = str + error + " Regions/Districts/Provinces  Failed.Please check excel and correct Regions/Districts/Provinces Entry";
                    //}
                }

                if (FS.Count > 0)
                {
                    ctx.ForecastSiteInfo.AddRange(FS);
                    ctx.SaveChanges();
                }

            }


            var forecastsiteinfo = ctx.ForecastSiteInfo.Join(ctx.Site, b => b.SiteID, c => c.SiteID, (b, c) => new { b, c }).Where(f => f.b.ForecastinfoID == id).Select(g => new ForecastSiteInfonew
            {
                SiteID = g.b.SiteID,
                SiteName = g.c.SiteName,
                CurrentPatient = g.b.CurrentPatient,
                TargetPatient = g.b.TargetPatient,
                ForecastinfoID = g.b.ForecastinfoID,
                PopulationNumber = g.b.PopulationNumber,
                PrevalenceRate = g.b.PrevalenceRate,
                ID = g.b.ID

            }).ToList();
            return forecastsiteinfo;
        }
        public string importexcel(IFormFile file,int userid,int countryid, string sheets,List<Matchrule> matchrules)
        {
            string str = "";
            try
            {

                string folderName = "Upload";

                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                StringBuilder sb = new StringBuilder();
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    // string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    //string fullPath = Path.Combine(newPath, file.FileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                    sheets = sheets.TrimEnd(',');
                    string[] p1;
                    p1 = sheets.Split(",");

                    for (int i = 0; i < p1.Length; i++)
                    {
                        p1[i] = p1[i].Trim();
                    }
                   
                   if( Array.IndexOf(p1, "Region")>=0)

                    str = validateandsaveregion(fullPath, "Region", sFileExtension, file, userid, countryid, matchrules.Where(b => b.regSheet == "Region").FirstOrDefault());
                    if (Array.IndexOf(p1, "Site") >= 0)
                    str = str + "#" + validateandsavesite(fullPath, "Site", sFileExtension, file, userid, countryid, matchrules.Where(b => b.regSheet == "Site").FirstOrDefault());
                    if (Array.IndexOf(p1, "Product") >= 0)
                     str = str + "#" + validateandsaveproduct(fullPath, "Product", sFileExtension, file, userid, matchrules.Where(b => b.regSheet == "Product").FirstOrDefault());
                     if (Array.IndexOf(p1, "Test") >= 0)
                        str = str + "#" + validateandsaveTest(fullPath, "Test", sFileExtension, file, userid, matchrules.Where(b => b.regSheet == "Test").FirstOrDefault());
                    if (Array.IndexOf(p1, "Instrument") >= 0)
                        str = str + "#" + validateandsaveinstrument(fullPath, "Instrument", sFileExtension, file, userid, matchrules.Where(b => b.regSheet == "Instrument").FirstOrDefault());
                     if (Array.IndexOf(p1, "Test Product Usage Rate") >= 0)
                        str = str + "#" + validateandsavetestproduct(fullPath, "Test Product Usage Rate", sFileExtension, file, userid, matchrules.Where(b => b.regSheet == "Test Product Usage Rate").FirstOrDefault());
                     if (Array.IndexOf(p1, "Consumables") >= 0)
                        str = str + "#" + validateandsaveconsumables(fullPath, "Consumables", sFileExtension, file, userid, matchrules.Where(b => b.regSheet == "Consumables").FirstOrDefault());
                     if (Array.IndexOf(p1, "Site Instrument") >= 0)
                        str = str + "#" + validateandsavesiteinstrument(fullPath, "Site Instrument", sFileExtension, file, userid, matchrules.Where(b => b.regSheet == "Site Instrument").FirstOrDefault());

                }
                return str;
            }
            catch(Exception ex)
            {
                str = ex.Message;
                return str;
            }

            
        }
        public DataTable importservice(List<string[]> jArray, int id,int userid)
        {
         //   DataTable DT = new DataTable();
            IList<ReportedData> _rdata;
          
            ServiceDT.Columns.Add("No");



            ServiceDT.Columns.Add("CategoryName");
            ServiceDT.Columns.Add("RegionName");
            ServiceDT.Columns.Add("SiteName");
            ServiceDT.Columns.Add("testName");
            ServiceDT.Columns.Add("Duration");
            ServiceDT.Columns.Add("Amount");
            ServiceDT.Columns.Add("StockOut");
            ServiceDT.Columns.Add("InstrumentDownTime");
            ServiceDT.Columns.Add("Description");
            ServiceDT.Columns.Add("SiteID");
            ServiceDT.Columns.Add("testID");
            ServiceDT.Columns.Add("CatID");
            ServiceDT.Columns.Add("Duration1");
            ServiceDT.Columns.Add("haserror");
            ServiceDT.Columns.Add("forecastid");
           GetDataServiceRow_New(jArray, id, userid);

            //foreach (ReportedData rd in _rdata)
            //{
            //    DataRow Dr = DT.NewRow();

            //    Dr["No"] = rd._rowno.ToString();
            //    if (_forecastInfo.DataUsage == "DATA_USAGE3")
            //    {
            //        Dr["CategoryName"] = rd._categoryName;
            //        Dr["RegionName"] = "";
            //        Dr["SiteName"] = "";
            //    }
            //    else
            //    {
            //        Dr["CategoryName"] = "";
            //        Dr["RegionName"] = rd._regionName;
            //        Dr["SiteName"] = rd._siteName;
            //    }
            //    Dr["testName"] = rd._testname;
            //    if (!Utility.Utility.IsDateTime(rd._duration))
            //    {

            //        try
            //        {
            //            DateTime dd = Utility.Utility.DurationToDateTime(rd._duration);
            //            if (rd._duration.StartsWith("Q") && (_forecastInfo.Period == "Yearly"))
            //            {

            //                rd._duration = dd.Year.ToString();
            //                Dr["Duration"] = Utility.Utility.DatetimeToDurationStr(_forecastInfo.Period, dd);
            //            }
            //            else
            //                Dr["Duration"] = rd._duration;
            //        }
            //        catch (Exception ex)
            //        {
            //            Dr["Duration"] = rd._duration;
            //            rd._hasError = true;
            //        }

            //    }
            //    else
            //    {

            //        string datestr = Utility.Utility.DatetimeToDurationStr(_forecastInfo.Period, DateTime.Parse(rd._duration));
            //        if (!rd._duration.StartsWith("Q"))
            //        {
            //            rd._duration = Utility.Utility.DatetimeToDurationStr(_forecastInfo.Period, DateTime.Parse(rd._duration));
            //            if (_forecastInfo.Period == "Yearly")
            //            {
            //                Dr["Duration"] = datestr;

            //            }
            //            else
            //            {
            //                Dr["Duration"] = rd._duration;

            //            }
            //        }
            //        else
            //        {
            //            {
            //                Dr["Duration"] = datestr;

            //            }


            //        }

            //    }



            //    Dr["Amount"] = rd._amount.ToString();
            //    Dr["StockOut"] = rd._stockout.ToString();
            //    Dr["InstrumentDownTime"] = rd._instrumentDownTime.ToString();
            //    //if (rd.HasError == true && rd.ErrorDescription == "")
            //    //    rd.ErrorDescription = " Product Consumed Required ";

            //    //     li.SubItems.Add(rd.ErrorDescription.ToString());
            //    if (Utility.Utility.validDate(rd._duration, _forecastInfo.Period))
            //        rd._hasError = true;

            //    if (rd._hasError == true && rd.errorDescription == "")
            //        rd.errorDescription = " Product Consumed Required ";
            //    Dr["Description"] = rd.errorDescription.ToString();

            //    Dr["SiteID"] = rd._site.SiteID;
            //    if (rd._test != null)
            //        Dr["testID"] = rd._test.TestID;
            //    if (_forecastInfo.DataUsage == "DATA_USAGE3")
            //    {
            //        Dr["CatID"] = rd._category.CategoryId;
            //    }
            //    else
            //    {
            //        Dr["CatID"] = 0;
            //    }
            //    Dr["Duration1"] = rd._duration;
            //    Dr["haserror"] = rd._hasError;
            //    Dr["forecastid"] = id;
            //    DT.Rows.Add(Dr);
            //}







            return ServiceDT;






        }

        private void GetDataServiceRow_New(List<string[]> jArray, int id, int userid)
        {
            string categoryName = null;
            string regionName;
            string siteName;
            string testName;
            string duration;
            decimal amount;
            int stockout;
            int instrumentDownTime;
            decimal adjusited;
            int rowno = 0;
            bool haserror;
            string cName = "";
            string rName = "";
            string sName = "";
            string tName = "";
            _forecastInfo = ctx.ForecastInfo.Find(id);
            ForecastCategory fcategory = null;
            Models.Region region = null;
            Site site = null;
            //MasterProduct product = null;
            Test test = null;
            string errorDescription = "";
            int errornos = 0;
            string errormsg = "";
            IList<ReportedData> rdlist = new List<ReportedData>();
            string[] dr = jArray[0];
            DataTable dtexcelcolor = new DataTable();
            dtexcelcolor.Columns.Add("Sheet");
            dtexcelcolor.Columns.Add("Row");
            dtexcelcolor.Columns.Add("Error");
            dtexcelcolor.Columns.Add("Color");
            dtexcelcolor.AcceptChanges();
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            var countryid = ctx.User.Find(userid).CountryId;
            var country = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault();
            var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>

               x.Id

          ).FirstOrDefault();

            List<Test> tests = new List<Test>();
            tests = ctx.Test.Where(b => b.UserId == userid || b.UserId == adminuserid).ToList();
            List<Region> regions = new List<Region>();
            regions = ctx.Region.Where(b => b.CountryId == countryid || b.UserId == userid).ToList();
            List<Site> sites = new List<Site>();
            sites = ctx.Site.Where(b => b.CountryId == countryid || b.UserId == userid).ToList();
            for (int i = 1; i < jArray.Count; i++)//ds.Tables[0].Rows.Count
            {
                if (jArray[i].Length > 0)
                {

                    errormsg = "";
                    rowno++;
                    haserror = false;
                    string[] dr1 = jArray[i];
                    //string[] dr2 = jArray[i + 1];
                    //string[] dr3 = jArray[i + 2];
                    //string[] dr4 = jArray[i + 3];
                    string[] g = jArray[0];
                    // int colid = 0;
                    int f = 3;
                    int colid;//0 
                    if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        colid = 2;
                    else
                        colid = 3;

                    //int noofcolumn = 0;
                    //noofcolumn = jArray[i].Length - 1;
                    //bool newerror = false;

                    //do
                    //{
                    //    if (dr1[noofcolumn] == "")
                    //        dr1[noofcolumn] = "0";
                    //    Decimal amount1 = Convert.ToDecimal(dr1[noofcolumn]);
                    //    if (Convert.ToDecimal(dr1[noofcolumn]) == 0)
                    //    {
                    //        newerror = true;
                    //        break;
                    //    }

                    //    noofcolumn--;
                    //}
                    //while (noofcolumn >= jArray[i].Length - 3);
                    //if (newerror == true)
                    //{
                    //    errornos++;
                    //    if (errornos > 20)
                    //    {
                    //        //  label7.Text = "";
                    //        //MessageBox.Show("There too many problem with Instrument data, please troubleshoot and try to import again.", "Importing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //        throw new Exception("There too many problem with Service data, please troubleshoot and try to import again.");
                    //    }
                    //    errormsg = "The Test is not performed in last three months";
                    //    //dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                    //    ////   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                    //    //errormsg = "Please Check excel Some test are not performed in recent last three months";

                    //    continue;

                    //}
                    //else
                    //{
                    //    //if (_forecastInfo.DatausageEnum == DataUsageEnum.DATA_USAGE3)
                    //    //    colid = 3;
                    //    //else
                    //    //    colid = 4;

                    //}

                    //  label7.Text = "Importing Data.....................";


                    do
                    {
                        regionName = "";
                        siteName = "";
                        errormsg = "";
                        if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        {
                            categoryName = Convert.ToString(dr1[0]).Trim();//(dr[colid++])
                            testName = Convert.ToString(dr1[1]).Trim();   //(dr[colid++])testName
                        }
                        else
                        {
                            regionName = Convert.ToString(dr1[0]).Trim(); //(dr[colid++]) region name
                            siteName = Convert.ToString(dr1[1]).Trim();           //(dr[colid++]) site name
                            testName = Convert.ToString(dr1[2]).Trim();   //(dr[colid++])testName 
                        }
                        string date;
                        string[] datearr;
                        if (country.Period != "Monthly" || country.Period != "Bimonthly")
                        {

                            datearr = dr[colid].Split("/");
                            if (datearr[0].Length == 1)
                                date = datearr[1] + "/0" + datearr[0] + "/" + datearr[2];
                            else
                                date = datearr[1] + "/" + datearr[0] + "/" + datearr[2];
                            //  duration = Convert.ToString(DateTime.Parse(date));
                            // duration = Convert.ToString(DateTime.Parse(dr[colid]));


                            //duration = Convert.ToString(DateTime.Parse("01 / 12 / 2020"));
                            duration = Convert.ToString(DateTime.Parse(date));
                        }
                        //duration = Convert.ToString(DateTime.FromOADate(Convert.ToDouble(dr[colid])));//(g[f]) Convert.ToString(dr[colid++]); //  reporting period(duration)
                        // f = f + 3;
                        else
                            duration = Convert.ToString(dr[colid]);

                        //duration = Convert.ToString(dr[colid++]); // reporting period(duration)

                        try
                        {
                            if (string.IsNullOrEmpty(dr1[colid]))
                                dr1[colid] = "0";
                            amount = Convert.ToDecimal(dr1[colid]);  //amount
                                                                     //if (amount == 0)
                                                                     //    haserror = true;
                        }
                        catch
                        {
                            haserror = true;
                            amount = 0;
                        }
                       
                            stockout = 0;     //stock out
                       
                            //if (string.IsNullOrEmpty(dr3[colid]))
                            //    dr3[colid] = "0";
                            instrumentDownTime = 0;    //instrumentDownTime
                       
                        //try
                        //{
                        //    adjusited = Convert.ToDecimal(dr[colid++]);     //adjusted
                        //}
                        //catch
                        //{
                        //    haserror = true;
                        adjusited = 0;
                        // }

                        ReportedData rd = null;
                        DataRow Dr = ServiceDT.NewRow();
                        if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        {
                            rd = new ReportedData
                            {
                                _rowno = rowno,
                                _categoryName = categoryName,
                                //  _siteName = siteName,
                                _testname = testName,
                                _duration = duration,
                                _amount = amount,
                                _stockout = stockout,
                                _instrumentDownTime = instrumentDownTime
                                //  rowno,_categoryname categoryName, productName, duration, amount, stockout, instrumentDownTime
                            };// (categoryName, rowno, testName, duration, amount, stockout, instrumentDownTime);
                            if (cName != categoryName)
                            {
                                if (!string.IsNullOrEmpty(categoryName))
                                {
                                    fcategory = ctx.ForecastCategory.Where(b => b.ForecastId == _forecastInfo.ForecastID && b.CategoryName == categoryName).FirstOrDefault();
                                }
                                else
                                    fcategory = null;
                                cName = categoryName;
                            }

                            if (fcategory != null)
                                rd._category = fcategory;
                            else
                            {
                                rd._hasError = true;
                                errorDescription = errorDescription + " Category Doesn't Exist";
                                //  errormsg = errormsg + " Category Doesn't Exist";
                                //   dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                                //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                            }






                        }
                        else
                        {
                            rd = new ReportedData
                            {
                                _rowno = rowno,
                                _regionName = regionName,
                                _siteName = siteName,
                                _testname = testName,
                                _duration = duration,
                                _amount = amount,
                                _stockout = stockout,
                                _instrumentDownTime = instrumentDownTime

                            };//(ro(regionName, rowno, siteName, testName, duration, amount, stockout, instrumentDownTime);

                            if (rName != regionName)
                            {
                                if (!string.IsNullOrEmpty(regionName))
                                {
                                    //region = ctx.Region.Where(b => b.RegionName == regionName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                    //if (region == null)
                                    //{
                                    region = regions.Where(b => b.RegionName == regionName ).FirstOrDefault();

                                    //   }

                                }
                                else
                                    region = null;

                            }

                            if (region != null)
                            {
                                rd._region = region;
                                rName = regionName;
                                if (sName != siteName)
                                {
                                    if (!string.IsNullOrEmpty(siteName))
                                    {
                                        //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                        //if (site == null)
                                        //{
                                        site = sites.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.CountryId == countryid || b.UserId == userid)).FirstOrDefault();

                                        //  }
                                    }
                                    else
                                        site = null;
                                    //sName = siteName;
                                }
                                if (site != null)
                                {
                                    rd._site = site;
                                    sName = siteName;
                                }
                                else
                                {
                                    //if (sName != siteName)
                                    //{
                                    //    if (!string.IsNullOrEmpty(siteName))
                                    //    {
                                    //        Site ST = new Site
                                    //        {
                                    //            CountryId = countryid,
                                    //            UserId = userid,
                                    //            SiteName = siteName,
                                    //            regionid = region.RegionID
                                    //        };
                                    //        ctx.Site.Add(ST);
                                    //        ctx.SaveChanges();
                                    //        rd._site = ST;
                                    //        site = ST;
                                    //    }
                                    //    sName = siteName;
                                    //}
                                    //else
                                    //{
                                    //    rd._site = site;
                                    //    //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                    //    //if (site == null)
                                    //    //{
                                    //    //    site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && b.CountryId == ctx.User.Where(c => c.Id == userid).Select(iz => iz.CountryId).FirstOrDefault()).FirstOrDefault();

                                    //    //}
                                    //}
                                    //haserror = true;
                                    //errorDescription = errorDescription + " Site Doesn't Exist";
                                    ////dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
                                    //      changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Site Doesn't Exist", Color.Red);
                                    //}
                                }
                            }
                            else
                            {
                                //if (rName != regionName)
                                //{

                                //    if (!string.IsNullOrEmpty(regionName))
                                //    {
                                //        Region Rg = new Region
                                //        {
                                //            RegionName = regionName,
                                //            UserId = userid,
                                //            CountryId = countryid
                                //        };
                                //        ctx.Region.Add(Rg);
                                //        ctx.SaveChanges();
                                //        rName = regionName;
                                //        rd._region = Rg;
                                //        if (sName != siteName)
                                //        {
                                //            if (!string.IsNullOrEmpty(siteName))
                                //            {

                                //                Site ST = new Site
                                //                {
                                //                    CountryId = countryid,
                                //                    UserId = userid,
                                //                    SiteName = siteName,
                                //                    regionid = Rg.RegionID
                                //                };
                                //                ctx.Site.Add(ST);
                                //                ctx.SaveChanges();
                                //                rd._site = site;
                                //            }

                                //        }
                                //        else
                                //        {
                                //            rd._site = site;
                                //        }
                                //        sName = siteName;





                                //    }
                                //}
                                //haserror = true;
                                // errorDescription = errorDescription + " Region Doesn't Exist";
                                //dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
                                //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Region Doesn't Exist", Color.Red);

                            }
                            // Dr["Cat


                        }



                        if (tName != testName)
                        {
                            if (!string.IsNullOrEmpty(testName))
                                test = tests.Where(b => b.TestName == testName).FirstOrDefault();
                            else
                                test = null;
                            tName = testName;
                        }

                        if (test != null)
                            rd._test = test;
                        else
                        {
                            haserror = true;
                            errorDescription = errorDescription + " Test Doesn't Exist";
                            //    errormsg = errormsg + " Test Doesn't Exist";
                            //  dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                            //   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                        }
                        if (haserror == false)
                        {

                            Dr["CategoryName"] = rd._categoryName; ;
                            Dr["RegionName"] = rd._regionName;
                            Dr["SiteName"] = rd._siteName;
                            Dr["testName"] = rd._testname;
                            rd._hasError = haserror;
                            rd.errorDescription = errorDescription;

                            if (!Utility.Utility.IsDateTime(rd._duration))
                            {

                                try
                                {
                                    DateTime dd = Utility.Utility.DurationToDateTime(rd._duration);
                                    if (rd._duration.StartsWith("Q") && (country.Period == "Yearly"))
                                    {

                                        rd._duration = dd.Year.ToString();
                                        Dr["Duration"] = Utility.Utility.DatetimeToDurationStr(country.Period, dd);
                                    }
                                    else
                                        Dr["Duration"] = rd._duration;
                                }
                                catch (Exception ex)
                                {
                                    Dr["Duration"] = rd._duration;
                                    rd._hasError = true;
                                }

                            }
                            else
                            {

                                string datestr = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
                                if (!rd._duration.StartsWith("Q"))
                                {
                                    rd._duration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
                                    if (country.Period == "Yearly")
                                    {
                                        Dr["Duration"] = datestr;

                                    }
                                    else
                                    {
                                        Dr["Duration"] = rd._duration;

                                    }
                                }
                                else
                                {
                                    {
                                        Dr["Duration"] = datestr;

                                    }


                                }

                            }



                            Dr["Amount"] = rd._amount.ToString();
                            Dr["StockOut"] = rd._stockout.ToString();
                            Dr["InstrumentDownTime"] = rd._instrumentDownTime.ToString();
                            //if (rd.HasError == true && rd.ErrorDescription == "")
                            //    rd.ErrorDescription = " Product Consumed Required ";

                            //     li.SubItems.Add(rd.ErrorDescription.ToString());
                            if (Utility.Utility.validDate(rd._duration, country.Period))
                                rd._hasError = true;

                            if (rd._hasError == true && rd.errorDescription == "")
                                rd.errorDescription = " Product Consumed Required ";
                            Dr["Description"] = rd.errorDescription.ToString();
                            if (rd._site != null)
                                Dr["SiteID"] = rd._site.SiteID;
                            else
                                Dr["SiteID"] = 0;
                            if (rd._test != null)
                                Dr["testID"] = rd._test.TestID;
                            else
                                Dr["testID"] = 0;
                            if (_forecastInfo.DataUsage == "DATA_USAGE3")
                            {
                                Dr["CatID"] = rd._category.CategoryId;
                            }
                            else
                            {
                                Dr["CatID"] = 0;
                            }
                            Dr["Duration1"] = rd._duration;
                            Dr["haserror"] = rd._hasError;
                            Dr["forecastid"] = id;
                            Dr["No"] = rd._rowno.ToString();
                            ServiceDT.Rows.Add(Dr);
                        }
                        // rdlist.Add(rd);
                        colid++;
                        errorDescription = "";
                    }
                    while (colid < g.Length && g[colid].ToString() != "");// dr.ItemArray.Length / ds.Tables[0].Rows.Count);
                }

            }
            if (errormsg != "")
            {
                // label7.Text = errormsg;

            }
            //else
            //{
            //    label7.Text = "Successsfully imported";
            //}

            //return rdlist;

        }
        //private void GetDataServiceRow(List<string[]> jArray, int id, int userid)
        //{
        //    string categoryName = null;
        //    string regionName;
        //    string siteName;
        //    string testName;
        //    string duration;
        //    decimal amount;
        //    int stockout;
        //    int instrumentDownTime;
        //    decimal adjusited;
        //    int rowno = 0;
        //    bool haserror;
        //    string cName = "";
        //    string rName = "";
        //    string sName = "";
        //    string tName = "";
        //    _forecastInfo = ctx.ForecastInfo.Find(id);
        //    ForecastCategory fcategory = null;
        //    Models.Region region = null;
        //    Site site = null;
        //    //MasterProduct product = null;
        //    Test test = null;
        //    string errorDescription = "";
        //    int errornos = 0;
        //    string errormsg = "";
        //    IList<ReportedData> rdlist = new List<ReportedData>();
        //    string[] dr = jArray[0];
        //    DataTable dtexcelcolor = new DataTable();
        //    dtexcelcolor.Columns.Add("Sheet");
        //    dtexcelcolor.Columns.Add("Row");
        //    dtexcelcolor.Columns.Add("Error");
        //    dtexcelcolor.Columns.Add("Color");
        //    dtexcelcolor.AcceptChanges();
        //    //foreach (DataRow dr in ds.Tables[0].Rows)
        //    //{
        //    var countryid = ctx.User.Find(userid).CountryId;
        //    var country = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault();
        //    var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>

        //       x.Id

        //  ).FirstOrDefault();
        //    for (int i = 1; i < jArray.Count; i = i + 4)//ds.Tables[0].Rows.Count
        //    {
        //        if (jArray[i].Length > 0)
        //        {

        //            errormsg = "";
        //            rowno++;
        //            haserror = false;
        //            string[] dr1 = jArray[i];
        //            string[] dr2 = jArray[i + 1];
        //            string[] dr3 = jArray[i + 2];
        //            string[] dr4 = jArray[i + 3];
        //            string[] g = jArray[0];
        //            // int colid = 0;
        //            int f = 3;
        //            int colid;//0 
        //            if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                colid = 3;
        //            else
        //                colid = 4;

        //            int noofcolumn = 0;
        //            noofcolumn = jArray[i].Length - 1;
        //            bool newerror = false;

        //            do
        //            {
        //                if (dr1[noofcolumn] == "")
        //                    dr1[noofcolumn] = "0";
        //                Decimal amount1 = Convert.ToDecimal(dr1[noofcolumn]);
        //                if (Convert.ToDecimal(dr1[noofcolumn]) == 0)
        //                {
        //                    newerror = true;
        //                    break;
        //                }

        //                noofcolumn--;
        //            }
        //            while (noofcolumn >= jArray[i].Length - 3);
        //            if (newerror == true)
        //            {
        //                errornos++;
        //                if (errornos > 20)
        //                {
        //                    //  label7.Text = "";
        //                    //MessageBox.Show("There too many problem with Instrument data, please troubleshoot and try to import again.", "Importing", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    throw new Exception("There too many problem with Service data, please troubleshoot and try to import again.");
        //                }
        //                errormsg = "The Test is not performed in last three months";
        //                //dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //                ////   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //                //errormsg = "Please Check excel Some test are not performed in recent last three months";

        //                continue;

        //            }
        //            else
        //            {
        //                //if (_forecastInfo.DatausageEnum == DataUsageEnum.DATA_USAGE3)
        //                //    colid = 3;
        //                //else
        //                //    colid = 4;

        //            }

        //            //  label7.Text = "Importing Data.....................";


        //            do
        //            {
        //                regionName = "";
        //                siteName = "";
        //                errormsg = "";
        //                if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                {
        //                    categoryName = Convert.ToString(dr1[0]).Trim();//(dr[colid++])
        //                    testName = Convert.ToString(dr1[1]).Trim();   //(dr[colid++])testName
        //                }
        //                else
        //                {
        //                    regionName = Convert.ToString(dr1[0]).Trim(); //(dr[colid++]) region name
        //                    siteName = Convert.ToString(dr1[1]).Trim();           //(dr[colid++]) site name
        //                    testName = Convert.ToString(dr1[2]).Trim();   //(dr[colid++])testName 
        //                }
        //                string date;
        //                string[] datearr;
        //                if (country.Period != "Monthly" || country.Period != "Bimonthly")
        //                {

        //                    datearr = dr[colid].Split("/");
        //                    if (datearr[0].Length == 1)
        //                        date = datearr[1] + "/0" + datearr[0] + "/" + datearr[2];
        //                    else
        //                        date = datearr[1] + "/" + datearr[0] + "/" + datearr[2];
        //                    //  duration = Convert.ToString(DateTime.Parse(date));
        //                    // duration = Convert.ToString(DateTime.Parse(dr[colid]));


        //                    //duration = Convert.ToString(DateTime.Parse("01 / 12 / 2020"));
        //                    duration = Convert.ToString(DateTime.Parse(date));
        //                }
        //                //duration = Convert.ToString(DateTime.FromOADate(Convert.ToDouble(dr[colid])));//(g[f]) Convert.ToString(dr[colid++]); //  reporting period(duration)
        //                // f = f + 3;
        //                else
        //                    duration = Convert.ToString(dr[colid]);

        //                //duration = Convert.ToString(dr[colid++]); // reporting period(duration)

        //                try
        //                {
        //                    if (string.IsNullOrEmpty(dr1[colid]))
        //                        dr1[colid] = "0";
        //                    amount = Convert.ToDecimal(dr1[colid]);  //amount
        //                                                             //if (amount == 0)
        //                                                             //    haserror = true;
        //                }
        //                catch
        //                {
        //                    haserror = true;
        //                    amount = 0;
        //                }
        //                try
        //                {
        //                    if (string.IsNullOrEmpty(dr2[colid]))
        //                        dr2[colid] = "0";
        //                    stockout = Convert.ToInt32(dr2[colid]);     //stock out
        //                }
        //                catch
        //                {
        //                    haserror = true;
        //                    stockout = 0;
        //                }
        //                try
        //                {
        //                    if (string.IsNullOrEmpty(dr3[colid]))
        //                        dr3[colid] = "0";
        //                    instrumentDownTime = Convert.ToInt32(dr3[colid]);     //instrumentDownTime
        //                }
        //                catch
        //                {
        //                    haserror = true;
        //                    instrumentDownTime = 0;
        //                }
        //                //try
        //                //{
        //                //    adjusited = Convert.ToDecimal(dr[colid++]);     //adjusted
        //                //}
        //                //catch
        //                //{
        //                //    haserror = true;
        //                adjusited = 0;
        //                // }

        //                ReportedData rd = null;
        //                DataRow Dr = ServiceDT.NewRow();
        //                if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                {
        //                    rd = new ReportedData(categoryName, rowno, testName, duration, amount, stockout, instrumentDownTime);
        //                    if (cName != categoryName)
        //                    {
        //                        if (!string.IsNullOrEmpty(categoryName))
        //                        {
        //                            fcategory = ctx.ForecastCategory.Where(b => b.ForecastId == _forecastInfo.ForecastID && b.CategoryName == categoryName).FirstOrDefault();
        //                        }
        //                        else
        //                            fcategory = null;
        //                        cName = categoryName;
        //                    }

        //                    if (fcategory != null)
        //                        rd._category = fcategory;
        //                    else
        //                    {
        //                        rd._hasError = true;
        //                        errorDescription = errorDescription + " Category Doesn't Exist";
        //                        //  errormsg = errormsg + " Category Doesn't Exist";
        //                        //   dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //                        //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //                    }






        //                }
        //                else
        //                {
        //                    rd = new ReportedData(regionName, rowno, siteName, testName, duration, amount, stockout, instrumentDownTime);

        //                    if (rName != regionName)
        //                    {
        //                        if (!string.IsNullOrEmpty(regionName))
        //                        {
        //                            //region = ctx.Region.Where(b => b.RegionName == regionName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
        //                            //if (region == null)
        //                            //{
        //                            region = ctx.Region.Where(b => b.RegionName == regionName && b.CountryId == countryid).FirstOrDefault();

        //                            //   }

        //                        }
        //                        else
        //                            region = null;

        //                    }

        //                    if (region != null)
        //                    {
        //                        rd._region = region;
        //                        rName = regionName;
        //                        if (sName != siteName)
        //                        {
        //                            if (!string.IsNullOrEmpty(siteName))
        //                            {
        //                                //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
        //                                //if (site == null)
        //                                //{
        //                                site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && b.CountryId == countryid).FirstOrDefault();

        //                                //  }
        //                            }
        //                            else
        //                                site = null;
        //                            sName = siteName;
        //                        }
        //                        if (site != null)
        //                        {
        //                            rd._site = site;
        //                            sName = siteName;
        //                        }
        //                        else
        //                        {
        //                            if (sName != siteName)
        //                            {
        //                                if (!string.IsNullOrEmpty(siteName))
        //                                {
        //                                    Site ST = new Site
        //                                    {
        //                                        CountryId = countryid,
        //                                        UserId = userid,
        //                                        SiteName = siteName,
        //                                        regionid = region.RegionID
        //                                    };
        //                                    ctx.Site.Add(ST);
        //                                    ctx.SaveChanges();
        //                                    rd._site = ST;
        //                                    site = ST;
        //                                }
        //                                sName = siteName;
        //                            }
        //                            else
        //                            {
        //                                rd._site = site;
        //                                //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
        //                                //if (site == null)
        //                                //{
        //                                //    site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && b.CountryId == ctx.User.Where(c => c.Id == userid).Select(iz => iz.CountryId).FirstOrDefault()).FirstOrDefault();

        //                                //}
        //                            }
        //                            //haserror = true;
        //                            //errorDescription = errorDescription + " Site Doesn't Exist";
        //                            ////dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
        //                            //      changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Site Doesn't Exist", Color.Red);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (rName != regionName)
        //                        {

        //                            if (!string.IsNullOrEmpty(regionName))
        //                            {
        //                                Region Rg = new Region
        //                                {
        //                                    RegionName = regionName,
        //                                    UserId = userid,
        //                                    CountryId = countryid
        //                                };
        //                                ctx.Region.Add(Rg);
        //                                ctx.SaveChanges();
        //                                rName = regionName;
        //                                rd._region = Rg;
        //                                if (sName != siteName)
        //                                {
        //                                    if (!string.IsNullOrEmpty(siteName))
        //                                    {

        //                                        Site ST = new Site
        //                                        {
        //                                            CountryId = countryid,
        //                                            UserId = userid,
        //                                            SiteName = siteName,
        //                                            regionid = Rg.RegionID
        //                                        };
        //                                        ctx.Site.Add(ST);
        //                                        ctx.SaveChanges();
        //                                        rd._site = site;
        //                                    }

        //                                }
        //                                else
        //                                {
        //                                    rd._site = site;
        //                                }
        //                                sName = siteName;





        //                            }
        //                        }
        //                        //haserror = true;
        //                        // errorDescription = errorDescription + " Region Doesn't Exist";
        //                        //dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
        //                        //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Region Doesn't Exist", Color.Red);

        //                    }
        //                    // Dr["Cat


        //                }



        //                if (tName != testName)
        //                {
        //                    if (!string.IsNullOrEmpty(testName))
        //                        test = ctx.Test.Where(b => b.TestName == testName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
        //                    else
        //                        test = null;
        //                    tName = testName;
        //                }

        //                if (test != null)
        //                    rd._test = test;
        //                else
        //                {
        //                    haserror = true;
        //                    errorDescription = errorDescription + " Test Doesn't Exist";
        //                    //    errormsg = errormsg + " Test Doesn't Exist";
        //                    //  dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //                    //   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //                }
        //                if (haserror == false)
        //                {

        //                    Dr["CategoryName"] = rd._categoryName; ;
        //                    Dr["RegionName"] = rd._regionName;
        //                    Dr["SiteName"] = rd._siteName;
        //                    Dr["testName"] = rd._testname;
        //                    rd._hasError = haserror;
        //                    rd.errorDescription = errorDescription;

        //                    if (!Utility.Utility.IsDateTime(rd._duration))
        //                    {

        //                        try
        //                        {
        //                            DateTime dd = Utility.Utility.DurationToDateTime(rd._duration);
        //                            if (rd._duration.StartsWith("Q") && (country.Period == "Yearly"))
        //                            {

        //                                rd._duration = dd.Year.ToString();
        //                                Dr["Duration"] = Utility.Utility.DatetimeToDurationStr(country.Period, dd);
        //                            }
        //                            else
        //                                Dr["Duration"] = rd._duration;
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            Dr["Duration"] = rd._duration;
        //                            rd._hasError = true;
        //                        }

        //                    }
        //                    else
        //                    {

        //                        string datestr = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
        //                        if (!rd._duration.StartsWith("Q"))
        //                        {
        //                            rd._duration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
        //                            if (country.Period == "Yearly")
        //                            {
        //                                Dr["Duration"] = datestr;

        //                            }
        //                            else
        //                            {
        //                                Dr["Duration"] = rd._duration;

        //                            }
        //                        }
        //                        else
        //                        {
        //                            {
        //                                Dr["Duration"] = datestr;

        //                            }


        //                        }

        //                    }



        //                    Dr["Amount"] = rd._amount.ToString();
        //                    Dr["StockOut"] = rd._stockout.ToString();
        //                    Dr["InstrumentDownTime"] = rd._instrumentDownTime.ToString();
        //                    //if (rd.HasError == true && rd.ErrorDescription == "")
        //                    //    rd.ErrorDescription = " Product Consumed Required ";

        //                    //     li.SubItems.Add(rd.ErrorDescription.ToString());
        //                    if (Utility.Utility.validDate(rd._duration, country.Period))
        //                        rd._hasError = true;

        //                    if (rd._hasError == true && rd.errorDescription == "")
        //                        rd.errorDescription = " Product Consumed Required ";
        //                    Dr["Description"] = rd.errorDescription.ToString();
        //                    if (rd._site != null)
        //                        Dr["SiteID"] = rd._site.SiteID;
        //                    else
        //                        Dr["SiteID"] = 0;
        //                    if (rd._test != null)
        //                        Dr["testID"] = rd._test.TestID;
        //                    else
        //                        Dr["testID"] = 0;
        //                    if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                    {
        //                        Dr["CatID"] = rd._category.CategoryId;
        //                    }
        //                    else
        //                    {
        //                        Dr["CatID"] = 0;
        //                    }
        //                    Dr["Duration1"] = rd._duration;
        //                    Dr["haserror"] = rd._hasError;
        //                    Dr["forecastid"] = id;
        //                    Dr["No"] = rd._rowno.ToString();
        //                    ServiceDT.Rows.Add(Dr);
        //                }
        //                // rdlist.Add(rd);
        //                colid++;
        //                errorDescription = "";
        //            }
        //            while (colid < g.Length && g[colid].ToString() != "");// dr.ItemArray.Length / ds.Tables[0].Rows.Count);
        //        }

        //    }
        //    if (errormsg != "")
        //    {
        //        // label7.Text = errormsg;

        //    }
        //    //else
        //    //{
        //    //    label7.Text = "Successsfully imported";
        //    //}

        //    //return rdlist;

        //}
        public Array getimporteddata(int id)
        {

            Array array;
            var forecastinfo = ctx.ForecastInfo.Find(id);



            if (forecastinfo.DataUsage != "DATA_USAGE3")
            {

                array = ctx.ForecastSiteProduct.Join(ctx.MasterProduct, b => b.ProductID, c => c.ProductID, (b, c) => new { b, c })
                    .Join(ctx.ForecastSite, d => d.b.ForecastSiteID, e => e.Id, (d, e) => new { d, e })
                    .Join(ctx.Site, f => f.e.SiteId, g => g.SiteID, (f, g) => new { f, g })
                    .Join(ctx.Region, h => h.g.regionid, i => i.RegionID, (h, i) => new { h, i }).Where(s => s.h.f.e.ForecastInfoId == id)
                    .Select(x => new
                    {
                        No = 0,
                        CategoryName = "",
                        x.i.RegionName,
                        x.h.g.SiteName,
                        ProName = x.h.f.d.c.ProductName,
                        Duration = x.h.f.d.b.CDuration,
                        durationdatetime=x.h.f.d.b.DurationDateTime,
                        Amount = x.h.f.d.b.AmountUsed,
                        x.h.f.d.b.StockOut,
                        InstrumentDownTime = x.h.f.d.b.InstrumentDowntime,
                        Description = "",
                        x.h.g.SiteID,
                        ProID = x.h.f.d.c.ProductID,
                        CatID = 0,
                        Duration1 = x.h.f.d.b.CDuration,
                        haserror = false,
                        forecastid = id

                    }).OrderBy(y=>y.ProID).ThenBy(z=>z.durationdatetime).ToArray();
            }
            else
            {
                array = ctx.ForecastCategoryProduct.Join(ctx.MasterProduct, b => b.ProductID, c => c.ProductID, (b, c) => new { b, c })
                 .Join(ctx.ForecastCategory, d => d.b.CategoryID, e => e.CategoryId, (d, e) => new { d, e })
       .Where(s => s.e.ForecastId == id)
                 .Select(x => new
                 {
                     No = 0,
                     x.e.CategoryName,
                     RegionName = "",
                     SiteName = "",
                     ProName = x.d.c.ProductName,
                     Duration = x.d.b.CDuration,

                     durationdatetime = x.d.b.DurationDateTime,
                     Amount = x.d.b.AmountUsed,
                     x.d.b.StockOut,
                     InstrumentDownTime = x.d.b.InstrumentDowntime,
                     Description = "",
                     SiteID = 0,
                     ProID = x.d.c.ProductID,
                     CatID = x.e.CategoryId,
                     Duration1 = x.d.b.CDuration,
                     haserror = false,
                     forecastid = id

                 }).OrderBy(y => y.ProID).ThenBy(z => z.durationdatetime).ToArray();
            }

            return array;

        }
        public Array getimportedservicedata(int id)
        {

            Array array;
            var forecastinfo = ctx.ForecastInfo.Find(id);



            if (forecastinfo.DataUsage != "DATA_USAGE3")
            {

                array = ctx.ForecastSiteTest.Join(ctx.Test, b => b.TestID, c => c.TestID, (b, c) => new { b, c })
                    .Join(ctx.ForecastSite, d => d.b.ForecastSiteID, e => e.Id, (d, e) => new { d, e })
                    .Join(ctx.Site, f => f.e.SiteId, g => g.SiteID, (f, g) => new { f, g })
                    .Join(ctx.Region, h => h.g.regionid, i => i.RegionID, (h, i) => new { h, i }).Where(s => s.h.f.e.ForecastInfoId == id)
                    .Select(x => new
                    {
                        No = 0,
                        CategoryName = "",
                        x.i.RegionName,
                        x.h.g.SiteName,
                        x.h.f.d.c.TestName,
                        Duration = x.h.f.d.b.CDuration,
                        durationdatetime = x.h.f.d.b.DurationDateTime,
                        Amount = x.h.f.d.b.AmountUsed,
                        x.h.f.d.b.StockOut,
                        InstrumentDownTime = x.h.f.d.b.InstrumentDowntime,
                        Description = "",
                        x.h.g.SiteID,
                        x.h.f.d.c.TestID,
                        CatID = 0,
                        Duration1 = x.h.f.d.b.CDuration,
                        haserror = false,
                        forecastid = id

                    }).OrderBy(y => y.TestID).ThenBy(z=>z.durationdatetime).ToArray();
            }
            else
            {
                array = ctx.ForecastCategoryTest.Join(ctx.Test, b => b.TestID, c => c.TestID, (b, c) => new { b, c })
                 .Join(ctx.ForecastCategory, d => d.b.CategoryID, e => e.CategoryId, (d, e) => new { d, e })
       .Where(s => s.e.ForecastId == id)
                 .Select(x => new
                 {
                     No = 0,
                     x.e.CategoryName,
                     RegionName = "",
                     SiteName = "",
                     x.d.c.TestName,
                     Duration = x.d.b.CDuration,
                     durationdatetime = x.d.b.DurationDateTime,
                     Amount = x.d.b.AmountUsed,
                     x.d.b.StockOut,
                     InstrumentDownTime = x.d.b.InstrumentDowntime,
                     Description = "",
                     SiteID = 0,
                     x.d.c.TestID,
                     CatID = x.e.CategoryId,
                     Duration1 = x.d.b.CDuration,
                     haserror = false,
                     forecastid = id

                 }).OrderBy(y => y.TestID).ThenBy(z => z.durationdatetime).ToArray();
            }

            return array;

        }
        //public string importservicenew(List<string[]> jArray, int id, int userid)
        //{
        //    //   DataTable DT = new DataTable();
        //    ForecastSite fs = new ForecastSite();
        //    ForecastCategory fcat = new ForecastCategory();
        //    List<receivereportdata> _rdata = new List<receivereportdata>();
        //    string categoryName = null;
        //    string regionName;
        //    string siteName;
        //    string testName;
        //    string duration;
        //    decimal amount;
        //    int stockout;
        //    int instrumentDownTime;
        //    decimal adjusited;
        //    int rowno = 0;
        //    bool haserror;
        //    string cName = "";
        //    string rName = "";
        //    string sName = "";
        //    string tName = "";
        //    string res = "";
        //    _forecastInfo = ctx.ForecastInfo.Find(id);
        //    ForecastCategory fcategory = null;
        //    bool isduplicate = false;
        //    Models.Region region = null;
        //    Site site = null;
        //    //MasterProduct product = null;
        //    Test test = null;
        //    string errorDescription = "";
        //    int errornos = 0;
        //    string errormsg = "";
        //    IList<ReportedData> rdlist = new List<ReportedData>();
        //    string[] dr = jArray[0];
        //    DataTable dtexcelcolor = new DataTable();
        //    dtexcelcolor.Columns.Add("Sheet");
        //    dtexcelcolor.Columns.Add("Row");
        //    dtexcelcolor.Columns.Add("Error");
        //    dtexcelcolor.Columns.Add("Color");
        //    dtexcelcolor.AcceptChanges();
        //    //foreach (DataRow dr in ds.Tables[0].Rows)
        //    //{
        //    var countryid = ctx.User.Find(userid).CountryId;
        //    var country = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault();
        //    var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>x.Id).FirstOrDefault();
        //    string fail = "";
        //    List<Test> tests = new List<Test>();
        //    tests = ctx.Test.Where(b => b.UserId == userid || b.UserId == adminuserid).ToList();
        //    List<Region> regions = new List<Region>();
        //    regions = ctx.Region.Where(b => b.CountryId == countryid || b.UserId == userid).ToList();
        //    List<Site> sites = new List<Site>();
        //    sites = ctx.Site.Where(b => b.CountryId == countryid || b.UserId == userid).ToList();
        //    for (int i = 1; i < jArray.Count; i++)//ds.Tables[0].Rows.Count
        //    {
        //        if (jArray[i].Length > 0)
        //        {

        //            errormsg = "";
        //            rowno++;
        //            haserror = false;
        //            string[] dr1 = jArray[i];
        //            //string[] dr2 = jArray[i + 1];
        //            //string[] dr3 = jArray[i + 2];
        //            //string[] dr4 = jArray[i + 3];
        //            string[] g = jArray[0];
        //            // int colid = 0;
        //            int f = 3;
        //            int colid;//0 
        //            if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                colid = 2;
        //            else
        //                colid = 3;

        //            int noofcolumn = 0;
        //            //noofcolumn = jArray[i].Length - 1;
        //            //bool newerror = false;

        //            //do
        //            //{
        //            //    if (dr1[noofcolumn] == "")
        //            //        dr1[noofcolumn] = "0";
        //            //    Decimal amount1 = Convert.ToDecimal(dr1[noofcolumn]);
        //            //    if (Convert.ToDecimal(dr1[noofcolumn]) == 0)
        //            //    {
        //            //        newerror = true;
        //            //        break;
        //            //    }

        //            //    noofcolumn--;
        //            //}
        //            //while (noofcolumn >= jArray[i].Length - 3);
        //            //if (newerror == true)
        //            //{
        //            //    errornos++;
        //            //    if (errornos > 20)
        //            //    {
        //            //        //  label7.Text = "";
        //            //        //MessageBox.Show("There too many problem with Instrument data, please troubleshoot and try to import again.", "Importing", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            //        throw new Exception("There too many problem with Service data, please troubleshoot and try to import again.");
        //            //    }
        //            //    errormsg = "The Test is not performed in last three months";
        //            //    //dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //            //    ////   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //            //    //errormsg = "Please Check excel Some test are not performed in recent last three months";

        //            //    continue;

        //            //}
        //            //else
        //            //{
        //            //    //if (_forecastInfo.DatausageEnum == DataUsageEnum.DATA_USAGE3)
        //            //    //    colid = 3;
        //            //    //else
        //            //    //    colid = 4;

        //            //}

        //            //  label7.Text = "Importing Data.....................";


        //            do
        //            {
        //                regionName = "";
        //                siteName = "";
        //                errormsg = "";
        //                if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                {
        //                    categoryName = Convert.ToString(dr1[0]).Trim();//(dr[colid++])
        //                    testName = Convert.ToString(dr1[1]).Trim();   //(dr[colid++])testName
        //                }
        //                else
        //                {
        //                    regionName = Convert.ToString(dr1[0]).Trim(); //(dr[colid++]) region name
        //                    siteName = Convert.ToString(dr1[1]).Trim();           //(dr[colid++]) site name
        //                    testName = Convert.ToString(dr1[2]).Trim();   //(dr[colid++])testName 
        //                }
        //                string date;
        //                string[] datearr;
        //                if (country.Period != "Monthly" || country.Period != "Bimonthly")
        //                {

        //                    datearr = dr[colid].Split("/");
        //                    if (datearr[0].Length == 1)
        //                        date = datearr[1] + "/0" + datearr[0] + "/" + datearr[2];
        //                    else
        //                        date = datearr[1] + "/" + datearr[0] + "/" + datearr[2];
        //                    //  duration = Convert.ToString(DateTime.Parse(date));
        //                    // duration = Convert.ToString(DateTime.Parse(dr[colid]));


        //                    //duration = Convert.ToString(DateTime.Parse("01 / 12 / 2020"));
        //                    duration = Convert.ToString(DateTime.Parse(date));
        //                }
        //                //duration = Convert.ToString(DateTime.FromOADate(Convert.ToDouble(dr[colid])));//(g[f]) Convert.ToString(dr[colid++]); //  reporting period(duration)
        //                // f = f + 3;
        //                else
        //                    duration = Convert.ToString(dr[colid]);

        //                //duration = Convert.ToString(dr[colid++]); // reporting period(duration)

        //                try
        //                {
        //                    if (string.IsNullOrEmpty(dr1[colid]))
        //                        dr1[colid] = "0";
        //                    amount = Convert.ToDecimal(dr1[colid]);  //amount
        //                                                             //if (amount == 0)
        //                                                             //    haserror = true;
        //                }
        //                catch
        //                {
        //                    haserror = true;
        //                    amount = 0;
        //                }

        //                    stockout = 0;

        //                    instrumentDownTime = 0;    //instrumentDownTime

        //                //try
        //                //{
        //                //    adjusited = Convert.ToDecimal(dr[colid++]);     //adjusted
        //                //}
        //                //catch
        //                //{
        //                //    haserror = true;
        //                adjusited = 0;
        //                // }

        //                ReportedData rd = null;
        //                DataRow Dr = ServiceDT.NewRow();
        //                if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                {
        //                    rd = new ReportedData
        //                    {
        //                        _rowno = rowno,
        //                        _categoryName = categoryName,
        //                        _testname = testName,
        //                        _duration = duration,
        //                        _amount = amount,
        //                        _stockout = stockout,
        //                        _instrumentDownTime = instrumentDownTime
        //                    };
        //                    if (cName != categoryName)
        //                    {
        //                        if (!string.IsNullOrEmpty(categoryName))
        //                        {
        //                            fcategory = ctx.ForecastCategory.Where(b => b.ForecastId == _forecastInfo.ForecastID && b.CategoryName == categoryName).FirstOrDefault();
        //                        }
        //                        else
        //                            fcategory = null;
        //                        cName = categoryName;
        //                    }

        //                    if (fcategory != null)
        //                        rd._category = fcategory;
        //                    else
        //                    {
        //                        rd._hasError = true;
        //                        errorDescription = errorDescription + " Category Doesn't Exist";
        //                        //  errormsg = errormsg + " Category Doesn't Exist";
        //                        //   dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //                        //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //                    }






        //                }
        //                else
        //                {
        //                    rd = new ReportedData
        //                    {
        //                        _rowno = rowno,
        //                        _regionName = regionName,
        //                        _siteName = siteName,
        //                        _testname =testName,
        //                        _duration = duration,
        //                        _amount = amount,
        //                        _stockout = stockout,
        //                        _instrumentDownTime = instrumentDownTime
        //                    };

        //                    if (rName != regionName)
        //                    {

        //                            region = regions.Where(b => b.RegionName.ToUpper() == regionName.ToUpper() ).FirstOrDefault();

        //                            //   }



        //                    }

        //                    if (region != null)
        //                    {
        //                        rd._region = region;
        //                        rName = regionName;
        //                        if (sName != siteName)
        //                        {
        //                            //if (!string.IsNullOrEmpty(siteName))
        //                            //{
        //                                //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
        //                                //if (site == null)
        //                                //{
        //                                site = sites.Where(b => b.SiteName.Trim().ToUpper() == siteName.Trim().ToUpper() && b.regionid == region.RegionID ).FirstOrDefault();

        //                            //    //  }
        //                            //}
        //                            //else
        //                            //    site = null;
        //                            //sName = siteName;
        //                        }
        //                        if (site != null)
        //                        {
        //                            rd._site = site;
        //                            sName = siteName;
        //                        }
        //                        else
        //                        {
        //                            haserror = true;
        //                            //res = siteName + " site does not belongs to region " + regionName;
        //                            //return res;
        //                            //if (sName != siteName)
        //                            //{
        //                            //    //if (!string.IsNullOrEmpty(siteName))
        //                            //    //{
        //                            //        Site ST = new Site
        //                            //        {
        //                            //            CountryId = countryid,
        //                            //            UserId = userid,
        //                            //            SiteName = siteName,
        //                            //            regionid = region.RegionID
        //                            //        };
        //                            //        ctx.Site.Add(ST);
        //                            //        ctx.SaveChanges();
        //                            //        rd._site = ST;
        //                            //        site = ST;
        //                            //    //}
        //                            //    sName = siteName;
        //                            //}
        //                            //else
        //                            //{
        //                            //    rd._site = site;
        //                            //    //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
        //                            //    //if (site == null)
        //                            //    //{
        //                            //    //    site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && b.CountryId == ctx.User.Where(c => c.Id == userid).Select(iz => iz.CountryId).FirstOrDefault()).FirstOrDefault();

        //                            //    //}
        //                            //}
        //                            //haserror = true;
        //                            //errorDescription = errorDescription + " Site Doesn't Exist";
        //                            ////dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
        //                            //      changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Site Doesn't Exist", Color.Red);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        haserror = true;
        //                        //res = regionName + " region may not preent either the current country or for current user";
        //                        //return res;
        //                        //if (rName != regionName)
        //                        //{

        //                        //    //if (!string.IsNullOrEmpty(regionName))
        //                        //    //{
        //                        //    //    Region Rg = new Region
        //                        //    //    {
        //                        //    //        RegionName = regionName,
        //                        //    //        UserId = userid,
        //                        //    //        CountryId = countryid
        //                        //    //    };
        //                        //    //    ctx.Region.Add(Rg);
        //                        //    //    ctx.SaveChanges();
        //                        //    //    rName = regionName;
        //                        //    //    rd._region = Rg;
        //                        //    //    if (sName != siteName)
        //                        //    //    {
        //                        //    //        if (!string.IsNullOrEmpty(siteName))
        //                        //    //        {

        //                        //    //            Site ST = new Site
        //                        //    //            {
        //                        //    //                CountryId = countryid,
        //                        //    //                UserId = userid,
        //                        //    //                SiteName = siteName,
        //                        //    //                regionid = Rg.RegionID
        //                        //    //            };
        //                        //    //            ctx.Site.Add(ST);
        //                        //    //            ctx.SaveChanges();
        //                        //    //            rd._site = site;
        //                        //    //        }

        //                        //    //    }
        //                        //    //    else
        //                        //    //    {
        //                        //    //        rd._site = site;
        //                        //    //    }
        //                        //    //    sName = siteName;





        //                        //    //}
        //                        //}
        //                        ////haserror = true;
        //                        // errorDescription = errorDescription + " Region Doesn't Exist";
        //                        //dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
        //                        //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Region Doesn't Exist", Color.Red);

        //                    }
        //                    // Dr["Cat


        //                }



        //                //if (tName != testName)
        //                //{

        //                    test = tests.Where(b => b.TestName.Trim().ToUpper() == testName.Trim().ToUpper()).FirstOrDefault();

        //                    tName = testName;
        //                //test = tests.Where(b => b.TestName.Trim().ToUpper() == testName.Trim().ToUpper()).FirstOrDefault();
        //                //}
        //                if (test==null)
        //                {
        //                    haserror = true;
        //                }
        //                //  if (test != null)
        //                rd._test = test;

        //                //else
        //                //{
        //                //    haserror = true;
        //                //    errorDescription = errorDescription + " Test Doesn't Exist";
        //                    //    errormsg = errormsg + " Test Doesn't Exist";
        //                    //  dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //                    //   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //               // }
        //                if (haserror == false)
        //                {
        //                receivereportdata RD = new receivereportdata();

        //                RD.CategoryName = rd._categoryName; ;
        //                RD.RegionName= rd._regionName;
        //                RD.SiteName = rd._siteName;
        //                RD.testName = rd._testname;
        //                    //rd._hasError = haserror;
        //                    //rd.errorDescription = errorDescription;

        //                    if (!Utility.Utility.IsDateTime(rd._duration))
        //                    {

        //                        try
        //                        {
        //                            DateTime dd = Utility.Utility.DurationToDateTime(rd._duration);
        //                            if ((country.Period == "Yearly"))
        //                            {

        //                                rd._duration = dd.Year.ToString();
        //                               RD.Duration= Utility.Utility.DatetimeToDurationStr(country.Period, dd);
        //                            }
        //                            else
        //                                RD.Duration = rd._duration;
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            RD.Duration = rd._duration;
        //                            rd._hasError = true;
        //                        }

        //                    }
        //                    else
        //                    {

        //                        string datestr = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));

        //                            //rd._duration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
        //                            //if (country.Period == "Yearly")
        //                            //{
        //                                RD.Duration = datestr;

        //                            //}
        //                            //else
        //                            //{
        //                            //    RD.Duration = rd._duration;

        //                            //}


        //                    }



        //                  RD.Amount= rd._amount;
        //                  RD.StockOut= rd._stockout;
        //                  RD.InstrumentDownTime = rd._instrumentDownTime;
        //                    //if (rd.HasError == true && rd.ErrorDescription == "")
        //                    //    rd.ErrorDescription = " Product Consumed Required ";

        //                    //     li.SubItems.Add(rd.ErrorDescription.ToString());
        //                    //if (Utility.Utility.validDate(rd._duration, country.Period))
        //                    //    rd._hasError = true;

        //                    //if (rd._hasError == true && rd.errorDescription == "")
        //                    //    rd.errorDescription = " Product Consumed Required ";
        //                    //Dr["Description"] = rd.errorDescription.ToString();
        //                    //if (rd._site != null)
        //                       RD.SiteID = rd._site.SiteID;
        //                    //else
        //                    //    Dr["SiteID"] = 0;
        //                    //if (rd._test != null)
        //                        RD.testID = rd._test.TestID;
        //                    //else
        //                    //    Dr["testID"] = 0;
        //                    if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                    {
        //                     RD.CatID = rd._category.CategoryId;
        //                    }
        //                    else
        //                    {
        //                        RD.CatID = 0;
        //                    }
        //                   RD.Duration1= rd._duration;
        //                    //Dr["haserror"] = rd._hasError;
        //                  RD.forecastid = id;
        //                  RD.No= rd._rowno;
        //                    _rdata.Add(RD);
        //                }
        //                // rdlist.Add(rd);
        //                colid++;
        //                errorDescription = "";
        //            }
        //            while (colid < g.Length && g[colid].ToString() != "");// dr.ItemArray.Length / ds.Tables[0].Rows.Count);
        //        }

        //    }

        //    var testarray = ctx.ForecastTest.Where(b => b.forecastID == _forecastInfo.ForecastID).Select(b => b.TestID).ToList();
        //    var distincttest = _rdata.GroupBy(b => b.testID).Select(x => x.Key).ToList();
        //    for (int j = 0; j < testarray.Count; j++)
        //    {
        //        if (distincttest.Contains(testarray[j]) != true)
        //        {
        //            fail = fail + "," + ctx.Test.Find(testarray[j]).TestName;
        //        }

        //    }

        //    List<ForecastSite> forecastSites = new List<ForecastSite>();
        //    List<Site> sitelst = new List<Site>();
        //    List<HistoricalData> hisdata = new List<HistoricalData>();
        //    hisdata = ctx.HistoricalData.ToList();
        //    forecastSites = ctx.ForecastSite.Where(b => b.ForecastInfoId == _forecastInfo.ForecastID).ToList();

        //    if (_forecastInfo.DataUsage != "DATA_USAGE3")
        //    {
        //        // IList<ForecastSiteTest> existingFsp = new List<ForecastSiteTest>();
        //        IList<HistoricalData> existingFsp = new List<HistoricalData>();
        //        IList<ForecastSiteTest> existingFsp1 = new List<ForecastSiteTest>();
        //        var getdistinctsiteids = _rdata.GroupBy(b => b.SiteID).Select(x => x.Key).ToList();
        //        for (int i = 0; i < getdistinctsiteids.Count; i++)
        //        {
        //            fs = forecastSites.Where(b => b.SiteId == getdistinctsiteids[i] && b.ForecastInfoId == _forecastInfo.ForecastID).FirstOrDefault();
        //            if (fs == null)
        //            {
        //                fs = new ForecastSite();
        //                fs.SiteId = getdistinctsiteids[i];
        //                fs.ForecastInfoId = _forecastInfo.ForecastID;
        //                ctx.ForecastSite.Add(fs);
        //                ctx.SaveChanges();

        //            }
        //            var dd = _rdata.Where(x => x.SiteID == getdistinctsiteids[i]).GroupBy(b => b.testID).Select(x => x.Key).ToList();
        //            decimal workingday =sites.Where(b => b.SiteID == getdistinctsiteids[i]).Select(x => x.WorkingDays).FirstOrDefault();

        //            for (int j = 0; j < dd.Count; j++)
        //            {
        //                if(testarray.Contains(dd[j]) == false)
        //                {
        //                    ForecastTest ft = new ForecastTest();
        //                    ft.TestID = dd[j];
        //                    ft.forecastID = _forecastInfo.ForecastID;
        //                    ft.UserId = userid;
        //                    ctx.ForecastTest.Add(ft);
        //                    //ctx.SaveChanges();
        //                    testarray.Add(dd[j]);
        //                }


        //                var isexist1 = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == fs.Id && b.TestID == dd[j]).ToList();
        //                var data1 = _rdata.Where(x => x.SiteID == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
        //                for (int k = 0; k < data1.Count; k++)
        //                {
        //                    if (!data1[k].haserror)
        //                    {
        //                        ForecastSiteTest fp = new ForecastSiteTest
        //                        {
        //                            ForecastSiteID = fs.Id,
        //                            TestID = data1[k].testID
        //                        };
        //                        if (!Utility.Utility.IsDateTime(data1[k].Duration))
        //                        {
        //                            fp.CDuration = data1[k].Duration;
        //                            fp.DurationDateTime = Utility.Utility.DurationToDateTime(data1[k].Duration);

        //                        }
        //                        else
        //                        {
        //                            fp.DurationDateTime = DateTime.Parse(data1[k].Duration);
        //                            fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data1[k].Duration));
        //                        }
        //                        fp.AmountUsed = data1[k].Amount;
        //                        fp.StockOut = data1[k].StockOut;
        //                        fp.InstrumentDowntime = data1[k].InstrumentDownTime;//b
        //                        if (fp.StockOut > 0)
        //                        {
        //                            int days = fp.StockOut;
        //                            // decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

        //                            if (days >= workingday)
        //                            {
        //                                days = 0;
        //                                fp.StockOut = 0;
        //                            }
        //                            if (days >= 0)
        //                                fp.StockOut = days;
        //                        }


        //                        // var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

        //                        if (isexist1.Count == 0)
        //                        {
        //                            fp.Adjusted = fp.AmountUsed;

        //                            if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
        //                                fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b

        //                            existingFsp1.Add(fp);
        //                        }
        //                        else
        //                        {
        //                            var exist1 = isexist1.Where(x => x.CDuration == fp.CDuration && x.DurationDateTime == fp.DurationDateTime).FirstOrDefault();
        //                            if (exist1 == null)
        //                            {
        //                                fp.Adjusted = fp.AmountUsed;

        //                                if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
        //                                    fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b

        //                                existingFsp1.Add(fp);
        //                            }
        //                            else
        //                            {
        //                                exist1.AmountUsed = data1[k].Amount;
        //                                exist1.StockOut = data1[k].StockOut;
        //                                exist1.InstrumentDowntime = data1[k].InstrumentDownTime;//b
        //                                exist1.Adjusted = fp.AmountUsed;
        //                                if ((exist1.InstrumentDowntime > 0 || exist1.StockOut > 0) && exist1.AmountUsed > 0)
        //                                    exist1.Adjusted = Utility.Utility.GetAdjustedVolume(exist1.AmountUsed, exist1.StockOut + exist1.InstrumentDowntime, country.Period, workingday);//b
        //                                //ctx.SaveChanges();

        //                            }
        //                        }
        //                    }
        //                }


        //            }
        //        }

        //        for (int i = 0; i < getdistinctsiteids.Count; i++)
        //        {
        //            var dd = _rdata.Where(x => x.SiteID == getdistinctsiteids[i]).GroupBy(b => b.testID).Select(x => x.Key).ToList();
        //            decimal workingday = sites.Where(b => b.SiteID == getdistinctsiteids[i]).Select(x => x.WorkingDays).FirstOrDefault();
        //            for (int j = 0; j < dd.Count; j++)
        //            {

        //                var isexist = hisdata.Where(b => b.SiteId == getdistinctsiteids[i] && b.TestId == dd[j]).ToList();

        //                var data = _rdata.Where(x => x.SiteID == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
        //                for (int k = 0; k < data.Count; k++)
        //                {
        //                    if (!data[k].haserror)
        //                    {
        //                        HistoricalData fp = new HistoricalData();

        //                        fp.SiteId = getdistinctsiteids[i];
        //                        fp.TestId = data[k].testID;
        //                        if (!Utility.Utility.IsDateTime(data[k].Duration))
        //                        {
        //                            fp.CDuration = data[k].Duration;
        //                            fp.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);

        //                        }
        //                        else
        //                        {
        //                            fp.DurationDateTime = DateTime.Parse(data[k].Duration);
        //                            fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
        //                        }
        //                        fp.AmountUsed = data[k].Amount;
        //                        fp.StockOut = data[k].StockOut;
        //                        fp.InstrumentDowntime = data[k].InstrumentDownTime;//b
        //                        if (fp.StockOut > 0)
        //                        {
        //                            int days = fp.StockOut;
        //                            // decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

        //                            if (days >= workingday)
        //                            {
        //                                days = 0;
        //                                fp.StockOut = 0;
        //                            }
        //                            if (days >= 0)
        //                                fp.StockOut = days;
        //                        }


        //                        // var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

        //                        if (isexist.Count == 0)
        //                        {
        //                            fp.Adjusted = fp.AmountUsed;

        //                            if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
        //                                fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b

        //                            existingFsp.Add(fp);
        //                        }
        //                        else
        //                        {
        //                            var exist1 = isexist.Where(x => x.CDuration == fp.CDuration && x.DurationDateTime == fp.DurationDateTime).FirstOrDefault();
        //                            if (exist1 == null)
        //                            {
        //                                fp.Adjusted = fp.AmountUsed;

        //                                if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
        //                                    fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b
        //                            }
        //                            else
        //                            {

        //                                exist1.AmountUsed = data[k].Amount;
        //                                exist1.StockOut = data[k].StockOut;
        //                                exist1.InstrumentDowntime = data[k].InstrumentDownTime;//b
        //                                exist1.Adjusted = fp.AmountUsed;
        //                                if ((exist1.InstrumentDowntime > 0 || exist1.StockOut > 0) && exist1.AmountUsed > 0)
        //                                    exist1.Adjusted = Utility.Utility.GetAdjustedVolume(exist1.AmountUsed, exist1.StockOut + exist1.InstrumentDowntime, country.Period, workingday);//b

        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //        ctx.HistoricalData.AddRange(existingFsp);
        //        ctx.SaveChanges();
        //        ctx.ForecastSiteTest.AddRange(existingFsp1);
        //        ctx.SaveChanges();
        //        if (fail != "")
        //        {
        //            res = "you are selected these test " + fail.TrimStart(',') + " but excel sheet doesn't have these";
        //        }
        //        else
        //        {
        //            res = "Data saved successfully";
        //        }
        //    }

        //    else
        //    {
        //        IList<HistoricalData> existingFcp = new List<HistoricalData>();
        //        IList<ForecastCategoryTest> existingFcp1 = new List<ForecastCategoryTest>();
        //        var getdistinctsiteids = _rdata.GroupBy(b => b.CategoryName).Select(x => x.Key).ToList();
        //        for (int i = 0; i < getdistinctsiteids.Count; i++)
        //        {
        //            var dd = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i]).GroupBy(b => b.testID).Select(x => x.Key).ToList();
        //            fcat = ctx.ForecastCategory.Where(b => b.CategoryName == getdistinctsiteids[i] && b.ForecastId == _forecastInfo.ForecastID).FirstOrDefault(); ;
        //            if (fcat == null)
        //            {
        //                fcat = new ForecastCategory();
        //                fcat.CategoryName = getdistinctsiteids[i];
        //                fcat.ForecastId = _forecastInfo.ForecastID;
        //                ctx.ForecastCategory.Add(fcat);
        //                ctx.SaveChanges();

        //            }
        //            decimal workingday = 22;

        //            for (int j = 0; j < dd.Count; j++)
        //            {
        //                //var isexist = ctx.ForecastCategoryTest.Where(b => b.CategoryID == fcat.CategoryId && b.TestID == dd[j]).ToList();
        //                var isexist = ctx.HistoricalData.Where(b => b.CategoryId == fcat.CategoryId && b.TestId == dd[j]).ToList();
        //                var data = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
        //                for (int k = 0; k < data.Count; k++)

        //                {
        //                    // ForecastCategoryTest fp = new ForecastCategoryTest();

        //                    HistoricalData fp = new HistoricalData();


        //                    isduplicate = false;

        //                    if (isexist.Count == 0)
        //                        fp = new HistoricalData();
        //                    else
        //                    {
        //                        var exist1 = isexist.Where(x => x.CDuration == data[k].Duration1).FirstOrDefault();
        //                        if (exist1 != null)
        //                        {
        //                            isduplicate = true;
        //                        }
        //                    }

        //                    fp.CategoryId = fcat.CategoryId;
        //                    fp.TestId = data[k].testID;

        //                    fp.AmountUsed = data[k].Amount;
        //                    fp.StockOut = data[k].StockOut;
        //                    fp.InstrumentDowntime = data[k].InstrumentDownTime;//b

        //                    if (!Utility.Utility.IsDateTime(data[k].Duration))
        //                    {
        //                        fp.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);
        //                        fp.CDuration = data[k].Duration;
        //                    }
        //                    else
        //                    {
        //                        fp.DurationDateTime = DateTime.Parse(data[k].Duration);
        //                        fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
        //                    }

        //                    if (fp.StockOut > 0)
        //                    {
        //                        int days = fp.StockOut;


        //                        if (days >= workingday)
        //                        {
        //                            days = 0;
        //                            fp.StockOut = 0;
        //                        }
        //                        if (days >= 0)
        //                            fp.StockOut = days;
        //                    }
        //                    if (!isduplicate)
        //                    {
        //                        //ctx.ForecastCategoryProduct.Add(fp);
        //                        //ctx.SaveChanges();

        //                        //fp.Adjusted = fp.AmountUsed;
        //                        //if (fp.AmountUsed == 0)
        //                        //{
        //                        //    try
        //                        //    {
        //                        //        Consumption cs = GetConsumption(fp.Product.Id, fp);
        //                        //        fp.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
        //                        //    }
        //                        //    catch { fp.Adjusted = fp.AmountUsed; }
        //                        //}
        //                        if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
        //                            fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, 22);//b
        //                        existingFcp.Add(fp);
        //                        // ctx.SaveChanges();
        //                    }






        //                }



        //                //forecastcategory

        //                var isexist1 = ctx.HistoricalData.Where(b => b.CategoryId == fcat.CategoryId && b.TestId == dd[j]).ToList();
        //                var data1 = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
        //                for (int k = 0; k < data1.Count; k++)

        //                {
        //                    // ForecastCategoryTest fp = new ForecastCategoryTest();

        //                    ForecastCategoryTest fp = new ForecastCategoryTest();


        //                    isduplicate = false;

        //                    if (isexist.Count == 0)
        //                        fp = new ForecastCategoryTest();
        //                    else
        //                    {
        //                        var exist1 = isexist.Where(x => x.CDuration == data1[k].Duration1).FirstOrDefault();
        //                        if (exist1 != null)
        //                        {
        //                            isduplicate = true;
        //                        }
        //                    }

        //                    fp.CategoryID = fcat.CategoryId;
        //                    fp.TestID = data1[k].testID;

        //                    fp.AmountUsed = data1[k].Amount;
        //                    fp.StockOut = data1[k].StockOut;
        //                    fp.InstrumentDowntime = data1[k].InstrumentDownTime;//b

        //                    if (!Utility.Utility.IsDateTime(data1[k].Duration))
        //                    {
        //                        fp.DurationDateTime = Utility.Utility.DurationToDateTime(data1[k].Duration);
        //                        fp.CDuration = data1[k].Duration;
        //                    }
        //                    else
        //                    {
        //                        fp.DurationDateTime = DateTime.Parse(data1[k].Duration);
        //                        fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data1[k].Duration));
        //                    }

        //                    if (fp.StockOut > 0)
        //                    {
        //                        int days = fp.StockOut;


        //                        if (days >= workingday)
        //                        {
        //                            days = 0;
        //                            fp.StockOut = 0;
        //                        }
        //                        if (days >= 0)
        //                            fp.StockOut = days;
        //                    }
        //                    if (!isduplicate)
        //                    {
        //                        //ctx.ForecastCategoryProduct.Add(fp);
        //                        //ctx.SaveChanges();

        //                        //fp.Adjusted = fp.AmountUsed;
        //                        //if (fp.AmountUsed == 0)
        //                        //{
        //                        //    try
        //                        //    {
        //                        //        Consumption cs = GetConsumption(fp.Product.Id, fp);
        //                        //        fp.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
        //                        //    }
        //                        //    catch { fp.Adjusted = fp.AmountUsed; }
        //                        //}
        //                        if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
        //                            fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, 22);//b
        //                        existingFcp1.Add(fp);
        //                        // ctx.SaveChanges();
        //                    }






        //                }
        //            }

        //        }

        //        ctx.HistoricalData.AddRange(existingFcp);
        //        ctx.ForecastCategoryTest.AddRange(existingFcp1);
        //        ctx.SaveChanges();
        //        res = "Data saved successfully";
        //    }

        //    return res;
        //}
        public string importservicenew(List<string[]> jArray, int id, int userid)
        {
            //   DataTable DT = new DataTable();
            ForecastSite fs = new ForecastSite();
            ForecastCategory fcat = new ForecastCategory();
            List<receivereportdata> _rdata = new List<receivereportdata>();
            string categoryName = null;
            string regionName;
            string siteName;
            string testName;
            string duration;
            decimal amount;
            int stockout;
            int instrumentDownTime;
            decimal adjusited;
            int rowno = 0;
            bool haserror;
            string cName = "";
            string rName = "";
            string sName = "";
            string tName = "";
            string res = "";
            _forecastInfo = ctx.ForecastInfo.Find(id);
            ForecastCategory fcategory = null;
            bool isduplicate = false;
            Models.Region region = null;
            Site site = null;
            //MasterProduct product = null;
            Test test = null;
            string errorDescription = "";
            int errornos = 0;
            string errormsg = "";
            IList<ReportedData> rdlist = new List<ReportedData>();
            string[] dr = jArray[0];
            DataTable dtexcelcolor = new DataTable();
            dtexcelcolor.Columns.Add("Sheet");
            dtexcelcolor.Columns.Add("Row");
            dtexcelcolor.Columns.Add("Error");
            dtexcelcolor.Columns.Add("Color");
            dtexcelcolor.AcceptChanges();
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            var countryid = ctx.User.Find(userid).CountryId;
            var country = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault();
            var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x => x.Id).FirstOrDefault();
            string fail = "";
            List<Test> tests = new List<Test>();
            tests = ctx.Test.Where(b => b.UserId == userid || b.UserId == adminuserid).ToList();
            List<Region> regions = new List<Region>();
            regions = ctx.Region.Where(b => b.CountryId == countryid || b.UserId == userid).ToList();
            List<Site> sites = new List<Site>();
            sites = ctx.Site.Where(b => b.CountryId == countryid || b.UserId == userid).ToList();
            for (int i = 1; i < jArray.Count; i++)//ds.Tables[0].Rows.Count
            {
                if (jArray[i].Length > 0)
                {

                    errormsg = "";
                    rowno++;
                    haserror = false;
                    string[] dr1 = jArray[i];
                    //string[] dr2 = jArray[i + 1];
                    //string[] dr3 = jArray[i + 2];
                    //string[] dr4 = jArray[i + 3];
                    string[] g = jArray[0];
                    // int colid = 0;
                    int f = 3;
                    int colid;//0 
                    if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        colid = 2;
                    else
                        colid = 3;

                    int noofcolumn = 0;
                    //noofcolumn = jArray[i].Length - 1;
                    //bool newerror = false;

                    //do
                    //{
                    //    if (dr1[noofcolumn] == "")
                    //        dr1[noofcolumn] = "0";
                    //    Decimal amount1 = Convert.ToDecimal(dr1[noofcolumn]);
                    //    if (Convert.ToDecimal(dr1[noofcolumn]) == 0)
                    //    {
                    //        newerror = true;
                    //        break;
                    //    }

                    //    noofcolumn--;
                    //}
                    //while (noofcolumn >= jArray[i].Length - 3);
                    //if (newerror == true)
                    //{
                    //    errornos++;
                    //    if (errornos > 20)
                    //    {
                    //        //  label7.Text = "";
                    //        //MessageBox.Show("There too many problem with Instrument data, please troubleshoot and try to import again.", "Importing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //        throw new Exception("There too many problem with Service data, please troubleshoot and try to import again.");
                    //    }
                    //    errormsg = "The Test is not performed in last three months";
                    //    //dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                    //    ////   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                    //    //errormsg = "Please Check excel Some test are not performed in recent last three months";

                    //    continue;

                    //}
                    //else
                    //{
                    //    //if (_forecastInfo.DatausageEnum == DataUsageEnum.DATA_USAGE3)
                    //    //    colid = 3;
                    //    //else
                    //    //    colid = 4;

                    //}

                    //  label7.Text = "Importing Data.....................";


                    do
                    {
                        regionName = "";
                        siteName = "";
                        errormsg = "";
                        if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        {
                            categoryName = Convert.ToString(dr1[0]).Trim();//(dr[colid++])
                            testName = Convert.ToString(dr1[1]).Trim();   //(dr[colid++])testName
                        }
                        else
                        {
                            regionName = Convert.ToString(dr1[0]).Trim(); //(dr[colid++]) region name
                            siteName = Convert.ToString(dr1[1]).Trim();           //(dr[colid++]) site name
                            testName = Convert.ToString(dr1[2]).Trim();   //(dr[colid++])testName 
                        }
                        string date;
                        string[] datearr;
                        if (country.Period != "Monthly" || country.Period != "Bimonthly")
                        {

                            datearr = dr[colid].Split("/");
                            if (datearr[0].Length == 1)
                                date = datearr[1] + "/0" + datearr[0] + "/" + datearr[2];
                            else
                                date = datearr[1] + "/" + datearr[0] + "/" + datearr[2];
                            //  duration = Convert.ToString(DateTime.Parse(date));
                            // duration = Convert.ToString(DateTime.Parse(dr[colid]));


                            //duration = Convert.ToString(DateTime.Parse("01 / 12 / 2020"));
                            duration = Convert.ToString(DateTime.Parse(date));
                        }
                        //duration = Convert.ToString(DateTime.FromOADate(Convert.ToDouble(dr[colid])));//(g[f]) Convert.ToString(dr[colid++]); //  reporting period(duration)
                        // f = f + 3;
                        else
                            duration = Convert.ToString(dr[colid]);

                        //duration = Convert.ToString(dr[colid++]); // reporting period(duration)

                        try
                        {
                            if (string.IsNullOrEmpty(dr1[colid]))
                                dr1[colid] = "0";
                            amount = Convert.ToDecimal(dr1[colid]);  //amount
                                                                     //if (amount == 0)
                                                                     //    haserror = true;
                        }
                        catch
                        {
                            haserror = true;
                            amount = 0;
                        }

                        stockout = 0;

                        instrumentDownTime = 0;    //instrumentDownTime

                        //try
                        //{
                        //    adjusited = Convert.ToDecimal(dr[colid++]);     //adjusted
                        //}
                        //catch
                        //{
                        //    haserror = true;
                        adjusited = 0;
                        // }

                        ReportedData rd = null;
                        DataRow Dr = ServiceDT.NewRow();
                        if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        {
                            rd = new ReportedData
                            {
                                _rowno = rowno,
                                _categoryName = categoryName,
                                _testname = testName,
                                _duration = duration,
                                _amount = amount,
                                _stockout = stockout,
                                _instrumentDownTime = instrumentDownTime
                            };
                            if (cName != categoryName)
                            {
                                if (!string.IsNullOrEmpty(categoryName))
                                {
                                    fcategory = ctx.ForecastCategory.Where(b => b.ForecastId == _forecastInfo.ForecastID && b.CategoryName == categoryName).FirstOrDefault();
                                }
                                else
                                    fcategory = null;
                                cName = categoryName;
                            }

                            if (fcategory != null)
                                rd._category = fcategory;
                            else
                            {
                                rd._hasError = true;
                                errorDescription = errorDescription + " Category Doesn't Exist";
                                //  errormsg = errormsg + " Category Doesn't Exist";
                                //   dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                                //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                            }






                        }
                        else
                        {
                            rd = new ReportedData
                            {
                                _rowno = rowno,
                                _regionName = regionName,
                                _siteName = siteName,
                                _testname = testName,
                                _duration = duration,
                                _amount = amount,
                                _stockout = stockout,
                                _instrumentDownTime = instrumentDownTime
                            };

                            if (rName != regionName)
                            {

                                region = regions.Where(b => b.RegionName.ToUpper() == regionName.ToUpper()).FirstOrDefault();

                                //   }



                            }

                            if (region != null)
                            {
                                rd._region = region;
                                rName = regionName;
                                if (sName != siteName)
                                {
                                    //if (!string.IsNullOrEmpty(siteName))
                                    //{
                                    //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                    //if (site == null)
                                    //{
                                    site = sites.Where(b => b.SiteName.ToUpper() == siteName.ToUpper() && b.regionid == region.RegionID).FirstOrDefault();

                                    //    //  }
                                    //}
                                    //else
                                    //    site = null;
                                    //sName = siteName;
                                }
                                if (site != null)
                                {
                                    rd._site = site;
                                    sName = siteName;
                                }
                                else
                                {
                                    //if (sName != siteName)
                                    //{
                                    //    //if (!string.IsNullOrEmpty(siteName))
                                    //    //{
                                    //        Site ST = new Site
                                    //        {
                                    //            CountryId = countryid,
                                    //            UserId = userid,
                                    //            SiteName = siteName,
                                    //            regionid = region.RegionID
                                    //        };
                                    //        ctx.Site.Add(ST);
                                    //        ctx.SaveChanges();
                                    //        rd._site = ST;
                                    //        site = ST;
                                    //    //}
                                    //    sName = siteName;
                                    //}
                                    //else
                                    //{
                                    //    rd._site = site;
                                    //    //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                    //    //if (site == null)
                                    //    //{
                                    //    //    site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && b.CountryId == ctx.User.Where(c => c.Id == userid).Select(iz => iz.CountryId).FirstOrDefault()).FirstOrDefault();

                                    //    //}
                                    //}
                                    //haserror = true;
                                    //errorDescription = errorDescription + " Site Doesn't Exist";
                                    ////dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
                                    //      changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Site Doesn't Exist", Color.Red);
                                }
                            }
                            else
                            {
                                if (rName != regionName)
                                {

                                    //if (!string.IsNullOrEmpty(regionName))
                                    //{
                                    //    Region Rg = new Region
                                    //    {
                                    //        RegionName = regionName,
                                    //        UserId = userid,
                                    //        CountryId = countryid
                                    //    };
                                    //    ctx.Region.Add(Rg);
                                    //    ctx.SaveChanges();
                                    //    rName = regionName;
                                    //    rd._region = Rg;
                                    //    if (sName != siteName)
                                    //    {
                                    //        if (!string.IsNullOrEmpty(siteName))
                                    //        {

                                    //            Site ST = new Site
                                    //            {
                                    //                CountryId = countryid,
                                    //                UserId = userid,
                                    //                SiteName = siteName,
                                    //                regionid = Rg.RegionID
                                    //            };
                                    //            ctx.Site.Add(ST);
                                    //            ctx.SaveChanges();
                                    //            rd._site = site;
                                    //        }

                                    //    }
                                    //    else
                                    //    {
                                    //        rd._site = site;
                                    //    }
                                    //    sName = siteName;





                                    //}
                                }
                                //haserror = true;
                                // errorDescription = errorDescription + " Region Doesn't Exist";
                                //dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
                                //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Region Doesn't Exist", Color.Red);

                            }
                            // Dr["Cat


                        }



                        //if (tName != testName)
                        //{

                        test = tests.Where(b => b.TestName.Trim().ToUpper() == testName.Trim().ToUpper()).FirstOrDefault();

                        tName = testName;
                        //test = tests.Where(b => b.TestName.Trim().ToUpper() == testName.Trim().ToUpper()).FirstOrDefault();
                        //}

                        //  if (test != null)
                        rd._test = test;

                        //else
                        //{
                        //    haserror = true;
                        //    errorDescription = errorDescription + " Test Doesn't Exist";
                        //    errormsg = errormsg + " Test Doesn't Exist";
                        //  dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                        //   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                        // }
                        if (haserror == false)
                        {
                            receivereportdata RD = new receivereportdata();

                            RD.CategoryName = rd._categoryName; ;
                            RD.RegionName = rd._regionName;
                            RD.SiteName = rd._siteName;
                            RD.testName = rd._testname;
                            //rd._hasError = haserror;
                            //rd.errorDescription = errorDescription;

                            if (!Utility.Utility.IsDateTime(rd._duration))
                            {

                                try
                                {
                                    DateTime dd = Utility.Utility.DurationToDateTime(rd._duration);
                                    if ((country.Period == "Yearly"))
                                    {

                                        rd._duration = dd.Year.ToString();
                                        RD.Duration = Utility.Utility.DatetimeToDurationStr(country.Period, dd);
                                    }
                                    else
                                        RD.Duration = rd._duration;
                                }
                                catch (Exception ex)
                                {
                                    RD.Duration = rd._duration;
                                    rd._hasError = true;
                                }

                            }
                            else
                            {

                                string datestr = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));

                                //rd._duration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
                                //if (country.Period == "Yearly")
                                //{
                                RD.Duration = datestr;

                                //}
                                //else
                                //{
                                //    RD.Duration = rd._duration;

                                //}


                            }



                            RD.Amount = rd._amount;
                            RD.StockOut = rd._stockout;
                            RD.InstrumentDownTime = rd._instrumentDownTime;
                            //if (rd.HasError == true && rd.ErrorDescription == "")
                            //    rd.ErrorDescription = " Product Consumed Required ";

                            //     li.SubItems.Add(rd.ErrorDescription.ToString());
                            //if (Utility.Utility.validDate(rd._duration, country.Period))
                            //    rd._hasError = true;

                            //if (rd._hasError == true && rd.errorDescription == "")
                            //    rd.errorDescription = " Product Consumed Required ";
                            //Dr["Description"] = rd.errorDescription.ToString();
                            //if (rd._site != null)
                            RD.SiteID = rd._site.SiteID;
                            //else
                            //    Dr["SiteID"] = 0;
                            //if (rd._test != null)
                            RD.testID = rd._test.TestID;
                            //else
                            //    Dr["testID"] = 0;
                            if (_forecastInfo.DataUsage == "DATA_USAGE3")
                            {
                                RD.CatID = rd._category.CategoryId;
                            }
                            else
                            {
                                RD.CatID = 0;
                            }
                            RD.Duration1 = rd._duration;
                            //Dr["haserror"] = rd._hasError;
                            RD.forecastid = id;
                            RD.No = rd._rowno;
                            _rdata.Add(RD);
                        }
                        // rdlist.Add(rd);
                        colid++;
                        errorDescription = "";
                    }
                    while (colid < g.Length && g[colid].ToString() != "");// dr.ItemArray.Length / ds.Tables[0].Rows.Count);
                }

            }

            var testarray = ctx.ForecastTest.Where(b => b.forecastID == _forecastInfo.ForecastID).Select(b => b.TestID).ToList();
            var distincttest = _rdata.GroupBy(b => b.testID).Select(x => x.Key).ToList();
            for (int j = 0; j < testarray.Count; j++)
            {
                if (distincttest.Contains(testarray[j]) != true)
                {
                    fail = fail + "," + ctx.Test.Find(testarray[j]).TestName;
                }

            }

            List<ForecastSite> forecastSites = new List<ForecastSite>();
            List<Site> sitelst = new List<Site>();
            List<HistoricalData> hisdata = new List<HistoricalData>();
            hisdata = ctx.HistoricalData.ToList();
            forecastSites = ctx.ForecastSite.Where(b => b.ForecastInfoId == _forecastInfo.ForecastID).ToList();

            if (_forecastInfo.DataUsage != "DATA_USAGE3")
            {
                // IList<ForecastSiteTest> existingFsp = new List<ForecastSiteTest>();
                IList<HistoricalData> existingFsp = new List<HistoricalData>();
                IList<ForecastSiteTest> existingFsp1 = new List<ForecastSiteTest>();
                var getdistinctsiteids = _rdata.GroupBy(b => b.SiteID).Select(x => x.Key).ToList();
                for (int i = 0; i < getdistinctsiteids.Count; i++)
                {
                    fs = forecastSites.Where(b => b.SiteId == getdistinctsiteids[i] && b.ForecastInfoId == _forecastInfo.ForecastID).FirstOrDefault();
                    if (fs == null)
                    {
                        fs = new ForecastSite();
                        fs.SiteId = getdistinctsiteids[i];
                        fs.ForecastInfoId = _forecastInfo.ForecastID;
                        ctx.ForecastSite.Add(fs);
                        ctx.SaveChanges();

                    }
                    var dd = _rdata.Where(x => x.SiteID == getdistinctsiteids[i]).GroupBy(b => b.testID).Select(x => x.Key).ToList();
                    decimal workingday = sites.Where(b => b.SiteID == getdistinctsiteids[i]).Select(x => x.WorkingDays).FirstOrDefault();

                    for (int j = 0; j < dd.Count; j++)
                    {
                        if (testarray.Contains(dd[j]) == false)
                        {
                            ForecastTest ft = new ForecastTest();
                            ft.TestID = dd[j];
                            ft.forecastID = _forecastInfo.ForecastID;
                            ft.UserId = userid;
                            ctx.ForecastTest.Add(ft);
                            //ctx.SaveChanges();
                            testarray.Add(dd[j]);
                        }


                        var isexist1 = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == fs.Id && b.TestID == dd[j]).ToList();
                        var data1 = _rdata.Where(x => x.SiteID == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
                        for (int k = 0; k < data1.Count; k++)
                        {
                            if (!data1[k].haserror)
                            {
                                ForecastSiteTest fp = new ForecastSiteTest
                                {
                                    ForecastSiteID = fs.Id,
                                    TestID = data1[k].testID
                                };
                                if (!Utility.Utility.IsDateTime(data1[k].Duration))
                                {
                                    fp.CDuration = data1[k].Duration;
                                    fp.DurationDateTime = Utility.Utility.DurationToDateTime(data1[k].Duration);

                                }
                                else
                                {
                                    fp.DurationDateTime = DateTime.Parse(data1[k].Duration);
                                    fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data1[k].Duration));
                                }
                                fp.AmountUsed = data1[k].Amount;
                                fp.StockOut = data1[k].StockOut;
                                fp.InstrumentDowntime = data1[k].InstrumentDownTime;//b
                                if (fp.StockOut > 0)
                                {
                                    int days = fp.StockOut;
                                    // decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

                                    if (days >= workingday)
                                    {
                                        days = 0;
                                        fp.StockOut = 0;
                                    }
                                    if (days >= 0)
                                        fp.StockOut = days;
                                }


                                // var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

                                if (isexist1.Count == 0)
                                {
                                    fp.Adjusted = fp.AmountUsed;

                                    if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                        fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b

                                    existingFsp1.Add(fp);
                                }
                                else
                                {
                                    var exist1 = isexist1.Where(x => x.CDuration == fp.CDuration && x.DurationDateTime == fp.DurationDateTime).FirstOrDefault();
                                    if (exist1 == null)
                                    {
                                        fp.Adjusted = fp.AmountUsed;

                                        if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                            fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b

                                        existingFsp1.Add(fp);
                                    }
                                    else
                                    {
                                        exist1.AmountUsed = data1[k].Amount;
                                        exist1.StockOut = data1[k].StockOut;
                                        exist1.InstrumentDowntime = data1[k].InstrumentDownTime;//b
                                        exist1.Adjusted = fp.AmountUsed;
                                        if ((exist1.InstrumentDowntime > 0 || exist1.StockOut > 0) && exist1.AmountUsed > 0)
                                            exist1.Adjusted = Utility.Utility.GetAdjustedVolume(exist1.AmountUsed, exist1.StockOut + exist1.InstrumentDowntime, country.Period, workingday);//b
                                        //ctx.SaveChanges();

                                    }
                                }
                            }
                        }


                    }
                }

                for (int i = 0; i < getdistinctsiteids.Count; i++)
                {
                    var dd = _rdata.Where(x => x.SiteID == getdistinctsiteids[i]).GroupBy(b => b.testID).Select(x => x.Key).ToList();
                    decimal workingday = sites.Where(b => b.SiteID == getdistinctsiteids[i]).Select(x => x.WorkingDays).FirstOrDefault();
                    for (int j = 0; j < dd.Count; j++)
                    {

                        var isexist = hisdata.Where(b => b.SiteId == getdistinctsiteids[i] && b.TestId == dd[j]).ToList();

                        var data = _rdata.Where(x => x.SiteID == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
                        for (int k = 0; k < data.Count; k++)
                        {
                            if (!data[k].haserror)
                            {
                                HistoricalData fp = new HistoricalData();

                                fp.SiteId = getdistinctsiteids[i];
                                fp.TestId = data[k].testID;
                                if (!Utility.Utility.IsDateTime(data[k].Duration))
                                {
                                    fp.CDuration = data[k].Duration;
                                    fp.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);

                                }
                                else
                                {
                                    fp.DurationDateTime = DateTime.Parse(data[k].Duration);
                                    fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
                                }
                                fp.AmountUsed = data[k].Amount;
                                fp.StockOut = data[k].StockOut;
                                fp.InstrumentDowntime = data[k].InstrumentDownTime;//b
                                if (fp.StockOut > 0)
                                {
                                    int days = fp.StockOut;
                                    // decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

                                    if (days >= workingday)
                                    {
                                        days = 0;
                                        fp.StockOut = 0;
                                    }
                                    if (days >= 0)
                                        fp.StockOut = days;
                                }


                                // var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

                                if (isexist.Count == 0)
                                {
                                    fp.Adjusted = fp.AmountUsed;

                                    if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                        fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b

                                    existingFsp.Add(fp);
                                }
                                else
                                {
                                    var exist1 = isexist.Where(x => x.CDuration == fp.CDuration && x.DurationDateTime == fp.DurationDateTime).FirstOrDefault();
                                    if (exist1 == null)
                                    {
                                        fp.Adjusted = fp.AmountUsed;

                                        if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                            fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b
                                    }
                                    else
                                    {

                                        exist1.AmountUsed = data[k].Amount;
                                        exist1.StockOut = data[k].StockOut;
                                        exist1.InstrumentDowntime = data[k].InstrumentDownTime;//b
                                        exist1.Adjusted = fp.AmountUsed;
                                        if ((exist1.InstrumentDowntime > 0 || exist1.StockOut > 0) && exist1.AmountUsed > 0)
                                            exist1.Adjusted = Utility.Utility.GetAdjustedVolume(exist1.AmountUsed, exist1.StockOut + exist1.InstrumentDowntime, country.Period, workingday);//b

                                    }
                                }
                            }
                        }
                    }

                }
                ctx.HistoricalData.AddRange(existingFsp);
                ctx.SaveChanges();
                ctx.ForecastSiteTest.AddRange(existingFsp1);
                ctx.SaveChanges();
                if (fail != "")
                {
                    res = "you are selected these test " + fail.TrimStart(',') + " but excel sheet doesn't have these";
                }
                else
                {
                    res = "Data saved successfully";
                }
            }

            else
            {
                IList<HistoricalData> existingFcp = new List<HistoricalData>();
                IList<ForecastCategoryTest> existingFcp1 = new List<ForecastCategoryTest>();
                var getdistinctsiteids = _rdata.GroupBy(b => b.CategoryName).Select(x => x.Key).ToList();
                for (int i = 0; i < getdistinctsiteids.Count; i++)
                {
                    var dd = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i]).GroupBy(b => b.testID).Select(x => x.Key).ToList();
                    fcat = ctx.ForecastCategory.Where(b => b.CategoryName == getdistinctsiteids[i] && b.ForecastId == _forecastInfo.ForecastID).FirstOrDefault(); ;
                    if (fcat == null)
                    {
                        fcat = new ForecastCategory();
                        fcat.CategoryName = getdistinctsiteids[i];
                        fcat.ForecastId = _forecastInfo.ForecastID;
                        ctx.ForecastCategory.Add(fcat);
                        ctx.SaveChanges();

                    }
                    decimal workingday = 22;

                    for (int j = 0; j < dd.Count; j++)
                    {
                        //var isexist = ctx.ForecastCategoryTest.Where(b => b.CategoryID == fcat.CategoryId && b.TestID == dd[j]).ToList();
                        var isexist = ctx.HistoricalData.Where(b => b.CategoryId == fcat.CategoryId && b.TestId == dd[j]).ToList();
                        var data = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
                        for (int k = 0; k < data.Count; k++)

                        {
                            // ForecastCategoryTest fp = new ForecastCategoryTest();

                            HistoricalData fp = new HistoricalData();


                            isduplicate = false;

                            if (isexist.Count == 0)
                                fp = new HistoricalData();
                            else
                            {
                                var exist1 = isexist.Where(x => x.CDuration == data[k].Duration1).FirstOrDefault();
                                if (exist1 != null)
                                {
                                    isduplicate = true;
                                }
                            }

                            fp.CategoryId = fcat.CategoryId;
                            fp.TestId = data[k].testID;

                            fp.AmountUsed = data[k].Amount;
                            fp.StockOut = data[k].StockOut;
                            fp.InstrumentDowntime = data[k].InstrumentDownTime;//b

                            if (!Utility.Utility.IsDateTime(data[k].Duration))
                            {
                                fp.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);
                                fp.CDuration = data[k].Duration;
                            }
                            else
                            {
                                fp.DurationDateTime = DateTime.Parse(data[k].Duration);
                                fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
                            }

                            if (fp.StockOut > 0)
                            {
                                int days = fp.StockOut;


                                if (days >= workingday)
                                {
                                    days = 0;
                                    fp.StockOut = 0;
                                }
                                if (days >= 0)
                                    fp.StockOut = days;
                            }
                            if (!isduplicate)
                            {
                                //ctx.ForecastCategoryProduct.Add(fp);
                                //ctx.SaveChanges();

                                //fp.Adjusted = fp.AmountUsed;
                                //if (fp.AmountUsed == 0)
                                //{
                                //    try
                                //    {
                                //        Consumption cs = GetConsumption(fp.Product.Id, fp);
                                //        fp.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
                                //    }
                                //    catch { fp.Adjusted = fp.AmountUsed; }
                                //}
                                if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                    fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, 22);//b
                                existingFcp.Add(fp);
                                // ctx.SaveChanges();
                            }






                        }



                        //forecastcategory

                        var isexist1 = ctx.HistoricalData.Where(b => b.CategoryId == fcat.CategoryId && b.TestId == dd[j]).ToList();
                        var data1 = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i] && x.testID == dd[j]).ToList();
                        for (int k = 0; k < data1.Count; k++)

                        {
                            // ForecastCategoryTest fp = new ForecastCategoryTest();

                            ForecastCategoryTest fp = new ForecastCategoryTest();


                            isduplicate = false;

                            if (isexist.Count == 0)
                                fp = new ForecastCategoryTest();
                            else
                            {
                                var exist1 = isexist.Where(x => x.CDuration == data1[k].Duration1).FirstOrDefault();
                                if (exist1 != null)
                                {
                                    isduplicate = true;
                                }
                            }

                            fp.CategoryID = fcat.CategoryId;
                            fp.TestID = data1[k].testID;

                            fp.AmountUsed = data1[k].Amount;
                            fp.StockOut = data1[k].StockOut;
                            fp.InstrumentDowntime = data1[k].InstrumentDownTime;//b

                            if (!Utility.Utility.IsDateTime(data1[k].Duration))
                            {
                                fp.DurationDateTime = Utility.Utility.DurationToDateTime(data1[k].Duration);
                                fp.CDuration = data1[k].Duration;
                            }
                            else
                            {
                                fp.DurationDateTime = DateTime.Parse(data1[k].Duration);
                                fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data1[k].Duration));
                            }

                            if (fp.StockOut > 0)
                            {
                                int days = fp.StockOut;


                                if (days >= workingday)
                                {
                                    days = 0;
                                    fp.StockOut = 0;
                                }
                                if (days >= 0)
                                    fp.StockOut = days;
                            }
                            if (!isduplicate)
                            {
                                //ctx.ForecastCategoryProduct.Add(fp);
                                //ctx.SaveChanges();

                                //fp.Adjusted = fp.AmountUsed;
                                //if (fp.AmountUsed == 0)
                                //{
                                //    try
                                //    {
                                //        Consumption cs = GetConsumption(fp.Product.Id, fp);
                                //        fp.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
                                //    }
                                //    catch { fp.Adjusted = fp.AmountUsed; }
                                //}
                                if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                    fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, 22);//b
                                existingFcp1.Add(fp);
                                // ctx.SaveChanges();
                            }






                        }
                    }

                }

                ctx.HistoricalData.AddRange(existingFcp);
                ctx.ForecastCategoryTest.AddRange(existingFcp1);
                ctx.SaveChanges();
                res = "Data saved successfully";
            }

            return res;
        }

        public string Importconsumptionnew(List<string[]> jArray, int id, int userid)
        {
            string categoryName = null;
            string regionName;
            string siteName;
            string productName;
            string duration;
            decimal amount;
            int stockout;
            int instrumentDownTime;


            int rowno = 0;
            bool haserror;
            string cName = "";
            string rName = "";
            string sName = "";
            string pName = "";
            string res = "";
            ForecastSite fs = new ForecastSite();
            ForecastCategory fcat = new ForecastCategory();
            List<receivereportdata> _rdata = new List<receivereportdata>();
            ForecastCategory fcategory = null;
            Models.Region region = null;
            Site site = null;
            MasterProduct product = null;
            //Test test = null;
        
            IList<ReportedData> rdlist = new List<ReportedData>();
            string[] dr = jArray[0];
            _forecastInfo = ctx.ForecastInfo.Find(id);
            var countryid = ctx.User.Find(userid).CountryId;
            var country = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault();
            var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x => x.Id).FirstOrDefault();

            List<MasterProduct> MP = new List<MasterProduct>();
            MP = ctx.MasterProduct.Where(b => b.UserId == userid || b.UserId == adminuserid).ToList();
            List<Region> regions = new List<Region>();
            regions = ctx.Region.Where(b => b.CountryId == countryid || b.UserId == userid).ToList();
            List<Site> sites = new List<Site>();
            sites = ctx.Site.Where(b =>b.CountryId == countryid || b.UserId == userid).ToList();
            int startCol = 3;//0 
            if (_forecastInfo.DataUsage == "DATA_USAGE3")
                startCol = 2;
            for (int i = 1; i < jArray.Count; i++)//ds.Tables[0].Rows.Count
            {
                int colid = startCol;
                rowno++;
                haserror = false;
                if (jArray[i].Length > 0)
                {
                    string[] dr1 = jArray[i];
                    //string[] dr2 = jArray[i + 1];
                    //string[] dr3 = jArray[i + 2];
                    //string[] dr4 = jArray[i + 3];
                    string[] g = jArray[0];
                    int f = 3;
                    

                    //int noofcolumn = 0;
                    //noofcolumn = jArray[i].Length - 1;
                    //bool newerror = false;
                    //do
                    //{
                    //    if (dr1[noofcolumn] == "")
                    //        dr1[noofcolumn] = "0";
                    //    Decimal amount1 = Convert.ToDecimal(dr1[noofcolumn]);
                    //    if (Convert.ToDecimal(dr1[noofcolumn]) == 0)
                    //    {
                    //        newerror = true;
                    //        break;
                    //    }

                    //    noofcolumn--;
                    //}
                    //while (noofcolumn >= jArray[i].Length - 3);
                    //if (newerror == true)
                    //{
                    //    errornos++;
                    //    if (errornos > 20)
                    //    {

                    //        // throw new Exception("There too many problem with Consumption data, please troubleshoot and try to import again.");
                    //    }
                    //    errormsg = "The product is not consumed in last three months";

                    //    //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                    //    errormsg = "Please Check excel Some product are not consumed in recent last three months";

                    //    continue;

                    //}
                    //else
                    //{
                        //if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        //    colid = 6;
                        //else
                        //    colid = 6;

                    //}
                    //  label7.Text = "Importing Data.....................";
                    do
                    {

                        regionName = "";
                        siteName = "";
                       // errorDescription = "";
                        if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        {
                            categoryName = Convert.ToString(dr1[0]).Trim();//(dr[colid++])
                            productName = Convert.ToString(dr1[1]).Trim();   //(dr[colid++])product name
                        }
                        else
                        {
                            regionName = Convert.ToString(dr1[0]).Trim(); //(dr[colid++]) region name
                            siteName = Convert.ToString(dr1[1]).Trim();           //(dr[colid++]) site name
                            productName = Convert.ToString(dr1[2]).Trim();   //(dr[colid++])product name
                        }
                      
                        string date;
                        string[] datearr;
                        if (country.Period != "Monthly" || country.Period != "Bimonthly")
                        {
                            datearr = dr[colid].Split("/");
                          

                            datearr = dr[colid].Split("/");
                            if (datearr[0].Length == 1)
                                date = datearr[1] + "/0" + datearr[0] + "/" + datearr[2];
                            else
                                date = datearr[1] + "/" + datearr[0] + "/" + datearr[2];
                            //  duration = Convert.ToString(DateTime.Parse(date));
                            // duration = Convert.ToString(DateTime.Parse(dr[colid]));


                            //duration = Convert.ToString(DateTime.Parse("01 / 12 / 2020"));
                            duration = Convert.ToString(DateTime.Parse(date));
                            //(g[f]) Convert.ToString(dr[colid++]); //  reporting period(duration)
                        }
                        else
                            duration = Convert.ToString(dr[colid]);
                        try
                        {
                            if (string.IsNullOrEmpty(dr1[colid]))
                                dr1[colid] = "0";
                            amount = Convert.ToDecimal(dr1[colid]); 
                        }
                        catch
                        {
                            haserror = true;
                            amount = 0;
                        }
                       
                        stockout = 0;
                       
                        instrumentDownTime = 0;    //instrumentDownTime
                       

                        
                        ReportedData rd = null;
                       


                        if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        {
                            rd = new ReportedData
                            {
                                _rowno = rowno,
                                _categoryName = categoryName,
                                _productName = productName,
                                _duration = duration,
                                _amount = amount,
                                _stockout = stockout,
                                _instrumentDownTime = instrumentDownTime
                            };
                            if (cName != categoryName)
                            {
                               
                                    fcategory = ctx.ForecastCategory.Where(b => b.ForecastId == _forecastInfo.ForecastID && b.CategoryName == categoryName).FirstOrDefault();
                               
                                cName = categoryName;
                            }

                            if (fcategory != null)
                                rd._category = fcategory;
                            else
                            {
                                rd._hasError = true;


                                //   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Category Doesn't Exist", Color.Red);
                            }
                            //Dr["CategoryName"] = rd._categoryName;
                            //Dr["RegionName"] = "";
                            //Dr["SiteName"] = "";

                        }
                        else
                        {
                            rd = new ReportedData
                            {
                                _rowno = rowno,
                                _regionName = regionName,
                                _siteName = siteName,
                                _productName = productName,
                                _duration = duration,
                                _amount = amount,
                                _stockout = stockout,
                                _instrumentDownTime = instrumentDownTime
                            };//(rowno, regionName, siteName, productName, duration, amount, stockout, instrumentDownTime);//b

                            //if (rName != regionName)
                            //{

                                region = regions.Where(b => b.RegionName.ToUpper() == regionName.ToUpper()).FirstOrDefault();//ctx.Region.Where(b => b.RegionName == regionName && (b.CountryId == countryid || b.UserId == userid)).FirstOrDefault();


                            //}

                            if (region != null)
                            {

                                rd._region = region;
                                rName = regionName;
                                //if (sName != siteName)
                                //{
                                  
                                        site = sites.Where(b => b.SiteName.ToUpper() == siteName.ToUpper() && b.regionid == region.RegionID).FirstOrDefault(); //ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.CountryId == countryid || b.UserId == userid)).FirstOrDefault();


                                //}
                                if (site != null)
                                {
                                    rd._site = site;
                                    sName = siteName;
                                }
                                else
                                {
                                    res = siteName+ " site does not belongs to region " +regionName;
                                    return res;
                                    //if (sName != siteName)
                                    //{
                                      
                                    //        Site ST = new Site
                                    //        {
                                    //            CountryId = countryid,
                                    //            UserId = userid,
                                    //            SiteName = siteName,
                                    //            regionid = region.RegionID
                                    //        };
                                    //        ctx.Site.Add(ST);
                                    //        ctx.SaveChanges();
                                    //        rd._site = ST;
                                    //        site = ST;
                                     
                                    //    sName = siteName;
                                    //}
                                    //else
                                    //{
                                    //    rd._site = site;
                                    //    //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                    //    //if (site == null)
                                    //    //{
                                    //    //    site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && b.CountryId == ctx.User.Where(c => c.Id == userid).Select(iz => iz.CountryId).FirstOrDefault()).FirstOrDefault();

                                    //    //}
                                    //}
                                    //haserror = true;
                                    //errorDescription = errorDescription + " Site Doesn't Exist";
                                    ////dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
                                    //      changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Site Doesn't Exist", Color.Red);
                                }
                            }
                            else
                            {
                                res = regionName + " region may not preent either the current country or for current user";
                                return res;
                              //  region = regions.Where(b => b.RegionName.ToUpper() == regionName.ToUpper()).FirstOrDefault();
                                //if (rName != regionName)
                                //{
                                 
                                //        Region Rg = new Region
                                //        {
                                //            RegionName = regionName,
                                //            UserId = userid,
                                //            CountryId = countryid
                                //        };
                                //        ctx.Region.Add(Rg);
                                //        ctx.SaveChanges();
                                //        rName = regionName;
                                //        rd._region = Rg;
                                //        if (sName != siteName)
                                //        {
                                           

                                //                Site ST = new Site
                                //                {
                                //                    CountryId = countryid,
                                //                    UserId = userid,
                                //                    SiteName = siteName,
                                //                    regionid = Rg.RegionID
                                //                };
                                //                ctx.Site.Add(ST);
                                //                ctx.SaveChanges();
                                //                rd._site = ST;
                                //                site = ST;
                                           
                                //        }
                                //        else
                                //        {
                                //            rd._site = site;
                                //        }




                                   
                                //}
                          
                            }
                            // Dr["CategoryName"] = "";

                        }

                        if (pName != productName)
                        {

                            product = MP.Where(b => b.ProductName.ToUpper() == productName.ToUpper()).FirstOrDefault();  //ctx.MasterProduct.Where(b => b.ProductName == productName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();


                            pName = productName;
                        }
                        if (product==null)
                        {
                            res = productName + " Product doee not exists";
                        }
                        //if (product != null)
                            rd._product = product;
                        //else
                        //{
                        //    haserror = true;
                        //    errorDescription = errorDescription + " Product Doesn't Exist";
                        //    //  dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
                        //    //   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Product Doesn't Exist", Color.Red);
                        //}
                        //if (haserror == false)
                        //{

                            receivereportdata RD = new receivereportdata();





                            if (_forecastInfo.DataUsage == "DATA_USAGE3")
                            {
                                RD.CategoryName = rd._categoryName;
                                RD.RegionName = "";
                                RD.SiteName = "";
                                //Dr["CategoryName"] = rd._categoryName;
                                //Dr["RegionName"] = "";
                                //Dr["SiteName"] = "";
                            }
                            else
                            {
                                RD.CategoryName = "";
                                RD.RegionName = rd._regionName;
                                RD.SiteName = rd._siteName;
                                //Dr["CategoryName"] = "";
                                //Dr["RegionName"] = rd._regionName;
                                //Dr["SiteName"] = rd._siteName;
                            }
                            RD.ProName = rd._productName;
                            //Dr["ProName"] = rd._productName;
                            //rd._hasError = haserror;
                            //rd.errorDescription = errorDescription;


                            if (!Utility.Utility.IsDateTime(rd._duration))
                            {

                                //try
                                //{
                                    DateTime dd = Utility.Utility.DurationToDateTime(rd._duration);
                                    if (country.Period == "Yearly")
                                    {

                                        rd._duration = dd.Year.ToString();
                                        RD.Duration = Utility.Utility.DatetimeToDurationStr(country.Period, dd);
                                    //    Dr["Duration"] = Utility.Utility.DatetimeToDurationStr(country.Period, dd);
                                    }
                                    else
                                        RD.Duration = rd._duration;
                                //}
                                //catch (Exception ex)
                                //{
                                //    Dr["Duration"] = rd._duration;
                                //    rd._hasError = true;
                                //}

                            }
                            else
                            {

                                string datestr = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
                                //if (!rd._duration.StartsWith("Q"))
                                //{
                                    //rd._duration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
                                    //if (country.Period == "Yearly")
                                    //{
                                        RD.Duration = datestr;

                                    //}
                                    //else
                                    //{
                                    //    RD.Duration = rd._duration;

                                    //}
                                //}
                                //else
                                //{
                                //    {
                                //        RD.Duration = datestr;

                                //    }


                                //}

                            }



                            RD.Amount = rd._amount;
                            //Dr["StockOut"] = rd._stockout.ToString();
                            //Dr["InstrumentDownTime"] = rd._instrumentDownTime.ToString();
                           
                            //if (Utility.Utility.validDate(rd._duration, country.Period))
                            //    rd._hasError = true;

                            //if (rd._hasError == true && rd.errorDescription == "")
                                //rd.errorDescription = " Product Consumed Required ";
                        //  RD. = rd.errorDescription.ToString();
                            //if (rd._site != null)
                               RD.SiteID = rd._site.SiteID;
                            //else
                            //    Dr["SiteID"] = 0;
                            //if (rd._product != null)
                               RD.ProID = rd._product.ProductID;
                            //else
                            //    Dr["ProID"] = 0;
                            
                            if (_forecastInfo.DataUsage == "DATA_USAGE3")
                            {RD.CatID= rd._category.CategoryId;
                            }
                            else
                            {
                                RD.CatID = 0;
                            }
                          // = rd._rowno.ToString();
                            RD.Duration1= rd._duration;
                           // Dr["haserror"] = rd._hasError;
                           RD.forecastid= id;
                           _rdata.Add(RD);
                        //}
                        //  rdlist.Add(rd);


                        //rd.RowNo
                        colid++;
                        //errorDescription = "";
                    }
                    while (colid < g.Length && g[colid].ToString() != "");// dr.ItemArray.Length / ds.Tables[0].Rows.Count);
                }
            }

         
            bool isduplicate = false;
            try
            {
                List<ForecastSite> forecastSites = new List<ForecastSite>();
                List<Site> sitelst = new List<Site>();
                List<HistoricalData> hisdata = new List<HistoricalData>();
                hisdata = ctx.HistoricalData.ToList();
                forecastSites = ctx.ForecastSite.Where(b => b.ForecastInfoId == _forecastInfo.ForecastID).ToList();
                sitelst = ctx.Site.ToList();
                var ForecastSiteProduct= ctx.ForecastSiteProduct.ToList();
                if (_forecastInfo.DataUsage != "DATA_USAGE3")
                {
                    IList<HistoricalData> existingFsp = new List<HistoricalData>();
                    IList<ForecastSiteProduct> existingFsp1 = new List<ForecastSiteProduct>();
                    var getdistinctsiteids = _rdata.GroupBy(b => b.SiteID).Select(x => x.Key).ToList();
                    for (int i = 0; i < getdistinctsiteids.Count; i++)
                    {
                        fs = forecastSites.Where(b => b.SiteId == getdistinctsiteids[i]).FirstOrDefault();//ctx.ForecastSite.Where(b => b.SiteId == getdistinctsiteids[i] && b.ForecastInfoId == _forecastInfo.ForecastID).FirstOrDefault();
                        if (fs == null)
                        {
                            fs = new ForecastSite();
                            fs.SiteId = getdistinctsiteids[i];
                            fs.ForecastInfoId = _forecastInfo.ForecastID;
                            ctx.ForecastSite.Add(fs);
                            ctx.SaveChanges();

                        }
                        var dd = _rdata.Where(x => x.SiteID == getdistinctsiteids[i]).GroupBy(b => b.ProID).Select(x => x.Key).ToList();
                        decimal workingday = sites.Where(b => b.SiteID == getdistinctsiteids[i]).Select(x => x.WorkingDays).FirstOrDefault();

                        for (int j = 0; j < dd.Count; j++)
                        {

                            //  var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.Id && b.ProductID == dd[j]).ToList();
                       


                            var isexist1 = ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.Id && b.ProductID == dd[j]).ToList();
                            //    var isexist1 = ctx.HistoricalData.Where(b => b.SiteId == getdistinctsiteids[i] && b.ProductId == dd[j]).ToList();
                            var data1 = _rdata.Where(x => x.SiteID == getdistinctsiteids[i] && x.ProID == dd[j]).ToList();
                            for (int k = 0; k < data1.Count; k++)
                            {
                                if (!data1[k].haserror)
                                {
                                    ForecastSiteProduct fp1 = new ForecastSiteProduct
                                    {
                                        ForecastSiteID = fs.Id,
                                        ProductID = data1[k].ProID
                                    };
                                    // fp.ProductId = data[k].ProID;
                                    if (!Utility.Utility.IsDateTime(data1[k].Duration))
                                    {
                                        fp1.CDuration = data1[k].Duration;
                                        fp1.DurationDateTime = Utility.Utility.DurationToDateTime(data1[k].Duration);

                                    }
                                    else
                                    {
                                        fp1.DurationDateTime = DateTime.Parse(data1[k].Duration);
                                        fp1.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data1[k].Duration));
                                    }
                                    fp1.AmountUsed = data1[k].Amount;
                                    fp1.StockOut = data1[k].StockOut;
                                    fp1.InstrumentDowntime = data1[k].InstrumentDownTime;//b
                                    if (fp1.StockOut > 0)
                                    {
                                        int days = fp1.StockOut;
                                        // decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

                                        if (days >= workingday)
                                        {
                                            days = 0;
                                            fp1.StockOut = 0;
                                        }
                                        if (days >= 0)
                                            fp1.StockOut = days;
                                    }


                                    // var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

                                    if (isexist1.Count == 0)
                                    {
                                        fp1.Adjusted = fp1.AmountUsed;

                                        if ((fp1.InstrumentDowntime > 0 || fp1.StockOut > 0) && fp1.AmountUsed > 0)
                                            fp1.Adjusted = Utility.Utility.GetAdjustedVolume(fp1.AmountUsed, fp1.StockOut + fp1.InstrumentDowntime, country.Period, workingday);//b

                                        existingFsp1.Add(fp1);
                                    }
                                    else
                                    {
                                        var exist1 = isexist1.Where(x => x.CDuration == fp1.CDuration && x.DurationDateTime == fp1.DurationDateTime).FirstOrDefault();
                                        if (exist1 == null)
                                        {
                                            fp1.Adjusted = fp1.AmountUsed;

                                            if ((fp1.InstrumentDowntime > 0 || fp1.StockOut > 0) && fp1.AmountUsed > 0)
                                                fp1.Adjusted = Utility.Utility.GetAdjustedVolume(fp1.AmountUsed, fp1.StockOut + fp1.InstrumentDowntime, country.Period, workingday);//b

                                            existingFsp1.Add(fp1);
                                        }
                                        else
                                        {
                                            exist1.AmountUsed = data1[k].Amount;
                                            exist1.StockOut = data1[k].StockOut;
                                            exist1.InstrumentDowntime = data1[k].InstrumentDownTime;//b
                                            exist1.Adjusted = fp1.AmountUsed;
                                            if ((exist1.InstrumentDowntime > 0 || exist1.StockOut > 0) && exist1.AmountUsed > 0)
                                                exist1.Adjusted = Utility.Utility.GetAdjustedVolume(exist1.AmountUsed, exist1.StockOut + exist1.InstrumentDowntime, country.Period, workingday);//b

                                            //ctx.SaveChanges();
                                        }
                                    }
                                }
                            }

                        }
                    }

                    
                    for (int i = 0; i < getdistinctsiteids.Count; i++)
                    {
                        var dd = _rdata.Where(x => x.SiteID == getdistinctsiteids[i]).GroupBy(b => b.ProID).Select(x => x.Key).ToList();
                        decimal workingday = sites.Where(b => b.SiteID == getdistinctsiteids[i]).Select(x => x.WorkingDays).FirstOrDefault();
                        for (int j = 0; j < dd.Count; j++)
                        {
                            var isexist = hisdata.Where(b => b.SiteId == getdistinctsiteids[i] && b.ProductId == dd[j]).ToList();
                            var data = _rdata.Where(x => x.SiteID == getdistinctsiteids[i] && x.ProID == dd[j]).ToList();
                            for (int k = 0; k < data.Count; k++)
                            {
                                if (!data[k].haserror)
                                {
                                    HistoricalData fp = new HistoricalData();

                                    fp.SiteId = getdistinctsiteids[i];
                                    fp.ProductId = data[k].ProID;
                                    if (!Utility.Utility.IsDateTime(data[k].Duration))
                                    {
                                        fp.CDuration = data[k].Duration;
                                        fp.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);

                                    }
                                    else
                                    {
                                        fp.DurationDateTime = DateTime.Parse(data[k].Duration);
                                        fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
                                    }
                                    fp.AmountUsed = data[k].Amount;
                                    fp.StockOut = data[k].StockOut;
                                    fp.InstrumentDowntime = data[k].InstrumentDownTime;//b
                                    if (fp.StockOut > 0)
                                    {
                                        int days = fp.StockOut;
                                        // decimal workingday = ctx.Site.Where(b => b.SiteID == _rdata.receivereportdata[i].SiteID).Select(x => x.WorkingDays).FirstOrDefault();

                                        if (days >= workingday)
                                        {
                                            days = 0;
                                            fp.StockOut = 0;
                                        }
                                        if (days >= 0)
                                            fp.StockOut = days;
                                    }


                                    // var isexist = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == fs.ForecastInfoId && b.ProductID == _rdata.receivereportdata[i].ProID && b.CDuration == fp.CDuration && b.DurationDateTime == fp.DurationDateTime).FirstOrDefault();

                                    if (isexist.Count == 0)
                                    {
                                        fp.Adjusted = fp.AmountUsed;

                                        if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                            fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b

                                        existingFsp.Add(fp);
                                    }
                                    else
                                    {
                                        var exist1 = isexist.Where(x => x.CDuration == fp.CDuration && x.DurationDateTime == fp.DurationDateTime).FirstOrDefault();
                                        if (exist1 == null)
                                        {
                                            fp.Adjusted = fp.AmountUsed;

                                            if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                                fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, workingday);//b


                                            existingFsp.Add(fp);
                                        }
                                        else
                                        {
                                            exist1.AmountUsed = data[k].Amount;
                                            exist1.StockOut = data[k].StockOut;
                                            exist1.InstrumentDowntime = data[k].InstrumentDownTime;//b
                                            exist1.Adjusted = fp.AmountUsed;
                                            if ((exist1.InstrumentDowntime > 0 || exist1.StockOut > 0) && exist1.AmountUsed > 0)
                                                exist1.Adjusted = Utility.Utility.GetAdjustedVolume(exist1.AmountUsed, exist1.StockOut + exist1.InstrumentDowntime, country.Period, workingday);//b

                                            //ctx.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    ctx.HistoricalData.AddRange(existingFsp);
                    ctx.SaveChanges();
                    ctx.ForecastSiteProduct.AddRange(existingFsp1);
                    ctx.SaveChanges();
                    res = "Data saved successfully";
                }
                else
                {
                    IList<HistoricalData> existingFcp = new List<HistoricalData>();
                    IList<ForecastCategoryProduct> existingFcp1 = new List<ForecastCategoryProduct>();
                    var getdistinctsiteids = _rdata.GroupBy(b => b.CategoryName).Select(x => x.Key).ToList();
                    for (int i = 0; i < getdistinctsiteids.Count; i++)
                    {
                        var dd = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i]).GroupBy(b => b.ProID).Select(x => x.Key).ToList();
                        fcat = ctx.ForecastCategory.Where(b => b.CategoryName == getdistinctsiteids[i] && b.ForecastId == _forecastInfo.ForecastID).FirstOrDefault(); ;
                        if (fcat == null)
                        {
                            fcat = new ForecastCategory();
                            fcat.CategoryName = getdistinctsiteids[i];
                            fcat.ForecastId = _forecastInfo.ForecastID;
                            ctx.ForecastCategory.Add(fcat);
                            ctx.SaveChanges();

                        }
                        decimal workingday = 22;

                        for (int j = 0; j < dd.Count; j++)
                        {
                            var isexist = hisdata.Where(b => b.CategoryId == fcat.CategoryId && b.ProductId == dd[j]).ToList();

                            var data = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i] && x.ProID == dd[j]).ToList();
                            for (int k = 0; k < data.Count; k++)

                            {
                                HistoricalData fp = new HistoricalData();




                                isduplicate = false;

                                if (isexist.Count == 0)
                                    fp = new HistoricalData();
                                else
                                {
                                    var exist1 = isexist.Where(x => x.CDuration == data[k].Duration1).FirstOrDefault();
                                    if (exist1 != null)
                                    {
                                        isduplicate = true;
                                    }
                                }

                                fp.CategoryId = fcat.CategoryId;
                                fp.ProductId = data[k].ProID;


                                fp.AmountUsed = data[k].Amount;
                                fp.StockOut = data[k].StockOut;
                                fp.InstrumentDowntime = data[k].InstrumentDownTime;//b

                                if (!Utility.Utility.IsDateTime(data[k].Duration))
                                {
                                    fp.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);
                                    fp.CDuration = data[k].Duration;
                                }
                                else
                                {
                                    fp.DurationDateTime = DateTime.Parse(data[k].Duration);
                                    fp.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
                                }

                                if (fp.StockOut > 0)
                                {
                                    int days = fp.StockOut;


                                    if (days >= workingday)
                                    {
                                        days = 0;
                                        fp.StockOut = 0;
                                    }
                                    if (days >= 0)
                                        fp.StockOut = days;
                                }
                                if (!isduplicate)
                                {
                                    //ctx.ForecastCategoryProduct.Add(fp);
                                    //ctx.SaveChanges();

                                    //fp.Adjusted = fp.AmountUsed;
                                    //if (fp.AmountUsed == 0)
                                    //{
                                    //    try
                                    //    {
                                    //        Consumption cs = GetConsumption(fp.Product.Id, fp);
                                    //        fp.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
                                    //    }
                                    //    catch { fp.Adjusted = fp.AmountUsed; }
                                    //}
                                    if ((fp.InstrumentDowntime > 0 || fp.StockOut > 0) && fp.AmountUsed > 0)
                                        fp.Adjusted = Utility.Utility.GetAdjustedVolume(fp.AmountUsed, fp.StockOut + fp.InstrumentDowntime, country.Period, 22);//b
                                    existingFcp.Add(fp);
                                    // ctx.SaveChanges();
                                }






                            }



                            //////////forecastcategory
                            ///
                            var isexist1 = ctx.ForecastCategoryProduct.Where(b => b.CategoryID == fcat.CategoryId && b.ProductID == dd[j]).ToList();

                            var data1 = _rdata.Where(x => x.CategoryName == getdistinctsiteids[i] && x.ProID == dd[j]).ToList();
                            for (int k = 0; k < data1.Count; k++)

                            {
                                ForecastCategoryProduct fp1 = new ForecastCategoryProduct();




                                isduplicate = false;

                                if (isexist.Count == 0)
                                    fp1 = new ForecastCategoryProduct();
                                else
                                {
                                    var exist1 = isexist1.Where(x => x.CDuration == data[k].Duration1).FirstOrDefault();
                                    if (exist1 != null)
                                    {
                                        isduplicate = true;
                                    }
                                }

                                fp1.CategoryID = fcat.CategoryId;
                                fp1.ProductID = data[k].ProID;


                                fp1.AmountUsed = data[k].Amount;
                                fp1.StockOut = data[k].StockOut;
                                fp1.InstrumentDowntime = data[k].InstrumentDownTime;//b

                                if (!Utility.Utility.IsDateTime(data[k].Duration))
                                {
                                    fp1.DurationDateTime = Utility.Utility.DurationToDateTime(data[k].Duration);
                                    fp1.CDuration = data[k].Duration;
                                }
                                else
                                {
                                    fp1.DurationDateTime = DateTime.Parse(data[k].Duration);
                                    fp1.CDuration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(data[k].Duration));
                                }

                                if (fp1.StockOut > 0)
                                {
                                    int days = fp1.StockOut;


                                    if (days >= workingday)
                                    {
                                        days = 0;
                                        fp1.StockOut = 0;
                                    }
                                    if (days >= 0)
                                        fp1.StockOut = days;
                                }
                                if (!isduplicate)
                                {
                                    //ctx.ForecastCategoryProduct.Add(fp1);
                                    //ctx.SaveChanges();

                                    //fp1.Adjusted = fp1.AmountUsed;
                                    //if (fp1.AmountUsed == 0)
                                    //{
                                    //    try
                                    //    {
                                    //        Consumption cs = GetConsumption(fp1.Product.Id, fp1);
                                    //        fp1.Adjusted = Math.Round(cs.TotalConsumption / cs.NoConsumption, 2, MidpointRounding.ToEven);
                                    //    }
                                    //    catch { fp1.Adjusted = fp1.AmountUsed; }
                                    //}
                                    if ((fp1.InstrumentDowntime > 0 || fp1.StockOut > 0) && fp1.AmountUsed > 0)
                                        fp1.Adjusted = Utility.Utility.GetAdjustedVolume(fp1.AmountUsed, fp1.StockOut + fp1.InstrumentDowntime, country.Period, 22);//b
                                    existingFcp1.Add(fp1);
                                    // ctx.SaveChanges();
                                }






                            }
                        }

                    }

                    ctx.HistoricalData.AddRange(existingFcp);
                    ctx.ForecastCategoryProduct.AddRange(existingFcp1);
                    ctx.SaveChanges();
                    res = "Data saved successfully";
                }

            }
            catch (Exception ex)
            {

                res = ex.Message;
                return res;
            }
            return res;

        }


        public DataTable importconsumption(List<string[]> jArray, int id,int userid)
        {
        
            IList<ReportedData> _rdata;
            ConsumptionDT.Columns.Add("No");



            ConsumptionDT.Columns.Add("CategoryName");
            ConsumptionDT.Columns.Add("RegionName");
            ConsumptionDT.Columns.Add("SiteName");
            ConsumptionDT.Columns.Add("ProName");
            ConsumptionDT.Columns.Add("Duration");
            ConsumptionDT.Columns.Add("Amount");
            ConsumptionDT.Columns.Add("StockOut");
            ConsumptionDT.Columns.Add("InstrumentDownTime");
            ConsumptionDT.Columns.Add("Description");

            ConsumptionDT.Columns.Add("SiteID");
            ConsumptionDT.Columns.Add("ProID");
            ConsumptionDT.Columns.Add("CatID");
            ConsumptionDT.Columns.Add("Duration1");
            ConsumptionDT.Columns.Add("haserror");
            ConsumptionDT.Columns.Add("forecastid");
          GetDataRow_New(jArray, id,userid);

       
            //foreach (ReportedData rd in _rdata)
            //{
            //    DataRow Dr = DT.NewRow();

            //    Dr["No"] = rd._rowno.ToString();






            //    if (_forecastInfo.DataUsage == "DATA_USAGE3")
            //    {
            //        Dr["CategoryName"] = rd._categoryName;
            //        Dr["RegionName"] = "";
            //        Dr["SiteName"] = "";
            //    }
            //    else
            //    {
            //        Dr["CategoryName"] = "";
            //        Dr["RegionName"] = rd._regionName;
            //        Dr["SiteName"] = rd._siteName;


            //    }
            //    Dr["ProName"] = rd._productName;


            //    if (!Utility.Utility.IsDateTime(rd._duration))
            //    {

            //        try
            //        {
            //            DateTime dd = Utility.Utility.DurationToDateTime(rd._duration);
            //            if (rd._duration.StartsWith("Q") && (_forecastInfo.Period == "Yearly"))
            //            {

            //                rd._duration = dd.Year.ToString();
            //                Dr["Duration"] = Utility.Utility.DatetimeToDurationStr(_forecastInfo.Period, dd);
            //            }
            //            else
            //                Dr["Duration"] = rd._duration;
            //        }
            //        catch (Exception ex)
            //        {
            //            Dr["Duration"] = rd._duration;
            //            rd._hasError = true;
            //        }

            //    }
            //    else
            //    {

            //        string datestr = Utility.Utility.DatetimeToDurationStr(_forecastInfo.Period, DateTime.Parse(rd._duration));
            //        if (!rd._duration.StartsWith("Q"))
            //        {
            //            rd._duration = Utility.Utility.DatetimeToDurationStr(_forecastInfo.Period, DateTime.Parse(rd._duration));
            //            if (_forecastInfo.Period == "Yearly")
            //            {
            //                Dr["Duration"] = datestr;

            //            }
            //            else
            //            {
            //                Dr["Duration"] = rd._duration;

            //            }
            //        }
            //        else
            //        {
            //            {
            //                Dr["Duration"] = datestr;

            //            }


            //        }

            //    }



            //    Dr["Amount"] = rd._amount.ToString();
            //    Dr["StockOut"] = rd._stockout.ToString();
            //    Dr["InstrumentDownTime"] = rd._instrumentDownTime.ToString();
            //    //if (rd.HasError == true && rd.ErrorDescription == "")
            //    //    rd.ErrorDescription = " Product Consumed Required ";

            //    //     li.SubItems.Add(rd.ErrorDescription.ToString());
            //    if (Utility.Utility.validDate(rd._duration, _forecastInfo.Period))
            //        rd._hasError = true;

            //    if (rd._hasError == true && rd.errorDescription == "")
            //        rd.errorDescription = " Product Consumed Required ";
            //    Dr["Description"] = rd.errorDescription.ToString();

            //    Dr["SiteID"] = rd._site.SiteID;
            //    if(rd._product!=null)
            //    Dr["ProID"] = rd._product.ProductID;
            //    if (_forecastInfo.DataUsage == "DATA_USAGE3")
            //    {
            //        Dr["CatID"] = rd._category.CategoryId;
            //    }
            //    else
            //    {
            //        Dr["CatID"] = 0;
            //    }
            //    Dr["Duration1"] = rd._duration;
            //    Dr["haserror"] = rd._hasError;
            //    Dr["forecastid"] = id;
            //    DT.Rows.Add(Dr);
            //}







            return ConsumptionDT;
        }
        private void GetDataRow_New(List<string[]> jArray, int id, int userid)
        {
            string categoryName = null;
            string regionName;
            string siteName;
            string productName;
            string duration;
            decimal amount;
            int stockout;
            int instrumentDownTime;
            decimal adjusited;
            int rowno = 0;
            bool haserror;
            string cName = "";
            string rName = "";
            string sName = "";
            string pName = "";
            int errornos = 0;
            string errormsg = "";
            ForecastCategory fcategory = null;
            Models.Region region = null;
            Site site = null;
            MasterProduct product = null;
            //Test test = null;
            string errorDescription = "";
            IList<ReportedData> rdlist = new List<ReportedData>();
            string[] dr = jArray[0];
            _forecastInfo = ctx.ForecastInfo.Find(id);
            var countryid = ctx.User.Find(userid).CountryId;
            var country = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault();
            var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>

                x.Id

           ).FirstOrDefault();
            List<MasterProduct> MP = new List<MasterProduct>();
            MP = ctx.MasterProduct.Where(b => b.UserId == userid || b.UserId == adminuserid).ToList();
            List<Region> regions = new List<Region>();
            regions = ctx.Region.Where(b => b.CountryId == countryid || b.UserId == userid).ToList();
            List<Site> sites = new List<Site>();
            sites = ctx.Site.Where(b => b.CountryId == countryid || b.UserId == userid).ToList();
            for (int i = 1; i < jArray.Count; i++)//ds.Tables[0].Rows.Count
            {
                rowno++;
                haserror = false;
                if (jArray[i].Length > 0)
                {
                    string[] dr1 = jArray[i];
                    //string[] dr2 = jArray[i + 1];
                    //string[] dr3 = jArray[i + 2];
                    //string[] dr4 = jArray[i + 3];
                    string[] g = jArray[0];
                    int f = 3;
                    int colid;//0 
                    if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        colid = 2;
                    else
                        colid = 3;

                    ////int noofcolumn = 0;
                    ////noofcolumn = jArray[i].Length - 1;
                    ////bool newerror = false;
                    ////do
                    ////{
                    ////    if (dr1[noofcolumn] == "")
                    ////        dr1[noofcolumn] = "0";
                    ////    Decimal amount1 = Convert.ToDecimal(dr1[noofcolumn]);
                    ////    if (Convert.ToDecimal(dr1[noofcolumn]) == 0)
                    ////    {
                    ////        newerror = true;
                    ////        break;
                    ////    }

                    ////    noofcolumn--;
                    ////}
                    ////while (noofcolumn >= jArray[i].Length - 3);
                    ////if (newerror == true)
                    ////{
                    ////    errornos++;
                    ////    if (errornos > 20)
                    ////    {

                    ////        // throw new Exception("There too many problem with Consumption data, please troubleshoot and try to import again.");
                    ////    }
                    ////    errormsg = "The product is not consumed in last three months";

                    ////    //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
                    ////    errormsg = "Please Check excel Some product are not consumed in recent last three months";

                    ////    continue;

                    ////}
                    ////else
                    ////{
                    ////    if (_forecastInfo.DataUsage == "DATA_USAGE3")
                    ////        colid = 2;
                    ////    else
                    ////        colid = 3;

                    ////}
                    //  label7.Text = "Importing Data.....................";
                    do
                    {

                        regionName = "";
                        siteName = "";
                        errorDescription = "";
                        if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        {
                            categoryName = Convert.ToString(dr1[0]).Trim();//(dr[colid++])
                            productName = Convert.ToString(dr1[1]).Trim();   //(dr[colid++])product name
                        }
                        else
                        {
                            regionName = Convert.ToString(dr1[0]).Trim(); //(dr[colid++]) region name
                            siteName = Convert.ToString(dr1[1]).Trim();           //(dr[colid++]) site name
                            productName = Convert.ToString(dr1[2]).Trim();   //(dr[colid++])product name
                        }
                        double dt;
                        string date;
                        string[] datearr;
                        if (country.Period != "Monthly" || country.Period != "Bimonthly")
                        {
                            datearr = dr[colid].Split("/");
                            //date = datearr[1] + "/0" + datearr[0] + "/" + datearr[2];
                            // DateTime DT= new DateTime()
                            //DateTime dateTime10;
                            //CultureInfo provider = CultureInfo.InvariantCulture;
                            //// It throws Argument null exception  
                            //bool y = DateTime.TryParseExact(dr[colid], "MM/dd/yyyy", provider,DateTimeStyles.None,out dateTime10);
                            //duration = Convert.ToString(DateTime.FromOADate(Convert.ToDouble(dr[colid])));
                            //duration = Convert.ToString(dateTime10);
                            ////duration = Convert.ToString(DateTime.Parse(dr[colid]));

                            datearr = dr[colid].Split("/");
                            if (datearr[0].Length == 1)
                                date = datearr[1] + "/0" + datearr[0] + "/" + datearr[2];
                            else
                                date = datearr[1] + "/" + datearr[0] + "/" + datearr[2];
                            //  duration = Convert.ToString(DateTime.Parse(date));
                            // duration = Convert.ToString(DateTime.Parse(dr[colid]));


                            //duration = Convert.ToString(DateTime.Parse("01 / 12 / 2020"));
                            duration = Convert.ToString(DateTime.Parse(date));
                            //(g[f]) Convert.ToString(dr[colid++]); //  reporting period(duration)
                        }
                        else
                            duration = Convert.ToString(dr[colid]);
                        try
                        {
                            if (string.IsNullOrEmpty(dr1[colid]))
                                dr1[colid] = "0";
                            amount = Convert.ToDecimal(dr1[colid]);  //amount
                                                                     // amount = 0;
                                                                     //}
                            if (amount == 0)
                            {

                            }
                        }
                        catch
                        {
                            haserror = true;
                            amount = 0;
                        }
                        try
                        {
                            //if (string.IsNullOrEmpty(dr2[colid]))
                            //    dr2[colid] = "0";
                            stockout = 0;     //stock out
                        }
                        catch
                        {
                            haserror = true;
                            stockout = 0;
                        }
                        try
                        {
                            //  if (string.IsNullOrEmpty(dr3[colid]))
                            //   dr3[colid] = "0";
                            instrumentDownTime = 0;    //instrumentDownTime
                        }
                        catch
                        {
                            haserror = true;
                            instrumentDownTime = 0;
                        }


                        adjusited = 0;
                        ReportedData rd = null;
                        DataRow Dr = ConsumptionDT.NewRow();


                        if (_forecastInfo.DataUsage == "DATA_USAGE3")
                        {
                            rd = new ReportedData
                            {
                                _rowno = rowno,
                                _categoryName = categoryName,
                                //  _siteName = siteName,
                                _productName = productName,
                                _duration = duration,
                                _amount = amount,
                                _stockout = stockout,
                                _instrumentDownTime = instrumentDownTime
                                //  rowno,_categoryname categoryName, productName, duration, amount, stockout, instrumentDownTime
                            };
                           // (rowno, categoryName, productName, duration, amount, stockout, instrumentDownTime);//b
                            if (cName != categoryName)
                            {
                                if (!string.IsNullOrEmpty(categoryName))
                                {
                                    fcategory = ctx.ForecastCategory.Where(b => b.ForecastId == _forecastInfo.ForecastID && b.CategoryName == categoryName).FirstOrDefault();
                                }
                                else
                                    fcategory = null;
                                cName = categoryName;
                            }

                            if (fcategory != null)
                                rd._category = fcategory;
                            else
                            {
                                rd._hasError = true;


                                //   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Category Doesn't Exist", Color.Red);
                            }
                            //Dr["CategoryName"] = rd._categoryName;
                            //Dr["RegionName"] = "";
                            //Dr["SiteName"] = "";

                        }
                        else
                        {
                            rd = new ReportedData
                            {
                                _rowno = rowno,
                                _regionName = regionName,
                                _siteName = siteName,
                                _productName = productName,
                                _duration = duration,
                                _amount = amount,
                                _stockout = stockout,
                                _instrumentDownTime = instrumentDownTime

                            };//(rowno, regionName, siteName, productName, duration, amount, stockout, instrumentDownTime);//b

                            if (rName != regionName)
                            {
                                if (!string.IsNullOrEmpty(regionName))
                                {
                                    //region = ctx.Region.Where(b => b.RegionName == regionName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                    //if (region == null)
                                    //{
                                    region = regions.Where(b => b.RegionName == regionName ).FirstOrDefault();

                                    //  }

                                }
                                else
                                    region = null;

                            }

                            if (region != null)
                            {
                                rd._region = region;
                                rName = regionName;
                                if (sName != siteName)
                                {
                                    if (!string.IsNullOrEmpty(siteName))
                                    {
                                        //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                        //if (site == null)
                                        //{
                                        site = sites.Where(b => b.SiteName == siteName && b.regionid == region.RegionID).FirstOrDefault();

                                        //}
                                    }
                                    else
                                        site = null;

                                }
                                if (site != null)
                                {
                                    rd._site = site;
                                    sName = siteName;
                                }
                                else
                                {
                                    if (sName != siteName)
                                    {
                                        if (!string.IsNullOrEmpty(siteName))
                                        {
                                            Site ST = new Site
                                            {
                                                CountryId = countryid,
                                                UserId = userid,
                                                SiteName = siteName,
                                                regionid = region.RegionID
                                            };
                                            ctx.Site.Add(ST);
                                            ctx.SaveChanges();
                                            rd._site = ST;
                                            site = ST;
                                        }
                                     
                                    }
                                    else
                                    {
                                        rd._site = site;
                                        //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                        //if (site == null)
                                        //{
                                        //    site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && b.CountryId == ctx.User.Where(c => c.Id == userid).Select(iz => iz.CountryId).FirstOrDefault()).FirstOrDefault();

                                        //}
                                    }

                                    sName = siteName;
                                    //haserror = true;
                                    //errorDescription = errorDescription + " Site Doesn't Exist";
                                    ////dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
                                    //      changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Site Doesn't Exist", Color.Red);
                                }
                            }
                            else
                            {
                                //if (rName != regionName)
                                //{
                                //    if (!string.IsNullOrEmpty(regionName))
                                //    {
                                //        Region Rg = new Region
                                //        {
                                //            RegionName = regionName,
                                //            UserId = userid,
                                //            CountryId = countryid
                                //        };
                                //        ctx.Region.Add(Rg);
                                //        ctx.SaveChanges();
                                //        rName = regionName;
                                //        rd._region = Rg;
                                //        if (sName != siteName)
                                //        {
                                //            if (!string.IsNullOrEmpty(siteName))
                                //            {

                                //                Site ST = new Site
                                //                {
                                //                    CountryId = countryid,
                                //                    UserId = userid,
                                //                    SiteName = siteName,
                                //                    regionid = Rg.RegionID
                                //                };
                                //                ctx.Site.Add(ST);
                                //                ctx.SaveChanges();
                                //                rd._site = ST;
                                //                site = ST;
                                //            }
                                //            sName = siteName;
                                //        }
                                //        else
                                //        {
                                //            rd._site = site;
                                //        }




                                //    }
                                //}
                                //haserror = true;
                                // errorDescription = errorDescription + " Region Doesn't Exist";
                                //dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
                                //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Region Doesn't Exist", Color.Red);

                            }
                            // Dr["CategoryName"] = "";

                        }

                        if (pName != productName)
                        {
                            if (!string.IsNullOrEmpty(productName))
                                product = MP.Where(b => b.ProductName == productName).FirstOrDefault();
                            else
                                product = null;
                            pName = productName;
                        }

                        if (product != null)
                            rd._product = product;
                        else
                        {
                            haserror = true;
                            errorDescription = errorDescription + " Product Doesn't Exist";
                            //  dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
                            //   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Product Doesn't Exist", Color.Red);
                        }
                        if (haserror == false)
                        {
                            if (_forecastInfo.DataUsage == "DATA_USAGE3")
                            {
                                Dr["CategoryName"] = rd._categoryName;
                                Dr["RegionName"] = "";
                                Dr["SiteName"] = "";
                            }
                            else
                            {
                                Dr["CategoryName"] = "";
                                Dr["RegionName"] = rd._regionName;
                                Dr["SiteName"] = rd._siteName;
                            }

                            Dr["ProName"] = rd._productName;
                            rd._hasError = haserror;
                            rd.errorDescription = errorDescription;


                            if (!Utility.Utility.IsDateTime(rd._duration))
                            {

                                try
                                {
                                    DateTime dd = Utility.Utility.DurationToDateTime(rd._duration);
                                    if (rd._duration.StartsWith("Q") && (country.Period == "Yearly"))
                                    {

                                        rd._duration = dd.Year.ToString();
                                        Dr["Duration"] = Utility.Utility.DatetimeToDurationStr(country.Period, dd);
                                    }
                                    else
                                        Dr["Duration"] = rd._duration;
                                }
                                catch (Exception ex)
                                {
                                    Dr["Duration"] = rd._duration;
                                    rd._hasError = true;
                                }

                            }
                            else
                            {

                              //  string datestr = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
                                //if (!rd._duration.StartsWith("Q"))
                                //{
                                    rd._duration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
                                    //if (country.Period == "Yearly")
                                    //{
                                    //    Dr["Duration"] = datestr;

                                    //}
                                    //else
                                    //{
                                        Dr["Duration"] = rd._duration;

                                    //}
                                //}
                                //else
                                //{
                                //    {
                                //        Dr["Duration"] = datestr;

                                //    }


                                //}

                            }



                            Dr["Amount"] = rd._amount.ToString();
                            Dr["StockOut"] = rd._stockout.ToString();
                            Dr["InstrumentDownTime"] = rd._instrumentDownTime.ToString();
                            //if (rd.HasError == true && rd.ErrorDescription == "")
                            //    rd.ErrorDescription = " Product Consumed Required ";

                            //     li.SubItems.Add(rd.ErrorDescription.ToString());
                            if (Utility.Utility.validDate(rd._duration, country.Period))
                                rd._hasError = true;

                            if (rd._hasError == true && rd.errorDescription == "")
                                rd.errorDescription = " Product Consumed Required ";
                            Dr["Description"] = rd.errorDescription.ToString();
                            if (rd._site != null)
                                Dr["SiteID"] = rd._site.SiteID;
                            else
                                Dr["SiteID"] = 0;
                            if (rd._product != null)
                                Dr["ProID"] = rd._product.ProductID;
                            else
                                Dr["ProID"] = 0;

                            if (_forecastInfo.DataUsage == "DATA_USAGE3")
                            {
                                Dr["CatID"] = rd._category.CategoryId;
                            }
                            else
                            {
                                Dr["CatID"] = 0;
                            }
                            Dr["No"] = rd._rowno.ToString();
                            Dr["Duration1"] = rd._duration;
                            Dr["haserror"] = rd._hasError;
                            Dr["forecastid"] = id;
                            ConsumptionDT.Rows.Add(Dr);
                        }
                        //  rdlist.Add(rd);


                        //rd.RowNo
                        colid++;
                        errorDescription = "";
                    }
                    while (colid < g.Length && g[colid].ToString() != "");// dr.ItemArray.Length / ds.Tables[0].Rows.Count);
                }
            }
            //if (errormsg != "")
            //{
            //    label7.Text = errormsg;

            //}

            //dtexcelcolor.AcceptChanges();
            //// changecolorexcelcolumnaddByDatatable(dtexcelcolor);
            //if (dtexcelcolor.Rows.Count > 0)
            //{
            //    changecolorexcelcolumnaddByDatatable(dtexcelcolor);
            //}
            //  return rdlist;
        }

        // private IList<ReportedData> GetDataRow(List<string[]> jArray, int id,int userid)
        //private void GetDataRow(List<string[]> jArray, int id, int userid)
        //{
        //    string categoryName = null;
        //    string regionName;
        //    string siteName;
        //    string productName;
        //    string duration;
        //    decimal amount;
        //    int stockout;
        //    int instrumentDownTime;
        //    decimal adjusited;
        //    int rowno = 0;
        //    bool haserror;
        //    string cName = "";
        //    string rName = "";
        //    string sName = "";
        //    string pName = "";
        //    int errornos = 0;
        //    string errormsg = "";
        //    ForecastCategory fcategory = null;
        //    Models.Region region = null;
        //    Site site = null;
        //    MasterProduct product = null;
        //    //Test test = null;
        //    string errorDescription = "";
        //    IList<ReportedData> rdlist = new List<ReportedData>();
        //    string[] dr = jArray[0];
        //    _forecastInfo = ctx.ForecastInfo.Find(id);
        //    var countryid = ctx.User.Find(userid).CountryId;
        //    var country = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault();
        //    var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>

        //        x.Id

        //   ).FirstOrDefault();
        //    for (int i = 1; i < jArray.Count; i = i + 4)//ds.Tables[0].Rows.Count
        //    {
        //        rowno++;
        //        haserror = false;
        //        if (jArray[i].Length > 0)
        //        {
        //            string[] dr1 = jArray[i];
        //            string[] dr2 = jArray[i + 1];
        //            string[] dr3 = jArray[i + 2];
        //            string[] dr4 = jArray[i + 3];
        //            string[] g = jArray[0];
        //            int f = 3;
        //            int colid;//0 
        //            if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                colid = 3;
        //            else
        //                colid = 4;

        //            int noofcolumn = 0;
        //            noofcolumn = jArray[i].Length - 1;
        //            bool newerror = false;
        //            do
        //            {
        //                if (dr1[noofcolumn] == "")
        //                    dr1[noofcolumn] = "0";
        //                Decimal amount1 = Convert.ToDecimal(dr1[noofcolumn]);
        //                if (Convert.ToDecimal(dr1[noofcolumn]) == 0)
        //                {
        //                    newerror = true;
        //                    break;
        //                }

        //                noofcolumn--;
        //            }
        //            while (noofcolumn >= jArray[i].Length - 3);
        //            if (newerror == true)
        //            {
        //                errornos++;
        //                if (errornos > 20)
        //                {

        //                   // throw new Exception("There too many problem with Consumption data, please troubleshoot and try to import again.");
        //                }
        //                errormsg = "The product is not consumed in last three months";

        //                //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errormsg, Color.Red);
        //                errormsg = "Please Check excel Some product are not consumed in recent last three months";

        //                continue;

        //            }
        //            else
        //            {
        //                if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                    colid = 3;
        //                else
        //                    colid = 4;

        //            }
        //            //  label7.Text = "Importing Data.....................";
        //            do
        //            {

        //                regionName = "";
        //                siteName = "";
        //                errorDescription = "";
        //                if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                {
        //                    categoryName = Convert.ToString(dr1[0]).Trim();//(dr[colid++])
        //                    productName = Convert.ToString(dr1[1]).Trim();   //(dr[colid++])product name
        //                }
        //                else
        //                {
        //                    regionName = Convert.ToString(dr1[0]).Trim(); //(dr[colid++]) region name
        //                    siteName = Convert.ToString(dr1[1]).Trim();           //(dr[colid++]) site name
        //                    productName = Convert.ToString(dr1[2]).Trim();   //(dr[colid++])product name
        //                }
        //                double dt;
        //                string date;
        //                string[] datearr;
        //                if (country.Period != "Monthly" || country.Period != "Bimonthly")
        //                {
        //                    datearr = dr[colid].Split("/");
        //                    //date = datearr[1] + "/0" + datearr[0] + "/" + datearr[2];
        //                    // DateTime DT= new DateTime()
        //                    //DateTime dateTime10;
        //                    //CultureInfo provider = CultureInfo.InvariantCulture;
        //                    //// It throws Argument null exception  
        //                    //bool y = DateTime.TryParseExact(dr[colid], "MM/dd/yyyy", provider,DateTimeStyles.None,out dateTime10);
        //                    //duration = Convert.ToString(DateTime.FromOADate(Convert.ToDouble(dr[colid])));
        //                    //duration = Convert.ToString(dateTime10);
        //                    ////duration = Convert.ToString(DateTime.Parse(dr[colid]));

        //                    datearr = dr[colid].Split("/");
        //                    if (datearr[0].Length == 1)
        //                        date = datearr[1] + "/0" + datearr[0] + "/" + datearr[2];
        //                    else
        //                        date = datearr[1] + "/" + datearr[0] + "/" + datearr[2];
        //                    //  duration = Convert.ToString(DateTime.Parse(date));
        //                    // duration = Convert.ToString(DateTime.Parse(dr[colid]));


        //                    //duration = Convert.ToString(DateTime.Parse("01 / 12 / 2020"));
        //                    duration = Convert.ToString(DateTime.Parse(date));
        //                    //(g[f]) Convert.ToString(dr[colid++]); //  reporting period(duration)
        //                }
        //                else
        //                    duration = Convert.ToString(dr[colid]);
        //                try
        //                {
        //                    if (string.IsNullOrEmpty(dr1[colid]))
        //                        dr1[colid] = "0";
        //                    amount = Convert.ToDecimal(dr1[colid]);  //amount
        //                                                             // amount = 0;
        //                                                             //}
        //                    if (amount == 0)
        //                    {

        //                    }
        //                }
        //                catch
        //                {
        //                    haserror = true;
        //                    amount = 0;
        //                }
        //                try
        //                {
        //                    if (string.IsNullOrEmpty(dr2[colid]))
        //                        dr2[colid] = "0";
        //                    stockout = Convert.ToInt32(dr2[colid]);     //stock out
        //                }
        //                catch
        //                {
        //                    haserror = true;
        //                    stockout = 0;
        //                }
        //                try
        //                {
        //                    if (string.IsNullOrEmpty(dr3[colid]))
        //                        dr3[colid] = "0";
        //                    instrumentDownTime = Convert.ToInt32(dr3[colid]);     //instrumentDownTime
        //                }
        //                catch
        //                {
        //                    haserror = true;
        //                    instrumentDownTime = 0;
        //                }


        //                adjusited = 0;
        //                ReportedData rd = null;
        //                DataRow Dr = ConsumptionDT.NewRow();


        //                if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                {
        //                    rd = new ReportedData(rowno, categoryName, productName, duration, amount, stockout, instrumentDownTime);//b
        //                    if (cName != categoryName)
        //                    {
        //                        if (!string.IsNullOrEmpty(categoryName))
        //                        {
        //                            fcategory = ctx.ForecastCategory.Where(b => b.ForecastId == _forecastInfo.ForecastID && b.CategoryName == categoryName).FirstOrDefault();
        //                        }
        //                        else
        //                            fcategory = null;
        //                        cName = categoryName;
        //                    }

        //                    if (fcategory != null)
        //                        rd._category = fcategory;
        //                    else
        //                    {
        //                        rd._hasError = true;


        //                        //   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Category Doesn't Exist", Color.Red);
        //                    }
        //                    //Dr["CategoryName"] = rd._categoryName;
        //                    //Dr["RegionName"] = "";
        //                    //Dr["SiteName"] = "";

        //                }
        //                else
        //                {
        //                    rd = new ReportedData(rowno, regionName, siteName, productName, duration, amount, stockout, instrumentDownTime);//b

        //                    if (rName != regionName)
        //                    {
        //                        if (!string.IsNullOrEmpty(regionName))
        //                        {
        //                            //region = ctx.Region.Where(b => b.RegionName == regionName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
        //                            //if (region == null)
        //                            //{
        //                                region = ctx.Region.Where(b => b.RegionName == regionName && b.CountryId == ctx.User.Where(c => c.Id == userid).Select(iz => iz.CountryId).FirstOrDefault()).FirstOrDefault();

        //                          //  }

        //                        }
        //                        else
        //                            region = null;

        //                    }

        //                    if (region != null)
        //                    {
        //                        rd._region = region;
        //                        rName = regionName;
        //                        if (sName != siteName)
        //                        {
        //                            if (!string.IsNullOrEmpty(siteName))
        //                            {
        //                                //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
        //                                //if (site == null)
        //                                //{
        //                                    site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && b.CountryId == ctx.User.Where(c => c.Id == userid).Select(iz => iz.CountryId).FirstOrDefault()).FirstOrDefault();

        //                                //}
        //                            }
        //                            else
        //                                site = null;

        //                        }
        //                        if (site != null)
        //                        {
        //                            rd._site = site;
        //                            sName = siteName;
        //                        }
        //                        else
        //                        {
        //                            if (sName != siteName)
        //                            {
        //                                if (!string.IsNullOrEmpty(siteName))
        //                                {
        //                                    Site ST = new Site
        //                                    {
        //                                        CountryId = countryid,
        //                                        UserId = userid,
        //                                        SiteName = siteName,
        //                                        regionid = region.RegionID
        //                                    };
        //                                    ctx.Site.Add(ST);
        //                                    ctx.SaveChanges();
        //                                    rd._site = ST;
        //                                    site = ST;
        //                                }
        //                                sName = siteName;
        //                            }
        //                            else
        //                            {
        //                                rd._site = site;
        //                                //site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
        //                                //if (site == null)
        //                                //{
        //                                //    site = ctx.Site.Where(b => b.SiteName == siteName && b.regionid == region.RegionID && b.CountryId == ctx.User.Where(c => c.Id == userid).Select(iz => iz.CountryId).FirstOrDefault()).FirstOrDefault();

        //                                //}
        //                            }
        //                            //haserror = true;
        //                            //errorDescription = errorDescription + " Site Doesn't Exist";
        //                            ////dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
        //                            //      changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Site Doesn't Exist", Color.Red);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (rName != regionName)
        //                        {
        //                            if (!string.IsNullOrEmpty(regionName))
        //                            {
        //                                Region Rg = new Region
        //                                {
        //                                    RegionName = regionName,
        //                                    UserId = userid,
        //                                    CountryId = countryid
        //                                };
        //                                ctx.Region.Add(Rg);
        //                                ctx.SaveChanges();
        //                                rName = regionName;
        //                                rd._region = Rg;
        //                                if (sName != siteName)
        //                                {
        //                                    if (!string.IsNullOrEmpty(siteName))
        //                                    {

        //                                        Site ST = new Site
        //                                        {
        //                                            CountryId = countryid,
        //                                            UserId = userid,
        //                                            SiteName = siteName,
        //                                            regionid = Rg.RegionID
        //                                        };
        //                                        ctx.Site.Add(ST);
        //                                        ctx.SaveChanges();
        //                                        rd._site = ST;
        //                                        site = ST;
        //                                    }
        //                                    sName = siteName;
        //                                }
        //                                else
        //                                {
        //                                    rd._site = site;
        //                                }




        //                            }
        //                        }
        //                        //haserror = true;
        //                        // errorDescription = errorDescription + " Region Doesn't Exist";
        //                        //dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
        //                        //  changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Region Doesn't Exist", Color.Red);

        //                    }
        //                   // Dr["CategoryName"] = "";

        //                }

        //                if (pName != productName)
        //                {
        //                    if (!string.IsNullOrEmpty(productName))
        //                        product = ctx.MasterProduct.Where(b => b.ProductName == productName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
        //                    else
        //                        product = null;
        //                    pName = productName;
        //                }

        //                if (product != null)
        //                    rd._product = product;
        //                else
        //                {
        //                    haserror = true;
        //                    errorDescription = errorDescription + " Product Doesn't Exist";
        //                    //  dtexcelcolor.Rows.Add(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, errorDescription, Color.Red);
        //                    //   changecolorexcelcolumnadd(LqtUtil.sheetname.Remove(LqtUtil.sheetname.IndexOf('$')).Trim('\''), i + 1, " Product Doesn't Exist", Color.Red);
        //                }
        //                if (haserror == false)
        //                {
        //                    if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                    {
        //                        Dr["CategoryName"] = rd._categoryName;
        //                        Dr["RegionName"] ="";
        //                        Dr["SiteName"] = "";
        //                    }
        //                    else
        //                    {
        //                        Dr["CategoryName"] = "";
        //                        Dr["RegionName"] = rd._regionName;
        //                        Dr["SiteName"] = rd._siteName;
        //                    }

        //                    Dr["ProName"] = rd._productName;
        //                    rd._hasError = haserror;
        //                    rd.errorDescription = errorDescription;


        //                    if (!Utility.Utility.IsDateTime(rd._duration))
        //                    {

        //                        try
        //                        {
        //                            DateTime dd = Utility.Utility.DurationToDateTime(rd._duration);
        //                            if (rd._duration.StartsWith("Q") && (country.Period == "Yearly"))
        //                            {

        //                                rd._duration = dd.Year.ToString();
        //                                Dr["Duration"] = Utility.Utility.DatetimeToDurationStr(country.Period, dd);
        //                            }
        //                            else
        //                                Dr["Duration"] = rd._duration;
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            Dr["Duration"] = rd._duration;
        //                            rd._hasError = true;
        //                        }

        //                    }
        //                    else
        //                    {

        //                        string datestr = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
        //                        if (!rd._duration.StartsWith("Q"))
        //                        {
        //                            rd._duration = Utility.Utility.DatetimeToDurationStr(country.Period, DateTime.Parse(rd._duration));
        //                            if (country.Period == "Yearly")
        //                            {
        //                                Dr["Duration"] = datestr;

        //                            }
        //                            else
        //                            {
        //                                Dr["Duration"] = rd._duration;

        //                            }
        //                        }
        //                        else
        //                        {
        //                            {
        //                                Dr["Duration"] = datestr;

        //                            }


        //                        }

        //                    }



        //                    Dr["Amount"] = rd._amount.ToString();
        //                    Dr["StockOut"] = rd._stockout.ToString();
        //                    Dr["InstrumentDownTime"] = rd._instrumentDownTime.ToString();
        //                    //if (rd.HasError == true && rd.ErrorDescription == "")
        //                    //    rd.ErrorDescription = " Product Consumed Required ";

        //                    //     li.SubItems.Add(rd.ErrorDescription.ToString());
        //                    if (Utility.Utility.validDate(rd._duration, country.Period))
        //                        rd._hasError = true;

        //                    if (rd._hasError == true && rd.errorDescription == "")
        //                        rd.errorDescription = " Product Consumed Required ";
        //                    Dr["Description"] = rd.errorDescription.ToString();
        //                    if (rd._site != null)
        //                        Dr["SiteID"] = rd._site.SiteID;
        //                    else
        //                        Dr["SiteID"] = 0;
        //                    if (rd._product != null)
        //                        Dr["ProID"] = rd._product.ProductID;
        //                    else
        //                        Dr["ProID"] = 0;

        //                    if (_forecastInfo.DataUsage == "DATA_USAGE3")
        //                    {
        //                        Dr["CatID"] = rd._category.CategoryId;
        //                    }
        //                    else
        //                    {
        //                        Dr["CatID"] = 0;
        //                    }
        //                    Dr["No"] = rd._rowno.ToString();
        //                    Dr["Duration1"] = rd._duration;
        //                    Dr["haserror"] = rd._hasError;
        //                    Dr["forecastid"] = id;
        //                    ConsumptionDT.Rows.Add(Dr);
        //                }
        //                //  rdlist.Add(rd);


        //                //rd.RowNo
        //                colid++;
        //                errorDescription = "";
        //            }
        //            while (colid < g.Length && g[colid].ToString() != "");// dr.ItemArray.Length / ds.Tables[0].Rows.Count);
        //        }
        //    }
        //    //if (errormsg != "")
        //    //{
        //    //    label7.Text = errormsg;

        //    //}

        //    //dtexcelcolor.AcceptChanges();
        //    //// changecolorexcelcolumnaddByDatatable(dtexcelcolor);
        //    //if (dtexcelcolor.Rows.Count > 0)
        //    //{
        //    //    changecolorexcelcolumnaddByDatatable(dtexcelcolor);
        //    //}
        //  //  return rdlist;
        //}

        private string validateandsavesiteinstrument(string fullpath, string sheetname, string Fileextension, IFormFile file,int userid,Matchrule matchrule)
        {
            string str = "";
            try
            {
                int error = 0;
                int noofsave = 0;
            
                ISheet sheet;
                IWorkbook workbook = null;
                IFont Font;
                ICellStyle cellStyle;
                string regionName;
                string testingArea;
                string siteName;
                string instrumentName;
                int quantity;
                int percentRun;
                SiteInstrument Sins = null;
                Site site = null;
                TestingArea TA = null;
                Instrument ins = null;
                List<SiteInstrument> SIList = new List<SiteInstrument>();
                using (var stream = new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (Fileextension == ".xls")
                    {

                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook  
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook   
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    //int cellCount = headerRow.LastCellNum;
                    //if (6 > cellCount)
                    //{
                    //    str = "Imported " + sheetname + " Sheet has less columns than needed.";
                    //    return str;
                    //}
                    //if (6 < cellCount)
                    //{
                    //    str = "Imported " + sheetname + " Sheet has too many columns.";
                    //    return str;
                    //}
                    //if (sheet.LastRowNum == 0)
                    //{
                    //    str = "Imported " + sheetname + " Sheet is empty.";
                    //    return str;
                    //}

          //          { type: "string", nullable: false, value: "Region", valueT: "" },
          //{ type: "string", nullable: false, value: "Site Name", valueT: "" },
          //{ type: "string", nullable: false, value: "Testing Area", valueT: "" },
          //{ type: "string", nullable: false, value: "Instrument Name", valueT: "" },
          //{ type: "number", nullable: false, value: "Quantity", valueT: "" },
          //{ type: "number", nullable: false, value: "%Run", valueT: "" },


                        int regionindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Region").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int siteindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Site Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int testingareaindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Testing Area").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int insindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Instrument Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;

                    int Quantityindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Quantity").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int Runindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "%Run").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                 //   int monthlycontrolindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Monthly Control Test").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                 //   int quartlycontrolindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Quarterly control Test").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //for (int j = row.FirstCellNum; j < cellCount; j++)
                        //{
                        regionName = Convert.ToString(row.GetCell(regionindex)).Trim();   //region name

                        siteName = Convert.ToString(row.GetCell(siteindex)).Trim();
                        testingArea = Convert.ToString(row.GetCell(testingareaindex)).Trim();
                        instrumentName = Convert.ToString(row.GetCell(insindex)).Trim();
                        try
                        {
                            quantity = int.Parse(Convert.ToString(row.GetCell(Quantityindex)));
                        }
                        catch
                        { quantity = 1; }
                        try
                        {
                            percentRun = int.Parse(Convert.ToString(row.GetCell(Runindex)));
                        }
                        catch { percentRun = 100; }

                        if (!string.IsNullOrEmpty(siteName))
                        {
                            site = ctx.Site.Where(b => b.SiteName == siteName &&  b.UserId==userid).FirstOrDefault();
                            if (site != null)
                            {
                                if (!string.IsNullOrEmpty(testingArea))
                                {
                                    TA = ctx.TestingArea.Where(b => b.AreaName == testingArea && b.UserId==userid).FirstOrDefault();
                                    if (TA != null && !string.IsNullOrEmpty(instrumentName))
                                    {
                                        ins = ctx.Instrument.Where(b => b.InstrumentName == instrumentName && b.UserId==userid).FirstOrDefault();
                                        if (ins != null)
                                        {


                                            if (SIList.Where(b => b.InstrumentID == ins.InstrumentID && b.SiteID == site.SiteID).FirstOrDefault() == null)
                                            {

                                                var SiteInstrument = ctx.siteinstrument.Where(b => b.InstrumentID == ins.InstrumentID && b.SiteID == site.SiteID).FirstOrDefault();
                                                if (SiteInstrument != null)
                                                {

                                                }
                                                else
                                                {
                                                    Sins = new SiteInstrument();
                                                    Sins.InstrumentID = ins.InstrumentID;
                                                    Sins.SiteID = site.SiteID;
                                                    Sins.Quantity = quantity;
                                                    Sins.TestRunPercentage = percentRun;
                                                    Sins.UserId = userid;
                                                    SIList.Add(Sins);
                                                    //ctx.siteinstrument.Add(Sins);
                                                    //ctx.SaveChanges();
                                                    noofsave = noofsave + 1;
                                                }
                                            }
                                          else
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }




                        if (error > 20)
                        {
                            str = "There too many problem with Site Instrument data, please troubleshoot and try to import again.";
                            return str;
                        }
                        //    sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        //    }
                        //}
                        //    sb.AppendLine("</tr>");
                    }
                    ctx.SaveChanges();
                    str = str + Environment.NewLine + noofsave + " Site Instrument  are imported and saved successfully.";
                    if (error > 0)
                    {
                        str = str + Environment.NewLine + error + " Site Instrument  Failed.Please check excel and correct Site Instrument Entry";
                    }
                }
                if(SIList.Count>0)
                {
                    ctx.siteinstrument.AddRange(SIList);
                    ctx.SaveChanges();
                }
                //  sb.Append("</table>");
                return str;
            }
            catch (Exception ex)
            {
                str = "The file is corrupt and import is unsuccessful. Please correct the file";
                return str;
            }
        }

        private string validateandsaveconsumables(string fullpath, string sheetname, string Fileextension, IFormFile file,int userid,Matchrule matchrule)
        {
            string str = "";
            var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>

                 x.Id

            ).FirstOrDefault();
            try
            {
                int error = 0;
                int noofsave = 0;
             
                ISheet sheet;
                IWorkbook workbook = null;
                IFont Font;
                ICellStyle cellStyle;
                string testName;
                string insName;
                string proName;
                string period;
                int noOfTest;
                decimal rate;
                string tName = "";
                string iName = "";
                bool isForTest = false;
                bool isForPeriod = false;
                bool isForInstrument = false;
                MasterConsumable cons = null;
                Test test = null;
                Instrument instrument = null;
                MasterProduct MP = null;
                ConsumableUsage CU = null;
                bool isnew = false;


                List<ConsumableUsage> CUList = new List<ConsumableUsage>();
                using (var stream = new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (Fileextension == ".xls")
                    {

                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook  
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook   
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                                                      //int cellCount = headerRow.LastCellNum;
                                                      //if (9 > cellCount)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet has less columns than needed.";
                                                      //    return str;
                                                      //}
                                                      //if (9 < cellCount)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet has too many columns.";
                                                      //    return str;
                                                      //}
                                                      //if (sheet.LastRowNum == 0)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet is empty.";
                                                      //    return str;
                                                      //}
          //          { type: "string", nullable: false, value: "Test Name", valueT: "" },
          //{ type: "string", nullable: false, value: "Instrument Name", valueT: "" },
          //{ type: "string", nullable: false, value: "Product Name", valueT: "" },
          //{ type: "string", nullable: false, value: "Period", valueT: "" },
          //{ type: "string", nullable: true, value: "Number Of Test", valueT: "" },
          //{ type: "number", nullable: false, value: "Rate", valueT: "" },
          //{ type: "string", nullable: true, value: "Per Test", valueT: "" },
          //{ type: "string", nullable: false, value: "Per Period", valueT: "" },
          //{ type: "string", nullable: true, value: "Per Instrument", valueT: "" },


                      int Testindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Test Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int Instrumentindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Instrument Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int Productindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Product Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int Periodindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Period").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int nooftestindex= headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Number Of Test").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int ratendex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Rate").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int pertestndex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Per Test").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int perperiodindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Per Period").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int perinsindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Per Instrument").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //for (int j = row.FirstCellNum; j < cellCount; j++)
                        //{

                        testName = Convert.ToString(row.GetCell(Testindex)).Trim();   //region name
                        insName = Convert.ToString(row.GetCell(Instrumentindex)).Trim();
                        proName = Convert.ToString(row.GetCell(Productindex)).Trim();
                        period = Convert.ToString(row.GetCell(Periodindex)).Trim();
                        try
                        {
                            noOfTest = int.Parse(Convert.ToString(row.GetCell(nooftestindex)));
                        }
                        catch
                        {
                            noOfTest = 0;
                        }
                        try
                        {
                            rate = decimal.Parse(Convert.ToString(row.GetCell(ratendex)));
                        }
                        catch
                        {
                            continue;
                        }

                        int tScore = 0;
                        int pScore = 0;
                        int iScore = 0;
                        if (Convert.ToString(row.GetCell(pertestndex)).ToLower().Trim() == "yes") tScore = 1;
                        if (Convert.ToString(row.GetCell(perperiodindex)).ToLower().Trim() == "yes") pScore = 1;
                        if (Convert.ToString(row.GetCell(perinsindex)).ToLower().Trim() == "yes") iScore = 1;
                        try { isForTest = Convert.ToBoolean((tScore)); }
                        catch { isForTest = false; }
                        try { isForPeriod = Convert.ToBoolean((pScore)); }
                        catch { isForPeriod = false; }
                        try { isForInstrument = Convert.ToBoolean((iScore)); }
                        catch { isForInstrument = false; }


                        if (!string.IsNullOrEmpty(Convert.ToString(row.GetCell(Testindex))))
                        {
                            test = ctx.Test.Where(b => b.TestName == testName && (b.UserId==userid || b.UserId== adminuserid)).FirstOrDefault();
                       


                        }

                        if (test != null)
                        {
                            // rd.Test = test;


                            if (!String.IsNullOrEmpty(insName))
                                instrument = ctx.Instrument.Where(b => b.InstrumentName == insName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                            else
                                instrument = null;

                            if (isForInstrument == true)
                            {

                                if (instrument != null)
                                {

                                    if (!String.IsNullOrEmpty(proName))
                                    {
                                        MP = ctx.MasterProduct.Where(b => b.ProductName == proName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                        if (MP == null)
                                        {
                                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Product Name is required";
                                            error = error + 1;
                                            continue;

                                        }
                                        else
                                        {
                                            if (!isnew)
                                            {
                                                CU = ctx.ConsumableUsage.Where(b => b.PerInstrument == true && b.InstrumentId == instrument.InstrumentID && b.ProductId == MP.ProductID).FirstOrDefault();
                                                if (CU != null)
                                                {
                                                    //str = str + Environment.NewLine + "Error in Row No." + i + ": Consumable Usage already exists";
                                                    //error = error + 1;
                                                    //continue;

                                                }
                                                else
                                                {
                                                    cons = ctx.MasterConsumable.Where(b => b.TestId == test.TestID).FirstOrDefault();
                                                    if (cons == null)
                                                    {
                                                        cons = new MasterConsumable();
                                                        cons.TestId = test.TestID;
                                                        cons.TestingAreaId = test.TestingAreaID;
                                                        cons.UserId = userid;
                                                        ctx.MasterConsumable.Add(cons);
                                                        ctx.SaveChanges();
                                                    }

                                                    CU = new ConsumableUsage();
                                                    CU.ConsumableId = cons.MasterCID;
                                                    CU.PerPeriod = false;
                                                    CU.NoOfTest = noOfTest;
                                                    CU.UsageRate = rate;
                                                    CU.PerTest = false;
                                                    CU.PerInstrument = true;
                                                    CU.ProductId = MP.ProductID;
                                                    CU.Period = period;
                                                    CU.InstrumentId = instrument.InstrumentID;
                                                    CU.UserId = userid;
                                                    CUList.Add(CU);
                                                    //ctx.ConsumableUsage.Add(CU);
                                                    //ctx.SaveChanges();
                                                    noofsave = noofsave + 1;
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Product Name is required";
                                        error = error + 1;
                                        continue;
                                    }
                                }

                            }// else
                            // haserror = true;
                            if (isForPeriod == true)
                            {
                                if (!String.IsNullOrEmpty(proName) && !String.IsNullOrEmpty(period))
                                {
                                    MP = ctx.MasterProduct.Where(b => b.ProductName == proName && b.UserId==userid).FirstOrDefault();
                                    if (MP == null)
                                    {
                                        str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Product Name is required";
                                        error = error + 1;
                                        continue;

                                    }
                                    else
                                    {
                                        if (!isnew)
                                        {
                                            CU = ctx.ConsumableUsage.Where(b => b.PerPeriod == true && b.ProductId == MP.ProductID).FirstOrDefault();
                                            if (CU != null)
                                            {
                                                //    str = str + Environment.NewLine + "Error in Row No." + i + ":Consumable Usage already exists";
                                                //    error = error + 1;
                                                //    continue;
                                            }
                                            else
                                            {
                                                cons = ctx.MasterConsumable.Where(b => b.TestId == test.TestID).FirstOrDefault();
                                                if (cons == null)
                                                {
                                                    cons = new MasterConsumable();
                                                    cons.TestId = test.TestID;
                                                    cons.TestingAreaId = test.TestingAreaID;
                                                    cons.UserId = userid;
                                                    ctx.MasterConsumable.Add(cons);
                                                    ctx.SaveChanges();
                                                }
                                                CU = new ConsumableUsage();
                                                CU.ConsumableId = cons.MasterCID;
                                                CU.PerPeriod = true;
                                                CU.NoOfTest = noOfTest;
                                                CU.UsageRate = rate;
                                                CU.PerTest = false;
                                                CU.PerInstrument = false;
                                                CU.ProductId = MP.ProductID;
                                                CU.Period = period;
                                                CU.UserId = userid;
                                                CUList.Add(CU);
                                                //ctx.ConsumableUsage.Add(CU);
                                                //ctx.SaveChanges();
                                                noofsave = noofsave + 1;
                                                continue;

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Product Name is required";
                                    error = error + 1;
                                    continue;
                                }
                            }
                            if (isForTest == true)
                            {
                                if (!String.IsNullOrEmpty(proName) && noOfTest > 0)
                                {
                                    MP = ctx.MasterProduct.Where(b => b.ProductName == proName && b.UserId==userid).FirstOrDefault();
                                    if (MP == null)
                                    {
                                        str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ": Product Name is required";
                                        error = error + 1;
                                        continue;

                                    }
                                    else
                                    {
                                        if (!isnew)
                                        {
                                            CU = ctx.ConsumableUsage.Where(b => b.PerTest == true && b.ProductId == MP.ProductID).FirstOrDefault();
                                            if (CU != null)
                                            {
                                                //str = str + Environment.NewLine + "Error in Row No." + i + ": Consumable Usage already exists";
                                                //error = error + 1;
                                                //continue;

                                            }
                                            else
                                            {
                                                cons = ctx.MasterConsumable.Where(b => b.TestId == test.TestID).FirstOrDefault();
                                                if (cons == null)
                                                {
                                                    cons = new MasterConsumable();
                                                    cons.TestId = test.TestID;
                                                    cons.TestingAreaId = test.TestingAreaID;
                                                    cons.UserId = userid;
                                                    ctx.MasterConsumable.Add(cons);
                                                    ctx.SaveChanges();
                                                }
                                                CU = new ConsumableUsage();
                                                CU.ConsumableId = cons.MasterCID;
                                                CU.PerPeriod = false;
                                                CU.NoOfTest = noOfTest;
                                                CU.UsageRate = rate;
                                                CU.PerTest = true;
                                                CU.PerInstrument = false;
                                                CU.ProductId = MP.ProductID;
                                                CU.Period = period;
                                                CU.InstrumentId = instrument.InstrumentID;
                                                CU.UserId = userid;
                                                CUList.Add(CU);
                                                //ctx.ConsumableUsage.Add(CU);
                                                //ctx.SaveChanges();
                                                noofsave = noofsave + 1;
                                                continue;
                                            }
                                        }
                                    }
                                }
                                else
                                {

                                    str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Product Name is required";
                                    error = error + 1;
                                    continue;

                                }
                            }

                        }
                        else
                        {

                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ": Test Name is required";
                            error = error + 1;
                            continue;

                        }



                        if (error > 20)
                        {
                            str = "There too many problem with Consumable Usage data, please troubleshoot and try to import again.";
                            return str;
                        }
                        //    sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        //    }
                        //}
                        //    sb.AppendLine("</tr>");
                    }
                    ctx.SaveChanges();
                    str = str + Environment.NewLine + noofsave + " Consumable Usage  are imported and saved successfully.";
                    if (error > 0)
                    {
                        str = str + Environment.NewLine + error + " Consumable Usage  Failed.Please check excel and correct Consumable Usage Entry";
                    }
                }

                if(CUList.Count>0)
                {
                    ctx.ConsumableUsage.AddRange(CUList);
                    ctx.SaveChanges();
                }
                //  sb.Append("</table>");
                return str;
            }
            catch (Exception ex)
            {
                str = "The file is corrupt and import is unsuccessful. Please correct the file";
                return str;
            }
        }
        private string validateandsavetestproduct(string fullpath, string sheetname, string Fileextension, IFormFile file,int userid,Matchrule matchrule)
        {
            string str = "";
            try
            {
                int error = 0;
                int noofsave = 0;
           
                ISheet sheet;
                IWorkbook workbook = null;
                IFont Font;
                ICellStyle cellStyle;
                string testName;
                string insName;
                string proName;
                decimal rate;
                string tName = "";
                string iName = "";
                string pName = "";
                bool isForControl = false;
                bool isForTest = false;

                Test test = null;
                Instrument instrument = null;
                MasterProduct product = null;
                ProductUsage PU = null;
                List<ProductUsage> PUList = new List<ProductUsage>();
                var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>

      x.Id

 ).FirstOrDefault();
                using (var stream = new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (Fileextension == ".xls")
                    {

                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook  
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook   
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                                                      //int cellCount = headerRow.LastCellNum;
                                                      //if (5 > cellCount)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet has less columns than needed.";
                                                      //    return str;
                                                      //}
                                                      //if (5 < cellCount)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet has too many columns.";
                                                      //    return str;
                                                      //}
                                                      //if (sheet.LastRowNum == 0)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet is empty.";
                                                      //    return str;
                                                      //}

          //          { type: "string", nullable: false, value: "Test Name", valueT: "" },
          //{ type: "string", nullable: false, value: "Instrument", valueT: "" },
          //{ type: "string", nullable: true, value: "Product", valueT: "" },
          //{ type: "number", nullable: true, value: "Rate", valueT: "" },
          //{ type: "number", nullable: true, value: "Is For Control", valueT: "" },



                      int testindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Test Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int insindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Instrument").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;

                    int proindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Product").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int Rateindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Rate").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int isforcontrolindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Is For Control").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //for (int j = row.FirstCellNum; j < cellCount; j++)
                        //{










                        testName = Convert.ToString(row.GetCell(testindex)).Trim();   //region name
                        insName = Convert.ToString(row.GetCell(insindex)).Trim();
                        proName = Convert.ToString(row.GetCell(proindex)).Trim();
                        try
                        {
                            rate = decimal.Parse(Convert.ToString(row.GetCell(Rateindex)));
                        }
                        catch
                        {
                            continue;
                        }
                        //try { isForControl = Convert.ToBoolean(int.Parse(Convert.ToString(dr[4]))); }
                        //catch { isForControl = false; }
                        //try { isForTest = Convert.ToBoolean(int.Parse(Convert.ToString(dr[5]))); }
                        //catch { isForTest = false; }
                        int cScore = 0;
                        int tScore = 0;
                        string _isForControl = Convert.ToString(row.GetCell(isforcontrolindex));
                        //string _isForTest = Convert.ToString(row.GetCell(5));
                        if (_isForControl.ToLower().Trim() == "yes") cScore = 1;
                      //  if (_isForTest.ToLower().Trim() == "yes") tScore = 1;
                        //try { isForControl = Convert.ToBoolean((Convert.ToString(dr[4]))); }
                        //catch { isForControl = false; }
                        //try { isForTest = Convert.ToBoolean((Convert.ToString(dr[5]))); }
                        //catch { isForTest = false; }


                        try { isForControl = Convert.ToBoolean(cScore); }
                        catch { isForControl = false; }
                        //try { isForTest = Convert.ToBoolean((tScore)); }
                        //catch { isForTest = false; }



                        if (!string.IsNullOrEmpty(testName))
                            test = ctx.Test.Where(b => b.TestName == testName && (b.UserId==userid || b.UserId==adminuserid)).FirstOrDefault();
                        else
                            test = null;



                        if (test != null)
                        {



                            if (!String.IsNullOrEmpty(insName))
                                instrument = ctx.Instrument.Where(b => b.InstrumentName == insName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                            else
                                instrument = null;



                            if (instrument != null)
                            {


                                if (!String.IsNullOrEmpty(proName))
                                {

                                    product = ctx.MasterProduct.Where(b => b.ProductName == proName && (b.UserId == userid || b.UserId == adminuserid)).FirstOrDefault();
                                    if (product == null)
                                    {
                                        str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Product Name is required";
                                        error = error + 1;
                                        continue;
                                    }
                                    else
                                    {
                                        if (PUList.Where(b => b.InstrumentId == instrument.InstrumentID && b.TestId == test.TestID && b.ProductId == product.ProductID && b.IsForControl== isForControl).FirstOrDefault() == null)
                                        {
                                            PU = ctx.ProductUsage.Where(b => b.InstrumentId == instrument.InstrumentID && b.TestId == test.TestID && b.ProductId == product.ProductID && b.IsForControl == isForControl).FirstOrDefault();
                                            if (PU != null)
                                            {
                                                // str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Product Usage Rate already exist";
                                                //error = error + 1;
                                                //continue;
                                            }
                                            else
                                            {
                                                PU = new ProductUsage();
                                                PU.InstrumentId = instrument.InstrumentID;
                                                PU.ProductId = product.ProductID;
                                                PU.IsForControl = isForControl;
                                                PU.Rate = rate;
                                                PU.TestId = test.TestID;
                                                PU.UserId = userid;
                                                PUList.Add(PU);

                                                noofsave = noofsave + 1;
                                            }
                                        }
                                        else
                                        {
                                            continue;

                                        }
                                       
                                       
                                    }
                                }
                                else
                                {
                                    str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Product Name is required";
                                    error = error + 1;
                                    continue;
                                }
                            }
                            else
                            {
                                str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Instrument Name is required";
                                error = error + 1;
                                continue;
                            }
                        }
                        else
                        {
                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Test Name is required";
                            error = error + 1;
                            continue;
                        }
                        //Font.Color = IndexedColors.Red.Index;
                        //ICell cell = row.GetCell(0);
                        //cellStyle.SetFont(Font);
                        //cell.CellStyle = cellStyle;
                        //return str;



                        if (error > 20)
                        {
                            str = "There too many problem with Test Product Usage data, please troubleshoot and try to import again.";
                            return str;
                        }
                        //    sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        //    }
                        //}
                        //    sb.AppendLine("</tr>");
                    }
                    ctx.SaveChanges();
                    str = str + Environment.NewLine + noofsave + " Test Product Usage data  are imported and saved successfully.";
                    if (error > 0)
                    {
                        str = str + Environment.NewLine + error + " Test Product Usage data  Failed.Please check excel and correct Test Product Usage Entry";
                    }
                }
                if(PUList.Count>0)
                {
                    ctx.ProductUsage.AddRange(PUList);
                    ctx.SaveChanges();
                }
                //  sb.Append("</table>");
                return str;
            }
            catch (Exception ex)
            {
                str = "The file is corrupt and import is unsuccessful. Please correct the file";
                return str;
            }
        }
        private string validateandsaveTest(string fullpath, string sheetname, string Fileextension, IFormFile file,int userid,Matchrule matchrule)
        {
            string str = "";
            try
            {
                int error = 0;
                int noofsave = 0;
              
                ISheet sheet;
                IWorkbook workbook = null;
                IFont Font;
                ICellStyle cellStyle;
                List<Test> Testlist = new List<Test>();
                var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>

             x.Id

        ).FirstOrDefault();
                using (var stream = new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (Fileextension == ".xls")
                    {

                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook  
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook   
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;

                    int testindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Test Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int areaindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Area Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    //if (2 > cellCount)
                    //{
                    //    str = "Imported " + sheetname + " Sheet has less columns than needed.";
                    //    return str;
                    //}
                    //if (2 < cellCount)
                    //{
                    //    str = "Imported " + sheetname + " Sheet has too many columns.";
                    //    return str;
                    //}
                    //if (sheet.LastRowNum == 0)
                    //{
                    //    str = "Imported " + sheetname + " Sheet is empty.";
                    //    return str;
                    //}
                    //          Test Name", valueT: "" },
                    //{ type: "string", nullable: false, value: "Area Name

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //for (int j = row.FirstCellNum; j < cellCount; j++)
                        //{
                        TestingArea TA = new TestingArea();
                        if (row.GetCell(areaindex) != null && row.GetCell(areaindex).ToString() != "")
                        {
                            TA = ctx.TestingArea.Where(b => b.AreaName == row.GetCell(areaindex).ToString() && (b.UserId == userid || b.UserId==adminuserid)).FirstOrDefault();
                            if (TA != null)
                            {

                            }
                            else
                            {
                                TA = new TestingArea();
                                TA.AreaName = row.GetCell(areaindex).ToString();
                                TA.UserId = userid;
                                ctx.TestingArea.Add(TA);
                                ctx.SaveChanges();
                            }

                        }
                        else
                        {
                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Testing Area Name Required";
                            error = error + 1;
                            continue;
                        }
                        if (row.GetCell(testindex) != null)
                        {
                            if (Testlist.Where(b => b.TestName == row.GetCell(testindex).ToString() && b.TestingAreaID == TA.TestingAreaID).FirstOrDefault() == null)
                            {
                                var Test = ctx.Test.Where(b => b.TestName == row.GetCell(testindex).ToString() &&( b.UserId == userid || b.UserId==adminuserid)).FirstOrDefault();
                                if (Test != null)
                                {

                                    //str = str + Environment.NewLine + "Error in Row No." + i + ":Test name already exists";
                                    //Font.Color = IndexedColors.Red.Index;
                                    //ICell cell = row.GetCell(0);
                                    //cellStyle.SetFont(Font);
                                    //cell.CellStyle = cellStyle;
                                    //error = error + 1;
                                    //continue;
                                }
                                else
                                {
                                    Test Ts = new Test();
                                    Ts.TestName = row.GetCell(testindex).ToString();
                                    Ts.TestingAreaID = TA.TestingAreaID;
                                    Ts.UserId = userid;
                                    //ctx.Test.Add(Ts);
                                    //ctx.SaveChanges();
                                    Testlist.Add(Ts);
                                    noofsave = noofsave + 1;
                                }
                            }
                            else
                            {
                                str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Test " + row.GetCell(testindex).ToString() + " already exists on sheet";
                                error = error + 1;
                                continue;
                            }
                        }
                        else
                        {
                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Test Name Required";
                            error = error + 1;
                            continue;
                        }
                        if (error > 20)
                        {
                            str = "There too many problem with Test data, please troubleshoot and try to import again.";
                            return str;
                        }
                        //    sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        //    }
                        //}
                        //    sb.AppendLine("</tr>");
                    }
                    ctx.SaveChanges();
                    str = str + Environment.NewLine + noofsave + " Test  are imported and saved successfully.";
                    if (error > 0)
                    {
                        str = str + Environment.NewLine + error + " Test  Failed.Please check excel and correct Test Entry";
                    }
                }
                if(Testlist.Count>0)
                {
                    ctx.Test.AddRange(Testlist);
                    ctx.SaveChanges();
                }
                //  sb.Append("</table>");
                return str;
            }
            catch (Exception ex)
            {
                str = "The file is corrupt and import is unsuccessful. Please correct the file";
                return str;
            }
        }

        private string validateandsaveregion(string fullpath, string sheetname, string Fileextension, IFormFile file,int userid,int countryid,Matchrule matchrule)
        {
            string str = "";
            if (countryid==0)
            {
                countryid= ctx.User.Find(userid).CountryId.Value;
            }
            try
            {
                int error = 0;
                int noofsave = 0;
            
                ISheet sheet;
                IWorkbook workbook = null;
                IFont Font;
                ICellStyle cellStyle;
                List<Region> Rgl = new List<Region>();
                using (var stream = new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite))
                {
                    
                    if (Fileextension == ".xls")
                    {
                        file.CopyTo(stream);
                        stream.Position = 0;
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook  
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle(); 
                        workbook = hssfwb;
                    }
                    else
                    {
                       
                       // file.CopyTo(stream);
                        stream.Position = 0;
                        //InputStream ExcelFileToRead = new FileInputStream("C:/Test.xlsx");
                        XSSFWorkbook hssfwb = new XSSFWorkbook((FileStream)stream); //This will read 2007 Excel format  
                     
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook   
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }
                    //Get Header Row
                    //int cellCount = headerRow.LastCellNum;
                    //if (2 > cellCount)
                    //{
                    //    str = "Imported " + sheetname + " Sheet has less columns than needed.";
                    //    return str;
                    //}
                    //if (2 < cellCount)
                    //{
                    //    str = "Imported " + sheetname + " Sheet has too many columns.";
                    //    return str;
                    //}
                    //if (sheet.LastRowNum == 0)
                    //{
                    //    str = "Imported " + sheetname + " Sheet is empty.";
                    //    return str;
                    //}
                    IRow headerRow = sheet.GetRow(0);
                    int regioncolumnindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Region").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int shortnamecolumnindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Short Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                      
                        
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //for (int j = row.FirstCellNum; j < cellCount; j++)
                        //{
                     //   int k=headerRow.Cells.Where(b => b.StringCellValue == "Region").FirstOrDefault().ColumnIndex;
                    
                        if (row.GetCell(regioncolumnindex) != null && row.GetCell(regioncolumnindex).ToString() != "")
                        {
                            if (Rgl.Where(b => b.RegionName == row.GetCell(regioncolumnindex).ToString()).FirstOrDefault() == null)
                            {

                                var region = ctx.Region.Where(b => b.RegionName == row.GetCell(regioncolumnindex).ToString() && (b.CountryId == countryid || b.UserId==userid)).FirstOrDefault();
                                if (region != null)
                                {

                                    //str = str + Environment.NewLine + "Error in Row No." + i +":Region name already exists"+Environment.NewLine;
                                    //Font.Color = IndexedColors.Red.Index;
                                    //ICell cell = row.GetCell(0);
                                    //cellStyle.SetFont(Font);
                                    //cell.CellStyle = cellStyle;

                                    ////workbook.Write(stream);
                                    ////stream.Close();
                                    //error = error+1;
                                }
                                else
                                {
                                    Models.Region Rg = new Models.Region();
                                    Rg.RegionName = row.GetCell(regioncolumnindex).ToString();
                                    Rg.ShortName = row.GetCell(shortnamecolumnindex).ToString();
                                    Rg.UserId = userid;
                                    Rg.CountryId = countryid;
                                    Rgl.Add(Rg);
                                    //ctx.Region.Add(Rg);
                                    //ctx.SaveChanges();
                                    noofsave = noofsave + 1;
                                }
                            }
                            else
                            {
                                str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Region " + row.GetCell(regioncolumnindex).ToString() + " already exists on sheet";
                                error = error + 1;
                                continue;
                            }
                        }
                        else
                        {
                            str = str + "Error in Row No." + (i + 1) + " : Region Name Required,";
                            error = error + 1;
                            continue;
                        }
                        if (error > 20)
                        {
                            str = "There too many problem with Region data, please troubleshoot and try to import again.";
                            return str;
                        }
                        //    sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        //    }
                        //}
                        //    sb.AppendLine("</tr>");
                    }

                    str = str + noofsave + " Regions/Districts/Provinces  are imported and saved successfully.,";
                    if (error > 0)
                    {
                        str = str + error + " Regions/Districts/Provinces  Failed.Please check excel and correct Regions/Districts/Provinces Entry";
                    }
                }

                if (Rgl.Count > 0)
                {
                    ctx.Region.AddRange(Rgl);
                    ctx.SaveChanges();
                }
                //  sb.Append("</table>");
                return str;
            }
            catch (Exception ex)
            {
                str = "The file is corrupt and import is unsuccessful. Please correct the file";
                return str;
            }
        }
        private string validateandsavesite(string fullpath, string sheetname, string Fileextension, IFormFile file,int userid,int countryid,Matchrule matchrule)
        {
            string str = "";
            try
            {
                int error = 0;
                int noofsave = 0;
              
                ISheet sheet;
                IWorkbook workbook = null;
                IFont Font;
                ICellStyle cellStyle;
                List<Site> SiteList = new List<Site>();
                using (var stream = new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (Fileextension == ".xls")
                    {

                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook  
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook   
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    //int cellCount = headerRow.LastCellNum;
                    //if (4 > cellCount)
                    //{
                    //    str = "Imported " + sheetname + " Sheet has less columns than needed.";
                    //    return str;
                    //}
                    //if (4 < cellCount)
                    //{
                    //    str = "Imported " + sheetname + " Sheet has too many columns.";
                    //    return str;
                    //}
                    //if (sheet.LastRowNum == 0)
                    //{
                    //    str = "Imported " + sheetname + " Sheet is empty.";
                    //    return str;
                    //}
                    int regionindex= headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Region").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int categoryindex= headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Site Category").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int siteindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Site Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int WorkingDaysindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Working Days").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    Models.Region region = new Models.Region();
                    SiteCategory sitecategory = new SiteCategory();
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //for (int j = row.FirstCellNum; j < cellCount; j++)
                        //{

                        if (Convert.ToInt32(row.GetCell(WorkingDaysindex).ToString()) > 31)
                        {
                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ": Working Days can't be greater than 31";
                            error = error + 1;
                            continue;
                        }


                        if (row.GetCell(regionindex) != null && row.GetCell(regionindex).ToString() != "")
                         {
                            region = ctx.Region.Where(b => b.RegionName == row.GetCell(regionindex).ToString() && (b.CountryId==countryid || b.UserId==userid)).FirstOrDefault();
                            if (region != null)
                            {


                            }
                            else
                            {
                                Models.Region Rg = new Models.Region();
                                Rg.RegionName = row.GetCell(regionindex).ToString();
                                Rg.UserId = userid;
                                Rg.CountryId = countryid;
                                ctx.Region.Add(Rg);
                                ctx.SaveChanges();
                                region = Rg;
                            }
                        }
                        else
                        {
                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Region is Required";
                            error = error + 1;
                            continue;
                        }




                        if (row.GetCell(categoryindex) != null && row.GetCell(categoryindex).ToString() != "")
                        {
                            sitecategory = ctx.SiteCategory.Where(b => b.CategoryName == row.GetCell(categoryindex).ToString() && b.UserId==userid).FirstOrDefault();
                            if (sitecategory != null)
                            {

                            }
                            else
                            {
                                SiteCategory sb = new SiteCategory();
                                sb.CategoryName = row.GetCell(categoryindex).ToString();
                                sb.UserId = userid;
                                ctx.SiteCategory.Add(sb);
                                ctx.SaveChanges();
                                sitecategory = sb;
                            }
                        }
                        else
                        {
                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Site Category is Required";
                            error = error + 1;
                            continue;
                        }


                        if (row.GetCell(siteindex) != null && row.GetCell(siteindex).ToString() != "")
                        {
                            if (SiteList.Where(b => b.SiteName == row.GetCell(siteindex).ToString() && b.regionid == region.RegionID).FirstOrDefault() == null)
                            {

                                var site = ctx.Site.Where(b => b.SiteName == row.GetCell(siteindex).ToString() && b.regionid == region.RegionID).FirstOrDefault();
                                if (site != null)
                                {
                                    //str = str + Environment.NewLine + "Error in Row No." + i + ":Site already exists";
                                    //error = error + 1;
                                    //continue;
                                }
                                else
                                {

                                    //SiteStatus ss = new SiteStatus();

                                    //ss.OpenedFrom = row.GetCell(10).ToString() != null ? Convert.ToDateTime(Convert.ToDateTime(row.GetCell(10).DateCellValue.ToString()).ToString("dd/MMM/yyyy")) : Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy"));
                                    Site st = new Site();
                                    st.SiteName = row.GetCell(siteindex).ToString();
                                    st.regionid = region.RegionID;
                                    st.CategoryID = sitecategory.CategoryID;
                                    st.WorkingDays = Convert.ToInt32(row.GetCell(WorkingDaysindex).ToString());
                                    //st.SiteLevel = row.GetCell(3).ToString();
                                    //st.Latitude = Convert.ToDecimal(row.GetCell(11).ToString());
                                    //st.Longitude = Convert.ToDecimal(row.GetCell(12).ToString());
                                    st.UserId = userid;
                                    st.CountryId = countryid;
                                    SiteList.Add(st);
                                    //ctx.Site.Add(st);
                                    //ctx.SaveChanges();
                                    //ss.SiteID = st.SiteID;
                                    //ctx.SiteStatus.Add(ss);
                                    //ctx.SaveChanges();
                                    noofsave = noofsave + 1;
                                }
                            }
                            else
                            {
                                str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":site " + row.GetCell(siteindex).ToString() + " already exists on sheet";
                                error = error + 1;
                                continue;
                            }
                        }
                        else
                        {
                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Site Name is Required";
                            error = error + 1;
                            continue;
                        }





                        if (error > 20)
                        {
                            str = "There too many problem with Site data, please troubleshoot and try to import again.";
                            return str;
                        }
                        //    sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        //    }
                        //}
                        //    sb.AppendLine("</tr>");
                    }

                    str = str + Environment.NewLine + noofsave + " Site  are imported and saved successfully.";
                    if (error > 0)
                    {
                        str = str + Environment.NewLine + error + " Site  Failed.Please check excel and correct Site Entry";
                    }
                }
                if (SiteList.Count > 0)
                {
                    ctx.Site.AddRange(SiteList);
                    ctx.SaveChanges();
                }
                //  sb.Append("</table>");
                return str;
            }
            catch (Exception ex)
            {
                str = "The file is corrupt and import is unsuccessful. Please correct the file";
                return str;
            }
        }

        private string validateandsaveproduct(string fullpath, string sheetname, string Fileextension, IFormFile file,int userid,Matchrule matchrule)
        {
            string str = "";
            try
            {
                int error = 0;
                int noofsave = 0;
            
                ISheet sheet;
                IWorkbook workbook = null;
                IFont Font;
                ICellStyle cellStyle;
                List<MasterProduct> MPlist = new List<MasterProduct>();
                List<ProductPrice> PPList = new List<ProductPrice>();
                var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>

      x.Id

 ).FirstOrDefault();
                using (var stream = new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (Fileextension == ".xls")
                    {

                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook  
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook   
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                                                      //int cellCount = headerRow.LastCellNum;
                                                      //if (7 > cellCount)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet has less columns than needed.";
                                                      //    return str;
                                                      //}
                                                      //if (7 < cellCount)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet has too many columns.";
                                                      //    return str;
                                                      //}
                                                      //if (sheet.LastRowNum == 0)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet is empty.";
                                                      //    return str;
                                                      //}
          //      value: "Product Name", valueT: "" },
          //{ type: "string", nullable: false, value: "Product Type", valueT: "" },
          //{ type: "string", nullable: false, value: "Basic Unit", valueT: "" },
          //{ type: "number", nullable: false, value: "Min Packs Per Site", valueT: "" },
          //{ type: "number", nullable: false, value: "Price", valueT: "" },
          //{ type: "number", nullable: false, value: "Packaging Size", valueT: "" },
          //{ type: "string", nullable: true, value: "Price As of Date", valueT: "" },
                 int productindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Product Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                int producttypeindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Product Type").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                int Minpackindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Min Packs Per Site").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                int basicunitindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Basic Unit").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;

                int priceindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Price").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                int packingsizeindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Packaging Size").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                int priceatdatesizeindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Price As of Date").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {

                        IRow row = sheet.GetRow(i);

                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //for (int j = row.FirstCellNum; j < cellCount; j++)
                        //{
                        string proName = Convert.ToString(row.GetCell(productindex)).Trim();
                        string catName = Convert.ToString(row.GetCell(producttypeindex)).Trim();
                       // string serial = Convert.ToString(row.GetCell(2));
                        string bunit = Convert.ToString(row.GetCell(basicunitindex));
                        var category = ctx.ProductType.Where(b => b.TypeName == catName && (b.UserId==userid || b.UserId==adminuserid)).FirstOrDefault();

                       // string specification = Convert.ToString(row.GetCell(3));
                        //bool testrapid = false;
                        //string rapidTest;
                        int minSize;
                        int psize;
                        decimal price;
                        DateTime pricedate;
                        try
                        {
                            if (!string.IsNullOrEmpty(catName))
                            {
                                category = ctx.ProductType.Where(b => b.TypeName == catName && (b.UserId == userid || b.UserId==adminuserid)).FirstOrDefault();
                                if (category == null)
                                {
                                    category = new ProductType();
                                    category.TypeName = catName;
                                    category.UserId = userid;
                                    ctx.ProductType.Add(category);
                                    ctx.SaveChanges();
                                }
                            }
                            else
                            {
                                category = ctx.ProductType.Where(b => b.TypeName == "OTHERS").FirstOrDefault();
                                if (category == null)
                                {
                                    category = new ProductType();
                                    category.TypeName = "OTHERS";
                                    category.UserId = userid;
                                    ctx.ProductType.Add(category);
                                    ctx.SaveChanges();
                                }
                            }
                            //if (category.UseInDemography)
                            //{
                            //    string[] group;
                            //    string str1 = "Screening,Confirmatory, Tie_Breaker";
                            //    group = str1.Split(',');


                            //    if (category.ClassOfTest == "RapidTest")
                            //    {
                            //        for (int j = 0; j < group.Length; j++)
                            //        {
                            //            if (Convert.ToString(row.GetCell(6)) == group[j])
                            //                testrapid = true;

                            //        }
                            //        if (testrapid)
                            //            rapidTest = Convert.ToString(row.GetCell(6));//b
                            //        else
                            //            rapidTest = null;
                            //    }
                            //    else
                            //        rapidTest = null;
                            //}
                            //else
                            //    rapidTest = null;

                        }
                        catch
                        {
                            //rapidTest = null;
                        }

                        try
                        {
                            psize = int.Parse(Convert.ToString(row.GetCell(packingsizeindex)));
                        }
                        catch
                        {
                            psize = 1;
                        }
                        try//b
                        {
                            minSize = int.Parse(Convert.ToString(row.GetCell(Minpackindex)));//b
                        }
                        catch
                        {
                            minSize = 1;
                        }

                        try
                        {
                            price = decimal.Parse(Convert.ToString(row.GetCell(priceindex).ToString().Trim('$')));
                        }
                        catch
                        {
                            price = 1;
                        }

                        try
                        {
                            pricedate = DateTime.Parse(Convert.ToString(row.GetCell(priceatdatesizeindex)));
                        }
                        catch
                        {
                            pricedate = DateTime.Now;
                        }



                        if (!string.IsNullOrEmpty(proName))
                        {

                            if (MPlist.Where(b => b.ProductName == proName && b.ProductTypeId == category.TypeID).FirstOrDefault() == null)
                            {
                                var product = ctx.MasterProduct.Where(b => b.ProductName == proName && ( b.UserId == userid || b.UserId==adminuserid)).FirstOrDefault();
                                if (product != null)
                                {
                                    //str = str + Environment.NewLine + "Error in Row No." + i + ":Product Name can't be duplicate";
                                    //error = error + 1;
                                    //continue;
                                }
                                else
                                {
                                    ProductPrice PP = new ProductPrice();
                                    PP.FromDate = pricedate;
                                    PP.PackSize = psize;
                                    PP.Price = price;
                                    PPList.Add(PP);

                                    MasterProduct MP = new MasterProduct();
                                    MP.ProductName = proName;
                                    MP.ProductTypeId = category.TypeID;
                                    // MP.SerialNo = serial;
                                    MP.BasicUnit = bunit;
                                    MP.MinimumPackPerSite = minSize;
                                    //MP.RapidTestGroup = rapidTest;
                                    //MP.Specification = specification;
                                    MP.UserId = userid;
                                    // MP._productPrices = PPList;
                                    MPlist.Add(MP);
                                    ctx.MasterProduct.Add(MP);
                                    ctx.SaveChanges();
                                    PP.ProductId = MP.ProductID;
                                    ctx.ProductPrice.Add(PP);
                                    ctx.SaveChanges();
                                    noofsave = noofsave + 1;
                                }
                            }
                            else
                            {
                                str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Product " + proName  + " already exists on sheet";
                                error = error + 1;
                                continue;
                            }
                        }

                        if (error > 20)
                        {
                            str = "There too many problem with Product data, please troubleshoot and try to import again.";
                            return str;
                        }
                        //    sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        //    }
                        //}
                        //    sb.AppendLine("</tr>");
                    }
                    ctx.SaveChanges();
                    str = str + Environment.NewLine + noofsave + " Product  are imported and saved successfully.";
                    if (error > 0)
                    {
                        str = str + Environment.NewLine + error + " Product  Failed.Please check excel and correct Product Entry";
                    }
                }
                //if (MPlist.Count>0)
                //{
                //    ctx.MasterProduct.AddRange(MPlist);
                //    ctx.SaveChanges();
                //}
                //  sb.Append("</table>");
                return str;
            }
            catch (Exception ex)
            {
                str = "The file is corrupt and import is unsuccessful. Please correct the file";
                return str;
            }
        }

        private bool DatarowValueToInt(object drvalue, out int result)
        {
            return Int32.TryParse(Convert.ToString(drvalue), out result);
        }
        private string validateandsaveinstrument(string fullpath, string sheetname, string Fileextension, IFormFile file,int userid,Matchrule matchrule)
        {
            string str = "";
            try
            {
                ManageQuantificationMenus MQ = new ManageQuantificationMenus(ctx);
                int error = 0;
                int noofsave = 0;
             
                ISheet sheet;
                IWorkbook workbook = null;
                IFont Font;
                ICellStyle cellStyle;
                int maxThroughput, dailyTest, perTestCtrl, weeklyTest, monthlyTest, quarterlyTest;
                List<Instrument> Inslist = new List<Instrument>();
                using (var stream = new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (Fileextension == ".xls")
                    {

                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook  
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheet(matchrule.docSheet); //get first sheet from workbook   
                        Font = hssfwb.CreateFont();
                        cellStyle = hssfwb.CreateCellStyle();
                        workbook = hssfwb;
                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                                                      //int cellCount = headerRow.LastCellNum;
                                                      //if (8 > cellCount)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet has less columns than needed.";
                                                      //    return str;
                                                      //}
                                                      //if (8 < cellCount)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet has too many columns.";
                                                      //    return str;
                                                      //}
                                                      //if (sheet.LastRowNum == 0)
                                                      //{
                                                      //    str = "Imported " + sheetname + " Sheet is empty.";
                                                      //    return str;
          //                                            //}
          //          { type: "string", nullable: false, value: "Testing Area", valueT: "" },
          //{ type: "string", nullable: false, value: "Instrument Name", valueT: "" },
          //{ type: "number", nullable: false, value: "Max Through Put", valueT: "" },
          //{ type: "number", nullable: false, value: "Per Test Control", valueT: "" },
          //{ type: "number", nullable: false, value: "Daily Control Test", valueT: "" },
          //{ type: "number", nullable: false, value: "Weekly Control Test", valueT: "" },
          //{ type: "number", nullable: false, value: "Monthly Control Test", valueT: "" },
          //{ type: "number", nullable: false, value: "Quarterly control Test", valueT: "" },


                       int areaindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Testing Area").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int insindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Instrument Name").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int Maxthroughputindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Max Through Put").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int percontrolindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Per Test Control").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;

                    int dailycontrolindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Daily Control Test").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int weeklycontrolindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Weekly Control Test").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int monthlycontrolindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Monthly Control Test").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    int quartlycontrolindex = headerRow.Cells.Where(b => b.StringCellValue == matchrule.columns.Where(c => c.regCol == "Quarterly control Test").FirstOrDefault().docCol).FirstOrDefault().ColumnIndex;
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //for (int j = row.FirstCellNum; j < cellCount; j++)
                        //{



                        if (!DatarowValueToInt(row.GetCell(Maxthroughputindex), out maxThroughput))
                            maxThroughput = 1;
                        if (!DatarowValueToInt(row.GetCell(percontrolindex), out perTestCtrl))
                            perTestCtrl = 0;
                        if (!DatarowValueToInt(row.GetCell(dailycontrolindex), out dailyTest))
                            dailyTest = 0;
                        if (!DatarowValueToInt(row.GetCell(weeklycontrolindex), out weeklyTest))
                            weeklyTest = 0;
                        if (!DatarowValueToInt(row.GetCell(monthlycontrolindex), out monthlyTest))
                            monthlyTest = 0;
                        if (!DatarowValueToInt(row.GetCell(quartlycontrolindex), out quarterlyTest))
                            quarterlyTest = 0;
                        TestingArea TA = new TestingArea();
                        if (row.GetCell(areaindex) != null && row.GetCell(areaindex).ToString() != "")
                        {
                            TA = ctx.TestingArea.Where(b => b.AreaName == row.GetCell(areaindex).ToString() && b.UserId==userid).FirstOrDefault();
                            if (TA != null)
                            {

                            }
                            else
                            {
                                TA = new TestingArea();
                                TA.AreaName = row.GetCell(areaindex).ToString();
                                TA.UserId = userid;
                                ctx.TestingArea.Add(TA);
                                ctx.SaveChanges();
                            }

                        }
                        else
                        {
                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Testing Area Name Required";
                            error = error + 1;
                            continue;
                        }
                        if (row.GetCell(insindex) != null && row.GetCell(insindex).ToString() != "")
                        {
                            if (Inslist.Where(b => b.InstrumentName == row.GetCell(insindex).ToString() && b.testingArea.TestingAreaID == TA.TestingAreaID).FirstOrDefault() == null)
                            {
                                var Instrument = ctx.Instrument.Where(b => b.InstrumentName == row.GetCell(insindex).ToString() && b.testingArea.TestingAreaID == TA.TestingAreaID && b.UserId == userid).FirstOrDefault();
                                if (Instrument != null)
                                {

                                    //str = str + Environment.NewLine + "Error in Row No." + i + ":Instrument name already exists";
                                    //Font.Color = IndexedColors.Red.Index;
                                    //ICell cell = row.GetCell(1);
                                    //cellStyle.SetFont(Font);
                                    //cell.CellStyle = cellStyle;

                                    //error = error + 1;
                                    //continue;
                                }
                                else
                                {


                                    Instrument Ts = new Instrument();
                                    Ts.InstrumentName = row.GetCell(insindex).ToString();
                                    Ts.MaxThroughPut = maxThroughput;
                                    // Ts.MonthMaxTPut = (rd.Rate * 5) * 22;
                                    Ts.testingArea = TA;
                                    Ts.DailyCtrlTest = dailyTest;
                                    Ts.WeeklyCtrlTest = weeklyTest;
                                    Ts.MonthlyCtrlTest = monthlyTest;
                                    Ts.QuarterlyCtrlTest = quarterlyTest;
                                    Ts.MaxTestBeforeCtrlTest = perTestCtrl;
                                    Ts.UserId = userid;
                                    Inslist.Add(Ts);
                                    //ctx.Instrument.Add(Ts);
                                    //ctx.SaveChanges();
                                    //if (Ts.testingArea.Category != null)
                                    //{
                                    //    ClassOfMorbidityTestEnum TACE = (ClassOfMorbidityTestEnum)Enum.Parse(typeof(ClassOfMorbidityTestEnum), Ts.testingArea.Category, true);
                                    //    if (Ts.testingArea.UseInDemography)
                                    //    {
                                    //        if (TACE == ClassOfMorbidityTestEnum.CD4 ||
                                    //            TACE == ClassOfMorbidityTestEnum.Chemistry ||
                                    //            TACE == ClassOfMorbidityTestEnum.Hematology ||
                                    //            TACE == ClassOfMorbidityTestEnum.ViralLoad)
                                    //        {
                                    //            MorbidityTest mtest = new MorbidityTest();
                                    //            mtest.InstrumentId = Ts.InstrumentID;
                                    //            mtest.ClassOfTest = Ts.testingArea.Category;

                                    //            ClassOfMorbidityTestEnum MS = (ClassOfMorbidityTestEnum)Enum.Parse(typeof(ClassOfMorbidityTestEnum), mtest.ClassOfTest, true);
                                    //            mtest.TestName = MQ.BuildTestName(Ts.InstrumentName, MS);
                                    //            MQ.CreateQuantifyMenus(mtest);

                                    //            ctx.MorbidityTest.Add(mtest);
                                    //            ctx.SaveChanges();
                                    //        }
                                    //    }
                                    //}
                                    noofsave = noofsave + 1;
                                }
                            }
                            else
                            {
                                str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Instrument " + row.GetCell(insindex).ToString() + " already exists on sheet";
                                error = error + 1;
                                continue;
                            }
                        }
                      
                        else
                        {
                            str = str + Environment.NewLine + "Error in Row No." + (i + 1) + ":Instrument Name Required";
                            error = error + 1;
                            continue;
                        }
                        if (error > 20)
                        {
                            str = "There too many problem with Instrument data, please troubleshoot and try to import again.";
                            return str;
                        }
                        //    sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        //    }
                        //}
                        //    sb.AppendLine("</tr>");
                    }
                  
                    str = str + Environment.NewLine + noofsave + " Instrument  are imported and saved successfully.";
                    if (error > 0)
                    {
                        str = str + Environment.NewLine + error + " Instrument  Failed.Please check excel and correct Instrument Entry";
                    }
                }
                if(Inslist.Count>0)
                {
                    ctx.Instrument.AddRange(Inslist);
                    ctx.SaveChanges();
                }
                //  sb.Append("</table>");
                return str;
            }
            catch (Exception ex)
            {
                str = "The file is corrupt and import is unsuccessful. Please correct the file";
                return str;
            }
        }
    }
}
