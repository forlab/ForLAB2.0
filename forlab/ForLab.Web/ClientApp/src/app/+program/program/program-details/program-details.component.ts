import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ProgramDto } from 'src/@core/models/program/Program';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { takeUntil } from 'rxjs/operators';
import { PatientAssumptionsComponent } from '../../patient-assumption/patient-assumptions/patient-assumptions.component';
import { ProductAssumptionsComponent } from '../../product-assumption/product-assumptions/product-assumptions.component';
import { ProgramTestsComponent } from '../../program-test/program-tests/program-tests.component';
import { TestingAssumptionsComponent } from '../../testing-assumption/testing-assumptions/testing-assumptions.component';

@Component({
  selector: 'app-program-details',
  templateUrl: './program-details.component.html',
  styleUrls: ['./program-details.component.scss']
})
export class ProgramDetailsComponent extends BaseService implements OnInit {

  // Children
  @ViewChild(PatientAssumptionsComponent, { static: false }) patientAssumptionsComponent: PatientAssumptionsComponent;
  @ViewChild(ProductAssumptionsComponent, { static: false }) productAssumptionsComponent: ProductAssumptionsComponent;
  @ViewChild(ProgramTestsComponent, { static: false }) programTestsComponent: ProgramTestsComponent;
  @ViewChild(TestingAssumptionsComponent, { static: false }) testingAssumptionsComponent: TestingAssumptionsComponent;

  programId: number;
  programDto: ProgramDto = new ProgramDto();
  loadingProgram = false;

  constructor(public injector: Injector) {
    super(injector);

    if (this.router.url.includes('details')) {
      this.activatedRoute.paramMap.subscribe(params => {
        this.programId = Number(params.get('programId'));
        this.loadDataById(this.programId);
      });
    }

  }

  ngOnInit(): void {
  }

  loadDataById(id: number) {
    this.loadingProgram = true;
    const url = ProgramsController.GetProgramDetails;
    let params: QueryParamsDto[] = [
      { key: 'programId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.programDto = res.data;
          this.loadingProgram = false;

          if (this.programDto.createdBy != this.loggedInUser?.id) {
            this.patientAssumptionsComponent.columns = this.patientAssumptionsComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            this.productAssumptionsComponent.columns = this.productAssumptionsComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            this.programTestsComponent.columns = this.programTestsComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            this.testingAssumptionsComponent.columns = this.testingAssumptionsComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
          }

        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

}
