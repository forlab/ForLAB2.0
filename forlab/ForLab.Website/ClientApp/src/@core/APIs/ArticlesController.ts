import { BaseURL } from '../config';

export const ArticlesController = {
    GetAll: BaseURL + `/api/Articles/GetAll`,
    GetAllAsDrp: BaseURL + `/api/Articles/GetAllAsDrp`,
    GetArticleDetails: BaseURL + `/api/Articles/GetArticleDetails`,
    CreateArticle: BaseURL + `/api/Articles/CreateArticle`,
    UpdateArticle: BaseURL + `/api/Articles/UpdateArticle`,
    UpdateIsActive: BaseURL + `/api/Articles/UpdateIsActive`,
    RemoveArticle: BaseURL + `/api/Articles/RemoveArticle`,
}