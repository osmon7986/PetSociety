import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivityGuideData, CreateGuideData } from '../interface/activityGuideData';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ActivityGuideService {

  constructor(private httpClient: HttpClient) { }

  api = 'https://localhost:7138';

  getGuideApi(activityId: number) {
    //Todo調整get的interface
    return this.httpClient.get<ActivityGuideData>(`${this.api}/ActivityGuide/${activityId}`,
      {
        params: {
          activityId: activityId.toString()
        }
      }
    );
  }
  //需重新調整為編輯功能
  postGuideApi(createGuideData: CreateGuideData): Observable<CreateGuideData> {
    return this.httpClient.post<CreateGuideData>(`${this.api}/ActivityGuide/GuideEditor`, createGuideData);
  }



}
