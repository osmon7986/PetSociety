import { HttpClient } from '@angular/common/http';
import { ActivityComment } from './../interface/activityComment';
import { Injectable } from '@angular/core';
import { ShowComment } from '../interface/ShowComment';

@Injectable({
  providedIn: 'root'
})
export class ActivityCommentService {

  constructor(private httpClient: HttpClient) { }

  api = 'https://localhost:7138';

  createActivityComment(ActivityComment: ActivityComment) {
    return this.httpClient.post<ActivityComment>(`${this.api}/ActivityComment`, ActivityComment);
  }

  getActivityCommentsByActivityId(activityId: number) {
    return this.httpClient.get<ShowComment[]>(`${this.api}/ActivityComment/comment/${activityId}`,
      {
        params: {
          activityId: activityId.toString()
        }
      }
    );
  }
}
