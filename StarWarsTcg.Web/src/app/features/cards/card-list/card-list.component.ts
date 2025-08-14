import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router'
import { CommonModule } from '@angular/common';
import { EditableLabelComponent } from '../../../shared/components/edit-label/edit-label.component';

import { AuthService } from '../../../core/services/auth.service';

import { FrequentCardsComponent } from '../../frequent-cards/frequent-cards.component';
import { CardSearchComponent } from '../card-search/card-search.component';
import { CardDisplayComponent } from '../../../shared/components/card-display/card-display.component';
import { Card } from '../../../models/Card';
import { Deck } from '../../../models/Deck';

import { DeckService, CardItem } from '../../../core/services/deck.service';
import { CardService } from '../../../core/services/card.service';
import { Observable } from 'rxjs';
import { DoughnutChartComponent } from "../../../shared/components/doughnut-chart/doughnut-chart.component";
import { ChartData } from 'chart.js';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FileUploadModalComponent } from "../../../shared/components/file-upload-modal/file-upload-modal.component";
import { DeckBuilderService } from '../../../core/services/deck-builder.service';

@Component({
  selector: 'app-card-list',
  imports: [
    CommonModule,
    RouterModule,
    CardSearchComponent,
    DoughnutChartComponent,
    FileUploadModalComponent,
    EditableLabelComponent
  ],
  templateUrl: './card-list.component.html',
  styleUrl: './card-list.component.css'
})
export class CardListComponent {
  private _deckService! : DeckService;
  private _cardService! : CardService;
  private _deckBuilderService! : DeckBuilderService;

  deckCards: CardItem[]; // CardId, Quantity
  cardCache: { [key: number]: Card } = {};

  errors: string[] = [];

  deckName = 'Your deck';
  deckDescription = 'Greatest Deck Ever';
  title = 'Deck Breakdown';
  subtitle = 'by Card Type';
  myDoughnutChartData: ChartData<'doughnut'> = {
    labels: [
      'Battle', 
      'Space', 
      'Mission', 
      'Ground', 
      'Character', 
      'Equipment', 
      'Location'
    ],
    datasets: [{
      label: '# of Cards',
      data: [0,0,0,0,0,0,0],
      backgroundColor: [
        'rgba(255, 99, 132, 0.8)', // Red
        'rgba(54, 162, 235, 0.8)', // Blue
        'rgba(255, 206, 86, 0.8)', // Yellow
        'rgb(51, 240, 124)', // Green
        'rgba(153, 102, 255, 0.8)',  // Purple
        'rgb(168,176,178)', //Steel Grey
        'rgb(173,125,55)' // Brown..-ish?
      ],
      borderColor: [
        'rgba(255, 99, 132, 1)',
        'rgba(54, 162, 235, 1)',
        'rgba(255, 206, 86, 1)',
        'rgb(51, 240, 124)', // Green
        'rgba(153, 102, 255, 1)',
        'rgb(168,176,178)',
        'rgb(173,125,55)'
      ],
      borderWidth: 1
    }]
  };

  constructor (
    private modalService: NgbModal,
    public readonly authService: AuthService,
    private readonly deckService: DeckService,
    private readonly deckBuilderService: DeckBuilderService,
    private readonly cardService: CardService
  ) {
    this._deckService = deckService;
    this._cardService = cardService;
    this._deckBuilderService = deckBuilderService;
    this.deckCards = this._deckService.cart().items;
  }

  onSelectedCard(selectedCard: Card) {
    console.log('A card was selected: ', selectedCard);
  }

  deckCardQuantity(): number {
    return this.deckCards.reduce((total, c) => total + c.quantity, 0);
  }

  cardAction(card: Card, action: 'Add' | 'Remove') {
    let cardItem : any = {
      cardId: card.id,
      quantity: 1
    };
    
    if (action == 'Add') {
      this.deckService.addItem(cardItem);
      //TODO: cardItem.deckId
      this.deckBuilderService.addItem(cardItem).subscribe(response => { });
      this.cardCache[card.id] = card;
    }
    if (action == 'Remove') {
      //TODO: cardItem.deckId
      //TODO: this.deckBuilderService.removeItem(cardItem).subscribe(response => { })
      this.deckService.removeItem(cardItem);
    }
    this.updateDeckCart();
  }

  removeCardFromDeck(cardItem: CardItem) {
    //TODO: Add a confirmation dialog
    //TODO: cardItem.deckId
    //TODO: this.deckBuilderService.removeItem(cardItem).subscribe(response => { })
    this.deckService.removeItem(cardItem);
    this.updateDeckCart();
  }

  updateDeckCart(){
    this.deckCards = this._deckService.cart().items; 
    this.validateDeck();  
  }

  emptyDeck() {
    let isConfirmed: boolean = true;
    //TODO: Add a confirmation dialog
    //this.message.confirm('Empty cart', 'Are you sure?', (isConfirmed) => {
      if (isConfirmed) {
        this.deckCards = [];
        this.deckService.emptyCart();
        //TODO: cardItem.deckId
        //TODO: this.deckBuilderService.removeDeck(deckId).subscribe(response => { })
        this.cardCache = {};
        this.errors = [];
        this.validateDeck();
      }
    //});
  }

