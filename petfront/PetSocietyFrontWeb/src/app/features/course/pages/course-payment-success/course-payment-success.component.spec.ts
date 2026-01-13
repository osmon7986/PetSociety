import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoursePaymentSuccessComponent } from './course-payment-success.component';

describe('CoursePaymentSuccessComponent', () => {
  let component: CoursePaymentSuccessComponent;
  let fixture: ComponentFixture<CoursePaymentSuccessComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoursePaymentSuccessComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoursePaymentSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
