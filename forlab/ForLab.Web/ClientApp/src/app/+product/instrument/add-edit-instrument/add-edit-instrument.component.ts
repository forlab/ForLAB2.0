import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { InstrumentDto } from 'src/@core/models/product/Instrument';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { InstrumentsController } from 'src/@core/APIs/InstrumentsController';
import { ThroughPutUnitEnum, ReagentSystemEnum, ControlRequirementUnitEnum } from 'src/@core/models/enum/Enums';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { VendorsController } from 'src/@core/APIs/VendorsController';
import { TestingAreasController } from 'src/@core/APIs/TestingAreasController';

@Component({
  selector: 'app-add-edit-instrument',
  templateUrl: './add-edit-instrument.component.html',
  styleUrls: ['./add-edit-instrument.component.sass']
})
export class AddEditInstrumentComponent extends BaseService implements OnInit {

  instrumentDto: InstrumentDto = new InstrumentDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  throughPutUnitEnum = ThroughPutUnitEnum;
  reagentSystemEnum = ReagentSystemEnum;
  controlRequirementUnitEnum = ControlRequirementUnitEnum;
  vendors$: Observable<any[]>;
  testingAreas$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: InstrumentDto,
    public dialogRef: MatDialogRef<AddEditInstrumentComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadVendors();
    this.loadTestingAreas();

    this.form = this.fb.group({
      name: new FormControl(null, [Validators.required]),
      vendorId: new FormControl(null, [Validators.required]),
      maxThroughPut: new FormControl(null, [Validators.required]),
      throughPutUnitId: new FormControl(null, [Validators.required]),
      reagentSystemId: new FormControl(null, [Validators.required]),
      controlRequirement: new FormControl(null, [Validators.required]),
      controlRequirementUnitId: new FormControl(null, [Validators.required]),
      testingAreaId: new FormControl(null, [Validators.required]),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.instrumentDto = this.data || new InstrumentDto();
    }

  }

  loadVendors() {
    this.vendors$ = this.httpService.GET(VendorsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadTestingAreas() {
    this.testingAreas$ = this.httpService.GET(TestingAreasController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.form.controls['name'].patchValue(this.data.name);
    this.form.controls['vendorId'].patchValue(this.data.vendorId);
    this.form.controls['maxThroughPut'].patchValue(this.data.maxThroughPut);
    this.form.controls['throughPutUnitId'].patchValue(this.data.throughPutUnitId);
    this.form.controls['reagentSystemId'].patchValue(this.data.reagentSystemId);
    this.form.controls['controlRequirement'].patchValue(this.data.controlRequirement);
    this.form.controls['controlRequirementUnitId'].patchValue(this.data.controlRequirementUnitId);
    this.form.controls['testingAreaId'].patchValue(this.data.testingAreaId);
    this.instrumentDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(InstrumentsController.CreateInstrument, this.instrumentDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Instrument is created successfully');
            this.dialogRef.close(this.instrumentDto);
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


  updateObject() {
    this.httpService.PUT(InstrumentsController.UpdateInstrument, this.instrumentDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.instrumentDto);
            this.alertService.success('Instrument is updated successfully');
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

  save() {
    const controls = this.form.controls;
    /** check form */
    if (this.form.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    }

    this.loading = true;

    // Set the data
    this.instrumentDto.name = this.form.getRawValue().name;
    this.instrumentDto.vendorId = Number(this.form.getRawValue().vendorId);
    this.instrumentDto.maxThroughPut = this.form.getRawValue().maxThroughPut;
    this.instrumentDto.throughPutUnitId = Number(this.form.getRawValue().throughPutUnitId);
    this.instrumentDto.reagentSystemId = Number(this.form.getRawValue().reagentSystemId);
    this.instrumentDto.controlRequirement = this.form.getRawValue().controlRequirement;
    this.instrumentDto.controlRequirementUnitId = Number(this.form.getRawValue().controlRequirementUnitId);
    this.instrumentDto.testingAreaId = Number(this.form.getRawValue().testingAreaId);

    if (this.mode === 'create') {
      this.createObject();
    } else if (this.mode === 'update') {
      this.updateObject();
    }
  }


  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
