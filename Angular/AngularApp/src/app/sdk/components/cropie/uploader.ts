/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, OnChanges } from "@angular/core";
import { NgbModal, NgbModalOptions } from "@ng-bootstrap/ng-bootstrap";
import { CropperViewComponent } from "./modal";
import { AppConfig } from "../../../configs/app.configs"

@Component({
    selector: "app-banneruploader",
    templateUrl: "./uploader.html",
    standalone: true
})
export class BannerUploaderComponent implements OnInit, OnChanges, OnDestroy {
  @Input() Info: any;
  @Input() Picture: string = "";
  @Input() Title: string = "";
  @Input() Sub_Title: string = "";
  @Input() Sub_Title_2: string = "";
  @Input() BtnText = "Change Photo";
  @Input() Width = 150;
  @Input() Height = 150;
  @Input() CSS = 'img-fluid';
  @Output() OnCropped = new EventEmitter<any>();
  selectedOption: string = "";
  showUploadBtn = true;
  @Input() CropOption = 0; // 0: user logo, 1: logo (e.g agency), 2: banner
  showLoader = false;
  constructor(private modalService: NgbModal,  public config: AppConfig) {}

  ngOnInit() {  }

  ngOnChanges() {

     this.Info = Object.assign({}, this.Info);
     if (this.Picture !== "") {
       this.Info.img_url = this.Picture;
     }
  }

  save() {
    const obj = {
      id: this.Info.id,
      picturename: this.Info.cropped_picture
    };

    this.OnCropped.emit(obj);
  }

  cancel() {
    this.showUploadBtn = true;
    // reset original photo
    this.Info.cropped_picture = this.Info.original_picture;
  }

  deleteImage(event: any) {
    this.Info.cropped_picture = "";
    this.Info.original_picture = "";
    const obj = {
      id: this.Info.id,
      picturename: ""
    };
    this.showLoader = true;
    event.stopPropagation();
  }

  changeListener($event: any): void {
    this.readThis($event.target);
    this.showUploadBtn = false;
  }

  readThis(inputValue: any): void {
    const file: File = inputValue.files[0];
    const myReader: FileReader = new FileReader();
    const _this = this;
    myReader.onloadend = function(e) {
      // you can perform an action with readed data here
      const _result: string = myReader.result as string
      // const _result: string | ArrayBuffer = reader.result;
      _this.Info.original_picture = _result;
      _this.Info.cropped_picture =_result;
     
      // _this.profile.setProfile(_this.imageForm);
      _this.editThumbnail();
    };
    myReader.readAsDataURL(file);
  }

  editThumbnail() {
    let _scroller = false;
    if (this.CropOption === 2) {
       _scroller = true
    }
    const _options: NgbModalOptions = {
      backdrop: false,
      size: 'lg',
    };
    const modalRef = this.modalService.open(CropperViewComponent, _options);
    
    modalRef.componentInstance.Info = {
      title: "Editor",
      data: this.Info,
      cropoption: this.CropOption,
      settings: { width: this.Width, height: this.Height },
      scroller: _scroller
    };
    modalRef.result.then(
      result => {
        this.Info.cropped_picture = result.data.image;
        this.Info.img_url = result.data.image;
        this.save();
      },
      dismissed => {
        console.log("dismissed");
      }
    );
  }

  ngOnDestroy(): void {
    
  }
}
