import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-protected',
  imports: [],
  templateUrl: './protected.component.html',
  styleUrl: './protected.component.css'
})
export class ProtectedComponent {
  weatherData: any;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get('/weatherforecast', { withCredentials: true }).subscribe(data => {
      this.weatherData = data;
    });
  }
}
