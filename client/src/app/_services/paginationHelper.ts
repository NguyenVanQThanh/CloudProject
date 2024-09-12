import { HttpClient, HttpParams, HttpResponse } from "@angular/common/http";
import { PaginatedResults } from "../_models/pagination";
import { map } from "rxjs";
import { signal } from "@angular/core";

export function getPaginationResult<T>(url: string, params: HttpParams, http: HttpClient) {
  const paginatedResult: PaginatedResults<T> = new PaginatedResults<T>();
  return http
    .get<T>(url, { observe: 'response', params })
    .pipe(
      map((response) => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination);
        }
        return paginatedResult;
      })
    );
}

export function setPaginationHeaders(pageNumber: number, pageSize: number) {
  let params = new HttpParams();
  if (pageNumber && pageSize) {
    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
  }
  return params;
}
export function setPaginatedResponse<T>(response: HttpResponse<T>,
  paginatedResultSignal: ReturnType<typeof signal<PaginatedResults<T> | null>>){
    paginatedResultSignal.set({
      items: response.body as T,
      pagination: JSON.parse(response.headers.get('Pagination')!)
    })
  }

