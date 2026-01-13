import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistrationGuideComponent } from './registration-guide.component';

describe('RegistrationGuideComponent', () => {
  let component: RegistrationGuideComponent;
  let fixture: ComponentFixture<RegistrationGuideComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegistrationGuideComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegistrationGuideComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
