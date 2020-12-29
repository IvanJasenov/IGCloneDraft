import { PhotoCommentsDto } from "./InstagramPhotos/photoCommentsDto";
import { photoLikesDto } from "./InstagramPhotos/photoLikesDto";

export interface Photo {
  id: number;
  url: string;
  isMain: boolean;
  isAppoved: boolean;
  photoCommentsDto: PhotoCommentsDto[];
  photoLikesDto: photoLikesDto[];
}
