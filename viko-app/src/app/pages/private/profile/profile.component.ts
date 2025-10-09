import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { UserInfo } from '../../../interfaces/interfaces';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit{
  private userService = inject(UserService) 
  user: any;

  ngOnInit(){
    this.userService.userInfo().subscribe({
      next: (user) => this.user = user,
      error: (err) => console.error('Erro ao carregar perfil:', err)
    })
    console.log(this.user);
  }
}
