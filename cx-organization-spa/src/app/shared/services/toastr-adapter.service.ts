import { Injectable } from '@angular/core';
import { ActiveToast, IndividualConfig, ToastrService } from 'ngx-toastr';

@Injectable()
export class ToastrAdapterService {
  constructor(private toastrService: ToastrService) {}

  success(
    message?: string,
    title?: string,
    override?: Partial<IndividualConfig>
  ): ActiveToast<any> {
    return this.toastrService.success(message, title, override);
  }

  info(
    message?: string,
    title?: string,
    override?: Partial<IndividualConfig>
  ): ActiveToast<any> {
    return this.toastrService.info(message, title, override);
  }

  error(
    message?: string,
    title?: string,
    override?: Partial<IndividualConfig>
  ): ActiveToast<any> {
    return this.toastrService.error(message, title, override);
  }

  warning(
    message?: string,
    title?: string,
    override?: Partial<IndividualConfig>
  ): ActiveToast<any> {
    return this.toastrService.warning(message, title, override);
  }
}
