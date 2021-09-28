import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { TestingAreaDto } from 'src/@core/models/lookup/TestingArea';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TestingAreasController } from 'src/@core/APIs/TestingAreasController';
import { takeUntil, map } from 'rxjs/operators';

@Component({
  selector: 'app-add-edit-testing-area',
  templateUrl: './add-edit-testing-area.component.html',
  styleUrls: ['./add-edit-testing-area.component.sass']
})
export class AddEditTestingAreaComponent extends BaseService implements OnInit {

  testingAreaDto: TestingAreaDto = new TestingAreaDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public data: TestingAreaDto,
    public dialogRef: MatDialogRef<AddEditTestingAreaComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.form = this.fb.group({
      name: new FormControl(null, [Validators.required]),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.testingAreaDto = this.data || new TestingAreaDto();
    }

  }

  setFormData() {
    this.form.controls['name'].patchValue(this.data.name);
    this.testingAreaDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(TestingAreasController.CreateTestingArea, this.testingAreaDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Testing Area is created successfully');
            this.dialogRef.close(this.testingAreaDto);
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
    this.httpService.PUT(TestingAreasController.UpdateTestingArea, this.testingAreaDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.testingAreaDto);
            this.alertService.success('Testing Area is updated successfully');
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
    this.testingAreaDto.name = this.form.getRawValue().name;

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
