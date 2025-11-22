import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { UserInfo } from '../../../interfaces/interfaces';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-users-table',
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './users-table.component.html',
  styleUrl: './users-table.component.scss'
})
export class UsersTableComponent implements OnInit {
  private userService = inject(UserService)

  loading: boolean = true

  admins: UserInfo[] = []
  teachers: UserInfo[] = []
  students: UserInfo[] = []

    // Sorting states
  sortColumnAdmins: keyof UserInfo | "" = "";
  sortAscAdmins: boolean = true;

  sortColumnTeachers: keyof UserInfo | "" = "";
  sortAscTeachers: boolean = true;

  sortColumnStudents: keyof UserInfo | "" = "";
  sortAscStudents: boolean = true;

  availableRoles = ["Student", "Teacher", "Admin"];

  ngOnInit(): void {
    this.getAllUsers()
  }

  getAllUsers() {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        const e: any = res
        const users: UserInfo[] = e

        this.admins = users.filter(u => u.role == "Admin")
        this.teachers = users.filter(u => u.role == "Teacher")
        this.students = users.filter(u => u.role == "Student")

        this.loading = false

      }
    })
  }

  onRoleChange(user: UserInfo, newRole: string) {

    // Atualizar no array local (UI)
    user.role = newRole;

    // Chamada API para persistir no backend
    this.userService.updateUserRole(user.username, newRole).subscribe({
      next: () => {
         console.log("Role updated successfully")

         this.getAllUsers()
      },
      error: (err) => console.error("Error updating role", err)
    });
  }

    compare(a: any, b: any, column: string, asc: boolean) {
    let valA = (a[column] ?? "").toString().toLowerCase();
    let valB = (b[column] ?? "").toString().toLowerCase();

    return asc
      ? valA.localeCompare(valB)
      : valB.localeCompare(valA);
  }

    //-----------------------------------------
  // SORT ADMIN TABLE
  //-----------------------------------------
  sortAdmins(column: keyof UserInfo) {
    if (this.sortColumnAdmins === column) {
      this.sortAscAdmins = !this.sortAscAdmins;
    } else {
      this.sortColumnAdmins = column;
      this.sortAscAdmins = true;
    }

    this.admins.sort((a, b) =>
      this.compare(a, b, column, this.sortAscAdmins)
    );
  }

  //-----------------------------------------
  // SORT TEACHER TABLE
  //-----------------------------------------
  sortTeachers(column: keyof UserInfo) {
    if (this.sortColumnTeachers === column) {
      this.sortAscTeachers = !this.sortAscTeachers;
    } else {
      this.sortColumnTeachers = column;
      this.sortAscTeachers = true;
    }

    this.teachers.sort((a, b) =>
      this.compare(a, b, column, this.sortAscTeachers)
    );
  }

  //-----------------------------------------
  // SORT STUDENT TABLE
  //-----------------------------------------
  sortStudents(column: keyof UserInfo) {
    if (this.sortColumnStudents === column) {
      this.sortAscStudents = !this.sortAscStudents;
    } else {
      this.sortColumnStudents = column;
      this.sortAscStudents = true;
    }

    this.students.sort((a, b) =>
      this.compare(a, b, column, this.sortAscStudents)
    );
  }
  

}
