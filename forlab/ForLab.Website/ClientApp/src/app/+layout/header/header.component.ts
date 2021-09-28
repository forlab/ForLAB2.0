import { Component, OnInit } from '@angular/core';
import { AdminPanelURL } from 'src/@core/config';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  adminPanelURL = AdminPanelURL;

  constructor() { }

  ngOnInit(): void {
  }

}
