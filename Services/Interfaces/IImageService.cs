﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TitanBlog.Services.Interfaces
{
    public interface IImageService
    {
        Task<byte[]> EncodeImageAsync(IFormFile image);
        Task<byte[]> EncodeImageAsync(string image);
        string DecodeImage(byte[] data, string type);
        string ContentType(IFormFile image);
        int Size(IFormFile image);
    }
}
