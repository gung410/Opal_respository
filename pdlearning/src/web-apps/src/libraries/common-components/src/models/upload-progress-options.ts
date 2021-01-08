export class UploadProgressOptions {
  constructor(public lineWidth: number, public canvasWidth: number) {}

  public get posX(): number {
    return this.canvasWidth / 2 + this.lineWidth / 2;
  }

  public get posY(): number {
    return this.posX;
  }
}
