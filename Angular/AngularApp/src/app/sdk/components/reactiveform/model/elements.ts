/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import { FormBase } from "./base";

/**
 * FORM CONTROL CLASSES
 * ===================
 * 
 * This file contains specialized form control classes that extend the base FormBase class.
 * Each control represents a specific UI form element with its own configuration.
 * 
 * Organization:
 * 1. Basic Input Controls
 * 2. Selection Controls
 * 3. Rich Editors & Media
 * 4. Specialized Inputs
 * 5. Composite Controls
 * 6. Buttons & UI Elements
 */

// ======================
// 1. BASIC INPUT CONTROLS
// ======================

/** Standard text input control */
export class Textbox extends FormBase<string> {
  override controlType = "textbox";
}

/** Multi-line text input control */
export class TextArea extends FormBase<string> {
  override controlType = "textarea";
}

/** Input with masked formatting (phone numbers, etc.) */
export class InputMask extends FormBase<string> {
  override controlType = "inputmask";
}

// ======================
// 2. SELECTION CONTROLS
// ======================

/** Standard dropdown/select control */
export class Dropdown extends FormBase<string> {
  override controlType = "dropdown";
}

/** PrimeNG enhanced dropdown control */
export class PrimeNgDropdown extends FormBase<string> {
  override controlType = "pdropdown";
}

/** Multi-select dropdown control */
export class MultiDropdown extends FormBase<string> {
  override controlType = "multidropdown";
}

/** Radio button list control */
export class RadioButtonList extends FormBase<string> {
  override controlType = "radiolist";
}

/** Single radio button control */
export class RadioButton extends FormBase<boolean> {
  override controlType = "radio";
}

/** Single checkbox control */
export class CheckBox extends FormBase<boolean> {
  override controlType = "check";
}

/** Checkbox list control */
export class CheckBoxList extends FormBase<string> {
  override controlType = "checklist";
}

/** Tag/chip selection control */
export class Chips extends FormBase<string> {
  override controlType = "chips";
}

// ========================
// 3. RICH EDITORS & MEDIA
// ========================

/** Rich text editor control */
export class MediaSoftEditor extends FormBase<string> {
  override controlType = "mediasoft_editor";
}

/** TinyMCE editor control */
export class TinyMyceEditor extends FormBase<string> {
  override controlType = "tinymce";
}

/** Media/file uploader control */
export class Uploader extends FormBase<string> {
  override controlType = "uploader";
}

/** Image cropping control */
export class ImageCropper extends FormBase<string> {
  override controlType = "cropper";
}

// ========================
// 4. SPECIALIZED INPUTS
// ========================

/** Google Maps location input */
export class GoogleMap extends FormBase<string> {
  override controlType = "google_map";
}

/** Date picker input */
export class DatePicker extends FormBase<string> {
  override controlType = "datepicker";
}


/** Label generator control */
export class LabelGenerator extends FormBase<string> {
  override controlType = "label_generator";
}

// ========================
// 5. COMPOSITE CONTROLS
// ========================

/** Multi-element container control */
export class MultiElement extends FormBase<string> {
  override controlType = "multielement";
}

/** Inline elements container */
export class InlineElements extends FormBase<string> {
  override controlType = "inlinelements";
}

/** Category selector control */
export class CategorySelector extends FormBase<string> {
  override controlType = "categoryselector";
}

/** Navigable category selector */
export class CategorySelectorNav extends FormBase<string> {
  override controlType = "categoryselectornav";
}


/** Section header (non-input) */
export class SectionHeader extends FormBase<string> {
  override controlType = "section";
}


/** Multi-Culture - Category Version */
export class MultiCulture_Category extends FormBase<string> {
  override controlType = "culture_category";
}
