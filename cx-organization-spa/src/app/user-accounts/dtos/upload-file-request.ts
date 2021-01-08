export class UploadFileRequest {
  file: File;
  currentUserExId: string;

  constructor(file: File, currentUserExId: string) {
    if (!file || !currentUserExId) {
      return;
    }

    this.file = file;
    this.currentUserExId = currentUserExId;
  }

  toFormData(): FormData {
    const formData = new FormData();
    formData.append('file', this.file, this.file.name);
    formData.append('currentUserExId', `${this.currentUserExId}`);

    return formData;
  }
}
