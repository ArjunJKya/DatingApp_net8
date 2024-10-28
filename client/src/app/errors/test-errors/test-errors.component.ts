import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';

@Component({
  selector: 'app-test-errors',
  standalone: true,
  imports: [],
  templateUrl: './test-errors.component.html',
  styleUrl: './test-errors.component.css'
})
export class TestErrorsComponent {

  baserUrl = 'https://localhost:5001/api/';
  private http = inject(HttpClient)
  validationErrors: string[] = [];

  get400Error() {
    this.http.get(this.baserUrl + 'buggy/bad-request').subscribe({
      next: respones => console.log(respones),
      error: error => console.log(error)
    })
  }

  get401Error() {
    this.http.get(this.baserUrl + 'buggy/auth').subscribe({
      next: respones => console.log(respones),
      error: error => console.log(error)
    })
  }

  get404Error() {
    this.http.get(this.baserUrl + 'buggy/not-found').subscribe({
      next: respones => console.log(respones),
      error: error => console.log(error)
    })
  }

  get500Error() {
    this.http.get(this.baserUrl + 'buggy/server-error').subscribe({
      next: respones => console.log(respones),
      error: error => console.log(error)
    })
  }

  get400ValidationError() {
    this.http.post(this.baserUrl + 'account/register', {}).subscribe({
      next: respones => console.log(respones),
      error: error => {
        console.log(error);
        this.validationErrors = error;
      }
    })
  }

}
