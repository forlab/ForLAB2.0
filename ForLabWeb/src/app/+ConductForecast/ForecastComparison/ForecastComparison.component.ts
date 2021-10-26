import { Component, OnInit, EventEmitter, Output } from '@angular/core';

import * as Highcharts from 'highcharts';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { APIwithActionService } from '../../shared/APIwithAction.service';
import { GlobalAPIService } from '../../shared/GlobalAPI.service';

@Component({
    selector: 'app-forecast-comparison',
    templateUrl: './ForecastComparison.component.html',
    styleUrls: ['ForecastComparison.component.css']
})

export class ForecastComparisonComponent implements OnInit {
    comparisonData = new Array();
    seriesData = new Array();
    region = new Array();

    constructor(private _fb: FormBuilder, private _avRoute: ActivatedRoute,
        private _router: Router, private _APIwithActionService: APIwithActionService) {
    }
    ngAfterViewChecked() {
    }
    ngOnInit() {
        this.comparisonData = [
            { name: "Forecast 2", y: 40, color: "#EFBF20" },
            { name: "Forecast 3", y: 25, color: "#9229E2" },
            { name: "Forecast 1", y: 35, color: "#E53560" },
        ];
        this.seriesData = [{
            name: 'Forecast 1',
            data: [4393, 5250, 5717, 6965, 7703, 7993, 6713, 5417, 6703, 7993, 8713, 7417],
            color: "#EFBF20"
        }, {
            name: 'Forecast 2',
            data: [2491, 2406, 2974, 2981, 3290, 3282, 3812, 4434, 5406, 6742, 7985, 9249],
            color: "#9229E2"
        }, {
            name: 'Forecast 3',
            data: [1174, 1772, 1600, 1977, 2018, 2437, 6214, 5938, 6018, 7437, 6214, 7938],
            color: "#E53560"
        }]
        this.region = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        this.onDrawCharts();

        // this._APIwithActionService.getDatabyID(246, 'Dasboard', 'Getforecastcomparision').subscribe((data) => {

        // })
    }

    onDrawCharts() {
        Highcharts.chart('comparison-line-chart', {
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


        Highcharts.chart('comparison-pie-chart', {
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
                        var text = this.renderer.html('<h1 id="total_pie" style="font-weight:bold; letter-spacing: 0.1em; color: #E53560">' + total + '%</h1>').add();
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
                data: this.comparisonData,
                innerSize: '50%',
                size: 180
            }]
        });
    }

}


