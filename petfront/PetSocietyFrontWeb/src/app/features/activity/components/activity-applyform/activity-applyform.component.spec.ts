import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivityApplyformComponent } from './activity-applyform.component';

describe('ActivityApplyformComponent', () => {
  let component: ActivityApplyformComponent;
  let fixture: ComponentFixture<ActivityApplyformComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActivityApplyformComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ActivityApplyformComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
