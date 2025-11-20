import { Component } from '@angular/core';
import { UsersTableComponent } from "../../../../components/admin/users-table/users-table.component";

@Component({
  selector: 'app-dashboard-admin',
  imports: [UsersTableComponent],
  templateUrl: './dashboard-admin.component.html',
  styleUrl: './dashboard-admin.component.scss'
})
export class DashboardAdminComponent {

}
