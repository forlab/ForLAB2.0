import { Component, Injector, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { DiseasesController } from 'src/@core/APIs/DiseasesController';
import { UsersController } from 'src/@core/APIs/UsersController';
import { DashboardController } from 'src/@core/APIs/DashboardController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { BaseService } from 'src/@core/services/base.service';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { RegionsController } from 'src/@core/APIs/RegionsController';
declare const $: any;

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent extends BaseService implements OnInit {

  // Data
  latestUsers$: Observable<any[]>;
  latestDiseases$: Observable<any[]>;
  cardCounts$: Observable<any>;
  numberOfLaboratories$: Observable<number>;
  numberOfDiseases$: Observable<number>;
  // Drp
  countries$: Observable<any[]>;
  lab_regions$: Observable<any[]>;
  chart_regions$: Observable<any[]>;
  // Bind Vars
  lab_countryId: number;
  lab_regionId: number;
  chart_countryId: number;
  chart_regionId: number;
  dis_countryId: number;
  // period filter
  usersChartPeriod: number = 8;
  questionsChartPeriod: number = 8;
  laboratoriesChartPeriod: number = 8;

  constructor(public injector: Injector) {
    super(injector);
  }

  // Users Chart
  public usersChartOptions = {
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
            labelString: 'Users Count'
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
  usersChartData = [
    {
      label: 'New Users',
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
  usersChartLabels = [];

  // Laboratories Chart
  public laboratoriesChartOptions = {
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
            labelString: 'Laboratories Count'
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
  laboratoriesChartData = [
    {
      label: 'New Laboratories',
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
  laboratoriesChartLabels = [];

  // Inquiry Questions Chart
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
    { data: [], label: 'Answered' },
    { data: [], label: 'Not Answered Yet' }
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

    // Load Drp
    this.loadCountries();
    // Load Data
    this.loadCardCounts();
    this.loadLatestUsers();
    this.loadLatestDiseases();
    this.loadNumberOfLaboratories();
    this.loadNumberOfDiseases();
    this.loadInquiryQuestionsChart();
    this.loadUsersChart();
    this.loadLaboratoriesChart();

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


  loadCountries() {
    this.countries$ = this.httpService.GET(CountriesController.GetAllAsDrp).pipe(map(res => res.data));
  }

  loadLabRegions() {
    let params: QueryParamsDto[] = [
      { key: 'countryId', value: this.lab_countryId },
    ];
    this.lab_regions$ = this.httpService.GET(RegionsController.GetAllAsDrp, params).pipe(map(res => res.data));
  }

  loadChartRegions() {
    let params: QueryParamsDto[] = [
      { key: 'countryId', value: this.chart_countryId },
    ];
    this.chart_regions$ = this.httpService.GET(RegionsController.GetAllAsDrp, params).pipe(map(res => res.data));
  }

  loadLatestUsers() {
    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: 5 },
      { key: 'pageIndex', value: 1 },
      { key: 'applySort', value: true },
      { key: 'sortProperty', value: 'createdOn' },
      { key: 'isAscending', value: false },
    ];
    this.latestUsers$ = this.httpService.GET(UsersController.GetAllUsers, params).pipe(map(res => res.data.list));
  }

  loadLatestDiseases() {
    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: 5 },
      { key: 'pageIndex', value: 1 },
      { key: 'applySort', value: true },
      { key: 'sortProperty', value: 'createdOn' },
      { key: 'isAscending', value: false },
      // Filter
      { key: 'isActive', value: true },
    ];
    this.latestDiseases$ = this.httpService.GET(DiseasesController.GetAll, params).pipe(map(res => res.data.list));
  }

  loadCardCounts() {
    this.cardCounts$ = this.httpService.GET(DashboardController.MainCardCounts).pipe(map(res => res.data));
  }

  loadNumberOfLaboratories() {
    let params: QueryParamsDto[] = [
      { key: 'countryId', value: this.lab_countryId },
      { key: 'regionId', value: this.lab_regionId },
    ];
    this.numberOfLaboratories$ = this.httpService.GET(DashboardController.NumberOfLaboratories, params).pipe(map(res => res.data));
  }

  loadNumberOfDiseases() {
    let params: QueryParamsDto[] = [
      { key: 'countryId', value: this.dis_countryId },
    ];
    this.numberOfDiseases$ = this.httpService.GET(DashboardController.NumberOfDiseases, params).pipe(map(res => res.data));
  }

  loadInquiryQuestionsChart() {
    let params: QueryParamsDto[] = [
      { key: 'numOfMonths', value: this.questionsChartPeriod },
    ];

    this.httpService.GET(DashboardController.InquiryQuestionsChart, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;

            // Set Data
            this.barChartLabels = res.data.lables;
            this.barChartData[0].data = res.data.answeredData;
            this.barChartData[1].data = res.data.notAnsweredData;

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

  loadUsersChart() {
    let params: QueryParamsDto[] = [
      { key: 'numOfMonths', value: this.usersChartPeriod },
    ];

    this.httpService.GET(DashboardController.UsersChart, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;

            // Set Data
            this.usersChartLabels = res.data.lables;
            this.usersChartData[0].data = res.data.usersData;

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


  loadLaboratoriesChart() {

    let params: QueryParamsDto[] = [
      { key: 'countryId', value: this.chart_countryId },
      { key: 'regionId', value: this.chart_regionId },
      { key: 'numOfMonths', value: this.laboratoriesChartPeriod },
    ];

    this.httpService.GET(DashboardController.LaboratoriesChart, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;

            // Set Data
            this.laboratoriesChartLabels = res.data.lables;
            this.laboratoriesChartData[0].data = res.data.laboratoriesData;

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
