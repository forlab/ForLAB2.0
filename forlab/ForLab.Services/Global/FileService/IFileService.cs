using ForLab.Core.Common;
using System.Collections.Generic;

namespace ForLab.Services.Global.FileService
{
    public interface IFileService<T>
    {
        GeneratedFile ExportToExcel(List<T> arg);
    }
}
