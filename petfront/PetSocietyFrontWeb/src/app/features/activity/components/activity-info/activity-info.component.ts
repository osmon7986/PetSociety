import { ActivityService } from './../../services/activity.service';
import { ActivityInfo } from './../../interface/activityInfo';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-activity-info',
  imports: [DatePipe,],
  templateUrl: './activity-info.component.html',
  styleUrl: './activity-info.component.css'
})
export class ActivityInfoComponent implements OnInit {

  private activityService = inject(ActivityService);
  private activatedRoute = inject(ActivatedRoute);

  activityInfo!: ActivityInfo;

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((data: ParamMap) => {
      //自url獲得活動id
      const inputId = data.get('id')
      var activityId = inputId ? + inputId : 0

      this.activityService.getActivityInfo(activityId).
        subscribe((data) => {
          try {
            this.activityInfo = data;
          } catch (error) {
            console.log(error);
          }
        })

    });
  }
}
