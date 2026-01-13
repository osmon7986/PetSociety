import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivityCalenderComponent } from './activity-calender.component';

describe('ActivityCalenderComponent', () => {
  let component: ActivityCalenderComponent;
  let fixture: ComponentFixture<ActivityCalenderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActivityCalenderComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ActivityCalenderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
