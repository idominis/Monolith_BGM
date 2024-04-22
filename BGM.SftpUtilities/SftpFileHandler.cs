using System;
using System.Collections.Generic;
using System.IO;
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

        // Method to download XML files from a directory only if they don't already exist locally
        public void DownloadXmlFilesFromDirectory(string remoteDirectoryPath, string localDirectoryPath)
        {
            using (var client = _clientManager.Connect())
            {
                // Ensure the local directory exists
                if (!Directory.Exists(localDirectoryPath))
                {
                    Directory.CreateDirectory(localDirectoryPath);
                }

                try
                {
                    // List all files and folders in the directory
                    var filesAndFolders = client.ListDirectory(remoteDirectoryPath);
                    foreach (var file in filesAndFolders)
                    {
                        if (!file.IsDirectory)
                        {
                            if (file.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                            {
                                string localFilePath = Path.Combine(localDirectoryPath, file.Name);

                                // Check if the file already exists locally
                                if (!File.Exists(localFilePath))
                                {
                                    using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                                    {
                                        client.DownloadFile(file.FullName, fileStream);
                                    }
                                }
                            }
                        }
                        else if (file.Name != "." && file.Name != "..")
                        {
                            // Recursive call to process subdirectories
                            DownloadXmlFilesFromDirectory(file.FullName, Path.Combine(localDirectoryPath, file.Name));
                        }
                    }
                }
                catch (SshException ex)
                {
                    Console.WriteLine($"SSH error: {ex.Message}");
                    // Optional: Implement retry logic here
                }
                finally
                {
                    _clientManager.Disconnect(client);  // Ensure the client is always disconnected
                }
            }
        }
    }
}
