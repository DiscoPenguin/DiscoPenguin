import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Card } from '../../../models/Card';

@Component({
  imports: [ CommonModule ],
  selector: 'app-card-display',
  templateUrl: './card-display.component.html',
  styleUrls: ['./card-display.component.css']
})
export class CardDisplayComponent {
  @Input() card!: Card | null;
  @Input() size: 'xs' | 's' | 'm' | 'l' | 'xl' = 'm'; // Default size
  @Input() totalQuantity: number = -1;
  @Input() frequency: number = -1;
  @Input() showFront: boolean = false;

  alignment(cardType: string | undefined) : string {
    if (
      cardType == 'Ground' ||
      cardType == 'Character' ||
      cardType == 'Space'
    )
    { return 'vertical'; }
    else { return 'horizontal'; }
  }
  toggleCard() {
    this.showFront = !this.showFront;
  }
}
