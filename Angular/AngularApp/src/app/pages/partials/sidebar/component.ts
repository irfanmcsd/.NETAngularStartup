import { Component, inject, Input, OnInit } from "@angular/core";
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CoreService } from "../../../sdk/services/coreService";
import { AppConfig } from '../../../configs/app.configs';
import { USER_ROLE_ENTITY, UserRole } from "../../../store_v2/auth/model";

// Interface for strongly-typed navigation items
interface NavItem {
  index: number;
  title: string;
  url: string;
  icon?: string;
  collapse?: boolean;
  sub_nav?: NavItem[];
}

@Component({
  selector: 'app-sidebar',
  templateUrl: './component.html',
  standalone: true,
  imports: [CommonModule, RouterModule],
  providers: [CoreService]
})
export class SideBarComponent implements OnInit {

  public appConfig = inject(AppConfig);
  private coreService = inject(CoreService);

  @Input() pageRole: UserRole = USER_ROLE_ENTITY;

  navigation: NavItem[] = [];

  constructor() {}

  ngOnInit(): void {
    if (this.pageRole.admin || this.pageRole.super_admin) {
       this.navigation = this.prepareAdminSidebarNav();
    } else {
       this.navigation =  this.prepareAccountSidebarNav();
    }
  }

 private prepareAccountSidebarNav(): NavItem[] {
    return [
      {
        index: 0,
        title: 'Dashboard',
        url: '/',
        icon: 'bx bx-home-circle',
        sub_nav: []
      },
      {
        index: 1,
        title: 'Profile',
        url: '/user-profile',
        icon: 'bx bx-user-circle',
        sub_nav: []
      },
      {
        index: 2,
        title: 'Manage Account',
        url: '/manage-account',
        icon: 'bx bx-cog',
        sub_nav: []
      },
    

    ];
  }

  /**
   * Prepares the sidebar navigation items
   * @returns Array of navigation items
   */
  private prepareAdminSidebarNav(): NavItem[] {
    return [
      {
        index: 0,
        title: 'Dashboard',
        url: '/',
        icon: 'bx bx-home-circle',
        sub_nav: []
      },
      {
        index: 1,
        title: 'Users',
        url: '/users',
        icon: 'bx bx-user-circle',
        sub_nav: []
      },
      
      {
        index: 5,
        title: 'Blogs',
        url: '/blogs',
        icon: 'bx bx-spreadsheet',
        sub_nav: []
      },
      {
        index: 1,
        title: 'Documentation',
        url: '/documentation',
        icon: 'bx bx-line-chart',
        sub_nav: []
      },
      {
        index: 6,
        title: 'Settings',
        url: '#',
        icon: 'bx bx-grid-alt',
        collapse: true,
        sub_nav: [
         
          {
            index: 2,
            title: 'Tags',
            url: '/tags'
          },
          {
            index: 3,
            title: 'Logs',
            url: '/logs'
          },
         
        ]
      },
     
    ];
  }

  /**
   * Toggles the collapse state of a navigation item
   * @param nav The navigation item to toggle
   */
  toggleMenu(nav: NavItem): void {
    nav.collapse = !nav.collapse;
  }
}