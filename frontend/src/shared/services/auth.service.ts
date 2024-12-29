import { BehaviorSubject } from 'rxjs';
import type { IUserRoles } from '../models/IUserRoles';

const AuthServiceToken = 'auth_token';

export class AuthService {
  private roles$ = new BehaviorSubject<IUserRoles>({ role: 'Admin' });

  getRoles() {
    return this.roles$.value;
  }

  getRoles$() {
    return this.roles$.asObservable();
  }
}

export const authService = new AuthService();
