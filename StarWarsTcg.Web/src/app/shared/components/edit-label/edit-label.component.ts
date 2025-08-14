import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
@Component({
  imports: [ FormsModule, CommonModule ],
  selector: 'app-editable-label',
  templateUrl: './edit-label.component.html',
  styleUrls: ['./edit-label.component.css']
})
export class EditableLabelComponent implements OnInit {
  // Renamed from 'initialValue' to 'value' to support [(value)] binding
  @Input() value: string = 'Click to edit';
  // Output event MUST be named 'value' + 'Change' for [(value)] to work
  @Output() valueChange = new EventEmitter<string>();

  // Internal property to hold the value being edited.
  // We use this to avoid directly mutating the @Input property.
  // This is a common pattern for "controlled components" in Angular.
  _internalLabelValue: string;

  isEditing: boolean = false;

  constructor() {
    // Initialize internal value from the input value
    this._internalLabelValue = this.value;
  }

  ngOnInit(): void {
    // Ensure internal value is updated if input changes after construction
    // (though for initial setup, constructor is often sufficient)
    this._internalLabelValue = this.value;
  }

  // Use ngOnChanges to react to changes in the @Input() 'value' from the parent
  // This is crucial if the parent can update 'value' programmatically
  ngOnChanges(changes: any): void {
    if (changes.value && changes.value.currentValue !== changes.value.previousValue) {
      this._internalLabelValue = changes.value.currentValue;
    }
  }


  /**
   * Toggles the editing mode between true and false.
   * When in edit mode, the input field is shown.
   * When in view mode, the label is shown.
   * Emits the current _internalLabelValue when transitioning from edit to view mode.
   */
  toggleEditMode(): void {
    this.isEditing = !this.isEditing;

    // If we just exited edit mode (i.e., clicked "Save")
    if (!this.isEditing) {
      // Emit the updated internal value back to the parent
      this.valueChange.emit(this._internalLabelValue);
      // It's also good practice to update the @Input 'value' here directly,
      // but emitting is what triggers the parent's binding.
      // If we don't do this, and the parent doesn't update its bound variable,
      // the next time the parent sends a new 'value' input, it might overwrite
      // the internally saved value before the parent gets a chance to update.
      this.value = this._internalLabelValue; // Update the Input property to reflect internal state
    }
  }
}