import { Component, OnInit } from '@angular/core';
declare const $: any;
@Component({
  selector: 'app-page403',
  templateUrl: './page403.component.html',
  styleUrls: ['./page403.component.scss']
})
export class Page403Component implements OnInit {
  constructor() {}
  ngOnInit() {
    var loginformcenter =
      ($(window).height() - $('.login100-form').height()) / 2 - 34;
    $('.login100-form').css('margin-top', loginformcenter);
  }
}
