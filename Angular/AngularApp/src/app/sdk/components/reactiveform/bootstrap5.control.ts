/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  AfterViewInit,
  ChangeDetectorRef,
  ViewChild,
  inject,
} from '@angular/core';
import { FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormBase } from './model/base';
import { Store } from '@ngrx/store';

// Store Selectors
import {
  renderNotify,
  renderSelector,
} from '../../../store_v2/core/notify/notify.reducers';
import { pushData } from '../../../store_v2/core/event/event.reducers';

// Third-party Components
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { CropperViewComponent } from '../../components/cropie/modal';
import { FormViewComponent } from '../../modals/forms/modal.component';

// Custom Form Components
import { LabelGeneratorComponent } from './components/labelgenerator/label.generator.component';
import { ChipSelectorComponent } from './components/chips/chips.component';
import { CustomSelectComponent } from './components/customselect/customselect.component';
import { CategorySelectorComponent } from './components/categoryselector/categoryselector.component';
import { MultiOptionsComponent } from './components/multioptions/multiopitons.component';
import { GoogleMapComponent } from './components/map/map.component';
//import { PlUploadDirective } from '../../directives/uploader/plupload';
import { MultiCategoryComponent } from './components/multicategory/multicategory.component';
//import { AutoCompleteComponent } from './components/autocomplete/autocomplete.component';
import { CultureCategoryComponent } from './components/multiculture/category.component';

// Angular Common
import {
  NgSwitch,
  NgIf,
  NgSwitchCase,
  NgClass,
  NgFor,
  JsonPipe,
} from '@angular/common';

// Third-party Modules
//import { DpDatePickerModule } from 'ng2-date-picker';
import { QuillModule } from 'ngx-quill';
import { NgxMaskDirective } from 'ngx-mask';

/**
 * Bootstrap4FormControl - A comprehensive form control wrapper component
 *
 * Features:
 * - Handles multiple form control types (inputs, selects, uploaders, etc.)
 * - Manages form validation and error messages
 * - Integrates with NgRx for state management
 * - Supports file uploads and image cropping
 * - Provides modal integration for complex controls
 * - Manages dynamic form behaviors
 */
@Component({
  selector: 'app-bootstrap-5',
  templateUrl: './bootstrap5.control.html',
  styleUrls: ['./bootstrap5.control.css'],
  providers: [],
  standalone: true,
  imports: [
    // Angular Modules
    FormsModule,
    ReactiveFormsModule,
    NgSwitch,
    NgIf,
    NgSwitchCase,
    NgClass,
    NgFor,
    JsonPipe,

    // Third-party Modules
    //DpDatePickerModule,
    QuillModule,
    NgxMaskDirective,

    // Custom Form Components
    CultureCategoryComponent,
    //AutoCompleteComponent,
    MultiCategoryComponent,
    //PlUploadDirective,
    GoogleMapComponent,
    MultiOptionsComponent,
    CategorySelectorComponent,
    CustomSelectComponent,
    ChipSelectorComponent,
    LabelGeneratorComponent,
  ],
})
export class Bootstrap4FormControl implements OnInit, AfterViewInit {
  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  private readonly store = inject(Store);
  private readonly modalService = inject(NgbModal);
  private readonly ref = inject(ChangeDetectorRef);

  /* ---------------------------- INPUT PROPERTIES ---------------------------- */
  @Input() control!: FormBase<any>;
  @Input() form!: FormGroup;
  @Input() isSubmit = false;
  @Input() navigationForm = false;

  /* ---------------------------- OUTPUT EVENTS ---------------------------- */
  @Output() OnDropdownSelectionChange = new EventEmitter<any>();
  @Output() OnFileRemoved = new EventEmitter<any>();
  @Output() uploadStatus = new EventEmitter<any>();
  @Output() uploadCompleted = new EventEmitter<any>();
  @Output() autoTextChange = new EventEmitter<string>();
  @Output() onFeatures = new EventEmitter<any>();

  /* ---------------------------- VIEW CHILDREN ---------------------------- */
  @ViewChild('auto') auto: any;

  /* ---------------------------- COMPONENT STATE ---------------------------- */
  showUploadBtn = true;
  cropperView = false;
  uploadedFiles: any = [];
  readCropperImage: string | ArrayBuffer = '';

