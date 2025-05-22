using DevCodeArchitect.DBContext;
using System.Text;

namespace DevCodeArchitect.Utilities;
public class Aws
{
    public static async Task<string> UploadPhoto(ApplicationDBContext context, string files, string filePath, string aws_directory)
    {
        var str = new StringBuilder();

        if (AwsSettings.Enabled 
            && !string.IsNullOrWhiteSpace(AwsSettings.S3.BucketName)
            && !string.IsNullOrWhiteSpace(AppSecrets.Aws.AccessKeyId)
            && !string.IsNullOrWhiteSpace(AppSecrets.Aws.SecretAccessKey))
        {

            var photos = files.Split(char.Parse(","));
            foreach (var photo in photos)
            {
                if (!string.IsNullOrEmpty(photo))
                {
                    if (!string.IsNullOrEmpty(str.ToString()))
                        str.Append(",");
                    if (!photo.StartsWith("http"))
                    {
                        str.Append(await _Process(context, photo, filePath, aws_directory));
                    }
                    else
                    {
                        str.Append(photo);
                    }
                }
            }

        }
        else
        {
            str.Append(files);
        }
        return str.ToString();
    }

    public static async Task<string> UploadPhotoV2(ApplicationDBContext context, string files, string local_files, string filePath, string aws_directory)
    {
        var str = new StringBuilder();

        if (AwsSettings.Enabled
            && !string.IsNullOrWhiteSpace(AwsSettings.S3.BucketName)
            && !string.IsNullOrWhiteSpace(AppSecrets.Aws.AccessKeyId)
            && !string.IsNullOrWhiteSpace(AppSecrets.Aws.SecretAccessKey))
        {

            var photos = files.Split(char.Parse(","));
            foreach (var photo in photos)
            {
                if (!string.IsNullOrEmpty(photo))
                {
                    if (!string.IsNullOrEmpty(str.ToString()))
                        str.Append(",");
                    if (!photo.StartsWith("http"))
                    {
                        str.Append(await _Process(context, photo, filePath, aws_directory));
                    }
                    else
                    {
                        str.Append(photo);
                    }
                }
            }

        }
        else
        {
            str.Append(local_files);
        }
        return str.ToString();
    }
    private static async Task<string> _Process(ApplicationDBContext context, string filename, string path, string aws_directory)
    {
        string Org_Path = path + "" + filename;

        filename = Guid.NewGuid().ToString().Substring(0, 8) + "-" + filename;

        string org_filename = aws_directory + filename;

        // add key to avoid duplications
        string contenttype = "image/jpeg";
        if (org_filename.EndsWith(".gif"))
            contenttype = "image/gif";
        else if (org_filename.EndsWith(".png"))
            contenttype = "image/png";

        string status = "";
        if (!System.IO.File.Exists(Org_Path))
        {
            return filename;
        }

        status = await AmazonUtil.UploadFileToS3Async(context, "", org_filename, AwsSettings.S3.BucketName, Org_Path);
  
        // delete file
        if (File.Exists(Org_Path))
            File.Delete(Org_Path);

        // prepare url
        var _publish_path = "";
        var public_url = AwsSettings.S3.BaseUrl;

        if (!string.IsNullOrEmpty(public_url))
        {
            if (public_url.StartsWith("http"))
                _publish_path = public_url;
            else
                _publish_path = "https://" + public_url;


            if (!_publish_path.EndsWith("/"))
                _publish_path = _publish_path + "/";

            // publish path
            if (!string.IsNullOrEmpty(aws_directory))
                _publish_path = _publish_path + aws_directory;

        }

        if (!string.IsNullOrEmpty(_publish_path))
            filename = _publish_path + "" + filename;


        return filename;
    }
}
/*
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.md', which is part of this source code package.
 * Copyright 2007 - 2024 MediaSoftPro
 * For more information email at support@mediasoftpro.com
 */
