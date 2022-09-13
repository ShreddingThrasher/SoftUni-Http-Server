using SoftUniHttpServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SoftUniHttpServer.Server.Responses
{
    public class FileResponse : Response
    {
        public string FileName { get; set; }

        public FileResponse(string fileName)
            : base(StatusCode.OK)
        {
            this.FileName = fileName;

            this.Headers.Add(Header.ContentDisposition, ContentType.FileContent);
        }

        public override string ToString()
        {
            if (File.Exists($"../../../{this.FileName}"))
            {
                this.Body = string.Empty;
                var fileContent = File.ReadAllBytes($"../../../{this.FileName}");

                var fileBytesCount = new FileInfo($"../../../{this.FileName}").Length;
                this.Headers.Add(Header.ContentLength, fileBytesCount.ToString());

                this.Headers.Add(Header.ContentDisposition,
                    $@"attachment; filename={this.FileName}");

            }

            return base.ToString();
        }
    }
}
