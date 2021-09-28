import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { HomeComponent } from './home/home.component';
import { ArticlesComponent } from './article/articles/articles.component';
import { ArticleDetailsComponent } from './article/article-details/article-details.component';
import { ContactUsComponent } from './contact-us/contact-us.component';
import { FaqComponent } from './faq/faq.component';
import { ChannelVideosComponent } from './channel-videos/channel-videos.component';
import { UsefulResourcesComponent } from './useful-resources/useful-resources.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path: 'articles',
    component: ArticlesComponent
  },
  {
    path: 'articles/details/:articleId',
    component: ArticleDetailsComponent
  },
  {
    path: 'contact-us',
    component: ContactUsComponent
  },
  {
    path: 'faq',
    component: FaqComponent
  },
  {
    path: 'channel-videos',
    component: ChannelVideosComponent
  },
  {
    path: 'useful-resources',
    component: UsefulResourcesComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PagesRoutingModule { }
