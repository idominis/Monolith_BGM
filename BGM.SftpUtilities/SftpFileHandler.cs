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

        public async Task UploadFileAsync(string localFilePath, string remotePath)
        {
            try
            {
                using (var client = _clientManager.Connect())
                {
                    // Extract the remote directory from the path
                    string remoteDirectory = Path.GetDirectoryName(remotePath);

                    // Check if the directory exists and create it if it doesn't
                    if (!client.Exists(remoteDirectory))
                    {
                        client.CreateDirectory(remoteDirectory); // Ensure this method is available or implement it
                    }

                    using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await Task.Run(() => client.UploadFile(fileStream, remotePath));
                    }
                    _clientManager.Disconnect(client);
                }
                Log.Information("File uploaded successfully: {LocalFilePath}", localFilePath);
                _statusUpdateService.RaiseStatusUpdated("File uploaded successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to upload file: {LocalFilePath}", localFilePath);
                throw;
            }
        }

        public async Task<bool> DownloadXmlFilesFromDirectoryAsync(string remoteDirectoryPath, string localBaseDirectoryPath)
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
                            newFilesDownloaded |= await DownloadXmlFilesFromDirectoryAsync(subDirectoryPath, localSubDirectoryPath);
                        }
                        if (!entry.IsDirectory && !entry.Name.EndsWith(".processed"))
                        {
                            SftpFile file = entry as SftpFile;
                            if (file != null)
                            {
                                string localFilePath = Path.Combine(localBaseDirectoryPath, file.Name);

                                if (File.Exists(localFilePath))
                                {
                                    Log.Information($"Skipping download, file already exists: {localFilePath}");
                                }
                                using (var fileStream = File.OpenWrite(localFilePath)) // Ensures the stream is closed and disposed
                                {
                                    client.DownloadFile(remoteDirectoryPath + "/" + file.Name, fileStream);
                                }

                                newFilesDownloaded |= ProcessFilesInDirectory(client, new[] { file }, localBaseDirectoryPath, remoteDirectoryPath);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to download XML files from directory: {DirectoryPath}", remoteDirectoryPath);
                throw;
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
