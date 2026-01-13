import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivityInfoComponent } from './activity-info.component';

describe('ActivityInfoComponent', () => {
  let component: ActivityInfoComponent;
  let fixture: ComponentFixture<ActivityInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActivityInfoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ActivityInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
