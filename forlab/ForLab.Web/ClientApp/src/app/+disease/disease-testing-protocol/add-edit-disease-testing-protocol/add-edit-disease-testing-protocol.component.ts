import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { DiseaseTestingProtocolDto } from 'src/@core/models/disease/DiseaseTestingProtocol';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DiseaseTestingProtocolsController } from 'src/@core/APIs/DiseaseTestingProtocolsController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { DiseasesController } from 'src/@core/APIs/DiseasesController';
import { TestingProtocolsController } from 'src/@core/APIs/TestingProtocolsController';

@Component({
  selector: 'app-add-edit-disease-testing-protocol',
  templateUrl: './add-edit-disease-testing-protocol.component.html',
  styleUrls: ['./add-edit-disease-testing-protocol.component.scss']
})
export class AddEditDiseaseTestingProtocolComponent extends BaseService implements OnInit {

  diseaseTestingProtocolDto: DiseaseTestingProtocolDto = new DiseaseTestingProtocolDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  diseases$: Observable<any[]>;
  testingProtocols$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: DiseaseTestingProtocolDto,
    public dialogRef: MatDialogRef<AddEditDiseaseTestingProtocolComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadDiseases();
    this.loadTestingProtocols();

    this.form = this.fb.group({
      diseaseId: new FormControl(null, [Validators.required]),
      testingProtocolId: new FormControl(null, [Validators.required]),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.diseaseTestingProtocolDto = this.data || new DiseaseTestingProtocolDto();
    }

  }

  loadDiseases() {
    this.diseases$ = this.httpService.GET(DiseasesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadTestingProtocols() {
    this.testingProtocols$ = this.httpService.GET(TestingProtocolsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.diseaseTestingProtocolDto = JSON.parse(JSON.stringify(this.data));
    this.form.controls['diseaseId'].patchValue(this.data.diseaseId);
    this.form.controls['testingProtocolId'].patchValue(this.data.testingProtocolId);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(DiseaseTestingProtocolsController.CreateDiseaseTestingProtocol, this.diseaseTestingProtocolDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Disease Testing Protocol is created successfully');
            this.dialogRef.close(this.diseaseTestingProtocolDto);
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
    this.httpService.PUT(DiseaseTestingProtocolsController.UpdateDiseaseTestingProtocol, this.diseaseTestingProtocolDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.diseaseTestingProtocolDto);
            this.alertService.success('Disease Testing Protocol is updated successfully');
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
    this.diseaseTestingProtocolDto.diseaseId = this.form.getRawValue().diseaseId;
    this.diseaseTestingProtocolDto.testingProtocolId = this.form.getRawValue().testingProtocolId;

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
