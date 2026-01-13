import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivityCalender } from '../interface/activityCalender';

@Injectable({
  providedIn: 'root'
})
export class ActivityCalenderService {

  constructor(private httpClient: HttpClient) { }

  api = 'https://localhost:7138';

  getCalenderApi() {
    return this.httpClient.get<ActivityCalender[]>(`${this.api}/Activity/calenders`);
  }
}
