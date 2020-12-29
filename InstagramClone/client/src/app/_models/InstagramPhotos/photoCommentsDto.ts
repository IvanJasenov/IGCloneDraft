export interface PhotoCommentsDto {
  photoId: number;
  appUserId: number;
  commentId: number;
  photoComentatoUsername: string;
  photoComment: string;
  dateCreated: Date;
}
