import { Member } from 'src/app/_models/member';

export interface photoLikesDto {
  photoId: number;
  userId: number;
  userDto: Partial<Member>;
  dateCreated: Date;
}
