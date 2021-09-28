class RegistredObject {
  objectName: string;
  acceptableSheetNames: string[];
}

export var RegistredObjects: RegistredObject[] = [
  // Lookups
  { objectName: 'PatientGroup', acceptableSheetNames: ['PatientGroup', 'Patient Group', 'Patient-Group', 'Patient Groups'] },
  { objectName: 'TestingArea', acceptableSheetNames: ['TestingArea', 'Testing Area', 'Testing-Area', 'Testing Areas'] },
  { objectName: 'LaboratoryLevel', acceptableSheetNames: ['LaboratoryLevel', 'Laboratory Level', 'Laboratory-Level', 'Laboratory Levels'] },
  { objectName: 'LaboratoryCategory', acceptableSheetNames: ['LaboratoryCategory', 'Laboratory Category', 'Laboratory-Category', 'Laboratory Categories'] },
  { objectName: 'Laboratory', acceptableSheetNames: ['Laboratory', 'Laboratories'] },
  { objectName: 'Country', acceptableSheetNames: ['Country', 'Countries'] },
  { objectName: 'Region', acceptableSheetNames: ['Region', 'Regions'] },
  // Testing
  { objectName: 'Test', acceptableSheetNames: ['Test', 'Tests'] },
  // Vendor
  { objectName: 'Vendor', acceptableSheetNames: ['Vendor', 'Vendors'] },
  { objectName: 'VendorContact', acceptableSheetNames: ['VendorContact', 'Vendor Contact', 'Vendor-Contact', 'Vendor Contacts'] },
  // Product
  { objectName: 'Product', acceptableSheetNames: ['Product', 'Products'] },
  { objectName: 'Instrument', acceptableSheetNames: ['Instrument', 'Instruments'] },
  { objectName: 'CountryProductPrice', acceptableSheetNames: ['CountryProductPrice', 'CountryProductPrices'] },
  { objectName: 'RegionProductPrice', acceptableSheetNames: ['RegionProductPrice', 'RegionProductPrices'] },
  { objectName: 'LaboratoryProductPrice', acceptableSheetNames: ['LaboratoryProductPrice', 'LaboratoryProductPrices'] },
  { objectName: 'ProductUsage', acceptableSheetNames: ['ProductUsage', 'ProductUsages'] },
  { objectName: 'TestUsage', acceptableSheetNames: ['TestUsage', 'TestUsages'] },
  // Laboratory
  { objectName: 'LaboratoryConsumption', acceptableSheetNames: ['LaboratoryConsumption', 'LaboratoryConsumptions', 'Histoical Consumption'] },
  { objectName: 'LaboratoryTestService', acceptableSheetNames: ['LaboratoryTestService', 'LaboratoryTestServices', 'Historical Service Data'] },
  { objectName: 'LaboratoryInstrument', acceptableSheetNames: ['LaboratoryInstrument', 'LaboratoryInstruments'] },
  { objectName: 'LaboratoryPatientStatistic', acceptableSheetNames: ['LaboratoryPatientStatistic', 'LaboratoryPatientStatistics'] },
  // DiseaseProgram
  { objectName: 'Program', acceptableSheetNames: ['Program', 'Programs'] },
  // { objectName: 'ProgramTest', acceptableSheetNames: ['ProgramTest', 'ProgramTests'] },
  { objectName: 'TestingAssumptionParameter', acceptableSheetNames: ['TestingAssumptionParameter', 'TestingAssumptionParameters'] },
  { objectName: 'ProductAssumptionParameter', acceptableSheetNames: ['ProductAssumptionParameter', 'ProductAssumptionParameters'] },
  { objectName: 'PatientAssumptionParameter', acceptableSheetNames: ['PatientAssumptionParameter', 'PatientAssumptionParameters'] },
  // Disease
  { objectName: 'CountryDiseaseIncident', acceptableSheetNames: ['CountryDiseaseIncident', 'CountryDiseaseIncidents'] },
  // { objectName: 'DiseaseTestingProtocol', acceptableSheetNames: ['DiseaseTestingProtocol', 'DiseaseTestingProtocols'] },
  { objectName: 'Disease', acceptableSheetNames: ['Disease', 'Diseases'] },
];