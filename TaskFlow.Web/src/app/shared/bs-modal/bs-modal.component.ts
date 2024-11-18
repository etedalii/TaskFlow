import { Component, input, Input } from '@angular/core';
import * as bootstrap from 'bootstrap';

@Component({
  selector: 'app-bs-modal',
  standalone: true,
  imports: [],
  templateUrl: './bs-modal.component.html',
  styleUrl: './bs-modal.component.css'
})
export class BsModalComponent{

  @Input() title: string = 'Modal Title';   // Title for the modal
  @Input() buttonLabel: string = 'Open';    // Label for the button to open the modal
  @Input() modalId: string = 'dynamicModal' // Unique ID for each modal instance
  sizeClass = input<'modal-lg' | 'modal-sm' | 'modal-xl'>('modal-sm');


  onCloseModal() {
    const modalElement = document.getElementById(this.modalId);
    const backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach((backdrop) => {
      backdrop.remove();
    });
    if(modalElement){
      modalElement.classList.remove('fade')      
      const modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
      modalInstance!.hide()
    }
  }
}
