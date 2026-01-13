import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestTokenComponent } from './test-token.component';

describe('TestTokenComponent', () => {
  let component: TestTokenComponent;
  let fixture: ComponentFixture<TestTokenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestTokenComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestTokenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
