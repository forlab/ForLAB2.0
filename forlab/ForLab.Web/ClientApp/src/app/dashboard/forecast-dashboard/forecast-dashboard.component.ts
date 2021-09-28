import { Component, Injector, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { DashboardController } from 'src/@core/APIs/DashboardController';
import { ForecastInfosController } from 'src/@core/APIs/ForecastInfosController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { CountryPeriodEnum } from 'src/@core/models/enum/Enums';
import { BaseService } from 'src/@core/services/base.service';
declare const $: any;

@Component({
  selector: 'app-forecast-dashboard',
  templateUrl: './forecast-dashboard.component.html',
  styleUrls: ['./forecast-dashboard.component.scss']
})
export class ForecastDashboardComponent extends BaseService implements OnInit {

  // Latest
  latestForecasts$: Observable<any[]>;
  cardCounts$: Observable<any[]>;
  forecastsChartPeriod: number = 8;
  countryPeriodId: number;
  // Drp
  countryPeriodEnum = CountryPeriodEnum;

  constructor(public injector: Injector) {
    super(injector);
  }

  // barChart
  public forecastsChartOptions: any = {
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
  public forecastsChartLabels: string[] = [];
  public forecastsChartType = 'bar';
  public forecastsChartLegend = false;
  public forecastsChartData: any[] = [
    { data: [], label: 'Service' },
    { data: [], label: 'Consumption' },
    { data: [], label: 'Morbidity (Target Base)' },
    { data: [], label: 'Morbidity (WHO Base)' },
  ];
  public forecastsChartColors: Array<any> = [
    {
      backgroundColor: 'rgba(76,175,80,1)',
      borderColor: 'rgba(211,211,211,1)',
      pointBackgroundColor: 'rgba(211,211,211,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(211,211,211,0.8)'
    },
    {
      backgroundColor: 'rgba(33,150,243,1)',
      borderColor: 'rgba(110, 104, 193,1)',
      pointBackgroundColor: 'rgba(110, 104, 193,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(110, 104, 193,0.8)'
    },
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

  // end bar chart
  ngOnInit() {

    // Load Latest
    this.loadCardCounts();
    this.loadLatestForecasts();
    this.loadForecastsChart();

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

  loadLatestForecasts() {
    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: 5 },
      { key: 'pageIndex', value: 1 },
      { key: 'applySort', value: true },
      { key: 'sortProperty', value: 'createdOn' },
      { key: 'isAscending', value: false },
      // Filter
      { key: 'isActive', value: true },
    ];
    this.latestForecasts$ = this.httpService.GET(ForecastInfosController.GetAll, params).pipe(map(res => res.data.list));
  }

  loadCardCounts() {
    this.cardCounts$ = this.httpService.GET(DashboardController.ForecastCardCounts).pipe(map(res => res.data));
  }

  loadForecastsChart() {
    let params: QueryParamsDto[] = [
      { key: 'numOfMonths', value: this.forecastsChartPeriod },
      { key: 'countryPeriodId', value: this.countryPeriodId },
    ];

    this.httpService.GET(DashboardController.ForecastsChart, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;

            // Set Data
            this.forecastsChartLabels = res.data.lables;
            this.forecastsChartData[0].data = res.data.serviceData;
            this.forecastsChartData[1].data = res.data.consumptionData;
            this.forecastsChartData[2].data = res.data.targetBaseData;
            this.forecastsChartData[3].data = res.data.whoBaseData;

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
  
}
