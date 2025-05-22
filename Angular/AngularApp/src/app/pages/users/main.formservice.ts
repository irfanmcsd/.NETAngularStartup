import { Injectable, inject } from '@angular/core';
import * as Controls from '../../sdk/components/reactiveform/model/elements';
import { FormBase } from '../../sdk/components/reactiveform/model/base';
import { AppConfig } from '../../configs/app.configs';
import { CoreService } from '../../sdk/services/coreService';

import { IUSER, UserModel } from '../../store_v2/user/model';
import { ICONFIG } from '../../store_v2/configs/model';
import { NormalRegex } from '../../sdk/services/regexService';

@Injectable()
export class FormService {
  private config = inject(AppConfig);
  private coreService = inject(CoreService);

  constructor() {}

  // reactive form generator
  GenerateControls(entity: UserModel, configs: ICONFIG[]) {
    const controls: FormBase<any>[] = [];

    let UserTypes = configs[0].configs.user.types;

    controls.push(
      new Controls.RadioButtonList({
        key: 'type',
        label: 'Service Provider',
        required: true,
        value: entity.type.toString(),
        options: UserTypes,
        isVisible: false,
        order: 1,
        helpblock: '',
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'firstname',
        label: 'First Name',
        value: entity.firstName,
        placeholder: 'First Name',
        minLength: 3,
        maxLength: 50,
        order: 2,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'lastname',
        label: 'Last Name',
        value: entity.lastName,
        placeholder: 'Last Name',
        minLength: 3,
        maxLength: 50,
        order: 3,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'email',
        label: 'Email Address',
        placeholder: 'Email',
        value: entity.email,
        required: true,
        email: true,
        order: 4,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'password',
        type: 'password',
        label: 'Password',
        value: entity.password,
        placeholder: 'Password',
        minLength: 5,
        maxLength: 20,
        required: true,
        pattern: NormalRegex.PASSWORD_REGEX,
        order: 5,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'cpassword',
        type: 'password',
        label: 'Confirm Password',
        placeholder: 'Confirm Password',
        value: entity.cpassword,
        minLength: 5,
        maxLength: 20,
        required: true,
        order: 6,
      })
    );

    return controls.sort((a, b) => a.order - b.order);
  }

  UpdateProfile(entity: UserModel, configs: ICONFIG[]) {
    const controls: FormBase<any>[] = [];

    controls.push(
      new Controls.Textbox({
        key: 'firstname',
        label: 'Name',
        value: entity.firstName,
        placeholder: 'Enter Full Name',
        minLength: 3,
        maxLength: 50,
        order: 2,
      })
    );

    /*controls.push(
      new Controls.Textbox({
        key: 'lastname',
        label: 'Last Name',
        value: entity.lastName,
        placeholder: 'Last Name',
        minLength: 3,
        maxLength: 50,
        order: 3,
      })
    );*/

    controls.push(
      new Controls.Textbox({
        key: 'phoneNumber',
        label: 'Phone',
        value: entity.phoneNumber,
        placeholder: 'Enter phone number',
        minLength: 6,
        maxLength: 50,
        order: 3,
      })
    );

    return controls.sort((a, b) => a.order - b.order);
  }

  UpdateUserProfile(entity: UserModel, configs: ICONFIG[]) {
    const controls: FormBase<any>[] = [];

    let width: number = 300;
    let height: number = 300;
    if (configs.length > 0) {
      let settings = configs[0].configs.settings.blog;
      width = settings.thumbWidth;
      height = settings.thumbHeight;
    }
    // Cover Settings
    const cropperOptions = {
      cropped_picture: entity.avatar,
      croptype: 1, // general cropped settings
      upload_id: 'cover_upload',
      colcss: 'col-md-12',
      settings: {
        width: width,
        height: height,
      },
      uploadbtntext: 'Upload Avatar',
      btncss: 'btn btn-outline-success px-5 radius-30 mt-3 mb-3',
    };

    controls.push(
      new Controls.ImageCropper({
        key: 'avatar',
        label: '',
        value: entity.avatar,
        required: false,
        cropperOptions: cropperOptions,
        order: 1,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'firstname',
        label: 'Name',
        value: entity.firstName,
        placeholder: 'Enter Full Name',
        minLength: 3,
        maxLength: 50,
        order: 2,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'lastname',
        label: 'Last Name',
        value: entity.lastName,
        placeholder: 'Last Name',
        minLength: 3,
        maxLength: 50,
        order: 3,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'phoneNumber',
        label: 'Phone',
        value: entity.phoneNumber,
        placeholder: 'Enter phone number',
        minLength: 6,
        maxLength: 50,
        order: 3,
      })
    );

    return controls.sort((a, b) => a.order - b.order);
  }

