import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class CountryDiseaseIncidentDto extends BaseEntityDto {
  countryId: number = null;
  diseaseId: number = null;
  year: number = null;
  incidence: number = null;
  incidencePer1kPopulation: number = null;
  incidencePer100kPopulation: number = null;
  prevalenceRate: number = null;
  prevalenceRatePer1kPopulation: number = null;
  prevalenceRatePer100kPopulation: number = null;
  note: string = null;

  // UI
  countryName: string = null;
  diseaseName: string = null;
}


export class CountryDiseaseIncidentFilterDto extends BaseFilter {
  countryId: number = null;
  diseaseId: number = null;
  year: number = null;
  incidence: number = null;
  incidencePer1kPopulation: number = null;
  incidencePer100kPopulation: number = null;
  prevalenceRate: number = null;
  prevalenceRatePer1kPopulation: number = null;
  prevalenceRatePer100kPopulation: number = null;
}