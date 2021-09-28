import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { AdminPanelURL } from 'src/@core/config';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent extends BaseService implements OnInit {

  adminPanelURL = AdminPanelURL;

  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
  }

  navigate(url: string) {
    this.router.navigate([url])
  }
}
