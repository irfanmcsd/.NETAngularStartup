/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
export class FormBase<T> {
  // Core Properties
  value: T | undefined;
  key: string = '';
  label: string = '';
  type: string = '';
  controlType: string = '';
  order: number = 1;
  required: boolean = false;
  isVisible: boolean = true;
  isdisabled: boolean = false;
  disabled: boolean = false;

  // Validation Properties
  email: boolean = false;
  minLength?: number;
  maxLength?: number;
  min?: number;
  max?: number;
  pattern?: string;

  // UI Properties
  placeholder: string = '';
  append_text: string = '';
  labelasheading: boolean = false;
  css: string = 'mt-1';
  colsize: string = 'col-md-12';
  rows: number = 4;
  inline: boolean = true;
  checked: boolean = false;

  // Content Properties
  options: any[] = [];
  items: any[] = [];
  checklist: any[] = [];
  helpblock: string = '';
  helpdescription: string = '';

  // Component-Specific Configuration
  uploadoptions: any = {};          // File uploader configuration
  cropperOptions: any = {};         // Image cropper settings
  formOptions: any = {};            // Modal form options
  multiselectOptions: any = {};     // Multi-select dropdown settings
  tinymiceOptions: any = {};        // TinyMCE editor configuration
  autocompleteOptions: any = {};    // Autocomplete settings
  datepickerOptions: any = {};      // Date picker configuration
  primeNgOptions: any = {};         // PrimeNG component settings
  extendedFormOptions: any = {};    // Extended form configurations
  categoryOptions: any = {};        // Category selector options
  mediaOptions: any = {};           // Media selector options
  maskOptions: any = {};            // Input mask settings

  // State Flags
  loader: boolean = false;
  labelGenerator: boolean = false;
  mapRender: boolean = false;

  /**
   * Creates a new FormBase instance
   * @param options Configuration options for the form control
   */
  constructor(options: {
    value?: T;
    type?: string;
    options?: any[];
    key?: string;
    label?: string;
    required?: boolean;
    email?: boolean;
    minLength?: number;
    maxLength?: number;
    min?: number;
    max?: number;
    pattern?: string;
    append_text?: string;
    order?: number;
    controlType?: string;
    checked?: boolean;
    checklist?: any[];
    helpdescription?: string;
    helpblock?: string;
    uploadoptions?: any;
    cropperOptions?: any;
    formOptions?: any;
    multiselectOptions?: any;
    tinymiceOptions?: any;
    autocompleteOptions?: any;
    datepickerOptions?: any;
    primeNgOptions?: any;
    extendedFormOptions?: any;
    categoryOptions?: any;
    mediaOptions?: any;
    maskOptions?: any;
    colsize?: string;
    rows?: number;
    disabled?: boolean;
    items?: any[];
    placeholder?: string;
    labelasheading?: boolean;
    isVisible?: boolean;
    css?: string;
    isdisabled?: boolean;
    loader?: boolean;
    labelGenerator?: boolean;
    mapRender?: boolean;
    inline?: boolean;
  } = {}) {
    // Core Properties
    this.value = options.value;
    this.type = options.type || '';
    this.key = options.key || '';
    this.label = options.label || '';
    this.required = !!options.required;
    this.email = !!options.email;
    this.order = options.order ?? 1;
    this.controlType = options.controlType || '';
    this.isdisabled = options.isdisabled ?? false;
    this.disabled = options.disabled ?? false;
    this.isVisible = options.isVisible ?? true;

    // Validation
    this.minLength = options.minLength;
    this.maxLength = options.maxLength;
    this.min = options.min;
    this.max = options.max;
    this.pattern = options.pattern;

    // UI Properties
    this.append_text = options.append_text || '';
    this.checked = options.checked ?? false;
    this.colsize = options.colsize ?? 'col-md-12';
    this.rows = options.rows ?? 4;
    this.placeholder = options.placeholder || '';
    this.labelasheading = options.labelasheading ?? false;
    this.css = options.css ?? 'mt-1';
    this.inline = options.inline ?? true;

    // Content
    this.options = options.options ?? [];
    this.items = options.items ?? [];
    this.checklist = options.checklist ?? [];
    this.helpblock = options.helpblock ?? '';
    this.helpdescription = options.helpdescription ?? '';

    // Component Configurations
    this.uploadoptions = options.uploadoptions ?? {};
    this.cropperOptions = options.cropperOptions ?? {};
    this.formOptions = options.formOptions ?? {};
    this.multiselectOptions = options.multiselectOptions ?? {};
    this.tinymiceOptions = options.tinymiceOptions ?? {};
    this.autocompleteOptions = options.autocompleteOptions ?? {};
    this.datepickerOptions = options.datepickerOptions ?? {};
    this.primeNgOptions = options.primeNgOptions ?? {};
    this.extendedFormOptions = options.extendedFormOptions ?? {};
    this.categoryOptions = options.categoryOptions ?? {};
    this.mediaOptions = options.mediaOptions ?? {};
    this.maskOptions = options.maskOptions ?? {};

    // State Flags
    this.loader = options.loader ?? false;
    this.labelGenerator = options.labelGenerator ?? false;
    this.mapRender = options.mapRender ?? false;
  }
}
