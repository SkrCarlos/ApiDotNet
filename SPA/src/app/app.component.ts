import { Component , OnInit} from '@angular/core';
import { AccountService } from './_services/account.service';
import { IUser } from './_models/iusers';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Citas App';

  constructor(private AccountService: AccountService) {}

  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser(): void {
    const userString = localStorage.getItem('user');
    if(!userString) return;
    const user: IUser = JSON.parse(userString);
    this.AccountService.setCurrentUser(user);
  }
}
