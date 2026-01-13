import { ActivatedRoute, Router } from '@angular/router';
import { CoursePlayback } from '../../interfaces/course-playback';
import { CourseService } from '../../services/course.service';
import { Component, inject, OnInit } from '@angular/core';
import { SafeUrlPipe } from '../../../../shared/pipes/safe-url.pipe';
import { ChapterPlayback } from '../../interfaces/chapter-playback';
import { BackButtonComponent } from "../../../../shared/back-button/back-button.component";
import { ChapterRecord } from '../../interfaces/chapter-record';
import { YouTubePlayer } from '@angular/youtube-player';

@Component({
  selector: 'app-course-playback',
  imports: [BackButtonComponent, YouTubePlayer],
  templateUrl: './course-playback.component.html',
  styleUrl: './course-playback.component.css'
})
export class CoursePlaybackComponent implements OnInit {

  playback: CoursePlayback | null = null; // 非同步API，先宣告為null
  backButtonLabel = '返回我的課程'
  record!: ChapterRecord;

  private route = inject(ActivatedRoute)

  constructor(
    private courseService: CourseService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.courseService.goToTop();
    const courseDetailId = +this.activatedRoute.snapshot.paramMap.get('courseDetailId')!;
    console.log(courseDetailId);
    this.courseService.getCoursePlayback(courseDetailId).subscribe((data) => {
      if (data) {
        this.playback = data;
        console.log(this.playback.currentChapter?.videoUrl);
      } else {
        this.router.navigate(['academy', 'my-course'])
      }
    })

    this.record = {
      courseDetailId,
      chapterId: 0,
    }
  }

  selectChapter(chapter: ChapterPlayback) {
    // 確認有切換章節，才更換目前播放內容和修改觀看紀錄
    if (chapter.chapterId != this.playback?.currentChapter?.chapterId) {
      // 更新目前章節撥放器影片和章節內容
      this.playback!.currentChapter = chapter;

      // 更新會員章節觀看紀錄
      this.record.chapterId = chapter.chapterId;
      console.log(this.record.chapterId);
      console.log(this.record);
      this.courseService.updateChapterRec(this.record).subscribe((response) => {
        if (response.status === 204) {
          console.log('修改成功');
        }
      })
    }
  }
  // 進入章節測驗
  navigateToQuiz() {
    const chapterId = this.playback?.currentChapter?.chapterId;
    if (chapterId) {
      this.router.navigate(['../', 'quiz', chapterId,], { relativeTo: this.route })
    }

  }
  goback() {
    this.router.navigate(['academy', 'my-course'])
  }

}
