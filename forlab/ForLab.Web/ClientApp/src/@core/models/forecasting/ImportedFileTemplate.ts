export class HistoicalConsumptionDto {
  regionId: number = null;
  laboratoryId: number = null;
  productId: number = null;
  
  // UI
  forecastCategoryName: string = null;
  regionName: string = null;
  laboratoryName: string = null;
  productName: string = null;
}

export class HistoicalServiceDataDto {
  regionId: number = null;
  laboratoryId: number = null;
  testId: number = null;
  
  // UI
  forecastCategoryName: string = null;
  regionName: string = null;
  laboratoryName: string = null;
  testName: string = null;
}

export class HistoicalTargetBaseDto {
  regionId: number = null;
  laboratoryId: number = null;
  programId: number = null;
  currentPatient: number = null;
  targetPatient: number = null;
  
  // UI
  forecastCategoryName: string = null;
  regionName: string = null;
  laboratoryName: string = null;
  programName: string = null;
}

export class HistoicalWhoBaseDto {
  countryId: number = null;
  diseaseId: number = null;
  
  // UI
  countryName: string = null;
  diseaseName: string = null;
}