import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { takeUntil, tap } from 'rxjs/operators';
import { ForecastResultsController } from 'src/@core/APIs/ForecastResultsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
declare const $: any;

@Component({
  selector: 'app-result-chart',
  templateUrl: './result-chart.component.html',
  styleUrls: ['./result-chart.component.scss']
})
export class ResultChartComponent extends BaseService implements OnInit {
  // Inputs
  @Input('forecastInfoId') forecastInfoId: number;
  // Children
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  // Vars
  totalCount: number;

  constructor(public injector: Injector) {
    super(injector);
  }

  // Chart
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
    { data: [], label: 'Total Price' },
    { data: [], label: 'Total Value' },
    { data: [], label: 'Amount Forecasted' },
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
      backgroundColor: 'rgba(155, 0, 0, 1)',
      borderColor: 'rgba(110, 104, 193,1)',
      pointBackgroundColor: 'rgba(110, 104, 193,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(110, 104, 193,0.8)'
    },
    {
      backgroundColor: 'rgba(110, 104, 193, 1)',
      borderColor: 'rgba(110, 104, 193,1)',
      pointBackgroundColor: 'rgba(110, 104, 193,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(110, 104, 193,0.8)'
    },
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
    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: this.paginator?.pageSize || 10 },
      { key: 'pageIndex', value: this.paginator?.pageIndex + 1 || 1 },
      { key: 'forecastInfoId', value: this.forecastInfoId },
    ];

    this.httpService.GET(ForecastResultsController.ForecastResultsChart, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.totalCount = res.data.total;

            // Set Data
            this.barChartLabels = res.data.list.lables;
            this.barChartData[0].data = res.data.list.totalPriceData;
            this.barChartData[1].data = res.data.list.totalValueData;
            this.barChartData[2].data = res.data.list.amountForecastedData;

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
