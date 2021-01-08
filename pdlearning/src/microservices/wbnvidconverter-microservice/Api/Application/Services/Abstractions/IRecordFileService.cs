using System.Collections.Generic;

namespace Microservice.WebinarVideoConverter.Application.Services.Abstractions
{
    public interface IRecordFileService
    {
        /// <summary>
        /// Get list of internal meeting was converted successfully in folder <see cref="RecordingConvertOptions.ConvertedVideoDir"/>.
        /// </summary>
        /// <returns>A list of internal meeting id .</returns>
        List<string> GetConvertedFileNames();
    }
}
