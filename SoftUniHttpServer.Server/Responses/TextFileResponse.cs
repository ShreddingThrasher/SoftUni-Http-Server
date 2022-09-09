﻿using SoftUniHttpServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SoftUniHttpServer.Server.Responses
{
    public class TextFileResponse : Response
    {
        public string FileName { get; set; }

        public TextFileResponse(string fileName)
            : base(StatusCode.OK)
        {
            this.FileName = fileName;

            this.Headers.Add(Header.ContentDisposition, ContentType.PlainText);
        }

        public override string ToString()
        {
            if (File.Exists(this.FileName))
            {
                this.Body = File.ReadAllTextAsync(this.FileName).Result;

                var fileBytesCount = new FileInfo(this.FileName).Length;
                this.Headers.Add(Header.ContentLength, fileBytesCount.ToString());

                this.Headers.Add(Header.ContentDisposition,
                    $@"attaqchment; filename={this.FileName}");

            }

            return base.ToString();
        }
    }
}
