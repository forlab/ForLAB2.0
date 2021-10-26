import { Component, OnInit, EventEmitter, Output, ElementRef, ViewChild } from '@angular/core';

import * as Highcharts from 'highcharts';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { APIwithActionService } from '../../shared/APIwithAction.service';
import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import * as XLSX from 'xlsx';
import * as jsPDF from 'jspdf';
import 'jspdf-autotable'
import { toXML } from 'jstoxml';


@Component({
    selector: 'app-forecast-report',
    templateUrl: './ForecastReport.component.html',
    styleUrls: ['ForecastReport.component.css']
})

export class ForecastReportComponent implements OnInit {
    pieData = new Array();
    seriesData = new Array();
    region = new Array();
    forecastID: number;
    titleoflinechart: string = "";
    titleofpiechart: string = "";
    forecastTitle = "";
    finalCost = "0";
    QCcost = "";
    Ccost = "";
    testcost = "";
    forecastPeriod = "";
    productsList = new Array();
    colorList = ["#E53560", "#9229E2", "#EFBF20", "#1BB192", "#052644", "#00BAF1", "#EA1455", "#00B08E"];
    htmlToDownload: any;

    constructor(private _avRoute: ActivatedRoute, private _router: Router, private _APIwithActionService: APIwithActionService) {
        if (this._avRoute.snapshot.params["id"]) {
            this.forecastID = this._avRoute.snapshot.params["id"];
        }

    }
    Print(): void {
        let printContents, popupWin;
        printContents = document.getElementById('conduct-forecast-contents').innerHTML;
        popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
        popupWin.document.open();
        popupWin.document.write(`
          <html>
            <head>
              <title>Forecast Comparision</title>
              
            </head>
        <body onload="window.print();window.close()">${printContents}</body>
          </html>`
        );
        popupWin.document.close();
    }
    onExportExcel() {
        let element = document.getElementById('report-table');
        const ws: XLSX.WorkSheet = XLSX.utils.table_to_sheet(element);
        const wb: XLSX.WorkBook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
        XLSX.writeFile(wb, 'report(' + this.forecastTitle + ').xlsx');
    }

    onExportPDF() {
        var pdfList = new Array();
        this.productsList.forEach(element => {
            var temp = new Array();
            temp.push(element["productName"]);
            temp.push(element["productType"]);
            temp.push(element["productNo"]);
            temp.push(element["productPrice"]);
            pdfList.push(temp);
        })
        var doc = new jsPDF();
        doc.autoTable({
            head: [['Product Name', 'Product Type', 'Quantity', 'Price USD']],
            body: pdfList
        });
        doc.save('report(' + this.forecastTitle + ').pdf');
    }
    onExportXML() {
        const data = toXML(this.productsList);
        const blob = new Blob([data], { type: 'text/xml' });
        var fileUrl = URL.createObjectURL(blob);
        var element = document.createElement('a');
        element.href = fileUrl;
        element.setAttribute('download', 'report(' + this.forecastTitle + ').xml');
        document.body.appendChild(element);
        element.click();
    }
    ngOnInit() {

        //color list: #EFBF20, #9229E2, #E53560
        this.onDrawCharts();
        this._APIwithActionService.getDatabyID(this.forecastID, "Forecsatinfo", "GetbyId").subscribe((respMethod) => {
            if (respMethod.methodology == "SERVICE STATSTICS" || respMethod.methodology == "CONSUMPTION") {
                this._APIwithActionService.getDatabyID(this.forecastID, "Report", "Getconsumptionsummary").subscribe((resp) => {
                    this.forecastTitle = resp.title;
                    this.forecastPeriod = resp.forecastperiod;
                    this.finalCost = resp.finalcost;
                    for (var idx = 0; idx < resp.data.length; idx++) {
                        this.productsList.push({
                            productName: resp.data[idx]["product name"],
                            productType: resp.data[idx]["product type"],
                            productNo: resp.data[idx]["total no of product"],
                            productPrice: resp.data[idx]["total price"],
                            percentage: Math.round(+(resp.data[idx]["total price"].replace(/,/g, '')) * 1000 / +(this.finalCost.replace(/,/g, ''))) / 10
                        })
                    }
                });
            } else {
                this._APIwithActionService.getDatabyID(this.forecastID, "Report", "Getdemographicsummary").subscribe((resp) => {
                    this.forecastTitle = resp.title;
                    this.forecastPeriod = resp.forecastperiod;
                    this.finalCost = resp.finalcost;
                    for (var idx = 0; idx < resp.data.length; idx++) {
                        this.productsList.push({
                            productName: resp.data[idx]["product name"],
                            productType: resp.data[idx]["product type"],
                            productNo: resp.data[idx]["total no of product"],
                            productPrice: resp.data[idx]["total price"],
                            percentage: Math.round(+(resp.data[idx]["total price"].replace(/,/g, '')) * 1000 / +(this.finalCost.replace(/,/g, ''))) / 10
                        })
                    }
                });
            }

            if (respMethod.methodology == "SERVICE STATSTICS" || respMethod.methodology == "MORBIDITY") {
                this.titleoflinechart = "Forecast Trend By Testing Area";
                this.titleofpiechart = "Cost By Testing Area";
            }
            else {
                this.titleoflinechart = "Forecast Trend By Product Type";
                this.titleofpiechart = "Cost By Product Type";
            }

        });

        this._APIwithActionService.getDatabyID(this.forecastID, "Conductforecast", "getcostparameter").subscribe((data) => {
            this.QCcost = data.qccost;
            this.Ccost = data.cccost;
            this.testcost = data.testcost;
        });
    }

