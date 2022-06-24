using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using Apiraiser.Services;
using Apiraiser.Models;
using Apiraiser.Enums;
using Apiraiser.Constants;
using Apiraiser.Helpers;
using Apiraiser.Filters;
using Amazon.S3.Model;
using System.IO;
using System.Net;
using Amazon.S3;
using Amazon;
using Microsoft.AspNetCore.Http;
using Amazon.S3.Transfer;

namespace Apiraiser.Plugins
{
    public class S3Plugin : BasePlugin
    {

        public S3Plugin()
        {

        }

        public async static Task<List<S3Bucket>> ListBuckets(string accessKeyId, string secretAccessKey, string region)
        {
            return await ListBuckets(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(region));
        }

        public async static Task<List<S3Bucket>> ListBuckets(string accessKeyId, string secretAccessKey, RegionEndpoint region)
        {
            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);
                ListBucketsResponse buckets = await client.ListBucketsAsync();
                return buckets.Buckets;
            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }
            return null;
        }

        public async static Task<byte[]> DownloadFileAsync(string accessKeyId, string secretAccessKey, string region, string bucket, string key)
        {
            return await DownloadFileAsync(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(region), bucket, key);
        }

        public async static Task<byte[]> DownloadFileAsync(string accessKeyId, string secretAccessKey, RegionEndpoint region, string bucket, string key)
        {

            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);
                MemoryStream ms = null;
                try
                {
                    GetObjectRequest getObjectRequest = new GetObjectRequest
                    {
                        BucketName = bucket,
                        Key = key
                    };

                    using (var response = await client.GetObjectAsync(getObjectRequest))
                    {
                        if (response.HttpStatusCode == HttpStatusCode.OK)
                        {
                            using (ms = new MemoryStream())
                            {
                                await response.ResponseStream.CopyToAsync(ms);
                            }
                        }
                    }

                    if (ms is null || ms.ToArray().Length < 1)
                        throw new FileNotFoundException(string.Format("The document '{0}' is not found", key));

                    return ms.ToArray();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }

            return null;
        }

        public async static Task<bool> UploadFileAsync(string accessKeyId, string secretAccessKey, string region, string bucket, string key, IFormFile file)
        {
            return await UploadFileAsync(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(region), bucket, key, file);
        }

        public async static Task<bool> UploadFileAsync(string accessKeyId, string secretAccessKey, RegionEndpoint region, string bucket, string key, IFormFile file)
        {
            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);

                using (var newMemoryStream = new MemoryStream())
                {
                    file.CopyTo(newMemoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = key,
                        BucketName = bucket,
                        ContentType = file.ContentType
                    };

                    var fileTransferUtility = new TransferUtility(client);

                    await fileTransferUtility.UploadAsync(uploadRequest);

                    return true;
                }
            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }

            return false;
        }

        public async static Task<bool> UploadFileAsync(string accessKeyId, string secretAccessKey, string region, string bucket, string key, Stream stream, string contentType)
        {
            return await UploadFileAsync(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(region), bucket, key, stream, contentType);
        }

        public async static Task<bool> UploadFileAsync(string accessKeyId, string secretAccessKey, RegionEndpoint region, string bucket, string key, Stream stream, string contentType)
        {
            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = key,
                    BucketName = bucket,
                    ContentType = contentType
                };

                var fileTransferUtility = new TransferUtility(client);

                await fileTransferUtility.UploadAsync(uploadRequest);

                return true;

            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }

            return false;
        }

        public async static Task<bool> DeleteFileAsync(string accessKeyId, string secretAccessKey, string region, string bucket, string key)
        {
            return await DeleteFileAsync(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(region), bucket, key);
        }

        public async static Task<bool> DeleteFileAsync(string accessKeyId, string secretAccessKey, RegionEndpoint region, string bucket, string key)
        {
            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);
                await client.DeleteObjectAsync(bucket, key);
                return true;
            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }

            return false;
        }
    }
}
