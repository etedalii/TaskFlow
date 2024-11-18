import {
  Component,
  DestroyRef,
  ElementRef,
  inject,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { ProjectService } from './project.service';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProjectNewComponent } from './project-new/project-new.component';
import { BsModalComponent } from '../shared/bs-modal/bs-modal.component';
import { ModalComponent } from '../shared/modal/modal.component';
import { Project } from './project.model';
import { firstValueFrom } from 'rxjs';
import { Modal } from 'bootstrap';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-project',
  standalone: true,
  imports: [
    DatePipe,
    RouterLink,
    ProjectNewComponent,
    BsModalComponent,
    ModalComponent,
  ],
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.css',
})
export class ProjectsComponent implements OnInit {
  @ViewChild(BsModalComponent) bsModal!: ElementRef<BsModalComponent>;
  private projectService = inject(ProjectService);
  private destroyRef = inject(DestroyRef);
  projects = this.projectService.loadedProjects;
  errorSignal = signal('');
  isFetching = signal(false);

  form?: FormGroup;
  private modalInstance: Modal | null = null;
  selectedProject: any = null;

  ngOnInit(): void {
    this.isFetching.set(true);
    const subscription = this.projectService.getProjects().subscribe({
      error: (error: Error) => {
        this.errorSignal.set(error.message);
      },
      complete: () => {
        this.isFetching.set(false);
      },
    });

    this.destroyRef.onDestroy(() => {
      subscription.unsubscribe();
    });
  }

  async onEditProject(id: number) {
    const data = await firstValueFrom(this.projectService.getProjectById(id));
    let fetchedProject: Project = data as Project;
    this.selectedProject = fetchedProject;
    // Open modal programmatically
    const modalElement = document.getElementById('projectNewModal');
    if (modalElement) {
      this.modalInstance = new Modal(modalElement);
      this.modalInstance.show();
    }
  }

  onDelteProject(id: number) {
    if (confirm('Are you sure to delete?')) {
      const subscribe = this.projectService.removeProject(id).subscribe({
        error: (error: Error) => {
          this.errorSignal.set(error.message);
        },
        complete: () => {
          console.log('deleted');
        },
      });

      this.destroyRef.onDestroy(() => {
        subscribe.unsubscribe();
      });
    }
  }
}
