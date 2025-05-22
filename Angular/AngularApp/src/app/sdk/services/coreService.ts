/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import { Injectable } from '@angular/core';
import * as crypto from 'crypto-js';
import * as Controls from '../../sdk/components/reactiveform/model/elements';

import { ExcelJson } from '../../sdk/interfaces/excel-json.interface';
import { UserRole } from '../../store_v2/auth/model';
@Injectable()
export class CoreService {
  constructor() {}

  // set secret pharase with your own keyword (to encrypt / decrypt sensetive values)
  pharase = 'VPKARHARSE4544';


  getChartOptions() {
    return [
      {
        key: 'bar',
        value: 'Bar Chart',
      },
      {
        key: 'line',
        value: 'Line Chart',
      },
      {
        key: 'pie',
        value: 'Pie Chart',
      },
      {
        key: 'donut',
        value: 'Donut Chart',
      },
      {
        key: 'column',
        value: 'Column Chart',
      },
      {
        key: 'area',
        value: 'Area Chart',
      }
    ]
  }

  ProcessTags(categories: any) {
    const arr = [];
    for (const category of categories) {
      arr.push({
        title: category.trim(),
        slug: category
          .trim()
          .replace(/\s+/g, "-")
          .toLowerCase()
      });
    }

    return arr;
  }

  public processRole(role: string[]): UserRole {
    let _isAdmin = false;
    let _isSuperAdmin = false;
    let _isReadonlyAdmin = false;
    let _isemployer = false;
    let _isagent = false;
    let _isuser = false;
    if (role !== undefined && role !== null) {
      if (role.length > 0) {
        if (
          role[0] === 'SuperAdmin' ||
          role[0] === 'Admin' ||
          role[0] === 'ReadonlyAdmin'
        ) {
          _isAdmin = true;
        }

        if (role[0] === 'SuperAdmin') {
          _isSuperAdmin = true;
        }

        if (role[0] === 'ReadonlyAdmin') {
          _isReadonlyAdmin = true;
        }

        if (role[0] === 'Employer') {
          _isemployer = true;
        }

        if (role[0] === 'Agent') {
          _isagent = true;
        }

        if (role[0] === 'User') {
          _isuser = true;
        }
      }
    }
    return {
      admin: _isAdmin,
      super_admin: _isSuperAdmin,
      readonly_admin: _isReadonlyAdmin,
      employer: _isemployer,
      agent: _isagent,
      user: _isuser,
    };
  }

  static UpperCase(str: string, everyword: boolean = false) {
    var _str = str.replace(/_/g, ' ');
    if (!everyword) {
      return _str.charAt(0).toUpperCase() + _str.slice(1).replace(/_/g, ' ');
    } else {
      const arr = _str.split(' ');
      for (var i = 0; i < arr.length; i++) {
        arr[i] = arr[i].charAt(0).toUpperCase() + arr[i].slice(1);
      }
      return arr.join(' ');
    }
  }

  getValue = <T>(obj: T, key: keyof T) => {
    return obj[key];
  };

  setValue = <T>(obj: T, key: keyof T, value: T[keyof T]) => {
    obj[key] = value;
  };

  getProperty<T, K extends keyof T>(obj: T, key: K) {
    return obj[key]; // Inferred type is T[K]
  }

  public isRoleAssigned(roles: any, roleName: string) {
    for (let role of roles) {
      if (role.name === roleName) return true;
    }
    return false;
  }

  public getAlphabet(value: number) {
    return (value + 9).toString(36).toUpperCase();
  }

  public datatoExcelFormat(data: any[] = [], title: string) {
    if (data.length > 0) {
      let table_header: any = {};
      var keyNames = Object.keys(data[0]);
      let index = 1;
      for (let key of keyNames) {
        table_header[this.getAlphabet(index)] = key;
        index++;
      }

      const edata: Array<ExcelJson> = [];
      const udt: ExcelJson = {
        data: [
          { A: title }, // title
          table_header, // table header
        ],
        skipHeader: true,
      };
      edata.push(udt);
      for (let item of data) {
        let obj: ExcelJson = {
          data: [],
        };
        index = 1;
        let _row: any = {};
        for (let prop in item) {
          _row[this.getAlphabet(index)] = item[prop];
          index++;
        }
        obj.data.push(_row);
        edata.push(obj);
      }
      return edata;
    } else {
      return [];
    }
  }

  public loadScript(url: string, id: string, c: any): void {
    if (!document.getElementById(id)) {
      const script = document.createElement('script');
      script.type = 'text/javascript';
      script.src = url;
      script.id = id;
      if (c) {
        script.addEventListener(
          'load',
          function (e) {
            c(null, e);
          },
          false
        );
      }
      document.head.appendChild(script);
    }
  }

  getButtonLink(list: any[], key: string) {
    for (let item of list) {
      if (item.text === key) {
        return item.url;
      }
    }
    return '/';
  }
  // Preparing data & control visibility of reactive form elements
  // order 50 set default for showAll, apply on below controls only
  markNavControlsVisible(
    controls: any,
    visible: boolean = false,
    order: number = 50
  ) {
    for (const control of controls) {
      if (control.order > order) {
        control.isVisible = visible;
      }
    }
  }

  bindControlData(
    controls: any,
    key: string,
    data: any,
    isVisible: boolean = true,
    isRequired: boolean = true,
    parent_child: boolean = false,
    default_value: string = '',
    default_text: string = '-- Select --'
  ) {
    let control = this.getControl(controls, key);
    if (control !== null) {
      control.options = this.prepareData(
        data,
        parent_child,
        default_value,
        default_text
      );
      control.isVisible = isVisible;
      control.required = isRequired;
    }
  }

  prepareData(
    data: any,
    parent_child: boolean = false,
    default_value: string = '',
    default_text: string = '-- Select --'
  ) {
    let items: any = [];
    items.push({
      key: default_value,
      value: default_text,
    });

    let categorySpaces = '';
    for (let item of data) {
      /*if (parent_child) {
        const dots = item.level.match(/\./g) || [];
        const total_dots = dots.length;
        item.dots = total_dots;
        categorySpaces = '';
        for (let i = 0; i <= total_dots - 1; i++) {
          categorySpaces = categorySpaces + '--> ';
        }
      }*/
      items.push({
        key: item.id,
        value: categorySpaces + item.title,
      });
    }
    return items;
  }

  hideControl(
    controls: any,
    key: string,
    isvisible: boolean = true,
    isrequired: boolean = true
  ) {
    let control = this.getControl(controls, key);
    if (control !== null) {
      control.isVisible = isvisible;
      control.required = isrequired;
    }
  }

  isControlVisible(controls: any, key: string) {
    let control = this.getControl(controls, key);
    if (control !== null && control.isVisible) {
      return true;
    } else {
      return false;
    }
  }

  disableControl(controls: any, key: string, isdisabled: boolean = true) {
    let control = this.getControl(controls, key);
    if (control !== null) {
      control.isdisabled = isdisabled;
    }
  }

  getControl(controls: any, key: string) {
    for (const control of controls) {
      if (control.key === key) {
        return control;
      }
    }
    return null;
  }

  bindParams(Query: any, Params: any) {
    for (let prop in Params) {
      if (this.isNumber(Params[prop])) {
        Query[prop] = parseInt(Params[prop], 10);
      } else {
        Query[prop] = Params[prop];
      }
    }
  }

  isNumber(n: any) {
    return !isNaN(parseFloat(n)) && !isNaN(n - 0);
  }

  setNumericFilter(
    param: any,
    options: any,
    props: any,
    isnumeric: boolean,
    object: string = ''
  ) {
    if (param !== undefined && param !== null) {
      if (!isnumeric) {
        if (object !== '') {
          options[object][props] = param;
        } else {
          options[props] = param;
        }
      } else {
        let value = parseInt(param, 10);
        if (!isNaN(value)) {
          if (object !== '') {
            options[object][props] = value;
          } else {
            options[props] = value;
          }
        }
      }
    }
  }

