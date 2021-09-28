import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ProductDto } from 'src/@core/models/product/Product';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { takeUntil } from 'rxjs/operators';
import { ProductTypeEnum } from 'src/@core/models/enum/Enums';
import { ProductUsagesComponent } from '../../product-usage/product-usages/product-usages.component';
import { RegionProductPricesComponent } from '../../region-product-price/region-product-prices/region-product-prices.component';
import { CountryProductPricesComponent } from '../../country-product-price/country-product-prices/country-product-prices.component';
import { LaboratoryProductPricesComponent } from '../../laboratory-product-price/laboratory-product-prices/laboratory-product-prices.component';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.sass']
})
export class ProductDetailsComponent extends BaseService implements OnInit {

  // Children
  @ViewChild(CountryProductPricesComponent, { static: false }) countryProductPricesComponent: CountryProductPricesComponent;
  @ViewChild(RegionProductPricesComponent, { static: false }) regionProductPricesComponent: RegionProductPricesComponent;
  @ViewChild(LaboratoryProductPricesComponent, { static: false }) laboratoryProductPricesComponent: LaboratoryProductPricesComponent;
  @ViewChild(ProductUsagesComponent, { static: false }) productUsagesComponent: ProductUsagesComponent;

  productId: number;
  productDto: ProductDto = new ProductDto();
  loadingProduct = false;
  productTypeEnum = ProductTypeEnum;

  constructor(public injector: Injector) {
    super(injector);

    if (this.router.url.includes('details')) {
      this.activatedRoute.paramMap.subscribe(params => {
        this.productId = Number(params.get('productId'));
        this.loadDataById(this.productId);
      });
    }

  }

  ngOnInit(): void {
  }

  loadDataById(id: number) {
    this.loadingProduct = true;
    const url = ProductsController.GetProductDetails;
    let params: QueryParamsDto[] = [
      { key: 'productId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.productDto = res.data;
          this.loadingProduct = false;

          if (this.productDto.createdBy != this.loggedInUser?.id) {
            this.countryProductPricesComponent.columns = this.countryProductPricesComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            this.regionProductPricesComponent.columns = this.regionProductPricesComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            this.laboratoryProductPricesComponent.columns = this.laboratoryProductPricesComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            if(this.productUsagesComponent) {
              this.productUsagesComponent.columns = this.productUsagesComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            }
          }

        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

}
