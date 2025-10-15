import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ConfigService {
  private config: any = null;

  constructor(private http: HttpClient) {}

  loadConfig(): Promise<void> {
    return lastValueFrom(this.http.get('config.json'))
      .then(cfg => { this.config = cfg; })
      .catch(() => { this.config = { apiBaseUrl: '/api' }; });
  }

  get apiBaseUrl(): string {
    return '/api';
  }
}
