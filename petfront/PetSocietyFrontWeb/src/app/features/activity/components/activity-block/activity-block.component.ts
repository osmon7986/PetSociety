import { ActivityGuideService } from '../../services/activity-guide.service';
import { Component, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-activity-block',
  imports: [],
  templateUrl: './activity-block.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrl: './activity-block.component.css'
})
export class ActivityBlockComponent {

  //記錄傳入html
  contentHtml = ''

  constructor(private activatedRoute: ActivatedRoute, private activityGuideService: ActivityGuideService) { }


  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((data: ParamMap) => {
      //自url獲得活動id
      const inputId = data.get('id')
      var activityId = inputId ? + inputId : 0

      this.activityGuideService.getGuideApi(activityId).subscribe
        ((data) => {
          try {
            this.contentHtml = data.activityEditorHtml;
          } catch (error) {
            console.log(error);
          }
        })

    }



    );


  }
}
