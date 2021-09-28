import { Component, OnInit, Injector } from '@angular/core';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { ChannelVideoDto, ChannelVideoFilterDto } from 'src/@core/models/CMS/ChannelVideo';
import { ChannelVideosController } from 'src/@core/APIs/ChannelVideosController';
import { takeUntil } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditChannelVideoComponent } from '../add-edit-channel-video/add-edit-channel-video.component';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-channel-videos',
  templateUrl: './channel-videos.component.html',
  styleUrls: ['./channel-videos.component.scss']
})
export class ChannelVideosComponent extends BaseService implements OnInit {

  filterDto: ChannelVideoFilterDto = new ChannelVideoFilterDto();
  data: ChannelVideoDto[];

  constructor(public injector: Injector, public domSanitizer: DomSanitizer) {
    super(injector);
  }

  ngOnInit() {
    this.loadData();
  }

  loadData() {

    let params: QueryParamsDto[] = [
      { key: 'isExternalResource', value: true },
    ];

    this.httpService.GET(ChannelVideosController.GetAll, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.data = res.data.list;
            // URL sanitization is causing refresh of the embedded YouTube video
            this.data?.forEach(x => {
              if (x.isExternalResource) {
                x.sanitizedUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(x.attachmentUrl) as any;
              }
            });
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

  reset() {
    let skipped: string[] = [];

    if (this.filterDto != null) {
      Object.keys(this.filterDto).forEach(key => {
        if (!skipped.includes(key)) {
          this.filterDto[key] = null;
        }
      });

      // Reload the data
      this.loadData();
    }
  }

  createObject() {
    this.dialog.open(AddEditChannelVideoComponent)
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateObject(channelVideo: ChannelVideoDto) {
    this.dialog.open(AddEditChannelVideoComponent, {
      data: channelVideo
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateIsActive(channelVideo: ChannelVideoDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: ChannelVideosController.UpdateIsActive,
        objectInfo: [{ key: 'Title', value: channelVideo.title }, { key: 'Extension Format', value: channelVideo.extensionFormat }],
        isActive: !channelVideo.isActive,
        queryParamsDto: { key: 'channelVideoId', value: channelVideo.id },
      }
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  deleteObject(channelVideo: ChannelVideoDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: ChannelVideosController.RemoveChannelVideo,
        objectInfo: [{ key: 'Title', value: channelVideo.title }, { key: 'Extension Format', value: channelVideo.extensionFormat }],
        queryParamsDto: { key: 'channelVideoId', value: channelVideo.id },
      }
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

}
