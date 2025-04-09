namespace VideoGameApi.Services
{
   using Minio;
   using Minio.DataModel;
    using Minio.DataModel.Args;
    using Minio.Exceptions;
   using System;
   using System.IO;
   using System.Threading.Tasks;

   public class FileUploadService
   {

       public FileUploadService()
       {
       }

       private async static Task Run(IMinioClient minio)
       {
            var bucketName = "videogame-images"; 
            var objectName = "images/example-image.jpg"; 
            var filePath = "C:\\Users\\username\\Pictures\\example-image.jpg"; 
            var contentType = "image/jpeg";

            try
            {
                // Check if the bucket exists
                var bucketExistsArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await minio.BucketExistsAsync(bucketExistsArgs).ConfigureAwait(false);

                if (!found)
                {
                    // Create the bucket if it doesn't exist
                    var makeBucketArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minio.MakeBucketAsync(makeBucketArgs).ConfigureAwait(false);
                }

                // Upload the image to the bucket
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(filePath)
                    .WithContentType(contentType);
                await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                Console.WriteLine("Successfully uploaded " + objectName);
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }
       }
   }
}
