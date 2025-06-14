import { Injectable, inject } from '@angular/core';
import * as Controls from '../../../sdk/components/reactiveform/model/elements';
import { FormBase } from '../../../sdk/components/reactiveform/model/base';
import { AppConfig } from '../../../configs/app.configs';
import { CoreService } from '../../../sdk/services/coreService';

// Models
import { ICONFIG } from '../../../store_v2/configs/model';
import {
  ICATEGORY,
  CATEGORY_QUERY_OBJECT,
  CategoryModel,
  CategorySettings,
} from '../../../store_v2/category/model';

@Injectable()
export class FormService {
  private config = inject(AppConfig);
  private coreService = inject(CoreService);

  constructor() {}

  // reactive form generator
  generateControls(entity: CategoryModel, configs: ICONFIG[]) {
    const controls: FormBase<any>[] = [];

    let isEdit: boolean = false;
    let labelGenerator: boolean = true;

    if (entity.id > 0) {
      isEdit = true;
      labelGenerator = false;
    }

    // cropper (avatar) => base64
    let width: number = 300;
    let height: number = 300;
    if (configs.length > 0) {
      let settings = configs[0].configs.settings.category;
      width = settings.thumbWidth;
      height = settings.thumbHeight;
    }
    const cropperOptions = {
      cropped_picture: entity.avatar,
      croptype: 1,
      upload_id: 'category_avatar',
      colcss: 'col-lg-2 col-md-4',
      settings: {
        width: width,
        height: height,
      },
      uploadbtntext: 'Upload Avatar',
      btncss: 'btn btn-outline-success px-5 radius-30',
    };
    controls.push(
      new Controls.ImageCropper({
        key: 'avatar',
        label: '',
        value: entity.avatar,
        required: false,
        cropperOptions: cropperOptions,
        helpblock:
          'Cropsize: ' +
          cropperOptions.settings.width +
          'x' +
          cropperOptions.settings.height,
        order: 0,
      })
    );

    // title
    let _title = entity.category_data.title;
   
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
            type: 5, // 5: categories, 6: Users, 2: Blogs
          },
        ],
        isVisible: true,
        order: 2,
      })
    );

    let categoryOptions: any = {
       type: 0, // 0: category, 1: blog
       cultures: [],
       data: entity.culture_categories
    }
    if (configs.length > 0) {
       categoryOptions.cultures = configs[0].configs.cultures;
    }

   
    controls.push(
      new Controls.MultiCulture_Category({
        key: 'culture_categories',
        categoryOptions: JSON.parse(JSON.stringify(categoryOptions)) ,
        tinymiceOptions: this.coreService.prepareInitEditorSettings(),
        required: false,
        order: 5,
        helpblock: '',
      })
    );

    return controls.sort((a, b) => a.order - b.order);
  }
}
