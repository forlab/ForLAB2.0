import { Component, OnInit, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';

@Component({
    selector: 'app-forecast-new-program-protocol',
    templateUrl: './ForecastNewProgramProtocol.component.html',
    styleUrls: ['ForecastNewProgramProtocol.component.css']
})

export class ForecastNewProgramProtocolComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    forecastId: number;
    forecastProtocolList: any[] = ["1 Year", "2 Years"];
    selectedProtocol = 0;

    constructor(private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef, private _fb: FormBuilder) { }

    ngOnInit() {

    }

    handleChangeProtocol(index) {
        this.selectedProtocol = index;
    }

    onCloseModal() {
        this.bsModalRef.hide();
    }

    openNextModal() {
        this.bsModalRef.hide();
        this.event.emit({ type: "next" });
    }

    openPreviousModal() {
        this.bsModalRef.hide();
        this.event.emit({ type: "back" });
    }

    cancelAndSelectCurrent() {
        this.bsModalRef.hide();
        this.event.emit({ type: "back", methodology: "cancelCreateProgram" });
    }

}

