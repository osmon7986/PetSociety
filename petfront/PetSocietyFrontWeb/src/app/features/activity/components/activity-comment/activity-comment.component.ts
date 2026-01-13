import { ActivityCommentService } from './../../services/activity-comment.service';
import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShowComment } from '../../interface/ShowComment';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-activity-comment',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './activity-comment.component.html',
  styleUrls: ['./activity-comment.component.css']
})
export class ActivityCommentComponent implements OnInit {

  constructor(private ActivityCommentService: ActivityCommentService, private activatedRoute: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((data: ParamMap) => {
      //自url獲得活動id
      const inputId = data.get('id')
      var activityId = inputId ? + inputId : 0
      this.ActivityCommentService.getActivityCommentsByActivityId(activityId).subscribe(data => {
        this.showComment = data;
      });
    })
  }
  @Input() showComment: ShowComment[] = [];


}
