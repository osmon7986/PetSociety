import { TestBed } from '@angular/core/testing';

import { ActivityCalenderService } from './activity-calender.service';

describe('ActivityCalenderService', () => {
  let service: ActivityCalenderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ActivityCalenderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
