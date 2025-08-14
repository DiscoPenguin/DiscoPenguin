import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AssetService, Asset } from '../../../../core/services/asset.service';
@Component({
  selector: 'app-icon-selector',
  imports: [ CommonModule, FormsModule],
  templateUrl: './icon-selector.component.html',
  styleUrl: './icon-selector.component.css'
})
export class IconSelectorComponent implements OnInit {
  @Input() chosenAsset: number | undefined;
  @Output() iconSelected = new EventEmitter<Asset>();

  icons: Asset[] = [];
  constructor(private assetService: AssetService) { }

  ngOnInit() {
    this.loadAssets();
  }
  loadAssets(): void {
    this.assetService.getAssets().subscribe(
      (assets) => {
        this.icons = assets.filter((a:Asset) => a.imageType == 'characters')
      },
      (error) => {
        console.error('Error loading assets:', error);
      }
    );
  }
  selectIcon(icon: Asset): void {
    this.iconSelected.emit(icon);
    this.chosenAsset = icon.id;
  }

}