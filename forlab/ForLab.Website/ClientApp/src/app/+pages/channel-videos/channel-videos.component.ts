import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ChannelVideoDto } from 'src/@core/models/CMS/ChannelVideo';
import { ChannelVideosController } from 'src/@core/APIs/ChannelVideosController';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-channel-videos',
  templateUrl: './channel-videos.component.html',
  styleUrls: ['./channel-videos.component.scss']
})
export class ChannelVideosComponent extends BaseService implements OnInit {

  channelVideos$: Observable<ChannelVideoDto[]>;
  
  constructor(public injector: Injector, public domSanitizer: DomSanitizer) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadChannelVideos();
  }

  loadChannelVideos() {
    let params: QueryParamsDto[] = [
      { key: 'isActive', value: true },
    ];
    this.channelVideos$ = this.httpService.GET(ChannelVideosController.GetAll, params).pipe(map(res => res.data.list));
  }

}