  loadDeck() : void {
    const modalRef = this.modalService.open(FileUploadModalComponent, {
      backdrop: 'static', // Optional: prevents closing by clicking outside
      keyboard: false,    // Optional: prevents closing by pressing ESC
      centered: true      // Optional: centers the modal vertically
    });

    // Subscribe to the closeDialog event emitter from the modal component
    modalRef.componentInstance.closeDialog.subscribe((result: boolean) => {
      console.log('Modal closed, result:', result);
      if (result) {
        console.log('File processing completed successfully!');
      } else {
        console.log('File processing cancelled or failed.');
      }
      modalRef.close(); // Close the NgbModal instance
    });

    modalRef.componentInstance.loadedCard.subscribe((c: Card) => {
      this.cardAction(c, "Add");
    });

    modalRef.closed.subscribe(() => {
      //TODO: Add a spinner to the loading sequence so the user knows when it's completed
      console.log('NgbModal completely closed.');
    });

    modalRef.dismissed.subscribe((reason) => {
        console.log('NgbModal dismissed:', reason);
    });
  }

  submitDeck() {
    //TODO: Implement deck submission
    let newDeck: Deck = {
      id: null,
      name: this.deckName,
      description: this.deckDescription,
      createdBy: this.authService.getAuthenticatedUser()?.id,
      createdAt: new Date(),
      lastUpdated: new Date(),
      isValid: false,
      isPublic: true, //TODO: Deck.IsPublic
    };
console.log('submitDeck() => ', newDeck);
    this.deckService.submitDeck(newDeck).subscribe({
      next: (createdDeck) => {
        console.log('Deck created successully ==> ', createdDeck);
        alert(`Deck "${createdDeck.name}" created successfully`);
      },
      error: (error) => {
        alert(`Failed to create Deck: ${error.message}`);
        console.error(error);
      }
    })
    
  }

  trackByCardId(index: number, card: any): string {
    return card.id;
  }

  alignment(cardType: string | undefined) : string {
    if (
      cardType == 'Ground' ||
      cardType == 'Character' ||
      cardType == 'Space'
    )
    { return 'vertical'; }
    else { return 'horizontal'; }
  }
  forceIcon(side: string | undefined): string {
    let forceIconPath = '/assets/icons/LightSaber_' + side + '.png';
    return forceIconPath;
  }

  public updateChartDataByLabel(label: string, newValue: number): void {
    const labels = this.myDoughnutChartData.labels;
    const currentDataset = this.myDoughnutChartData.datasets[0];

    if (!labels || !currentDataset) {
      console.error('Chart labels or dataset not found.');
      return;
    }

    const index = labels.indexOf(label);

    if (index !== -1) {
      // Create a new data array to ensure immutability and trigger ngOnChanges
      const newData = [...currentDataset.data];
      newData[index] = newValue;

      // Create a new dataset object with the updated data
      const updatedDataset = {
        ...currentDataset,
        data: newData
      };

      // Create a new ChartData object with the updated dataset
      this.myDoughnutChartData = {
        ...this.myDoughnutChartData,
        datasets: [updatedDataset]
      };
    } else {
      console.warn(`Label '${label}' not found in chart data.`);
    }
  }

  validateDeck(): boolean {
    this.errors = [];

    // Rule 1: Deck must have no less than 40 cards
    if (this.deckCardQuantity() < 60 && this.deckCardQuantity() > 0){
      this.errors.push('The deck must have at least 60 cards.');
    }

    // Rule 2: Deck can have up to four copies of any single card
    const cardCount: { [key: number]: number } = {};
    for (const dc of this.deckCards) {
      cardCount[dc.cardId] = (cardCount[dc.cardId] || 0) + dc.quantity;
      if (cardCount[dc.cardId] > 4) {
        this.errors.push(`You can have a maximum of 4 copies of any single card and version [${dc.cardId}].`);
      }
    }

    //#region Card type counts
    let lightCount: number = 0;
    let darkCount: number = 0;
    let battleCount: number = 0;
    let equipmentCount: number = 0;
    let locationCount: number = 0;
    let missionCount: number = 0;
    let groundCount: number = 0;
    let spaceCount: number = 0;
    let characterCount: number = 0;

    for (const cardId in this.cardCache) {
      const card = this.cardCache[cardId];
      const existingCard = this.deckCards.find(c => c.cardId == parseInt(cardId));
      const quantity = existingCard?.quantity || 0;
      if (card) {
        switch (card.type.toLowerCase()) {
          case 'ground':
            groundCount += quantity;
            break;
          case 'character':
            characterCount += quantity;
            break;
          case 'space':
            spaceCount += quantity;
            break;
          case 'mission':
            missionCount += quantity;
            break;
          case 'battle':
            battleCount += quantity;
            break;
          case 'equipment':
            equipmentCount += quantity;
            break;
          case 'location':
            locationCount += quantity;
            break;
        }
        if (card.side == 'L') { lightCount++; }
        if (card.side == 'D') { darkCount++; }
      }
    }

    this.updateChartDataByLabel('Space', spaceCount);
    this.updateChartDataByLabel('Ground', groundCount);
    this.updateChartDataByLabel('Character', characterCount);
    this.updateChartDataByLabel('Mission', missionCount);
    this.updateChartDataByLabel('Equipment', equipmentCount);
    this.updateChartDataByLabel('Location', locationCount);
    this.updateChartDataByLabel('Battle', battleCount);
    //#endregion

    // Rule 3: Cannot have cards of opposing force types
    if (lightCount > 0 && darkCount > 0) {
      this.errors.push(`A deck cannot have opposing Force types`);
    }

    // Rule 4: Each arena must have at least 12 cards
    if (groundCount < 12) { this.errors.push(`A deck must contain at least 12 ground units`); }
    if (characterCount < 12) { this.errors.push(`A deck must contain at least 12 character units`); }
    if (spaceCount < 12) { this.errors.push(`A deck must contain at least 12 space units`); }

    // Rule 5: No arena can have more than double the smallest arena population
    if (Math.min(groundCount, characterCount, spaceCount) * 2 < Math.max(groundCount, characterCount, spaceCount)){
      this.errors.push(`No arena can contain more than twice the cards of the smallest arena`);
    }

    return (this.errors.length == 0);
  }
}
