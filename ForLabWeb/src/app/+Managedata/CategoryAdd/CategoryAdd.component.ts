import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { BsModalRef } from 'ngx-bootstrap';

@Component({
    selector: 'app-CateAdd',
    templateUrl: './CategoryAdd.component.html',
    styleUrls: ['./CategoryAdd.component.css']
})

export class AddSitecategoryComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    itemID: any;
    submitBtnName = "Add Category";
    categoryForm: FormGroup;
    errorMessage: any;
    catList = new Array();

    constructor(private _fb: FormBuilder, private _GlobalAPIService: GlobalAPIService, public bsModalRef: BsModalRef) {
    }

    getSiteCategories() {
        this._GlobalAPIService.getDataList('SiteCategory').subscribe((data) => {
            this.catList = data
        }), err => {
            console.log(err);
        }
    }

    ngOnInit() {
        this.categoryForm = this._fb.group({
            CategoryID: 0,
            categoryName: ['', Validators.compose([Validators.required, Validators.maxLength(64)])]
        })
        if (this.itemID > 0) {
            this.submitBtnName = "Update Category";
            this._GlobalAPIService.getDatabyID(this.itemID, 'SiteCategory').subscribe((resp) => {
                this.categoryForm.setValue({
                    CategoryID: resp["categoryID"],
                    categoryName: resp["categoryName"]
                });
            }, error => this.errorMessage = error);
        }
    }

    save() {
        if (!this.itemID) {
            this._GlobalAPIService.postAPI(this.categoryForm.value, 'SiteCategory').subscribe((data) => {
                if (data["_body"] != "0") {
                    this._GlobalAPIService.SuccessMessage("SiteCategory Saved Successfully");
                    this.event.emit(this.categoryForm.value);
                    this.bsModalRef.hide();
                } else {
                    this._GlobalAPIService.FailureMessage("SiteCategory already exists");
                }
            }, error => this.errorMessage = error)
        } else {
            this._GlobalAPIService.putAPI(this.itemID, this.categoryForm.value, 'SiteCategory').subscribe((data) => {
                if (data["_body"] == "Success") {
                    this._GlobalAPIService.SuccessMessage("SiteCategory Updated Successfully");
                    this.event.emit(this.categoryForm.value);
                    this.bsModalRef.hide();
                } else {
                    this._GlobalAPIService.FailureMessage("SiteCategory already exists");
                }
            }, error => this.errorMessage = error)
        }
    }

    onCloseModal() {
        this.bsModalRef.hide();
    }
}  