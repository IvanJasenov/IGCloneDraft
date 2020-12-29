import { User } from './user';

export class UserParams {
  gender: string;
  minAge = 18;
  maxAge = 99;
  pageNumber = 1;
  itemsPerPage = 5;
  orderBy = 'lastActive'; // by default to last active as in the API

  // i do the gender filtering in the api, so this one is not really needed
  constructor(user: User) {
    this.gender = user.gender;
  }
}
