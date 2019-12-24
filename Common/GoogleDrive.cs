using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Upload;
using Google = Google.Apis.Drive.v3.Data;
using SlackDataModels = Common.Slack.DataModels;

namespace Common
{
    public class GoogleDrive
    {
        private DriveService _google;
        private UserCredential _googleCredential;
        private readonly IAmazonS3 _s3Client;
        private readonly Logger _logger;
        private static string[] _scopes = { DriveService.Scope.DriveFile };
        private static string _applicationName = "Slack Backup";
        public GoogleDrive()
        {
            _logger = new Logger();
            _s3Client = new AmazonS3Client(RegionEndpoint.USWest2);
            _google = AuthorizeGoogleCredentials().Result;
        }
        //For Testing Only
        public GoogleDrive(BasicAWSCredentials credentials)
        {
            _logger = new Logger();
            _s3Client = new AmazonS3Client(credentials, RegionEndpoint.USWest2);
            _google = AuthorizeGoogleCredentials().Result;
        }

        /// <summary>
        /// A Function that authorizes the saved google credentials
        /// </summary>
        /// <returns>DriveService</returns>  

        private async Task<DriveService> AuthorizeGoogleCredentials()
        {
            string directory = Environment.GetEnvironmentVariable("GOOGLEDRIVETOKENDIRECTORY");
            string fileName = Environment.GetEnvironmentVariable("GOOGLEDRIVETOKENRESPONSEFILE");
            string s3CredentialsKey = Environment.GetEnvironmentVariable("GOOGLEDRIVETOKENS3CREDENTIALKEY");
            string s3TokenResponseKey = Environment.GetEnvironmentVariable("GOOGLEDRIVETOKENS3TOKENRESPONSEKEY");

            var request = new GetObjectRequest
            {
                BucketName = System.Environment.GetEnvironmentVariable("S3BUCKET"),
                Key = s3TokenResponseKey
            };

            //Expecting this to catch on the first time
            try
            {
                using (var response = await _s3Client.GetObjectAsync(request))
                {
                    using ( var stream = response.ResponseStream)
                    {
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }  
                        using ( FileStream file = new FileStream($"{directory}/{fileName}", FileMode.Create))
                        {
                            stream.CopyTo(file);
                        }
                    }
                };
            }
            catch(Exception e)
            {
                _logger.WriteLine("google-authorization", Severity.Info, DateTime.Now, e.Message);
                _logger.WriteLine("google-authorization", Severity.Info, DateTime.Now, "Getting authorization token");
            }

            //Updates the key for the creds file
            request.Key = s3CredentialsKey;

            using (var response = await _s3Client.GetObjectAsync(request))
            {
                using ( var stream = response.ResponseStream)
                {
                    string credPath = "/tmp/token.json";
                    _googleCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        _scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true));
                    Console.WriteLine("Credential file saved to: " + credPath);
                }
            };

            using ( FileStream file = new FileStream($"{directory}/{fileName}", FileMode.Open, FileAccess.Read))
            {
                var putObjectRequest = new PutObjectRequest
				{
					BucketName = System.Environment.GetEnvironmentVariable("S3BUCKET"),
					Key = s3TokenResponseKey,
					InputStream = file,
					AutoCloseStream = true
				};

                await _s3Client.PutObjectAsync(putObjectRequest);
            }

            // Create Drive API service.
            return new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = _googleCredential,
                        ApplicationName = _applicationName,
                    });

        }

        /// <summary>
        /// A Function that takes a name, and optional parents, and creates a folder.
        /// </summary>
        /// <param name="name">Folder name</param>
        /// <param name="parents">Optional list of folder parents</param>
        /// <returns>Folder Id</returns>  

        public string CreateFolder(string name, List<string> parents = null)
        {
            //See if Folder already exists
            var list = _google.Files.List();

            list.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{name}'";
            Google::FileList files = list.Execute();

            if(files.Files.Count == 0) //Folder doesn't exist
            {
                _logger.WriteLine("folder-creation", Severity.Info, DateTime.Now, $"Creating Folder Name: {name}");

                Google::File fileMetaData = new Google::File(){
                    Name = name,
                    Parents = parents,
                    MimeType = "application/vnd.google-apps.folder"
                };

                Google::File file = _google.Files.Create(fileMetaData)
                                .Execute();
                
                return file.Id;
            }
            else //Folder already exists
            {
                string id = files.Files[0].Id;
                _logger.WriteLine("folder-creation", Severity.Info, DateTime.Now, $"Folder Name: {name} already exists with Id: {id}");
                return id;
            }
        }

        /// <summary>
        /// A Function that takes a stream and uploads the file to Google Drive
        /// </summary>
        /// <param name="file">SlackDataModels file</param>
        /// <param name="stream"> File Stream</param>
        /// <param name="parents">Optional list of file parents</param>

        public void UploadFile(SlackDataModels :: File file, Stream stream, List<string> parents = null)
        {
            //See if files  already exists
            var list = _google.Files.List();

            list.Q = $"name = '{file.id}'";
            Google::FileList files = list.Execute();

            if(files.Files.Count > 0)
            {
                _logger.WriteLine("file-transfer", Severity.Info, DateTime.Now, $"File: {file.id} already exists, not transferring to Google Drive");
            }
            else{
                try
                {
                    _logger.WriteLine("file-transfer", Severity.Info, DateTime.Now, $"Transfering File: {file.id} to Google Drive");
                    Google::File fileMetaData = new Google::File(){
                        Name = file.id,
                        Parents = parents,
                        Description = file.name
                    };
                    FilesResource.CreateMediaUpload request;

                    request = _google.Files.Create(fileMetaData, stream, file.mimetype);
                    request.Fields = "id";
                    IUploadProgress progress = request.Upload();

                    if(progress.Status == UploadStatus.Completed)
                    {
                        _logger.WriteLine("file-transfer", Severity.Info, DateTime.Now, $"Transfer for File: {file.id} complete to Google Drive");
                    }
                    else
                    {
                        _logger.WriteLine("file-transfer", Severity.Error, DateTime.Now, $"Transfer for File: {file.id} had an error to Google Drive: {progress.Exception.Message}");
                    }
                }
                finally
                {
                    stream.Dispose();
                }
            }
        }

    }
}