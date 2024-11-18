import {
  Component,
  DestroyRef,
  inject,
  Input,
  input,
  OnChanges,
  signal,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { BsModalComponent } from '../../shared/bs-modal/bs-modal.component';
import { ProjectService } from '../project.service';
import { Project } from '../project.model';

const formatDate = (date: string | Date | null) => {
  if (!date) return '';
  const d = new Date(date);
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
};

@Component({
  selector: 'app-project-new',
  standalone: true,
  imports: [ReactiveFormsModule, BsModalComponent],
  templateUrl: './project-new.component.html',
  styleUrl: './project-new.component.css',
})
export class ProjectNewComponent implements OnChanges {
  @ViewChild(BsModalComponent) bsModal!: BsModalComponent;
  form = new FormGroup({
    id: new FormControl('0'),
    name: new FormControl('', { validators: [Validators.required] }),
    description: new FormControl('', { validators: [Validators.required] }),
    startDate: new FormControl('', { validators: [Validators.required] }),
    endDate: new FormControl('', { validators: [Validators.required] }),
    user: new FormControl('', { validators: [Validators.required] }),
  });

  private projectService = inject(ProjectService);
  private destroyRef = inject(DestroyRef);
  errorSignal = signal('');
  @Input() projectData: any;

  ngOnChanges(changes: SimpleChanges) {
    if (changes['projectData'] && this.projectData) {
      this.form.patchValue({
        id: this.projectData.id || '0',
        name: this.projectData.name || '',
        description: this.projectData.description || '',
        startDate: formatDate(this.projectData.startDate),
        endDate: formatDate(this.projectData.endDate),
        user: this.projectData.user || '',
      });
    }
  }

  onSaveProject() {
    if (this.form.invalid) {
      return;
    }

    let projec: Project = {
      id: +this.form.value.id!,
      name: this.form.value.name!,
      description: this.form.value.description!,
      startDate: new Date(this.form.value.startDate!),
      endDate: new Date(this.form.value.endDate!),
      user: '6de85501-c457-452a-9827-7a7d6e24231d', //after Auth I need to fix TODO
      ownerId: '6de85501-c457-452a-9827-7a7d6e24231d',
    };

    let subscription: any;
    if (projec.id === 0) {
      subscription = this.projectService.addProject(projec).subscribe({
        error: (error: Error) => {
          console.log(error.message);
          this.errorSignal.set(error.message);
        },
        complete: () => {
          console.log('completed');
          this.form.reset();
          // this.router.navigate(['projects'], {
          //   onSameUrlNavigation: 'reload',
          // })
          this.closeModal();
        },
      });
    } else {
      subscription = this.projectService.updateProject(projec.id,projec).subscribe({
        error: (error: Error) => {
          console.log(error.message);
          this.errorSignal.set(error.message);
        },
        complete: () => {
          this.form.reset();
          this.closeModal();
        },
      })
    }

    this.destroyRef.onDestroy(() => {
      subscription.unsubscribe();
    });
  }

  closeModal() {
    this.form.reset();
    this.bsModal.onCloseModal();
  }
}
