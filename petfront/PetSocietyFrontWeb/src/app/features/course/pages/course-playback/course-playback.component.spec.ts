import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoursePlaybackComponent } from './course-playback.component';

describe('CoursePlaybackComponent', () => {
  let component: CoursePlaybackComponent;
  let fixture: ComponentFixture<CoursePlaybackComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoursePlaybackComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoursePlaybackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
