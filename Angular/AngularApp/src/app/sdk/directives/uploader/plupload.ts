/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
/*
declare var require: any;
import $ from 'jquery';
const plupload = require("../../../../../public/assets/plugins/plupload-2.3.6/js/plupload.full.min.js");
import {
  Component,
  Input,
  Output,
  ElementRef,
  EventEmitter,
  OnInit,
  OnChanges,
  ChangeDetectorRef
} from "@angular/core";
//import { iPlUpload } from "./iPlUpload";

@Component({
    selector: "app-plupload",
    templateUrl: "./plupload.html",
    standalone: true
})
export class PlUploadDirective implements OnInit, OnChanges {
  @Input() options: any;
  @Input() totaluploads = 0; // share stats of already uploaded files
  @Input() disposeUploader = false;

  @Output() onCompletion = new EventEmitter<any>();
  @Output() uploadProgress = new EventEmitter<any>();

  selectedFiles: any = [];
  uploadedFiles: any = [];
  ProcessCompleted = false;
  startUploading = false;
  tempfilename = "";
  message = "";
  showProgress = false;
  isInitialized = false;
  private el: ElementRef;
  uploader: any;

  constructor(el: ElementRef, private ref: ChangeDetectorRef) {
    this.el = el;
  }

  ngOnInit() {}
  
  InitializeUploader() {
    const _Options = this.options;
    let _uploadedFiles = this.uploadedFiles;
    const _OnCompletion = this.onCompletion;
    const _uploadProgress = this.uploadProgress;
    let _selectedFiles = this.selectedFiles;
    
    const _this = this;
   
    const uploader = new plupload.Uploader({
      runtimes: "html5,html4",
      browse_button: "pickfiles", // you can pass an id...
      container: "plupload_container",
      drop_element: "plupload_container", // 'FileUploadContainer',
      multi_selection: true,
      unique_names: _Options.unique_names,
      chunk_size: _Options.chunksize,
      url: _Options.handlerpath,
      headers: { UGID: "0", UName: _Options.username },
      filters: {
        max_file_size: _Options.maxfilesize,
        mime_types: [
          { title: _Options.extensiontitle, extensions: _Options.extensions }
        ]
      }
    });
    uploader.bind("Init", function(up: any, params: any) {});
    uploader.init();
    uploader.bind("PostInit", function() {
      $("#plupload_container").on(
        {
          click: function(e: any) {
            this.ProcessCompleted = false;
            this.startUploading = true;
            uploader.start();
            return false;
          }
        },
        "#uploadfiles"
      );
    });
    $(() => {
      uploader.bind("FilesAdded", (up: any, files: any) => {
        console.log("files added bind");
        if (_Options.filename !== "") {
          this.tempfilename = _Options.filename;
        }

        const _max_files = _Options.maxallowedfiles - this.totaluploads;

        _selectedFiles = files;
        _this.selectedFiles = files;
        if (_this.selectedFiles.length > _max_files) {
          
          _this.uploadProgress.emit({
             validation: false,
             message: 'You can\'t upload more than ' + _Options.maxallowedfiles + ' files'
          })
          
          $.each(files, function(i: any, file: any) {
            uploader.removeFile(file);
          });
          _this.selectedFiles = [];
          $("#uploadfiles").hide();
        } else {
          _this.message = "";
          for (let i = 0; i <= this.selectedFiles.length - 1; i++) {
            this.selectedFiles[i].css = "progress-bar-danger";
            this.selectedFiles[i].percent = 0;
            $("#progress_container").append(
              '<div class="mb-3 ">' +
              _this.selectedFiles[i].name +
                '</div><div class="progress"><div id="progress_' +
                _this.selectedFiles[i].id +
                '" class="progress-bar" role="progressbar" style="width: 0%;" aria-valuemin="0" aria-valuemax="100"><span id="pvalue_' +
                _this.selectedFiles[i].id +
                '">0%</span></div></div>'
            );
          }
          $("#pickfiles").hide();
          _this.ProcessCompleted = false;
          _this.startUploading = true;

          uploader.start();

          _this.uploadProgress.emit({
            validation: true,
            message: 'Uploaded started...'
          })
        }

        up.refresh();
      });
    });

    uploader.bind("UploadProgress", function(up: any, file: any) {
      $("#progress_" + file.id).attr("style", "width: " + file.percent + "%");
      $("#pvalue_" + file.id).html(file.percent + "%");

      for (let i = 0; i <= _selectedFiles.length - 1; i++) {
        if (file.id === _selectedFiles[i].id) {
          _selectedFiles[i].percent = file.percent;
        }
      }
     
    });
    uploader.bind("Error", function(up: any, err: any) {
      

      _this.uploadProgress.emit({
        validation: false,
        message: err.message + ', File: ' + err.file.name
      })
  
      up.refresh(); 
    });
    uploader.bind("FileUploaded", function(up: any, file: any, info: any) {
      const rpcResponse = JSON.parse(info.response);
      // let result = '';
      _this.showProgress = false;
      if (typeof rpcResponse !== "undefined" && rpcResponse.result === "OK") {
        _uploadedFiles.push(rpcResponse);

        $("#progress_" + file.id).addClass("bg-success");
        for (let i = 0; i <= _selectedFiles.length - 1; i++) {
          if (file.id === _selectedFiles[i].id) {
            _selectedFiles[i].percent = 100;
            _selectedFiles[i].css = "progress-bar-success";
          }
        }
        if (_selectedFiles.length === _uploadedFiles.length) {
          _this.ProcessCompleted = true;
          $("#pickfiles").show();

          _OnCompletion.emit(_uploadedFiles);
          // cleanup progress data once submitted
          $("#progress_container").html("");
          // reset
          _this.startUploading = false;
          // _this.fileUploaded = false;
          _selectedFiles = [];
          _uploadedFiles = [];
        }
      } else {
        
        let code;
        let message;
        if (typeof rpcResponse.error !== "undefined") {
          code = rpcResponse.error.code;
          message = rpcResponse.error.message;
          if (message === undefined || message === "") {
            message = rpcResponse.error.data;
          }
        } else {
          code = 0;
          message = "Error uploading the file to the server";
        }
        uploader.trigger("Error", {
          code: code,
          message: message,
          file: ""
        });
        _this.uploadProgress.emit({
          validation: false,
          message: code + ', ' + message
        })
      }
      _this.ref.detectChanges();
    });
  }

  ngOnChanges() {
    if (!this.isInitialized) {
      this.InitializeUploader();
      this.isInitialized = true;
    }
    
  }
}
*/