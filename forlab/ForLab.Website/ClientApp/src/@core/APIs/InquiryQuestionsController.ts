import { BaseURL } from '../config';

export const InquiryQuestionsController = {
    GetAll: BaseURL + `/api/InquiryQuestions/GetAll`,
    GetAllAsDrp: BaseURL + `/api/InquiryQuestions/GetAllAsDrp`,
    GetInquiryQuestionDetails: BaseURL + `/api/InquiryQuestions/GetInquiryQuestionDetails`,
    CreateInquiryQuestion: BaseURL + `/api/InquiryQuestions/CreateInquiryQuestion`,
    UpdateInquiryQuestion: BaseURL + `/api/InquiryQuestions/UpdateInquiryQuestion`,
    UpdateIsActive: BaseURL + `/api/InquiryQuestions/UpdateIsActive`,
    RemoveInquiryQuestion: BaseURL + `/api/InquiryQuestions/RemoveInquiryQuestion`,
    CreateInquiryQuestionReply: BaseURL + `/api/InquiryQuestions/CreateInquiryQuestionReply`,
}