import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  Transactions: Transaction[] = [];
  FilteredTransactions: Transaction[] = [];

  Status: string = '';
  UserName: string = '';
  FromDate: string = '';
  ToDate: string = '';
  IsModalOpen: boolean = false;
  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
  }

  GetPayments() {
    if (!this.FromDate || !this.ToDate) {
      alert('Please select valid date range');
      return;
    }
    this.apiService.getPayments(this.FromDate, this.ToDate, this.Status, this.UserName).subscribe((res: any) => {
      if (res) {
        this.Transactions = res;
      }
    })
  }
  OpenAddTransactionModal() {
    this.IsModalOpen = true;
  }
  CloseAddTransactionModal() {
    this.IsModalOpen = false;
  }

  ResponseCallBack() {
    this.IsModalOpen = false;
  }

}

interface Transaction {
  TransactionId: string;
  Status: string;
  Currency: string;
  Amount: number;
  TransactionDate: string;
  PaymentMode: string;
  CustomerName: string;
  Email: string;
  UserName: string;
}
