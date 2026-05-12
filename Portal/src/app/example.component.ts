import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ClrFormsModule, ClrDropdownModule } from '@clr/angular';

@Component({
  selector: 'app-example',
  templateUrl: './example.component.html',
  styleUrl: './example.component.scss',

  imports: [FormsModule, ClrFormsModule, ClrDropdownModule],
})
export class ExampleComponent {
  form = {
    type: 'local',
    username: '',
    password: '',
    rememberMe: false,
  };
}
