import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { UserInfo } from '../../../interfaces/interfaces';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit{
  private userService = inject(UserService)
  userName: string = '';

  ngOnInit(): void {
    this.userInfo()
  }
  
  private userInfo() {
    this.userService.userInfo().subscribe({
      next: (res: UserInfo | false) => {
        if (res == false) {
          this.userName = ''
          return;
        }

        this.userName = res.name
      },
      error: (_) =>{
        console.error("Name not found", _)
      }
    })
  }
}