    onDrawCharts() {
        this._APIwithActionService.getDatabyID(this.forecastID, "Conductforecast", "Getforecastsummarydurationforsitenew").subscribe((resp) => {
            this._APIwithActionService.getDatabyID(this.forecastID, "Conductforecast", "Getdistinctdurationnew").subscribe((resp1) => {
                this.seriesData = new Array();
                for (let idx = 0; idx < resp.length; idx++) {
                    this.seriesData.push({
                        name: resp[idx].name,
                        data: resp[idx].data,
                        color: this.colorList[idx]
                    });
                }

                resp1.forEach((element: any) => {
                    this.region.push(element.duration.substring(0, 1).toUpperCase() + element.duration.substring(1, 3));
                });
                Highcharts.chart('report-line-chart', {
                    legend: {
                        layout: 'horizontal',
                        align: 'center',
                        verticalAlign: 'top'
                    },
                    credits: {
                        enabled: false
                    },
                    title: {
                        text: null
                    },
                    yAxis: {
                        title: null,
                        gridLineDashStyle: 'dot',
                    },
                    xAxis: {
                        categories: this.region,
                        crosshair: true,
                        lineWidth: 0,
                        minorGridLineWidth: 0,
                        minorTickLength: 0,
                        tickLength: 0
                    },
                    plotOptions: {
                        series: {
                            label: {
                                connectorAllowed: false
                            },
                            marker: {
                                enabled: false
                            }
                        },
                    },
                    series: this.seriesData
                });
            })
        })
        this._APIwithActionService.getDatabyID(this.forecastID, "Conductforecast", "GetProducttypecostratioNEW").subscribe((resp) => {
            this.pieData = new Array();
            for (let idx = 0; idx < resp.length; idx++) {
                this.pieData.push({
                    name: resp[idx].name,
                    y: resp[idx].y,
                    color: this.colorList[idx]
                });
            }
            Highcharts.chart('report-pie-chart', {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    type: 'pie',
                    events: {
                        render: function (e) {
                            if (document.getElementById("total_pie")) {
                                document.getElementById("total_pie").parentElement.remove();
                            }
                            var total = 0;
                            for (var i = 0, len = this.series[0].yData.length; i < len; i++) {
                                total += this.series[0].yData[i];
                            }
                            var text = this.renderer.html('<h1 id="total_pie" style="font-weight:bold; letter-spacing: 0.1em; color: #E53560">' + Math.floor(total) + '%</h1>').add();
                            var textBBox = text.getBBox();
                            var x = this.plotLeft + (this.plotWidth * 0.5) - (textBBox.width * 0.5);
                            var y = this.plotTop + (this.plotHeight * 0.5) - (textBBox.height * 0.5);
                            text.attr({ x: x, y: y + 6 });
                        },
                    }
                },
                legend: false,
                credits: {
                    enabled: false
                },
                title: {
                    text: null,
                    style: {
                        fontSize: '14px',
                        fontWeight: 'bold'
                    },
                    verticalAlign: 'bottom'
                },
                tooltip: {
                    pointFormat: '{series.name}: <b>{point.y}</b>'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            useHTML: true,
                            enabled: true,
                            formatter: function () {
                                return '<span style="color:' + this.point.color + '; font-size: 12px;">' + this.point.name + '<br>' + this.point.y + '%</span>';
                            }
                        },
                        showInLegend: true
                    }
                },
                series: [{
                    colorByPoint: true,
                    data: this.pieData,
                    innerSize: '50%',
                    size: 180
                }]
            });
        })

    }

}


