import { BaseURL } from '../config';

export const DashboardController = {
    // Main Dashboard
    MainCardCounts: BaseURL + `/api/Dashboard/MainCardCounts`,
    NumberOfLaboratories: BaseURL + `/api/Dashboard/NumberOfLaboratories`,
    NumberOfDiseases: BaseURL + `/api/Dashboard/NumberOfDiseases`,
    InquiryQuestionsChart: BaseURL + `/api/Dashboard/InquiryQuestionsChart`,
    UsersChart: BaseURL + `/api/Dashboard/UsersChart`,
    LaboratoriesChart: BaseURL + `/api/Dashboard/LaboratoriesChart`,

    // Forecast Dashboard
    ForecastCardCounts: BaseURL + `/api/Dashboard/ForecastCardCounts`,
    ForecastsChart: BaseURL + `/api/Dashboard/ForecastsChart`,
    NumberOfForecasts: BaseURL + `/api/Dashboard/NumberOfForecasts`,
} 