import { ActivityComment } from './../../interface/activityComment';
import { ActivityCommentService } from './../../services/activity-comment.service';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastService } from '../../../member/services/toast.service';

@Component({
  selector: 'app-activity-new-comment',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './activity-new-comment.component.html',
  styleUrls: ['./activity-new-comment.component.css']
})
export class ActivityNewCommentComponent implements OnInit {
  activityId!: number;

  commentForm = new FormGroup({
    comment: new FormControl('', [Validators.required])
  });

  constructor(
    private activityCommentService: ActivityCommentService,
    private route: ActivatedRoute,
    private toast: ToastService,
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.activityId = +id;
      }
    });
  }

  onSubmit() {
    if (this.commentForm.valid && this.commentForm.value.comment && this.activityId) {
      console.log('Comment submitted:', this.commentForm.value.comment);
      const activityComment: ActivityComment = {
        activityId: this.activityId,
        activityComment1: this.commentForm.value.comment
      };

      this.activityCommentService.createActivityComment(activityComment).subscribe({
        next: (res) => {
          this.toast.success('留言成功');
          this.commentForm.reset();
        },
        error: (err) => {
          this.toast.error(err.error?.message);
          console.log(err);
        }
      });
    }
  }

  example() {
    this.commentForm.setValue({
      comment: '活動辦成這樣，我看你這輩子就這樣了，真可悲！'
    });
  }
}
