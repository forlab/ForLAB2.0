using ForLabApi.DataInterface;
using ForLabApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ForLabApi.Repositories
{
    public class ConductforecastAccessRepositories : IConductforecast<Costclass, ConductforecastDasboard, ConductDashboardchartdata>
    {

        ForLabContext ctx;
        private List<ForecastedResult> _listFresult;
        private List<Finalreslist> _Finalreslist;
        private bool _forecastWithoutError;
        private DateTime lastEntryDate;
        private string[] param1;
        string mainurl = "http://34.69.151.1:8000/api/";
        public ConductforecastAccessRepositories(ForLabContext c)
        {
            ctx = c;
            //return ctx;
        }
        public IList<ConductDashboardchartdata> GetProducttypecostratio(int id, int fid)
        {
            decimal total = 0;
            List<ConductDashboardchartdata> Db = new List<ConductDashboardchartdata>();
            var list = ctx.ForecastedResult.Join(ctx.ForecastInfo, b => b.ForecastId, c => c.ForecastID, (b, c) => new { b, c })
                .Where(s => s.b.PackPrice != 0 && s.b.ForecastId == fid && s.b.SiteId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension))
                .GroupBy(f => f.b.ProductTypeId)
                .Select(g => new
                {
                    g.Key,
                    ProductType = g.Max(h => h.b.ProductType),
                    Totalprice = g.Sum(h => h.b.PackPrice)

                }).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                total = total + list[i].Totalprice;
            }

            for (int i = 0; i < list.Count; i++)
            {

                Db.Add(new ConductDashboardchartdata
                {
                    Name = list[i].ProductType,
                    Y = Math.Round((list[i].Totalprice / total) * 100, 2)

                });


            }
            return Db;
        }



        public IList<ConductDashboardchartdata> GetProducttypecostratioNEW(int fid)
        {
            decimal total = 0;
            List<ConductDashboardchartdata> Db = new List<ConductDashboardchartdata>();
            var forecastinfo = ctx.ForecastInfo.Find(fid);
            if (forecastinfo.Methodology.ToUpper() == "CONSUMPTION")
            {
                var list = ctx.ForecastedResult.Join(ctx.ForecastInfo, b => b.ForecastId, c => c.ForecastID, (b, c) => new { b, c })
                .Where(s => s.b.PackPrice != 0 && s.b.ForecastId == fid) //&& s.b.SiteId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension)
                .GroupBy(f => f.b.ProductTypeId)
                .Select(g => new
                {
                    ProductTypeId = g.Key,
                    ProductType = g.Max(h => h.b.ProductType),
                    Totalprice = g.Sum(h => h.b.PackPrice)

                }).ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    total = total + list[i].Totalprice;
                }

                for (int i = 0; i < list.Count; i++)
                {

                    Db.Add(new ConductDashboardchartdata
                    {
                        Name = list[i].ProductType,
                        Y = Math.Round((list[i].Totalprice / total) * 100, 2)

                    });


                }
            }
            else
            {
                var list = ctx.ForecastedResult.Join(ctx.ForecastInfo, b => b.ForecastId, c => c.ForecastID, (b, c) => new { b, c })
                 .Where(s => s.b.PackPrice != 0 && s.b.ForecastId == fid) //&& s.b.SiteId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension)
                 .GroupBy(f => f.b.TestingArea)
                 .Select(g => new
                 {
                     TestingArea = g.Key,
                     ProductType = g.Max(h => h.b.ProductType),
                     Totalprice = g.Sum(h => h.b.PackPrice)

                 }).ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    total = total + list[i].Totalprice;
                }

                for (int i = 0; i < list.Count; i++)
                {

                    Db.Add(new ConductDashboardchartdata
                    {
                        Name = list[i].TestingArea,
                        Y = Math.Round((list[i].Totalprice / total) * 100, 2)

                    });


                }

            }
            return Db;
        }


        public IList<ConductDashboardchartdata> GetdemoProducttypecostratioNEW(int fid)
        {
            decimal total = 0;
            List<ConductDashboardchartdata> Db = new List<ConductDashboardchartdata>();


            var list = ctx.ForecastedResult.Where(x => x.ForecastId == fid && x.TotalValue != 0)
            .GroupBy(s => new { s.SiteId, s.ProductTypeId, s.ProductId })
            .Select(n => new
            {
                n.Key.ProductTypeId,
                n.Key.SiteId,
                ProductType = n.Max(s => s.ProductType),
                totalcost = Math.Round(Math.Ceiling(n.Sum(s => s.TotalValue) / n.Max(s => s.PackQty)) * n.Max(s => s.PackPrice), 2),
                productname = n.Max(s => s.ProductId)
            }).ToList();


            var list1 = list.GroupBy(s => s.ProductTypeId)
                .Select(g => new
                {
                    g.Key,
                    ProductType = g.Max(h => h.ProductType),
                    Totalprice = g.Sum(h => h.totalcost)

                }).ToList();

            for (int i = 0; i < list1.Count; i++)
            {
                total = total + list1[i].Totalprice;
            }

            for (int i = 0; i < list1.Count; i++)
            {

                Db.Add(new ConductDashboardchartdata
                {
                    Name = list1[i].ProductType,
                    Y = Math.Round((list1[i].Totalprice / total) * 100, 2)

                });


            }
            return Db;
        }


        public Costclass getcostparameter(int id)
        {
            Costclass cc = new Costclass();

            cc.Totalcost = String.Format("{0:n}", ctx.ForecastedResult.Where(b => b.ForecastId == id).Sum(x => x.PackPrice));


            cc.Qccost = String.Format("{0:n}", ctx.ForecastedResult.Where(b => b.ForecastId == id && b.IsForGeneralConsumable == false  &&  b.IsForControl==true).Sum(x => x.PackPrice));
            cc.Cccost = String.Format("{0:n}", ctx.ForecastedResult.Where(b => b.ForecastId == id && b.IsForGeneralConsumable == true).Sum(x => x.PackPrice));
            cc.Testcost = String.Format("{0:n}", ctx.ForecastedResult.Where(b => b.ForecastId == id && b.IsForGeneralConsumable == false && b.IsForControl == false).Sum(x => x.PackPrice));
            return cc;
        }

        public Costclass getdemocostparameter(int id)
        {
            Costclass cc = new Costclass();
            var list = ctx.ForecastedResult.Where(x => x.ForecastId == id && x.TotalValue != 0)
                .GroupBy(s => new { s.SiteId, s.ProductTypeId, s.ProductId })
                .Select(n => new
                {
                    n.Key.ProductTypeId,
                    n.Key.SiteId,
                    totalcost = Math.Round(Math.Ceiling(n.Sum(s => s.TotalValue) / n.Max(s => s.PackQty)) * n.Max(s => s.PackPrice), 2),
                    productname = n.Max(s => s.ProductId)
                }).ToList();

            cc.Totalcost = String.Format("{0:n}", list.Sum(x => x.totalcost));

            var list1 = ctx.ForecastedResult.Where(x => x.ForecastId == id && x.TotalValue != 0 && x.IsForGeneralConsumable == false)
                       .GroupBy(s => new { s.SiteId, s.ProductTypeId, s.ProductId })
                       .Select(n => new
                       {
                           n.Key.ProductTypeId,
                           n.Key.SiteId,
                           totalcost = Math.Round(Math.Ceiling(n.Sum(s => s.TotalValue) / n.Max(s => s.PackQty)) * n.Max(s => s.PackPrice), 2),
                           productname = n.Max(s => s.ProductId)
                       }).ToList();
            cc.Qccost = String.Format("{0:n}", list1.Sum(x => x.totalcost));
            var list2 = ctx.ForecastedResult.Where(x => x.ForecastId == id && x.TotalValue != 0 && x.IsForGeneralConsumable == true)
                         .GroupBy(s => new { s.SiteId, s.ProductTypeId, s.ProductId })
                         .Select(n => new
                         {
                             n.Key.ProductTypeId,
                             n.Key.SiteId,
                             totalcost = Math.Round(Math.Ceiling(n.Sum(s => s.TotalValue) / n.Max(s => s.PackQty)) * n.Max(s => s.PackPrice), 2),
                             productname = n.Max(s => s.ProductId)
                         }).ToList();
            cc.Cccost = String.Format("{0:n}", list2.Sum(x => x.totalcost));

            return cc;
        }
        //  Math.Round(Math.Ceiling(n.Sum(s => s.TotalValue) / n.Max(s => s.PackQty)) * n.Max(s => s.PackPrice), 2),
        public IList<ConductDashboardchartdata> GettestingareacostratioNEW(int fid)
        {
            decimal total = 0;
            List<ConductDashboardchartdata> Db = new List<ConductDashboardchartdata>();
            var list = ctx.ForecastedResult.Join(ctx.Test, b => b.TestId, c => c.TestID, (b, c) => new { b, c })
                .Join(ctx.TestingArea, d => d.c.TestingAreaID, e => e.TestingAreaID, (d, e) => new { d, e })
                   .Where(s => s.d.b.ForecastId == fid) //&& s.b.SiteId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension)
                .GroupBy(f => new { f.d.c.TestingAreaID })
                .Select(g => new
                {
                    g.Key,
                    TestingArea = g.Max(h => h.d.b.TestingArea),
                    Totalprice = g.Sum(h => h.d.b.PackPrice)

                }).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                total = total + list[i].Totalprice;
            }

            for (int i = 0; i < list.Count; i++)
            {

                Db.Add(new ConductDashboardchartdata
                {
                    Name = list[i].TestingArea,
                    Y = Math.Round((list[i].Totalprice / total) * 100, 2)

                });


            }
            return Db;
        }
        public IList<ConductDashboardchartdata> GetProducttypecostratiocategory(int id, int fid)
        {
            decimal total = 0;
            List<ConductDashboardchartdata> Db = new List<ConductDashboardchartdata>();
            var list = ctx.ForecastedResult.Join(ctx.ForecastInfo, b => b.ForecastId, c => c.ForecastID, (b, c) => new { b, c })
                .Where(s => s.b.PackPrice != 0 && s.b.ForecastId == fid && s.b.CategoryId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension))
                .GroupBy(f => f.b.ProductTypeId)
                .Select(g => new
                {
                    g.Key,
                    ProductType = g.Max(h => h.b.ProductType),
                    Totalprice = g.Sum(h => h.b.PackPrice)

                }).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                total = total + list[i].Totalprice;
            }

            for (int i = 0; i < list.Count; i++)
            {

                Db.Add(new ConductDashboardchartdata
                {
                    Name = list[i].ProductType,
                    Y = Math.Round((list[i].Totalprice / total) * 100, 2)

                });


            }
            return Db;
        }

        private DateTime getmaxdate(string Period, DateTime startdate, int extension)
        {
            DateTime Enddate;
            int @MonthAdded;
            @MonthAdded = extension;


            if (Period == "Bimonthly")
                @MonthAdded = extension * 2;

            if (Period == "Quarterly")
                @MonthAdded = extension * 3;

            if (Period == "Yearly") ;
            @MonthAdded = extension * 12;


            Enddate = startdate.AddMonths(@MonthAdded);
            return Enddate;

        }
        public Array Getdistinctduration(int id, int fid)
        {
            var ss = ctx.ForecastedResult.Join(ctx.ForecastInfo, b => b.ForecastId, c => c.ForecastID, (b, c) => new { b, c })
                  .Where(s => s.b.ForecastId == fid && s.b.SiteId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension))
               .GroupBy(f => new { f.b.ProductTypeId, f.b.Duration, f.b.DurationDateTime }).Select(x => new
               {
                   TotalPrice = x.Sum(g => g.b.PackPrice),
                   ProductType = x.Max(g => g.b.ProductType),
                   x.Key.Duration,
                   x.Key.DurationDateTime,
                   x.Key.ProductTypeId


               }).OrderBy(h => h.DurationDateTime);
            var distinctduration = ss.GroupBy(s => s.DurationDateTime).Select(x => new { Duration = x.Max(f => f.Duration) }).ToArray();
            return distinctduration;

        }
        public Array Getdistinctdurationnew(int fid)
        {
            var ss = ctx.ForecastedResult.Join(ctx.ForecastInfo, b => b.ForecastId, c => c.ForecastID, (b, c) => new { b, c })
                  .Where(s => s.b.ForecastId == fid) //&& s.b.SiteId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension)
               .GroupBy(f => new { f.b.ProductTypeId, f.b.Duration, f.b.DurationDateTime }).Select(x => new
               {
                   TotalPrice = x.Sum(g => g.b.PackPrice),
                   ProductType = x.Max(g => g.b.ProductType),
                   x.Key.Duration,
                   x.Key.DurationDateTime,
                   x.Key.ProductTypeId


               }).OrderBy(h => h.DurationDateTime);
            var distinctduration = ss.GroupBy(s => s.DurationDateTime).Select(x => new { Duration = x.Max(f => f.Duration) }).ToArray();
            return distinctduration;

        }
        public Array Getdistinctdurationservice(int fid)
        {
            var ss = ctx.ForecastedResult.Join(ctx.Test, b => b.TestId, c => c.TestID, (b, c) => new { b, c })
                .Join(ctx.TestingArea, d => d.c.TestingAreaID, e => e.TestingAreaID, (d, e) => new { d, e })
                  .Where(s => s.d.b.ForecastId == fid) //&& s.b.SiteId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension)
               .GroupBy(f => new { f.d.c.TestingAreaID, f.d.b.Duration, f.d.b.DurationDateTime }).Select(x => new
               {
                   TotalPrice = x.Sum(g => g.d.b.PackPrice),
                   ProductType = x.Max(g => g.d.b.TestingArea),
                   x.Key.Duration,
                   x.Key.DurationDateTime,
                   x.Key.TestingAreaID


               }).OrderBy(h => h.DurationDateTime);
            var distinctduration = ss.GroupBy(s => s.DurationDateTime).Select(x => new { Duration = x.Max(f => f.Duration) }).ToArray();
            return distinctduration;

        }
        public IList<ConductforecastDasboard> Getforecastsummarydurationforcategory(int id, int fid)
        {
            var ss = ctx.ForecastedResult.Join(ctx.ForecastInfo, b => b.ForecastId, c => c.ForecastID, (b, c) => new { b, c })
               .Where(s => s.b.ForecastId == fid && s.b.CategoryId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension))
            .GroupBy(f => new { f.b.ProductTypeId, f.b.Duration, f.b.DurationDateTime }).Select(x => new
            {
                TotalPrice = x.Sum(g => g.b.PackPrice),
                ProductType = x.Max(g => g.b.ProductType),
                x.Key.Duration,
                x.Key.DurationDateTime,
                x.Key.ProductTypeId


            }).OrderBy(h => h.DurationDateTime).ToList();


            var distinctproducttype = ss.GroupBy(s => s.ProductTypeId)
                .Select(x => new { x.Key, producttypename = x.Max(f => f.ProductType) }).ToList();

            var distinctduration = ss.GroupBy(s => s.DurationDateTime).Select(x => new { Duration = x.Max(f => f.Duration) }).ToList();

            List<ConductforecastDasboard> Db = new List<ConductforecastDasboard>();
            if (distinctduration.Count == 0)
                return Db;


            decimal[] data1 = new decimal[distinctduration.Count];



            for (int j = 0; j < distinctproducttype.Count; j++)
            {

                data1 = new decimal[distinctduration.Count];
                for (int i = 0; i < distinctduration.Count; i++)
                {
                    data1[i] = ss.Where(b => b.ProductTypeId == distinctproducttype[j].Key && b.Duration == distinctduration[i].Duration).Select(c => c.TotalPrice).FirstOrDefault();

                }

                Db.Add(new ConductforecastDasboard
                {

                    Name = distinctproducttype[j].producttypename,
                    Data = data1

                });



            }
            return Db;

        }
        public IList<ConductforecastDasboard> Getforecastsummarydurationforsite(int id, int fid)
        {

            //var forecastinfo = ctx.ForecastInfo.Find(fid);


            var ss = ctx.ForecastedResult.Join(ctx.ForecastInfo, b => b.ForecastId, c => c.ForecastID, (b, c) => new { b, c })
                   .Where(s => s.b.PackPrice != 0 && s.b.ForecastId == fid && s.b.SiteId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension))
                .GroupBy(f => new { f.b.ProductTypeId, f.b.Duration, f.b.DurationDateTime }).Select(x => new
                {
                    TotalPrice = x.Sum(g => g.b.PackPrice),
                    ProductType = x.Max(g => g.b.ProductType),
                    x.Key.Duration,
                    x.Key.DurationDateTime,
                    x.Key.ProductTypeId


                }).OrderBy(h => h.DurationDateTime).ToList();


            var distinctproducttype = ss.GroupBy(s => s.ProductTypeId)
                .Select(x => new { x.Key, producttypename = x.Max(f => f.ProductType) }).ToList();

            var distinctduration = ss.GroupBy(s => s.DurationDateTime).Select(x => new { Duration = x.Max(f => f.Duration) }).ToList();

            List<ConductforecastDasboard> Db = new List<ConductforecastDasboard>();
            if (distinctduration.Count == 0)
                return Db;


            decimal[] data1 = new decimal[distinctduration.Count];



            for (int j = 0; j < distinctproducttype.Count; j++)
            {

                data1 = new decimal[distinctduration.Count];
                for (int i = 0; i < distinctduration.Count; i++)
                {
                    data1[i] = ss.Where(b => b.ProductTypeId == distinctproducttype[j].Key && b.Duration == distinctduration[i].Duration).Select(c => c.TotalPrice).FirstOrDefault();

                }

                Db.Add(new ConductforecastDasboard
                {

                    Name = distinctproducttype[j].producttypename,
                    Data = data1

                });



            }
            return Db;

        }



        public IList<ConductforecastDasboard> Getforecastsummarydurationforsitenew(int fid)
        {

         var forecastinfo = ctx.ForecastInfo.Find(fid);
            List<ConductforecastDasboard> Db = new List<ConductforecastDasboard>();
            if (forecastinfo.Methodology.ToUpper() == "CONSUMPTION")
            {
                var ss = ctx.ForecastedResult.Join(ctx.ForecastInfo, b => b.ForecastId, c => c.ForecastID, (b, c) => new { b, c })
             .Where(s => s.b.ForecastId == fid &&  s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension))
          .GroupBy(f => new { f.b.ProductTypeId, f.b.Duration, f.b.DurationDateTime }).Select(x => new
          {
              TotalPrice = x.Sum(g => g.b.PackPrice),
              ProductType = x.Max(g => g.b.ProductType),
              x.Key.Duration,
              x.Key.DurationDateTime,
              x.Key.ProductTypeId


          }).OrderBy(h => h.DurationDateTime).ToList();


                var distinctproducttype = ss.GroupBy(s => s.ProductTypeId)
                    .Select(x => new { x.Key, producttypename = x.Max(f => f.ProductType) }).ToList();

                var distinctduration = ss.GroupBy(s => s.DurationDateTime).Select(x => new { Duration = x.Max(f => f.Duration) }).ToList();

              //  List<ConductforecastDasboard> Db = new List<ConductforecastDasboard>();
                if (distinctduration.Count == 0)
                    return Db;


                decimal[] data1 = new decimal[distinctduration.Count];



                for (int j = 0; j < distinctproducttype.Count; j++)
                {

                    data1 = new decimal[distinctduration.Count];
                    for (int i = 0; i < distinctduration.Count; i++)
                    {
                        data1[i] = ss.Where(b => b.ProductTypeId == distinctproducttype[j].Key && b.Duration == distinctduration[i].Duration).Select(c => c.TotalPrice).FirstOrDefault();

                    }

                    Db.Add(new ConductforecastDasboard
                    {

                        Name = distinctproducttype[j].producttypename,
                        Data = data1

                    });



                }
            }
            else
            {
                var ss = ctx.ForecastedResult.Join(ctx.ForecastInfo, b => b.ForecastId, c => c.ForecastID, (b, c) => new { b, c })
                       .Where(s => s.b.PackPrice != 0 && s.b.ForecastId == fid) //&& s.b.SiteId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension)
                    .GroupBy(f => new { f.b.TestingArea, f.b.Duration, f.b.DurationDateTime }).Select(x => new
                    {
                        TotalPrice = x.Sum(g => g.b.PackPrice),
                        ProductType = x.Max(g => g.b.ProductType),
                        x.Key.Duration,
                        x.Key.DurationDateTime,
                        x.Key.TestingArea


                    }).OrderBy(h => h.DurationDateTime).ToList();


                var distinctproducttype = ss.GroupBy(s => s.TestingArea)
                    .Select(x => new { TestingArea = x.Key }).ToList();

                var distinctduration = ss.GroupBy(s => s.DurationDateTime).Select(x => new { Duration = x.Max(f => f.Duration) }).ToList();


                if (distinctduration.Count == 0)
                    return Db;


                decimal[] data1 = new decimal[distinctduration.Count];



                for (int j = 0; j < distinctproducttype.Count; j++)
                {

                    data1 = new decimal[distinctduration.Count];
                    for (int i = 0; i < distinctduration.Count; i++)
                    {
                        data1[i] = ss.Where(b => b.TestingArea == distinctproducttype[j].TestingArea && b.Duration == distinctduration[i].Duration).Select(c => c.TotalPrice).FirstOrDefault();

                    }

                    Db.Add(new ConductforecastDasboard
                    {

                        Name = distinctproducttype[j].TestingArea,
                        Data = data1

                    });



                }
            }
            return Db;

        }


        public IList<ConductforecastDasboard> Getforecastsummarydurationforsiteservice(int fid)
        {

            //var forecastinfo = ctx.ForecastInfo.Find(fid);


            var ss = ctx.ForecastedResult.Join(ctx.Test, b => b.TestId, c => c.TestID, (b, c) => new { b, c })
                .Join(ctx.TestingArea, d => d.c.TestingAreaID, e => e.TestingAreaID, (d, e) => new { d, e })
                   .Where(s => s.d.b.ForecastId == fid) //&& s.b.SiteId == id && s.b.DurationDateTime >= s.c.StartDate && s.b.DurationDateTime < getmaxdate(s.c.Period, s.c.StartDate, s.c.Extension)
                .GroupBy(f => new { f.d.c.TestingAreaID, f.d.b.Duration, f.d.b.DurationDateTime }).Select(x => new
                {
                    TotalPrice = x.Sum(g => g.d.b.PackPrice),
                    TestingArea = x.Max(g => g.d.b.TestingArea),
                    x.Key.Duration,
                    x.Key.DurationDateTime,
                    x.Key.TestingAreaID


                }).OrderBy(h => h.DurationDateTime);


            var distinctproducttype = ss.GroupBy(s => s.TestingAreaID)
                .Select(x => new { x.Key, TestingArea = x.Max(f => f.TestingArea) }).ToList();

            var distinctduration = ss.GroupBy(s => s.DurationDateTime).Select(x => new { Duration = x.Max(f => f.Duration) }).ToList();

            List<ConductforecastDasboard> Db = new List<ConductforecastDasboard>();
            if (distinctduration.Count == 0)
                return Db;


            decimal[] data1 = new decimal[distinctduration.Count];



            for (int j = 0; j < distinctproducttype.Count; j++)
            {

                data1 = new decimal[distinctduration.Count];
                for (int i = 0; i < distinctduration.Count; i++)
                {
                    data1[i] = ss.Where(b => b.TestingAreaID == distinctproducttype[j].Key && b.Duration == distinctduration[i].Duration).Select(c => c.TotalPrice).FirstOrDefault();

                }

                Db.Add(new ConductforecastDasboard
                {

                    Name = distinctproducttype[j].TestingArea,
                    Data = data1

                });



            }
            return Db;

        }

        public Array Getforecastsite(int id)
        {
            Array A;
            var forecastinfo = ctx.ForecastInfo.Find(id);
            if (forecastinfo.DataUsage == "DATA_USAGE1" || forecastinfo.DataUsage == "DATA_USAGE2")
            {
                A = ctx.ForecastSite.Join(ctx.Site, b => b.SiteId, c => c.SiteID, (b, c) => new { b, c }).Where(s => s.b.ForecastInfoId == id)
                    .Select(x => new
                    {

                        siteid = x.b.SiteId,
                        sitename = x.c.SiteName
                    }).ToArray();
            }
            else
            {
                A = ctx.ForecastCategory.Where(b => b.ForecastId == id).Select(x => new
                {
                    siteid = x.CategoryId,
                    sitename = x.CategoryName

                }).ToArray();
            }

            return A;
        }
        public Array Getforecastlist(string metho, string datausage, int userid)
        {
            var forcastlist = ctx.ForecastInfo.Where(b => b.Methodology == metho && b.DataUsage == datausage && b.UserId == userid).Select(x => new
            {
                x.ForecastID,
                x.ForecastNo
            }

            ).ToArray();
            return forcastlist;
        }
        public string Calculateforecast(int id, string MethodType)
        {
            string res = "";

            try

            {
                param1 = MethodType.TrimEnd(',').Split(",");
                var _forecastInfo = ctx.ForecastInfo.Find(id);
               



                _listFresult = new List<ForecastedResult>();
                _Finalreslist = new List<Finalreslist>();
                if (_forecastInfo.DataUsage == "DATA_USAGE1" || _forecastInfo.DataUsage == "DATA_USAGE2")
                    ForcastSiteHistoricalData_new(_forecastInfo);
                // ForcastSiteHistoricalData(_forecastInfo);
                else
                    ForcastCategoryHistoricalData(_forecastInfo);



                SaveBulkForecastedResult(_forecastInfo);

                _forecastWithoutError = true;
                if (_forecastWithoutError == true)
                {
                    _forecastInfo.Status = "CLOSED";
                    _forecastInfo.Method = param1[0];
                    //_forecastInfo.ForecastDate = DateTime.Now;
                    _forecastInfo.Westage = Convert.ToDecimal(param1[1]);
                    _forecastInfo.Scaleup = Convert.ToDecimal(param1[2]);
                    ctx.SaveChanges();
                    res = "Forecasting process completed successfully";
                }


            }
            catch (Exception ex)
            {
                _forecastWithoutError = false;

                res = "";

            }

            return res;
        }
         private void ForcastSiteHistoricalData_new(ForecastInfo finfo)
        {
            try
            {
                string postURL = "http://34.69.151.1:8000/api/forecasts/";
                int months;
                var forecastsite = ctx.ForecastSite.Where(b => b.ForecastInfoId == finfo.ForecastID).ToList();
                // var forecastperiod = finfo.StartDate - finfo.ForecastDate;
                months = (finfo.ForecastDate.Value.Year - finfo.StartDate.Year) * 12;
                months -= finfo.StartDate.Month;
                months += finfo.ForecastDate.Value.Month;
                //   var hisdata = ctx.ForecastSite.Where(b=>b.ForecastInfoId == finfo.ForecastID).ToList();
                if (months == 0)

                {
                    months = 1;

                }
                // return months < 0 ? 0 : months + 1;
                //  forecastperiod.Value.TotalDays
                string Period = "";
                string apiparam = "";
                int countryid = ctx.User.Find(finfo.UserId).CountryId.Value;
                Period = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault().Period;
                if (Period == "Monthly")
                {
                    apiparam = "m";
                }
                else if (Period == "Yearly")
                {
                    apiparam = "y";
                }
                else
                {
                    apiparam = "m";
                }

                string xmlstring = "";
                var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x => x.Id).FirstOrDefault();
                List<MLModel> MLForecast = new List<MLModel>();
                var sites = ctx.Site.Where(b => b.UserId == finfo.UserId || b.CountryId == countryid).ToList();
                List<MasterProduct> MLProduct = new List<MasterProduct>();
                List<ProductType> MLProducttype = new List<ProductType>();
                List<Test> Tests = new List<Test>();
                List<TestingArea> TestingAreas = new List<TestingArea>();
                if (finfo.Methodology == "CONSUMPTION")
                {
                    MLProduct = ctx.MasterProduct.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
                    MLProducttype = ctx.ProductType.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
                }
                else
                {
                    Tests = ctx.Test.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
                    TestingAreas = ctx.TestingArea.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
                }

                ForecastCaculateModel fcm = new ForecastCaculateModel();
                fcm.forecastPeriod = months;
                fcm.frequency = apiparam;
                fcm.productSite = new List<ProductSite>();
                fcm.testSite = new List<TestSite>();

                for (int s = 0; s < forecastsite.Count; s++)
                {
                    if (finfo.Methodology == "CONSUMPTION")
                    {
                        ProductSite ps = new ProductSite();
                        ps.productIds = new List<int>();
                        IList<int> products = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == forecastsite[s].Id).GroupBy(x => x.ProductID).Select(x => x.Key).ToList();

                        if (products.Count > 0)
                        {
                            for (int e = 0; e < products.Count; e++)
                            {
                                ps.productIds.Add(products[e]);
                            }
                        }
                        ps.siteId = forecastsite[s].SiteId;
                        fcm.productSite.Add(ps);

                    }
                    else if (finfo.Methodology == "SERVICE STATSTICS")
                    {
                        TestSite ts = new TestSite();
                        ts.testIds = new List<int>();
                        IList<int> tests = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == forecastsite[s].Id).GroupBy(x => x.TestID).Select(f => f.Key).ToList();

                        if (tests.Count > 0)
                        {
                            for (int e = 0; e < tests.Count; e++)

                            {
                                ts.testIds.Add(tests[e]);
                            }
                            ts.siteId = forecastsite[s].SiteId;
                            fcm.testSite.Add(ts);

                            //HttpWebRequest request = default(HttpWebRequest);
                            //HttpWebResponse response = null;
                            //StreamReader reader = default(StreamReader);
                            //////  http://34.69.151.1:8000/api/forecasts/?productId=0&siteId=649&forecastPeriod=7&frequency=m
                            ///////get global region 
                            //string url = mainurl + "forecasts/?" + testids + "&siteId=" + forecastsite[s].SiteId + "&forecastPeriod=" + months + "&frequency=" + apiparam;
                            //request = (HttpWebRequest)WebRequest.Create(url);
                            //request.Timeout = 200000;
                            //response = (HttpWebResponse)request.GetResponse();
                            //reader = new StreamReader(response.GetResponseStream());
                            //xmlstring = reader.ReadToEnd();

                            //MLForecast = JsonConvert.DeserializeObject<List<MLModel>>(xmlstring);
                            //ReadDatasetconsumptionnew(finfo, MLForecast, Period, forecastsite[s].Id, sites, MLProduct, MLProducttype, Tests, TestingAreas);
                        }

                    }
                }


                HttpWebRequest request = default(HttpWebRequest);
                HttpWebResponse response = null;
                StreamReader reader = default(StreamReader);
                request = (HttpWebRequest)WebRequest.Create(postURL);

                var obj = JsonConvert.SerializeObject(fcm);

                request.Method = "POST";
                request.ContentType = "application/json";
                //request.ContentType = "application/json";
                //request.ContentLength = data.Length;

                using (var stream = new StreamWriter(request.GetRequestStream()))
                {
                  //  string json = new JavaScriptSerializer().Serialize(obj);
                    stream.Write(obj);
                    //stream.Flush();
                    //stream.Close();
                }

                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                xmlstring = reader.ReadToEnd();
             
                //HttpClient httpClient = new HttpClient();
                //httpClient.Timeout = TimeSpan.FromMinutes(30);
             
                //var content = new StringContent(obj, Encoding.UTF8, "application/json");
                //var result = await httpClient.PostAsync(postURL, content);

                //string responseBody = await result.Content.ReadAsStringAsync();
                List<MLResponseModel> MLResponseForecast = new List<MLResponseModel>();
                MLResponseForecast = JsonConvert.DeserializeObject<List<MLResponseModel>>(xmlstring);
                for (int s = 0; s < forecastsite.Count; s++)
                {
                    ReadDatasetconsumptionnew(finfo, MLResponseForecast, Period, forecastsite[s].Id, sites, MLProduct, MLProducttype, Tests, TestingAreas);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }

        public void ForcastCategoryHistoricalData(ForecastInfo FiFo)
        {
            int months;
            string xmlstring = "";
            var forecastcategory = ctx.ForecastCategory.Where(b => b.ForecastId == FiFo.ForecastID).ToList();
            months = (FiFo.ForecastDate.Value.Year - FiFo.StartDate.Year) * 12;
            months -= FiFo.StartDate.Month;
            months += FiFo.ForecastDate.Value.Month;
            string Period = "";
            string apiparam = "";
            int countryid = ctx.User.Find(FiFo.UserId).CountryId.Value;
            Period = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault().Period;
            if (Period == "Monthly")
            {
                apiparam = "m";
            }
            else if (Period == "Yearly")
            {
                apiparam = "y";
            }
            else
            {
                apiparam = "m";
            }

            MLModel MLForecast = new MLModel();
            foreach (ForecastCategory c in forecastcategory)
            {
                if (FiFo.Methodology == "CONSUMPTION")
                {
                    IList<int> products = ctx.ForecastCategoryProduct.Where(b => b.CategoryID == c.CategoryId).GroupBy(x => x.ProductID).Select(x => x.Key).ToList();
                    foreach (int p in products)
                    {
                        //   ClearChart1SeriesPoints();
                        List<ForecastCategoryProduct> fcatProduct = ctx.ForecastCategoryProduct.Where(b => b.CategoryID == c.CategoryId && b.ProductID == p).OrderBy(x => x.DurationDateTime).ToList();
                        DataTable InputDs = new DataTable();
                        InputDs.Columns.Add("X");
                        InputDs.Columns.Add("Y");
                        foreach (ForecastCategoryProduct cp in fcatProduct)
                        {


                            DataRow Dr = InputDs.NewRow();
                            Dr["X"] = cp.DurationDateTime;
                            Dr["Y"] = cp.Adjusted;
                            InputDs.Rows.Add(Dr);
                        }

                        HttpWebRequest request = default(HttpWebRequest);
                        HttpWebResponse response = null;
                        StreamReader reader = default(StreamReader);

                        /////get global region 
                        string url = mainurl + "forecasts/?productId=" + p + "&CategoryId=" + c.CategoryId + "&forecastPeriod=" + months + "&frequency = " + apiparam;
                        request = (HttpWebRequest)WebRequest.Create(url);

                        response = (HttpWebResponse)request.GetResponse();
                        reader = new StreamReader(response.GetResponseStream());
                        xmlstring = reader.ReadToEnd();

                        MLForecast = JsonConvert.DeserializeObject<MLModel>(xmlstring);



                        //  DataTable ds = Calculateforecastmultiplemethod(InputDs, FiFo);
                        ReadDatasetservice(FiFo, 0, c.CategoryId, p, 0, MLForecast, lastEntryDate, InputDs, Period);
                        // ReadDataset(FiFo, 0, c.CategoryId, p, 0, MLForecast, lastEntryDate, InputDs, Period);
                    }
                }
                else if (FiFo.Methodology == "SERVICE_STATISTIC")
                {
                    IList<int> tests = ctx.ForecastCategoryTest.Where(b => b.CategoryID == c.CategoryId).GroupBy(x => x.TestID).Select(x => x.Key).ToList();
                    foreach (int p in tests)
                    {

                        List<ForecastCategoryTest> fcatTest = ctx.ForecastCategoryTest.Where(b => b.CategoryID == c.CategoryId && b.TestID == p).OrderBy(x => x.DurationDateTime).ToList();

                        DataTable InputDs = new DataTable();
                        InputDs.Columns.Add("X");
                        InputDs.Columns.Add("Y");
                        foreach (ForecastCategoryTest ft in fcatTest)
                        {
                            DataRow Dr = InputDs.NewRow();
                            Dr["X"] = ft.DurationDateTime;
                            Dr["Y"] = ft.Adjusted;
                            InputDs.Rows.Add(Dr);
                        }
                        HttpWebRequest request = default(HttpWebRequest);
                        HttpWebResponse response = null;
                        StreamReader reader = default(StreamReader);

                        /////get global region 
                        string url = mainurl + "forecasts/?testId=" + p + "&CategoryId=" + c.CategoryId + "&forecastPeriod=" + months + "&frequency = " + apiparam;
                        request = (HttpWebRequest)WebRequest.Create(url);

                        response = (HttpWebResponse)request.GetResponse();
                        reader = new StreamReader(response.GetResponseStream());
                        xmlstring = reader.ReadToEnd();

                        MLForecast = JsonConvert.DeserializeObject<MLModel>(xmlstring);

                        lastEntryDate = fcatTest[fcatTest.Count - 1].DurationDateTime.Value;
                        //DataTable ds = Calculateforecastmultiplemethod(InputDs, FiFo);

                        ReadDatasetservice(FiFo, 0, c.CategoryId, 0, p, MLForecast, lastEntryDate, InputDs, Period);
                    }
                }
            }
        }

        private void SaveBulkForecastedResult(ForecastInfo finfo)
        {

            // string Producttypename = "";
            /// ProductPrice pp = new ProductPrice();
            MasterProduct MP = new MasterProduct();
            try
            {


                //      var Forecastcategories = ctx.ForecastCategory.Where(b => b.ForecastId == finfo.ForecastID).ToList();
                //   var forecastsite = ctx.Site.ToList();
                // var tests = ctx.Test.ToList();
                // var testarealist = ctx.TestingArea.ToList();
                //if (proid > 0)
                //{
                //    MP = ctx.MasterProduct.Find(proid);
                //    MP._productPrices = ctx.ProductPrice.Where(b => b.ProductId == proid).ToList();
                //    Producttypename = ctx.ProductType.Find(MP.ProductTypeId).TypeName;

                //}
                var Masterproduct = ctx.MasterProduct.ToList();
                var productpriceList = ctx.ProductPrice.ToList();
                var producttypelist = ctx.ProductType.ToList();
                var ForecastConsumableUsagelist = ctx.ForecastConsumableUsage.Where(b => b.Forecastid == finfo.ForecastID).ToList();
                var ForecastProductUsage = ctx.ForecastProductUsage.Where(b => b.Forecastid == finfo.ForecastID).ToList();
                var forecastins = ctx.ForecastIns.Where(b => b.forecastID == finfo.ForecastID).ToList();
                var inslist = ctx.Instrument.ToList();
                var siteins = ctx.siteinstrument.ToList();
                List<ForecastedResult> Finalresult = new List<ForecastedResult>();
                for (int i = 0; i < _Finalreslist.Count; i++)
                {

                    //  Finalreslist FP = new Finalreslist();
                    ForecastedResult FP = new ForecastedResult
                    {
                        ProductId = _Finalreslist[i].ProductId,
                        CategoryId = _Finalreslist[i].CategoryId,
                        ForecastId = _Finalreslist[i].ForecastId,
                        TestId = _Finalreslist[i].TestId,
                        ForecastValue = _Finalreslist[i].ForecastValue,
                        TotalValue = _Finalreslist[i].TotalValue,
                        DurationDateTime = _Finalreslist[i].DurationDateTime,
                        SiteId = _Finalreslist[i].SiteId,
                        Duration = _Finalreslist[i].Duration,
                        IsHistory = _Finalreslist[i].IsHistory,
                        HistoricalValue = _Finalreslist[i].HistoricalValue,
                        ServiceConverted = _Finalreslist[i].ServiceConverted,
                        TestingArea = _Finalreslist[i].TestingArea,
                        PackQty = _Finalreslist[i].Noofpack
                    };
                    if (_Finalreslist[i].ProductId > 0)
                    {

                        //MasterProduct p = Masterproduct.Where(b => b.ProductID == _listFresult[i].ProductId).FirstOrDefault();
                        //p._productPrices = productpriceList.Where(b => b.ProductId == _listFresult[i].ProductId).ToList();
                        //converting quantity to packsize commented out
                        //int packSize = p.GetActiveProductPrice(_listFresult[i].DurationDateTime).PackSize;
                        //_listFresult[i].PackQty = GetNoofPackage(packSize, _listFresult[i].TotalValue);

                        //rounding forecasted pack quantity

                        //   _listFresult[i].PackQty = Nopack;
                        MP = ctx.MasterProduct.Find(_Finalreslist[i].ProductId);
                        //FP.PackPrice = FP.PackQty * MP.GetActiveProductPrice(_Finalreslist[i].DurationDateTime).packcost;

                        FP.ProductTypeId = MP.ProductTypeId;
                        FP.ProductType = _Finalreslist[i].ProductType;
                        Finalresult.Add(FP);

                    }



                    //test to product
                    if (FP.TestId > 0)
                    {
                        //Test test = ctx.Test.Find(_listFresult[i].TestId);

                        #region Forecast General Consumables


                        //     IList<ForecastedResult> _consumablesDailyFlist = new List<ForecastedResult>();

                        var filterbytestidconsumusage = ForecastConsumableUsagelist.Where(b => b.TestId == FP.TestId).ToList();
                        //   foreach (ForecastConsumableUsage cusage in filterbytestidconsumusage)//DataRepository.GetConsumableUsageByTestId(_listFresult[i].TestId))
                        for (int j = 0; j < filterbytestidconsumusage.Count; j++)

                        {
                            //
                            ForecastedResult consumableresult = new ForecastedResult
                            {
                                //  consumableresult = _Finalreslist[i];
                                //copyvalues
                                ForecastId = FP.ForecastId,
                                TestId = FP.TestId,
                                DurationDateTime = FP.DurationDateTime,
                                SiteId = FP.SiteId,
                                CategoryId = FP.CategoryId,
                                Duration = FP.Duration,
                                IsHistory = FP.IsHistory,
                                TestingArea = FP.TestingArea,
                                ForecastValue = FP.ForecastValue,
                                TotalValue = FP.TotalValue
                            };
                            //endcopy
                            decimal Qty = 0;


                            if (filterbytestidconsumusage[j].PerInstrument)
                            {
                                if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                                {
                                    if (_Finalreslist[i].SiteId > 0)
                                    {
                                        //     int siteins = 0;//forecastins.Where(b => b.InsID == filterbytestidconsumusage[j].InstrumentId).FirstOrDefault().Quantity;
                                        var siteins1 = siteins.FirstOrDefault(e => e.InstrumentID == filterbytestidconsumusage[j].InstrumentId && e.SiteID== _Finalreslist[i].SiteId);
                                        if (siteins1 != null)
                                        {
                                            switch (filterbytestidconsumusage[j].Period.ToUpper())
                                            {
                                                case "DAILY":
                                                    Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinDay * siteins1.Quantity;
                                                    break;
                                                case "WEEKLY":
                                                    Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinWeek * siteins1.Quantity;
                                                    break;
                                                case "MONTHLY":
                                                    Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinMonth * siteins1.Quantity;
                                                    break;
                                                case "QUARTERLY":
                                                    Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinQuarter * siteins1.Quantity;
                                                    break;
                                                case "YEARLY":
                                                    Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinYear;
                                                    break;
                                                default:
                                                    break;
                                            }

                                          //  Qty = Qty * siteins.;
                                        }
                                    }
                                }
                            }
                            if (filterbytestidconsumusage[j].PerPeriod)
                            {
                                if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                                {
                                    if (_Finalreslist[i].SiteId > 0)
                                    {

                                        switch (filterbytestidconsumusage[j].Period.ToUpper())
                                        {
                                            case "DAILY":
                                                Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinDay;
                                                break;
                                            case "WEEKLY":
                                                Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinWeek;
                                                break;
                                            case "MONTHLY":
                                                Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinMonth;
                                                break;
                                            case "QUARTERLY":
                                                Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinQuarter;
                                                break;
                                            case "YEARLY":
                                                Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinYear;
                                                break;
                                            default:
                                                break;
                                        }



                                        //if (filterbytestidconsumusage[j].Period == "Daily")
                                        //{
                                        //    Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
                                        //}
                                        //if (filterbytestidconsumusage[j].Period == "Weekly")
                                        //{
                                        //    Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
                                        //}
                                        //if (filterbytestidconsumusage[j].Period == "Monthly")
                                        //{
                                        //    Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
                                        //}
                                        //if (filterbytestidconsumusage[j].Period == "Quarterly")
                                        //{
                                        //    Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
                                        //}
                                        //if (filterbytestidconsumusage[j].Period == "Yearly")
                                        //{
                                        //    Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
                                        //}
                                    }
                                }
                            }
                            if (filterbytestidconsumusage[j].PerTest)
                            {
                                if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                                {
                                    if (_Finalreslist[i].SiteId > 0)
                                    {
                                        Qty = (FP.ForecastValue / (filterbytestidconsumusage[j].UsageRate * filterbytestidconsumusage[j].NoOfTest));  // filterbytestidconsumusage[j].UsageRate * (FP.ForecastValue / filterbytestidconsumusage[j].NoOfTest);
                                    }
                                }
                            }

                            consumableresult.TotalValue = Qty;
                            MasterProduct MP1 = new MasterProduct();
                            MP1 = Masterproduct.Where(b => b.ProductID == filterbytestidconsumusage[j].ProductId).FirstOrDefault();
                            MP1._productPrices = productpriceList.Where(b => b.ProductId == MP1.ProductID).ToList();
                            int packSize = MP1.GetActiveProductPrice(FP.DurationDateTime).packsize;
                            consumableresult.ProductId = filterbytestidconsumusage[j].ProductId;
                            //consumableresult.PackQty = GetNoofPackage(packSize, Qty);
                            //consumableresult.PackPrice = consumableresult.PackQty * MP1.GetActiveProductPrice(FP.DurationDateTime).packcost;
                            consumableresult.ProductTypeId = MP1.ProductTypeId;
                            consumableresult.ProductType = ctx.ProductType.Find(MP1.ProductTypeId).TypeName;
                            consumableresult.IsForGeneralConsumable = true;
                            consumableresult.ServiceConverted = true;
                            Finalresult.Add(consumableresult);


                        }



                        #endregion

                        #region Forecast Control Test

                        //////if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                        //////{
                        //////    if (fsite != null)
                        //////    {
                        //////        //ForecastIns siteins =forecastins.Join(ctx.Instrument, b => b.InsID, c => c.InstrumentID, (b, c) => new { b, c })
                        //////        //    .Where(x => x.c.testingArea.TestingAreaID == testareaid)
                        //////        //    .Select(s => new ForecastIns
                        //////        //    {

                        //////        //        ID = s.b.ID,
                        //////        //        InsID = s.b.InsID
                        //////        //    }).FirstOrDefault();
                        //////        //fsite.GetSiteInstrumentByTA(test.TestingArea.Id);
                        //////        if (forecastins != null)
                        //////        {
                        //////            if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
                        //////            {
                        //////                _Finalreslist[i].ControlTest = _Finalreslist[i].FPinDay * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.DailyCtrlTest).FirstOrDefault();
                        //////            }
                        //////            if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
                        //////            {
                        //////                _Finalreslist[i].ControlTest = _Finalreslist[i].FPinWeek * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
                        //////            }
                        //////            if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
                        //////            {
                        //////                _listFresult[i].ControlTest = _Finalreslist[i].FPinMonth * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();

                        //////            }
                        //////            if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
                        //////            {
                        //////                _listFresult[i].ControlTest = _Finalreslist[i].FPinQuarter * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
                        //////            }
                        //////            if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
                        //////            {
                        //////                _listFresult[i].ControlTest = ((_Finalreslist[i].TotalValue * (forecastins.FirstOrDefault().TestRunPercentage / 100)) / inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
                        //////            }
                        //////        }
                        //////    }
                        //////}

                        #endregion

                        #region Test Test to Product
                        var productusage = ForecastProductUsage.Where(b => b.TestId == _Finalreslist[i].TestId && b.IsForControl == false).ToList();
                        // foreach (ForecastProductUsage pu in productusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
                        for (int ki = 0; ki < productusage.Count; ki++)

                        {
                            //ForecastedResult c_listFresult = new ForecastedResult
                            //{
                            //    //copyvalues
                            //    ForecastId = FP.ForecastId,
                            //    TestId = FP.TestId,
                            //    DurationDateTime = FP.DurationDateTime,
                            //    SiteId = FP.SiteId,
                            //    CategoryId = FP.CategoryId,
                            //    Duration = FP.Duration,
                            //    IsHistory = FP.IsHistory,
                            //    TestingArea = FP.TestingArea
                            //};

                            ForecastedResult c_listFresult = new ForecastedResult
                            {
                                //  consumableresult = _Finalreslist[i];
                                //copyvalues
                                ForecastId = FP.ForecastId,
                                TestId = FP.TestId,
                                DurationDateTime = FP.DurationDateTime,
                                SiteId = FP.SiteId,
                                CategoryId = FP.CategoryId,
                                Duration = FP.Duration,
                                IsHistory = FP.IsHistory,
                                TestingArea = FP.TestingArea,
                                ForecastValue = FP.ForecastValue,
                                TotalValue = FP.TotalValue
                            };
                            //endcopy

                            //if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                            //{
                            //    if (_Finalreslist[i].SiteId > 0)
                            //    {
                            //        var siteins1 = siteins.Where(b => b.InstrumentID == productusage[ki].InstrumentId && b.SiteID==_Finalreslist[i].SiteId).FirstOrDefault();
                            //        if (siteins1 != null)
                            //        {
                                        decimal Qty = productusage[ki].Rate * _Finalreslist[i].ForecastValue ;
                                        c_listFresult.TotalValue = Qty;
                                        MasterProduct p = Masterproduct.Where(b => b.ProductID == productusage[ki].ProductId).FirstOrDefault();
                                        p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
                                        int packSize = p.GetActiveProductPrice(_Finalreslist[i].DurationDateTime).packsize;
                                        c_listFresult.ProductId = productusage[ki].ProductId;
                                        //c_listFresult.PackQty = GetNoofPackage(packSize, Qty);
                                        //c_listFresult.PackPrice = c_listFresult.PackQty * p.GetActiveProductPrice(FP.DurationDateTime).packcost;

                                        c_listFresult.ProductTypeId = Masterproduct.Where(b => b.ProductID == productusage[ki].ProductId).FirstOrDefault().ProductTypeId;
                                        c_listFresult.ProductType = producttypelist.Where(b => b.TypeID == c_listFresult.ProductTypeId).FirstOrDefault().TypeName;
                                        c_listFresult.ServiceConverted = true;
                                        c_listFresult.IsForGeneralConsumable = false;
                                        c_listFresult.IsForControl = false;
                                        Finalresult.Add(c_listFresult);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    //if (fsite != null)
                            //    //{
                            //    //    ForecastCategoryInstrument fcins = DataRepository.GetForecastCategoryInstrumentById(pu.Instrument.Id);

                            //    //    if (fcins != null)
                            //    //    {
                            //    //        decimal Qty = pu.Rate * _listFresult[i].TotalValue * fcins.TestRunPercentage;
                            //    //        c_listFresult[i].TotalValue = Qty;
                            //    //        int packSize = pu.Product.GetActiveProductPrice(_listFresult[i].DurationDateTime).PackSize;
                            //    //        c_listFresult[i].ProductId = pu.Product.Id;
                            //    //        c_listFresult[i].PackQty = GetNoofPackage(packSize, Qty);
                            //    //        c_listFresult[i].PackPrice = c_listFresult[i].PackQty * pu.Product.GetActiveProductPrice(_listFresult[i].DurationDateTime).Price;

                            //    //        c_listFresult[i].ProductTypeId = pu.Product.ProductType.Id;
                            //    //        c_listFresult[i].ProductType = pu.Product.ProductType.TypeName;
                            //    //        c_listFresult[i].ServiceConverted = true;
                            //    //        _list_listFresult[i].Add(c_listFresult[i]);
                            //    //    }
                            //    //}
                            //}
                        }
                        ///////// 
                        #endregion

                        #region Control Test to Product


                        var cproductusage = ForecastProductUsage.Where(b => b.TestId == _Finalreslist[i].TestId && b.IsForControl == true).ToList();
                        // foreach (ForecastProductUsage pu in cproductusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
                        for (int z = 0; z < cproductusage.Count; z++)

                        {
                            //ForecastedResult cprod = new ForecastedResult();
                            ForecastedResult cprod = new ForecastedResult
                            {
                                //  consumableresult = _Finalreslist[i];
                                //copyvalues
                                ForecastId = FP.ForecastId,
                                TestId = FP.TestId,
                                DurationDateTime = FP.DurationDateTime,
                                SiteId = FP.SiteId,
                                CategoryId = FP.CategoryId,
                                Duration = FP.Duration,
                                IsHistory = FP.IsHistory,
                                TestingArea = FP.TestingArea,
                                ForecastValue = FP.ForecastValue,
                                TotalValue = FP.TotalValue
                            };
                            //copyvalues
                            //c_listFresult[i].ForecastId = _listFresult[i].ForecastId;
                            //c_listFresult[i].TestId = _listFresult[i].TestId;
                            //c_listFresult[i].DurationDateTime = _listFresult[i].DurationDateTime;
                            //c_listFresult[i].SiteId = _listFresult[i].SiteId;
                            //c_listFresult[i].CategoryId = _listFresult[i].CategoryId;
                            //c_listFresult[i].Duration = _listFresult[i].Duration;
                            //c_listFresult[i].IsHistory = _listFresult[i].IsHistory;
                            //c_listFresult[i].TestingArea = _listFresult[i].TestingArea;

                            cprod.IsForControl = true;
                            //endcopy

                            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                            {
                                if (_Finalreslist[i].SiteId > 0)
                                {
                                    var siteins1 = siteins.Where(b => b.InstrumentID == cproductusage[z].InstrumentId && b.SiteID== _Finalreslist[i].SiteId).FirstOrDefault();
                                    if (siteins1 != null)
                                    {


                                        decimal Qty = 0;
                                        if (inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
                                        {
                                            Qty = cproductusage[z].Rate * _Finalreslist[i].FPinDay * inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.DailyCtrlTest).FirstOrDefault();
                                            // c_listFresult[i].ForecastValue = fPinDay * siteins.Quantity * siteins.Instrument.DailyCtrlTest;
                                        }
                                        if (inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
                                        {
                                            Qty = cproductusage[z].Rate * _Finalreslist[i].FPinWeek  * inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
                                            // c_listFresult[i].ForecastValue = fPinWeek * siteins.Quantity * siteins.Instrument.WeeklyCtrlTest;
                                        }
                                        if (inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
                                        {
                                            Qty = cproductusage[z].Rate * _Finalreslist[i].FPinMonth *  inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();
                                            // c_listFresult[i].ForecastValue = fPinMonth * siteins.Quantity * siteins.Instrument.MonthlyCtrlTest;

                                        }
                                        if (inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
                                        {
                                            Qty = cproductusage[z].Rate * _Finalreslist[i].FPinQuarter * inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
                                            //c_listFresult[i].ForecastValue = fPinQuarter * siteins.Quantity * siteins.Instrument.QuarterlyCtrlTest;
                                        }
                                        if (inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
                                        {
                                            Qty = cproductusage[z].Rate * ((_Finalreslist[i].TotalValue * (siteins1.TestRunPercentage / 100)) / inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
                                            // c_listFresult[i].ForecastValue = ((_listFresult[i].TotalValue * (siteins.TestRunPercentage / 100)) / siteins.Instrument.MaxTestBeforeCtrlTest);
                                        }


                                        //decimal Qty = pu.Rate * _listFresult[i].TotalValue * siteins.TestRunPercentage / 100;

                                        cprod.TotalValue = Qty;
                                        MasterProduct p = Masterproduct.Where(b => b.ProductID == cproductusage[z].ProductId).FirstOrDefault();
                                        p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
                                        int packSize = p.GetActiveProductPrice(_Finalreslist[i].DurationDateTime).packsize;
                                        cprod.ProductId = cproductusage[z].ProductId;
                                        //cprod.PackQty = GetNoofPackage(packSize, Qty);
                                        //cprod.PackPrice = cprod.PackQty * p.GetActiveProductPrice(_Finalreslist[i].DurationDateTime).packcost;

                                        cprod.ProductTypeId = Masterproduct.Where(b => b.ProductID == cproductusage[z].ProductId).FirstOrDefault().ProductTypeId;
                                        cprod.ProductType = producttypelist.Where(b => b.TypeID == cprod.ProductTypeId).FirstOrDefault().TypeName;
                                        cprod.ServiceConverted = true;
                                        cprod.IsForGeneralConsumable = false;
                                        cprod.IsForControl = true;
                                        Finalresult.Add(cprod);
                                    }
                                }
                            }
                            else
                            {

                            }
                        }
                        ///////// 
                        #endregion

                    }
                    //SqlConnection sqlConnection = ConnectionManager.GetInstance().GetSqlConnection();
                    //DataTable _listFresult[i]dt = GenericToDataTable.ConvertTo(_list_listFresult[i]);
                    //if (sqlConnection.State != ConnectionState.Open)
                    //{
                    //    sqlConnection.Open();
                    //}
                    //using (var bulkCopy = new SqlBulkCopy(sqlConnection))
                    //{
                    //    bulkCopy.DestinationTableName = "dbo.ForecastedResult";
                    //    bulkCopy.WriteToServer(_listFresult[i]dt);
                    //}
                }

                var final = Finalresult.GroupBy(s => new { s.SiteId, s.ProductTypeId, s.ProductId }).Select(s => new ForecastedResult
                {
                    ForecastId = s.Max(x=>x.ForecastId),
                    TestId = s.Max(x => x.TestId),
                    DurationDateTime = s.Max(x => x.DurationDateTime),
                    SiteId = s.Key.SiteId,
                    CategoryId = s.Max(x => x.CategoryId),
                    Duration = s.Max(x => x.Duration),
                    IsHistory = s.Max(x => x.IsHistory),
                    TestingArea = s.Max(x => x.TestingArea),
                    ForecastValue = s.Sum(x => x.ForecastValue),
                    TotalValue = s.Sum(x => x.TotalValue),
                  ProductId = s.Key.ProductId,
                    //c_listFresult.PackQty = GetNoofPackage(packSize, Qty);
                    //c_listFresult.PackPrice = c_listFresult.PackQty * p.GetActiveProductPrice(FP.DurationDateTime).packcost;

                    ProductTypeId = s.Key.ProductTypeId,
                    ProductType = s.Max(x => x.ProductType),
                    ServiceConverted = s.Max(x => x.ServiceConverted),
                    IsForGeneralConsumable = s.Max(x => x.IsForGeneralConsumable),
                    IsForControl = s.Max(x => x.IsForControl),

                }).ToList();
                for (int i = 0; i < final.Count; i++)
                {
                    MasterProduct p = Masterproduct.Where(b => b.ProductID == final[i].ProductId).FirstOrDefault();
                    p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
                    int packSize = p.GetActiveProductPrice(final[i].DurationDateTime).packsize;
                 
                    int packqty = GetNoofPackage(packSize, final[i].TotalValue);
                    final[i].PackQty = packqty;
                    final[i].PackPrice = packqty * p.GetActiveProductPrice(final[i].DurationDateTime).packcost;
                }


                if (finfo.Status != "OPEN")
                {
                    //if (
                    //    MessageBox.Show(
                    //        "This forecast is already forecasted.Are you sure do you want to overwrite this forecast?",
                    //        "Forecasting Process", MessageBoxButtons.YesNo, MessageBoxIcon.Question) !=
                    //    DialogResult.Yes)
                    //{
                    //    return;
                    //}
                    var forecastresult = ctx.ForecastedResult.Where(b => b.ForecastId == finfo.ForecastID).ToList();
                    ctx.ForecastedResult.RemoveRange(forecastresult);
                    ctx.SaveChanges();
                }
                ctx.ForecastedResult.AddRange(final);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        //private void SaveBulkForecastedResult_new(ForecastInfo finfo)
        //{

        //    // string Producttypename = "";
        //    /// ProductPrice pp = new ProductPrice();
        //    MasterProduct MP = new MasterProduct();
        //    try
        //    {


        //        //      var Forecastcategories = ctx.ForecastCategory.Where(b => b.ForecastId == finfo.ForecastID).ToList();
        //        //   var forecastsite = ctx.Site.ToList();
        //        // var tests = ctx.Test.ToList();
        //        // var testarealist = ctx.TestingArea.ToList();
        //        //if (proid > 0)
        //        //{
        //        //    MP = ctx.MasterProduct.Find(proid);
        //        //    MP._productPrices = ctx.ProductPrice.Where(b => b.ProductId == proid).ToList();
        //        //    Producttypename = ctx.ProductType.Find(MP.ProductTypeId).TypeName;

        //        //}
        //        var Masterproduct = ctx.MasterProduct.ToList();
        //        var productpriceList = ctx.ProductPrice.ToList();
        //        var producttypelist = ctx.ProductType.ToList();
        //        var ForecastConsumableUsagelist = ctx.ForecastConsumableUsage.Where(b => b.Forecastid == finfo.ForecastID).ToList();
        //        var ForecastProductUsage = ctx.ForecastProductUsage.Where(b => b.Forecastid == finfo.ForecastID).ToList();
        //        var forecastins = ctx.ForecastIns.Where(b => b.forecastID == finfo.ForecastID).ToList();
        //        var inslist = ctx.Instrument.ToList();
        //        var siteins = ctx.siteinstrument.ToList();
        //        List<ForecastedResult> Finalresult = new List<ForecastedResult>();
        //        DataTable dt = new DataTable();
        //        dt.Columns.Add("ProductId");
        //        dt.Columns.Add("CategoryId");
        //        dt.Columns.Add("ForecastId");
        //        dt.Columns.Add("TestId");

        //        dt.Columns.Add("ForecastValue");
        //        dt.Columns.Add("TotalValue");
        //        dt.Columns.Add("DurationDateTime");
        //        dt.Columns.Add("SiteId");
        //        dt.Columns.Add("Duration");
        //        dt.Columns.Add("IsHistory");

        //        dt.Columns.Add("HistoricalValue");
        //        dt.Columns.Add("ServiceConverted");
        //        dt.Columns.Add("TestingArea");
        //        dt.Columns.Add("PackQty");
        //        dt.Columns.Add("PackPrice");
        //        dt.Columns.Add("ProductTypeId");
        //        dt.Columns.Add("ProductType");
        //        dt.Columns.Add("IsForGeneralConsumable");
        //        dt.Columns.Add("IsForControl");

        //        DataRow Dr;
        //        for (int i = 0; i < _Finalreslist.Count; i++)
        //        {

        //            //  Finalreslist FP = new Finalreslist();
        //            ForecastedResult FP = new ForecastedResult
        //            {
        //                ProductId = _Finalreslist[i].ProductId,
        //                CategoryId = _Finalreslist[i].CategoryId,
        //                ForecastId = _Finalreslist[i].ForecastId,
        //                TestId = _Finalreslist[i].TestId,
        //                ForecastValue = _Finalreslist[i].ForecastValue,
        //                TotalValue = _Finalreslist[i].TotalValue,
        //                DurationDateTime = _Finalreslist[i].DurationDateTime,
        //                SiteId = _Finalreslist[i].SiteId,
        //                Duration = _Finalreslist[i].Duration,
        //                IsHistory = _Finalreslist[i].IsHistory,
        //                HistoricalValue = _Finalreslist[i].HistoricalValue,
        //                ServiceConverted = _Finalreslist[i].ServiceConverted,
        //                TestingArea = _Finalreslist[i].TestingArea,
        //                PackQty = _Finalreslist[i].Noofpack
        //            };
        //            if (_Finalreslist[i].ProductId > 0)
        //            {
        //                MP = ctx.MasterProduct.Find(_Finalreslist[i].ProductId);
        //                Dr = dt.NewRow();
        //                Dr["ProductId"] = _Finalreslist[i].ProductId;
        //                Dr["CategoryId"] = _Finalreslist[i].CategoryId;
        //                Dr["ForecastId"] = _Finalreslist[i].ForecastId;
        //                Dr["TestId"] = _Finalreslist[i].TestId;

        //                Dr["ForecastValue"] = _Finalreslist[i].ForecastValue;
        //                Dr["TotalValue"] = _Finalreslist[i].TotalValue;
        //                Dr["DurationDateTime"] = _Finalreslist[i].DurationDateTime;
        //                Dr["SiteId"] = _Finalreslist[i].SiteId;
        //                Dr["Duration"] = _Finalreslist[i].Duration;
        //                Dr["IsHistory"] = _Finalreslist[i].IsHistory;

        //                Dr["HistoricalValue"] = _Finalreslist[i].HistoricalValue;
        //                Dr["ServiceConverted"] = _Finalreslist[i].ServiceConverted;
        //                Dr["TestingArea"] = _Finalreslist[i].TestingArea;
        //                Dr["PackQty"] = _Finalreslist[i].Noofpack;
        //                Dr["PackPrice"] = FP.PackQty * MP.GetActiveProductPrice(_Finalreslist[i].DurationDateTime).packcost;
        //                Dr["ProductTypeId"] = MP.ProductTypeId;
        //                Dr["ProductType"] = _Finalreslist[i].ProductType;
        //                dt.Rows.Add(Dr);
        //                //FP.PackPrice = FP.PackQty * MP.GetActiveProductPrice(_Finalreslist[i].DurationDateTime).packcost;

        //                //FP.ProductTypeId = MP.ProductTypeId;
        //                //FP.ProductType = _Finalreslist[i].ProductType;
        //                //Finalresult.Add(FP);

        //            }



        //            //test to product
        //            if (FP.TestId > 0)
        //            {
        //                //Test test = ctx.Test.Find(_listFresult[i].TestId);

        //                #region Forecast General Consumables


        //                //     IList<ForecastedResult> _consumablesDailyFlist = new List<ForecastedResult>();

        //                var filterbytestidconsumusage = ForecastConsumableUsagelist.Where(b => b.TestId == FP.TestId).ToList();
        //                //   foreach (ForecastConsumableUsage cusage in filterbytestidconsumusage)//DataRepository.GetConsumableUsageByTestId(_listFresult[i].TestId))
        //                for (int j = 0; j < filterbytestidconsumusage.Count; j++)
        //                {
        //                    //
                           
        //                    Dr = dt.NewRow();
                           
        //                    Dr["CategoryId"] = _Finalreslist[i].CategoryId;
        //                    Dr["ForecastId"] = _Finalreslist[i].ForecastId;
        //                    Dr["TestId"] = _Finalreslist[i].TestId;

        //                    Dr["ForecastValue"] = _Finalreslist[i].ForecastValue;
        //                  //  Dr["TotalValue"] = _Finalreslist[i].TotalValue;
        //                    Dr["DurationDateTime"] = _Finalreslist[i].DurationDateTime;
        //                    Dr["SiteId"] = _Finalreslist[i].SiteId;
        //                    Dr["Duration"] = _Finalreslist[i].Duration;
        //                    Dr["IsHistory"] = _Finalreslist[i].IsHistory;

                          
        //                    Dr["TestingArea"] = _Finalreslist[i].TestingArea;
                         
        //                    //endcopy
        //                    decimal Qty = 0;


        //                    if (filterbytestidconsumusage[j].PerInstrument)
        //                    {
        //                        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //                        {
        //                            if (_Finalreslist[i].SiteId > 0)
        //                            {
        //                                //     int siteins = 0;//forecastins.Where(b => b.InsID == filterbytestidconsumusage[j].InstrumentId).FirstOrDefault().Quantity;
        //                                var siteins1 = inslist.FirstOrDefault(e => e.InstrumentID == filterbytestidconsumusage[j].InstrumentId);
        //                                if (siteins1 != null)
        //                                {
        //                                    switch (filterbytestidconsumusage[j].Period.ToUpper())
        //                                    {
        //                                        case "DAILY":
        //                                            Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinDay;
        //                                            break;
        //                                        case "WEEKLY":
        //                                            Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinWeek;
        //                                            break;
        //                                        case "MONTHLY":
        //                                            Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinMonth;
        //                                            break;
        //                                        case "QUARTERLY":
        //                                            Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinQuarter;
        //                                            break;
        //                                        case "YEARLY":
        //                                            Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinYear;
        //                                            break;
        //                                        default:
        //                                            break;
        //                                    }

        //                                    //  Qty = Qty * siteins.;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    if (filterbytestidconsumusage[j].PerPeriod)
        //                    {
        //                        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //                        {
        //                            if (_Finalreslist[i].SiteId > 0)
        //                            {

        //                                switch (filterbytestidconsumusage[j].Period.ToUpper())
        //                                {
        //                                    case "DAILY":
        //                                        Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinDay;
        //                                        break;
        //                                    case "WEEKLY":
        //                                        Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinWeek;
        //                                        break;
        //                                    case "MONTHLY":
        //                                        Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinMonth;
        //                                        break;
        //                                    case "QUARTERLY":
        //                                        Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinQuarter;
        //                                        break;
        //                                    case "YEARLY":
        //                                        Qty = filterbytestidconsumusage[j].UsageRate * _Finalreslist[i].FPinYear;
        //                                        break;
        //                                    default:
        //                                        break;
        //                                }



        //                                //if (filterbytestidconsumusage[j].Period == "Daily")
        //                                //{
        //                                //    Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
        //                                //}
        //                                //if (filterbytestidconsumusage[j].Period == "Weekly")
        //                                //{
        //                                //    Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
        //                                //}
        //                                //if (filterbytestidconsumusage[j].Period == "Monthly")
        //                                //{
        //                                //    Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
        //                                //}
        //                                //if (filterbytestidconsumusage[j].Period == "Quarterly")
        //                                //{
        //                                //    Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
        //                                //}
        //                                //if (filterbytestidconsumusage[j].Period == "Yearly")
        //                                //{
        //                                //    Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
        //                                //}
        //                            }
        //                        }
        //                    }
        //                    if (filterbytestidconsumusage[j].PerTest)
        //                    {
        //                        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //                        {
        //                            if (_Finalreslist[i].SiteId > 0)
        //                            {
        //                                Qty = filterbytestidconsumusage[j].UsageRate * (FP.TotalValue / filterbytestidconsumusage[j].NoOfTest);
        //                            }
        //                        }
        //                    }

                          
        //                    Dr["TotalValue"] = Qty;
        //                    MasterProduct MP1 = new MasterProduct();
        //                    MP1 = Masterproduct.Where(b => b.ProductID == filterbytestidconsumusage[j].ProductId).FirstOrDefault();
        //                    MP1._productPrices = productpriceList.Where(b => b.ProductId == MP1.ProductID).ToList();
        //                    int packSize = MP1.GetActiveProductPrice(FP.DurationDateTime).packsize;
        //                    Dr["ProductId"] = filterbytestidconsumusage[j].ProductId;
        //                 //   Dr["HistoricalValue"] = _Finalreslist[i].HistoricalValue;
        //                    Dr["ServiceConverted"] = true;
        //                    Dr["PackQty"] = GetNoofPackage(packSize, Qty);
        //                    Dr["PackPrice"] = GetNoofPackage(packSize, Qty) * MP1.GetActiveProductPrice(FP.DurationDateTime).packcost;
        //                    Dr["ProductTypeId"] = MP1.ProductTypeId;
        //                    Dr["ProductType"] = ctx.ProductType.Find(MP1.ProductTypeId).TypeName;
        //                    Dr["IsForGeneralConsumable"] = true;
        //                    dt.Rows.Add(Dr);
                     

        //                }



        //                #endregion

        //                #region Forecast Control Test

        //                //////if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //                //////{
        //                //////    if (fsite != null)
        //                //////    {
        //                //////        //ForecastIns siteins =forecastins.Join(ctx.Instrument, b => b.InsID, c => c.InstrumentID, (b, c) => new { b, c })
        //                //////        //    .Where(x => x.c.testingArea.TestingAreaID == testareaid)
        //                //////        //    .Select(s => new ForecastIns
        //                //////        //    {

        //                //////        //        ID = s.b.ID,
        //                //////        //        InsID = s.b.InsID
        //                //////        //    }).FirstOrDefault();
        //                //////        //fsite.GetSiteInstrumentByTA(test.TestingArea.Id);
        //                //////        if (forecastins != null)
        //                //////        {
        //                //////            if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
        //                //////            {
        //                //////                _Finalreslist[i].ControlTest = _Finalreslist[i].FPinDay * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.DailyCtrlTest).FirstOrDefault();
        //                //////            }
        //                //////            if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
        //                //////            {
        //                //////                _Finalreslist[i].ControlTest = _Finalreslist[i].FPinWeek * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
        //                //////            }
        //                //////            if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
        //                //////            {
        //                //////                _listFresult[i].ControlTest = _Finalreslist[i].FPinMonth * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();

        //                //////            }
        //                //////            if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
        //                //////            {
        //                //////                _listFresult[i].ControlTest = _Finalreslist[i].FPinQuarter * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
        //                //////            }
        //                //////            if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
        //                //////            {
        //                //////                _listFresult[i].ControlTest = ((_Finalreslist[i].TotalValue * (forecastins.FirstOrDefault().TestRunPercentage / 100)) / inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
        //                //////            }
        //                //////        }
        //                //////    }
        //                //////}

        //                #endregion

        //                #region Test Test to Product
        //                var productusage = ForecastProductUsage.Where(b => b.TestId == _Finalreslist[i].TestId && b.IsForControl == false).ToList();
        //                // foreach (ForecastProductUsage pu in productusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
        //                for (int ki = 0; ki < productusage.Count; ki++)
        //                {
        //                    //ForecastedResult c_listFresult = new ForecastedResult
        //                    //{
        //                    //    //copyvalues
        //                    //    ForecastId = FP.ForecastId,
        //                    //    TestId = FP.TestId,
        //                    //    DurationDateTime = FP.DurationDateTime,
        //                    //    SiteId = FP.SiteId,
        //                    //    CategoryId = FP.CategoryId,
        //                    //    Duration = FP.Duration,
        //                    //    IsHistory = FP.IsHistory,
        //                    //    TestingArea = FP.TestingArea
        //                    //};

        //                    //ForecastedResult c_listFresult = new ForecastedResult
        //                    //{
        //                    //    //  consumableresult = _Finalreslist[i];
        //                    //    //copyvalues
        //                    //    ForecastId = FP.ForecastId,
        //                    //    TestId = FP.TestId,
        //                    //    DurationDateTime = FP.DurationDateTime,
        //                    //    SiteId = FP.SiteId,
        //                    //    CategoryId = FP.CategoryId,
        //                    //    Duration = FP.Duration,
        //                    //    IsHistory = FP.IsHistory,
        //                    //    TestingArea = FP.TestingArea,
        //                    //    ForecastValue = FP.ForecastValue,
        //                    //    TotalValue = FP.TotalValue
        //                    //};
        //                    Dr = dt.NewRow();

        //                    Dr["CategoryId"] = _Finalreslist[i].CategoryId;
        //                    Dr["ForecastId"] = _Finalreslist[i].ForecastId;
        //                    Dr["TestId"] = _Finalreslist[i].TestId;

        //                    Dr["ForecastValue"] = _Finalreslist[i].ForecastValue;
        //                  //  Dr["TotalValue"] = _Finalreslist[i].TotalValue;
        //                    Dr["DurationDateTime"] = _Finalreslist[i].DurationDateTime;
        //                    Dr["SiteId"] = _Finalreslist[i].SiteId;
        //                    Dr["Duration"] = _Finalreslist[i].Duration;
        //                    Dr["IsHistory"] = _Finalreslist[i].IsHistory;


        //                    Dr["TestingArea"] = _Finalreslist[i].TestingArea;
        //                    //endcopy

        //                    //if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //                    //{
        //                    //    if (_Finalreslist[i].SiteId > 0)
        //                    //    {
        //                    //        var siteins1 = siteins.Where(b => b.InstrumentID == productusage[ki].InstrumentId && b.SiteID==_Finalreslist[i].SiteId).FirstOrDefault();
        //                    //        if (siteins1 != null)
        //                    //        {
        //                    decimal Qty = productusage[ki].Rate * _Finalreslist[i].ForecastValue;
        //                    Dr["TotalValue"] = Qty;
        //                    //c_listFresult.TotalValue = Qty;
        //                    MasterProduct p = Masterproduct.Where(b => b.ProductID == productusage[ki].ProductId).FirstOrDefault();
        //                    p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
        //                    int packSize = p.GetActiveProductPrice(_Finalreslist[i].DurationDateTime).packsize;

        //                    Dr["ProductId"] = productusage[ki].ProductId;
        //                    //   Dr["HistoricalValue"] = _Finalreslist[i].HistoricalValue;
        //                    Dr["ServiceConverted"] = true;
        //                    Dr["PackQty"] = GetNoofPackage(packSize, Qty);
        //                    Dr["PackPrice"] = GetNoofPackage(packSize, Qty) * p.GetActiveProductPrice(FP.DurationDateTime).packcost;
        //                    Dr["ProductTypeId"] =p.ProductTypeId;
        //                    Dr["ProductType"] = producttypelist.Where(b => b.TypeID == p.ProductTypeId).FirstOrDefault().TypeName;
        //                    Dr["IsForGeneralConsumable"] = true;
        //                    Dr["IsForControl"] = false;
        //                    dt.Rows.Add(Dr);
        //                    //c_listFresult.ProductId = productusage[ki].ProductId;
        //                    //c_listFresult.PackQty = GetNoofPackage(packSize, Qty);
        //                    //c_listFresult.PackPrice = c_listFresult.PackQty * p.GetActiveProductPrice(FP.DurationDateTime).packcost;

        //                    //c_listFresult.ProductTypeId = Masterproduct.Where(b => b.ProductID == productusage[ki].ProductId).FirstOrDefault().ProductTypeId;
        //                    //c_listFresult.ProductType = producttypelist.Where(b => b.TypeID == c_listFresult.ProductTypeId).FirstOrDefault().TypeName;
        //                    //c_listFresult.ServiceConverted = true;
        //                    //c_listFresult.IsForGeneralConsumable = false;
        //                    //c_listFresult.IsForControl = false;
        //                    //Finalresult.Add(c_listFresult);
        //                    //        }
        //                    //    }
        //                    //}
        //                    //else
        //                    //{
        //                    //    //if (fsite != null)
        //                    //    //{
        //                    //    //    ForecastCategoryInstrument fcins = DataRepository.GetForecastCategoryInstrumentById(pu.Instrument.Id);

        //                    //    //    if (fcins != null)
        //                    //    //    {
        //                    //    //        decimal Qty = pu.Rate * _listFresult[i].TotalValue * fcins.TestRunPercentage;
        //                    //    //        c_listFresult[i].TotalValue = Qty;
        //                    //    //        int packSize = pu.Product.GetActiveProductPrice(_listFresult[i].DurationDateTime).PackSize;
        //                    //    //        c_listFresult[i].ProductId = pu.Product.Id;
        //                    //    //        c_listFresult[i].PackQty = GetNoofPackage(packSize, Qty);
        //                    //    //        c_listFresult[i].PackPrice = c_listFresult[i].PackQty * pu.Product.GetActiveProductPrice(_listFresult[i].DurationDateTime).Price;

        //                    //    //        c_listFresult[i].ProductTypeId = pu.Product.ProductType.Id;
        //                    //    //        c_listFresult[i].ProductType = pu.Product.ProductType.TypeName;
        //                    //    //        c_listFresult[i].ServiceConverted = true;
        //                    //    //        _list_listFresult[i].Add(c_listFresult[i]);
        //                    //    //    }
        //                    //    //}
        //                    //}
        //                }
        //                ///////// 
        //                #endregion

        //                #region Control Test to Product


        //                var cproductusage = ForecastProductUsage.Where(b => b.TestId == _Finalreslist[i].TestId && b.IsForControl == true).ToList();
        //                // foreach (ForecastProductUsage pu in cproductusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
        //                for (int z = 0; z < cproductusage.Count; z++)
        //                {
                           
        //                    Dr = dt.NewRow();

        //                    Dr["CategoryId"] = _Finalreslist[i].CategoryId;
        //                    Dr["ForecastId"] = _Finalreslist[i].ForecastId;
        //                    Dr["TestId"] = _Finalreslist[i].TestId;

        //                    Dr["ForecastValue"] = _Finalreslist[i].ForecastValue;
        //                    //  Dr["TotalValue"] = _Finalreslist[i].TotalValue;
        //                    Dr["DurationDateTime"] = _Finalreslist[i].DurationDateTime;
        //                    Dr["SiteId"] = _Finalreslist[i].SiteId;
        //                    Dr["Duration"] = _Finalreslist[i].Duration;
        //                    Dr["IsHistory"] = _Finalreslist[i].IsHistory;


        //                    Dr["TestingArea"] = _Finalreslist[i].TestingArea;
        //                    //copyvalues
        //                    //c_listFresult[i].ForecastId = _listFresult[i].ForecastId;
        //                    //c_listFresult[i].TestId = _listFresult[i].TestId;
        //                    //c_listFresult[i].DurationDateTime = _listFresult[i].DurationDateTime;
        //                    //c_listFresult[i].SiteId = _listFresult[i].SiteId;
        //                    //c_listFresult[i].CategoryId = _listFresult[i].CategoryId;
        //                    //c_listFresult[i].Duration = _listFresult[i].Duration;
        //                    //c_listFresult[i].IsHistory = _listFresult[i].IsHistory;
        //                    //c_listFresult[i].TestingArea = _listFresult[i].TestingArea;
        //                    Dr["IsForControl"] = true;
                          
        //                    //endcopy

        //                    if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //                    {
        //                        if (_Finalreslist[i].SiteId > 0)
        //                        {
        //                            var siteins1 = siteins.Where(b => b.InstrumentID == cproductusage[z].InstrumentId && b.SiteID == _Finalreslist[i].SiteId).FirstOrDefault();
        //                            if (siteins1 != null)
        //                            {


        //                                decimal Qty = 0;
        //                                if (inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
        //                                {
        //                                    Qty = cproductusage[z].Rate * _Finalreslist[i].FPinDay * inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.DailyCtrlTest).FirstOrDefault();
        //                                    // c_listFresult[i].ForecastValue = fPinDay * siteins.Quantity * siteins.Instrument.DailyCtrlTest;
        //                                }
        //                                if (inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
        //                                {
        //                                    Qty = cproductusage[z].Rate * _Finalreslist[i].FPinWeek * inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
        //                                    // c_listFresult[i].ForecastValue = fPinWeek * siteins.Quantity * siteins.Instrument.WeeklyCtrlTest;
        //                                }
        //                                if (inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
        //                                {
        //                                    Qty = cproductusage[z].Rate * _Finalreslist[i].FPinMonth * inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();
        //                                    // c_listFresult[i].ForecastValue = fPinMonth * siteins.Quantity * siteins.Instrument.MonthlyCtrlTest;

        //                                }
        //                                if (inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
        //                                {
        //                                    Qty = cproductusage[z].Rate * _Finalreslist[i].FPinQuarter * inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
        //                                    //c_listFresult[i].ForecastValue = fPinQuarter * siteins.Quantity * siteins.Instrument.QuarterlyCtrlTest;
        //                                }
        //                                if (inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
        //                                {
        //                                    Qty = cproductusage[z].Rate * ((_Finalreslist[i].TotalValue * (siteins1.TestRunPercentage / 100)) / inslist.Where(s => s.InstrumentID == siteins1.InstrumentID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
        //                                    // c_listFresult[i].ForecastValue = ((_listFresult[i].TotalValue * (siteins.TestRunPercentage / 100)) / siteins.Instrument.MaxTestBeforeCtrlTest);
        //                                }


        //                                //decimal Qty = pu.Rate * _listFresult[i].TotalValue * siteins.TestRunPercentage / 100;

                                       
        //                                MasterProduct p = Masterproduct.Where(b => b.ProductID == cproductusage[z].ProductId).FirstOrDefault();
        //                                p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
        //                                int packSize = p.GetActiveProductPrice(_Finalreslist[i].DurationDateTime).packsize;
                                    
        //                                Dr["ProductId"] = cproductusage[z].ProductId;
        //                                //   Dr["HistoricalValue"] = _Finalreslist[i].HistoricalValue;
        //                                Dr["ServiceConverted"] = true;
        //                                Dr["PackQty"] = GetNoofPackage(packSize, Qty);
        //                                Dr["PackPrice"] = GetNoofPackage(packSize, Qty) * p.GetActiveProductPrice(_Finalreslist[i].DurationDateTime).packcost;
        //                                Dr["ProductTypeId"] = p.ProductTypeId;
        //                                Dr["ProductType"] = producttypelist.Where(b => b.TypeID == p.ProductTypeId).FirstOrDefault().TypeName;
        //                                Dr["IsForGeneralConsumable"] = false;
        //                                dt.Rows.Add(Dr);
                                       
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //                ///////// 
        //                #endregion

        //            }
        //            //SqlConnection sqlConnection = ConnectionManager.GetInstance().GetSqlConnection();
        //            //DataTable _listFresult[i]dt = GenericToDataTable.ConvertTo(_list_listFresult[i]);
        //            //if (sqlConnection.State != ConnectionState.Open)
        //            //{
        //            //    sqlConnection.Open();
        //            //}
        //            //using (var bulkCopy = new SqlBulkCopy(sqlConnection))
        //            //{
        //            //    bulkCopy.DestinationTableName = "dbo.ForecastedResult";
        //            //    bulkCopy.WriteToServer(_listFresult[i]dt);
        //            //}
        //        }


        //        using (SqlConnection sqlConnection = new SqlConnection(ctx.ForecastedResult..ConnectionString))
        //        {
        //            sqlConnection.Open();

        //            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.TableLock, null)
        //            {
        //                DestinationTableName = "dbo.ForecastedResult";
        //        };

        //            sqlBulkCopy.WriteToServer(dt);

        //            sqlConnection.Close();
        //        }
        //        //ctx.ForecastedResult.AddRange(Finalresult);
        //        //ctx.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
        private void ForcastSiteHistoricalData(ForecastInfo finfo)
        {
            int months;
            var forecastsite = ctx.ForecastSite.Where(b => b.ForecastInfoId == finfo.ForecastID).ToList();
            // var forecastperiod = finfo.StartDate - finfo.ForecastDate;
            months = (finfo.ForecastDate.Value.Year - finfo.StartDate.Year) * 12;
            months -= finfo.StartDate.Month;
            months += finfo.ForecastDate.Value.Month;
         //   var hisdata = ctx.ForecastSite.Where(b=>b.ForecastInfoId == finfo.ForecastID).ToList();
            if (months == 0)

            {
                months = 1;

            }
            // return months < 0 ? 0 : months + 1;
            //  forecastperiod.Value.TotalDays
            string Period = "";
            string apiparam = "";
            int countryid = ctx.User.Find(finfo.UserId).CountryId.Value;
            Period = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault().Period;
            if (Period == "Monthly")
            {
                apiparam = "m";
            }
            else if (Period == "Yearly")
            {
                apiparam = "y";
            }
            else
            {
                apiparam = "m";
            }

            string xmlstring = "";
            MLModel MLForecast = new MLModel();
            //  foreach (ForecastSite s in forecastsite)
            for (int s = 0; s < forecastsite.Count; s++)

            {
                if (finfo.Methodology == "CONSUMPTION")
                {
                    IList<int> products = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == forecastsite[s].Id).GroupBy(x => x.ProductID).Select(x => x.Key).ToList();


                    //   foreach (int p in products)
                    for (int e = 0; e < products.Count; e++)

                    {

                        DataTable InputDs = new DataTable();
                        InputDs.Columns.Add("X");
                        InputDs.Columns.Add("Y");
                        if (products[e] != 0)
                        {


                            List<ForecastSiteProduct> fsiteProduct = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == forecastsite[s].Id && b.ProductID == products[e]).OrderBy(x => x.DurationDateTime).ToList();
                            for (int f = 0; f < fsiteProduct.Count; f++)
                            {
                                DataRow Dr = InputDs.NewRow();
                                Dr["X"] = fsiteProduct[f].DurationDateTime;
                                Dr["Y"] = fsiteProduct[f].Adjusted;
                                InputDs.Rows.Add(Dr);
                            }
                            HttpWebRequest request = default(HttpWebRequest);
                            HttpWebResponse response = null;
                            StreamReader reader = default(StreamReader);
                            ////  http://34.69.151.1:8000/api/forecasts/?productId=0&siteId=649&forecastPeriod=7&frequency=m
                            /////get global region 
                            string url = mainurl + "forecasts/?productId=" + products[e] + "&siteId=" + forecastsite[s].SiteId + "&forecastPeriod=" + months + "&frequency=" + apiparam;
                            request = (HttpWebRequest)WebRequest.Create(url);

                            response = (HttpWebResponse)request.GetResponse();
                            reader = new StreamReader(response.GetResponseStream());
                            xmlstring = reader.ReadToEnd();

                            MLForecast = JsonConvert.DeserializeObject<MLModel>(xmlstring);

                            //gets last Input date
                            lastEntryDate = fsiteProduct[fsiteProduct.Count - 1].DurationDateTime.Value;
                       
                            ReadDatasetservice(finfo, forecastsite[s].SiteId, 0, products[e], 0, MLForecast, lastEntryDate, InputDs, Period);
                            //     ReadDataset(finfo, forecastsite[s].SiteId, 0, products[e], 0, MLForecast, lastEntryDate, InputDs, Period);

                        }
                    }
                }
                else if (finfo.Methodology == "SERVICE STATSTICS")
                {
                    IList<int> tests = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == forecastsite[s].Id).GroupBy(x => x.TestID).Select(f => f.Key).ToList();

                    for (int k = 0; k < tests.Count; k++)
                    {

                        //}
                        //foreach (int p in tests)
                        //{

                        List<ForecastSiteTest> fsiteTest = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == forecastsite[s].Id && b.TestID == tests[k]).OrderBy(f => f.DurationDateTime).ToList();


                        DataTable InputDs = new DataTable();
                        InputDs.Columns.Add("X");
                        InputDs.Columns.Add("Y");
                        for (int f = 0; f < fsiteTest.Count; f++)
                        {
                            DataRow Dr = InputDs.NewRow();
                            Dr["X"] = fsiteTest[f].DurationDateTime;
                            Dr["Y"] = fsiteTest[f].Adjusted;
                            InputDs.Rows.Add(Dr);
                        }

                        HttpWebRequest request = default(HttpWebRequest);
                        HttpWebResponse response = null;
                        StreamReader reader = default(StreamReader);

                        /////get global region 
                        string url = mainurl + "forecasts/?testId=" + tests[k] + "&siteId=" + forecastsite[s].SiteId + "&forecastPeriod=" + months + "&frequency=" + apiparam;
                        request = (HttpWebRequest)WebRequest.Create(url);

                        response = (HttpWebResponse)request.GetResponse();
                        reader = new StreamReader(response.GetResponseStream());
                        xmlstring = reader.ReadToEnd();

                        MLForecast = JsonConvert.DeserializeObject<MLModel>(xmlstring);

                        lastEntryDate = fsiteTest[fsiteTest.Count - 1].DurationDateTime.Value;

                        //    DataTable ds = Calculateforecastmultiplemethod(InputDs, finfo);

                        //CalculateChart1(finfo.Extension); m,
                        //DataTable ds = chart1.DataManipulator.ExportSeriesValues("Forecasting");
                        ReadDatasetservice(finfo, forecastsite[s].SiteId, 0, 0, tests[k], MLForecast, lastEntryDate, InputDs, Period);

                    }
                }
            }
        }


<<<<<<< HEAD
        //private void ForcastSiteHistoricalData_new(ForecastInfo finfo)
        //{
        //    int months;
        //    var forecastsite = ctx.ForecastSite.Where(b => b.ForecastInfoId == finfo.ForecastID).ToList();
        //    // var forecastperiod = finfo.StartDate - finfo.ForecastDate;
        //    months = (finfo.ForecastDate.Value.Year - finfo.StartDate.Year) * 12;
        //    months -= finfo.StartDate.Month;
        //    months += finfo.ForecastDate.Value.Month;
        //    //   var hisdata = ctx.ForecastSite.Where(b=>b.ForecastInfoId == finfo.ForecastID).ToList();
        //    if (months == 0)

        //    {
        //        months = 1;

        //    }
        //    // return months < 0 ? 0 : months + 1;
        //    //  forecastperiod.Value.TotalDays
        //    string Period = "";
        //    string apiparam = "";
        //    int countryid = ctx.User.Find(finfo.UserId).CountryId.Value;
        //    Period = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault().Period;
        //    if (Period == "Monthly")
        //    {
        //        apiparam = "m";
        //    }
        //    else if (Period == "Yearly")
        //    {
        //        apiparam = "y";
        //    }
        //    else
        //    {
        //        apiparam = "m";
        //    }

        //    string xmlstring = "";
        //    var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x =>

        //      x.Id

        // ).FirstOrDefault();
        //    List<MLModel> MLForecast = new List<MLModel>();
        //    var sites = ctx.Site.Where(b => b.UserId == finfo.UserId || b.CountryId == countryid).ToList();
        //    List<MasterProduct> MLProduct = new List<MasterProduct>();
        //    List<ProductType> MLProducttype = new List<ProductType>();
        //    List<Test> Tests = new List<Test>();
        //    List<TestingArea> TestingAreas = new List<TestingArea>();
        //    if (finfo.Methodology == "CONSUMPTION")
        //    {
        //        MLProduct=ctx.MasterProduct.Where(b=>b.UserId==finfo.UserId || b.UserId== adminuserid).ToList();
        //        MLProducttype=ctx.ProductType.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
        //    }
        //    else
        //    {
        //        Tests=ctx.Test.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
        //        TestingAreas = ctx.TestingArea.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
        //    }



        //        //  foreach (ForecastSite s in forecastsite)
        //        for (int s = 0; s < forecastsite.Count; s++)

        //     {
        //        if (finfo.Methodology == "CONSUMPTION")
        //        {
        //            IList<int> products = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == forecastsite[s].Id).GroupBy(x => x.ProductID).Select(x => x.Key).ToList();
        //            string productids = "";
        //            if (products.Count > 0)
        //            {
        //                for (int e = 0; e < products.Count; e++)

        //                {
        //                    productids = productids + "productId=" + products[e] + "&";
        //                }

        //             //   productids.TrimEnd('&');
        //                HttpWebRequest request = default(HttpWebRequest);
        //                HttpWebResponse response = null;
        //                StreamReader reader = default(StreamReader);
        //                ////  http://34.69.151.1:8000/api/forecasts/?productId=0&siteId=649&forecastPeriod=7&frequency=m
        //                /////get global region 
        //                ///

        //                string url = mainurl + "forecasts/?" + productids.TrimEnd('&') + "&siteId=" + forecastsite[s].SiteId + "&forecastPeriod=" + months + "&frequency=" + apiparam;
        //                request = (HttpWebRequest)WebRequest.Create(url);
        //                request.Timeout = 200000;
        //                response = (HttpWebResponse)request.GetResponse();
        //                reader = new StreamReader(response.GetResponseStream());
        //                xmlstring = reader.ReadToEnd();

        //                MLForecast = JsonConvert.DeserializeObject<List<MLModel>>(xmlstring);
        //                ReadDatasetconsumptionnew(finfo, MLForecast, Period, forecastsite[s].Id,sites,MLProduct,MLProducttype,Tests, TestingAreas);
        //            }
        //            //ReadDatasetservice(finfo, forecastsite[s].SiteId, 0, products[e], 0, MLForecast, lastEntryDate, InputDs, Period);
        //            ////   foreach (int p in products)
        //            //for (int e = 0; e < products.Count; e++)

        //            //{

        //            //    DataTable InputDs = new DataTable();
        //            //    InputDs.Columns.Add("X");
        //            //    InputDs.Columns.Add("Y");
        //            //    if (products[e] != 0)
        //            //    {


        //            //        List<ForecastSiteProduct> fsiteProduct = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == forecastsite[s].Id && b.ProductID == products[e]).OrderBy(x => x.DurationDateTime).ToList();
        //            //        for (int f = 0; f < fsiteProduct.Count; f++)
        //            //        {
        //            //            DataRow Dr = InputDs.NewRow();
        //            //            Dr["X"] = fsiteProduct[f].DurationDateTime;
        //            //            Dr["Y"] = fsiteProduct[f].Adjusted;
        //            //            InputDs.Rows.Add(Dr);
        //            //        }
        //            //        HttpWebRequest request = default(HttpWebRequest);
        //            //        HttpWebResponse response = null;
        //            //        StreamReader reader = default(StreamReader);
        //            //        ////  http://34.69.151.1:8000/api/forecasts/?productId=0&siteId=649&forecastPeriod=7&frequency=m
        //            //        /////get global region 
        //            //        string url = mainurl + "forecasts/?productId=" + products[e] + "&siteId=" + forecastsite[s].SiteId + "&forecastPeriod=" + months + "&frequency=" + apiparam;
        //            //        request = (HttpWebRequest)WebRequest.Create(url);

        //            //        response = (HttpWebResponse)request.GetResponse();
        //            //        reader = new StreamReader(response.GetResponseStream());
        //            //        xmlstring = reader.ReadToEnd();

        //            //        MLForecast = JsonConvert.DeserializeObject<MLModel>(xmlstring);

        //            //        //gets last Input date
        //            //        lastEntryDate = fsiteProduct[fsiteProduct.Count - 1].DurationDateTime.Value;

        //            //        ReadDatasetservice(finfo, forecastsite[s].SiteId, 0, products[e], 0, MLForecast, lastEntryDate, InputDs, Period);
        //            //        //     ReadDataset(finfo, forecastsite[s].SiteId, 0, products[e], 0, MLForecast, lastEntryDate, InputDs, Period);

        //            //    }
        //            //}
        //        }
        //        else if (finfo.Methodology == "SERVICE STATSTICS")
        //        {
        //            IList<int> tests = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == forecastsite[s].Id).GroupBy(x => x.TestID).Select(f => f.Key).ToList();
        //            string testids = "";
        //            if (tests.Count > 0)
        //            {


        //                for (int e = 0; e < tests.Count; e++)

        //                {
        //                    testids = testids + "testId=" + tests[e] + "&";
        //                }
        //            //    testids.TrimEnd('&');
        //                HttpWebRequest request = default(HttpWebRequest);
        //                HttpWebResponse response = null;
        //                StreamReader reader = default(StreamReader);
        //                ////  http://34.69.151.1:8000/api/forecasts/?productId=0&siteId=649&forecastPeriod=7&frequency=m
        //                /////get global region 
        //                string url = mainurl + "forecasts/?" + testids.TrimEnd('&') + "&siteId=" + forecastsite[s].SiteId + "&forecastPeriod=" + months + "&frequency=" + apiparam;
        //                request = (HttpWebRequest)WebRequest.Create(url);
        //                request.Timeout = 200000;
        //                response = (HttpWebResponse)request.GetResponse();
        //                reader = new StreamReader(response.GetResponseStream());
        //                xmlstring = reader.ReadToEnd();

        //                MLForecast = JsonConvert.DeserializeObject<List<MLModel>>(xmlstring);
        //                ReadDatasetconsumptionnew(finfo, MLForecast, Period, forecastsite[s].Id, sites, MLProduct, MLProducttype, Tests, TestingAreas);
        //            }
        //            //for (int k = 0; k < tests.Count; k++)
        //            //{

        //            //    //}
        //            //    //foreach (int p in tests)
        //            //    //{

        //            //    List<ForecastSiteTest> fsiteTest = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == forecastsite[s].Id && b.TestID == tests[k]).OrderBy(f => f.DurationDateTime).ToList();


        //            //    DataTable InputDs = new DataTable();
        //            //    InputDs.Columns.Add("X");
        //            //    InputDs.Columns.Add("Y");
        //            //    for (int f = 0; f < fsiteTest.Count; f++)
        //            //    {
        //            //        DataRow Dr = InputDs.NewRow();
        //            //        Dr["X"] = fsiteTest[f].DurationDateTime;
        //            //        Dr["Y"] = fsiteTest[f].Adjusted;
        //            //        InputDs.Rows.Add(Dr);
        //            //    }

        //            //    HttpWebRequest request = default(HttpWebRequest);
        //            //    HttpWebResponse response = null;
        //            //    StreamReader reader = default(StreamReader);

        //            //    /////get global region 
        //            //    string url = mainurl + "forecasts/?testId=" + tests[k] + "&siteId=" + forecastsite[s].SiteId + "&forecastPeriod=" + months + "&frequency=" + apiparam;
        //            //    request = (HttpWebRequest)WebRequest.Create(url);

        //            //    response = (HttpWebResponse)request.GetResponse();
        //            //    reader = new StreamReader(response.GetResponseStream());
        //            //    xmlstring = reader.ReadToEnd();

        //            //    MLForecast = JsonConvert.DeserializeObject<MLModel>(xmlstring);

        //            //    lastEntryDate = fsiteTest[fsiteTest.Count - 1].DurationDateTime.Value;

        //            //    //    DataTable ds = Calculateforecastmultiplemethod(InputDs, finfo);

        //            //    //CalculateChart1(finfo.Extension); m,
        //            //    //DataTable ds = chart1.DataManipulator.ExportSeriesValues("Forecasting");
        //            //    ReadDatasetservice(finfo, forecastsite[s].SiteId, 0, 0, tests[k], MLForecast, lastEntryDate, InputDs, Period);

        //            //}
        //        }
        //    }
        //}
=======
        async private void ForcastSiteHistoricalData_new(ForecastInfo finfo)
        {
            try
            {
                string postURL = "http://34.69.151.1:8000/api/forecasts/";
                int months;
                var forecastsite = ctx.ForecastSite.Where(b => b.ForecastInfoId == finfo.ForecastID).ToList();
                // var forecastperiod = finfo.StartDate - finfo.ForecastDate;
                months = (finfo.ForecastDate.Value.Year - finfo.StartDate.Year) * 12;
                months -= finfo.StartDate.Month;
                months += finfo.ForecastDate.Value.Month;
                //   var hisdata = ctx.ForecastSite.Where(b=>b.ForecastInfoId == finfo.ForecastID).ToList();
                if (months == 0)

                {
                    months = 1;

                }
                // return months < 0 ? 0 : months + 1;
                //  forecastperiod.Value.TotalDays
                string Period = "";
                string apiparam = "";
                int countryid = ctx.User.Find(finfo.UserId).CountryId.Value;
                Period = ctx.Country.Where(b => b.Id == countryid).FirstOrDefault().Period;
                if (Period == "Monthly")
                {
                    apiparam = "m";
                }
                else if (Period == "Yearly")
                {
                    apiparam = "y";
                }
                else
                {
                    apiparam = "m";
                }

                string xmlstring = "";
                var adminuserid = ctx.User.Where(b => b.Role == "admin").Select(x => x.Id).FirstOrDefault();
                List<MLModel> MLForecast = new List<MLModel>();
                var sites = ctx.Site.Where(b => b.UserId == finfo.UserId || b.CountryId == countryid).ToList();
                List<MasterProduct> MLProduct = new List<MasterProduct>();
                List<ProductType> MLProducttype = new List<ProductType>();
                List<Test> Tests = new List<Test>();
                List<TestingArea> TestingAreas = new List<TestingArea>();
                if (finfo.Methodology == "CONSUMPTION")
                {
                    MLProduct = ctx.MasterProduct.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
                    MLProducttype = ctx.ProductType.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
                }
                else
                {
                    Tests = ctx.Test.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
                    TestingAreas = ctx.TestingArea.Where(b => b.UserId == finfo.UserId || b.UserId == adminuserid).ToList();
                }

                ForecastCaculateModel fcm = new ForecastCaculateModel();
                fcm.forecastPeriod = months;
                fcm.frequency = apiparam;
                fcm.productSite = new List<ProductSite>();

                for (int s = 0; s < forecastsite.Count; s++)
                {
                    ProductSite ps = new ProductSite();
                    ps.productIds = new List<int>();
                    if (finfo.Methodology == "CONSUMPTION")
                    {
                        IList<int> products = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == forecastsite[s].Id).GroupBy(x => x.ProductID).Select(x => x.Key).ToList();

                        if (products.Count > 0)
                        {
                            for (int e = 0; e < products.Count; e++)
                            {
                                ps.productIds.Add(products[e]);
                            }
                        }
                        ps.siteId = forecastsite[s].SiteId;
                        fcm.productSite.Add(ps);

                    }
                    else if (finfo.Methodology == "SERVICE STATSTICS")
                    {
                        IList<int> tests = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == forecastsite[s].Id).GroupBy(x => x.TestID).Select(f => f.Key).ToList();

                        string testids = "";
                        if (tests.Count > 0)
                        {
                            for (int e = 0; e < tests.Count; e++)

                            {
                                testids = testids + "testId=" + tests[e] + "&";
                            }

                            //HttpWebRequest request = default(HttpWebRequest);
                            //HttpWebResponse response = null;
                            //StreamReader reader = default(StreamReader);
                            //////  http://34.69.151.1:8000/api/forecasts/?productId=0&siteId=649&forecastPeriod=7&frequency=m
                            ///////get global region 
                            //string url = mainurl + "forecasts/?" + testids + "&siteId=" + forecastsite[s].SiteId + "&forecastPeriod=" + months + "&frequency=" + apiparam;
                            //request = (HttpWebRequest)WebRequest.Create(url);
                            //request.Timeout = 200000;
                            //response = (HttpWebResponse)request.GetResponse();
                            //reader = new StreamReader(response.GetResponseStream());
                            //xmlstring = reader.ReadToEnd();

                            //MLForecast = JsonConvert.DeserializeObject<List<MLModel>>(xmlstring);
                            //ReadDatasetconsumptionnew(finfo, MLForecast, Period, forecastsite[s].Id, sites, MLProduct, MLProducttype, Tests, TestingAreas);
                        }

                    }
                }


                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(30);
                var obj = JsonConvert.SerializeObject(fcm);
                var content = new StringContent(obj, Encoding.UTF8, "application/json");
                var result = await httpClient.PostAsync(postURL, content);

                string responseBody = await result.Content.ReadAsStringAsync();
                List<MLResponseModel> MLResponseForecast = new List<MLResponseModel>();
                MLResponseForecast = JsonConvert.DeserializeObject<List<MLResponseModel>>(responseBody);
                for (int s = 0; s < forecastsite.Count; s++)
                {
                    ReadDatasetconsumptionnew(finfo, MLResponseForecast, Period, forecastsite[s].Id, sites, MLProduct, MLProducttype, Tests, TestingAreas);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        
        }
>>>>>>> Devops
        private void ReadDataset(ForecastInfo finfo, int siteid, int catid, int proid, int testid, MLModel ds, DateTime lastDate, DataTable inputDs, string Period)
        {
            int period = 0;

            foreach (MLModelResult row in ds.Forecasts)
            {
                DateTime ddate = Convert.ToDateTime(row.Datetime);

                //if (ddate.Day > 29)
                //{
                //    if (ddate.Month == 12)
                //        ddate = new DateTime(ddate.Year + 1, 1, 1);
                //    ddate = new DateTime(ddate.Year, ddate.Month+1, 1);
                //}

                ForecastedResult fresult = new ForecastedResult();
                fresult.ForecastId = finfo.ForecastID;
                fresult.SiteId = siteid;
                fresult.CategoryId = catid;
                fresult.TestId = testid;
                fresult.ProductId = proid;
                fresult.DurationDateTime = Utility.Utility.CorrectDateTime(ddate.Date);//ddate.Date;

                if (Convert.ToDecimal(row.Forecast) < 0)
                    fresult.ForecastValue = 0;
                else
                    fresult.ForecastValue = Convert.ToDecimal(row.Forecast);


                if (ddate > lastDate)
                {
                    fresult.IsHistory = false;
                    fresult.HistoricalValue = 0;
                }
                else
                {
                    DataRow rowinput = inputDs.Rows[period];
                    fresult.HistoricalValue = Convert.ToDecimal(rowinput[1]);
                    fresult.IsHistory = true;
                }

                if (Period == "Monthly" || Period == "Bimonthly")
                    fresult.Duration = String.Format("{0}-{1}", Utility.Utility.Months[fresult.DurationDateTime.Month - 1], fresult.DurationDateTime.Year);
                else if (Period == "Quarterly")
                    fresult.Duration = String.Format("Qua{0}-{1}", Utility.Utility.GetQuarter(fresult.DurationDateTime), fresult.DurationDateTime.Year);
                else
                    fresult.Duration = String.Format("{0}", fresult.DurationDateTime.Year);

                //fresult.TotalValue = fresult.ForecastValue + (fresult.ForecastValue * (Convert.ToInt32(param1[1]) / 100));
                //fresult.TotalValue += prevValue * (Convert.ToInt32(param1[2]) / 100);
                //prevValue = fresult.TotalValue;

                fresult.ServiceConverted = false;

                Site fsite = null;
                if (fresult.SiteId > 0)
                    fsite = ctx.Site.Find(fresult.SiteId);

                ForecastCategory fc = null;
                if (fresult.CategoryId > 0)
                    fc = ctx.ForecastCategory.Where(b => b.CategoryId == fresult.CategoryId).FirstOrDefault();


                int workingdays;
                #region Forecast Period Conversion
                if (finfo.DataUsage == "DATA_USAGE3")
                {
                    int[] sitesid = ctx.ForecastCategorySite.Where(c => c.CategoryID == catid).Select(s => s.SiteID).ToArray();
                    workingdays = Convert.ToInt32(ctx.Site.Where(b => sitesid.Contains(b.SiteID)).Average(s => s.WorkingDays));


                }
                else
                {
                    workingdays = fsite.WorkingDays;

                }
                string fprd = Period;
                decimal fPinDay = 0, fPinWeek = 0, fPinMonth = 0, fPinQuarter = 0, fPinYear = 0;
                if (fprd == "Yearly")
                {
                    fPinDay = workingdays * 12;
                    fPinMonth = 12;
                    fPinWeek = (workingdays / 4) * 12;
                    fPinQuarter = 4;
                    fPinYear = 1;
                }
                if (fprd == "Quarterly")
                {
                    fPinDay = workingdays * 3;
                    fPinMonth = 3;
                    fPinWeek = (workingdays / 4) * 3;
                    fPinQuarter = 1;
                    fPinYear = 1 / 4;
                }
                if (fprd == "Bimonthly")
                {
                    fPinDay = workingdays * 2;
                    fPinMonth = 2;
                    fPinWeek = (workingdays / 4) * 2;
                    fPinQuarter = 2 / 3;
                    fPinYear = 1 / 6;
                }
                if (fprd == "Monthly")
                {
                    fPinDay = workingdays;
                    fPinMonth = 1;
                    fPinWeek = (workingdays / 4);
                    fPinQuarter = 1 / 3;
                    fPinYear = 1 / 12;
                }
                #endregion




                //get TestingArea
                if (fresult.TestId > 0)
                {
                    Test test = ctx.Test.Find(fresult.TestId);
                    fresult.TestingArea = ctx.TestingArea.Find(test.TestingAreaID).AreaName;

                }

                int Nopack = int.Parse(decimal.Round(fresult.ForecastValue, 0).ToString());

                if (Nopack < fresult.ForecastValue)
                    Nopack = Nopack + 1;
                if (Nopack == 0)
                    Nopack = 0;
                fresult.TotalValue = Nopack;
                //get packQty
                if (fresult.ProductId > 0)
                {
                    MasterProduct p = ctx.MasterProduct.Find(fresult.ProductId);
                    p._productPrices = ctx.ProductPrice.Where(b => b.ProductId == p.ProductID).ToList();
                    //converting quantity to packsize commented out
                    //int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).PackSize;
                    //fresult.PackQty = GetNoofPackage(packSize, fresult.TotalValue);

                    //rounding forecasted pack quantity

                    fresult.PackQty = Nopack;

                    fresult.PackPrice = fresult.PackQty * p.GetActiveProductPrice(fresult.DurationDateTime).packcost;

                    fresult.ProductTypeId = p.ProductTypeId;
                    fresult.ProductType = ctx.ProductType.Find(p.ProductTypeId).TypeName;
                    _listFresult.Add(fresult);

                }



                //test to product
                if (fresult.TestId > 0)
                {
                    Test test = ctx.Test.Find(fresult.TestId);

                    #region Forecast General Consumables


                    IList<ForecastedResult> _consumablesDailyFlist = new List<ForecastedResult>();


                    foreach (ConsumableUsage cusage in GetAllConsumableUsageByTestId(fresult.TestId))//DataRepository.GetConsumableUsageByTestId(fresult.TestId))
                    {
                        //
                        ForecastedResult consumableFresult = new ForecastedResult();
                        //copyvalues
                        consumableFresult.ForecastId = fresult.ForecastId;
                        consumableFresult.TestId = fresult.TestId;
                        consumableFresult.DurationDateTime = fresult.DurationDateTime;
                        consumableFresult.SiteId = fresult.SiteId;
                        consumableFresult.CategoryId = fresult.CategoryId;
                        consumableFresult.Duration = fresult.Duration;
                        consumableFresult.IsHistory = fresult.IsHistory;
                        consumableFresult.TestingArea = fresult.TestingArea;
                        consumableFresult.ForecastValue = fresult.ForecastValue;
                        consumableFresult.TotalValue = fresult.TotalValue;
                        //endcopy
                        decimal Qty = 0;


                        if (cusage.PerInstrument)
                        {
                            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                            {
                                if (fsite != null)
                                {
                                    SiteInstrument siteins = ctx.siteinstrument.Where(b => b.InstrumentID == cusage.InstrumentId && b.SiteID == fsite.SiteID).FirstOrDefault();
                                    if (siteins != null)
                                    {
                                        if (cusage.Period == "Daily")
                                        {
                                            Qty = cusage.UsageRate * fPinDay;
                                        }
                                        if (cusage.Period == "Weekly")
                                        {
                                            Qty = cusage.UsageRate * fPinWeek;
                                        }
                                        if (cusage.Period == "Monthly")
                                        {
                                            Qty = cusage.UsageRate * fPinMonth;
                                        }
                                        if (cusage.Period == "Quarterly")
                                        {
                                            Qty = cusage.UsageRate * fPinQuarter;
                                        }
                                        if (cusage.Period == "Yearly")
                                        {
                                            Qty = cusage.UsageRate * fPinYear;
                                        }
                                        Qty = Qty * siteins.Quantity;
                                    }
                                }
                            }
                        }
                        if (cusage.PerPeriod)
                        {
                            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                            {
                                if (fsite != null)
                                {
                                    if (cusage.Period == "Daily")
                                    {
                                        Qty = cusage.UsageRate * fPinDay;
                                    }
                                    if (cusage.Period == "Weekly")
                                    {
                                        Qty = cusage.UsageRate * fPinWeek;
                                    }
                                    if (cusage.Period == "Monthly")
                                    {
                                        Qty = cusage.UsageRate * fPinMonth;
                                    }
                                    if (cusage.Period == "Quarterly")
                                    {
                                        Qty = cusage.UsageRate * fPinQuarter;
                                    }
                                    if (cusage.Period == "Yearly")
                                    {
                                        Qty = cusage.UsageRate * fPinYear;
                                    }
                                }
                            }
                        }
                        if (cusage.PerTest)
                        {
                            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                            {
                                if (fsite != null)
                                {
                                    Qty = cusage.UsageRate * (fresult.TotalValue / cusage.NoOfTest);
                                }
                            }
                        }

                        consumableFresult.TotalValue = Qty;
                        MasterProduct MP = new MasterProduct();
                        MP = ctx.MasterProduct.Find(cusage.ProductId);
                        MP._productPrices = ctx.ProductPrice.Where(b => b.ProductId == MP.ProductID).ToList();
                        int packSize = MP.GetActiveProductPrice(fresult.DurationDateTime).packsize;
                        consumableFresult.ProductId = cusage.ProductId;
                        consumableFresult.PackQty = GetNoofPackage(packSize, Qty);
                        consumableFresult.PackPrice = consumableFresult.PackQty * MP.GetActiveProductPrice(fresult.DurationDateTime).packcost;
                        consumableFresult.ProductTypeId = MP.ProductTypeId;
                        consumableFresult.ProductType = ctx.ProductType.Find(MP.ProductTypeId).TypeName;
                        consumableFresult.IsForGeneralConsumable = true;
                        consumableFresult.ServiceConverted = true;
                        _listFresult.Add(consumableFresult);


                    }



                    #endregion

                    #region Forecast Control Test

                    if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                    {
                        if (fsite != null)
                        {
                            SiteInstrument siteins = ctx.siteinstrument.Join(ctx.Instrument, b => b.InstrumentID, c => c.InstrumentID, (b, c) => new { b, c }).Where(x => x.c.testingArea.TestingAreaID == test.TestingAreaID && x.b.SiteID == fsite.SiteID).GroupBy(s => s.c.testingArea.TestingAreaID)
                                .Select(s => new SiteInstrument
                                {

                                    ID = s.Max(x => x.b.ID),
                                    InstrumentID = s.Max(x => x.b.InstrumentID)
                                }).FirstOrDefault();
                            //fsite.GetSiteInstrumentByTA(test.TestingArea.Id);
                            if (siteins != null)
                            {
                                if (ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
                                {
                                    fresult.ControlTest = fPinDay * siteins.Quantity * ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.DailyCtrlTest).FirstOrDefault();
                                }
                                if (ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
                                {
                                    fresult.ControlTest = fPinWeek * siteins.Quantity * ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
                                }
                                if (ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
                                {
                                    fresult.ControlTest = fPinMonth * siteins.Quantity * ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();

                                }
                                if (ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
                                {
                                    fresult.ControlTest = fPinQuarter * siteins.Quantity * ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
                                }
                                if (ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
                                {
                                    fresult.ControlTest = ((fresult.TotalValue * (siteins.TestRunPercentage / 100)) / ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
                                }
                            }
                        }
                    }

                    #endregion

                    #region Test Test to Product
                    var productusage = ctx.ProductUsage.Where(b => b.TestId == test.TestID && b.IsForControl == false).ToList();
                    foreach (ProductUsage pu in productusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
                    {
                        ForecastedResult cfresult = new ForecastedResult();
                        //copyvalues
                        cfresult.ForecastId = fresult.ForecastId;
                        cfresult.TestId = fresult.TestId;
                        cfresult.DurationDateTime = fresult.DurationDateTime;
                        cfresult.SiteId = fresult.SiteId;
                        cfresult.CategoryId = fresult.CategoryId;
                        cfresult.Duration = fresult.Duration;
                        cfresult.IsHistory = fresult.IsHistory;
                        cfresult.TestingArea = fresult.TestingArea;
                        //endcopy

                        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                        {
                            if (fsite != null)
                            {
                                SiteInstrument siteins = ctx.siteinstrument.Where(b => b.InstrumentID == pu.InstrumentId && b.SiteID == fsite.SiteID).FirstOrDefault(); if (siteins != null)
                                {
                                    decimal Qty = pu.Rate * fresult.TotalValue * siteins.TestRunPercentage / 100;
                                    cfresult.TotalValue = Qty;
                                    MasterProduct p = ctx.MasterProduct.Find(pu.ProductId);
                                    p._productPrices = ctx.ProductPrice.Where(b => b.ProductId == p.ProductID).ToList();
                                    int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).packsize;
                                    cfresult.ProductId = pu.ProductId;
                                    cfresult.PackQty = GetNoofPackage(packSize, Qty);
                                    cfresult.PackPrice = cfresult.PackQty * p.GetActiveProductPrice(fresult.DurationDateTime).packcost;

                                    cfresult.ProductTypeId = ctx.MasterProduct.Find(pu.ProductId).ProductTypeId;
                                    cfresult.ProductType = ctx.ProductType.Find(ctx.MasterProduct.Find(pu.ProductId).ProductTypeId).TypeName;
                                    cfresult.ServiceConverted = true;
                                    _listFresult.Add(cfresult);
                                }
                            }
                        }
                        else
                        {
                            //if (fsite != null)
                            //{
                            //    ForecastCategoryInstrument fcins = DataRepository.GetForecastCategoryInstrumentById(pu.Instrument.Id);

                            //    if (fcins != null)
                            //    {
                            //        decimal Qty = pu.Rate * fresult.TotalValue * fcins.TestRunPercentage;
                            //        cfresult.TotalValue = Qty;
                            //        int packSize = pu.Product.GetActiveProductPrice(fresult.DurationDateTime).PackSize;
                            //        cfresult.ProductId = pu.Product.Id;
                            //        cfresult.PackQty = GetNoofPackage(packSize, Qty);
                            //        cfresult.PackPrice = cfresult.PackQty * pu.Product.GetActiveProductPrice(fresult.DurationDateTime).Price;

                            //        cfresult.ProductTypeId = pu.Product.ProductType.Id;
                            //        cfresult.ProductType = pu.Product.ProductType.TypeName;
                            //        cfresult.ServiceConverted = true;
                            //        _listFresult.Add(cfresult);
                            //    }
                            //}
                        }
                    }
                    ///////// 
                    #endregion

                    #region Control Test to Product



                    foreach (ProductUsage pu in test.GetProductUsageByType(true)) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
                    {
                        ForecastedResult cfresult = new ForecastedResult();
                        //copyvalues
                        cfresult.ForecastId = fresult.ForecastId;
                        cfresult.TestId = fresult.TestId;
                        cfresult.DurationDateTime = fresult.DurationDateTime;
                        cfresult.SiteId = fresult.SiteId;
                        cfresult.CategoryId = fresult.CategoryId;
                        cfresult.Duration = fresult.Duration;
                        cfresult.IsHistory = fresult.IsHistory;
                        cfresult.TestingArea = fresult.TestingArea;

                        cfresult.IsForControl = true;
                        //endcopy

                        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                        {
                            if (fsite != null)
                            {
                                SiteInstrument siteins = ctx.siteinstrument.Where(b => b.InstrumentID == pu.InstrumentId && b.SiteID == fsite.SiteID).FirstOrDefault();
                                if (siteins != null)
                                {


                                    decimal Qty = 0;
                                    if (ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
                                    {
                                        Qty = pu.Rate * fPinDay * siteins.Quantity * ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.DailyCtrlTest).FirstOrDefault();
                                        // cfresult.ForecastValue = fPinDay * siteins.Quantity * siteins.Instrument.DailyCtrlTest;
                                    }
                                    if (ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
                                    {
                                        Qty = pu.Rate * fPinWeek * siteins.Quantity * ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
                                        // cfresult.ForecastValue = fPinWeek * siteins.Quantity * siteins.Instrument.WeeklyCtrlTest;
                                    }
                                    if (ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
                                    {
                                        Qty = pu.Rate * fPinMonth * siteins.Quantity * ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();
                                        // cfresult.ForecastValue = fPinMonth * siteins.Quantity * siteins.Instrument.MonthlyCtrlTest;

                                    }
                                    if (ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
                                    {
                                        Qty = pu.Rate * fPinQuarter * siteins.Quantity * ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
                                        //cfresult.ForecastValue = fPinQuarter * siteins.Quantity * siteins.Instrument.QuarterlyCtrlTest;
                                    }
                                    if (ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
                                    {
                                        Qty = pu.Rate * ((fresult.TotalValue * (siteins.TestRunPercentage / 100)) / ctx.Instrument.Where(s => s.InstrumentID == siteins.InstrumentID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
                                        // cfresult.ForecastValue = ((fresult.TotalValue * (siteins.TestRunPercentage / 100)) / siteins.Instrument.MaxTestBeforeCtrlTest);
                                    }


                                    //decimal Qty = pu.Rate * fresult.TotalValue * siteins.TestRunPercentage / 100;

                                    cfresult.TotalValue = Qty;
                                    MasterProduct p = ctx.MasterProduct.Find(pu.ProductId);
                                    p._productPrices = ctx.ProductPrice.Where(b => b.ProductId == p.ProductID).ToList();
                                    int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).packsize;
                                    cfresult.ProductId = pu.ProductId;
                                    cfresult.PackQty = GetNoofPackage(packSize, Qty);
                                    cfresult.PackPrice = cfresult.PackQty * p.GetActiveProductPrice(fresult.DurationDateTime).packcost;

                                    cfresult.ProductTypeId = ctx.MasterProduct.Find(pu.ProductId).ProductTypeId;
                                    cfresult.ProductType = ctx.ProductType.Find(ctx.MasterProduct.Find(pu.ProductId).ProductTypeId).TypeName;
                                    cfresult.ServiceConverted = true;
                                    _listFresult.Add(cfresult);
                                }
                            }
                        }
                        else
                        {
                            //if (fsite != null)
                            //{
                            //    ForecastCategoryInstrument fcins = DataRepository.GetForecastCategoryInstrumentById(pu.Instrument.Id);

                            //    if (fcins != null)
                            //    {
                            //        decimal Qty = 0;
                            //        if (fcins.Instrument.DailyCtrlTest > 0)
                            //        {
                            //            Qty = pu.Rate * fPinDay * fcins.Instrument.DailyCtrlTest;
                            //            //cfresult.ForecastValue = fPinDay * fcins.Instrument.DailyCtrlTest;
                            //        }
                            //        if (fcins.Instrument.WeeklyCtrlTest > 0)
                            //        {
                            //            Qty = pu.Rate * fPinWeek * fcins.Instrument.WeeklyCtrlTest;
                            //            //cfresult.ForecastValue = fPinWeek * fcins.Instrument.WeeklyCtrlTest;
                            //        }
                            //        if (fcins.Instrument.MonthlyCtrlTest > 0)
                            //        {
                            //            Qty = pu.Rate * fPinMonth * fcins.Instrument.MonthlyCtrlTest;
                            //            //cfresult.ForecastValue = fPinMonth * fcins.Instrument.MonthlyCtrlTest;
                            //        }
                            //        if (fcins.Instrument.QuarterlyCtrlTest > 0)
                            //        {
                            //            Qty = pu.Rate * fPinQuarter * fcins.Instrument.QuarterlyCtrlTest;
                            //            //cfresult.ForecastValue = fPinQuarter * fcins.Instrument.QuarterlyCtrlTest;
                            //        }
                            //        if (fcins.Instrument.MaxTestBeforeCtrlTest > 0)
                            //        {
                            //            Qty = pu.Rate * ((fresult.TotalValue * (fcins.TestRunPercentage / 100)) / fcins.Instrument.MaxTestBeforeCtrlTest);
                            //            //cfresult.ForecastValue = ((fresult.TotalValue * (fcins.TestRunPercentage / 100)) / fcins.Instrument.MaxTestBeforeCtrlTest);
                            //        }

                            //        //decimal Qty = pu.Rate * fresult.TotalValue * fcins.TestRunPercentage;
                            //        cfresult.TotalValue = Qty;
                            //        int packSize = pu.Product.GetActiveProductPrice(fresult.DurationDateTime).PackSize;
                            //        cfresult.ProductId = pu.Product.Id;
                            //        cfresult.PackQty = GetNoofPackage(packSize, Qty);
                            //        cfresult.PackPrice = cfresult.PackQty * pu.Product.GetActiveProductPrice(fresult.DurationDateTime).Price;

                            //        cfresult.ProductTypeId = pu.Product.ProductType.Id;
                            //        cfresult.ProductType = pu.Product.ProductType.TypeName;
                            //        cfresult.ServiceConverted = true;
                            //        cfresult.IsForControl = true;
                            //        _listFresult.Add(cfresult);
                            //    }
                            //}
                        }
                    }
                    ///////// 
                    #endregion

                }
                //end test to product

                period++;
            }
        }


        private void ReadDatasetservice(ForecastInfo finfo, int siteid, int catid, int proid, int testid, MLModel ds, DateTime lastDate, DataTable inputDs, string Period)
        {
            Stopwatch ss = new Stopwatch();
            int workingdays = 0, testareaid = 0;
            Site fsite = new Site();
            if (siteid > 0)
            {
                fsite = ctx.Site.Find(siteid);
            }
            //if (siteid>0)
            //{period = 0, 

            ss.Start();


            //} 
            string Producttypename = "";
            /// ProductPrice pp = new ProductPrice();
            MasterProduct MP = new MasterProduct();

            //      var Forecastcategories = ctx.ForecastCategory.Where(b => b.ForecastId == finfo.ForecastID).ToList();
            //   var forecastsite = ctx.Site.ToList();
            // var tests = ctx.Test.ToList();
            // var testarealist = ctx.TestingArea.ToList();
            if (proid > 0)
            {
                MP = ctx.MasterProduct.Find(proid);
                //    MP._productPrices = ctx.ProductPrice.Where(b => b.ProductId == proid).ToList();
                Producttypename = ctx.ProductType.Find(MP.ProductTypeId).TypeName;

            }
            ForecastCategory fc = new ForecastCategory();
            if (catid > 0)
            {
                fc = ctx.ForecastCategory.Where(b => b.ForecastId == finfo.ForecastID && b.CategoryId == catid).FirstOrDefault();
            }

            string testareaname = "";
            if (testid > 0)
            {

                var testinfo = ctx.Test.Join(ctx.TestingArea, b => b.TestingAreaID, c => c.TestingAreaID, (b, c) => new { b, c }).Where(z => z.b.TestID == testid).FirstOrDefault();
                testareaid = testinfo.b.TestingAreaID;
                testareaname = testinfo.c.AreaName;
            }
            if (finfo.DataUsage == "DATA_USAGE3")
            {
                int[] sitesid = ctx.ForecastCategorySite.Where(c => c.CategoryID == catid).Select(s => s.SiteID).ToArray();
                workingdays = Convert.ToInt32(ctx.Site.Where(b => sitesid.Contains(b.SiteID)).Average(s => s.WorkingDays));


            }

            for (int i = 0; i < ds.Forecasts.Count; i++)

            {
                DateTime ddate = Convert.ToDateTime(ds.Forecasts[i].Datetime);



                Finalreslist fresult = new Finalreslist
                {
                    ForecastId = finfo.ForecastID,
                    SiteId = siteid,
                    CategoryId = catid,
                    TestId = testid,
                    ProductId = proid,
                    DurationDateTime = Utility.Utility.CorrectDateTime(ddate.Date)//ddate.Date;
                };

                //if (Convert.ToDecimal(ds.Forecasts[i].Forecast) < 0)
                //    fresult.ForecastValue = 0;
                //else
                fresult.ForecastValue = Convert.ToDecimal(ds.Forecasts[i].Forecast) < 0 ? 0 : Convert.ToDecimal(ds.Forecasts[i].Forecast);
           

              
                fresult.IsHistory = ddate > lastDate ? false : true;

                if (i<inputDs.Rows.Count)
                {
                    DataRow rowinput = inputDs.Rows[i];

                    fresult.HistoricalValue = ddate > lastDate ? 0 : Convert.ToDecimal(rowinput[1]);
                }
                else
                {
                    fresult.HistoricalValue = 0;
                }
                //    fresult.HistoricalValue = Convert.ToDecimal(rowinput[1]);
                //    fresult.IsHistory = true;
                //}

                //if (Period == "Monthly" || Period == "Bimonthly")
                //    fresult.Duration = String.Format("{0}-{1}", Utility.Utility.Months[fresult.DurationDateTime.Month - 1], fresult.DurationDateTime.Year);              
                //else
                //    fresult.Duration = String.Format("{0}", fresult.DurationDateTime.Year);
                fresult.Duration = Period == "Monthly" || Period == "Bimonthly" ? String.Format("{0}-{1}", Utility.Utility.Months[fresult.DurationDateTime.Month - 1], fresult.DurationDateTime.Year) : String.Format("{0}", fresult.DurationDateTime.Year);

                fresult.ServiceConverted = false;

                //Site fsite = null;
                //if (fresult.SiteId > 0)
                //    fsite = forecastsite.Where(b => b.SiteID == fresult.SiteId).FirstOrDefault();

                //ForecastCategory fc = null;
                //if (fresult.CategoryId > 0)
                //    fc = Forecastcategories.Where(b => b.CategoryId == fresult.CategoryId).FirstOrDefault();



                #region Forecast Period Conversion
                //if (finfo.DataUsage != "DATA_USAGE3")
                //{
                //    workingdays = fsite.Single().WorkingDays;   

                //}
                workingdays = finfo.DataUsage != "DATA_USAGE3" ? fsite.WorkingDays : 0;

                string fprd = Period;
                decimal fPinDay = 0, fPinWeek = 0, fPinMonth = 0, fPinQuarter = 0, fPinYear = 0;
                switch (fprd)
                {
                    case "Yearly":
                        fresult.FPinDay = workingdays * 12;
                        fresult.FPinMonth = 12;
                        fresult.FPinWeek = (workingdays / 4) * 12;
                        fresult.FPinQuarter = 4;
                        fresult.FPinYear = 1;
                        break;
                    case "Bimonthly":
                        fresult.FPinDay = workingdays * 2;
                        fresult.FPinMonth = 2;
                        fresult.FPinWeek = (workingdays / 4) * 2;
                        fresult.FPinQuarter = 2 / 3;
                        fresult.FPinYear = 1 / 6;
                        break;
                    case "Monthly":
                        fresult.FPinDay = workingdays;
                        fresult.FPinMonth = 1;
                        fresult.FPinWeek = (workingdays / 4);
                        fresult.FPinQuarter = 1 / 3;
                        fresult.FPinYear = 1 / 12;
                        break;
                    default:
                        break;
                }

                #endregion
                fresult.TestingArea = testareaname;

                // }

                int Nopack = int.Parse(decimal.Round(fresult.ForecastValue, 0).ToString());

                //if (Nopack < fresult.ForecastValue)
                //    Nopack = Nopack + 1;
                //if (Nopack == 0)
                //    Nopack = 0;
                fresult.Noofpack = Nopack < fresult.ForecastValue ? Nopack + 1 : Nopack;
                fresult.TotalValue = Nopack < fresult.ForecastValue ? Nopack + 1 : Nopack;
                fresult.ProductType = Producttypename;
                _Finalreslist.Add(fresult);
                //get packQty
                ////////if (fresult.ProductId > 0)
                ////////{
                ////////    //MasterProduct p = Masterproduct.Where(b => b.ProductID == fresult.ProductId).FirstOrDefault();
                ////////    //p._productPrices = productpriceList.Where(b => b.ProductId == fresult.ProductId).ToList();
                ////////    //converting quantity to packsize commented out
                ////////    //int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).PackSize;
                ////////    //fresult.PackQty = GetNoofPackage(packSize, fresult.TotalValue);

                ////////    //rounding forecasted pack quantity

                ////////    fresult.PackQty = Nopack;

                ////////    fresult.PackPrice = fresult.PackQty * MP.GetActiveProductPrice(fresult.DurationDateTime).packcost;

                ////////    fresult.ProductTypeId = MP.ProductTypeId;
                ////////    fresult.ProductType = Producttypename;
                ////////    _listFresult.Add(fresult);

                ////////}



                //////////test to product
                ////////if (fresult.TestId > 0)
                ////////{
                ////////    //Test test = ctx.Test.Find(fresult.TestId);

                ////////    #region Forecast General Consumables


                ////////    //     IList<ForecastedResult> _consumablesDailyFlist = new List<ForecastedResult>();

                ////////    var filterbytestidconsumusage = ForecastConsumableUsagelist.Where(b => b.TestId == fresult.TestId).ToList();
                ////////    //   foreach (ForecastConsumableUsage cusage in filterbytestidconsumusage)//DataRepository.GetConsumableUsageByTestId(fresult.TestId))
                ////////    for (int j = 0; j < filterbytestidconsumusage.Count; j++)

                ////////    {
                ////////        //
                ////////        ForecastedResult consumableFresult = new ForecastedResult();
                ////////        consumableFresult = fresult;
                ////////        //copyvalues
                ////////        //consumableFresult.ForecastId = fresult.ForecastId;
                ////////        //consumableFresult.TestId = fresult.TestId;
                ////////        //consumableFresult.DurationDateTime = fresult.DurationDateTime;
                ////////        //consumableFresult.SiteId = fresult.SiteId;
                ////////        //consumableFresult.CategoryId = fresult.CategoryId;
                ////////        //consumableFresult.Duration = fresult.Duration;
                ////////        //consumableFresult.IsHistory = fresult.IsHistory;
                ////////        //consumableFresult.TestingArea = fresult.TestingArea;
                ////////        //consumableFresult.ForecastValue = fresult.ForecastValue;
                ////////        //consumableFresult.TotalValue = fresult.TotalValue;
                ////////        //endcopy
                ////////        decimal Qty = 0;


                ////////        if (filterbytestidconsumusage[j].PerInstrument)
                ////////        {
                ////////            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////            {
                ////////                if (fsite != null)
                ////////                {
                ////////                    //     int siteins = 0;//forecastins.Where(b => b.InsID == filterbytestidconsumusage[j].InstrumentId).FirstOrDefault().Quantity;
                ////////                    var siteins = forecastins.FirstOrDefault(e => e.InsID == filterbytestidconsumusage[j].InstrumentId);
                ////////                    if (siteins != null)
                ////////                    {
                ////////                        switch (filterbytestidconsumusage[j].Period)
                ////////                        {
                ////////                            case "Daily":
                ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
                ////////                                break;
                ////////                            case "Weekly":
                ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
                ////////                                break;
                ////////                            case "Monthly":
                ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
                ////////                                break;
                ////////                            case "Quarterly":
                ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
                ////////                                break;
                ////////                            case "Yearly":
                ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
                ////////                                break;
                ////////                            default:
                ////////                                break;
                ////////                        }

                ////////                        Qty = Qty * siteins.Quantity;
                ////////                    }
                ////////                }
                ////////            }
                ////////        }
                ////////        if (filterbytestidconsumusage[j].PerPeriod)
                ////////        {
                ////////            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////            {
                ////////                if (fsite != null)
                ////////                {

                ////////                    switch (filterbytestidconsumusage[j].Period)
                ////////                    {
                ////////                        case "Daily":
                ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
                ////////                            break;
                ////////                        case "Weekly":
                ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
                ////////                            break;
                ////////                        case "Monthly":
                ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
                ////////                            break;
                ////////                        case "Quarterly":
                ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
                ////////                            break;
                ////////                        case "Yearly":
                ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
                ////////                            break;
                ////////                        default:
                ////////                            break;
                ////////                    }


                ////////                    //if (filterbytestidconsumusage[j].Period == "Daily")
                ////////                    //{
                ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
                ////////                    //}
                ////////                    //if (filterbytestidconsumusage[j].Period == "Weekly")
                ////////                    //{
                ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
                ////////                    //}
                ////////                    //if (filterbytestidconsumusage[j].Period == "Monthly")
                ////////                    //{
                ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
                ////////                    //}
                ////////                    //if (filterbytestidconsumusage[j].Period == "Quarterly")
                ////////                    //{
                ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
                ////////                    //}
                ////////                    //if (filterbytestidconsumusage[j].Period == "Yearly")
                ////////                    //{
                ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
                ////////                    //}
                ////////                }
                ////////            }
                ////////        }
                ////////        if (filterbytestidconsumusage[j].PerTest)
                ////////        {
                ////////            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////            {
                ////////                if (fsite != null)
                ////////                {
                ////////                    Qty = filterbytestidconsumusage[j].UsageRate * (fresult.TotalValue / filterbytestidconsumusage[j].NoOfTest);
                ////////                }
                ////////            }
                ////////        }

                ////////        consumableFresult.TotalValue = Qty;
                ////////        MasterProduct MP1 = new MasterProduct();
                ////////        MP1 = Masterproduct.Where(b => b.ProductID == filterbytestidconsumusage[j].ProductId).FirstOrDefault();
                ////////        MP1._productPrices = productpriceList.Where(b => b.ProductId == MP1.ProductID).ToList();
                ////////        int packSize = MP1.GetActiveProductPrice(fresult.DurationDateTime).packsize;
                ////////        consumableFresult.ProductId = filterbytestidconsumusage[j].ProductId;
                ////////        consumableFresult.PackQty = GetNoofPackage(packSize, Qty);
                ////////        consumableFresult.PackPrice = consumableFresult.PackQty * MP1.GetActiveProductPrice(fresult.DurationDateTime).packcost;
                ////////        consumableFresult.ProductTypeId = MP1.ProductTypeId;
                ////////        consumableFresult.ProductType = ctx.ProductType.Find(MP1.ProductTypeId).TypeName;
                ////////        consumableFresult.IsForGeneralConsumable = true;
                ////////        consumableFresult.ServiceConverted = true;
                ////////        _listFresult.Add(consumableFresult);


                ////////    }



                ////////    #endregion

                ////////    #region Forecast Control Test

                ////////    if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////    {
                ////////        if (fsite != null)
                ////////        {
                ////////            //ForecastIns siteins =forecastins.Join(ctx.Instrument, b => b.InsID, c => c.InstrumentID, (b, c) => new { b, c })
                ////////            //    .Where(x => x.c.testingArea.TestingAreaID == testareaid)
                ////////            //    .Select(s => new ForecastIns
                ////////            //    {

                ////////            //        ID = s.b.ID,
                ////////            //        InsID = s.b.InsID
                ////////            //    }).FirstOrDefault();
                ////////            //fsite.GetSiteInstrumentByTA(test.TestingArea.Id);
                ////////            if (forecastins != null)
                ////////            {
                ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
                ////////                {
                ////////                    fresult.ControlTest = fPinDay * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.DailyCtrlTest).FirstOrDefault();
                ////////                }
                ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
                ////////                {
                ////////                    fresult.ControlTest = fPinWeek * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
                ////////                }
                ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
                ////////                {
                ////////                    fresult.ControlTest = fPinMonth * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();

                ////////                }
                ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
                ////////                {
                ////////                    fresult.ControlTest = fPinQuarter * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
                ////////                }
                ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
                ////////                {
                ////////                    fresult.ControlTest = ((fresult.TotalValue * (forecastins.FirstOrDefault().TestRunPercentage / 100)) / inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
                ////////                }
                ////////            }
                ////////        }
                ////////    }

                ////////    #endregion

                ////////    #region Test Test to Product
                ////////    var productusage = ForecastProductUsage.Where(b => b.TestId == testid && b.IsForControl == false).ToList();
                ////////    // foreach (ForecastProductUsage pu in productusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
                ////////    for (int ki = 0; ki < productusage.Count; ki++)

                ////////    {
                ////////        ForecastedResult cfresult = new ForecastedResult
                ////////        {
                ////////            //copyvalues
                ////////            ForecastId = fresult.ForecastId,
                ////////            TestId = fresult.TestId,
                ////////            DurationDateTime = fresult.DurationDateTime,
                ////////            SiteId = fresult.SiteId,
                ////////            CategoryId = fresult.CategoryId,
                ////////            Duration = fresult.Duration,
                ////////            IsHistory = fresult.IsHistory,
                ////////            TestingArea = fresult.TestingArea
                ////////        };
                ////////        //endcopy

                ////////        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////        {
                ////////            if (fsite != null)
                ////////            {
                ////////                ForecastIns siteins = ctx.ForecastIns.Where(b => b.InsID == productusage[ki].InstrumentId && b.forecastID == fresult.ForecastId).FirstOrDefault();
                ////////                if (siteins != null)
                ////////                {
                ////////                    decimal Qty = productusage[ki].Rate * fresult.TotalValue * siteins.TestRunPercentage / 100;
                ////////                    cfresult.TotalValue = Qty;
                ////////                    MasterProduct p = Masterproduct.Where(b => b.ProductID == productusage[ki].ProductId).FirstOrDefault();
                ////////                    p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
                ////////                    int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).packsize;
                ////////                    cfresult.ProductId = productusage[ki].ProductId;
                ////////                    cfresult.PackQty = GetNoofPackage(packSize, Qty);
                ////////                    cfresult.PackPrice = cfresult.PackQty * p.GetActiveProductPrice(fresult.DurationDateTime).packcost;

                ////////                    cfresult.ProductTypeId = Masterproduct.Where(b => b.ProductID == productusage[ki].ProductId).FirstOrDefault().ProductTypeId;
                ////////                    cfresult.ProductType = producttypelist.Where(b => b.TypeID == cfresult.ProductTypeId).FirstOrDefault().TypeName;
                ////////                    cfresult.ServiceConverted = true;
                ////////                    _listFresult.Add(cfresult);
                ////////                }
                ////////            }
                ////////        }
                ////////        else
                ////////        {
                ////////            //if (fsite != null)
                ////////            //{
                ////////            //    ForecastCategoryInstrument fcins = DataRepository.GetForecastCategoryInstrumentById(pu.Instrument.Id);

                ////////            //    if (fcins != null)
                ////////            //    {
                ////////            //        decimal Qty = pu.Rate * fresult.TotalValue * fcins.TestRunPercentage;
                ////////            //        cfresult.TotalValue = Qty;
                ////////            //        int packSize = pu.Product.GetActiveProductPrice(fresult.DurationDateTime).PackSize;
                ////////            //        cfresult.ProductId = pu.Product.Id;
                ////////            //        cfresult.PackQty = GetNoofPackage(packSize, Qty);
                ////////            //        cfresult.PackPrice = cfresult.PackQty * pu.Product.GetActiveProductPrice(fresult.DurationDateTime).Price;

                ////////            //        cfresult.ProductTypeId = pu.Product.ProductType.Id;
                ////////            //        cfresult.ProductType = pu.Product.ProductType.TypeName;
                ////////            //        cfresult.ServiceConverted = true;
                ////////            //        _listFresult.Add(cfresult);
                ////////            //    }
                ////////            //}
                ////////        }
                ////////    }
                ////////    ///////// 
                ////////    #endregion

                ////////    #region Control Test to Product


                ////////    var cproductusage = ForecastProductUsage.Where(b => b.TestId == testid && b.IsForControl == true).ToList();
                ////////    // foreach (ForecastProductUsage pu in cproductusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
                ////////    for (int z = 0; z < cproductusage.Count; z++)

                ////////    {
                ////////        ForecastedResult cfresult = new ForecastedResult();
                ////////        cfresult = fresult;
                ////////        //copyvalues
                ////////        //cfresult.ForecastId = fresult.ForecastId;
                ////////        //cfresult.TestId = fresult.TestId;
                ////////        //cfresult.DurationDateTime = fresult.DurationDateTime;
                ////////        //cfresult.SiteId = fresult.SiteId;
                ////////        //cfresult.CategoryId = fresult.CategoryId;
                ////////        //cfresult.Duration = fresult.Duration;
                ////////        //cfresult.IsHistory = fresult.IsHistory;
                ////////        //cfresult.TestingArea = fresult.TestingArea;

                ////////        cfresult.IsForControl = true;
                ////////        //endcopy

                ////////        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////        {
                ////////            if (fsite != null)
                ////////            {
                ////////                ForecastIns siteins = forecastins.Where(b => b.InsID == cproductusage[z].InstrumentId).FirstOrDefault();
                ////////                if (siteins != null)
                ////////                {


                ////////                    decimal Qty = 0;
                ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
                ////////                    {
                ////////                        Qty = cproductusage[z].Rate * fPinDay * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.DailyCtrlTest).FirstOrDefault();
                ////////                        // cfresult.ForecastValue = fPinDay * siteins.Quantity * siteins.Instrument.DailyCtrlTest;
                ////////                    }
                ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
                ////////                    {
                ////////                        Qty = cproductusage[z].Rate * fPinWeek * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
                ////////                        // cfresult.ForecastValue = fPinWeek * siteins.Quantity * siteins.Instrument.WeeklyCtrlTest;
                ////////                    }
                ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
                ////////                    {
                ////////                        Qty = cproductusage[z].Rate * fPinMonth * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();
                ////////                        // cfresult.ForecastValue = fPinMonth * siteins.Quantity * siteins.Instrument.MonthlyCtrlTest;

                ////////                    }
                ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
                ////////                    {
                ////////                        Qty = cproductusage[z].Rate * fPinQuarter * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
                ////////                        //cfresult.ForecastValue = fPinQuarter * siteins.Quantity * siteins.Instrument.QuarterlyCtrlTest;
                ////////                    }
                ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
                ////////                    {
                ////////                        Qty = cproductusage[z].Rate * ((fresult.TotalValue * (siteins.TestRunPercentage / 100)) / inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
                ////////                        // cfresult.ForecastValue = ((fresult.TotalValue * (siteins.TestRunPercentage / 100)) / siteins.Instrument.MaxTestBeforeCtrlTest);
                ////////                    }


                ////////                    //decimal Qty = pu.Rate * fresult.TotalValue * siteins.TestRunPercentage / 100;

                ////////                    cfresult.TotalValue = Qty;
                ////////                    MasterProduct p = Masterproduct.Where(b => b.ProductID == cproductusage[z].ProductId).FirstOrDefault();
                ////////                    p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
                ////////                    int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).packsize;
                ////////                    cfresult.ProductId = cproductusage[z].ProductId;
                ////////                    cfresult.PackQty = GetNoofPackage(packSize, Qty);
                ////////                    cfresult.PackPrice = cfresult.PackQty * p.GetActiveProductPrice(fresult.DurationDateTime).packcost;

                ////////                    cfresult.ProductTypeId = Masterproduct.Where(b => b.ProductID == cproductusage[z].ProductId).FirstOrDefault().ProductTypeId;
                ////////                    cfresult.ProductType = producttypelist.Where(b => b.TypeID == cfresult.ProductTypeId).FirstOrDefault().TypeName;
                ////////                    cfresult.ServiceConverted = true;
                ////////                    _listFresult.Add(cfresult);
                ////////                }
                ////////            }
                ////////        }
                ////////        else
                ////////        {

                ////////        }
                ////////    }
                ////////    ///////// 
                ////////    #endregion

                ////////}
                //end test to product

                // period++;
            }
            ss.Stop();
        }

<<<<<<< HEAD
        //private void ReadDatasetconsumptionnew(ForecastInfo finfo, List<MLModel> ds,  string Period,int forecastsiteid,List<Site> sites,List<MasterProduct> masterpro,List<ProductType> masterproducttype,List<Test> tests,List<TestingArea> testingAreas)
        //{
        //    Stopwatch ss = new Stopwatch();
        //    int workingdays = 0, testareaid = 0;
        //    Site fsite = new Site();

        //    ss.Start();
        //    DataTable inputDs = new DataTable();
        //    inputDs.Columns.Add("X");
        //    inputDs.Columns.Add("Y");
        //    DateTime lastDate;
        //    //} 
        //    string Producttypename = "";
        //    /// ProductPrice pp = new ProductPrice();
        //    MasterProduct MP = new MasterProduct();
        //    List<ForecastSiteProduct> forecastsiteproduct = new List<ForecastSiteProduct>();
        //    List<ForecastSiteTest> ForecastSiteTest = new List<ForecastSiteTest>();
        //    //var sites = ctx.Site.ToList();
        //    //var masterpro = ctx.MasterProduct.ToList();
        //    //var masterproducttype = ctx.ProductType.ToList();
        //    //var tests = ctx.Test.ToList();
        //    if (finfo.Methodology == "CONSUMPTION")
        //    {
        //        forecastsiteproduct = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == forecastsiteid).ToList();
        //    }
        //    else
        //    {
        //        ForecastSiteTest = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == forecastsiteid).ToList();
        //    }
        //    for (int i = 0; i < ds.Count; i++)

        //    {

        //        if (finfo.Methodology == "CONSUMPTION")
        //        {

        //            List<ForecastSiteProduct> fsiteProduct = forecastsiteproduct.Where(b =>b.ProductID == ds[i].ProductId).OrderBy(x => x.DurationDateTime).ToList();
        //            for (int f = 0; f < fsiteProduct.Count; f++)
        //            {
        //                DataRow Dr = inputDs.NewRow();
        //                Dr["X"] = fsiteProduct[f].DurationDateTime;
        //                Dr["Y"] = fsiteProduct[f].Adjusted;
        //                inputDs.Rows.Add(Dr);
        //            }
        //            lastDate = fsiteProduct[fsiteProduct.Count - 1].DurationDateTime.Value;
        //        }
        //        else
        //        {
        //            List<ForecastSiteTest> fsiteTest = ForecastSiteTest.Where(b =>  b.TestID == ds[i].TestId).OrderBy(f => f.DurationDateTime).ToList();


        //            //  DataTable InputDs = new DataTable();
        //            //inputDs.Columns.Add("X");
        //            //inputDs.Columns.Add("Y");
        //            for (int f = 0; f < fsiteTest.Count; f++)
        //            {
        //                DataRow Dr = inputDs.NewRow();
        //                Dr["X"] = fsiteTest[f].DurationDateTime;
        //                Dr["Y"] = fsiteTest[f].Adjusted;
        //                inputDs.Rows.Add(Dr);
        //            }
        //            lastDate = fsiteTest[fsiteTest.Count - 1].DurationDateTime.Value;
        //        }
        //        if (ds[i].SiteId > 0)
        //        {
        //            fsite = sites.Where(b=>b.SiteID==ds[i].SiteId).FirstOrDefault();
        //        }
        //        //if (siteid>0)
        //        //{period = 0, 

        //        //      var Forecastcategories = ctx.ForecastCategory.Where(b => b.ForecastId == finfo.ForecastID).ToList();
        //        //   var forecastsite = ctx.Site.ToList();
        //        // var tests = ctx.Test.ToList();
        //        // var testarealist = ctx.TestingArea.ToList();
        //        if (ds[i].ProductId > 0)
        //        {
        //            MP = masterpro.Where(b => b.ProductID == ds[i].ProductId).FirstOrDefault();
        //            //    MP._productPrices = ctx.ProductPrice.Where(b => b.ProductId == proid).ToList();
        //            Producttypename = masterproducttype.Where(b => b.TypeID == MP.ProductTypeId).FirstOrDefault().TypeName;

        //        }
        //        ForecastCategory fc = new ForecastCategory();
        //        if (ds[i].CategoryId > 0)
        //        {
        //            fc = ctx.ForecastCategory.Where(b => b.ForecastId == finfo.ForecastID && b.CategoryId == ds[i].CategoryId).FirstOrDefault();
        //        }

        //        string testareaname = "";
        //        if (ds[i].TestId > 0)
        //        {

        //            var testinfo =tests.Join(testingAreas, b => b.TestingAreaID, c => c.TestingAreaID, (b, c) => new { b, c }).Where(z => z.b.TestID == ds[i].TestId).FirstOrDefault();
        //            testareaid = testinfo.b.TestingAreaID;
        //            testareaname = testinfo.c.AreaName;
        //        }
        //        if (finfo.DataUsage == "DATA_USAGE3")
        //        {
        //            int[] sitesid = ctx.ForecastCategorySite.Where(c => c.CategoryID == ds[i].CategoryId).Select(s => s.SiteID).ToArray();
        //            workingdays = Convert.ToInt32(sites.Where(b => sitesid.Contains(b.SiteID)).Average(s => s.WorkingDays));


        //        }

        //        for (int j = 0; j < ds[i].Forecasts.Count; j++)
        //        {


        //        DateTime ddate = Convert.ToDateTime(ds[i].Forecasts[j].Datetime);



        //        Finalreslist fresult = new Finalreslist
        //        {
        //            ForecastId = finfo.ForecastID,
        //            SiteId = ds[i].SiteId,
        //            CategoryId = ds[i].CategoryId,
        //            TestId = ds[i].TestId,
        //            ProductId = ds[i].ProductId,
        //            DurationDateTime = Utility.Utility.CorrectDateTime(ddate.Date)//ddate.Date;
        //        };

        //        //if (Convert.ToDecimal(ds.Forecasts[i].Forecast) < 0)
        //        //    fresult.ForecastValue = 0;
        //        //else
        //        fresult.ForecastValue = Convert.ToDecimal(ds[i].Forecasts[j].Forecast) < 0 ? 0 : Convert.ToDecimal(ds[i].Forecasts[j].Forecast);



        //        fresult.IsHistory = ddate > lastDate ? false : true;

        //        if (i < inputDs.Rows.Count)
        //        {
        //            DataRow rowinput = inputDs.Rows[i];

        //            fresult.HistoricalValue = ddate > lastDate ? 0 : Convert.ToDecimal(rowinput[1]);
        //        }
        //        else
        //        {
        //            fresult.HistoricalValue = 0;
        //        }
        //        //    fresult.HistoricalValue = Convert.ToDecimal(rowinput[1]);
        //        //    fresult.IsHistory = true;
        //        //}

        //        //if (Period == "Monthly" || Period == "Bimonthly")
        //        //    fresult.Duration = String.Format("{0}-{1}", Utility.Utility.Months[fresult.DurationDateTime.Month - 1], fresult.DurationDateTime.Year);              
        //        //else
        //        //    fresult.Duration = String.Format("{0}", fresult.DurationDateTime.Year);
        //        fresult.Duration = Period == "Monthly" || Period == "Bimonthly" ? String.Format("{0}-{1}", Utility.Utility.Months[fresult.DurationDateTime.Month - 1], fresult.DurationDateTime.Year) : String.Format("{0}", fresult.DurationDateTime.Year);

        //        fresult.ServiceConverted = false;

        //        //Site fsite = null;
        //        //if (fresult.SiteId > 0)
        //        //    fsite = forecastsite.Where(b => b.SiteID == fresult.SiteId).FirstOrDefault();

        //        //ForecastCategory fc = null;
        //        //if (fresult.CategoryId > 0)
        //        //    fc = Forecastcategories.Where(b => b.CategoryId == fresult.CategoryId).FirstOrDefault();



        //        #region Forecast Period Conversion
        //        //if (finfo.DataUsage != "DATA_USAGE3")
        //        //{
        //        //    workingdays = fsite.Single().WorkingDays;   

        //        //}
        //        workingdays = finfo.DataUsage != "DATA_USAGE3" ? fsite.WorkingDays : 0;

        //        string fprd = Period;
        //        decimal fPinDay = 0, fPinWeek = 0, fPinMonth = 0, fPinYear = 0;
        //        switch (fprd)
        //        {
        //            case "Yearly":
        //                fresult.FPinDay = workingdays * 12;
        //                fresult.FPinMonth = 12;
        //                fresult.FPinWeek = (workingdays / 4) * 12;
        //                fresult.FPinQuarter = 4;
        //                fresult.FPinYear = 1;
        //                break;
        //            case "Bimonthly":
        //                fresult.FPinDay = workingdays * 2;
        //                fresult.FPinMonth = 2;
        //                fresult.FPinWeek = (workingdays / 4) * 2;
        //                fresult.FPinQuarter = 2 / 3;
        //                fresult.FPinYear = 1 / 6;
        //                break;
        //            case "Monthly":
        //                fresult.FPinDay = workingdays;
        //                fresult.FPinMonth = 1;
        //                fresult.FPinWeek = (workingdays / 4);
        //                fresult.FPinQuarter = 1 / 3;
        //                fresult.FPinYear = 1 / 12;
        //                break;
        //            default:
        //                break;
        //        }

        //        #endregion
        //        fresult.TestingArea = testareaname;

        //        // }

        //        int Nopack = int.Parse(decimal.Round(fresult.ForecastValue, 0).ToString());

        //        //if (Nopack < fresult.ForecastValue)
        //        //    Nopack = Nopack + 1;
        //        //if (Nopack == 0)
        //        //    Nopack = 0;
        //        fresult.Noofpack = Nopack < fresult.ForecastValue ? Nopack + 1 : Nopack;
        //        fresult.TotalValue = Nopack < fresult.ForecastValue ? Nopack + 1 : Nopack;
        //        fresult.ProductType = Producttypename;
        //        _Finalreslist.Add(fresult);
        //    }
        //        //get packQty
        //        ////////if (fresult.ProductId > 0)
        //        ////////{
        //        ////////    //MasterProduct p = Masterproduct.Where(b => b.ProductID == fresult.ProductId).FirstOrDefault();
        //        ////////    //p._productPrices = productpriceList.Where(b => b.ProductId == fresult.ProductId).ToList();
        //        ////////    //converting quantity to packsize commented out
        //        ////////    //int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).PackSize;
        //        ////////    //fresult.PackQty = GetNoofPackage(packSize, fresult.TotalValue);

        //        ////////    //rounding forecasted pack quantity

        //        ////////    fresult.PackQty = Nopack;

        //        ////////    fresult.PackPrice = fresult.PackQty * MP.GetActiveProductPrice(fresult.DurationDateTime).packcost;

        //        ////////    fresult.ProductTypeId = MP.ProductTypeId;
        //        ////////    fresult.ProductType = Producttypename;
        //        ////////    _listFresult.Add(fresult);

        //        ////////}



        //        //////////test to product
        //        ////////if (fresult.TestId > 0)
        //        ////////{
        //        ////////    //Test test = ctx.Test.Find(fresult.TestId);

        //        ////////    #region Forecast General Consumables


        //        ////////    //     IList<ForecastedResult> _consumablesDailyFlist = new List<ForecastedResult>();

        //        ////////    var filterbytestidconsumusage = ForecastConsumableUsagelist.Where(b => b.TestId == fresult.TestId).ToList();
        //        ////////    //   foreach (ForecastConsumableUsage cusage in filterbytestidconsumusage)//DataRepository.GetConsumableUsageByTestId(fresult.TestId))
        //        ////////    for (int j = 0; j < filterbytestidconsumusage.Count; j++)

        //        ////////    {
        //        ////////        //
        //        ////////        ForecastedResult consumableFresult = new ForecastedResult();
        //        ////////        consumableFresult = fresult;
        //        ////////        //copyvalues
        //        ////////        //consumableFresult.ForecastId = fresult.ForecastId;
        //        ////////        //consumableFresult.TestId = fresult.TestId;
        //        ////////        //consumableFresult.DurationDateTime = fresult.DurationDateTime;
        //        ////////        //consumableFresult.SiteId = fresult.SiteId;
        //        ////////        //consumableFresult.CategoryId = fresult.CategoryId;
        //        ////////        //consumableFresult.Duration = fresult.Duration;
        //        ////////        //consumableFresult.IsHistory = fresult.IsHistory;
        //        ////////        //consumableFresult.TestingArea = fresult.TestingArea;
        //        ////////        //consumableFresult.ForecastValue = fresult.ForecastValue;
        //        ////////        //consumableFresult.TotalValue = fresult.TotalValue;
        //        ////////        //endcopy
        //        ////////        decimal Qty = 0;


        //        ////////        if (filterbytestidconsumusage[j].PerInstrument)
        //        ////////        {
        //        ////////            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //        ////////            {
        //        ////////                if (fsite != null)
        //        ////////                {
        //        ////////                    //     int siteins = 0;//forecastins.Where(b => b.InsID == filterbytestidconsumusage[j].InstrumentId).FirstOrDefault().Quantity;
        //        ////////                    var siteins = forecastins.FirstOrDefault(e => e.InsID == filterbytestidconsumusage[j].InstrumentId);
        //        ////////                    if (siteins != null)
        //        ////////                    {
        //        ////////                        switch (filterbytestidconsumusage[j].Period)
        //        ////////                        {
        //        ////////                            case "Daily":
        //        ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
        //        ////////                                break;
        //        ////////                            case "Weekly":
        //        ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
        //        ////////                                break;
        //        ////////                            case "Monthly":
        //        ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
        //        ////////                                break;
        //        ////////                            case "Quarterly":
        //        ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
        //        ////////                                break;
        //        ////////                            case "Yearly":
        //        ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
        //        ////////                                break;
        //        ////////                            default:
        //        ////////                                break;
        //        ////////                        }

        //        ////////                        Qty = Qty * siteins.Quantity;
        //        ////////                    }
        //        ////////                }
        //        ////////            }
        //        ////////        }
        //        ////////        if (filterbytestidconsumusage[j].PerPeriod)
        //        ////////        {
        //        ////////            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //        ////////            {
        //        ////////                if (fsite != null)
        //        ////////                {

        //        ////////                    switch (filterbytestidconsumusage[j].Period)
        //        ////////                    {
        //        ////////                        case "Daily":
        //        ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
        //        ////////                            break;
        //        ////////                        case "Weekly":
        //        ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
        //        ////////                            break;
        //        ////////                        case "Monthly":
        //        ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
        //        ////////                            break;
        //        ////////                        case "Quarterly":
        //        ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
        //        ////////                            break;
        //        ////////                        case "Yearly":
        //        ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
        //        ////////                            break;
        //        ////////                        default:
        //        ////////                            break;
        //        ////////                    }


        //        ////////                    //if (filterbytestidconsumusage[j].Period == "Daily")
        //        ////////                    //{
        //        ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
        //        ////////                    //}
        //        ////////                    //if (filterbytestidconsumusage[j].Period == "Weekly")
        //        ////////                    //{
        //        ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
        //        ////////                    //}
        //        ////////                    //if (filterbytestidconsumusage[j].Period == "Monthly")
        //        ////////                    //{
        //        ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
        //        ////////                    //}
        //        ////////                    //if (filterbytestidconsumusage[j].Period == "Quarterly")
        //        ////////                    //{
        //        ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
        //        ////////                    //}
        //        ////////                    //if (filterbytestidconsumusage[j].Period == "Yearly")
        //        ////////                    //{
        //        ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
        //        ////////                    //}
        //        ////////                }
        //        ////////            }
        //        ////////        }
        //        ////////        if (filterbytestidconsumusage[j].PerTest)
        //        ////////        {
        //        ////////            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //        ////////            {
        //        ////////                if (fsite != null)
        //        ////////                {
        //        ////////                    Qty = filterbytestidconsumusage[j].UsageRate * (fresult.TotalValue / filterbytestidconsumusage[j].NoOfTest);
        //        ////////                }
        //        ////////            }
        //        ////////        }

        //        ////////        consumableFresult.TotalValue = Qty;
        //        ////////        MasterProduct MP1 = new MasterProduct();
        //        ////////        MP1 = Masterproduct.Where(b => b.ProductID == filterbytestidconsumusage[j].ProductId).FirstOrDefault();
        //        ////////        MP1._productPrices = productpriceList.Where(b => b.ProductId == MP1.ProductID).ToList();
        //        ////////        int packSize = MP1.GetActiveProductPrice(fresult.DurationDateTime).packsize;
        //        ////////        consumableFresult.ProductId = filterbytestidconsumusage[j].ProductId;
        //        ////////        consumableFresult.PackQty = GetNoofPackage(packSize, Qty);
        //        ////////        consumableFresult.PackPrice = consumableFresult.PackQty * MP1.GetActiveProductPrice(fresult.DurationDateTime).packcost;
        //        ////////        consumableFresult.ProductTypeId = MP1.ProductTypeId;
        //        ////////        consumableFresult.ProductType = ctx.ProductType.Find(MP1.ProductTypeId).TypeName;
        //        ////////        consumableFresult.IsForGeneralConsumable = true;
        //        ////////        consumableFresult.ServiceConverted = true;
        //        ////////        _listFresult.Add(consumableFresult);


        //        ////////    }



        //        ////////    #endregion

        //        ////////    #region Forecast Control Test

        //        ////////    if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //        ////////    {
        //        ////////        if (fsite != null)
        //        ////////        {
        //        ////////            //ForecastIns siteins =forecastins.Join(ctx.Instrument, b => b.InsID, c => c.InstrumentID, (b, c) => new { b, c })
        //        ////////            //    .Where(x => x.c.testingArea.TestingAreaID == testareaid)
        //        ////////            //    .Select(s => new ForecastIns
        //        ////////            //    {

        //        ////////            //        ID = s.b.ID,
        //        ////////            //        InsID = s.b.InsID
        //        ////////            //    }).FirstOrDefault();
        //        ////////            //fsite.GetSiteInstrumentByTA(test.TestingArea.Id);
        //        ////////            if (forecastins != null)
        //        ////////            {
        //        ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
        //        ////////                {
        //        ////////                    fresult.ControlTest = fPinDay * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.DailyCtrlTest).FirstOrDefault();
        //        ////////                }
        //        ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
        //        ////////                {
        //        ////////                    fresult.ControlTest = fPinWeek * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
        //        ////////                }
        //        ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
        //        ////////                {
        //        ////////                    fresult.ControlTest = fPinMonth * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();

        //        ////////                }
        //        ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
        //        ////////                {
        //        ////////                    fresult.ControlTest = fPinQuarter * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
        //        ////////                }
        //        ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
        //        ////////                {
        //        ////////                    fresult.ControlTest = ((fresult.TotalValue * (forecastins.FirstOrDefault().TestRunPercentage / 100)) / inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
        //        ////////                }
        //        ////////            }
        //        ////////        }
        //        ////////    }

        //        ////////    #endregion

        //        ////////    #region Test Test to Product
        //        ////////    var productusage = ForecastProductUsage.Where(b => b.TestId == testid && b.IsForControl == false).ToList();
        //        ////////    // foreach (ForecastProductUsage pu in productusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
        //        ////////    for (int ki = 0; ki < productusage.Count; ki++)

        //        ////////    {
        //        ////////        ForecastedResult cfresult = new ForecastedResult
        //        ////////        {
        //        ////////            //copyvalues
        //        ////////            ForecastId = fresult.ForecastId,
        //        ////////            TestId = fresult.TestId,
        //        ////////            DurationDateTime = fresult.DurationDateTime,
        //        ////////            SiteId = fresult.SiteId,
        //        ////////            CategoryId = fresult.CategoryId,
        //        ////////            Duration = fresult.Duration,
        //        ////////            IsHistory = fresult.IsHistory,
        //        ////////            TestingArea = fresult.TestingArea
        //        ////////        };
        //        ////////        //endcopy

        //        ////////        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //        ////////        {
        //        ////////            if (fsite != null)
        //        ////////            {
        //        ////////                ForecastIns siteins = ctx.ForecastIns.Where(b => b.InsID == productusage[ki].InstrumentId && b.forecastID == fresult.ForecastId).FirstOrDefault();
        //        ////////                if (siteins != null)
        //        ////////                {
        //        ////////                    decimal Qty = productusage[ki].Rate * fresult.TotalValue * siteins.TestRunPercentage / 100;
        //        ////////                    cfresult.TotalValue = Qty;
        //        ////////                    MasterProduct p = Masterproduct.Where(b => b.ProductID == productusage[ki].ProductId).FirstOrDefault();
        //        ////////                    p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
        //        ////////                    int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).packsize;
        //        ////////                    cfresult.ProductId = productusage[ki].ProductId;
        //        ////////                    cfresult.PackQty = GetNoofPackage(packSize, Qty);
        //        ////////                    cfresult.PackPrice = cfresult.PackQty * p.GetActiveProductPrice(fresult.DurationDateTime).packcost;

        //        ////////                    cfresult.ProductTypeId = Masterproduct.Where(b => b.ProductID == productusage[ki].ProductId).FirstOrDefault().ProductTypeId;
        //        ////////                    cfresult.ProductType = producttypelist.Where(b => b.TypeID == cfresult.ProductTypeId).FirstOrDefault().TypeName;
        //        ////////                    cfresult.ServiceConverted = true;
        //        ////////                    _listFresult.Add(cfresult);
        //        ////////                }
        //        ////////            }
        //        ////////        }
        //        ////////        else
        //        ////////        {
        //        ////////            //if (fsite != null)
        //        ////////            //{
        //        ////////            //    ForecastCategoryInstrument fcins = DataRepository.GetForecastCategoryInstrumentById(pu.Instrument.Id);

        //        ////////            //    if (fcins != null)
        //        ////////            //    {
        //        ////////            //        decimal Qty = pu.Rate * fresult.TotalValue * fcins.TestRunPercentage;
        //        ////////            //        cfresult.TotalValue = Qty;
        //        ////////            //        int packSize = pu.Product.GetActiveProductPrice(fresult.DurationDateTime).PackSize;
        //        ////////            //        cfresult.ProductId = pu.Product.Id;
        //        ////////            //        cfresult.PackQty = GetNoofPackage(packSize, Qty);
        //        ////////            //        cfresult.PackPrice = cfresult.PackQty * pu.Product.GetActiveProductPrice(fresult.DurationDateTime).Price;

        //        ////////            //        cfresult.ProductTypeId = pu.Product.ProductType.Id;
        //        ////////            //        cfresult.ProductType = pu.Product.ProductType.TypeName;
        //        ////////            //        cfresult.ServiceConverted = true;
        //        ////////            //        _listFresult.Add(cfresult);
        //        ////////            //    }
        //        ////////            //}
        //        ////////        }
        //        ////////    }
        //        ////////    ///////// 
        //        ////////    #endregion

        //        ////////    #region Control Test to Product


        //        ////////    var cproductusage = ForecastProductUsage.Where(b => b.TestId == testid && b.IsForControl == true).ToList();
        //        ////////    // foreach (ForecastProductUsage pu in cproductusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
        //        ////////    for (int z = 0; z < cproductusage.Count; z++)

        //        ////////    {
        //        ////////        ForecastedResult cfresult = new ForecastedResult();
        //        ////////        cfresult = fresult;
        //        ////////        //copyvalues
        //        ////////        //cfresult.ForecastId = fresult.ForecastId;
        //        ////////        //cfresult.TestId = fresult.TestId;
        //        ////////        //cfresult.DurationDateTime = fresult.DurationDateTime;
        //        ////////        //cfresult.SiteId = fresult.SiteId;
        //        ////////        //cfresult.CategoryId = fresult.CategoryId;
        //        ////////        //cfresult.Duration = fresult.Duration;
        //        ////////        //cfresult.IsHistory = fresult.IsHistory;
        //        ////////        //cfresult.TestingArea = fresult.TestingArea;

        //        ////////        cfresult.IsForControl = true;
        //        ////////        //endcopy

        //        ////////        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
        //        ////////        {
        //        ////////            if (fsite != null)
        //        ////////            {
        //        ////////                ForecastIns siteins = forecastins.Where(b => b.InsID == cproductusage[z].InstrumentId).FirstOrDefault();
        //        ////////                if (siteins != null)
        //        ////////                {


        //        ////////                    decimal Qty = 0;
        //        ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
        //        ////////                    {
        //        ////////                        Qty = cproductusage[z].Rate * fPinDay * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.DailyCtrlTest).FirstOrDefault();
        //        ////////                        // cfresult.ForecastValue = fPinDay * siteins.Quantity * siteins.Instrument.DailyCtrlTest;
        //        ////////                    }
        //        ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
        //        ////////                    {
        //        ////////                        Qty = cproductusage[z].Rate * fPinWeek * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
        //        ////////                        // cfresult.ForecastValue = fPinWeek * siteins.Quantity * siteins.Instrument.WeeklyCtrlTest;
        //        ////////                    }
        //        ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
        //        ////////                    {
        //        ////////                        Qty = cproductusage[z].Rate * fPinMonth * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();
        //        ////////                        // cfresult.ForecastValue = fPinMonth * siteins.Quantity * siteins.Instrument.MonthlyCtrlTest;

        //        ////////                    }
        //        ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
        //        ////////                    {
        //        ////////                        Qty = cproductusage[z].Rate * fPinQuarter * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
        //        ////////                        //cfresult.ForecastValue = fPinQuarter * siteins.Quantity * siteins.Instrument.QuarterlyCtrlTest;
        //        ////////                    }
        //        ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
        //        ////////                    {
        //        ////////                        Qty = cproductusage[z].Rate * ((fresult.TotalValue * (siteins.TestRunPercentage / 100)) / inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
        //        ////////                        // cfresult.ForecastValue = ((fresult.TotalValue * (siteins.TestRunPercentage / 100)) / siteins.Instrument.MaxTestBeforeCtrlTest);
        //        ////////                    }


        //        ////////                    //decimal Qty = pu.Rate * fresult.TotalValue * siteins.TestRunPercentage / 100;

        //        ////////                    cfresult.TotalValue = Qty;
        //        ////////                    MasterProduct p = Masterproduct.Where(b => b.ProductID == cproductusage[z].ProductId).FirstOrDefault();
        //        ////////                    p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
        //        ////////                    int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).packsize;
        //        ////////                    cfresult.ProductId = cproductusage[z].ProductId;
        //        ////////                    cfresult.PackQty = GetNoofPackage(packSize, Qty);
        //        ////////                    cfresult.PackPrice = cfresult.PackQty * p.GetActiveProductPrice(fresult.DurationDateTime).packcost;

        //        ////////                    cfresult.ProductTypeId = Masterproduct.Where(b => b.ProductID == cproductusage[z].ProductId).FirstOrDefault().ProductTypeId;
        //        ////////                    cfresult.ProductType = producttypelist.Where(b => b.TypeID == cfresult.ProductTypeId).FirstOrDefault().TypeName;
        //        ////////                    cfresult.ServiceConverted = true;
        //        ////////                    _listFresult.Add(cfresult);
        //        ////////                }
        //        ////////            }
        //        ////////        }
        //        ////////        else
        //        ////////        {

        //        ////////        }
        //        ////////    }
        //        ////////    ///////// 
        //        ////////    #endregion

        //        ////////}
        //        //end test to product

        //        // period++;
        //    }
        //    ss.Stop();
        //}

        private void ReadDatasetconsumptionnew(ForecastInfo finfo, List<MLResponseModel> ds, string Period, int forecastsiteid, List<Site> sites, List<MasterProduct> masterpro, List<ProductType> masterproducttype, List<Test> tests, List<TestingArea> testingAreas)
=======
        private void ReadDatasetconsumptionnew(ForecastInfo finfo, List<MLResponseModel> ds,  string Period,int forecastsiteid,List<Site> sites,List<MasterProduct> masterpro,List<ProductType> masterproducttype,List<Test> tests,List<TestingArea> testingAreas)
>>>>>>> Devops
        {
            Stopwatch ss = new Stopwatch();
            int workingdays = 0, testareaid = 0;
            Site fsite = new Site();

            ss.Start();
            DataTable inputDs = new DataTable();
            inputDs.Columns.Add("X");
            inputDs.Columns.Add("Y");
            DateTime lastDate;
            //} 
            string Producttypename = "";
            /// ProductPrice pp = new ProductPrice();
            MasterProduct MP = new MasterProduct();
            List<ForecastSiteProduct> forecastsiteproduct = new List<ForecastSiteProduct>();
            List<ForecastSiteTest> ForecastSiteTest = new List<ForecastSiteTest>();
            //var sites = ctx.Site.ToList();
            //var masterpro = ctx.MasterProduct.ToList();
            //var masterproducttype = ctx.ProductType.ToList();
            //var tests = ctx.Test.ToList();
            if (finfo.Methodology == "CONSUMPTION")
            {
                forecastsiteproduct = ctx.ForecastSiteProduct.Where(b => b.ForecastSiteID == forecastsiteid).ToList();
            }
            else
            {
                ForecastSiteTest = ctx.ForecastSiteTest.Where(b => b.ForecastSiteID == forecastsiteid).ToList();
            }
            for (int i = 0; i < ds.Count; i++)

            {

                if (finfo.Methodology == "CONSUMPTION")
                {

                    List<ForecastSiteProduct> fsiteProduct = forecastsiteproduct.Where(b => b.ProductID == ds[i].ProductId).OrderBy(x => x.DurationDateTime).ToList();
                    for (int f = 0; f < fsiteProduct.Count; f++)
                    {
                        DataRow Dr = inputDs.NewRow();
                        Dr["X"] = fsiteProduct[f].DurationDateTime;
                        Dr["Y"] = fsiteProduct[f].Adjusted;
                        inputDs.Rows.Add(Dr);
                    }
                    lastDate = fsiteProduct[fsiteProduct.Count - 1].DurationDateTime.Value;
                }
                else
                {
                    List<ForecastSiteTest> fsiteTest = ForecastSiteTest.Where(b => b.TestID == ds[i].TestId).OrderBy(f => f.DurationDateTime).ToList();


                    //  DataTable InputDs = new DataTable();
                    //inputDs.Columns.Add("X");
                    //inputDs.Columns.Add("Y");
                    for (int f = 0; f < fsiteTest.Count; f++)
                    {
                        DataRow Dr = inputDs.NewRow();
                        Dr["X"] = fsiteTest[f].DurationDateTime;
                        Dr["Y"] = fsiteTest[f].Adjusted;
                        inputDs.Rows.Add(Dr);
                    }
                    lastDate = fsiteTest[fsiteTest.Count - 1].DurationDateTime.Value;
                }
                if (ds[i].SiteId > 0)
                {
                    fsite = sites.Where(b => b.SiteID == ds[i].SiteId).FirstOrDefault();
                }
                //if (siteid>0)
                //{period = 0, 

                //      var Forecastcategories = ctx.ForecastCategory.Where(b => b.ForecastId == finfo.ForecastID).ToList();
                //   var forecastsite = ctx.Site.ToList();
                // var tests = ctx.Test.ToList();
                // var testarealist = ctx.TestingArea.ToList();
                if (ds[i].ProductId > 0)
                {
                    MP = masterpro.Where(b => b.ProductID == ds[i].ProductId).FirstOrDefault();
                    //    MP._productPrices = ctx.ProductPrice.Where(b => b.ProductId == proid).ToList();
                    Producttypename = masterproducttype.Where(b => b.TypeID == MP.ProductTypeId).FirstOrDefault().TypeName;

                }
                ForecastCategory fc = new ForecastCategory();
                if (ds[i].CategoryId > 0)
                {
                    fc = ctx.ForecastCategory.Where(b => b.ForecastId == finfo.ForecastID && b.CategoryId == ds[i].CategoryId).FirstOrDefault();
                }

                string testareaname = "";
                if (ds[i].TestId > 0)
                {

                    var testinfo = tests.Join(testingAreas, b => b.TestingAreaID, c => c.TestingAreaID, (b, c) => new { b, c }).Where(z => z.b.TestID == ds[i].TestId).FirstOrDefault();
                    testareaid = testinfo.b.TestingAreaID;
                    testareaname = testinfo.c.AreaName;
                }
                if (finfo.DataUsage == "DATA_USAGE3")
                {
                    int[] sitesid = ctx.ForecastCategorySite.Where(c => c.CategoryID == ds[i].CategoryId).Select(s => s.SiteID).ToArray();
                    workingdays = Convert.ToInt32(sites.Where(b => sitesid.Contains(b.SiteID)).Average(s => s.WorkingDays));


                }

                for (int j = 0; j < ds[i].forecasts.Count; j++)
                {

<<<<<<< HEAD
=======
              
                DateTime ddate = Convert.ToDateTime(ds[i].forecasts[j].datetime);
>>>>>>> Devops

                    DateTime ddate = Convert.ToDateTime(ds[i].forecasts[j].datetime);



<<<<<<< HEAD
                    Finalreslist fresult = new Finalreslist
                    {
                        ForecastId = finfo.ForecastID,
                        SiteId = ds[i].SiteId,
                        CategoryId = ds[i].CategoryId,
                        TestId = ds[i].TestId,
                        ProductId = ds[i].ProductId,
                        DurationDateTime = Utility.Utility.CorrectDateTime(ddate.Date)//ddate.Date;
                    };
=======
                //if (Convert.ToDecimal(ds.Forecasts[i].Forecast) < 0)
                //    fresult.ForecastValue = 0;
                //else
                fresult.ForecastValue = Convert.ToDecimal(ds[i].forecasts[j].forecast) < 0 ? 0 : Convert.ToDecimal(ds[i].forecasts[j].forecast);
>>>>>>> Devops

                    //if (Convert.ToDecimal(ds.Forecasts[i].Forecast) < 0)
                    //    fresult.ForecastValue = 0;
                    //else
                    fresult.ForecastValue = Convert.ToDecimal(ds[i].forecasts[j].forecast) < 0 ? 0 : Convert.ToDecimal(ds[i].forecasts[j].forecast);



                    fresult.IsHistory = ddate > lastDate ? false : true;

                    if (i < inputDs.Rows.Count)
                    {
                        DataRow rowinput = inputDs.Rows[i];

                        fresult.HistoricalValue = ddate > lastDate ? 0 : Convert.ToDecimal(rowinput[1]);
                    }
                    else
                    {
                        fresult.HistoricalValue = 0;
                    }
                    //    fresult.HistoricalValue = Convert.ToDecimal(rowinput[1]);
                    //    fresult.IsHistory = true;
                    //}

                    //if (Period == "Monthly" || Period == "Bimonthly")
                    //    fresult.Duration = String.Format("{0}-{1}", Utility.Utility.Months[fresult.DurationDateTime.Month - 1], fresult.DurationDateTime.Year);              
                    //else
                    //    fresult.Duration = String.Format("{0}", fresult.DurationDateTime.Year);
                    fresult.Duration = Period == "Monthly" || Period == "Bimonthly" ? String.Format("{0}-{1}", Utility.Utility.Months[fresult.DurationDateTime.Month - 1], fresult.DurationDateTime.Year) : String.Format("{0}", fresult.DurationDateTime.Year);

                    fresult.ServiceConverted = false;

                    //Site fsite = null;
                    //if (fresult.SiteId > 0)
                    //    fsite = forecastsite.Where(b => b.SiteID == fresult.SiteId).FirstOrDefault();

                    //ForecastCategory fc = null;
                    //if (fresult.CategoryId > 0)
                    //    fc = Forecastcategories.Where(b => b.CategoryId == fresult.CategoryId).FirstOrDefault();



                    #region Forecast Period Conversion
                    //if (finfo.DataUsage != "DATA_USAGE3")
                    //{
                    //    workingdays = fsite.Single().WorkingDays;   

                    //}
                    workingdays = finfo.DataUsage != "DATA_USAGE3" ? fsite.WorkingDays : 0;

                    string fprd = Period;
                    decimal fPinDay = 0, fPinWeek = 0, fPinMonth = 0, fPinQuarter = 0, fPinYear = 0;
                    switch (fprd)
                    {
                        case "Yearly":
                            fresult.FPinDay = workingdays * 12;
                            fresult.FPinMonth = 12;
                            fresult.FPinWeek = (workingdays / 4) * 12;
                            fresult.FPinQuarter = 4;
                            fresult.FPinYear = 1;
                            break;
                        case "Bimonthly":
                            fresult.FPinDay = workingdays * 2;
                            fresult.FPinMonth = 2;
                            fresult.FPinWeek = (workingdays / 4) * 2;
                            fresult.FPinQuarter = 2 / 3;
                            fresult.FPinYear = 1 / 6;
                            break;
                        case "Monthly":
                            fresult.FPinDay = workingdays;
                            fresult.FPinMonth = 1;
                            fresult.FPinWeek = (workingdays / 4);
                            fresult.FPinQuarter = 1 / 3;
                            fresult.FPinYear = 1 / 12;
                            break;
                        default:
                            break;
                    }

                    #endregion
                    fresult.TestingArea = testareaname;

                    // }

                    int Nopack = int.Parse(decimal.Round(fresult.ForecastValue, 0).ToString());

                    //if (Nopack < fresult.ForecastValue)
                    //    Nopack = Nopack + 1;
                    //if (Nopack == 0)
                    //    Nopack = 0;
                    fresult.Noofpack = Nopack < fresult.ForecastValue ? Nopack + 1 : Nopack;
                    fresult.TotalValue = Nopack < fresult.ForecastValue ? Nopack + 1 : Nopack;
                    fresult.ProductType = Producttypename;
                    _Finalreslist.Add(fresult);
                }
                //get packQty
                ////////if (fresult.ProductId > 0)
                ////////{
                ////////    //MasterProduct p = Masterproduct.Where(b => b.ProductID == fresult.ProductId).FirstOrDefault();
                ////////    //p._productPrices = productpriceList.Where(b => b.ProductId == fresult.ProductId).ToList();
                ////////    //converting quantity to packsize commented out
                ////////    //int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).PackSize;
                ////////    //fresult.PackQty = GetNoofPackage(packSize, fresult.TotalValue);

                ////////    //rounding forecasted pack quantity

                ////////    fresult.PackQty = Nopack;

                ////////    fresult.PackPrice = fresult.PackQty * MP.GetActiveProductPrice(fresult.DurationDateTime).packcost;

                ////////    fresult.ProductTypeId = MP.ProductTypeId;
                ////////    fresult.ProductType = Producttypename;
                ////////    _listFresult.Add(fresult);

                ////////}



                //////////test to product
                ////////if (fresult.TestId > 0)
                ////////{
                ////////    //Test test = ctx.Test.Find(fresult.TestId);

                ////////    #region Forecast General Consumables


                ////////    //     IList<ForecastedResult> _consumablesDailyFlist = new List<ForecastedResult>();

                ////////    var filterbytestidconsumusage = ForecastConsumableUsagelist.Where(b => b.TestId == fresult.TestId).ToList();
                ////////    //   foreach (ForecastConsumableUsage cusage in filterbytestidconsumusage)//DataRepository.GetConsumableUsageByTestId(fresult.TestId))
                ////////    for (int j = 0; j < filterbytestidconsumusage.Count; j++)

                ////////    {
                ////////        //
                ////////        ForecastedResult consumableFresult = new ForecastedResult();
                ////////        consumableFresult = fresult;
                ////////        //copyvalues
                ////////        //consumableFresult.ForecastId = fresult.ForecastId;
                ////////        //consumableFresult.TestId = fresult.TestId;
                ////////        //consumableFresult.DurationDateTime = fresult.DurationDateTime;
                ////////        //consumableFresult.SiteId = fresult.SiteId;
                ////////        //consumableFresult.CategoryId = fresult.CategoryId;
                ////////        //consumableFresult.Duration = fresult.Duration;
                ////////        //consumableFresult.IsHistory = fresult.IsHistory;
                ////////        //consumableFresult.TestingArea = fresult.TestingArea;
                ////////        //consumableFresult.ForecastValue = fresult.ForecastValue;
                ////////        //consumableFresult.TotalValue = fresult.TotalValue;
                ////////        //endcopy
                ////////        decimal Qty = 0;


                ////////        if (filterbytestidconsumusage[j].PerInstrument)
                ////////        {
                ////////            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////            {
                ////////                if (fsite != null)
                ////////                {
                ////////                    //     int siteins = 0;//forecastins.Where(b => b.InsID == filterbytestidconsumusage[j].InstrumentId).FirstOrDefault().Quantity;
                ////////                    var siteins = forecastins.FirstOrDefault(e => e.InsID == filterbytestidconsumusage[j].InstrumentId);
                ////////                    if (siteins != null)
                ////////                    {
                ////////                        switch (filterbytestidconsumusage[j].Period)
                ////////                        {
                ////////                            case "Daily":
                ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
                ////////                                break;
                ////////                            case "Weekly":
                ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
                ////////                                break;
                ////////                            case "Monthly":
                ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
                ////////                                break;
                ////////                            case "Quarterly":
                ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
                ////////                                break;
                ////////                            case "Yearly":
                ////////                                Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
                ////////                                break;
                ////////                            default:
                ////////                                break;
                ////////                        }

                ////////                        Qty = Qty * siteins.Quantity;
                ////////                    }
                ////////                }
                ////////            }
                ////////        }
                ////////        if (filterbytestidconsumusage[j].PerPeriod)
                ////////        {
                ////////            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////            {
                ////////                if (fsite != null)
                ////////                {

                ////////                    switch (filterbytestidconsumusage[j].Period)
                ////////                    {
                ////////                        case "Daily":
                ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
                ////////                            break;
                ////////                        case "Weekly":
                ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
                ////////                            break;
                ////////                        case "Monthly":
                ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
                ////////                            break;
                ////////                        case "Quarterly":
                ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
                ////////                            break;
                ////////                        case "Yearly":
                ////////                            Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
                ////////                            break;
                ////////                        default:
                ////////                            break;
                ////////                    }


                ////////                    //if (filterbytestidconsumusage[j].Period == "Daily")
                ////////                    //{
                ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinDay;
                ////////                    //}
                ////////                    //if (filterbytestidconsumusage[j].Period == "Weekly")
                ////////                    //{
                ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinWeek;
                ////////                    //}
                ////////                    //if (filterbytestidconsumusage[j].Period == "Monthly")
                ////////                    //{
                ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinMonth;
                ////////                    //}
                ////////                    //if (filterbytestidconsumusage[j].Period == "Quarterly")
                ////////                    //{
                ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinQuarter;
                ////////                    //}
                ////////                    //if (filterbytestidconsumusage[j].Period == "Yearly")
                ////////                    //{
                ////////                    //    Qty = filterbytestidconsumusage[j].UsageRate * fPinYear;
                ////////                    //}
                ////////                }
                ////////            }
                ////////        }
                ////////        if (filterbytestidconsumusage[j].PerTest)
                ////////        {
                ////////            if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////            {
                ////////                if (fsite != null)
                ////////                {
                ////////                    Qty = filterbytestidconsumusage[j].UsageRate * (fresult.TotalValue / filterbytestidconsumusage[j].NoOfTest);
                ////////                }
                ////////            }
                ////////        }

                ////////        consumableFresult.TotalValue = Qty;
                ////////        MasterProduct MP1 = new MasterProduct();
                ////////        MP1 = Masterproduct.Where(b => b.ProductID == filterbytestidconsumusage[j].ProductId).FirstOrDefault();
                ////////        MP1._productPrices = productpriceList.Where(b => b.ProductId == MP1.ProductID).ToList();
                ////////        int packSize = MP1.GetActiveProductPrice(fresult.DurationDateTime).packsize;
                ////////        consumableFresult.ProductId = filterbytestidconsumusage[j].ProductId;
                ////////        consumableFresult.PackQty = GetNoofPackage(packSize, Qty);
                ////////        consumableFresult.PackPrice = consumableFresult.PackQty * MP1.GetActiveProductPrice(fresult.DurationDateTime).packcost;
                ////////        consumableFresult.ProductTypeId = MP1.ProductTypeId;
                ////////        consumableFresult.ProductType = ctx.ProductType.Find(MP1.ProductTypeId).TypeName;
                ////////        consumableFresult.IsForGeneralConsumable = true;
                ////////        consumableFresult.ServiceConverted = true;
                ////////        _listFresult.Add(consumableFresult);


                ////////    }



                ////////    #endregion

                ////////    #region Forecast Control Test

                ////////    if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////    {
                ////////        if (fsite != null)
                ////////        {
                ////////            //ForecastIns siteins =forecastins.Join(ctx.Instrument, b => b.InsID, c => c.InstrumentID, (b, c) => new { b, c })
                ////////            //    .Where(x => x.c.testingArea.TestingAreaID == testareaid)
                ////////            //    .Select(s => new ForecastIns
                ////////            //    {

                ////////            //        ID = s.b.ID,
                ////////            //        InsID = s.b.InsID
                ////////            //    }).FirstOrDefault();
                ////////            //fsite.GetSiteInstrumentByTA(test.TestingArea.Id);
                ////////            if (forecastins != null)
                ////////            {
                ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
                ////////                {
                ////////                    fresult.ControlTest = fPinDay * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.DailyCtrlTest).FirstOrDefault();
                ////////                }
                ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
                ////////                {
                ////////                    fresult.ControlTest = fPinWeek * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
                ////////                }
                ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
                ////////                {
                ////////                    fresult.ControlTest = fPinMonth * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();

                ////////                }
                ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
                ////////                {
                ////////                    fresult.ControlTest = fPinQuarter * forecastins.FirstOrDefault().Quantity * inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
                ////////                }
                ////////                if (inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
                ////////                {
                ////////                    fresult.ControlTest = ((fresult.TotalValue * (forecastins.FirstOrDefault().TestRunPercentage / 100)) / inslist.Where(s => s.InstrumentID == forecastins.FirstOrDefault().InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
                ////////                }
                ////////            }
                ////////        }
                ////////    }

                ////////    #endregion

                ////////    #region Test Test to Product
                ////////    var productusage = ForecastProductUsage.Where(b => b.TestId == testid && b.IsForControl == false).ToList();
                ////////    // foreach (ForecastProductUsage pu in productusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
                ////////    for (int ki = 0; ki < productusage.Count; ki++)

                ////////    {
                ////////        ForecastedResult cfresult = new ForecastedResult
                ////////        {
                ////////            //copyvalues
                ////////            ForecastId = fresult.ForecastId,
                ////////            TestId = fresult.TestId,
                ////////            DurationDateTime = fresult.DurationDateTime,
                ////////            SiteId = fresult.SiteId,
                ////////            CategoryId = fresult.CategoryId,
                ////////            Duration = fresult.Duration,
                ////////            IsHistory = fresult.IsHistory,
                ////////            TestingArea = fresult.TestingArea
                ////////        };
                ////////        //endcopy

                ////////        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////        {
                ////////            if (fsite != null)
                ////////            {
                ////////                ForecastIns siteins = ctx.ForecastIns.Where(b => b.InsID == productusage[ki].InstrumentId && b.forecastID == fresult.ForecastId).FirstOrDefault();
                ////////                if (siteins != null)
                ////////                {
                ////////                    decimal Qty = productusage[ki].Rate * fresult.TotalValue * siteins.TestRunPercentage / 100;
                ////////                    cfresult.TotalValue = Qty;
                ////////                    MasterProduct p = Masterproduct.Where(b => b.ProductID == productusage[ki].ProductId).FirstOrDefault();
                ////////                    p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
                ////////                    int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).packsize;
                ////////                    cfresult.ProductId = productusage[ki].ProductId;
                ////////                    cfresult.PackQty = GetNoofPackage(packSize, Qty);
                ////////                    cfresult.PackPrice = cfresult.PackQty * p.GetActiveProductPrice(fresult.DurationDateTime).packcost;

                ////////                    cfresult.ProductTypeId = Masterproduct.Where(b => b.ProductID == productusage[ki].ProductId).FirstOrDefault().ProductTypeId;
                ////////                    cfresult.ProductType = producttypelist.Where(b => b.TypeID == cfresult.ProductTypeId).FirstOrDefault().TypeName;
                ////////                    cfresult.ServiceConverted = true;
                ////////                    _listFresult.Add(cfresult);
                ////////                }
                ////////            }
                ////////        }
                ////////        else
                ////////        {
                ////////            //if (fsite != null)
                ////////            //{
                ////////            //    ForecastCategoryInstrument fcins = DataRepository.GetForecastCategoryInstrumentById(pu.Instrument.Id);

                ////////            //    if (fcins != null)
                ////////            //    {
                ////////            //        decimal Qty = pu.Rate * fresult.TotalValue * fcins.TestRunPercentage;
                ////////            //        cfresult.TotalValue = Qty;
                ////////            //        int packSize = pu.Product.GetActiveProductPrice(fresult.DurationDateTime).PackSize;
                ////////            //        cfresult.ProductId = pu.Product.Id;
                ////////            //        cfresult.PackQty = GetNoofPackage(packSize, Qty);
                ////////            //        cfresult.PackPrice = cfresult.PackQty * pu.Product.GetActiveProductPrice(fresult.DurationDateTime).Price;

                ////////            //        cfresult.ProductTypeId = pu.Product.ProductType.Id;
                ////////            //        cfresult.ProductType = pu.Product.ProductType.TypeName;
                ////////            //        cfresult.ServiceConverted = true;
                ////////            //        _listFresult.Add(cfresult);
                ////////            //    }
                ////////            //}
                ////////        }
                ////////    }
                ////////    ///////// 
                ////////    #endregion

                ////////    #region Control Test to Product


                ////////    var cproductusage = ForecastProductUsage.Where(b => b.TestId == testid && b.IsForControl == true).ToList();
                ////////    // foreach (ForecastProductUsage pu in cproductusage) //change on aug 22.2014 (ProductUsage pu in test.ProductUsages)
                ////////    for (int z = 0; z < cproductusage.Count; z++)

                ////////    {
                ////////        ForecastedResult cfresult = new ForecastedResult();
                ////////        cfresult = fresult;
                ////////        //copyvalues
                ////////        //cfresult.ForecastId = fresult.ForecastId;
                ////////        //cfresult.TestId = fresult.TestId;
                ////////        //cfresult.DurationDateTime = fresult.DurationDateTime;
                ////////        //cfresult.SiteId = fresult.SiteId;
                ////////        //cfresult.CategoryId = fresult.CategoryId;
                ////////        //cfresult.Duration = fresult.Duration;
                ////////        //cfresult.IsHistory = fresult.IsHistory;
                ////////        //cfresult.TestingArea = fresult.TestingArea;

                ////////        cfresult.IsForControl = true;
                ////////        //endcopy

                ////////        if (finfo.DataUsage == "DATA_USAGE1" || finfo.DataUsage == "DATA_USAGE2")
                ////////        {
                ////////            if (fsite != null)
                ////////            {
                ////////                ForecastIns siteins = forecastins.Where(b => b.InsID == cproductusage[z].InstrumentId).FirstOrDefault();
                ////////                if (siteins != null)
                ////////                {


                ////////                    decimal Qty = 0;
                ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.DailyCtrlTest).FirstOrDefault() > 0)
                ////////                    {
                ////////                        Qty = cproductusage[z].Rate * fPinDay * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.DailyCtrlTest).FirstOrDefault();
                ////////                        // cfresult.ForecastValue = fPinDay * siteins.Quantity * siteins.Instrument.DailyCtrlTest;
                ////////                    }
                ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault() > 0)
                ////////                    {
                ////////                        Qty = cproductusage[z].Rate * fPinWeek * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.WeeklyCtrlTest).FirstOrDefault();
                ////////                        // cfresult.ForecastValue = fPinWeek * siteins.Quantity * siteins.Instrument.WeeklyCtrlTest;
                ////////                    }
                ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault() > 0)
                ////////                    {
                ////////                        Qty = cproductusage[z].Rate * fPinMonth * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MonthlyCtrlTest).FirstOrDefault();
                ////////                        // cfresult.ForecastValue = fPinMonth * siteins.Quantity * siteins.Instrument.MonthlyCtrlTest;

                ////////                    }
                ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault() > 0)
                ////////                    {
                ////////                        Qty = cproductusage[z].Rate * fPinQuarter * siteins.Quantity * inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.QuarterlyCtrlTest).FirstOrDefault();
                ////////                        //cfresult.ForecastValue = fPinQuarter * siteins.Quantity * siteins.Instrument.QuarterlyCtrlTest;
                ////////                    }
                ////////                    if (inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault() > 0)
                ////////                    {
                ////////                        Qty = cproductusage[z].Rate * ((fresult.TotalValue * (siteins.TestRunPercentage / 100)) / inslist.Where(s => s.InstrumentID == siteins.InsID).Select(s => s.MaxTestBeforeCtrlTest).FirstOrDefault());
                ////////                        // cfresult.ForecastValue = ((fresult.TotalValue * (siteins.TestRunPercentage / 100)) / siteins.Instrument.MaxTestBeforeCtrlTest);
                ////////                    }


                ////////                    //decimal Qty = pu.Rate * fresult.TotalValue * siteins.TestRunPercentage / 100;

                ////////                    cfresult.TotalValue = Qty;
                ////////                    MasterProduct p = Masterproduct.Where(b => b.ProductID == cproductusage[z].ProductId).FirstOrDefault();
                ////////                    p._productPrices = productpriceList.Where(b => b.ProductId == p.ProductID).ToList();
                ////////                    int packSize = p.GetActiveProductPrice(fresult.DurationDateTime).packsize;
                ////////                    cfresult.ProductId = cproductusage[z].ProductId;
                ////////                    cfresult.PackQty = GetNoofPackage(packSize, Qty);
                ////////                    cfresult.PackPrice = cfresult.PackQty * p.GetActiveProductPrice(fresult.DurationDateTime).packcost;

                ////////                    cfresult.ProductTypeId = Masterproduct.Where(b => b.ProductID == cproductusage[z].ProductId).FirstOrDefault().ProductTypeId;
                ////////                    cfresult.ProductType = producttypelist.Where(b => b.TypeID == cfresult.ProductTypeId).FirstOrDefault().TypeName;
                ////////                    cfresult.ServiceConverted = true;
                ////////                    _listFresult.Add(cfresult);
                ////////                }
                ////////            }
                ////////        }
                ////////        else
                ////////        {

                ////////        }
                ////////    }
                ////////    ///////// 
                ////////    #endregion

                ////////}
                //end test to product

                // period++;
            }
            ss.Stop();
        }

        private int GetNoofPackage(int packSize, decimal noofproduct)
        {
            int Nopack;
            decimal Result;
            if (packSize == 0)
                Result = noofproduct;
            else
                Result = noofproduct / packSize;

            Nopack = int.Parse(decimal.Round(Result, 0).ToString());

            if (Nopack < Result)
                Nopack = Nopack + 1;
            if (Nopack == 0)
                Nopack = 0;

            return Nopack;
        }
        public IList<ConsumableUsage> GetAllConsumableUsageByTestId(int testId)
        {
            IList<MasterConsumable> _mainConsumableList = ctx.MasterConsumable.ToList();
            IList<ConsumableUsage> _cUsageList = new List<ConsumableUsage>();
            foreach (MasterConsumable mc in _mainConsumableList)
            {

                if (mc.TestId == testId)
                {
                    var ConsumableUsages = ctx.ConsumableUsage.Where(b => b.ConsumableId == mc.MasterCID).ToList();
                    foreach (ConsumableUsage cu in ConsumableUsages)
                        _cUsageList.Add(cu);
                }

            }
            return _cUsageList;
        }




        public IList<ForecastConsumableUsage> GetAllserviceConsumableUsageByTestId(int testId, int forecastid)
        {
            //  IList<MasterConsumable> _mainConsumableList = ctx.MasterConsumable.ToList();
            IList<ForecastConsumableUsage> _cUsageList = new List<ForecastConsumableUsage>();

            var ConsumableUsages = ctx.ForecastConsumableUsage.Where(b => b.TestId == testId && b.Forecastid == forecastid).ToList();
            foreach (ForecastConsumableUsage cu in ConsumableUsages)
                _cUsageList.Add(cu);

            return _cUsageList;
        }
        public DataTable Calculateforecastmultiplemethod(DataTable inputds, ForecastInfo forecastInfo)
        {
            DataTable ds = new DataTable();
            double a = 0;
            double b = 0;
            double sigmaxy = 0;
            double averagex = 0;
            double averagey = 0;
            double xsqr = 0;
            double totalx = 0;
            double totaly = 0;
            ds.Columns.Add("0");
            ds.Columns.Add("1");
            if (param1[0] == "Specifiedpercentagegrowth")
            {
                for (int i = 0; i < inputds.Rows.Count; i++)
                {
                    DataRow Dr = ds.NewRow();
                    Dr[0] = Convert.ToDateTime(inputds.Rows[i][0]);
                    Dr[1] = Convert.ToDecimal(inputds.Rows[i][1]) + (Convert.ToDecimal(inputds.Rows[i][1]) * (Convert.ToDecimal(param1[2]) / 100));
                    ds.Rows.Add(Dr);
                }

            }
            else if (param1[0] == "Simplemovingaverage")
            {
                if (inputds.Rows.Count > 3)
                {
                    for (int i = 0; i < inputds.Rows.Count; i++)
                    {
                        DataRow Dr = ds.NewRow();
                        Dr[0] = Convert.ToDateTime(inputds.Rows[i][0]);
                        if (param1[3] == "3")
                            Dr[1] = (Convert.ToDecimal(inputds.Rows[inputds.Rows.Count - 1][1]) + Convert.ToDecimal(inputds.Rows[inputds.Rows.Count - 2][1]) + Convert.ToDecimal(inputds.Rows[inputds.Rows.Count - 3][1])) / 3;
                        else
                            Dr[1] = (Convert.ToDecimal(inputds.Rows[inputds.Rows.Count - 1][1]) + Convert.ToDecimal(inputds.Rows[inputds.Rows.Count - 2][1]) + Convert.ToDecimal(inputds.Rows[inputds.Rows.Count - 3][1]) + Convert.ToDecimal(inputds.Rows[inputds.Rows.Count - 4][1]) + Convert.ToDecimal(inputds.Rows[inputds.Rows.Count - 5][1])) / 5;
                        ds.Rows.Add(Dr);
                    }
                }
                else
                {
                    ds = inputds;
                }

            }

            else if (param1[0] == "Weightedmovingaverage")
            {
                if (inputds.Rows.Count > 3)
                {
                    for (int i = 0; i < inputds.Rows.Count; i++)
                    {
                        DataRow Dr = ds.NewRow();
                        Dr[0] = Convert.ToDateTime(inputds.Rows[i][0]);
                        if (param1[3] == "3")
                            Dr[1] = ((Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 1][1]) * 0.6) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 2][1]) * 0.3) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 3][1]) * 0.1));
                        else
                            Dr[1] = ((Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 1][1]) * 0.5) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 2][1]) * 0.2) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 3][1]) * 0.1) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 4][1]) * 0.1) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 5][1]) * 0.1));
                        ds.Rows.Add(Dr);
                    }
                }
                else
                {
                    ds = inputds;
                }
            }
            else if (param1[0] == "Simplelinearregression")
            {
                double xy = 0;
                for (int i = 0; i < inputds.Rows.Count; i++)
                {
                    //DataRow Dr = ds.NewRow();
                    //Dr[0] = Convert.ToDateTime(inputds.Rows[i][0]);
                    //Dr[1] = Convert.ToDouble(inputds.Rows[i][1]);
                    //ds.Rows.Add(Dr);

                    xy = (Convert.ToDateTime(inputds.Rows[i][0]).Month) * Convert.ToDouble(inputds.Rows[i][1]);
                    sigmaxy = sigmaxy + xy;

                    totalx = totalx + Convert.ToDateTime(inputds.Rows[i][0]).Month;
                    totaly = totaly + Convert.ToDouble(inputds.Rows[i][1]);
                    xsqr = xsqr + (Convert.ToDateTime(inputds.Rows[i][0]).Month * Convert.ToDateTime(inputds.Rows[i][0]).Month);
                    //if (param1[3] == "3")

                    //    Dr[1] = ((Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 1][1]) * 0.6) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 2][1]) * 0.3) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 3][1]) * 0.1));
                    //else
                    //    Dr[1] = ((Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 1][1]) * 0.5) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 2][1]) * 0.2) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 3][1]) * 0.1) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 4][1]) * 0.1) + (Convert.ToDouble(inputds.Rows[inputds.Rows.Count - 5][1]) * 0.1));

                }
                if (inputds.Rows.Count > 0)
                {
                    totalx = totalx / inputds.Rows.Count;
                    totaly = totaly / inputds.Rows.Count;
                    b = (sigmaxy - (inputds.Rows.Count * totalx * totaly)) / (xsqr - (inputds.Rows.Count * totalx * totalx));
                    a = totaly - (b * totalx);
                }
                for (int i = 0; i < inputds.Rows.Count; i++)
                {
                    DataRow Dr = ds.NewRow();
                    Dr[0] = Convert.ToDateTime(inputds.Rows[i][0]);
                    Dr[1] = Math.Round(a + Convert.ToDateTime(inputds.Rows[i][0]).Month * b, 2);
                    ds.Rows.Add(Dr);
                }
            }

            //  ds = inputds;
            int ext;

            int interval = 0;
            if (forecastInfo.Period == "Monthly")
            {
                interval = 1;
            }
            else if (forecastInfo.Period == "Bimonthly")
            {
                interval = 2;
            }
            else if (forecastInfo.Period == "Quarterly")
            {
                interval = 3;
            }
            else
            {

                interval = 12;
            }
            ext = forecastInfo.Extension;
            if (param1[0] == "Specifiedpercentagegrowth")
            {
                while (ext > 0)
                {
                    DataRow Dr = ds.NewRow();
                    Dr[0] = Convert.ToDateTime(ds.Rows[ds.Rows.Count - 1][0]).AddMonths(interval);
                    Dr[1] = Convert.ToDecimal(ds.Rows[ds.Rows.Count - 1][1]) + (Convert.ToDecimal(ds.Rows[ds.Rows.Count - 1][1]) * (Convert.ToDecimal(param1[2]) / 100));
                    ds.Rows.Add(Dr);
                    ext--;
                }
            }
            else if (param1[0] == "Simplemovingaverage")
            {
                while (ext > 0)
                {
                    DataRow Dr = ds.NewRow();
                    Dr[0] = Convert.ToDateTime(ds.Rows[ds.Rows.Count - 1][0]).AddMonths(interval);
                    if (param1[3] == "3")
                        Dr[1] = (Convert.ToDecimal(ds.Rows[ds.Rows.Count - 1][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 2][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 3][1])) / 3;
                    else
                        Dr[1] = (Convert.ToDecimal(ds.Rows[ds.Rows.Count - 1][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 2][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 3][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 4][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 5][1])) / 3;
                    ds.Rows.Add(Dr);
                    ext--;
                }
            }
            else if (param1[0] == "Simplelinearregression")
            {
                while (ext > 0)
                {
                    DataRow Dr = ds.NewRow();
                    Dr[0] = Convert.ToDateTime(ds.Rows[ds.Rows.Count - 1][0]).AddMonths(interval);
                    // if (param1[3] == "3")
                    Dr[1] = Math.Round(a + (ds.Rows.Count + interval) * b, 2); //(Convert.ToDecimal(ds.Rows[ds.Rows.Count - 1][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 2][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 3][1])) / 3;
                    //else
                    //    Dr[1] = (Convert.ToDecimal(ds.Rows[ds.Rows.Count - 1][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 2][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 3][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 4][1]) + Convert.ToDecimal(ds.Rows[ds.Rows.Count - 5][1])) / 3;
                    ds.Rows.Add(Dr);
                    ext--;
                }
            }
            else if (param1[0] == "Weightedmovingaverage")
            {
                while (ext > 0)
                {
                    DataRow Dr = ds.NewRow();
                    Dr[0] = Convert.ToDateTime(ds.Rows[ds.Rows.Count - 1][0]).AddMonths(interval);
                    if (param1[3] == "3")
                        Dr[1] = ((Convert.ToDouble(ds.Rows[ds.Rows.Count - 1][1]) * 0.6) + (Convert.ToDouble(ds.Rows[ds.Rows.Count - 2][1]) * 0.3) + (Convert.ToDouble(ds.Rows[ds.Rows.Count - 3][1]) * 0.1));
                    else
                        Dr[1] = ((Convert.ToDouble(ds.Rows[ds.Rows.Count - 1][1]) * 0.5) + (Convert.ToDouble(ds.Rows[ds.Rows.Count - 2][1]) * 0.2) + (Convert.ToDouble(ds.Rows[ds.Rows.Count - 3][1]) * 0.1) + (Convert.ToDouble(ds.Rows[ds.Rows.Count - 4][1]) * 0.1) + (Convert.ToDouble(ds.Rows[ds.Rows.Count - 5][1]) * 0.1));
                    ds.Rows.Add(Dr);
                    ext--;
                }
            }

            return ds;
        }

    }
}
