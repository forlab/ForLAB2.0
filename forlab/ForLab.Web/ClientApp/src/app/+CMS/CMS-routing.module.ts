import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { ArticlesComponent } from './article/articles/articles.component';
import { AddEditArticleComponent } from './article/add-edit-article/add-edit-article.component';
import { InquiryQuestionDetailsComponent } from './inquiry-question/inquiry-question-details/inquiry-question-details.component';
import { InquiryQuestionsComponent } from './inquiry-question/inquiry-questions/inquiry-questions.component';
import { FrequentlyAskedQuestionsComponent } from './frequently-asked-question/frequently-asked-questions/frequently-asked-questions.component';
import { UsefulResourcesComponent } from './useful-resource/useful-resources/useful-resources.component';
import { ChannelVideosComponent } from './channel-video/channel-videos/channel-videos.component';
import { FeaturesComponent } from './feature/features/features.component';
import { ContactInfoComponent } from './contact-info/contact-info.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'articles',
    pathMatch: 'full'
  },
  {
    path: 'articles',
    component: ArticlesComponent
  },
  {
    path: 'articles/add',
    component: AddEditArticleComponent
  },
  {
    path: 'articles/update/:articleId',
    component: AddEditArticleComponent
  },
  {
    path: 'inquiry-questions',
    component: InquiryQuestionsComponent
  },
  {
    path: 'inquiry-questions/details/:inquiryQuestionId',
    component: InquiryQuestionDetailsComponent
  },
  {
    path: 'frequently-asked-questions',
    component: FrequentlyAskedQuestionsComponent
  },
  {
    path: 'useful-resources',
    component: UsefulResourcesComponent
  },
  {
    path: 'channel-videos',
    component: ChannelVideosComponent
  },
  {
    path: 'features',
    component: FeaturesComponent
  },
  {
    path: 'contact-info',
    component: ContactInfoComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CMSRoutingModule { }
