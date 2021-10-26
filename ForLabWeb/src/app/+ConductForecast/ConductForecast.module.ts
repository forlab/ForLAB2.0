
import { NgModule } from '@angular/core';
import { NgxLoadingModule } from 'ngx-loading';

import { SmartadminModule } from '../shared/smartadmin.module';
import { SmartadminDatatableModule } from "../shared/ui/datatable/smartadmin-datatable.module";
import { routing } from './ConductForecast.routing';
import { SmartadminEditorsModule } from "../shared/forms/editors/smartadmin-editors.module";

import { JqueryUiModule } from "../shared/ui/jquery-ui/jquery-ui.module";
import { ReactiveFormsModule } from '@angular/forms';
import { BsDatepickerModule, ModalModule } from 'ngx-bootstrap';
import { APIwithActionService } from '../shared/APIwithAction.service';
import { GlobalAPIService } from '../shared/GlobalAPI.service';
import { NgCircleProgressModule } from 'ng-circle-progress';

import { ConductForecastComponent } from './ConductForecast.component';
import { ForecastTestAddComponent } from './ForecastTestAdd/ForecastTestAdd.component';
import { ForecastInstrAddComponent } from './ForecastInstrAdd/ForecastInstrAdd.component';
import { ForecastInstrListComponent } from './ForecastInstrList/ForecastInstrList.component';
import { ForecastProductTestComponent } from './ForecastProductTest/ForecastProductTest.component';
import { ForecastMethodSelectComponent } from './ForecastMethodSelect/ForecastMethodSelect.component';
import { ForecastComparisonComponent } from './ForecastComparison/ForecastComparison.component';
import { ForecastConductSiteComponent } from './ForecastConductSite/ForecastConductSite.component';
import { ForecastImportServiceComponent } from './ForecastImportService/ForecastImportService.component';
import { ForecastFactorOutputComponent } from './ForecastFactorOutput/ForecastFactorOutput.component';
import { ForecastCalculationSelectComponent } from './ForecastCalculationSelect/ForecastCalculationSelect.component';
import { ForecastSuccessComponent } from './ForecastSuccess/ForecastSuccess.component';
import { ForecastMorbidityDiseaseComponent } from './ForecastMorbidityDisease/ForecastMorbidityDisease.component';
import { ForecastMorbidityGroupComponent } from './ForecastMorbidityGroup/ForecastMorbidityGroup.component';
import { ForecastMorbiditySiteComponent } from './ForecastMorbiditySite/ForecastMorbiditySite.component';
import { ForecastMorbidityTestComponent } from './ForecastMorbidityTest/ForecastMorbidityTest.component';
import { ForecastMorbidityProdTypeComponent } from './ForecastMorbidityProdType/ForecastMorbidityProdType.component';
import { ForecastNewProgramComponent } from './ForecastNewProgram/ForecastNewProgram.component';
import { ForecastNewProgramGroupComponent } from './ForecastNewProgramGroup/ForecastNewProgramGroup.component';
import { ForecastNewProgAssumPatientComponent } from './ForecastNewProgAssumPatient/ForecastNewProgAssumPatient.component';
import { ForecastNewProgAssumTestingComponent } from './ForecastNewProgAssumTesting/ForecastNewProgAssumTesting.component';
import { ForecastNewProgAssumProductComponent } from './ForecastNewProgAssumProduct/ForecastNewProgAssumProduct.component';
import { ForecastNewProgramProtocolComponent } from './ForecastNewProgramProtocol/ForecastNewProgramProtocol.component';
import { ConductForecastListComponent } from './ConductForecastList/ConductForecastList.component';
import { ForecastAddComponent } from './ForecastAdd/ForecastAdd.component';
import { ForecastReportComponent } from './ForecastReport/ForecastReport.component';
import { DeleteModalComponent } from 'app/shared/ui/datatable/DeleteModal/DeleteModal.component';


@NgModule({
  declarations: [
    ConductForecastComponent,
    ConductForecastListComponent,
    ForecastAddComponent,
    ForecastTestAddComponent,
    ForecastInstrAddComponent,
    ForecastComparisonComponent,
    ForecastReportComponent,
    ForecastInstrListComponent,
    ForecastProductTestComponent,
    ForecastMethodSelectComponent,
    ForecastConductSiteComponent,
    ForecastImportServiceComponent,
    ForecastFactorOutputComponent,
    ForecastCalculationSelectComponent,
    ForecastSuccessComponent,
    ForecastMorbidityDiseaseComponent,
    ForecastMorbiditySiteComponent,
    ForecastMorbidityGroupComponent,
    ForecastMorbidityTestComponent,
    ForecastMorbidityProdTypeComponent,
    ForecastNewProgramComponent,
    ForecastNewProgramGroupComponent,
    ForecastNewProgAssumPatientComponent,
    ForecastNewProgAssumTestingComponent,
    ForecastNewProgAssumProductComponent,
    ForecastNewProgramProtocolComponent
  ],
  entryComponents: [
    ForecastAddComponent,
    ForecastTestAddComponent,
    ForecastInstrAddComponent,
    ForecastInstrListComponent,
    ForecastProductTestComponent,
    ForecastMethodSelectComponent,
    ForecastConductSiteComponent,
    ForecastImportServiceComponent,
    ForecastFactorOutputComponent,
    ForecastCalculationSelectComponent,
    ForecastSuccessComponent,
    ForecastMorbidityDiseaseComponent,
    ForecastMorbiditySiteComponent,
    ForecastMorbidityGroupComponent,
    ForecastMorbidityTestComponent,
    ForecastMorbidityProdTypeComponent,
    ForecastNewProgramComponent,
    ForecastNewProgramGroupComponent,
    ForecastNewProgAssumPatientComponent,
    ForecastNewProgAssumTestingComponent,
    ForecastNewProgAssumProductComponent,
    ForecastNewProgramProtocolComponent,
    DeleteModalComponent
  ],
  imports: [
    routing,
    SmartadminModule,
    SmartadminEditorsModule,
    SmartadminDatatableModule,
    JqueryUiModule,
    ReactiveFormsModule,
    BsDatepickerModule.forRoot(),
    ModalModule.forRoot(),
    NgxLoadingModule.forRoot({}),
    NgCircleProgressModule.forRoot({
      "radius": 12,
      "space": -2,
      "maxPercent": 100,
      "outerStrokeWidth": 2,
      "outerStrokeColor": "#20bdef",
      "innerStrokeColor": "#e8eaec",
      "innerStrokeWidth": 2,
      "animateTitle": false,
      "showTitle": false,
      "showSubtitle": false,
      "showUnits": false,
      "showBackground": false
    })

  ],
  exports: [
    ConductForecastComponent,
  ],
  providers: [APIwithActionService, GlobalAPIService]

})
export class ConductForecastModule {

}