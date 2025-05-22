/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

export interface iStyleConfig {
  tabular: boolean;
}

export interface iUploadOptions {
  handlerpath: string;
  pickfilecaption: string;
  uploadfilecaption: string;
  max_file_size: string;
  chunksize: string;
  plupload_root: string;
  headers: any;
  extensiontitle: string;
  extensions: string;
  filepath?: string;
  removehandler?: string;
}
