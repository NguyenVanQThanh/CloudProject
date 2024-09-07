import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../_models/user';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';
import { take } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {
  private adminService = inject(AdminService);
  private modalService = inject(BsModalService);
  private toastrService = inject(ToastrService);
  users : User[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  ngOnInit(): void {
    this.getUsersWithRoles();
    // console.log(this.users);
  }
  openRolesModal(user: User){
    const initialState : ModalOptions = {
      class: 'modal-lg',
      initialState: {
        title: 'User roles',
        userName: user.userName,
        selectedRoles: [...user.roles],
        availableRoles: ['Admin', 'Moderator', 'Member'],
        rolesUpdated: false
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, initialState);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        if (this.bsModalRef.content && this.bsModalRef.content.rolesUpdated){
          const selectedRoles = this.bsModalRef.content.selectedRoles;
          this.adminService.updateUserRoles(user.userName,selectedRoles).subscribe({
            next: roles => {
              user.roles = roles;
              this.toastrService.success("Role updated successfully")
              this.getUsersWithRoles(); // If you want to update the roles list as well
            }
          });
        }
      }
    })
  }
  getUsersWithRoles(){
    this.adminService.getUserWithRoles().subscribe({
      next: users => {
        this.users = users;
        console.log(users)
      }
    })
  }

}
