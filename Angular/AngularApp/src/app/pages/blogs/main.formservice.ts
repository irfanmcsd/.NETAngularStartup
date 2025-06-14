import { Injectable, inject } from '@angular/core';
import * as Controls from '../../sdk/components/reactiveform/model/elements';
import { FormBase } from '../../sdk/components/reactiveform/model/base';
import { AppConfig } from '../../configs/app.configs';
import { CoreService } from '../../sdk/services/coreService';

// Models
import { BlogModel } from '../../store_v2/blog/model';
import { ICONFIG } from '../../store_v2/configs/model';
import { Types as CategoryTypes } from '../../store_v2/category/model';

@Injectable()
export class FormService {
  private config = inject(AppConfig);
  private coreService = inject(CoreService);

  constructor() {}

  // reactive form generator
  generateControls(entity: BlogModel, configs: ICONFIG[]) {
    const controls: FormBase<any>[] = [];

    let isEdit: boolean = false;
    let labelGenerator: boolean = true;
    if (entity.id > 0) {
       isEdit = true;
       labelGenerator = false;
    }
    let width: number = 300;
    let height: number = 300;
    if (configs.length > 0) {
      let settings = configs[0].configs.settings.blog;
      width = settings.thumbWidth;
      height = settings.thumbHeight;
    }
    // Cover Settings
    const cropperOptions = {
      cropped_picture: entity.cover,
      croptype: 1, // general cropped settings
      upload_id: 'cover_upload',
      colcss: 'col-md-12',
      settings: {
        width: width,
        height: height,
      },
      uploadbtntext: 'Upload Cover',
      btncss: 'btn btn-outline-success px-5 radius-30 mt-3 mb-3',
    };

    controls.push(
      new Controls.ImageCropper({
        key: 'cover',
        label: '',
        value: entity.cover,
        required: false,
        cropperOptions: cropperOptions,
        order: 1,
      })
    );

    // title
    let _title = entity.blog_data.title;

    controls.push(
      new Controls.Textbox({
        key: 'title',
        label: 'Slug',
        placeholder: 'Enter term to generate slug',
        value: _title,
        required: true,
        minLength: 3,
        maxLength: 150,
        order: 1,
        labelGenerator: labelGenerator,
        helpblock: '',
      })
    );

    // term -> slug
    controls.push(
      new Controls.LabelGenerator({
        key: 'term',
        value: entity.term,
        options: [
          {
            rawText: '',
            slug: entity.term,
            isupdate: isEdit,
            type: 2, // 5: categories, 6: Users, 2: Blogs
          },
        ],
        isVisible: true,
        order: 2,
      })
    );

    
    let dataOptions: any = {
      type: 1, // 0: category, 1: blog
      cultures: [],
      data: entity.blog_culture_data,
    };
    if (configs.length > 0) {
      dataOptions.cultures = configs[0].configs.cultures;
    }

    controls.push(
      new Controls.MultiCulture_Category({
        key: 'blog_culture_data',
        categoryOptions: dataOptions,
        tinymiceOptions: this.coreService.prepareInitAdvacneEditorSettings(),
        required: false,
        order: 5,
        helpblock: '',
      })
    );

    let _options: any = this.coreService.getMultiCategorySettings();
    _options.categorytype = CategoryTypes.Blogs;
    _options.dropdown_type = 1; // 0: normal, 1: multi dropdown
    _options.isparentchild = true; // if dropdown_type = 0
    _options.manualload = false; // if true skip auto category loading

    controls.push(
      new Controls.MultiDropdown({
        key: 'categorylist',
        label: 'Select Categories',
        categoryOptions: this.coreService.prepareSelectedItems(
          entity.categorylist
        ),
        multiselectOptions: _options, // this.coreService.getMultiCategorySettings(),
        required: false,
        helpblock: `Please select one or more categories.`,
        order: 6,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'tags',
        label: 'Labels',
        value: entity.tags,
        required: false,
        order: 7,
        maxLength: 500,
        helpblock: `Please enter one or more tags, separated by commas.`,
      })
    );

    let publish_types: any = [
      { key: '1', value: 'Draft' },
      { key: '0', value: 'Publish' },
    ];

    controls.push(
      new Controls.RadioButtonList({
        key: 'isdraft',
        label: 'Publish Status',
        required: true,
        value: entity.isdraft.toString(),
        options: publish_types,
        colsize: 'col-md-4 col-sm-6',
        order: 9,
      })
    );

    return controls.sort((a, b) => a.order - b.order);
  }
}
