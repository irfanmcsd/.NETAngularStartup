import { Component, OnInit, Input, inject } from "@angular/core"
import { AppConfig } from '../../../configs/app.configs'
import { CommonModule } from "@angular/common"
import { CoreService } from "../../../sdk/services/coreService"
import { Initial_User_Entity, UserModel } from "../../../store_v2/user/model";

@Component({
    selector: 'app-header',
    templateUrl: './component.html',
    standalone: true,
    imports: [CommonModule],
    providers: [ CoreService ]
})
export class HeaderComponent implements OnInit{
     
    @Input() isAuthenticated: boolean = false;
    @Input() User: UserModel = Object.assign({}, Initial_User_Entity);
    
    private config = inject(AppConfig);
    private coreService = inject(CoreService);

    constructor() {
    }

    ngOnInit(): void {
        
    }
    
}