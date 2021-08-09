using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TitanBlog.Services.Interfaces;

namespace TitanBlog.Services
{
    public class BasicImageService : IImageService
    {
        public string ContentType(IFormFile image)
        {
            return Path.GetExtension(image?.FileName);
        }

        public string DecodeImage(byte[] data, string type)
        {
            if (data is null || type is null) return null;
            return $"data:{type};base64,{Convert.ToBase64String(data)}";
        }

        public async Task<byte[]> EncodeImageAsync(IFormFile image)
        {
            if (image is null) return null;

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> EncodeImageAsync(string fileName)
        {
            var imagePath = $"{Directory.GetCurrentDirectory()}/wwwroot/images/{fileName}";
            return await File.ReadAllBytesAsync(imagePath);
        }

        public bool IsValidType(IFormFile image)
        {
            var type = ContentType(image);

            var typeList = new List<string>();
            typeList.Add(".png");
            typeList.Add(".jpg");
            typeList.Add(".jpeg");
            typeList.Add(".bmp");
            typeList.Add(".tiff");
            typeList.Add(".gif");

            var isValid = typeList.Contains(type);
            return isValid;
        }

        public int Size(IFormFile image)
        {
            return Convert.ToInt32(image?.Length);
        }

    }
}
