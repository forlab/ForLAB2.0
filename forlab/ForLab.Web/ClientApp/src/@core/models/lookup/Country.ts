import { Lookup } from '../common/Lookup';
import { BaseFilter } from '../common/BaseFilter';

export class CountryDto extends Lookup {
  continentId: number = null;
  countryPeriodId: number = null;
  shortCode: string = null;
  shortName: string = null;
  nativeName: string = null;
  flag: string = null;
  currencyCode: string = null;
  callingCode: string = null;
  latitude: string = null;
  longitude: string = null;
  population: number = null;

  // UI
  continentName: string = null;
  countryPeriodName: string = null;
}


export class CountryFilterDto extends BaseFilter {
  name: string = null;
  continentId: number = null;
  countryPeriodId: number = null;
  shortCode: string = null;
  nativeName: string = null;
  currencyCode: string = null;
  callingCode: string = null;
  latitude: string = null;
  longitude: string = null;
  population: number = null;
}