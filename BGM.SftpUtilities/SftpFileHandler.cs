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

        public bool DownloadXmlFilesFromDirectory(string remoteDirectoryPath, string localBaseDirectoryPath)
        {
            bool newFilesDownloaded = false;
            try
            {
                using (var client = _clientManager.Connect())
                {
                    var entries = client.ListDirectory(remoteDirectoryPath);

                    foreach (var entry in entries)
                    {
                        if (entry.IsDirectory && entry.Name != "." && entry.Name != "..")
                        {
                            // Recursively handle subdirectories
                            string subDirectoryPath = Path.Combine(remoteDirectoryPath, entry.Name);
                            string localSubDirectoryPath = Path.Combine(localBaseDirectoryPath, entry.Name);
                            Directory.CreateDirectory(localSubDirectoryPath); // Ensure the directory exists locally
                            newFilesDownloaded |= DownloadXmlFilesFromDirectory(subDirectoryPath, localSubDirectoryPath);
                        }
                        else if (!entry.IsDirectory && !entry.Name.EndsWith(".processed"))
                        {
                            // Process files directly in the current directory
                            SftpFile file = entry as SftpFile; // Explicitly cast to SftpFile
                            if (file != null)
                            {
                                newFilesDownloaded |= ProcessFilesInDirectory(client, new[] { file }, localBaseDirectoryPath, remoteDirectoryPath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to download XML files from directory: {DirectoryPath}", remoteDirectoryPath);
                throw; // Optionally rethrow to handle these exceptions further up the call stack
            }

            return newFilesDownloaded;
        }


        private bool ProcessFilesInDirectory(SftpClient client, IEnumerable<SftpFile> files, string localDirectoryPath, string remoteDirectoryPath)
        {
            bool downloaded = false;
            foreach (var file in files)
            {
                string localFilePath = Path.Combine(localDirectoryPath, file.Name);
                if (!File.Exists(localFilePath))
                {
                    using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                    {
                        client.DownloadFile(file.FullName, fileStream);
                    }
                    downloaded = true;

                    // Rename the file on server to mark as processed
                    string processedFilePath = file.FullName + ".processed";
                    client.RenameFile(file.FullName, processedFilePath);
                    Log.Information("File {FileName} downloaded and marked as processed.", file.Name);
                }
            }
            return downloaded;
        }






    }
}
