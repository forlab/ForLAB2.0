import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { InquiryQuestionDto, InquiryQuestionReplyDto } from 'src/@core/models/CMS/InquiryQuestion';
import { InquiryQuestionsController } from 'src/@core/APIs/InquiryQuestionsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { takeUntil } from 'rxjs/operators';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-inquiry-question-details',
  templateUrl: './inquiry-question-details.component.html',
  styleUrls: ['./inquiry-question-details.component.scss']
})
export class InquiryQuestionDetailsComponent extends BaseService implements OnInit {

  form: FormGroup;
  inquiryQuestionId: number;
  inquiryQuestionDto: InquiryQuestionDto = new InquiryQuestionDto();
  inquiryQuestionReplyDto: InquiryQuestionReplyDto = new InquiryQuestionReplyDto();
  loadingInquiryQuestion = false;

  constructor(public injector: Injector, private fb: FormBuilder) {
    super(injector);

    if (this.router.url.includes('details')) {
      this.activatedRoute.paramMap.subscribe(params => {
        this.inquiryQuestionId = Number(params.get('inquiryQuestionId'));
        this.loadDataById(this.inquiryQuestionId);
      });
    }

  }

  ngOnInit(): void {
    this.form = this.fb.group({
      message: new FormControl(null, [Validators.required]),
    });
  }

  loadDataById(id: number) {
    this.loadingInquiryQuestion = true;
    let params: QueryParamsDto[] = [
      { key: 'inquiryQuestionId', value: id },
    ];

    this.httpService.GET(InquiryQuestionsController.GetInquiryQuestionDetails, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.inquiryQuestionDto = res.data;
          this.loadingInquiryQuestion = false;
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

  sendReply() {
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
    this.inquiryQuestionReplyDto.message = this.form.getRawValue().message;
    this.inquiryQuestionReplyDto.inquiryQuestionId = this.inquiryQuestionDto.id;

    this.httpService.POST(InquiryQuestionsController.CreateInquiryQuestionReply, this.inquiryQuestionReplyDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Reply is sent successfully');
            this.form.reset();
            this.loadDataById(this.inquiryQuestionId);
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

}
