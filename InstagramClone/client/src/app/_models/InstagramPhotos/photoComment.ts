export class PhotoCommentObject {
  photoCommentOriginal: string
  photoCommentEdited: string

  constructor(photoCommentOriginal: string, photoCommentEdited: string) {
    this.photoCommentOriginal = photoCommentOriginal;
    this.photoCommentEdited = photoCommentEdited;
  }
}