  /* ---------------------------- ERROR MESSAGES ---------------------------- */
  private errorMessages: Record<string, (params: any) => string> = {
    email: () => 'Invalid email address',
    required: (params) => `${params.key} is required`,
    minlength: (params) =>
      `The min number of characters is ${params.requiredLength}`,
    maxlength: (params) =>
      `The max allowed number of characters is ${params.requiredLength}, you typed ${params.actualLength}`,
    min: (params) =>
      `Value (entered: ${params.actual}) should be greater than ${params.min}`,
    max: (params) =>
      `Value (entered: ${params.actual}) should be less than ${params.max}`,
    pattern: (params) => `Incorrect ${params.key}`,
    years: (params) => params.message,
    countryCity: (params) => params.message,
    uniqueName: (params) => params.message,
    telephoneNumbers: (params) => params.message,
    telephoneNumber: (params) => params.message,
  };

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */

  ngOnInit(): void {
    if (
      this.control.controlType === 'uploader' &&
      this.control.value?.length > 0
    ) {
      this.uploadedFiles = [...this.control.value];
    }
  }

  ngAfterViewInit(): void {}

  /* ---------------------------- FORM VALIDATION METHODS ---------------------------- */

  get isValid(): boolean {
    return this.form.controls[this.control.key].valid;
  }

  hasErrors(): boolean {
    const control = this.form.controls[this.control.key];
    return (
      !!control?.errors && (control.dirty || control.touched || this.isSubmit)
    );
  }

  listOfErrors(): string[] {
    const errors = this.form.controls[this.control.key].errors;
    return errors
      ? Object.keys(errors).map((field) =>
          this.getErrorMessage(field, errors[field])
        )
      : [];
  }

  private getErrorMessage(type: string, params: any): string {
    const { key, label } = this.control;

    switch (type) {
      case 'required':
        return label ? `${label} is required` : `${key} is required`;
      case 'pattern':
        return label ? `Incorrect ${label}` : `Incorrect ${key}`;
      default:
        return this.errorMessages[type]?.(params) || 'Invalid field';
    }
  }

  /* ---------------------------- CONTROL EVENT HANDLERS ---------------------------- */

  onFocusOutEvent(
    key: string,
    labelGenerator: boolean,
    mapRender: boolean,
    event: Event
  ): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;

