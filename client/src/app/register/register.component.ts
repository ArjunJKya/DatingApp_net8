import { Component, EventEmitter, inject, input, Input, output, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  // @Input() userFormHomeComponent: any;         //angular old verson Input decorator
  //userFormHomeComponent = input.required<any>();  //anglar 17.0.0 input docartor 
  // @Output() cancelRegister =  new EventEmitter(); //angular old verson Output decorator
  
  cancelRegister = output<boolean>();                   //anglar 17.0.0 output docartor 

  private accountService=inject(AccountService)
  modal: any = {};
  user:any;

  register() {
   this.accountService.register(this.modal).subscribe({
    next: response =>{
      console.log(response);
      this.cancle();
    },
    error:error=>console.log(error)
   })
  }


  cancle() {
    this.cancelRegister.emit(false);
  }

}
