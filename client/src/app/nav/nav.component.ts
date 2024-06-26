import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent implements OnInit
{
  model: any = {}
  loggedIn = false;
  constructor(private accountService: AccountService) { }
  ngOnInit(): void { }
  login()
  {
    this.accountService.login(this.model).subscribe
      ({
        next: response => { console.log(response); this.loggedIn = true; },
        error: error => console.log(error)
      })
  }
  logout() {
    this.loggedIn = false;
  }
}
