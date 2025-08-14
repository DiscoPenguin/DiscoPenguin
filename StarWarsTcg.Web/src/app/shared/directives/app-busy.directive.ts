import {
  Directive,
  Input,
  ElementRef,
  Renderer2,
  OnChanges,
  SimpleChanges,
  HostBinding,
  OnDestroy
} from '@angular/core';

@Directive({
  selector: '[isBusy]',
  standalone: true
})
export class BusyIfDirective implements OnChanges, OnDestroy {
  private static index = 0;
  //TODO: [busyIf] Directive
  @Input() busyIf: boolean = false;
  @Input() spinnerSize: string = '2x';
  @Input() spinnerColor: string = '#333';

  isBusy:boolean = false;
  private spinnerName = '';
  private overlayElement: HTMLElement | null = null;
  private spinnerIconElement: HTMLElement | null = null;
  
  @HostBinding('class.app-busy-container') get isBusyContainer(): boolean {
    return this.busyIf;
  }

  constructor(
    private el: ElementRef,
    private renderer: Renderer2
  ) { 
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['busyIf']) {
      if (this.busyIf) {
        this.showBusyIndicator();
      } else {
        this.hideBusyIndicator();
      }
    }
  } 
    private showBusyIndicator(): void {
    if (this.overlayElement) {
      // If already showing, do nothing
      return;
    }

    // 1. Create the overlay element
    this.overlayElement = this.renderer.createElement('div');
    this.renderer.addClass(this.overlayElement, 'app-busy-overlay');

    // Apply spinner color dynamically
    this.renderer.setStyle(this.overlayElement, 'color', this.spinnerColor);

    // 2. Create the Font Awesome spinner icon element
    this.spinnerIconElement = this.renderer.createElement('i');
    this.renderer.addClass(this.spinnerIconElement, 'fas');
    this.renderer.addClass(this.spinnerIconElement, 'fa-spinner');
    this.renderer.addClass(this.spinnerIconElement, 'fa-spin');
    this.renderer.addClass(this.spinnerIconElement, `fa-${this.spinnerSize}`); // Apply dynamic size

    this.renderer.appendChild(this.overlayElement, this.spinnerIconElement);
    this.renderer.appendChild(this.el.nativeElement, this.overlayElement);
  } 
  private hideBusyIndicator(): void {
    if (this.overlayElement) {
      this.renderer.removeChild(this.el.nativeElement, this.overlayElement);
      this.overlayElement = null; // Clear reference
      this.spinnerIconElement = null; // Clear icon reference
    }
  }
  ngOnDestroy(): void {
    this.hideBusyIndicator();
  }
}
