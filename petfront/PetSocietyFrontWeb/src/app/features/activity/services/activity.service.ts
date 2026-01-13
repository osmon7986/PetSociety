import { ActivityInfo } from '../interface/activityInfo';
import { CreateGuideData } from './../interface/activityGuideData';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ActivityTable } from '../interface/activityTable';
import { BehaviorSubject, map, Observable, shareReplay, switchMap } from 'rxjs';
import { ActivityApply } from '../interface/activityApply';
import { CreateActivityPayload } from '../interface/createActivityPayload';
import { ShowMarker } from '../interface/showMarker';

@Injectable({
  providedIn: 'root'
})
export class ActivityService {
  private http = inject(HttpClient);
  private readonly api = 'https://localhost:7138';
  private rawData$ = this.http.get<ActivityTable[]>(`${this.api}/Activity/intro`)
    .pipe(
      shareReplay(1)
    );

  private categorySubject = new BehaviorSubject<string>('所有活動');

  constructor() { }



  tableData$ = this.categorySubject.pipe(
    switchMap(category => {
      const params: any = {};
      if (category !== '所有活動') {
        params['category'] = category;
      }
      return this.http.get<ActivityTable[]>(`${this.api}/Activity/intro`, { params });
    }),
    shareReplay(1)
  );

  adData$ = this.rawData$.pipe(
    map(items => items.slice(0, 4).map(i => ({ activityId: i.activityId, title: i.title, location: i.location, startTime: i.startTime, activityImg: i.activityImg })))
  );

  setCategory(category: string): void {
    this.categorySubject.next(category);
  }


  createActivity(applyData: ActivityApply, guideData: CreateGuideData): Observable<any> {

    const payload: CreateActivityPayload = {
      applyData: applyData,
      guideData: guideData
    };
    return this.http.post<ActivityApply>(`${this.api}/Activity/NewActivity`, payload);
  }
  getActivityInfo(activityId: number) {
    return this.http.get<ActivityInfo>(`${this.api}/Activity/info/${activityId}`,
      {
        params: {
          activityId: activityId.toString()
        }
      }
    );
  };

  getMap(activityId: number) {
    return this.http.get<ShowMarker>(`${this.api}/Activity/map/${activityId}`);
  };
}
