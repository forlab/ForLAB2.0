import { NgModule } from '@angular/core';
import { PagesRoutingModule } from './pages-routing.module';
import { LayoutModule } from 'src/app/+layout/layout.module';
import { CoreModule } from 'src/@core/core.module';
// Google Maps
import { AgmCoreModule } from '@agm/core';
import { GoogleMapsKey } from 'src/@core/config';
// Libs
import { NgxPaginationModule } from 'ngx-pagination';
// Components
import { HomeComponent } from './home/home.component';
import { ArticlesComponent } from './article/articles/articles.component';
import { ArticleDetailsComponent } from './article/article-details/article-details.component';
import { FaqComponent } from './faq/faq.component';
import { ContactUsComponent } from './contact-us/contact-us.component';
import { FeaturesSectionComponent } from './sections/features-section/features-section.component';
import { DownloadSectionComponent } from './sections/download-section/download-section.component';
import { LatestArticlesSectionComponent } from './sections/latest-articles-section/latest-articles-section.component';
import { ContactSectionComponent } from './sections/contact-section/contact-section.component';
import { PartnersSectionComponent } from './sections/partners-section/partners-section.component';
import { ChannelVideosComponent } from './channel-videos/channel-videos.component';
import { UsefulResourcesComponent } from './useful-resources/useful-resources.component';


@NgModule({
  declarations: [

    HomeComponent,

    ArticlesComponent,

    ArticleDetailsComponent,

    FaqComponent,

    ContactUsComponent,

    FeaturesSectionComponent,

    DownloadSectionComponent,

    LatestArticlesSectionComponent,

    ContactSectionComponent,

    PartnersSectionComponent,

    ChannelVideosComponent,

    UsefulResourcesComponent],
  imports: [
    // Google Maps
    AgmCoreModule.forRoot({
      apiKey: GoogleMapsKey,
      libraries: ["places"]
    }),
    // App
    PagesRoutingModule,
    LayoutModule,
    CoreModule,
    // Libs
    NgxPaginationModule
  ],
})
export class PagesModule { }
