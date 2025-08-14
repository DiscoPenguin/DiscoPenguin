import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router'
import { CommonModule } from '@angular/common';

import { CardDisplayComponent } from "../../../shared/components/card-display/card-display.component";
import { Card } from '../../../models/Card';

@Component({
  selector: 'app-deck-builder',
  imports: [CommonModule, RouterModule, CardDisplayComponent],
  templateUrl: './deck-builder.component.html',
  styleUrl: './deck-builder.component.css'
})
export class DeckBuilderComponent {

  constructor() {

  }
}
