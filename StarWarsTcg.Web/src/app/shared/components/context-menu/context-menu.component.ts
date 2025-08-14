import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';

@Component({
  imports: [ CommonModule ],
  selector: 'app-context-menu',
  templateUrl: './context-menu.component.html',
  styleUrls: ['./context-menu.component.css']
})
export class ContextMenuComponent {
  public menuVisible: boolean = false;
  public menuPosition = { x: '0px', y: '0px' };

  @HostListener('document:contextmenu', ['$event'])
  onRightClick(event: MouseEvent) {
    event.preventDefault();
    this.menuVisible = true;
    this.menuPosition.x = `${event.clientX}px`;
    this.menuPosition.y = `${event.clientY}px`;
  }

  @HostListener('document:click')
  onClick() {
    this.menuVisible = false;
  }

  onMenuItemClick(item: string) {
    console.log(`Clicked on ${item}`);
    this.menuVisible = false; // Hide the menu after selection
  }
}

/* Implementation pattern:
  <div class="container">
    <h1>Right Click to Open Context Menu</h1>
    <app-context-menu></app-context-menu>
  </div>
*/
