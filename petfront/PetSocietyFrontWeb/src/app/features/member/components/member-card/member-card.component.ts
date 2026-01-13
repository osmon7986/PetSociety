import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MemberDto } from '../../Interfaces/member.dto';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent {
  @Input() member!: MemberDto | null;

  @Input() avatarUrl: string = 'images/member/default-avatar.png';
}
