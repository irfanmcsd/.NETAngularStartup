﻿using System;
using System.IO;
using Microsoft.Net.Http.Headers;

namespace DevCodeArchitect.Utilities;

public static class MultipartRequestHelper
{
    public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
    {
        var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
        if (string.IsNullOrWhiteSpace(boundary.ToString()))
        {
            throw new InvalidDataException("Missing content-type boundary.");
        }

        if (boundary.Length > lengthLimit)
        {
            throw new InvalidDataException(
                $"Multipart boundary length limit {lengthLimit} exceeded.");
        }

        return boundary.ToString();
    }

    public static bool IsMultipartContentType(string contentType)
    {
        return !string.IsNullOrEmpty(contentType)
               && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue? contentDisposition)
    {
        // Content-Disposition: form-data; name="key";
        return contentDisposition != null
               && contentDisposition.DispositionType.Equals("form-data")
               && string.IsNullOrEmpty(contentDisposition.FileName.ToString())
               && string.IsNullOrEmpty(contentDisposition.FileNameStar.ToString());
    }

    public static bool HasFileContentDisposition(ContentDispositionHeaderValue? contentDisposition)
    {
        // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
        return contentDisposition != null
               && contentDisposition.DispositionType.Equals("form-data")
               && (!string.IsNullOrEmpty(contentDisposition.FileName.ToString())
                   || !string.IsNullOrEmpty(contentDisposition.FileNameStar.ToString()));
    }
}


/*
* This file is subject to the terms and conditions defined in
* file 'LICENSE.md', which is part of this source code package.
* Copyright 2007 - 2024 MediaSoftPro
* For more information email at support@mediasoftpro.com
*/

