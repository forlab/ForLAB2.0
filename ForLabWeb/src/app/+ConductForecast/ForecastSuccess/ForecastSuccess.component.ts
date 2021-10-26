import { Component, OnInit, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';

@Component({
    selector: 'app-forecast-success',
    templateUrl: './ForecastSuccess.component.html',
    styleUrls: ['ForecastSuccess.component.css']
})

export class ForecastSuccessComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    forecastId: number;
    forecastName: string;

    constructor(public bsModalRef: BsModalRef, private _APIwithActionService: APIwithActionService) {
    }

    ngAfterViewChecked() {
    }

    ngOnInit() {
        this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "GetbyId").subscribe((resp) => {
            this.forecastName = resp.forecastNo;
        });
    }

    save() {
    }

    // handleSelectCalculation(index) {
    //     this.currentCalculation = index;
    // }

    onCloseModal() {
        this.bsModalRef.hide();
    }

    openNextModal() {
        this.bsModalRef.hide();
        this.event.emit({ type: "next" });
    }
}

