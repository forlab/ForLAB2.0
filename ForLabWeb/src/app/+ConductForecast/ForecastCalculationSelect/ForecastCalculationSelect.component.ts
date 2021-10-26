import { Component, OnInit, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';

@Component({
    selector: 'app-forecast-calculation-select',
    templateUrl: './ForecastCalculationSelect.component.html',
    styleUrls: ['ForecastCalculationSelect.component.css']
})

export class ForecastCalculationSelectComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    currentCalculation = 0;
    forecastId: number;
    Methodology: string = "";
    Scaleup: number = 0;
    WastageRate: number = 0;
    Months: number = 3;
    Forecastcalcmethod: string = "Simplemovingaverage";
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
        if (this.forecastId > 0) {
            this.currentCalculation = -1;
            this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "GetbyId").subscribe((resp) => {
                this.Methodology = resp["methodology"];
                this.WastageRate = resp["westage"];
                this.Scaleup = resp["scaleup"];
                this.Forecastcalcmethod = resp["method"];
                switch (resp["method"]) {
                    case "Specifiedpercentagegrowth":
                        this.currentCalculation = 0;
                        break;
                    case "Simplemovingaverage":
                        this.currentCalculation = 1;
                        break;
                    case "Weightedmovingaverage":
                        this.currentCalculation = 2;
                        break;
                    case "Simplelinearregression":
                        this.currentCalculation = 3;
                        break;
                    default:
                        break;
                }
            });
        }
    }

    save() {
    }

    handleSelectCalculation(index) {
        this.currentCalculation = index;
    }

    onCloseModal() {
        this.bsModalRef.hide();
    }

    openNextModal() {
        this._APIwithActionService.getDatabyID(this.forecastId, "Conductforecast", "Calculateforecast", "MethodType=" + this.Forecastcalcmethod + "," + this.WastageRate + "," + this.Scaleup + "," + this.Months).subscribe((data) => {
            console.log('data', data);
            this.bsModalRef.hide();
            if (this.Methodology == "CONSUMPTION") {
                this.event.emit({ type: "next" });
                // this._router.navigate(["/Forecast/ForecastChartnew", this.forecastId,]);
            } else if (this.Methodology == "SERVICE STATSTICS") {
                this.event.emit({ type: "next" });
                // this._router.navigate(["/Forecast/ForecastChartservice", this.forecastId,]);
            }
        });
    }

    openPreviousModal() {
        this.bsModalRef.hide();
        this.event.emit({ type: "back" });
    }

}

