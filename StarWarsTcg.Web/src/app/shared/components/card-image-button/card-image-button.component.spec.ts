import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardImageButtonComponent } from './card-image-button.component';

describe('CardImageButtonComponent', () => {
  let component: CardImageButtonComponent;
  let fixture: ComponentFixture<CardImageButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardImageButtonComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CardImageButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
