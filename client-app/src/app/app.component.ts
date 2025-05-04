import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'client-app';

  constructor(private http: HttpClient, private router: Router) {}

  login() {
    // this.http.get('/account/login', { withCredentials: true }).subscribe();
    window.location.href = '/account/login';
    // this.http.get('/account/login',{ withCredentials: true }).subscribe(response => {
    //   debugger
    //   // Assuming the response contains the redirect URL to Keycloak
    //   // window.location.href = response.redire;
    // });
  }
}
