import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { PaginatedResult } from '../_models/pagination';


export function getPaginatedResult<T>(url, params, http: HttpClient) {
  const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

  return http.get<T>(url, { observe: 'response', params })
            .pipe(map((response) => {
                paginatedResult.result = response.body;
                // if there is a header
                if (response.headers.get('Pagination') !== null) {
                  paginatedResult.pagination = JSON.parse(
                    response.headers.get('Pagination')
                  );
                }
                // this means return the paginatedResult which is of type PaginatedResult<T> as observable
                return paginatedResult;
              })
    );
}

export function getPaginationHeaders(pageNumber: number, itemsPerPage: number) {
  let params = new HttpParams();
  // set up the query params
  params = params.append('pageNumber', pageNumber.toString());
  params = params.append('itemsPerPage', itemsPerPage.toString());

  return params;
}
