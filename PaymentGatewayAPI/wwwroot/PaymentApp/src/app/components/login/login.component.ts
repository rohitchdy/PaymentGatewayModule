import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm!: FormGroup;
  constructor(private fb: FormBuilder, private router: Router, private apiService: ApiService, public authService: AuthService) {
    this.loginForm = this.fb.group({
      UserName: ['', [Validators.required]],
      Password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
  }

  login() {
    if (this.loginForm.valid) {
      this.apiService.Login(this.loginForm.value).subscribe(
        (response: any) => {
          if (response && response.token) {
            this.authService.login(response.token);
            this.authService.CheckIfUserAuthenticated()
            this.router.navigate(['/checkout']);
          } else {
            alert('Invalid login response');
          }
        },
        (error) => {
          alert('Login failed! Please check your credentials.');
        }
      );
    }
  }


}
