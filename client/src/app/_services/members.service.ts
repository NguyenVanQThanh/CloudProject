import { inject, Injectable, model, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResults } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { setPaginationHeaders, getPaginationResult } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  private accountService = inject(AccountService)
  membersCache = new Map();
  user = this.accountService.currentUser();
  userParams =  signal<UserParams>(new UserParams(this.user));
  constructor(private http: HttpClient) {

  }

  getMembers(userParams: UserParams) {
    const response = this.membersCache.get(Object.values(userParams).join('-'));
    if (response) {
      console.log("Get Response");
      return of(response);
    }
    let params = setPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender ?? '');
    params = params.append('orderBy', userParams.orderBy);

    return getPaginationResult<Member[]>(
      this.baseUrl + 'users',
      params,
      this.http
    )
    .pipe(
      map((response) => {
        this.membersCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    );
  }
  getMember(username: string) {
    const member = [...this.membersCache.values()]
        .reduce((arr, elem)=>
          arr.concat(elem.result)
        ,[])
        .find((member: Member)=> member.userName === username);
    if (member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }
  //  getHttpOptions(){
  //   const userString = localStorage.getItem('user');
  //   if (!userString) return;
  //   const user = JSON.parse(userString);
  //   return {
  //     headers: new HttpHeaders({
  //       Authorization: `Bearer ${user.token}`
  //     })
  //   }
  //  }
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = { ...this.members[index], ...member };
      })
    );
  }
  setMainPhoto(photoId: Number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }
  deletePhoto(photoId: Number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
  addLike(username: string){
    return this.http.post(this.baseUrl + 'likes/' + username,{});
  }
  getLikes(predicate: string, pageNumber: number, pageSize: number){
    let params = setPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return getPaginationResult<Member[]>(this.baseUrl + 'likes', params, this.http);
  }
  getUserParams(){
    return this.userParams;
  }
  setUserParams(params: UserParams){
    this.userParams.set(params);
  }
  resetUserParams(){
    this.userParams.set(new UserParams(this.user));
  }

}