  getParamValue(param: any, isnumeric: boolean) {
    if (param !== undefined && param !== null) {
      if (!isnumeric) {
        return param;
      } else {
        let value = parseInt(param, 10);
        if (!isNaN(value)) {
          return value;
        }
      }
    } else {
      if (!isnumeric) {
        return '';
      } else {
        return 0;
      }
    }
  }

  getParamValue_v2(param: any, isnumeric: boolean, default_value: any) {
    if (param !== undefined && param !== null) {
      if (!isnumeric) {
        return param;
      } else {
        let value = parseInt(param, 10);
        if (!isNaN(value)) {
          return value;
        }
      }
    } else {
      return default_value;
    }
  }

  validateJSON(json: string) {
    let flag = false;
    if (json === null || json === '') {
      return false;
    }

    if (json.trim().startsWith('{') || json.trim().startsWith('[')) {
      return true;
    } else {
      return false;
    }
  }

  isFeatureAvailable(features: any[], key: string) {
    for (let feature of features) {
      if (feature === key) {
        return true;
      }
    }
    return false;
  }

  generateDashboardNavigation(
    configs: any,
    domain_id: number,
    developerMode: boolean = false
  ) {
    let nav: any = [];

    let feature_set: any[] = [];
    if (configs.features !== undefined && configs.features.list !== undefined) {
      feature_set = configs.features.list;
    }
    // Home
    nav.push({
      index: 0,
      title: 'Dashboard',
      url: '/',
      icon: 'bx bx-home-circle',
      sub_nav: [],
    });

    // Domains

    nav.push({
      index: 1,
      title: 'Workspaces',
      url: '/domains',
      icon: 'bx bx-coin-stack',
      sub_nav: [],
    });

    // Workspace
    nav.push({
      index: 2,
      title: 'Current Workspace',
      url: '/domains/profile/' + domain_id,
      icon: 'bx bx-grid-alt',
      sub_nav: [],
    });

    // Contents
    /*nav.push({
      index: 2,
      title: 'Current Workspace',
      url: '#',
      icon: 'bx bx-grid-alt',
      sub_nav: [
        {
          index: 0,
          title: 'Manage Workspace',
          url: '/domains/profile/' + domain_id,
        },
        {
          index: 1,
          title: 'Manage Layout',
          url: '/settings/layouts/' + domain_id,
        },
      
        {
          index: 2,
          title: 'Website Settings',
          url: '/domains/settings/' + domain_id,
        },

        {
          index: 3,
          title: 'Manage Website Menu',
          url: '/domains/menu/99',
        },

        {
          index: 4,
          title: 'Manage Website Footer',
          url: '/domains/menu/98',
        },

        {
          index: 5,
          title: 'Manage Sitemap',
          url: '/domains/menu/97',
        },
      ],
    });*/

    /*if (this.isFeatureAvailable(feature_set, "_LOCATION_PROFILE_")) {
      nav.push({
        index: 3,
        title: 'Locations',
        url: '/settings/locations',
        icon: 'bx bx-world',
        sub_nav: [],
      });
    }*/

    // Users
    nav.push({
      index: 4,
      title: 'Users',
      url: '/users',
      icon: 'bx bx-user-circle',
      sub_nav: [],
    });

    if (this.isFeatureAvailable(feature_set, '_ADLISTING_PROFILE_')) {
      // Ad Listings
      nav.push({
        index: 5,
        title: 'Ad Listings',
        url: '#',
        icon: 'bx bx-dice-6',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Ads',
            url: '/adlistings',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/adlistings/reports',
          },
        ],
      });
    }

    if (this.isFeatureAvailable(feature_set, '_PRODUCT_PROFILE_')) {
      // Products
      nav.push({
        index: 6,
        title: 'Products',
        url: '#',
        icon: 'bx bx-shopping-bag',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Products',
            url: '/products',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/products/reports',
          },
        ],
      });
    }

    if (this.isFeatureAvailable(feature_set, '_DIRECTORY_PROFILE_')) {
      // Directories
      nav.push({
        index: 7,
        title: 'Directories',
        url: '#',
        icon: 'bx bx-been-here',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Directories',
            url: '/directories',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/directories/reports',
          },
        ],
      });
    }

    if (this.isFeatureAvailable(feature_set, '_JOB_PROFILE_')) {
      // Jobs
      nav.push({
        index: 8,
        title: 'Jobs',
        url: '#',
        icon: 'bx bx-walk',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Jobs',
            url: '/jobs',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/jobs/reports',
          },
        ],
      });
    }
    if (this.isFeatureAvailable(feature_set, '_COMPANY_PROFILE_')) {
      // Companies
      nav.push({
        index: 9,
        title: 'Companies',
        url: '#',
        icon: 'bx bx-buildings',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Companies',
            url: '/companies',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/companies/reports',
          },
        ],
      });
    }

    // Header [Media Management]
    nav.push({
      title: 'Media Management',
      url: '',
      icon: '',
      sub_nav: [],
    });

    // Albums
    if (this.isFeatureAvailable(feature_set, '_ALBUM_PROFILE_')) {
      nav.push({
        index: 10,
        title: 'Albums',
        url: '#',
        icon: 'bx bx bx-music',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Albums',
            url: '/media/albums',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/media/albums/reports',
          },
        ],
      });
    }

    // Media
    if (
      this.isFeatureAvailable(feature_set, '_PHOTO_PROFILE_') ||
      this.isFeatureAvailable(feature_set, '_VIDEO_PROFILE_') ||
      this.isFeatureAvailable(feature_set, '_AUDIO_PROFILE_') ||
      this.isFeatureAvailable(feature_set, '_MOVIE_PROFILE_')
    ) {
      nav.push({
        index: 11,
        title: 'Media',
        url: '#',
        icon: 'bx bx-camera-movie',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Media',
            url: '/media/media',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/media/media/reports',
          },
        ],
      });
    }

    if (this.isFeatureAvailable(feature_set, '_FORUM_PROFILE_')) {
      // Header [Support Management]
      nav.push({
        title: 'Support Management',
        url: '',
        icon: '',
        sub_nav: [],
      });

      // Forums
      nav.push({
        index: 13,
        title: 'Forums',
        url: '/forums',
        icon: 'bx bx bx-support',
        sub_nav: [],
      });

      // Forum Topics
      nav.push({
        index: 14,
        title: 'Topics',
        url: '#',
        icon: 'bx bx bx-columns',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Topics',
            url: '/topics',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/topics/reports',
          },
        ],
      });
    }

    // Header [Main Contents]
    nav.push({
      title: 'Contents & Posts',
      url: '',
      icon: '',
      sub_nav: [],
    });

    if (this.isFeatureAvailable(feature_set, '_QA_PROFILE_')) {
      // Knowledgebase
      nav.push({
        index: 15,
        title: 'Knowledgebase',
        url: '/qa',
        icon: 'bx bx-brain',
        sub_nav: [
          {
            index: 0,
            title: 'Manage QA',
            url: '/qa',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/qa/reports',
          },
        ],
      });
    }
    if (this.isFeatureAvailable(feature_set, '_BLOG_PROFILE_')) {
      // Blogs
      nav.push({
        index: 16,
        title: 'Blogs',
        url: '#',
        icon: 'bx bx-spreadsheet',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Blogs',
            url: '/blogs',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/blogs/reports',
          },
        ],
      });
    }
    if (this.isFeatureAvailable(feature_set, '_POLL_PROFILE_')) {
      // Polls & Quiz
      nav.push({
        index: 17,
        title: 'Polls & Quiz',
        url: '#',
        icon: 'bx bx-poll',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Polls',
            url: '/polls',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/polls/reports',
          },
        ],
      });
    }
    if (this.isFeatureAvailable(feature_set, '_EVENT_PROFILE_')) {
      // Polls & Quiz
      nav.push({
        index: 18,
        title: 'Events',
        url: '/events',
        icon: 'bx bx-calendar-week',
        sub_nav: [
          {
            index: 0,
            title: 'Manage Events',
            url: '/events',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/events/reports',
          },
        ],
      });
    }

    if (this.isFeatureAvailable(feature_set, '_PROFILE_PROFILE_')) {
      // Profiles
      nav.push({
        index: 19,
        title: 'Profiles',
        url: '/profiles',
        icon: 'bx bx-server',
        sub_nav: [],
      });
    }

    if (this.isFeatureAvailable(feature_set, '_PROJECT_PROFILE_')) {
      // Projects
      nav.push({
        index: 20,
        title: 'Projects',
        url: '/domains/projects/' + domain_id,
        icon: 'bx bx-folder-open',
        sub_nav: [],
      });
    }

    if (configs.reports !== undefined) {
      // Header [Reports]

      nav.push({
        title: 'Reports & Stats',
        url: '',
        icon: '',
        sub_nav: [],
      });

      // Orders
      nav.push({
        index: 21,
        title: 'Orders',
        url: '#',
        icon: 'bx bx-line-chart',
        sub_nav: [
          {
            index: 0,
            title: 'Dashboard',
            url: '/orders/dashboard',
          },
          {
            index: 1,
            title: 'Manage Orders',
            url: '/orders',
          },
          {
            index: 2,
            title: 'Report Builder',
            url: '/orders/reports',
          },
        ],
      });

      // Abuse Reports
      nav.push({
        index: 22,
        title: 'Abuse Reports',
        url: '#',
        icon: 'bx bx-line-chart',
        sub_nav: [
          {
            index: 0,
            title: 'Manage',
            url: '/abuse',
          },
          {
            index: 1,
            title: 'Report Builder',
            url: '/abuse/reports',
          },
        ],
      });
    }

    // Header [Other]
    nav.push({
      title: 'Other',
      url: '',
      icon: '',
      sub_nav: [],
    });

    // Settings
    let _navSettings: any = {
      index: 24,
      title: 'Settings',
      url: '#',
      icon: 'bx bx-grid-alt',
      sub_nav: [
        {
          index: 0,
          title: 'Locations',
          url: '/settings/locations',
        },
        {
          index: 1,
          title: 'Domains',
          url: '/domains',
        },
        {
          index: 2,
          title: 'Labels & Tags',
          url: '/settings/tags',
        },

        {
          index: 4,
          title: 'Block IP',
          url: '/settings/blockip',
        },
        {
          index: 5,
          title: 'Dictionary',
          url: '/settings/dictionary',
        },
        {
          index: 6,
          title: 'Language',
          url: '/settings/language',
        },
        {
          index: 7,
          title: 'Log',
          url: '/settings/log',
        },
        {
          index: 8,
          title: 'Mail Templates',
          url: '/settings/mailtemplates',
        },
        {
          index: 9,
          title: 'Roles & Permissions',
          url: '/settings/roles',
        },
        {
          index: 10,
          title: 'Packages',
          url: '/settings/packages',
        },
        {
          index: 11,
          title: 'Cron Jobs',
          url: '/settings/cron',
        },
        {
          index: 12,
          title: 'Report Templates',
          url: '/settings/reporttemplates',
        },
        {
          index: 13,
          title: 'Content Types',
          url: '/settings/contenttypes',
        },
        {
          index: 14,
          title: 'Gamify',
          url: '/settings/gamify',
        },
        {
          index: 23,
          title: 'Partners',
          url: '/partners',
        },
      ],
    };

    nav.push(_navSettings);

    nav.push({
      index: 100,
      title: 'Developer Toolkit',
      url: '/toolkit',
      icon: 'bx bx-book',
      sub_nav: [],
    });

    if (developerMode) {
    }

    return nav;
  }

  processMediaObject(id: number, files: any, mediaobject: any) {
    let _mediaOBJ: any = [];
    if (id === 0) {
      let media: any = [];
      if (files.length > 0) {
        for (const file of files) {
          let _media_object: any = {
            key: file.filename,
            files: [
              {
                isdefault: false,
              },
            ],
          };
          if (file.selected) {
            _media_object.isdefault = true;
          }
          media.push(_media_object);
        }
      }
      _mediaOBJ = media;
    } else {
      if (mediaobject === null) {
        let media: any = [];
        if (files.length > 0) {
          for (const file of files) {
            let _media_object: any = {
              key: file.filename,
              files: [
                {
                  isdefault: false,
                },
              ],
            };
            if (file.selected) {
              _media_object.isdefault = true;
            }
            media.push(_media_object);
          }
        }
        _mediaOBJ = media;
      } else {
        // update enty
        if (files.length > 0) {
          let _media = Object.assign([], mediaobject);
          // add newly added media
          for (const file of files) {
            if (!this.isMediaExist(_media, file.filename)) {
              _media.push({
                key: file.filename,
                files: [
                  {
                    isdefault: false,
                  },
                ],
              });
            }
            /*if (!this.isMediaExist(_media, file.filename)) {
             */
          }
          // remove delete file
          for (const media of _media) {
            if (!this.isMediaRemoved(files, media)) {
              media.deleted = true;
            }
          }
          // set default media
          for (const file of files) {
            this.setDefaultMedia(_media, file);
          }

          _mediaOBJ = _media;
        }
      }
    }
    return _mediaOBJ;
  }

  isMediaRemoved(updates_files: any, media: any) {
    let flag = false;
    for (const item of updates_files) {
      if (item.filename === media.key) {
        flag = true;
      }
    }
    return flag;
  }

  isMediaExist(media: any, file: any) {
    let flag = false;
    for (const item of media) {
      if (item.key == file) {
        flag = true;
      }
    }
    return flag;
  }

  setDefaultMedia(media: any, file: any) {
    for (const item of media) {
      if (item.key == file.filename) {
        item.isdefault = file.selected;
      }
    }
  }

  encrypt(value: any): string {
    const encrypt = crypto.AES.encrypt(value.toString(), this.pharase);
    return this.replaceAll(encrypt.toString(), '/', '**');
  }

  decrypt(value: any): string {
    // const decrypt_text =  value.toString().replace('**', '/');
    if (value !== undefined && value !== '' && value !== '0') {
      const decrypt_text = this.replaceAll(value.toString(), '**', '/');
      const decrypt = crypto.AES.decrypt(decrypt_text, this.pharase);
      return decrypt.toString(crypto.enc.Utf8);
    } else {
      return '0';
    }
  }

  replaceAll(target: any, search: any, replacement: any): string {
    // return target.replace(new RegExp(search, 'g'), replacement);
    return target.split(search).join(replacement);
  }

  updateCategories(
    controls: any,
    categories: any,
    key = 'categories',
    keyvalue: boolean = false
  ) {
    if (controls !== undefined) {
      if (controls.length > 0) {
        for (const control of controls) {
          if (control.key === key) {
            let values: any = [];
            for (const category of categories) {
              if (keyvalue) {
                values.push({
                  key: category.value,
                  value: category.title,
                });
              } else {
                values.push({
                  key: category.id,
                  value: category.title,
                });
              }
            }

            control.options = values;
            setTimeout(() => {
              control.multiselectOptions.dropdownList = values;
            }, 2000);
          }
        }
      }
    }
  }

  bindLocationData(control: any, list: any[]) {
    if (list !== undefined && list.length > 0) {
      for (let post of list) {
        if (post.state !== null) {
          post.title =
            post.title + ' (' + post.state + ' - ' + post.country + ')';
        }
      }
      control.autocompleteOptions.data = list;
      control.autocompleteOptions.isLoading = false;
    }
  }

  updateCategories_v2(
    controls: any,
    categories: any,
    key = 'categories',
    default_obj: any = null
  ) {
    if (controls !== undefined) {
      if (controls.length > 0) {
        for (const control of controls) {
          if (control.key === key) {
            let values: any = [];
            if (default_obj !== null) {
              values.push(default_obj);
            }
            for (const category of categories) {
              values.push({
                key: category.id,
                value: category.title,
              });
            }

            control.options = values;
            /*setTimeout(() => {
              control.multiselectOptions.dropdownList = values
            }, 2000)*/
          }
        }
      }
    }
  }

  // selected list of category items to array of category ids (supported by api) etc. [34, 43]
  returnSelectedCategoryArray(categories: any) {
    let categorylist = [];
    if (categories !== undefined && categories !== null && categories !== '') {
      categorylist = categories;
    }
    /*if (categories !== undefined && categories.length > 0) {
      for (let category of categories) {
        categorylist.push(category.key);
      }
    }*/
    return categorylist;
  }

  /* -------------------------------------------------------------------------- */
  /*                  ng-multiselect-dropdown dropdown settings                 */
  /* -------------------------------------------------------------------------- */
  getMultiCategorySettings(
    singleSelection = false,
    placeholder = 'Select Categories',
    list: any = []
  ) {
    return {
      placeholder: placeholder,
      dropdownList: list,
      dropdownSettings: {
        /*singleSelection: singleSelection,
        idField: 'key',
        textField: 'value',
        selectAllText: 'Select All',
        unSelectAllText: 'UnSelect All',*/
        disabled: false,
        itemsShowLimit: 10,
        allowSearchFilter: true,
      },
    };
  }

  /* -------------------------------------------------------------------------- */
  /*             Return category array with ng-multiselect-dropdown             */
  /* -------------------------------------------------------------------------- */
  prepareSelectedItems(catorylist: any) {
    let selectedItems = [];
    if (
      catorylist !== undefined &&
      catorylist !== null &&
      catorylist.length > 0
    ) {
      for (let item of catorylist) {
        if (item.category !== undefined && item.category !== null) {
          selectedItems.push(item.category.id);
          /*selectedItems.push({
            key: item.category.id,
            value: item.category.title,
          });*/
        } else {
          selectedItems.push(item.id);
          /*selectedItems.push({
            key: item.id,
            value: item.title,
          });*/
        }
      }
    }
    return selectedItems;
  }

  /*prepareSelectedItems_v4(catorylist: any) {
    let selectedItems = [];
    if (catorylist !== undefined && catorylist !== null && catorylist.length > 0) {
      for (let item of catorylist) {
        selectedItems.push({
          key: item.category.id,
          value: item.category.title,
        });
      }
    }
    return selectedItems;
  }*/

  isJsonString(str: string) {
    try {
      JSON.parse(str);
    } catch (e) {
      return false;
    }
    return true;
  }

  /* -------------------------------------------------------------------------- */
  /*             Return select array with ng-multiselect-dropdown [not category] */
  /* -------------------------------------------------------------------------- */
  prepareSelectedItems_v3(catorylist: any) {
    let selectedItems = [];
    if (catorylist !== undefined && catorylist.length > 0) {
      for (let item of catorylist) {
        selectedItems.push({
          key: item.id,
          value: item.title,
        });
      }
    }
    return selectedItems;
  }

  /* -------------------------------------------------------------------------- */
  /*   Return category array with ng-multiselect-dropdown via comma separated list and array         */
  /* -------------------------------------------------------------------------- */
  // selected_list = comma separated list e.g "3,4"
  // array = list array
  prepareSelectedItems_v2(selected_list = '', list: any = []) {
    let selectedItems: any = [];
    if (selected_list !== null && selected_list !== '') {
      let selectedList = selected_list.split(',');
      for (let item of selectedList) {
        for (let _listItem of list) {
          if (_listItem.id === parseInt(item, 10)) {
            selectedItems.push({
              key: _listItem.id,
              value: _listItem.title,
            });
          }
        }
      }
    }

    return selectedItems;
  }

  /* -------------------------------------------------------------------------- */
  /*                          Google Map Limited Settings                         */
  /* -------------------------------------------------------------------------- */
  initGoogleMapOptions() {
    return {
      enableMarkerWindow: false,
      width: '750px',
      height: '400px',
      zoom: 11,
      draggable: true,
      map_key: '',
    };
  }

  /* -------------------------------------------------------------------------- */
  /*                          Quill Limited Settings                         */
  /* -------------------------------------------------------------------------- */
  prepareInitEditorSettings() {
    // Quill Editor Options
    return {
      toolbar: [
        ['bold', 'italic', 'underline', 'strike'],
        ['blockquote', 'code-block'],
        [{ list: 'ordered' }, { list: 'bullet' }],
        [{ header: [1, 2, 3, 4, 5, 6, false] }],
        [{ color: [] }, { background: [] }],
        ['link'],
        ['clean'],
      ],
    };
    /*return {
      base_url: '/tinymce', // Root for resources
      suffix: '.min', // Suffix to use when loading resources
      height: 450,
      plugins: 'lists advlist',
      toolbar: 'undo redo | bold italic | bullist numlist outdent indent',
    };*/
  }

  /* -------------------------------------------------------------------------- */
  /*                          Quill Advance Settings                         */
  /* -------------------------------------------------------------------------- */
  prepareInitAdvacneEditorSettings() {
    // Quill Editor Options
    return {
      toolbar: [
        ['bold', 'italic', 'underline', 'strike'], // toggled buttons
        ['blockquote', 'code-block'],

        [{ header: 1 }, { header: 2 }], // custom button values
        [{ list: 'ordered' }, { list: 'bullet' }],
        [{ script: 'sub' }, { script: 'super' }], // superscript/subscript
        [{ indent: '-1' }, { indent: '+1' }], // outdent/indent
        [{ direction: 'rtl' }], // text direction

        [{ size: ['small', false, 'large', 'huge'] }], // custom dropdown
        [{ header: [1, 2, 3, 4, 5, 6, false] }],

        [{ color: [] }, { background: [] }], // dropdown with defaults from theme
        [{ font: [] }],
        [{ align: [] }],

        ['clean'], // remove formatting button

        ['link', 'image', 'video'], // link and image, video
      ],
    };
    /*return {
      base_url: '/tinymce', // Root for resources
      suffix: '.min', // Suffix to use when loading resources
      plugins:
        'print preview paste importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount imagetools textpattern noneditable help charmap quickbars emoticons',
      menubar: 'file edit view insert format tools table help',
      toolbar:
        'undo redo | bold italic underline strikethrough | fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist | forecolor backcolor removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media template link anchor codesample | ltr rtl',
      toolbar_sticky: true,
      autosave_ask_before_unload: true,
      autosave_interval: '30s',
      autosave_prefix: '{path}{query}-{id}-',
      autosave_restore_when_empty: false,
      autosave_retention: '2m',
      image_advtab: true,
      content_css: [
        '//fonts.googleapis.com/css?family=Lato:300,300i,400,400i',
        '//www.tiny.cloud/css/codepen.min.css',
      ],
      importcss_append: true,
      height: 600,
      template_cdate_format: '[Date Created (CDATE): %m/%d/%Y : %H:%M:%S]',
      template_mdate_format: '[Date Modified (MDATE): %m/%d/%Y : %H:%M:%S]',
      image_caption: true,
      quickbars_selection_toolbar:
        'bold italic | quicklink h2 h3 blockquote quickimage quicktable',
      noneditable_noneditable_class: 'mceNonEditable',
      toolbar_drawer: 'sliding',
    };*/
  }

  prepareNavQuery(query: any, option: any, prop: any) {
    let _value = option[prop];
    if (this.isNumber(_value)) {
      _value = parseInt(_value, 10);
    }

    if (prop.includes('.')) {
      let prop_str = prop.split('.');
      if (query[prop_str[0]] === undefined || query[prop_str[0]] === null) {
        query[prop_str[0]] = {};
      }
      query[prop_str[0]][prop_str[1]] = _value;
    } else {
      query[prop] = _value;
    }
  }

  fixQuery(query: any) {
    // location filter
    query.location_entity = null;

    let _city = '';
    let _state = '';
    let _country = '';
    let _zip = '';
    if (query.city !== undefined && query.city !== null && query.city !== '') {
      _city = query.city;
    }
    if (
      query.state !== undefined &&
      query.state !== null &&
      query.state !== ''
    ) {
      _state = query.state;
    }
    if (
      query.country !== undefined &&
      query.country !== null &&
      query.country !== ''
    ) {
      _country = query.country;
    }
    if (query.zip !== undefined && query.zip !== null && query.zip !== '') {
      _zip = query.zip;
    }

    if (_city !== '' || _state !== '' || _country !== '' || _zip !== '') {
      query.location_entity = {
        city: _city,
        state: _state,
        country: _country,
        zip: _zip,
      };
    }
  }

  fixFormats(option: any) {
    if (
      option.start_date !== undefined &&
      option.start_date !== null &&
      option.start_date !== ''
    ) {
      option.start_date = this.dateString2Date(option.start_date);
    }

    if (
      option.end_date !== undefined &&
      option.end_date !== null &&
      option.end_date !== ''
    ) {
      option.end_date = this.dateString2Date(option.end_date);
    }

    // multi dropdown
    option.categories = [];
    if (
      option.categories_drp !== undefined &&
      option.categories_drp !== null &&
      option.categories_drp.length > 0
    ) {
      for (let _value of option.categories_drp) {
        option.categories.push(_value.key.toString());
      }
    }

    // autocomplete
    option.locationid = 0;
    if (option.location !== undefined && option.location.id !== undefined) {
      option.locationid = option.location.id;
      option.location = null;
    }

    // categoryid
    if (
      option.categoryid === undefined ||
      option.categoryid === null ||
      option.categoryid === ''
    ) {
      option.categoryid = 0;
    }

    // pending approval
    if (option.pending_approval !== undefined && option.pending_approval) {
      option.isapproved = 0;
    } else {
      option.isapproved = 2;
    }

    // disabled contents
    if (option.disabled_content !== undefined && option.disabled_content) {
      option.isenabled = 0;
    } else {
      option.isenabled = 2;
    }

    // inactive users
    if (option.inactive_users !== undefined && option.inactive_users) {
      option.emailConfirmed = 0;
    } else {
      option.emailConfirmed = 2;
    }

    // expired contents
    if (option.expired_content !== undefined && option.expired_content) {
      option.expired_ads = true;
    } else {
      option.expired_ads = null;
    }

    // archive contents
    if (option.archive_content !== undefined && option.archive_content) {
      option.isarchive = 1;
    } else {
      option.isarchive = 0;
    }
  }

  dateString2Date(dateString: string) {
    // accepted format DD-MM-YYYY => ISO YYYY-MM-DD
    const dt = dateString.split(/\-|\s/);
    let format_date = dt[2] + '-' + dt[1] + '-' + dt[0];
    return new Date(format_date);
  }

  initMaskConfigs(mask: string) {
     // mask references
     // (000) 000-0000||+0 (000) 000-0000||+00 (000) 000-0000
     // d0/M0/0000
     // ref: https://jsdaddy.github.io/ngx-mask/#8

     return {
        init: true,
        showMaskTyped: false,
        mask: mask,  
     }
  }

  datepickerConfigs() {
  
    return {
      placeholder: '',
      format: 'DD-MM-YYYY',
      allowMultiSelect: false,
      firstDayOfWeek: 'su',
      monthFormat: 'MMM-YYYY',
      max: undefined,
      min: undefined,
      yearFormat: 'YYYY',
      maxTime: undefined,
      minTime: undefined,
      showTwentyFourHours: false,
      timeSeparator: ':',
      showMultipleYearsNavigation: false,
      multipleYearsNavigateBy: 10,
      unSelectOnClick: true,
      showGoToCurrent: true,
    };

    // ngprime date configurations
    // https://primeng.org/calendar

    /* 
    d - day of month (no leading zero)
    dd - day of month (two digit)
    o - day of the year (no leading zeros)
    oo - day of the year (three digit)
    D - day name short
    DD - day name long
    m - month of year (no leading zero)
    mm - month of year (two digit)
    M - month name short
    MM - month name long
    y - year (two digit)
    yy - year (four digit)
    @ - Unix timestamp (ms since 01/01/1970)
    ! - Windows ticks (100ns since 01/01/0001)
    '...' - literal text
    '' - single quote */

    /*var minDate = new Date();
    minDate.setDate(minDate.getDate() - 5);

    var maxDate = new Date();
    maxDate.setDate(maxDate.getDate() + 5);*/

    /*return {
      view: null, // year, month
      dateFormat: 'dd.mm.yy',
      showIcon: true,
      showOnFocus: true,
      timeOnly: false,
      hourFormat: null, // 12, 24, null
      iconDisplay: 'input', // input
      inputId: 'icondisplay', // icondisplay
      minDate: null,
      maxDate: null,
      readonlyInput: true,
      selectionMode: 'range', // multiple, range
      showButtonBar: false,
    };*/


  }

  /* primeNG OTP Options */
  /* https://primeng.org/inputotp */
  getPrimeNGOTPConfigs() {
    return {
      mask: false,
      integerOnly: false,
      length: 6,
    };
  }

  getCheckboxList(data: any[]) {
    /* {
           label: '',
           group: 'unique_name',
           value: '' 
       } */
    return {
      data: data,
    };
  }

  getPrimNGChipsConfigs() {
    return {
      max: 3,
      placeholder: 'Maximum 3 items',
      separator: null, // ",",
      maxLength: 30, // maximum length of a chip
      allowDuplicate: false,
      showClear: true,
    };
  }

  getPrimNGColorPickerConfigs() {
    return {
      format: 'hex', // rgb, hex, hsb
    };
  }

  getPrimNGDropdownConfigs() {
    return {
      placeholder: 'Select City',
      optionLabel: 'title',
      optionValue: 'id',
      checkmark: true,
      showClear: true,
      filter: true,
      filterBy: 'title',
    };
  }

  getPrimNGMaskConfigs() {
    return {
      placeholder: '21303-0929322-7',
      mask: '99/99/9999', // normal: 99999-9999999-9 phone: (999) 999-9999, serial no: a*-999-a999
      slotChar: 'mm/dd/yyyy',
    };
  }

  renderDropdownControl(
    attr: any,
    id: string,
    placeholder: string,
    value: string,
    isrequired: any,
    order: number
  ) {
    // dropdown
    let options: any = [];
    options.push({ key: '', value: 'Select ' + attr.title });
    if (attr.variable_type === 2) {
      // year dropdown
      let max_year: number = attr.max;
      let min_year: number = attr.min;
      if (attr.max === 0) {
        max_year = new Date().getFullYear();
      }
      if (attr.min === 0) {
        min_year = max_year - 20;
      }
      for (let i = max_year; i >= min_year; i--) {
        options.push({ key: i.toString(), value: i.toString() });
      }
    } else {
      if (attr.options !== undefined) {
        options = attr.options;
      }
    }
    return new Controls.Dropdown({
      key: id,
      label: attr.title,
      required: isrequired,
      value: value,
      options: options,
      order: order,
      helpblock: attr.helpblock,
    });
  }

  renderRichTextControl(
    attr: any,
    id: string,
    placeholder: string,
    value: string,
    isrequired: any,
    order: number,
    pattern: string,
    description: string
  ) {
    // rich text area
    const rich_text_control_obj: any = {
      key: id,
      label: attr.title,
      placeholder: placeholder,
      tinymiceOptions: this.prepareInitAdvacneEditorSettings(),
      value: value,
      required: isrequired,
      order: order,
      pattern: pattern,
      helpblock: description,
    };
    return new Controls.TinyMyceEditor(rich_text_control_obj);
  }

  renderUploaderControl(attr: any, config: any, id: string, order: number) {
    // uploader
    let btn_text: string = 'Upload Files';
    if (attr.btn_text !== undefined && attr.btn_text !== null) {
      btn_text = attr.btn_text;
    }
    let btn_css: string = 'btn btn-primary';
    if (attr.btn_css !== undefined && attr.btn_css !== null) {
      btn_css = attr.btn_css;
    }
    let max_filesize: string = '11mb';
    if (attr.max_filesize !== undefined && attr.max_filesize !== null) {
      max_filesize = attr.max_filesize;
    }
    let extensions: string = 'jpg,jpeg,png';
    if (attr.extensions !== undefined && attr.extensions !== null) {
      extensions = attr.extensions;
    }
    let max_files: number = 10;
    if (attr.max_files !== undefined && attr.max_files !== null) {
      max_files = parseInt(attr.max_files, 10);
    }
    let files: any = [];
    if (attr.value !== undefined && attr.value !== null && attr.value !== '') {
      files = JSON.parse(attr.value);
    }
    let preview: number = 0;
    if (attr.preview !== undefined && attr.preview !== null) {
      preview = 0; // allow additional options like title, description
      /*switch (attr.preview) {
        case 'list':
          preview = 0;
          break;
        case 'gallery':
          preview = 1;
          break;
      }*/
    }

    return new Controls.Uploader({
      key: id,
      label: '',
      value: files,
      required: false,
      helpblock: '',
      uploadoptions: {
        previewoption: preview, // 0: photo preview loader, 1: normal preview
        handlerpath: config.getConfig('host') + 'api/upload/contents',
        pickfilecaption: btn_text,
        uploadfilecaption: 'Start Uploading',
        pickbuttoncss: btn_css,
        maxfilesize: max_filesize,
        unique_names: true,
        chunksize: '8mb',
        headers: {},
        extensiontitle: 'Files',
        extensions: extensions,
        filepath: '',
        username: '',
        removehandler: '',
        maxallowedfiles: max_files,
        showFileName: false, // show filename with media file
        showoriginalSize: true, // show media in original size
        value: files,
        helpblock: 'Upload one or more images (maximum of ' + max_files + ')',
      },
      order: order,
    });
  }

  renderCropperControl(
    attr: any,
    id: string,
    value: string,
    placeholder: string,
    isrequired: any,
    order: number
  ) {
    // cropper
    let width: number = 400;
    if (attr.width !== undefined && attr.width !== null) {
      width = parseInt(attr.width, 10);
    }
    let height: number = 400;
    if (attr.height !== undefined && attr.height !== null) {
      height = parseInt(attr.height, 10);
    }
    let src = '';
    if (attr.value !== undefined && attr.value !== null) {
      src = attr.value;
    }
    // Image Cropper
    const cropperOptions = {
      cropped_picture: src,
      croptype: 1, // general cropped settings
      upload_id: 'cover_upload', //'cover_upload',
      colcss: 'col-lg-4 col-md-6',
      settings: {
        width: width, // settings.thumbnail_width,
        height: height, //settings.thumbnail_height,
      },
      uploadbtntext: 'Upload Image',
      btncss: 'mt-1 mb-1 btn btn-outline-success px-5 radius-30',
    };

    const cropper_control_obj: any = {
      key: id,
      label: attr.title,
      placeholder: placeholder,
      cropperOptions: cropperOptions,
      value: value,
      required: isrequired,
      order: order,
      helpblock:
        'Cropsize: ' +
        cropperOptions.settings.width +
        'x' +
        cropperOptions.settings.height,
    };

    return new Controls.ImageCropper(cropper_control_obj);
  }

  /* -------------------------------------------------------------------------- */
  /*                      dynamic form attribute processing                     */
  /* -------------------------------------------------------------------------- */
  renderDynamicControls_v2(controls: any, sections: any, isedit: boolean) {
    if (sections != null && sections.length > 0) {
      let orderIndex = 100;
      let attrOrderIndex = orderIndex;
      for (let sec of sections) {
        if (sec.showsection === 1) {
          controls.push(
            new Controls.SectionHeader({
              key: 'section_' + orderIndex,
              label: sec.title,
              order: orderIndex,
              css: 'mt-4',
            })
          );
        }
        if (sec.attributes != null && sec.attributes.length > 0) {
          for (let attr of sec.attributes) {
            let isRquired = false;
            if (attr.isrequired === 1) {
              isRquired = true;
            }
            let pattern = '';
            if (attr.variable_type === 1) {
              pattern = '[0-9]+';
            }

            let PlaceHolder = '';
            let ElementValue = '';

            if (isedit && !attr.not_set) {
              ElementValue = attr.value;
            } else {
              PlaceHolder = attr.value;
            }

            let Order = attrOrderIndex;

            // if group by elements by section is disabled, use priority as order index to arrange your elements with your settings
            /*if (sec.showsection === 0) {
              Order = attr.priority;
            }*/

            switch (attr.element_type) {
              case 0:
                // text box
                const control_obj: any = {
                  key: 'attr_' + attr.id,
                  label: attr.title,
                  placeholder: PlaceHolder,
                  value: ElementValue,
                  required: isRquired,
                  order: Order,
                  pattern: pattern,
                  helpblock: attr.helpblock,
                };
                if (attr.min > 0) {
                  control_obj.minLength = attr.min;
                }
                if (attr.max > 0) {
                  control_obj.minLength = attr.max;
                }
                controls.push(new Controls.Textbox(control_obj));
                break;
              case 1:
                // text area
                const text_area_control_obj: any = {
                  key: 'attr_' + attr.id,
                  label: attr.title,
                  placeholder: PlaceHolder,
                  value: ElementValue,
                  required: isRquired,
                  order: Order,
                  pattern: pattern,
                  helpblock: attr.helpblock,
                };
                if (attr.min > 0) {
                  text_area_control_obj.minLength = attr.min;
                }
                if (attr.max > 0) {
                  text_area_control_obj.maxLength = attr.max;
                }
                controls.push(new Controls.TextArea(text_area_control_obj));
                break;
              case 5:
                // rich text area
                const rich_text_control_obj: any = {
                  key: 'attr_' + attr.id,
                  label: attr.title,
                  value: ElementValue,
                  required: isRquired,
                  tinymiceOptions: this.prepareInitEditorSettings(),
                  order: Order,
                  helpblock: attr.helpblock,
                };
                if (attr.min > 0) {
                  rich_text_control_obj.minLength = attr.min;
                }
                if (attr.max > 0) {
                  rich_text_control_obj.maxLength = attr.max;
                }
                controls.push(
                  new Controls.TinyMyceEditor(rich_text_control_obj)
                );
                break;
              case 2:
                // dropdown
                let options = [];
                options.push({ key: '', value: 'Select ' + attr.title });
                if (attr.variable_type === 2) {
                  // year dropdown
                  let max_year: number = attr.max;
                  let min_year: number = attr.min;
                  if (attr.max === 0) {
                    max_year = new Date().getFullYear();
                  }
                  if (attr.min === 0) {
                    min_year = max_year - 20;
                  }

                  for (let i = max_year; i >= min_year; i--) {
                    options.push({ key: i.toString(), value: i.toString() });
                  }
                } else {
                  if (attr.options !== '') {
                    var nameArr = attr.options.split(',');
                    for (let item of nameArr) {
                      options.push({ key: item, value: item });
                    }
                  }
                }
                controls.push(
                  new Controls.Dropdown({
                    key: 'attr_' + attr.id,
                    label: attr.title,
                    required: isRquired,
                    value: ElementValue,
                    options: options,
                    order: Order,
                    helpblock: attr.helpblock,
                  })
                );

                break;

              case 3:
                // checkbox
                let _value = ElementValue == 'true' ? true : false;
                controls.push(
                  new Controls.CheckBox({
                    key: 'attr_' + attr.id,
                    label: attr.title,
                    value: _value,
                    checked: _value,
                    required: isRquired,
                    order: Order,
                    helpblock: attr.helpblock,
                  })
                );
                break;
              case 4:
                // radio button list
                if (attr.options !== '') {
                  var nameArr = attr.options.split(',');
                  let options = [];
                  options.push({ key: '', value: 'Select ' + attr.title });
                  for (let item of nameArr) {
                    options.push({ key: item, value: item });
                  }
                  controls.push(
                    new Controls.RadioButtonList({
                      key: 'attr_' + attr.id,
                      label: attr.title,
                      required: isRquired,
                      value: ElementValue,
                      options: options,
                      order: Order,
                      helpblock: attr.helpblock,
                    })
                  );
                }
                break;
            }

            attrOrderIndex++;
          }
        }

        orderIndex = orderIndex + 100;
        attrOrderIndex = orderIndex;
      }
    }
  }

  /*renderDynamicControls(controls: any, options: any, isedit: boolean) {
    if (options.length > 0) {
      let orderIndex = 100;
      let attrOrderIndex = orderIndex;
      for (let option of options) {
        if (option.showsection === 1 || option.showsection === 2) {
          controls.push(
            new Controls.SectionHeader({
              key: 'section_' + orderIndex,
              label: option.title,
              order: orderIndex,
            })
          );
        }

        // attributes
        for (let attr of option.attributes) {
        
          let isRquired = false;
          if (attr.isrequired === 1) {
            isRquired = true;
          }
          let pattern = '';
          if (attr.variable_type === 1) {
            pattern = '[0-9]+';
          }

          let PlaceHolder = '';
          let ElementValue = '';

          if (isedit && !attr.not_set) {
            ElementValue = attr.value;
          } else {
            PlaceHolder = attr.value;
          }

          let Order = attrOrderIndex;
          // if group by elements by section is disabled, use priority as order index to arrange your elements with your settings
          if (option.showsection === 0) {
            Order = attr.priority;
          }
        
          switch (attr.element_type) {
            case 0:
              // text box
              const control_obj: any = {
                key: 'attr_' + attr.id,
                label: attr.title,
                placeholder: PlaceHolder,
                value: ElementValue,
                required: isRquired,
                order: Order,
                pattern: pattern,
                helpblock: attr.helpblock,
              };
              if (attr.min > 0) {
                control_obj.minLength = attr.min;
              }
              if (attr.max > 0) {
                control_obj.minLength = attr.max;
              }
              controls.push(new Controls.Textbox(control_obj));
              break;
            case 1:
              // text area
              const text_area_control_obj: any = {
                key: 'attr_' + attr.id,
                label: attr.title,
                placeholder: PlaceHolder,
                value: ElementValue,
                required: isRquired,
                order: Order,
                pattern: pattern,
                helpblock: attr.helpblock,
              };
              if (attr.min > 0) {
                text_area_control_obj.minLength = attr.min;
              }
              if (attr.max > 0) {
                text_area_control_obj.maxLength = attr.max;
              }
              controls.push(new Controls.TextArea(text_area_control_obj));
              break;
            case 5:
              // rich text area
              const rich_text_control_obj: any = {
                key: 'attr_' + attr.id,
                label: attr.title,
                value: ElementValue,
                required: isRquired,
                tinymiceOptions: this.prepareInitEditorSettings(),
                order: Order,
                helpblock: attr.helpblock,
              };
              if (attr.min > 0) {
                rich_text_control_obj.minLength = attr.min;
              }
              if (attr.max > 0) {
                rich_text_control_obj.maxLength = attr.max;
              }
              controls.push(new Controls.TinyMyceEditor(rich_text_control_obj));
              break;
            case 2:
              // dropdown
              let options = [];
              options.push({ key: '', value: 'Select ' + attr.title });
              if (attr.variable_type === 2) {
                // year dropdown
                let max_year: number = attr.max;
                let min_year: number = attr.min;
                if (attr.max === 0) {
                  max_year = new Date().getFullYear();
                }
                if (attr.min === 0) {
                  min_year = max_year - 20;
                }

                for (let i = max_year; i >= min_year; i--) {
                  options.push({ key: i.toString(), value: i.toString() });
                }
              } else {
                if (attr.options !== '') {
                  var nameArr = attr.options.split(',');
                  for (let item of nameArr) {
                    options.push({ key: item, value: item });
                  }
                }
              }
              controls.push(
                new Controls.Dropdown({
                  key: 'attr_' + attr.id,
                  label: attr.title,
                  required: isRquired,
                  value: ElementValue,
                  options: options,
                  order: Order,
                  helpblock: attr.helpblock,
                })
              );

              break;

            case 6:
              // timezone
              /*let timezone_options = [];
              timezone_options.push({ key: '', value: 'Select ' + attr.title });
              for (let timezone of timeZones.LIST) {
                timezone_options.push({
                  key: timezone.offset,
                  value: timezone.text,
                });
              }

              controls.push(
                new Controls.Dropdown({
                  key: 'attr_' + attr.id,
                  label: attr.title,
                  required: isRquired,
                  value: ElementValue,
                  options: timezone_options,
                  order: Order,
                  helpblock: attr.helpblock,
                })
              );*/
  /*
              break;
            case 3:
              // checkbox
              let _value = ElementValue == 'true' ? true : false;
              controls.push(
                new Controls.CheckBox({
                  key: 'attr_' + attr.id,
                  label: attr.title,
                  value: _value,
                  checked: _value,
                  required: isRquired,
                  order: Order,
                  helpblock: attr.helpblock,
                })
              );
              break;
            case 4:
              // radio button list
              if (attr.options !== '') {
                var nameArr = attr.options.split(',');
                let options = [];
                options.push({ key: '', value: 'Select ' + attr.title });
                for (let item of nameArr) {
                  options.push({ key: item, value: item });
                }
                controls.push(
                  new Controls.RadioButtonList({
                    key: 'attr_' + attr.id,
                    label: attr.title,
                    required: isRquired,
                    value: ElementValue,
                    options: options,
                    order: Order,
                    helpblock: attr.helpblock,
                  })
                );
              }
              break;
          }

          attrOrderIndex++;
        }
        // close attributes
        orderIndex = orderIndex + 100;
        attrOrderIndex = orderIndex;
      }
    }
  }*/

  groupbySections(attrs: any) {
    let sections: any[] = [];

    if (attrs !== null) {
      for (let attr of attrs) {
        if (attr.section != null) {
          if (!this.isSectionExist(sections, attr.section)) {
            let _section: any = attr.section;
            _section.attributes = this.getSectionAttributes(attrs, _section.id);
            sections.push(_section);
          }
        }
      }
    }

    return sections;
  }

  isSectionExist(sections: any[], section: any) {
    for (let sec of sections) {
      if (sec.title === section.title) {
        return true;
      }
    }
    return false;
  }

  getSectionAttributes(attrs: any[], sectionid: number) {
    let _attributes: any[] = [];
    for (let attr of attrs) {
      if (attr.section !== null) {
        if (attr.section.id === sectionid) {
          _attributes.push(attr);
        }
      }
    }
    return _attributes;
  }

  /* -------------------------------------------------------------------------- */
  /*              map saved data with available dynamic attributes              */
  /* -------------------------------------------------------------------------- */
  prepareDynamicControlData(info: any, attrs: any[], values: any[]) {
    if (info.options === undefined || info.options === null) {
      info.options = [];
    }
    if (info.options.length > 0) {
      for (let option of info.options) {
        for (let attr of attrs) {
          if (this.checkAttributeExist(attr, values)) {
            for (let value of info.attr_values) {
              if (value.attr_id === attr.id) {
                attr.value = value.value;
                if (value.value === null || value.value === '') {
                  attr.not_set = true;
                } else {
                  attr.not_set = false;
                }
              }
            }
          } else {
            attr.not_set = true;
          }
        }
      }
    }
  }

  // Product Options (Dynamic)
  prepareDynamicOptionsControlData(info: any) {
    if (info.prd_options === undefined || info.prd_options === null) {
      info.prd_options = [];
    }
    if (info.prd_options.length > 0) {
      for (let option of info.prd_options) {
        for (let attr of option.attributes) {
          if (this.checkAttributeExist(attr, info.attr_option_values)) {
            for (let value of info.attr_option_values) {
              if (value.attr_id === attr.id) {
                attr.value = value.value;
                if (value.value === null || value.value === '') {
                  attr.not_set = true;
                } else {
                  attr.not_set = false;
                }
              }
            }
          } else {
            attr.not_set = true;
          }
        }
      }
    }
  }

  checkAttributeExist(option: any, values: any) {
    if (values !== null) {
      for (let value of values) {
        if (value.attr_id === option.id) {
          return true;
        }
      }
      return false;
    } else {
      return false;
    }
  }

  /* -------------------------------------------------------------------------- */
  /*                 process submit dynamic data for submission                 */
  /* -------------------------------------------------------------------------- */

  bindDynamicAttrValues(payload: any, info: any) {
    let arr: any[] = []
    for (const prop of Object.keys(payload)) {
      if (prop.includes('attr_')) {
        let id = parseInt(prop.replace('attr_', ''), 10);
        let value = payload[prop];
        if (info.attr_values !== null && info.attr_values.length > 0) {
          for (let opt of info.attr_values) {
            if (opt.attr_id == id) {
              opt.value = value;
            }
          }
        } else if (info.options !== null && info.options.length > 0) {
          for (let opt of info.options) {
            if (opt.id == id) {
              let _opt: any = Object.assign({}, opt)
              _opt.id = 0;
              _opt.attr_id = id;
              _opt.value = value;
              arr.push(_opt)
            }
          }
        }
        
      }
    }
    return arr
  }
  /*processDynamicControlsData(payload: any, info: any) {
    const arr = [];
    for (const prop of Object.keys(payload)) {
      if (prop.includes('attr_')) {
        const id = parseInt(prop.replace('attr_', ''), 10);
        const obj: any = {
          id: 0,
          value: payload[prop],
        };

        if (info.attr_values !== null && info.attr_values.length > 0) {
          for (const option of info.options) {
            for (const attr of option.attributes) {
              if (attr.id === id) {
                obj.attr_id = id;
                obj.title = attr.title;
                obj.priority = attr.priority;
                obj.listing_attr = attr.listing_attr;
              }
            }
          }
          // edit case
          for (const attr of info.attr_values) {
            if (attr.attr_id === id) {
              obj.id = attr.id;
            }
          }
        } else {
          // insert case extend to three sections
          for (const option of info.options) {
            for (const attr of option.attributes) {
              if (attr.id === id) {
                obj.attr_id = id;
                obj.title = attr.title;
                obj.priority = attr.priority;
                obj.listing_attr = attr.listing_attr;
              }
            }
          }
        }
        arr.push(obj);
      }
    }
    return arr;
  }*/

  // Product Options
  processDynamicProductControlsData(payload: any, info: any) {
    const arr = [];
    for (const prop of Object.keys(payload)) {
      if (prop.includes('attr_')) {
        const id = parseInt(prop.replace('attr_', ''), 10);
        const obj: any = {
          id: 0,
          value: payload[prop],
        };

        if (info.attr_option_values.length > 0) {
          for (const option of info.prd_options) {
            for (const attr of option.attributes) {
              if (attr.id === id) {
                obj.attr_id = id;
                obj.title = attr.title;
                obj.priority = attr.priority;
              }
            }
          }
          // edit case
          for (const attr of info.attr_option_values) {
            if (attr.attr_id === id) {
              obj.id = attr.id;
            }
          }
        } else {
          // insert case extend to three sections
          for (const option of info.prd_options) {
            for (const attr of option.attributes) {
              if (attr.id === id) {
                obj.attr_id = id;
                obj.title = attr.title;
                obj.priority = attr.priority;
              }
            }
          }
        }
        arr.push(obj);
      }
    }
    return arr;
  }

  /* -------------------------------------------------------------------------- */
  /*                             generate random id                             */
  /* -------------------------------------------------------------------------- */
  makeid(length: number = 5) {
    let text = '';
    const possible =
      'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

    for (let i = 0; i < length; i++) {
      text += possible.charAt(Math.floor(Math.random() * possible.length));
    }
    return text;
  }

  /* -------------------------------------------------------------------------- */
  /*                             attach encrypted id                            */
  /* -------------------------------------------------------------------------- */
  attachEncryptedId(data: any) {
    if (data.length > 0) {
      for (const item of data) {
        item.enc_id = this.encrypt(item.id);
      }
    }
  }

  checkListSelection(list: any) {
    if (list !== null && list.length > 0) {
      for (let item of list) {
        if (item.selected !== undefined && item.selected) {
          return true;
        }
      }
    }
    return false;
  }

  getSelectedItems(list: any) {
    let arr: any = [];
    if (list !== null && list.length > 0) {
      for (let item of list) {
        if (item.selected !== undefined && item.selected) {
          arr.push(item);
        }
      }
    }
    return arr;
  }

  prepareActionItems(list: any, action: any) {
    let copy_list = JSON.parse(JSON.stringify(list));

    if (copy_list !== null && copy_list.length > 0) {
      for (let item of copy_list) {
        item.actionstatus = action.actionstatus;
      }
      return list;
    }
    return [];
  }

  preparePageSizeOptions() {
    return [
      {
        key: 21,
        value: 21,
      },

      {
        key: 50,
        value: 50,
      },
      {
        key: 100,
        value: 100,
      },
    ];
  }

  is_numeric(data: any) {
    return /^\d+$/.test(data);
  }

  decimalAdjust(value: any, exp: any) {
    // If the exp is undefined or zero...
    if (typeof exp === 'undefined' || +exp === 0) {
      return Math.round(value);
    }
    value = +value;
    exp = +exp;
    // If the value is not a number or the exp is not an integer...
    if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0)) {
      return NaN;
    }
    // Shift
    value = value.toString().split('e');
    value = Math.round(+(value[0] + 'e' + (value[1] ? +value[1] - exp : -exp)));
    // Shift back
    value = value.toString().split('e');
    return +(value[0] + 'e' + (value[1] ? +value[1] + exp : exp));
  }

  random(length: number = 8) {
    // Declare all characters
    let chars =
      'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

    // Pick characers randomly
    let str = '';
    for (let i = 0; i < length; i++) {
      str += chars.charAt(Math.floor(Math.random() * chars.length));
    }

    return str;
  }
}
