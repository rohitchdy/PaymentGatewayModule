import { Component, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  AppName: string = environment.appName;
  IsLoggedIn: boolean = false;
  constructor(private authService: AuthService, private router: Router) {
    this.authService.isLoggedIn$.subscribe(status => {
      this.IsLoggedIn = status;
    });
  }

  ngOnInit(): void {
  }
  login() {
    this.router.navigate(['/login']);
  }
  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

}
