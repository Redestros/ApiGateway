import { Routes } from '@angular/router';
import { ProtectedComponent } from './protected/protected.component';
import { AppComponent } from './app.component';

export const routes: Routes = [
    { path: 'private', component: ProtectedComponent }
];
