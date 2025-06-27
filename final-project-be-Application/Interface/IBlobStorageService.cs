using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IBlobStorageService
    {
        Task UploadFileAsync(string fileName, Stream fileStream);
        Task DeleteFileIfExistsAsync(string fileName);
    }
}
