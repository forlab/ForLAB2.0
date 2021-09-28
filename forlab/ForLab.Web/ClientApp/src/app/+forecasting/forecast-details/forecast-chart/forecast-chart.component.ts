import { Component, Injector, Input, OnInit, OnChanges, ViewChild } from '@angular/core';
import { map, takeUntil, tap } from 'rxjs/operators';
import { ForecastMorbidityTargetBasesController } from 'src/@core/APIs/ForecastMorbidityTargetBasesController';
import { ForecastMorbidityWhoBasesController } from 'src/@core/APIs/ForecastMorbidityWhoBasesController';
import { ForecastLaboratoryTestServicesController } from 'src/@core/APIs/ForecastLaboratoryTestServicesController';
import { ForecastLaboratoryConsumptionsController } from 'src/@core/APIs/ForecastLaboratoryConsumptionsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { BaseService } from 'src/@core/services/base.service';
import { ForecastMethodologyEnum } from 'src/@core/models/enum/Enums';
import { MatPaginator } from '@angular/material/paginator';
declare const $: any;

@Component({
  selector: 'app-forecast-chart',
  templateUrl: './forecast-chart.component.html',
  styleUrls: ['./forecast-chart.component.scss']
})
export class ForecastChartComponent extends BaseService implements OnInit, OnChanges {
  // Inputs
  @Input('forecastInfoId') forecastInfoId: number;
  @Input('forecastMethodologyId') forecastMethodologyId: number;
  @Input('isTargetBased') isTargetBased: boolean;
  // Children
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  //Drp
  forecastMethodologyEnum = ForecastMethodologyEnum;
  // Vars
  totalCount: number;

  constructor(public injector: Injector) {
    super(injector);
  }

  // Chart
  public chartOptions = {
    responsive: true,
    tooltips: {
      mode: 'index',
      titleFontSize: 12,
      titleFontColor: '#000',
      bodyFontColor: '#000',
      backgroundColor: '#fff',
      cornerRadius: 3,
      intersect: false
    },
    legend: {
      display: false,
      labels: {
        usePointStyle: true
      }
    },
    scales: {
      xAxes: [
        {
          display: true,
          gridLines: {
            display: false,
            drawBorder: false
          },
          scaleLabel: {
            display: false,
            labelString: 'Month'
          },
          ticks: {
            fontColor: '#9aa0ac' // Font Color
          }
        }
      ],
      yAxes: [
        {
          display: true,
          gridLines: {
            display: false,
            drawBorder: false
          },
          scaleLabel: {
            display: true,
            labelString: 'Amount Forecasted'
          },
          ticks: {
            fontColor: '#9aa0ac' // Font Color
          }
        }
      ]
    },
    title: {
      display: false,
      text: 'Normal Legend'
    }
  };
  chartData = [
    {
      label: 'Amount Forecasted',
      data: [],
      borderWidth: 4,
      pointStyle: 'circle',
      pointRadius: 4,
      borderColor: 'rgba(37,188,232,.7)',
      pointBackgroundColor: 'rgba(37,188,232,.2)',
      backgroundColor: 'rgba(37,188,232,.2)',
      pointBorderColor: 'transparent'
    },
  ];
  chartLabels = [];

  // Target Base Chart
  public barChartOptions: any = {
    scaleShowVerticalLines: false,
    responsive: true,
    scales: {
      xAxes: [
        {
          ticks: {
            fontFamily: 'Poppins',
            fontColor: '#9aa0ac' // Font Color
          }
        }
      ],
      yAxes: [
        {
          ticks: {
            beginAtZero: true,
            fontFamily: 'Poppins',
            fontColor: '#9aa0ac' // Font Color
          }
        }
      ]
    }
  };
  public barChartLabels: string[] = [];
  public barChartType = 'bar';
  public barChartLegend = false;
  public barChartData: any[] = [
    { data: [], label: 'Current Patient' },
    { data: [], label: 'Target Patient' }
  ];
  public barChartColors: Array<any> = [
    {
      backgroundColor: 'rgba(211,211,211,1)',
      borderColor: 'rgba(211,211,211,1)',
      pointBackgroundColor: 'rgba(211,211,211,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(211,211,211,0.8)'
    },
    {
      backgroundColor: 'rgba(110, 104, 193, 1)',
      borderColor: 'rgba(110, 104, 193,1)',
      pointBackgroundColor: 'rgba(110, 104, 193,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(110, 104, 193,0.8)'
    }
  ];