  ChangeEmailControls(entity: UserModel, configs: ICONFIG[]) {
    const controls: FormBase<any>[] = [];

    controls.push(
      new Controls.Textbox({
        key: 'email',
        label: 'Email Address',
        placeholder: 'Email',
        value: entity.email,
        required: true,
        email: true,
        order: 4,
      })
    );

    return controls.sort((a, b) => a.order - b.order);
  }

  ChangePasswordControls(entity: UserModel, configs: ICONFIG[]) {
    const controls: FormBase<any>[] = [];

    controls.push(
      new Controls.Textbox({
        key: 'password',
        type: 'password',
        label: 'Password',
        value: entity.password,
        placeholder: 'Password',
        minLength: 5,
        maxLength: 20,
        required: true,
        pattern: NormalRegex.PASSWORD_REGEX,
        order: 5,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'cpassword',
        type: 'password',
        label: 'Confirm Password',
        placeholder: 'Confirm Password',
        value: entity.cpassword,
        minLength: 5,
        maxLength: 20,
        required: true,
        order: 6,
      })
    );

    return controls.sort((a, b) => a.order - b.order);
  }

  ChangeUserPasswordControls(entity: UserModel, configs: ICONFIG[]) {
    const controls: FormBase<any>[] = [];

     controls.push(
      new Controls.Textbox({
        key: 'epassword',
        type: 'password',
        label: 'Current Password',
        value: entity.epassword,
        placeholder: 'Enter current password',
        minLength: 5,
        maxLength: 20,
        required: true,
        pattern: NormalRegex.PASSWORD_REGEX,
        order: 4,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'password',
        type: 'password',
        label: 'Password',
        value: entity.password,
        placeholder: 'Enter New Password',
        minLength: 5,
        maxLength: 20,
        required: true,
        pattern: NormalRegex.PASSWORD_REGEX,
        order: 5,
      })
    );

    controls.push(
      new Controls.Textbox({
        key: 'cpassword',
        type: 'password',
        label: 'Confirm Password',
        placeholder: 'Confirm Password',
        value: entity.cpassword,
        minLength: 5,
        maxLength: 20,
        required: true,
        order: 6,
      })
    );

    return controls.sort((a, b) => a.order - b.order);
  }

  UpdateUserRoleControls(entity: UserModel, configs: ICONFIG[]) {
    const controls: FormBase<any>[] = [];

    let UserTypes = configs[0].configs.user.types;

    controls.push(
      new Controls.RadioButtonList({
        key: 'type',
        label: 'Service Provider',
        required: true,
        value: entity.type.toString(),
        options: UserTypes,
        isVisible: false,
        order: 1,
        helpblock: '',
      })
    );

    return controls.sort((a, b) => a.order - b.order);
  }

  UpdateAvatarControls(entity: UserModel, configs: ICONFIG[]) {
    const controls: FormBase<any>[] = [];

    let width: number = 300;
    let height: number = 300;
    if (configs.length > 0) {
      let settings = configs[0].configs.settings.blog;
      width = settings.thumbWidth;
      height = settings.thumbHeight;
    }
    // Cover Settings
    const cropperOptions = {
      cropped_picture: entity.avatar,
      croptype: 1, // general cropped settings
      upload_id: 'cover_upload',
      colcss: 'col-md-12',
      settings: {
        width: width,
        height: height,
      },
      uploadbtntext: 'Upload Avatar',
      btncss: 'btn btn-outline-success px-5 radius-30 mt-3 mb-3',
    };

    controls.push(
      new Controls.ImageCropper({
        key: 'avatar',
        label: '',
        value: entity.avatar,
        required: false,
        cropperOptions: cropperOptions,
        order: 1,
      })
    );

    return controls.sort((a, b) => a.order - b.order);
  }
}
