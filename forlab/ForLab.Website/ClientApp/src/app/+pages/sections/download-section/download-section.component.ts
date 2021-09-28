import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-download-section',
  templateUrl: './download-section.component.html',
  styleUrls: ['./download-section.component.scss']
})
export class DownloadSectionComponent implements OnInit {

  desktopUrl = 'https://storage.googleapis.com/forlabaj.appspot.com/For%20Lab.zip';

  constructor() { }

  ngOnInit(): void {
  }

}
