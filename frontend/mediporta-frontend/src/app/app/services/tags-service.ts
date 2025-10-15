import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config-service';

export interface TagDto {
  name: string;
  count: number;
  percentage: number;
}

@Injectable({ providedIn: 'root' })
export class TagsService {
  constructor(private http: HttpClient, private config: ConfigService) {}

  getTags(pageNumber: number, pageSize: number, sortBy: string, ascending: boolean): Observable<TagDto[]> {
    const params = new HttpParams()
      .set('pageNumber', String(pageNumber))
      .set('pageSize', String(pageSize))
      .set('sortBy', sortBy)
      .set('ascending', String(ascending));
    const url = `${this.config.apiBaseUrl}/tags`;
    return this.http.get<TagDto[]>(url, { params });
  }

  refreshTags() {
    const url = `${this.config.apiBaseUrl}/tags/refresh`;
    return this.http.post(url, {});
  }
}
