import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import { UserService } from 'app-services/user.service';

@Component({
  selector: 'avatar',
  templateUrl: './avatar.component.html',
  styleUrls: ['./avatar.component.scss'],
})
export class AvatarComponent implements OnInit, OnChanges {
  @Input() avatarUrl: string;

  constructor(private userService: UserService) {}

  ngOnInit() {}

  ngOnChanges(changes: SimpleChanges): void {}
}
