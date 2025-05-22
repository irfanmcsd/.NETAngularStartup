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
  OnInit,
  OnChanges,
  EventEmitter,
  ChangeDetectorRef,
  AfterViewInit 
} from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { AppConfig } from '../../../configs/app.configs';
import { FormBase } from "./model/base";
import { ControlService } from "./services/control.service";
import { Bootstrap4FormControl } from "./bootstrap5.control";
import { LoaderComponent } from "../loader/loader.component";
import { NgIf, NgFor } from "@angular/common";

/**
 * DynamicModalFormComponent - A reusable dynamic form component with modal support
 * 
 * Features:
 * - Dynamically generates forms based on control configurations
 * - Handles form submission and validation
 * - Supports modal display with customizable buttons
 * - Manages file uploads and dropdown selections
 * - Provides skip/cancel functionality
 * - Integrates with ControlService for form generation
 */
@Component({
  selector: "dynamic-modal-form",
  templateUrl: "./dynamic-modal-form.html",
  providers: [ControlService],
  standalone: true,
  imports: [
    NgIf, 
    LoaderComponent, 
    FormsModule, 
    ReactiveFormsModule, 
    NgFor, 
    Bootstrap4FormControl
  ]
})
export class DynamicModalFormComponent implements OnInit, OnChanges, AfterViewInit {
  /* ---------------------------- INPUT PROPERTIES ---------------------------- */
  @Input() controls: FormBase<any>[] = [];      // Array of form controls configuration
  @Input() navigationForm = false;              // Flag for navigation-style form
  @Input() showCancel = true;                   // Show/hide cancel button
  @Input() showModal = true;                    // Display as modal
  @Input() submitText = "Submit";               // Submit button text
  @Input() cancelText = "Cancel";               // Cancel button text
  @Input() showSkip = false;                    // Show/hide skip button
  @Input() skipBtnText = "Back";                // Skip button text
  @Input() submitBtnCss = "btn btn-dark px-5";  // Submit button CSS classes
  @Input() submitValidation = true;             // Enable form validation
  @Input() showLoader = false;                  // Show/hide loading indicator

  /* ---------------------------- OUTPUT EVENTS ---------------------------- */
  @Output() OnClose = new EventEmitter<any>();            // Emits on modal close
  @Output() OnSkip = new EventEmitter<any>();             // Emits on skip action
  @Output() OnSubmit = new EventEmitter<any>();           // Emits on form submit
  @Output() OnDropdownSelection = new EventEmitter<any>();// Emits dropdown changes
  @Output() FileRemoved = new EventEmitter<any>();        // Emits file removal
  @Output() onFeatures = new EventEmitter<any>();         // Emits feature events

  /* ---------------------------- FORM STATE ---------------------------- */
  form!: FormGroup;                            // Reactive form group
  payLoad = "";                                // Form payload data
  isSubmit = false;                            // Form submission state
  disableSubmit = false;                       // Submit button disabled state
  tempSubmitText = "";                         // Temporary submit button text

  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  constructor(
    private qcs: ControlService,               // Control service for form generation
    private ref: ChangeDetectorRef,            // Change detector reference
    public config: AppConfig                   // Application configuration
  ) {}

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */
  
  ngOnInit(): void {
    this.generateForm();
  }

  ngAfterViewInit(): void {
    this.ref.detectChanges();
  }

  ngOnChanges(): void {
    this.generateForm();
  }

  /* ---------------------------- FORM MANAGEMENT ---------------------------- */
  
  /**
   * Generates the reactive form based on control configurations
   */
  generateForm(): void {
    if (this.controls?.length > 0) {
      this.form = this.qcs.toFormGroup(this.controls);
    }
  }

  /**
   * Handles form submission with validation
   */
  onSubmit(): void {
    this.isSubmit = this.submitValidation;
    
    if (this.form.valid) {
      this.payLoad = this.form.getRawValue();
      this.OnSubmit.emit(this.payLoad);
      this.clearFormValidators();
    }
  }

  /**
   * Clears all form validators after submission
   */
  private clearFormValidators(): void {
    this.controls.forEach(control => {
      this.form.get(control.key)?.clearValidators();
      this.form.get(control.key)?.updateValueAndValidity();
    });
  }

  /* ---------------------------- EVENT HANDLERS ---------------------------- */
  
  /**
   * Handles dropdown selection changes
   * @param payload The selection payload
   */
  onDropdownSelectionChange(payload: any): void {
    this.OnDropdownSelection.emit(payload);
  }

  /**
   * Handles file removal events
   * @param payload The file removal payload
   */
  onFileRemoved(payload: any): void {
    this.FileRemoved.emit(payload);
  }

