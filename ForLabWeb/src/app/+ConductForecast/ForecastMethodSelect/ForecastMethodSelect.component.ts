import { Component, OnInit, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';

@Component({
    selector: 'app-forecast-instr-list',
    templateUrl: './ForecastMethodSelect.component.html',
    styleUrls: ['ForecastMethodSelect.component.css']
})

export class ForecastMethodSelectComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    forecastId: number;
    currentMethod = -1;
    methodology: string = "";
    exist: number = 0;
    loading = true;

    constructor(
        private _avRoute: ActivatedRoute,
        private _router: Router,
        private _APIwithActionService: APIwithActionService,
        public bsModalRef: BsModalRef,
        private modalService: BsModalService
    ) {
    }

    ngAfterViewChecked() {
    }

    ngOnInit() {
        this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "GetbyId").subscribe((resp) => {
            this.loading = false;
            this.methodology = resp["methodology"];
            console.log('this.methodology', this.methodology);
            switch (this.methodology) {
                case "SERVICE STATSTICS":
                    this.currentMethod = 0;
                    break;
                case "CONSUMPTION":
                    this.currentMethod = 1;
                    break;
                case "MORBIDITY":
                    this.currentMethod = 2;
                    break;
            }
        });
        this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "Isdataimported").subscribe((resp) => {
            this.exist = resp;
        });
    }

    handleSelectMethod(index) {
        this.currentMethod = index;
        if (index == 0) {
            this.methodology = "SERVICE STATSTICS";
        } else if (index == 1) {
            this.methodology = "CONSUMPTION";
        } else {
            this.methodology = "MORBIDITY";
        }
        this._APIwithActionService.putAPI(this.forecastId, this.methodology, "Forecsatinfo", "Put").subscribe((data) => {
            // this.openNextModal(this.methodology);
        });
    }

    onCloseModal() {
        this.bsModalRef.hide();
    }

    openNextModal() {
        this.bsModalRef.hide();
        this.event.emit({ type: "next", methodology: this.methodology });
    }

    openPreviousModal() {
        this.bsModalRef.hide();
        this.event.emit({ type: "back" });
    }

}

