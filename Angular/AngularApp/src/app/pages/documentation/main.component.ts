import { Component } from "@angular/core"
import { MainCategoryComponent } from "../../sections/category/components/main.component";

@Component({
    templateUrl: './main.html',
    standalone: true,
    imports: [
        MainCategoryComponent
    ]
})
export class MainComponent {
     
    constructor() {}
    
}