import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

import { CardService } from '../../../core/services/card.service';
import { Card } from '../../../models/Card';
import { CardImageButtonComponent } from "../../../shared/components/card-image-button/card-image-button.component";

@Component({
  imports: [CommonModule, ReactiveFormsModule, CardImageButtonComponent],
  selector: 'app-card-search',
  templateUrl: './card-search.component.html',
  styleUrls: ['./card-search.component.css']
})
export class CardSearchComponent {
  searchForm: FormGroup;
  results: Card[] = [];
  totalResults: number = 0;
  pageNumber: number = 1;
  pageSize: number = 10;
  
  @Output() addCard = new EventEmitter<Card>();
  @Output() removeCard = new EventEmitter<Card>();

  constructor(private fb: FormBuilder, private cardService: CardService) {
    this.searchForm = this.fb.group({
      searchTerm: [''],
      useLikeness: true,
      sortField: ['Name'],
      sortDirection: ['asc'],
      pageNumber: [this.pageNumber],
      pageSize: [this.pageSize]
    });
  }
  addCardtoDeck(card: Card) {
    this.addCard.emit(card);
  }
  removeCardfromDeck(card: Card) {
    this.removeCard.emit(card);
  }
  onSubmit() {
    const searchCriteria = this.searchForm.value;
    this.cardService.searchCards(searchCriteria).subscribe(response => {
      this.results = response.items;
      this.totalResults = response.totalCount;
    });
  }

  imagePath(c: Card | undefined) : string {
    if (!c || !c.expansionSet || ! c.imageFile) {
      return "/assets/images/cards/cardback.jpg";
    }
    return "/assets/images/cards/" + c.expansionSet + "/" + c.imageFile + ".jpg";
  }

  forceIcon(side: string | undefined): string {
    let forceIconPath = '/assets/icons/LightSaber_' + side + '.png';
    return forceIconPath;
  }

}
