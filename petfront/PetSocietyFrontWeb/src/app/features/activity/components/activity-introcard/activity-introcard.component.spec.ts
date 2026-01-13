import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivityIntrocardComponent } from './activity-introcard.component';

describe('ActivityIntrocardComponent', () => {
  let component: ActivityIntrocardComponent;
  let fixture: ComponentFixture<ActivityIntrocardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActivityIntrocardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ActivityIntrocardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
