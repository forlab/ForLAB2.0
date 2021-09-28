import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { FrequentlyAskedQuestionDto } from 'src/@core/models/CMS/FrequentlyAskedQuestion';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FrequentlyAskedQuestionsController } from 'src/@core/APIs/FrequentlyAskedQuestionsController';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-add-edit-frequently-asked-question',
  templateUrl: './add-edit-frequently-asked-question.component.html',
  styleUrls: ['./add-edit-frequently-asked-question.component.scss']
})
export class AddEditFrequentlyAskedQuestionComponent extends BaseService implements OnInit {

  frequentlyAskedQuestionDto: FrequentlyAskedQuestionDto = new FrequentlyAskedQuestionDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public data: FrequentlyAskedQuestionDto,
    public dialogRef: MatDialogRef<AddEditFrequentlyAskedQuestionComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.form = this.fb.group({
      question: new FormControl(null, [Validators.required]),
      answer: new FormControl(null, [Validators.required]),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.frequentlyAskedQuestionDto = this.data || new FrequentlyAskedQuestionDto();
    }

  }

  setFormData() {
    this.form.controls['question'].patchValue(this.data.question);
    this.form.controls['answer'].patchValue(this.data.answer);
    this.frequentlyAskedQuestionDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(FrequentlyAskedQuestionsController.CreateFrequentlyAskedQuestion, this.frequentlyAskedQuestionDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Frequently Asked Question is created successfully');
            this.dialogRef.close(this.frequentlyAskedQuestionDto);
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
    this.httpService.PUT(FrequentlyAskedQuestionsController.UpdateFrequentlyAskedQuestion, this.frequentlyAskedQuestionDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.frequentlyAskedQuestionDto);
            this.alertService.success('Frequently Asked Question is updated successfully');
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
    this.frequentlyAskedQuestionDto.question = this.form.getRawValue().question;
    this.frequentlyAskedQuestionDto.answer = this.form.getRawValue().answer;

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
