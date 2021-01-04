import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PhotoComment } from '../_models/creteComment';
import { PhotoCommentObject } from '../_models/InstagramPhotos/photoComment';
import { photoLikesDto } from '../_models/InstagramPhotos/photoLikesDto';
import { PhCommentCreatorDto, PhotoWithComments } from '../_models/photoWithComments';

@Injectable({
  providedIn: 'root'
})
export class PhotosService {
  baseUrl = environment.apiUrl;
  private photoLikes = new BehaviorSubject<photoLikesDto[]>(null);
  public photoLikes$ = this.photoLikes.asObservable();

  constructor(private http: HttpClient) { }

  getPhotos() {
    return this.http.get<Partial<PhotoWithComments[]>>(this.baseUrl + 'photo' + '/get-all-photos-with-com-urs');
  }

  createPhotoComment(phtoComment: PhotoComment, photoId: number) {
    return this.http.post<PhCommentCreatorDto>(this.baseUrl + 'photo' + '/create-comment/' + photoId, phtoComment);
  }

  getCommentsForPhoto(photoId: number) {
    return this.http.get<PhotoWithComments>(this.baseUrl + 'photo/comments-for-photo/' + photoId);
  }

  deletePhotoComment(photoId: number) {
    return this.http.delete(this.baseUrl + 'photo/delete-photo-comment/' + photoId);
  }

  editPhotoComment(photoId: number, commentObject: PhotoCommentObject) {
    return this.http.put(this.baseUrl + 'photo/edit-photo-comment/' + photoId, commentObject);
  }

  // obsolete
  getNumberOfLikesPerPhoto(photoId: number) {
    this.http.get(this.baseUrl + 'photo/get-photo-likes-by-photoId/' + photoId).subscribe((res: photoLikesDto[]) => {
      this.photoLikes.next(res);
    }, error => console.log('error:', error));
  }

  getLikesForPhotoId(photoId: number) {
    return this.http.get<photoLikesDto[]>(this.baseUrl + 'photo/get-photo-likes-by-photoId/' + photoId);
  }
}
