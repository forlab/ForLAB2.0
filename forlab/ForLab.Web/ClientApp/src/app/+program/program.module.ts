import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsModule as chartjsModule } from 'ng2-charts';
import { NgxEchartsModule } from 'ngx-echarts';
import { MorrisJsModule } from 'angular-morris-js';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
// Angualr Matrial
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTabsModule } from '@angular/material/tabs';
// Routing
import { ProgramRoutingModule } from './program-routing.module';
import { CoreModule } from 'src/@core/core.module';
import { ImportFeatureModule } from 'src/app/+import-feature/import-feature.module';
// Components
import { ProgramsComponent } from './program/programs/programs.component';
import { AddProgramComponent } from './program/add-program/add-program.component';
import { EditProgramComponent } from './program/edit-program/edit-program.component';
import { ProgramDetailsComponent } from './program/program-details/program-details.component';
import { AddEditProgramTestComponent } from './program-test/add-edit-program-test/add-edit-program-test.component';
import { ProgramTestsComponent } from './program-test/program-tests/program-tests.component';
import { AddEditPatientAssumptionComponent } from './patient-assumption/add-edit-patient-assumption/add-edit-patient-assumption.component';
import { PatientAssumptionsComponent } from './patient-assumption/patient-assumptions/patient-assumptions.component';
import { ProductAssumptionsComponent } from './product-assumption/product-assumptions/product-assumptions.component';
import { AddEditProductAssumptionComponent } from './product-assumption/add-edit-product-assumption/add-edit-product-assumption.component';
import { AddEditTestingAssumptionComponent } from './testing-assumption/add-edit-testing-assumption/add-edit-testing-assumption.component';
import { TestingAssumptionsComponent } from './testing-assumption/testing-assumptions/testing-assumptions.component';

@NgModule({
  declarations: [
    ProgramsComponent, 
    AddProgramComponent, 
    EditProgramComponent, 
    ProgramDetailsComponent, 
    AddEditProgramTestComponent, 
    ProgramTestsComponent, 
    AddEditPatientAssumptionComponent, 
    PatientAssumptionsComponent, 
    ProductAssumptionsComponent, 
    AddEditProductAssumptionComponent, 
    AddEditTestingAssumptionComponent, 
    TestingAssumptionsComponent,
  ],
  imports: [
    ImportFeatureModule,
    CoreModule,
    CommonModule,
    ProgramRoutingModule,
    chartjsModule,
    NgxEchartsModule,
    MorrisJsModule,
    MatIconModule,
    MatButtonModule,
    MatPaginatorModule,
    MatSortModule,
    MatMenuModule,
    MatTableModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    NgxDatatableModule,
    MatExpansionModule,
    MatTooltipModule,
    MatStepperModule,
    MatTabsModule
  ]
})
export class ProgramModule {}
