import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { TestDto } from 'src/@core/models/testing/Test';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TestsController } from 'src/@core/APIs/TestsController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { TestingAreasController } from 'src/@core/APIs/TestingAreasController';

@Component({
  selector: 'app-add-edit-test',
  templateUrl: './add-edit-test.component.html',
  styleUrls: ['./add-edit-test.component.scss']
})
export class AddEditTestComponent extends BaseService implements OnInit {


  testDto: TestDto = new TestDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  testingAreas$: Observable<any[]>;
  
  constructor(@Inject(MAT_DIALOG_DATA) public data: TestDto,
    public dialogRef: MatDialogRef<AddEditTestComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.loadTestingAreas();
    
    this.form = this.fb.group({
      testingAreaId: new FormControl(null, [Validators.required]),
      name: new FormControl(null, [Validators.required]),
      shortName: new FormControl(null, [Validators.required]),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.testDto = this.data || new TestDto();
    }

  }

  loadTestingAreas() {
    this.testingAreas$ = this.httpService.GET(TestingAreasController.GetAllAsDrp).pipe(map(res => res.data));
  }
  
  setFormData() {
    this.form.controls['testingAreaId'].patchValue(this.data.testingAreaId);
    this.form.controls['name'].patchValue(this.data.name);
    this.form.controls['shortName'].patchValue(this.data.shortName);
    this.testDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(TestsController.CreateTest, this.testDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Test is created successfully');
            this.dialogRef.close(this.testDto);
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
    this.httpService.PUT(TestsController.UpdateTest, this.testDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.testDto);
            this.alertService.success('Test is updated successfully');
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
    this.testDto.testingAreaId = this.form.getRawValue().testingAreaId;
    this.testDto.name = this.form.getRawValue().name;
    this.testDto.shortName = this.form.getRawValue().shortName;

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
