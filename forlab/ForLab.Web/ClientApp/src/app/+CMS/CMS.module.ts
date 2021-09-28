import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsModule as chartjsModule } from 'ng2-charts';
import { NgxEchartsModule } from 'ngx-echarts';
import { MorrisJsModule } from 'angular-morris-js';
// Libs
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { CKEditorModule } from '@ckeditor/ckeditor5-angular';
// Angualr Matrial
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatCardModule } from '@angular/material/card';
// Google Maps
import { AgmCoreModule } from '@agm/core';
import { GooglePlaceModule } from "ngx-google-places-autocomplete";
import { GoogleMapsKey } from 'src/@core/config';
// Routing
import { CMSRoutingModule } from './CMS-routing.module';
import { CoreModule } from 'src/@core/core.module';
// Components
import { ArticlesComponent } from './article/articles/articles.component';
import { AddEditArticleComponent } from './article/add-edit-article/add-edit-article.component';
import { InquiryQuestionDetailsComponent } from './inquiry-question/inquiry-question-details/inquiry-question-details.component';
import { InquiryQuestionsComponent } from './inquiry-question/inquiry-questions/inquiry-questions.component';
import { FrequentlyAskedQuestionsComponent } from './frequently-asked-question/frequently-asked-questions/frequently-asked-questions.component';
import { AddEditFrequentlyAskedQuestionComponent } from './frequently-asked-question/add-edit-frequently-asked-question/add-edit-frequently-asked-question.component';
import { UsefulResourcesComponent } from './useful-resource/useful-resources/useful-resources.component';
import { AddEditUsefulResourceComponent } from './useful-resource/add-edit-useful-resource/add-edit-useful-resource.component';
import { AddEditChannelVideoComponent } from './channel-video/add-edit-channel-video/add-edit-channel-video.component';
import { ChannelVideosComponent } from './channel-video/channel-videos/channel-videos.component';
import { FeaturesComponent } from './feature/features/features.component';
import { AddEditFeatureComponent } from './feature/add-edit-feature/add-edit-feature.component';
import { ContactInfoComponent } from './contact-info/contact-info.component';


@NgModule({
  declarations: [
    ArticlesComponent,
    AddEditArticleComponent,
    InquiryQuestionDetailsComponent,
    InquiryQuestionsComponent,
    FrequentlyAskedQuestionsComponent,
    AddEditFrequentlyAskedQuestionComponent,
    UsefulResourcesComponent,
    AddEditUsefulResourceComponent,
    AddEditChannelVideoComponent,
    ChannelVideosComponent,
    FeaturesComponent,
    AddEditFeatureComponent,
    ContactInfoComponent],
  imports: [
    // Google Maps
    GooglePlaceModule,
    AgmCoreModule.forRoot({
      apiKey: GoogleMapsKey,
      libraries: ["places"]
    }),
    // App
    CoreModule,
    CommonModule,
    CMSRoutingModule,
    chartjsModule,
    NgxEchartsModule,
    MorrisJsModule,
    MatIconModule,
    MatButtonModule,
    MatPaginatorModule,
    MatSortModule,
    MatMenuModule,
    MatTableModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    NgxDatatableModule,
    MatExpansionModule,
    MatTooltipModule,
    CKEditorModule,
    MatCardModule
  ]
})
export class CMSModule { }
