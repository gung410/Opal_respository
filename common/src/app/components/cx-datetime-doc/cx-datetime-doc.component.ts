import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'cx-datetime-doc',
  templateUrl: './cx-datetime-doc.component.html',
  styleUrls: ['./cx-datetime-doc.component.scss']
})
export class CxDatetimeDocComponent implements OnInit {
  datetime: string;
  date = new Date();
  minDate = new Date();
  maxDate = new Date();
  placeholder = 'dd/mm/yyyy';
  format = 'dd/mm/yy';
  constructor() {
    this.maxDate.setDate(this.maxDate.getDate() + 5);
  }

  ngOnInit() {
  }

  onDateChange(date: Date) {
    if (date) {
      console.log('Date change: ' + date.toDateString());
    } else {
      console.log('invalid date');
    }
  }

  submit() {
    console.log('Date submit:');
    console.log(this.date);
  }
}
