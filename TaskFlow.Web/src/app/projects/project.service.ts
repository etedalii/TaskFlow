import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Project } from './project.model';
import { ErrorService } from '../shared/error.service';
import {
  catchError,
  exhaustMap,
  map,
  switchMap,
  take,
  tap,
  throwError,
} from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from '../auth/auth.service';
const headers = new HttpHeaders({
  'Content-Type': 'application/json',
});

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  private httpClient = inject(HttpClient);
  private projects = signal<Project[]>([]);
  private errorService = inject(ErrorService);
  // API URL from the environment configuration
  private readonly API_URL = `${environment.apiUrl}/Project`;
  private authService = inject(AuthService);

  loadedProjects = this.projects.asReadonly();

  getProjects() {
     return this.fetchProjects('error').pipe(
        tap({
          next: (projects) => {
            this.projects.set(projects);
          },
        })
      );
  }

  getProjectById(id: number) {
    return this.httpClient.get(this.API_URL + '/' + id).pipe(
      catchError((error) => {
        this.errorService.showError('Failed to fetch selected entity');
        return throwError(() => new Error('Failed to fetch selected entity'));
      })
    );
  }

  addProject(project: Project) {
    return this.httpClient
      .post(this.API_URL, project, { headers })
      .pipe(
        catchError((error) => {
          this.errorService.showError('Failed to save data');
          return throwError(() => new Error('Failed to save data'));
        })
      )
      .pipe(
        tap({
          next: (data) => {
            let temp = data as Project;
            const prevProjects = this.projects();
            this.projects.update((prevProject) => [...prevProject, temp]);
          },
        })
      );
  }

  updateProject(id: number, project: Project) {
    const prevProjects = this.projects();
    if (prevProjects.some((p) => p.id == id)) {
      this.projects.set(prevProjects.filter((p) => p.id !== id));
      this.projects.update((prevProject) => [...prevProject, project]);
    }

    return this.httpClient.put(`${this.API_URL}/${id}`, project).pipe(
      catchError((error) => {
        this.projects.set(prevProjects);
        this.errorService.showError('Failed to store selected');
        return throwError(() => new Error('Failed to store selected'));
      })
    );
  }

  removeProject(id: number) {
    const prevProjects = this.projects();
    if (prevProjects.some((p) => p.id === id)) {
      this.projects.set(prevProjects.filter((p) => p.id !== id));
    }
    return this.httpClient.delete(this.API_URL + '/' + id).pipe(
      catchError((error) => {
        this.projects.set(prevProjects);
        this.errorService.showError('Failed to remove selected entity');
        return throwError(() => new Error('Failed to remove selected entity'));
      })
    );
  }

  private fetchProjects(errorMessage: string) {
    /*
, {
        headers: new HttpHeaders().set('Authorization', `Bearer ${token}`),
      }
    */
    return this.httpClient
      .get<{ data: Project[] }>(this.API_URL,{headers})
      .pipe(
        map((response) => response.data),
        catchError((err) => {
          return throwError(() => new Error(errorMessage));
        })
      );
  }
}
