import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardService } from '../../core/services/card.service';
import { Card } from '../../models/Card';
import { CardFrequency } from '../../models/CardFrequency';
import { CardDisplayComponent } from '../../shared/components/card-display/card-display.component';
@Component({
  selector: 'app-frequent-cards',
  imports: [CommonModule, CardDisplayComponent ],
  templateUrl: './frequent-cards.component.html',
  styleUrl: './frequent-cards.component.css'
})
export class FrequentCardsComponent implements OnInit {
  frequentCards: CardFrequency[] = [];
  isLoading: boolean = false;

  constructor (
    private cardService: CardService
  ) {}
  ngOnInit(): void {
    this.getFrequentCards();
  }

  getFrequentCards() {
    this.isLoading = true;
    let frequentCards = this.cardService.getFrequentCards().subscribe({
      next: (response: any[]) => {
        this.frequentCards = response.map(item => new CardFrequency(item));
        this.frequentCards.forEach(c => {
          let fc = this.cardService.getCardById(c.cardId).subscribe({
            next: (response: any) => { c.card = response; },
            error: (e) => console.error(e),
            //complete: () => console.info('complete')
          });
        });
      },
      error: (e) => console.error(e),
      //complete: () => console.info('complete')
    });
    this.isLoading = false;
  }

}
