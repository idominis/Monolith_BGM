using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace BGM.SftpUtilities
{
    public class SftpFileHandler
    {
        private SftpClientManager _clientManager;

        public SftpFileHandler(SftpClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public void UploadFile(string localFilePath, string remotePath)
        {
            using (var client = _clientManager.Connect())
            {
                using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                {
                    client.UploadFile(fileStream, remotePath);
                }
                _clientManager.Disconnect(client);
            }
        }

        // Method to download XML files from a directory
        public bool DownloadXmlFilesFromDirectory(string remoteDirectoryPath, string localBaseDirectoryPath)
        {
            bool newFilesDownloaded = false;

            using (var client = _clientManager.Connect())
            {
                // Get the list of directories in the remote directory
                var directories = client.ListDirectory(remoteDirectoryPath).Where(x => x.IsDirectory && x.Name != "." && x.Name != "..");

                foreach (var directory in directories)
                {
                    // Create a local directory with the same name if it doesn't exist
                    string localDirectoryPath = Path.Combine(localBaseDirectoryPath, directory.Name);
                    Directory.CreateDirectory(localDirectoryPath);

                    // Get the list of files in the remote directory
                    var files = client.ListDirectory(Path.Combine(remoteDirectoryPath, directory.Name)).Where(x => !x.IsDirectory);

                    foreach (var file in files)
                    {
                        // Skip the file if it has been marked as processed
                        if (file.Name.EndsWith(".processed"))
                        {
                            continue;
                        }

                        // Check if the file has already been downloaded
                        string localFilePath = Path.Combine(localDirectoryPath, file.Name);
                        if (File.Exists(localFilePath))
                        {
                            continue;
                        }

                        // Download the file
                        using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                        {
                            client.DownloadFile(file.FullName, fileStream);
                        }

                        newFilesDownloaded = true;
          
                        // Rename the remote file to mark it as processed
                        string processedFilePath = file.FullName + ".processed";
                        client.RenameFile(file.FullName, processedFilePath);
                    }
                }

                _clientManager.Disconnect(client);
            }

            return newFilesDownloaded;
        }

    }

}
