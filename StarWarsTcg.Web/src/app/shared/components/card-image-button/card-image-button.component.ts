import { Component, Input } from '@angular/core';
import { CommonModule

 } from '@angular/common';
@Component({
  selector: 'app-card-image-button',
  imports: [ CommonModule ],
  templateUrl: './card-image-button.component.html',
  styleUrl: './card-image-button.component.css'
})
export class CardImageButtonComponent {
  @Input() redirectUrl: string = '';
  @Input() imageUrl: string | undefined = '';
  @Input() alignment: 'horizontal' | 'vertical' = 'horizontal';
  
  isHovered: boolean = false;

  onButtonClick() {
    if (this.redirectUrl) {
      window.location.href = this.redirectUrl;
    }
  }

  onMouseEnter() {
    this.isHovered = true;
  }

  onMouseLeave() {
    this.isHovered = false;
  }
}