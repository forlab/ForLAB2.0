
export interface Pagination {
   pageIndex?;
   pageSize?;
   filter?;
}
export class PaginationResult<T> {
   result: T;
   pagination: Pagination;
}