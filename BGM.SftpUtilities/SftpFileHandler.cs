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

        public string GetLatestRemoteDirectory(string remoteBaseDirectoryPath)
        {
            using (var client = _clientManager.Connect())
            {
                // Get the list of directories
                var directories = client.ListDirectory(remoteBaseDirectoryPath);

                // Parse the directory names as dates and find the latest
                DateTime latestDate = DateTime.MinValue;
                string latestDirectory = null;
                foreach (var directory in directories)
                {
                    if (DateTime.TryParse(directory.Name, out DateTime date) && date > latestDate)
                    {
                        latestDate = date;
                        latestDirectory = directory.Name;
                    }
                }

                return Path.Combine(remoteBaseDirectoryPath, latestDirectory);
            }
        }

        public string GetLatestLocalDirectory(string localBaseDirectoryPath)
        {
            // Get the list of directories
            var directories = Directory.GetDirectories(localBaseDirectoryPath);

            // Parse the directory names as dates and find the latest
            DateTime latestDate = DateTime.MinValue;
            string latestDirectory = null;
            foreach (var directory in directories)
            {
                string directoryName = Path.GetFileName(directory);
                if (DateTime.TryParse(directoryName, out DateTime date) && date > latestDate)
                {
                    latestDate = date;
                    latestDirectory = directoryName;
                }
            }

            return Path.Combine(localBaseDirectoryPath, latestDirectory);
        }

        public bool AreAllFilesDownloaded(string remoteDirectoryPath, string localDirectoryPath)
        {
            using (var client = _clientManager.Connect())
            {
                // Get the list of files in the remote directory
                var remoteFiles = client.ListDirectory(remoteDirectoryPath);

                // Check if each file exists in the local directory
                foreach (var remoteFile in remoteFiles)
                {
                    string localFilePath = Path.Combine(localDirectoryPath, remoteFile.Name);
                    if (!File.Exists(localFilePath))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
