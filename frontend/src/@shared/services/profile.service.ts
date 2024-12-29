import { AuthService, authService } from '@/shared/services/auth.service';
import { JwtHelperService } from '@/shared/services/jwthelper.service';
import { map, of } from 'rxjs';

export class ProfileService {
  constructor(
    private auth: AuthService,
    private jwtHelper: JwtHelperService
  ) {
  }

  public getLogin$() {
    return of('osulima');
  }
}

export const profileService = new ProfileService(authService, new JwtHelperService());