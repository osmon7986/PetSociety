import { Component, Input } from '@angular/core';
import { CardComponent } from "../../../../shared/card/card.component";
import { ActivityIntroCard } from '../../interface/activityIntroCard';
import { DatePipe } from '@angular/common';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-activity-introcard',
  imports: [CardComponent, DatePipe, RouterLink],
  templateUrl: './activity-introcard.component.html',
  styleUrl: './activity-introcard.component.css'
})
export class ActivityIntrocardComponent {

  //將intro.component的activityIntroCard物件傳入此處
  @Input() activityIntroCard!: ActivityIntroCard

}