  /**
   * Handles feature submission events
   * @param payload The feature payload
   */
  onFeatureSubmit(payload: any): void {
    this.onFeatures.emit(payload);
  }

  /**
   * Handles upload completion
   * @param files The uploaded files
   */
  uploadCompleted(files: any): void {
    this.submitText = this.tempSubmitText;
    this.disableSubmit = false;
  }

  /**
   * Handles upload progress
   * @param isenabled Upload state
   */
  uploadProgress(isenabled: any): void {
    this.disableSubmit = true;
    this.tempSubmitText = this.submitText;
    this.submitText = "Uploading...";
  }

  /**
   * Closes the modal form
   * @param event The click event
   */
  close(event: Event): void {
    this.OnClose.emit(true);
    event.preventDefault();
  }

  /**
   * Handles skip action
   * @param event The click event
   */
  skip(event: Event): void {
    this.isSubmit = false;
    this.clearFormValidators();
    this.payLoad = this.form.getRawValue();
    this.OnSkip.emit(this.payLoad);
    event.preventDefault();
  }
}

/*
import {
  Component,
  Input,
  Output,
  OnInit,
  OnChanges,
  EventEmitter,
  ChangeDetectorRef,
  AfterViewInit 
} from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { AppConfig } from '../../../configs/app.configs'
import { FormBase } from "./model/base";
import { ControlService } from "./services/control.service";
import { Bootstrap4FormControl } from "./bootstrap5.control";
import { LoaderComponent } from "../loader/loader.component";
import { NgIf, NgFor } from "@angular/common";

@Component({
    selector: "dynamic-modal-form",
    // changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: "./dynamic-modal-form.html",
    providers: [ControlService],
    standalone: true,
    imports: [NgIf, LoaderComponent, FormsModule, ReactiveFormsModule, NgFor, Bootstrap4FormControl]
})
export class DynamicModalFormComponent implements OnInit, OnChanges, AfterViewInit {
  @Input() controls: FormBase<any>[] = [];
  @Input() navigationForm = false;
  @Input() showCancel = true;
  @Input() showModal = true;
  @Input() submitText = "Submit";
  @Input() cancelText = "Cancel";
  @Input() showSkip = false;
  @Input() skipBtnText = "Back";

  @Input() submitBtnCss = "btn btn-dark px-5";
  @Input() submitValidation = true
  @Output() OnClose = new EventEmitter<any>();
  @Output() OnSkip = new EventEmitter<any>();
  @Output() OnSubmit = new EventEmitter<any>();
  @Output() OnDropdownSelection = new EventEmitter<any>();
  @Output() FileRemoved = new EventEmitter<any>();
  @Output() onFeatures = new EventEmitter<any>();
  
  @Input() showLoader = false;

  tempSubmitText = "";
  form!: FormGroup;
  payLoad = "";
  isSubmit = false;
  
  disableSubmit = false;

  constructor(private qcs: ControlService,private ref: ChangeDetectorRef,
    public config: AppConfig) {}

  ngOnInit() {
    
    this.generateForm();
    
  }

  ngAfterViewInit() {
  
    this.ref.detectChanges();
  }

  ngOnChanges() {
    
    this.generateForm();
  }

  generateForm() {
    if (this.controls.length > 0) {
      this.form = this.qcs.toFormGroup(this.controls);
    }
  
  }

  OnDropdownSelectionChange(payload: any) {
    this.OnDropdownSelection.emit(payload);
  }


  OnFileRemoved(payload: any) {
    this.FileRemoved.emit(payload);
  }

  onFeatureSubmit(payload: any) {
    this.onFeatures.emit(payload);
  }

  uploadCompleted(files: any) {
    this.submitText = this.tempSubmitText;
    this.disableSubmit = false;
  }

  uploadProgress(isenabled: any) {
    this.disableSubmit = true;
    this.tempSubmitText = this.submitText;
    this.submitText = "Uploading...";
  }

  onSubmit() {

    this.isSubmit = this.submitValidation;
    if (this.form.valid) {
      this.payLoad = this.form.getRawValue(); //this.form.value;
      this.OnSubmit.emit(this.payLoad);
      for (const control of this.controls) {
        this.form.get(control.key)?.clearValidators();
        this.form.get(control.key)?.updateValueAndValidity();
      }
    } 
  }

  close(event: any) {
    this.OnClose.emit(true);
    event.preventDefault();
  }

  skip(event: any) {
   
    this.isSubmit = false;
    
    for (const control of this.controls) {
     
      this.form.get(control.key)?.clearValidators();
      this.form.get(control.key)?.updateValueAndValidity();
    }

    this.payLoad = this.form.getRawValue(); //this.form.value;

    this.OnSkip.emit(this.payLoad);
    event.preventDefault();
  }
  
}
*/