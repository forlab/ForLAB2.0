import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
// Material
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
// Pipes
import { EnumOptionListPipe } from './pipes/enumOptionListPipe';
import { HumanizePipe } from './pipes/humanizePipe';
import { StringFilterByPipe } from './pipes/stringFilterByPipe';
// Directives
import { ConfirmDeleteComponent } from './directives/confirm-delete/confirm-delete.component';
import { ConfirmActiveComponent } from './directives/confirm-active/confirm-active.component';
import { ConfirmActiveSelectedComponent } from './directives/confirm-active-selected/confirm-active-selected.component';
import { ConfirmExportComponent } from './directives/confirm-export/confirm-export.component';
import { ImportErrorsComponent } from './directives/import-errors/import-errors.component';
// Lib
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { NgxMaskModule } from 'ngx-mask';
import { AvatarModule } from 'ngx-avatar';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { RxReactiveFormsModule } from '@rxweb/reactive-form-validators';
// Permission
import { NgxPermissionsModule } from 'ngx-permissions';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    // Material
    MatRadioModule,
    MatButtonModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatExpansionModule,
    MatSnackBarModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    // lib
    PerfectScrollbarModule,
    AvatarModule,
    NgxMatSelectSearchModule,
    NgxMaskModule,
    RxReactiveFormsModule,
    // Permission
    NgxPermissionsModule.forChild(),
  ],
  declarations: [
    // Directives
    ConfirmDeleteComponent,
    ConfirmActiveComponent,
    ConfirmExportComponent,
    ImportErrorsComponent,
    ConfirmActiveSelectedComponent,
    // pipes
    EnumOptionListPipe,
    HumanizePipe,
    StringFilterByPipe,
  ],
  exports: [
    // lib
    PerfectScrollbarModule,
    AvatarModule,
    NgxMatSelectSearchModule,
    FormsModule,
    ReactiveFormsModule,
    NgxMaskModule,
    RxReactiveFormsModule,
    // Permission
    NgxPermissionsModule,
    // Material
    MatButtonModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatExpansionModule,
    MatRadioModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    // Directives
    ConfirmDeleteComponent,
    ConfirmActiveComponent,
    ConfirmExportComponent,
    ImportErrorsComponent,
    ConfirmActiveSelectedComponent,
    // pipes
    EnumOptionListPipe,
    HumanizePipe,
    StringFilterByPipe,
  ],
  providers: [
  ]
})
export class CoreModule {
}
