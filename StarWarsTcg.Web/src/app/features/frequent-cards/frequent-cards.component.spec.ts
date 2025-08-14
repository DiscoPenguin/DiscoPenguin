import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FrequentCardsComponent } from './frequent-cards.component';

describe('FrequentCardsComponent', () => {
  let component: FrequentCardsComponent;
  let fixture: ComponentFixture<FrequentCardsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FrequentCardsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FrequentCardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
