import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { ActivityNewCommentComponent } from './activity-new-comment.component';

describe('ActivityNewCommentComponent', () => {
  let component: ActivityNewCommentComponent;
  let fixture: ComponentFixture<ActivityNewCommentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ReactiveFormsModule,
        NoopAnimationsModule,
        ActivityNewCommentComponent
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ActivityNewCommentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have an invalid form when comment is empty', () => {
    expect(component.commentForm.valid).toBeFalsy();
  });

  it('should have a valid form when comment is not empty', () => {
    component.commentForm.controls['comment'].setValue('A test comment');
    expect(component.commentForm.valid).toBeTruthy();
  });

  it('should call onSubmit method', () => {
    spyOn(component, 'onSubmit');
    const button = fixture.nativeElement.querySelector('button');
    component.commentForm.controls['comment'].setValue('A test comment');
    fixture.detectChanges();
    button.click();
    expect(component.onSubmit).toHaveBeenCalled();
  });

  it('should reset the form after submission', () => {
    component.commentForm.controls['comment'].setValue('A test comment');
    component.onSubmit();
    expect(component.commentForm.value.comment).toBeNull();
  });
});