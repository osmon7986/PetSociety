import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BreadcrumbBannerComponent } from './breadcrumb-banner.component';

describe('BreadcrumbBannerComponent', () => {
  let component: BreadcrumbBannerComponent;
  let fixture: ComponentFixture<BreadcrumbBannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BreadcrumbBannerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BreadcrumbBannerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
