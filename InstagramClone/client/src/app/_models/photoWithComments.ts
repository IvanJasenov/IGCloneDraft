import { User } from './user';

export interface PhotoWithComments {
  id: number;
  url: string;
  isMain: boolean;
  photoOwner: User;
  phCommentCreatorDtos: PhCommentCreatorDto[];
  userDto: User;
}

export interface PhCommentCreatorDto {
  comment: string;
  dateCreated: Date;
  commentCreator: User;
}
