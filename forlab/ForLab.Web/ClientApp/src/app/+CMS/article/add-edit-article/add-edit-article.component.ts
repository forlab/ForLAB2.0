import { Component, OnInit, Injector, ViewChild, ElementRef } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';
import { ArticleDto, ArticleImageDto } from 'src/@core/models/CMS/Article';
import { ArticlesController } from 'src/@core/APIs/ArticlesController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { DynamicScriptLoaderService } from 'src/app/shared/services/dynamic-script-loader.service';
import { ConfigurationDto } from 'src/@core/models/configuration/Configuration';
import { ConfigurationsController } from 'src/@core/APIs/ConfigurationsController';
import * as moment from 'moment';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
declare const $: any;

@Component({
  selector: 'app-add-edit-article',
  templateUrl: './add-edit-article.component.html',
  styleUrls: ['./add-edit-article.component.scss']
})
export class AddEditArticleComponent extends BaseService implements OnInit {

  // Editor
  public Editor = ClassicEditor;
  config: any = {};
  // Vars
  form: FormGroup;
  loadingArticle: boolean = false;
  articleId: number;
  articleDto: ArticleDto = new ArticleDto();
  isUpdate: boolean = false;
  filesUpload: Array<File> = [];
  configurationDto: ConfigurationDto = new ConfigurationDto();

  constructor(private fb: FormBuilder, public injector: Injector, private dynamicScriptLoader: DynamicScriptLoaderService) {
    super(injector);

    if (this.router.url.includes('update')) {
      this.isUpdate = true;
      this.activatedRoute.paramMap.subscribe(params => {
        this.articleId = Number(params.get('articleId'));
        this.loadDataById(this.articleId);
      });
    }
  }

  ngOnInit() {

    this.startScript();
    this.loadConfiguration();

    this.form = this.fb.group({
      title: [null, Validators.compose([Validators.required])],
      content: [null, Validators.compose([Validators.required])],
      providedBy: [null, Validators.compose([Validators.required])],
      providedDate: [null, Validators.compose([Validators.required])],
    });

  }

  loadConfiguration() {
    const url = ConfigurationsController.GetConfigurationDetails;
    this.httpService.GET(url)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.configurationDto = res.data;
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

  // Image Gallery
  async startScript() {
    await this.dynamicScriptLoader
      .load('lightgallery')
      .then(data => {
        this.loadData();
      })
      .catch(error => console.log(error));
  }
  private loadData() {
    $('#aniimated-thumbnials').lightGallery({
      thumbnail: true,
      selector: 'a'
    });
  }

  loadDataById(id: number) {
    this.loadingArticle = true;
    const url = ArticlesController.GetArticleDetails;
    let params: QueryParamsDto[] = [
      { key: 'articleId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.articleDto = res.data;
          this.loadingArticle = false;
          this.setFormValue();
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

  setFormValue() {
    this.form.controls["title"].patchValue(this.articleDto.title);
    this.form.controls["content"].patchValue(this.articleDto.content);
    this.form.controls["providedBy"].patchValue(this.articleDto.providedBy);
    this.form.controls["providedDate"].patchValue(this.articleDto.providedDate);
    this._ref.detectChanges();
  }

  submitForm() {
    const controls = this.form.controls;
    /** check form */
    if (this.form.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    }

    this.loading = true;

    // Set the values
    this.articleDto.title = this.form.getRawValue().title;
    this.articleDto.content = this.form.getRawValue().content;
    this.articleDto.providedBy = this.form.getRawValue().providedBy;
    this.articleDto.providedDate = moment(this.form.getRawValue().providedDate).add(1, 'd').toISOString();

    let formData: FormData = new FormData();
    if (this.filesUpload && this.filesUpload.length > 0) {
      for (let i = 0; i < this.filesUpload.length; i++)
        formData.append('file_upload' + i, this.filesUpload[i], this.filesUpload[i].name);
    }
    formData.append('articleDto', JSON.stringify(this.articleDto));


    if (this.isUpdate) {

      // call service
      this.httpService.PUT(ArticlesController.UpdateArticle, formData, null, true)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe(res => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Article is updated successfully');
            this.router.navigate(['/CMS/articles']);
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

    } else {

      // call service
      this.httpService.POST(ArticlesController.CreateArticle, formData, null, true)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe(res => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Article is created successfully');
            this.router.navigate(['/CMS/articles']);
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

  // Images
  saveFilesToVariable(event) {

    if (!event || !event.target.files[0]) {
      return;
    }

    let files = event.target.files as any[];
    for (let i = 0; i < files.length; i++) {
      if (this.articleDto.articleImageDtos.findIndex(x => !x.isDeleted && !x.isExternalResource && x.attachmentName == files[i].name) > -1) {
        this.alertService.error(`File is already exist`);
        event.target.value = '';
        return;
      }
      else if ((files[i].size / 1000) >= this.configurationDto.attachmentsMaxSize) {
        this.alertService.error(`File size must be smaller than ${this.configurationDto.attachmentsMaxSize} KB`);
        event.target.value = '';
        return;
      }
    }

    if (files && files[0]) {
      for (let i = 0; i < files.length; i++) {
        this.filesUpload.unshift(files[i]);
        let attchDto: ArticleImageDto = new ArticleImageDto();
        attchDto.attachmentName = files[i].name;
        attchDto.extensionFormat = files[i].name.substr(files[i].name.lastIndexOf('.') + 1);
        attchDto.attachmentSize = files[i].size;
        this.articleDto.articleImageDtos.unshift(attchDto);
        // Render the image
        var reader = new FileReader();
        reader.onloadend = (e:any) => {
          attchDto.attachmentUrl = e.target.result;
        };
        reader.readAsDataURL(files[i]);
      }

      this._ref.detectChanges();
    }

  }

  removeFileFromFileList(fileName: string) {
    this.filesUpload.forEach(file => {
      if (file.name == fileName) {
        const index = this.filesUpload.indexOf(file);
        this.filesUpload.splice(index, 1);
      }
    });
  }

  removeFile(file: ArticleImageDto) {
    const index = this.articleDto.articleImageDtos.indexOf(file);
    if (this.articleDto.articleImageDtos[index].id && this.articleDto.articleImageDtos[index].id > 0) {
      this.articleDto.articleImageDtos[index].isDeleted = true;
    } else {
      this.articleDto.articleImageDtos.splice(index, 1);
      this.removeFileFromFileList(file.attachmentName);
    }
    this._ref.detectChanges();
  }



}
