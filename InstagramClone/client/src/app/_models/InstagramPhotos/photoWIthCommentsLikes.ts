import { Photo } from "../photo";

export interface PhotosWithCommentsLikes {
  id: number;
  username: string;
  photoUrl: string;
  age: number;
  knownAs: string;
  dateCreated: Date;
  lastActive: Date;
  gender: string;
  introduction: string;
  lookingFor: string;
  interests: string;
  city: string;
  country: string;
  photosDto: Photo;
}
