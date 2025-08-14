import { Component, OnInit, Output, EventEmitter, ɵɵsetComponentScope } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import * as converter from 'xml-js';

import { CardService } from '../../../core/services/card.service';
import { Card } from '../../../models/Card';
import { DeckBuilderService } from '../../../core/services/deck-builder.service';
import { DeckItem } from '../../../core/services/deck-builder.service';

@Component({
  selector: 'app-file-upload-modal',
  templateUrl: './file-upload-modal.component.html',
  styleUrls: ['./file-upload-modal.component.css'],
  imports: [CommonModule ]
})
export class FileUploadModalComponent {
  @Output() closeDialog = new EventEmitter<boolean>();
  @Output() loadedCard = new EventEmitter<Card>();

  fileEvent: Event | null = null;
  selectedFile: File | null = null;
  uploadProgress: number = 0;
  digestionProgress: number = 0;
  currentMessage: string = 'Please select a file to upload.';
  currentCardName: string = '';
  isUploading: boolean = false;
  isDigesting: boolean = false;

  missingCards: string[] = [];
  deckOfCards: Card[] = [];
  deckOfItems: DeckItem[] = [];

  constructor (
    private readonly cardService: CardService,
    private readonly deckBuilderService : DeckBuilderService
  ){

  }

  ngOnInit(): void {}

  onFileSelected(event: any): void {
    this.fileEvent = event;
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.currentMessage = `File selected: ${this.selectedFile.name}. Click 'Upload' to proceed.`;
      this.uploadProgress = 0;
      this.digestionProgress = 0;
    } else {
      this.selectedFile = null;
      this.currentMessage = 'Please select a file to upload.';
    }
  }

  async uploadFile() {
    if (!this.selectedFile) {
      this.currentMessage = 'No file selected. Please choose a file first.';
      return;
    }

    const deckBuilderId = await this.deckBuilderService.getNextDeckId();
    if (typeof deckBuilderId !== 'number') {
      this.currentMessage = 'Failed to get a valid deckBuilderId';
      return;
    }

    this.isDigesting = false;
    this.currentMessage = 'Digesting file...';
    this.digestionProgress = 0;

    this.isDigesting = true;

//    https://www.bezkoder.com/angular-17-file-upload-progress-bar/
//      https://www.npmjs.com/package/xml-js (dependency)
// ==> /home/ralph/Dropbox/SWTCG/angular/src/app/components/file-upload/*.*  
    const reader = new FileReader();
    reader.onload = async (e: any) => {
      let inputXml = e.target.result;
      let result1 = converter.xml2json(inputXml, { compact: true, spaces: 2 });
      const JSONData = JSON.parse(result1);
      let xml = JSONData;
      let list: any = [];
      list = Array.isArray(JSONData.deck.superzone.card) ? JSONData.deck.superzone.card : [];
      if (!list) { this.currentMessage = 'Invalid file'; return; }

      const totalLines = list.length;
      let lineNumber : number = 0;
      for (let item of list){
        item.class_name = 'info';
        if (lineNumber%2==0) {
          item.class_name = 'success';
        }

        const foundCard : Card | undefined = this.deckOfCards.find((c) => c.name == item.name._text);
        if (foundCard !== undefined) {
          this.loadedCard.emit(foundCard);
          continue;
        }

        let searchCriteria: any = {
          searchTerm: item.name._text ? item.name._text : '',
          useLikeness: false,
          sortField: 'Name',
          sortDirection: 'asc',
          pageNumber: 1,
          pageSize: 1
        };

        try {
          const result = await this.cardService.searchCards(searchCriteria).toPromise();
          this.currentCardName = "(" + result?.items[0].expansionSet + ") " + result?.items[0].name;
          var di:DeckItem = {
            cardId: result?.items[0].id,
            deckId: deckBuilderId,
            quantity: 1
          };
          this.deckOfItems.push(di);
          this.deckOfCards.push(result?.items[0]);
          this.loadedCard.emit(result?.items[0]);
        } catch (e) {
          const httpError = e as HttpErrorResponse;
          if (httpError.status === 404) {
            this.missingCards.push("(" + item.set._text + ") " + item.name._text);
          }
        }
        this.digestionProgress = Math.floor((lineNumber / (totalLines)) * 100);
        this.currentMessage = `Digesting file contents: Processing line ${lineNumber} of ${totalLines}...`;
        lineNumber++;
      }
      this.currentCardName = '';

      if (this.missingCards.length <= 0) {
        this.deckOfItems.forEach((di: DeckItem) => {
          const result = this.deckBuilderService.addItem(di).subscribe(response => {
          });
        });
      }
      
      this.isDigesting = false;
      this.currentMessage = 'File digestion complete!';
      if (this.missingCards.length <= 0) { this.closeModal(true); }
      else { }
    };
    reader.readAsText(this.selectedFile);
  }

  closeModal(success: boolean = false): void {
    this.closeDialog.emit(success);
  }

}
