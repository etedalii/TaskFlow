import { Routes } from '@angular/router';
import { ProjectsComponent } from './projects/projects.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TaskComponent } from './task/task.component';
import { ProjectNewComponent } from './projects/project-new/project-new.component';


export const routes: Routes = [
  {
    path: '',
    component: DashboardComponent,
  },
  {
    path: 'projects',
    component: ProjectsComponent,
  },
  {
    path: 'projects/new',
    component: ProjectNewComponent,
  },
  {
    path: 'tasks',
    component: TaskComponent,
  },
];
