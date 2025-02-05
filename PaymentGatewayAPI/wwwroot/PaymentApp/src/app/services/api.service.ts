import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { from, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  Login(loginRequest: any) {
    return this.http.post(`${this.apiUrl}/Authentication/Login`, loginRequest);
  }
  processPayment(paymentData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Payment/ProcessPayment`, paymentData);
  }

  getPayments(fromDate: string, toDate: string, status: string, userName: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/Payment/Payments?fromDate=${fromDate}&toDate=${toDate}&status=${status}&userName=${userName}`);
  }
}