    if (mapRender || labelGenerator) {
      const actionType = mapRender ? 'map-render' : 'text-focus';
      this.store.dispatch(
        pushData({
          event: [{ action: actionType, data: { key, value } }],
        })
      );
    }
  }

  onItemSelect(item: any): void {
    this.emitDropdownChange(item.value);
  }

  /* ---------------------------- FILE UPLOAD METHODS ---------------------------- */

  filesUploaded(files: any[]): void {
    if (files.length > 0) {
      files[0].selected = this.uploadedFiles.length === 0;
      this.uploadedFiles = [...this.uploadedFiles, ...files];
      this.control.value = this.uploadedFiles;
      this.uploadCompleted.emit(files);
    }
  }

  uploadProgress(obj: any): void {
    if (obj.validation === false) {
      this.store.dispatch(
        renderNotify({
          title: obj.message,
          text: '',
          css: 'bg-danger',
        })
      );
    }
    this.uploadStatus.emit(obj);
  }

  removedItems(output: { files: any[]; removedItems: any }): void {
    this.uploadedFiles = output.files;
    this.control.value = this.uploadedFiles;
    this.OnFileRemoved.emit({
      key: this.control.key,
      file: output.removedItems,
    });
  }

  /* ---------------------------- IMAGE CROPPER METHODS ---------------------------- */

  handleImageUpload(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.readImageFile(input.files[0]);
    }
  }

  private readImageFile(file: File): void {
    const reader = new FileReader();
    reader.onloadend = () => {
      this.readCropperImage = reader.result as string;
      this.control.cropperOptions.original_picture = this.readCropperImage;
      this.ref.detectChanges();
      this.openCropperModal();
    };
    reader.readAsDataURL(file);
  }

  private openCropperModal(): void {
    const modalRef = this.modalService.open(CropperViewComponent, {
      backdrop: false,
      size: 'lg',
    });

    modalRef.componentInstance.Info = {
      title: 'Editor',
      data: this.control.cropperOptions,
      cropoption: this.control.cropperOptions.croptype,
      settings: this.control.cropperOptions.settings,
      scroller: true,
    };

    modalRef.result.then(
      (result) => {
        this.control.value = result.data.image;
        this.control.cropperOptions.cropped_picture = result.data.image;
        this.ref.detectChanges();
      },
      () => console.log('Cropper modal dismissed')
    );
  }

  /* ---------------------------- UTILITY METHODS ---------------------------- */

  getKey(index: number, item: any): string {
    return item.key;
  }

  private emitDropdownChange(value: any): void {
    this.OnDropdownSelectionChange.emit({
      key: this.control.key,
      value,
    });
  }

  removeCropperImage(event: any) {
    this.control.cropperOptions.cropped_picture = '';
    event.stopPropagation();
  }

  remove(obj: any, index: number, event: any) {
    if (index > -1) {
      this.uploadedFiles.splice(index, 1);
      this.removedItems({ files: this.uploadedFiles, removedItems: obj });
    }
    event.stopPropagation();
  }

  selected(obj: any, event: any) {
    for (let file of this.uploadedFiles) {
      file.selected = false;
    }
    obj.selected = true;
    event.stopPropagation();
  }

  changeOption(option: any) {
    this.control.value = option;

    this.OnDropdownSelectionChange.emit({
      key: this.control.key,
      value: this.control.value,
    });
  }

  // Image Cropper Functionality
  changeListener($event: any): void {
    this.readThis($event.target);
  }

  readThis(inputValue: any): void {
    const file: File = inputValue.files[0];
    const myReader: FileReader = new FileReader();
    const _this = this;
    myReader.onloadend = function (e) {
      const reader: string = myReader.result as string;
      _this.readCropperImage = reader;
      _this.control.cropperOptions.original_picture = _this.readCropperImage;
      _this.ref.detectChanges();
      _this.modal_popup();
    };
    myReader.readAsDataURL(file);
  }

  modal_popup() {
    const _options: NgbModalOptions = {
      backdrop: false,
      size: 'lg',
    };
    console.log('crop options');
    console.log(this.control.cropperOptions);
    const modalRef = this.modalService.open(CropperViewComponent, _options);
    modalRef.componentInstance.Info = {
      title: 'Editor',
      data: this.control.cropperOptions,
      cropoption: this.control.cropperOptions.croptype,
      settings: this.control.cropperOptions.settings,
      scroller: true,
    };
    modalRef.result.then(
      (result) => {
        this.control.value = result.data.image;
        this.control.cropperOptions.cropped_picture = result.data.image;
        this.ref.detectChanges();
      },
      (dismissed) => {
        console.log('dismissed');
      }
    );
  }

  selectedDropdownValue(key: any, value: any) {
    this.OnDropdownSelectionChange.emit({ key, value });
  }

  helpLoader(help: string, event: any) {
    const _options: NgbModalOptions = {
      backdrop: false,
      size: 'lg',
    };
    const _this = this;
    const modalRef = this.modalService.open(FormViewComponent, _options);
    modalRef.componentInstance.Info = {
      type: 1,
      title: 'Help & Instructions',
      description: help,
    };
    modalRef.result.then(
      (result) => {},
      (dismissed) => {
        console.log('dismissed');
      }
    );
    event.stopPropagation();
  }

}