  ngOnInit() {

    this.loadChart();

    $('#sparkline').sparkline([5, 6, 7, 2, 0, -4, -2, 4], {
      type: 'bar'
    });
    $('#sparkline2').sparkline([5, 6, 7, 9, 9, 5, 3, 2, 2, 4, 6, 7], {
      type: 'line'
    });
    $('#sparkline3').sparkline([5, 6, 7, 9, 9, 5, 3, 2, 2, 4, 6, 7], {
      type: 'line'
    });
    $('#sparkline4').sparkline([4, 6, 7, 7, 4, 3, 2, 1, 4, 4], {
      type: 'discrete'
    });
    $('#sparkline5').sparkline([1, 1, 2], {
      type: 'pie'
    });
    $('#sparkline6').sparkline([2, -4, 5, 2, 0, 4, -2, 4], {
      type: 'bar'
    });
  }

  ngAfterViewInit() {
    this.loadChart();
    this.paginator.page.pipe(tap(() => this.loadChart())).subscribe();
  }

  ngOnChanges() {
    this.loadChart();
  }

  loadChart() {
    if (!this.url) {
      return;
    }

    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: this.paginator?.pageSize || 10 },
      { key: 'pageIndex', value: this.paginator?.pageIndex + 1 || 1 },
      { key: 'forecastInfoId', value: this.forecastInfoId },
    ];

    this.httpService.GET(this.url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.totalCount = res.data.total;

            // Set Data
            if (this.forecastMethodologyId == this.forecastMethodologyEnum.DempgraphicMorbidity && this.isTargetBased) {
              this.barChartLabels = res.data.list.lables;
              this.barChartData[0].data = res.data.list.currentPatientData;
              this.barChartData[1].data = res.data.list.targetPatientData;
            } else {
              this.chartLabels = res.data.list.lables;
              this.chartData[0].data = res.data.list.data;
            }

            this._ref.detectChanges();
          } else {
            this.alertService.error(res.message);
            this.loading = false;
            this._ref.detectChanges();
          }
        }, err => {
          this.alertService.exception();
          this.loading = false;
          this._ref.detectChanges();
        });
  }

  get title(): string {
    if (this.forecastMethodologyId == this.forecastMethodologyEnum.Service) {
      return 'Forecast Laboratory Test Services';
    }
    else if (this.forecastMethodologyId == this.forecastMethodologyEnum.Consumption) {
      return 'Forecast Laboratory Consumptions';
    }
    else if (this.forecastMethodologyId == this.forecastMethodologyEnum.DempgraphicMorbidity && !this.isTargetBased) {
      return 'Morbidity WHO Base';
    }
    else if (this.forecastMethodologyId == this.forecastMethodologyEnum.DempgraphicMorbidity && this.isTargetBased) {
      return 'Morbidity Target Base';
    }
  }

  get url(): string {
    if (this.forecastMethodologyId == this.forecastMethodologyEnum.Service) {
      return ForecastLaboratoryTestServicesController.ForecastLaboratoryTestServicesChart;
    }
    else if (this.forecastMethodologyId == this.forecastMethodologyEnum.Consumption) {
      return ForecastLaboratoryConsumptionsController.ForecastLaboratoryConsumptionsChart;
    }
    else if (this.forecastMethodologyId == this.forecastMethodologyEnum.DempgraphicMorbidity && !this.isTargetBased) {
      return ForecastMorbidityWhoBasesController.ForecastMorbidityWhoBasesChart;
    }
    else if (this.forecastMethodologyId == this.forecastMethodologyEnum.DempgraphicMorbidity && this.isTargetBased) {
      return ForecastMorbidityTargetBasesController.ForecastMorbidityTargetBasesChart;
    }
  }
}
