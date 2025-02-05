import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-payment-checkout',
  templateUrl: './payment-checkout.component.html',
  styleUrls: ['./payment-checkout.component.css']
})
export class PaymentCheckoutComponent implements OnInit {
  checkoutForm!: FormGroup;
  PaymentModes = ['Card', 'Bank'];
  SelectedPaymentMode: string = '';

  constructor(private fb: FormBuilder, private apiService: ApiService) {
    this.checkoutForm = this.fb.group({
      FullName: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]],
      Currency: ['', Validators.required],
      Amount: [0, Validators.required],
      PaymentMode: ['', Validators.required],
      CardNumber: [''],
      ExpiryDate: [''],
      CVV: [''],
      AccountNumber: [''],
      BankName: ['']
    });

    this.checkoutForm.get('PaymentMode')?.valueChanges.subscribe((value) => {
      this.SelectedPaymentMode = value;
      this.UpdateFormValidators();
    });
  }

  UpdateFormValidators(): void {
    const cardNumberControl = this.checkoutForm.get('CardNumber');
    const expiryDateControl = this.checkoutForm.get('ExpiryDate');
    const cvvControl = this.checkoutForm.get('CVV');
    const accountNumberControl = this.checkoutForm.get('AccountNumber');
    const bankNameControl = this.checkoutForm.get('BankName');

    if (this.SelectedPaymentMode === 'Card') {
      cardNumberControl?.setValidators([Validators.required, Validators.pattern(/^\d{16}$/)]);
      expiryDateControl?.setValidators([Validators.required, Validators.pattern(/^(0[1-9]|1[0-2])\/\d{2}$/)]);
      cvvControl?.setValidators([Validators.required, Validators.pattern(/^\d{3}$/)]);
      accountNumberControl?.clearValidators();
      bankNameControl?.clearValidators();
    }
    else if (this.SelectedPaymentMode === 'Bank') {
      accountNumberControl?.setValidators([Validators.required, Validators.pattern(/^\d{9,18}$/)]);
      bankNameControl?.setValidators([Validators.required]);
      cardNumberControl?.clearValidators();
      expiryDateControl?.clearValidators();
      cvvControl?.clearValidators();
    }

    cardNumberControl?.updateValueAndValidity();
    expiryDateControl?.updateValueAndValidity();
    cvvControl?.updateValueAndValidity();
    accountNumberControl?.updateValueAndValidity();
    bankNameControl?.updateValueAndValidity();
  }

  ngOnInit(): void {
  }


  submitPayment() {
    if (this.checkoutForm.valid) {
      this.apiService.processPayment(this.checkoutForm.value).subscribe(
        (response: any) => {
          if (response) {
            alert('Payement Successful');
            this.checkoutForm.reset();
          } else {
            alert('Payment failed');
          }
        },
        (error) => {
          alert('Payment process failed.');
        }
      );
    }
  }

}
