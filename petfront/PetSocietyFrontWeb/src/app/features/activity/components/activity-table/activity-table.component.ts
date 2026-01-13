import { Component, Input } from '@angular/core';
import { TableComponent } from "../../../../shared/table/table.component";
import { RouterLink } from "@angular/router";
import { ActivityTable } from '../../interface/activityTable';
import { DatePipe } from '@angular/common';


@Component({
  selector: 'app-activity-table',
  imports: [TableComponent, RouterLink, DatePipe],
  templateUrl: './activity-table.component.html',
  styleUrl: './activity-table.component.css'
})
export class ActivityTableComponent {

  //將intro.component的activityJson物件傳入此處
  @Input() activityTable!: ActivityTable[];

}
