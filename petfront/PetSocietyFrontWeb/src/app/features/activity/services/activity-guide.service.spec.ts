import { TestBed } from '@angular/core/testing';

import { ActivityGuideService } from './activity-guide.service';

describe('ActivityGuideService', () => {
  let service: ActivityGuideService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ActivityGuideService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
