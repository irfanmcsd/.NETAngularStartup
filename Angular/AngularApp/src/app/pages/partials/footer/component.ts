import { Component } from "@angular/core"

@Component({
    selector: 'app-footer',
    standalone: true,
    templateUrl: './component.html'
})
export class FooterComponent {
     
    year = new Date().getFullYear()
    
    constructor() {}
    
}