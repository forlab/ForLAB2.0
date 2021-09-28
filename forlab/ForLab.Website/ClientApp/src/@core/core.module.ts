import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
// Libs
import { ToastrModule } from 'ngx-toastr';
// Pipes
import { EnumOptionListPipe } from './pipes/enumOptionListPipe';
import { HumanizePipe } from './pipes/humanizePipe';
import { StringFilterByPipe } from './pipes/stringFilterByPipe';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
    }),
  ],
  declarations: [
    // pipes
    EnumOptionListPipe,
    HumanizePipe,
    StringFilterByPipe,
  ],
  exports: [
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    // pipes
    EnumOptionListPipe,
    HumanizePipe,
    StringFilterByPipe,
    // Libs
    ToastrModule,
  ],
  providers: [
  ]
})
export class CoreModule {
}
