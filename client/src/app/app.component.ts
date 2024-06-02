import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  constructor(private http: HttpClient) { }
  title = 'Dating App';
  users: any;

  ngOnInit(): void  {
    this.http.get("http://localhost/api/users").subscribe({
      next: response => this.users = response,
      error: () => { console.log(Error)},
      complete: () => { console.log("Request has completed") } });
    }
}
