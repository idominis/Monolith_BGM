using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monolith_BGM.Src
{
    public class FileManager
    {
        private string _baseDirectoryPath;
        private string _remoteDetailsDirectoryPath;
        private string _remoteHeadersDirectoryPath;
        private string _baseDirectoryXmlCreatedPath;

        public FileManager()
        {
            // Get the path to the user's Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Combine the Documents path with your additional folder structure
            _baseDirectoryPath = Path.Combine(documentsPath, "BGM_project", "local", "data_received");
            _baseDirectoryXmlCreatedPath = Path.Combine(documentsPath, "BGM_project", "local", "XML_created");
            _remoteDetailsDirectoryPath = @"\purchasingOrdersDetails"; // RebexTinySftpServer.exe
            _remoteHeadersDirectoryPath = @"\purchasingOrdersHeaders"; // RebexTinySftpServer.exe
            // Ensure the directory exists
            Directory.CreateDirectory(_baseDirectoryPath);
        }

        public string GetBaseDirectoryPath()
        {
            return _baseDirectoryPath;
        }

        public string GetBaseDirectoryXmlCreatedPath()
        {
            return _baseDirectoryXmlCreatedPath;
        }

        public string GetRemoteDetailsDirectoryPath()
        {
            return _remoteDetailsDirectoryPath;
        }

        public string GetRemoteHeadersDirectoryPath()
        {
            return _remoteHeadersDirectoryPath;
        }

        // If you need to handle paths for specific types of files or directories:
        public string GetSpecificPath(string subDirectory)
        {
            var fullPath = Path.Combine(_baseDirectoryPath, subDirectory);
            Directory.CreateDirectory(fullPath); // Ensure this subdirectory exists
            return fullPath;
        }
    }
}
