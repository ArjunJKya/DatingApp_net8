import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private http = inject(HttpClient)
  baseUrl='https://localhost:5001/api/';

  currentUser = signal<User | null>(null);

  login(modal:any){
    return this.http.post<User>(this.baseUrl + 'account/login',modal).pipe(
      map(user => {
        if(user){
          localStorage.setItem('user',JSON.stringify(user));
          this.currentUser.set(user);
        }
      })
    )
  }

  register(modal:any){
    return this.http.post<User>(this.baseUrl + 'account/register',modal).pipe(
      map(user => {
        if(user){
          localStorage.setItem('user',JSON.stringify(user));
          this.currentUser.set(user);
        }
        return user;
      })
    )
  
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  
}
