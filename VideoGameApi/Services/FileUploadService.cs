//namespace VideoGameApi.Services
//{
//    using Minio;
//    using Minio.Exceptions;
//    using System;
//    using System.IO;
//    using System.Threading.Tasks;

//    public class FileUploadService
//    {
//        private readonly IMinioClient _minioClient;
//        private readonly string _bucketName = "videogame-images";

//        public FileUploadService(IMinioClient minioClient)
//        {
//            _minioClient = minioClient;
//        }

//        private async static Task Run(IMinioClient minio)
//        {
//            var bucketName = "mymusic";
//            var location = "us-east-1";
//            var objectName = "golden-oldies.zip";
//            var filePath = "C:\\Users\\username\\Downloads\\golden_oldies.mp3";
//            var contentType = "application/zip";

//            try
//            {
//                // Make a bucket on the server, if not already present.
//                var beArgs = new BucketExistsArgs()
//                    .WithBucket(bucketName);
//                bool found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
//                if (!found)
//                {
//                    var mbArgs = new MakeBucketArgs()
//                        .WithBucket(bucketName);
//                    await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
//                }
//                // Upload a file to bucket.
//                var putObjectArgs = new PutObjectArgs()
//                    .WithBucket(bucketName)
//                    .WithObject(objectName)
//                    .WithFileName(filePath)
//                    .WithContentType(contentType);
//                await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
//                Console.WriteLine("Successfully uploaded " + objectName);
//            }
//            catch (MinioException e)
//            {
//                Console.WriteLine("File Upload Error: {0}", e.Message);
//            }
//        }
//    }
//}
