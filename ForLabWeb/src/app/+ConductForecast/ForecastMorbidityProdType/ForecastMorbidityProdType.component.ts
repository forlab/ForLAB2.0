import { Component, OnInit, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';

@Component({
    selector: 'app-forecast-morbidity-prod-type',
    templateUrl: './ForecastMorbidityProdType.component.html',
    styleUrls: ['ForecastMorbidityProdType.component.css']
})

export class ForecastMorbidityProdTypeComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    forecastId: number;
    programId: number;
    prodTypeList = new Array();
    Productass: FormGroup;
    typeID: number;
    typeName: string;
    WastageRate: number = 0;
  
    Scaleup: number = 0;
    parameterlist = new Array();
    loading = true;
    constructor(private _fb: FormBuilder, private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef, private _GlobalAPIService: GlobalAPIService) { }

    ngOnInit() {
        this._GlobalAPIService.getDataList("ProductType").subscribe((data) => {
            this.prodTypeList = data.aaData;
            console.log(this.prodTypeList);
        }), (err) => {
            console.log(err);
        };

        this.Productass = this._fb.group({
            typeID: 0,
            _productassumption: this._fb.array([]),
        });

        if (this.forecastId > 0) {
            // this._APIwithActionService.getDatabyID(this.forecastid, "Assumption", "GetforecastDynamiccontrol", "entitytype=4").subscribe((data) => {
            //     this.controlArray = data;
            // });
            // this._APIwithActionService.getDatabyID(this.forecastid, "Assumption", "getdynamicheader", "entitytype=4").subscribe((data2) => {
            //     this.HeaderArray = data2;
            this._APIwithActionService.getDatabyID(this.forecastId, 'Forecsatinfo', 'GetbyId').subscribe((resp) => {

                
                this.WastageRate = resp["westage"];
                this.Scaleup = resp["scaleup"];
            })
            this._APIwithActionService.getDatabyID(this.forecastId, "Assumption", "GetforecastproductAssumption").subscribe((data) => {
                this.loading = false;
                this.parameterlist = data[0].table;
                console.log(this.parameterlist);
                let ss = <FormArray>(
                    this.Productass.controls["_productassumption"]
                );
                for (let index = 0; index < this.parameterlist.length; index++) {
                    ss.push(this._fb.group(this.parameterlist[index]));
                }
            });
            // });
        }
    }


    delproductprice(index) {

    }

    onCreateProdType() {
        let index: number;
        let productassumption = <FormArray>(
            this.Productass.controls["_productassumption"]
        );
        let isexist: boolean = false;
        productassumption.getRawValue().forEach((x) => {
            if (x.productTypeID == this.typeID) {
                isexist = true;
                return;
            }
        });
        if (isexist == false) {
            productassumption.push(
                this._fb.group({
                    id: 0,
                    forecastinfoID: 0,
                    productTypeID: 0,
                    productTypeName: "",
                })
            );
            index = productassumption.length == 0 ? 0 : productassumption.length - 1;

            productassumption.controls[index].patchValue({
                id: 0,
                forecastinfoID: this.forecastId,
                productTypeID: this.typeID,
                productTypeName: this.typeName,
            });
        }
    }

    getProdTypeList(args) {
        this.typeID = args.target.value;
        this.typeName = args.target.options[args.target.selectedIndex].text;
    }


    onCloseModal() {
        this.bsModalRef.hide();
    }

    openNextModal() {
        let productassumption = <FormArray>(this.Productass.controls["_productassumption"]);
        let productassumptionlist = new Array();
        productassumption.getRawValue().forEach((x) => {
            productassumptionlist.push({
                ID: x.id,
                ForecastinfoID: x.forecastinfoID,
                ProductTypeID: x.productTypeID,
                UserId: 0,
            });
        });
        this._APIwithActionService.postAPI(productassumptionlist, "Assumption", "SaveproductAssumption").subscribe((data) => {
            this._APIwithActionService.getDatabyID(this.forecastId, "Conductforecast", "Calculateforecast", "MethodType=linearregression," + this.WastageRate + "," + this.Scaleup + ",3").subscribe((data) => {
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

