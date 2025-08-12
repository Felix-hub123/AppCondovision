using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Helper
{
    public interface IBlobHelper
    {
        Task<Guid> UploadBlobAsync(IFormFile file, string folderName);
    }
}
