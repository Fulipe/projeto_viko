import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { UserInfo } from '../../../interfaces/interfaces';

@Component({
  selector: 'app-users-table',
  imports: [],
  templateUrl: './users-table.component.html',
  styleUrl: './users-table.component.scss'
})
export class UsersTableComponent implements OnInit{
  private userService = inject(UserService)
  
  loading: boolean = true

  teachers: UserInfo[] = []
  students: UserInfo[] = []

  ngOnInit(): void {
    this.getAllUsers()
  }

  getAllUsers(){
    this.userService.getAllUsers().subscribe({
      next: (res)=>{
        const e:any = res
        const users: UserInfo[] = e

        this.teachers = users.filter(u => u.role == "Teacher")
        this.students = users.filter(u => u.role == "Student")

        this.loading = false
        
      }    
    })
  }
}