/*
import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  AfterViewInit,
  ChangeDetectorRef,
  ViewChild,
  inject,
} from '@angular/core';
import { FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormBase } from './model/base';
import { Store } from '@ngrx/store';

// Selectors
import { renderNotify, renderSelector } from '../../../store_v2/core/notify/notify.reducers';
import { pushData } from '../../../store_v2/core/event/event.reducers';

// cropper directives
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { CropperViewComponent } from '../../components/cropie/modal';
import { FormViewComponent } from '../../modals/forms/modal.component';

import { LabelGeneratorComponent } from './components/labelgenerator/label.generator.component';
import { ChipSelectorComponent } from './components/chips/chips.component';
import { CustomSelectComponent } from './components/customselect/customselect.component';
import { CategorySelectorComponent } from './components/categoryselector/categoryselector.component';
import { MultiOptionsComponent } from './components/multioptions/multiopitons.component';
import { GoogleMapComponent } from './components/map/map.component';
import { PlUploadDirective } from '../../directives/uploader/plupload';
import { MultiCategoryComponent } from './components/multicategory/multicategory.component';
import { AutoCompleteComponent } from './components/autocomplete/autocomplete.component';
import { CategorySelectorNavComponent } from './components/categoryselectornav/categoryselectornav.component';
import {
  NgSwitch,
  NgIf,
  NgSwitchCase,
  NgClass,
  NgFor,
  JsonPipe,
} from '@angular/common';
import { DpDatePickerModule } from 'ng2-date-picker';
import { QuillModule } from 'ngx-quill';
import { NgxMaskDirective } from 'ngx-mask';

@Component({
  selector: 'app-bootstrap-5',
  templateUrl: './bootstrap5.control.html',
  styleUrls: ['./bootstrap5.control.css'],
  providers: [],
  standalone: true,
  imports: [
    DpDatePickerModule,
    QuillModule,
    NgxMaskDirective,
    FormsModule,
    ReactiveFormsModule,
    NgSwitch,
    NgIf,
    NgSwitchCase,
    NgClass,
    NgFor,
    JsonPipe,
    CategorySelectorNavComponent,
    AutoCompleteComponent,
    MultiCategoryComponent,
    PlUploadDirective,
    GoogleMapComponent,
    MultiOptionsComponent,
    CategorySelectorComponent,
    CustomSelectComponent,
    ChipSelectorComponent,
    LabelGeneratorComponent,
  ],
})
export class Bootstrap4FormControl implements OnInit, AfterViewInit {
  private store = inject(Store);
  private modalService = inject(NgbModal);

  @Input() control!: FormBase<any>;
  @Input() form!: FormGroup;
  @Input() isSubmit = false;
  @Input() navigationForm = false;
  @Output() OnDropdownSelectionChange = new EventEmitter<any>();

  @Output() OnFileRemoved = new EventEmitter<any>();
  @Output() uploadStatus = new EventEmitter<any>();
  @Output() uploadCompleted = new EventEmitter<any>();
  @Output() autoTextChange = new EventEmitter<string>();
  @Output() onFeatures = new EventEmitter<any>();

  constructor(private ref: ChangeDetectorRef) {}

  @ViewChild('auto') auto: any;
  showUploadBtn = true;
  cropperView = false;
  uploadedFiles: any = [];
  readCropperImage: string | ArrayBuffer = '';

  private errorMessages: any = {
    email: (params: any) => 'Invalid email address',
    required: (params: any) => params.key + ' is required',
    minlength: (params: any) =>
      'The min number of characters is ' + params.requiredLength, //  (params.requiredLength - params.actualLength) + ' characters needed',
    maxlength: (params: any) =>
      'The max allowed number of characters is ' +
      params.requiredLength +
      ', you typed ' +
      params.actualLength,
    min: (params: any) =>
      'Value (entered: ' +
      params.actual +
      ') should be greater than ' +
      params.min,
    max: (params: any) =>
      'Value (entered: ' +
      params.actual +
      ') should be less than ' +
      params.max,
    pattern: (params: any) => 'Incorrect ' + params.key,
    years: (params: any) => params.message,
    countryCity: (params: any) => params.message,
    uniqueName: (params: any) => params.message,
    telephoneNumbers: (params: any) => params.message,
    telephoneNumber: (params: any) => params.message,
  };

  ngOnInit() {
    // console.log('ng on after init called');
    if (this.control.controlType === 'uploader') {
      if (this.control.value.length > 0) {
        for (const file of this.control.value) {
          this.uploadedFiles.push(file);
        }
      }
    }
  }

  ngAfterViewInit() {}

  get isValid() {
    return this.form.controls[this.control.key].valid;
  }

  hasErrors(): boolean | null {

    return (
      this.form.controls[this.control.key] &&
      this.form.controls[this.control.key].errors &&
      (this.form.controls[this.control.key].dirty ||
        this.form.controls[this.control.key].touched ||
        this.isSubmit)
    );
  }

  listOfErrors(): string[] {
    if (this.form.controls[this.control.key].errors! !== null) {
      return Object.keys(this.form.controls[this.control.key].errors!).map(
        (field) =>
          this.getMessage(
            field,
            this.control.key,
            this.control.label,
            this.form.controls[this.control.key].errors![field]
          )
      );
    } else {
      return [];
    }
  }

  private getMessage(type: string, key: string, label: string, params: any) {
    if (type === 'required') {
      if (label !== undefined && label !== '') {
        return label + ' is required';
      } else {
        return key + ' is required';
      }
    } else if (type === 'pattern') {
      if (label !== '') {
        return 'Incorrect ' + label;
      } else {
        return 'Incorrect ' + key;
      }
    } else {
      return this.errorMessages[type](params);
    }
  }

  // pass dropdown selection value with key reference to parent component
  selectedDropdownValue(key: any, value: any) {
    this.OnDropdownSelectionChange.emit({ key, value });
  }

  onFocusOutEvent(key: any, labelGenerator: any, mapRender: any, event: any) {
    // address textbox => googlemap render component
    if (mapRender) {
      this.store.dispatch(
        pushData({
          event: [{
            action: 'map-render',
            data: {
              key: key,
              value: event.target.value,
            },
          }]
        })
      );
      
    }
    // label => label generator component
    else if (labelGenerator) {
      this.store.dispatch(
        pushData({
          event: [{
            action: 'text-focus',
            data: {
              key: key,
              value: event.target.value,
            },
          }]
        })
      );
    }
  }

  onItemSelect(item: any) {
    this.OnDropdownSelectionChange.emit({
      key: this.control.key,
      value: item.value,
    });
  }

  filesUploaded(files: any) {
    for (const file of files) {
      if (this.uploadedFiles.length === 0) {
        file.selected = true;
      }
      this.uploadedFiles.push(file);
    }

    this.control.value = this.uploadedFiles;
    this.uploadCompleted.emit(files);
  }

  uploadProgress(obj: any) {
    if (obj.validation === false) {
      this.store.dispatch(
        renderNotify({ title: obj.message, text: '', css: 'bg-danger' })
      );
     
    }
    this.uploadStatus.emit(obj);
  }

  filesUploaded_v2(files: any) {
    //
  }

  removedItems(output: any) {
    this.uploadedFiles = output.files;
    this.control.value = this.uploadedFiles;
    this.OnFileRemoved.emit({
      key: this.control.key,
      file: output.removedItems,
    });
  }

  choose(value: any, event: any) {
    this.control.value = value;
  }

  changeCheckedValues(arr: any) {
    const options = arr
      .filter((opt: any) => opt.checked)
      .map((opt: any) => opt.value);
  }

  // Image Cropper Functionality
  changeListener($event: any): void {
    this.readThis($event.target);
  }

  readThis(inputValue: any): void {
    const file: File = inputValue.files[0];
    const myReader: FileReader = new FileReader();
    const _this = this;
    myReader.onloadend = function (e) {
      const reader: string = myReader.result as string;
      _this.readCropperImage = reader;
      _this.control.cropperOptions.original_picture = _this.readCropperImage;
      _this.ref.detectChanges();
      _this.modal_popup();
    };
    myReader.readAsDataURL(file);
  }

  modal_popup() {
    const _options: NgbModalOptions = {
      backdrop: false,
      size: 'lg',
    };
    console.log('crop options');
    console.log(this.control.cropperOptions);
    const modalRef = this.modalService.open(CropperViewComponent, _options);
    modalRef.componentInstance.Info = {
      title: 'Editor',
      data: this.control.cropperOptions,
      cropoption: this.control.cropperOptions.croptype,
      settings: this.control.cropperOptions.settings,
      scroller: true,
    };
    modalRef.result.then(
      (result) => {
        this.control.value = result.data.image;
        this.control.cropperOptions.cropped_picture = result.data.image;
        this.ref.detectChanges();
      },
      (dismissed) => {
        console.log('dismissed');
      }
    );
  }

  toggleModelForm(event: any) {
    const _options: NgbModalOptions = {
      backdrop: false,
      size: 'lg',
    };
    const _this = this;
    //console.log("crop options");
    //console.log(this.control.cropperOptions);
    const modalRef = this.modalService.open(FormViewComponent, _options);
    modalRef.componentInstance.Info = {
      type: 0,
      title: this.control.formOptions.title,
      data: this.control.formOptions.controls,
      isedit: this.control.formOptions.isedit,
    };
    modalRef.result.then(
      (result) => {
        _this.onFeatures.emit({ data: result.data });
      },
      (dismissed) => {
        console.log('dismissed');
      }
    );
    event.stopPropagation();
  }

  removeCropperImage(event: any) {
    this.control.cropperOptions.cropped_picture = '';
    event.stopPropagation();
  }

  remove(obj: any, index: number, event: any) {
    if (index > -1) {
      this.uploadedFiles.splice(index, 1);
      this.removedItems({ files: this.uploadedFiles, removedItems: obj });
    }
    event.stopPropagation();
  }

  selected(obj: any, event: any) {
    for (let file of this.uploadedFiles) {
      file.selected = false;
    }
    obj.selected = true;
    event.stopPropagation();
  }

  changeOption(option: any) {
    this.control.value = option;

    this.OnDropdownSelectionChange.emit({
      key: this.control.key,
      value: this.control.value,
    });
  }

  getKey(index: number, item: any): string {
    return item.key;
  }

  helpLoader(help: string, event: any) {
    const _options: NgbModalOptions = {
      backdrop: false,
      size: 'lg',
    };
    const _this = this;
    const modalRef = this.modalService.open(FormViewComponent, _options);
    modalRef.componentInstance.Info = {
      type: 1,
      title: 'Help & Instructions',
      description: help,
    };
    modalRef.result.then(
      (result) => {},
      (dismissed) => {
        console.log('dismissed');
      }
    );
    event.stopPropagation();
  }

  onCheckChanged(event: any) {
    console.log(event);
  }
}
*/
