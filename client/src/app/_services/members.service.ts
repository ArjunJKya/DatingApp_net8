import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, model, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { AccountService } from './account.service';
import { map, of, tap } from 'rxjs';
import { Photo } from '../_models/Photo';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { UrlSegment } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  baseUrl = environment.apiUrl;
 // members = signal<Member[]>([]);
 user = this.accountService.currentUser();
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  memberCase =  new Map();
  //userParams = model<UserParams>(new UserParams(this.user));

  // resetUserParams(){
  //    this.userParams.set(new UserParams(this.user));
  // }

  
  getMembers(userParamas:UserParams){
    const response = this.memberCase.get(Object.values(userParamas).join('-'));
    console.log(Object.values(userParamas).join('-'));

    if(response)return this.setPaginatedResponse(response);
     
    let params = this.setPaginationHeaders(userParamas.pageNumber,userParamas.pageSize);

      params = params.append('minAge', userParamas.minAge);
      params = params.append('maxAge', userParamas.maxAge);
      params = params.append('gender', userParamas.gender);
      params = params.append('orderBy', userParamas.orderBy);

    return this.http.get<Member[]>(this.baseUrl + 'users',{observe : 'response',params}).subscribe({
      next:response => {
        this.setPaginatedResponse(response);
        this.memberCase.set(Object.values(userParamas).join('-'),response)
      }
    });
  }

  private setPaginatedResponse(response : HttpResponse<Member[]>){
    this.paginatedResult.set({
      items: response.body as Member[],
      pagination:JSON.parse(response.headers.get('Pagination')!)
    })
  }

  private setPaginationHeaders(pageNumber:number,pageSize:number){
    let params = new HttpParams();
    if(pageNumber && pageSize){
      params = params.append('pageNumber',pageNumber);
      params = params.append('pageSize',pageSize);
    }
    return params; 
  }

  getMember(username : string){
    const member : Member = [...this.memberCase.values()]
      .reduce((arr,elem) => arr.concat(elem.body),[])
      .find((m:Member)=>m.username === username);

      if(member) return of (member);

    // const member = this.members().find(x => x.username === username)
    // if(member !== undefined) return of (member) ;
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member:Member){
    return this.http.put(this.baseUrl + 'users',member).pipe(
      // tap(() => {
      //   this.members.update(members => members.map(m => m.username === member.username ? member : m))
      // })
    )
  }

  setMainPhoto(photo:Photo){
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photo.id,{}).pipe(
      // tap(()=>{
      //   this.members.update(members => members.map(m => {
      //     if(m.photos.includes(photo)){
      //       m.photoUrl= photo.url
      //     }
      //     return m;
      //   }))
      // })
    )
  }

  deletePhoto(photo:Photo){
    return this.http.delete(this.baseUrl + 'users/delete-photo/'+ photo.id).pipe(
      // tap(()=>{
      //   this.members.update(members => members.map(m => {
      //     if(m.photos.includes(photo)){
      //       m.photos = m.photos.filter(x=>x.id === photo.id)
      //     }
      //     return m;
      //   }))
      // })

    );
  }


}
