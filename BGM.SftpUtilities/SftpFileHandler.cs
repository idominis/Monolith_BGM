using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using BGM.Common;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using Serilog;

namespace BGM.SftpUtilities
{
    public class SftpFileHandler
    {
        private SftpClientManager _clientManager;
        private readonly IStatusUpdateService _statusUpdateService;

        public SftpFileHandler(SftpClientManager clientManager, IStatusUpdateService statusUpdateService)
        {
            _clientManager = clientManager;
            _statusUpdateService = statusUpdateService;
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
            try
            {
                using (var client = _clientManager.Connect())
                {
                    // Log successful connection
                    _statusUpdateService.RaiseStatusUpdated("Connected to SFTP server successfully");  // TODO
                    Log.Information("Connected to SFTP server successfully.");

                    var directories = client.ListDirectory(remoteDirectoryPath).Where(x => x.IsDirectory && x.Name != "." && x.Name != "..");
                    foreach (var directory in directories)
                    {
                        string localDirectoryPath = Path.Combine(localBaseDirectoryPath, directory.Name);
                        Directory.CreateDirectory(localDirectoryPath); // Safe to call even if directory exists

                        var files = client.ListDirectory(Path.Combine(remoteDirectoryPath, directory.Name)).Where(x => !x.IsDirectory);
                        foreach (var file in files)
                        {
                            string localFilePath = Path.Combine(localDirectoryPath, file.Name);
                            if (File.Exists(localFilePath) || file.Name.EndsWith(".processed"))
                                continue;

                            using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                            {
                                client.DownloadFile(file.FullName, fileStream);
                            }
                            newFilesDownloaded = true;

                            // Rename file on server to mark as processed
                            string processedFilePath = file.FullName + ".processed";
                            client.RenameFile(file.FullName, processedFilePath);
                            Log.Information("File {FileName} downloaded and marked as processed.", file.Name);
                            _statusUpdateService.RaiseStatusUpdated("Files downloaded and marked as processed"); // TODO
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to download XML files from directory: {DirectoryPath}", remoteDirectoryPath);
                throw; // Consider rethrowing to handle these exceptions further up the call stack
            }

            return newFilesDownloaded;
        }
    }
}
