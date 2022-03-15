import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Profile } from './profile.model';
import { PasswordChange } from './password.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  constructor(private http: HttpClient) { }

  getPublisher(id: string): Observable<Profile> {
    return this.http.get<Profile>(environment.publishersApiUrl + "publishers/" + id)
  }

  editPublisher(id: string, model: Profile): Observable<null> {
    return this.http.put<null>(environment.publishersApiUrl + "publishers/" + id, model)
  }

  changePassword(model: PasswordChange) {
    return this.http.put(environment.identityApiUrl + 'identity/changePassword', model);
  }
}
