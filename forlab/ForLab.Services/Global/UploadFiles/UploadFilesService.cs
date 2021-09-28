using ForLab.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForLab.Services.Global.UploadFiles
{
    public class UploadFilesService: IUploadFilesService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IResponseDTO _response;
        public UploadFilesService(IHostingEnvironment hostingEnvironment, IResponseDTO responseDTO)
        {
            _hostingEnvironment = hostingEnvironment;
            _response = responseDTO;
        }
        public async Task<IResponseDTO> UploadFile(string path, IFormFile file, bool deleteOldFiles = false)
        {
            if (file != null)
            {
                try
                {
                    if (!Directory.Exists($"{_hostingEnvironment.WebRootPath}\\{path}"))
                    {
                        Directory.CreateDirectory($"{_hostingEnvironment.WebRootPath}\\{path}");
                    }
                    else
                    {
                        if (deleteOldFiles)
                        {
                            Array.ForEach(Directory.GetFiles($"{_hostingEnvironment.WebRootPath}\\{path}"),
                                    delegate (string filePath) { File.Delete(filePath); });
                        }
                    }

                    using (FileStream filestream = File.Create($"{_hostingEnvironment.WebRootPath}\\{path}\\{file.FileName}"))
                    {
                        await file.CopyToAsync(filestream);
                        await filestream.FlushAsync();

                        var newFullPath = $"\\{path}\\{file.FileName}";

                        _response.Message = "Done";
                        _response.IsPassed = true;
                        _response.Data = newFullPath;

                    }
                }
                catch (Exception ex)
                {
                    _response.Message = ex.Message;
                    _response.Data = null;
                    _response.IsPassed = false;
                }
            }
            else
            {
                _response.Data = null;
                _response.Message = "Un_Successfull";
                _response.IsPassed = false;
            }

            return _response;

        }

        public async Task<IResponseDTO> UploadFiles(string path, List<IFormFile> files, bool deleteOldFiles = false)
        {
            if (files != null && files.Count() > 0)
            {
                try
                {
                    if (!Directory.Exists($"{_hostingEnvironment.WebRootPath}\\{path}"))
                    {
                        Directory.CreateDirectory($"{_hostingEnvironment.WebRootPath}\\{path}");
                    }
                    else
                    {
                        if (deleteOldFiles)
                        {
                            Array.ForEach(Directory.GetFiles($"{_hostingEnvironment.WebRootPath}\\{path}"),
                                    delegate (string filePath) { File.Delete(filePath); });
                        }

                    }
                    List<string> newFullPaths = new List<string>();

                    foreach (var file in files)
                    {

                        using (FileStream filestream = File.Create($"{_hostingEnvironment.WebRootPath}\\{path}\\{file.FileName}"))
                        {
                            await file.CopyToAsync(filestream);
                            await filestream.FlushAsync();

                            var newFullPath = $"\\{path}\\{file.FileName}";

                            newFullPaths.Add(newFullPath);
                        }
                    }

                    _response.Message = "Done";
                    _response.IsPassed = true;
                    _response.Data = newFullPaths;
                }
                catch (Exception ex)
                {
                    _response.Message = ex.Message;
                    _response.Data = null;
                    _response.IsPassed = false;
                }
            }
            else
            {
                _response.Data = new List<string>();
                _response.Message = "Empty array";
                _response.IsPassed = true;
            }

            return _response;
        }

        public string CopyFile(string oldAttachmentUrl, string newAttachmentUrl)
        {
            try
            {
                string fileName = Path.GetFileName(oldAttachmentUrl);

                if (!Directory.Exists($"{ _hostingEnvironment.WebRootPath}\\{newAttachmentUrl}"))
                {
                    Directory.CreateDirectory($"{ _hostingEnvironment.WebRootPath}\\{newAttachmentUrl}");
                }
                string sourceFile = $"{ _hostingEnvironment.WebRootPath}\\{oldAttachmentUrl}";
                string destFile = $"{ _hostingEnvironment.WebRootPath}\\{newAttachmentUrl}\\{fileName}";
                if (!File.Exists(destFile))
                {
                    File.Copy(sourceFile, destFile);
                    return $"\\{newAttachmentUrl}\\{fileName}";
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.Data = null;
                _response.IsPassed = false;
            }

            return null;
        }
    }
}
