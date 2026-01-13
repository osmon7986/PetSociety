import { Component, inject } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastService } from '../../../member/services/toast.service';
import { routes } from '../../../../app.routes';

@Component({
  selector: 'app-registration',
  imports: [ReactiveFormsModule],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.css'
})



export class RegistrationComponent {
  private toast = inject(ToastService);
  private router = inject(Router)
  registrationForm!: FormGroup;

  onSubmit(): void {
    this.toast.success('報名成功！');
    this.router.navigate(['/activity']);
  }
}
