import { TestBed } from '@angular/core/testing';

import { ActivityCommentService } from './activity-comment.service';

describe('ActivityCommentService', () => {
  let service: ActivityCommentService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ActivityCommentService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
