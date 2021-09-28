import { BaseURL } from '../config';

export const FrequentlyAskedQuestionsController = {
  GetAll: BaseURL + `/api/FrequentlyAskedQuestions/GetAll`,
  GetAllAsDrp: BaseURL + `/api/FrequentlyAskedQuestions/GetAllAsDrp`,
  GetFrequentlyAskedQuestionDetails: BaseURL + `/api/FrequentlyAskedQuestions/GetFrequentlyAskedQuestionDetails`,
  CreateFrequentlyAskedQuestion: BaseURL + `/api/FrequentlyAskedQuestions/CreateFrequentlyAskedQuestion`,
  UpdateFrequentlyAskedQuestion: BaseURL + `/api/FrequentlyAskedQuestions/UpdateFrequentlyAskedQuestion`,
  UpdateIsActive: BaseURL + `/api/FrequentlyAskedQuestions/UpdateIsActive`,
  RemoveFrequentlyAskedQuestion: BaseURL + `/api/FrequentlyAskedQuestions/RemoveFrequentlyAskedQuestion`,
}