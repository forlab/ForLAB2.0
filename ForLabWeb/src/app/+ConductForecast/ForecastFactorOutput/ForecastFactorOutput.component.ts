import { Component, OnInit, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';

@Component({
    selector: 'app-forecast-factor-output',
    templateUrl: './ForecastFactorOutput.component.html',
    styleUrls: ['ForecastFactorOutput.component.css']
})

export class ForecastFactorOutputComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    // currentConductType = 0;
    forecastId: number;
    forecastfactor: FormGroup;
    loading = false;
    constructor(
        private _fb: FormBuilder,
        private _router: Router,
        private _APIwithActionService: APIwithActionService,
        public bsModalRef: BsModalRef,
        private modalService: BsModalService
    ) {
    }

    ngAfterViewChecked() {
    }

    ngOnInit() {
        if (this.forecastId > 0) {
            this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "GetbyId").subscribe((resp) => {
                this.forecastfactor.patchValue({
                    WastageRate: resp["westage"],
                    Scaleup: resp["scaleup"],
                });
            });
        }
        this.forecastfactor = this._fb.group({
            WastageRate: [0, Validators.required],
            Scaleup: [0, Validators.required],
        });
    }

    save() {
    }


    onCloseModal() {
        this.bsModalRef.hide();
    }

    openNextModal() {
        this.loading = true;
        var data = this.forecastfactor.controls["WastageRate"].value + "," + this.forecastfactor.controls["Scaleup"].value;
        this._APIwithActionService.putAPI(this.forecastId, data, "Forecsatinfo", "Put02").subscribe((data1) => {
          
            this._APIwithActionService.getDatabyID(this.forecastId, "Conductforecast", "Calculateforecast", "MethodType=linearregression," + this.forecastfactor.controls["WastageRate"].value + "," + this.forecastfactor.controls["Scaleup"].value + ",3").subscribe((data) => {
                this.loading = false;
            this.bsModalRef.hide();
            this.event.emit({ type: "next" });
            });
        });
    }

    openPreviousModal() {
        this.bsModalRef.hide();
        this.event.emit({ type: "back" });
    }

}

