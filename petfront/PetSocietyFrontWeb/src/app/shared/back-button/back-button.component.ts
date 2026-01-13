import { Component, EventEmitter, Input, Output } from '@angular/core';
import { OutletContext, Router } from '@angular/router';

@Component({
  selector: 'app-back-button',
  imports: [],
  templateUrl: './back-button.component.html',
  styleUrl: './back-button.component.css'
})
export class BackButtonComponent {
  @Input() buttonLabel: string = '文字';
  @Output() back = new EventEmitter<void>();

  constructor(private router: Router) { }

  /**Navigate to last page */
  onBack() {
    this.back.emit();
  }
}
