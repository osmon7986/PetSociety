import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivityCommentComponent } from './activity-comment.component';

describe('ActivityCommentComponent', () => {
  let component: ActivityCommentComponent;
  let fixture: ComponentFixture<ActivityCommentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ActivityCommentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ActivityCommentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});