using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monolith_BGM.Src
{
    // --------------------------------------------------------------------------------
    /// <summary>
    /// FileManager
    /// </summary>
    // --------------------------------------------------------------------------------
    public class FileManager
    {
        /// <summary>
        /// The base directory path
        /// </summary>
        private string _baseDirectoryPath;
        /// <summary>
        /// The remote details directory path
        /// </summary>
        private string _remoteDetailsDirectoryPath;
        /// <summary>
        /// The remote headers directory path
        /// </summary>
        private string _remoteHeadersDirectoryPath;
        /// <summary>
        /// The base directory XML created path
        /// </summary>
        private string _baseDirectoryXmlCreatedPath;

        // ********************************************************************************
        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <returns></returns>
        // <created>,5/26/2024</created>
        // <changed>,5/26/2024</changed>
        // ********************************************************************************
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

        /// <summary>
        /// Gets the base directory path.
        /// </summary>
        /// <returns></returns>
        public string GetBaseDirectoryPath()
        {
            return _baseDirectoryPath;
        }

        /// <summary>
        /// Gets the base directory XML created path.
        /// </summary>
        /// <returns></returns>
        public string GetBaseDirectoryXmlCreatedPath()
        {
            return _baseDirectoryXmlCreatedPath;
        }

        /// <summary>
        /// Gets the remote details directory path.
        /// </summary>
        /// <returns></returns>
        public string GetRemoteDetailsDirectoryPath()
        {
            return _remoteDetailsDirectoryPath;
        }

        /// <summary>
        /// Gets the remote headers directory path.
        /// </summary>
        /// <returns></returns>
        public string GetRemoteHeadersDirectoryPath()
        {
            return _remoteHeadersDirectoryPath;
        }

        // If you need to handle paths for specific types of files or directories:
        /// <summary>
        /// Gets the specific path.
        /// </summary>
        /// <param name="subDirectory">The sub directory.</param>
        /// <returns></returns>
        public string GetSpecificPath(string subDirectory)
        {
            var fullPath = Path.Combine(_baseDirectoryPath, subDirectory);
            Directory.CreateDirectory(fullPath); // Ensure this subdirectory exists
            return fullPath;
        }
    }
}
