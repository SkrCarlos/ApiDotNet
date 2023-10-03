import { Component , OnInit} from '@angular/core';
import { HttpClient } from '@angular/common/http'; // Import HttpClientModule
import { AccountService } from './_services/account.service';
import { IUser } from './_models/iusers';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Citas App';
  users:any;

  constructor(private http: HttpClient, private AccountService: AccountService) {}

  ngOnInit(): void {
    this.getUsers();
    this.setCurrentUser();
  }

  getUsers(): void {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request completed')
    });
  }

  setCurrentUser(): void {
    const userString = localStorage.getItem('user');
    if(!userString) return;
    const user: IUser = JSON.parse(userString);
    this.AccountService.setCurrentUser(user);
  }
}
