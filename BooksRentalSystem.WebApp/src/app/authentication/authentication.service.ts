import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { LoginFormModel } from "./login/login.model";
import { RegisterModelForm } from "./register/register.model";

@Injectable({
  providedIn: "root",
})
export class AuthenticationService {
  constructor(private http: HttpClient) {}

  register(model: RegisterModelForm): Observable<any> {
    return this.http.post(
      environment.identityApiUrl + "identity/register",
      model
    );
  }

  login(model: LoginFormModel): Observable<any> {
    return this.http.post(environment.identityApiUrl + "identity/login", model);
  }

  getPublisherId(): Observable<any> {
    return this.http.get(environment.publishersApiUrl + "publishers/id");
  }

  setToken(token: string) {
    localStorage.setItem("token", token);
  }

  setId(publisherId: string) {
    localStorage.setItem("publisherId", publisherId);
  }
}
