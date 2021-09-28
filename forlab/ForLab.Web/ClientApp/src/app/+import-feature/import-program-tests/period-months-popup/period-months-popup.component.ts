import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CalculationPeriodEnum } from 'src/@core/models/enum/Enums';
import { takeUntil, map } from 'rxjs/operators';
import { ProgramTestDto } from 'src/@core/models/program/ProgramTest';
import { TestingProtocolCalculationPeriodMonthDto } from 'src/@core/models/testing/TestingProtocolCalculationPeriodMonth';
import { TestingProtocolDto } from 'src/@core/models/testing/TestingProtocol';

@Component({
  selector: 'app-period-months-popup',
  templateUrl: './period-months-popup.component.html',
  styleUrls: ['./period-months-popup.component.scss']
})
export class PeriodMonthsPopupComponent extends BaseService implements OnInit {

  form: FormGroup;
  // Drp
  calculationPeriodEnum = CalculationPeriodEnum;

  constructor(@Inject(MAT_DIALOG_DATA) public programTestDto: ProgramTestDto,
    public dialogRef: MatDialogRef<PeriodMonthsPopupComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.form = this.fb.group({
      calculationPeriodId: new FormControl(null, [Validators.required]),
    });

    this.form.controls['calculationPeriodId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(calculationPeriodId => {
        this.calcMonths();
      });

    this.form.controls['calculationPeriodId'].patchValue(this.programTestDto?.testingProtocolDto?.calculationPeriodId, {emitEvent: false})
  }
  calcMonths() {
    const calculationPeriodId = this.form.getRawValue().calculationPeriodId;
    if (!calculationPeriodId) return;

    if(!this.programTestDto?.testingProtocolDto) {
      this.programTestDto.testingProtocolDto = new TestingProtocolDto();
    }

    if (calculationPeriodId == this.calculationPeriodEnum.OneYear) {
      this.programTestDto.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos = Array.from(Array(12), (_, i) => i + 1).map(x => {
        let val = new TestingProtocolCalculationPeriodMonthDto();
        val.calculationPeriodMonthId = x;
        val.calculationPeriodMonthName = `M${x}`;
        return val;
      });
    } else if (calculationPeriodId == this.calculationPeriodEnum.TwoYears) {
      this.programTestDto.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos = Array.from(Array(24), (_, i) => i + 1).map(x => {
        let val = new TestingProtocolCalculationPeriodMonthDto();
        val.calculationPeriodMonthId = 12 + x;
        val.calculationPeriodMonthName = `M${x}`;
        return val;
      });
    }

  }

  save() {
    const controls = this.form.controls;
    /** check form */
    if (this.form.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    }

    // Set the data
    this.programTestDto.testingProtocolDto.calculationPeriodId = this.form.getRawValue().calculationPeriodId;
    this.dialogRef.close(this.programTestDto);
  }
}
