import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChapterQuizComponent } from './chapter-quiz.component';

describe('ChapterQuizComponent', () => {
  let component: ChapterQuizComponent;
  let fixture: ComponentFixture<ChapterQuizComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChapterQuizComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChapterQuizComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
