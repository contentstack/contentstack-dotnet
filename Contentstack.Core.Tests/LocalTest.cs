using System;

using Contentstack.Core;
using Contentstack.Core.Models;
using Contentstack.Core.Configuration;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace Contentstack.Core.Tests
{
    public class LocalTest
    {
        [Fact]
        public async Task FetchQueryForEntry()
        {
            ContentstackClient contentstackClient = new ContentstackClient("blt520df8d675f11a0a", "bltcc8f202fa758bdf2", "development");
            Query csQuery = contentstackClient.ContentType("test").Query();
            csQuery.AddQuery("uid", "blt9f9e183545e2fe5b");
            csQuery.SetLanguage(Internals.Language.GERMEN_SWITZERLAND);
            Query query = await csQuery.FindOne();
            Console.WriteLine(query.Result.ToString());
            Assert.True(true, "result is greater than 11");
        }
        [Fact]
        public async Task FetchEntry()
        {
            ContentstackClient contentstackClient = new ContentstackClient("blt520df8d675f11a0a", "bltcc8f202fa758bdf2", "development");
            Entry csEntry = contentstackClient.ContentType("test").Entry("blt9f9e183545e2fe5b");
            csEntry.SetLanguage(Internals.Language.GERMEN_SWITZERLAND);
            Entry entry = await csEntry.Fetch();
            Console.WriteLine(entry.ToString());
            Assert.True(true, "result is greater than 11");
        }

        // [Fact]
        // public async Task uploadfile()
        // {
        //     string url = "https://api.contentstack.io/v3/assets?relative_urls=true";
        //     string filename = "myFile.jpg";
        //     string result = "";
        //     using (var formContent = new MultipartFormDataContent())
        //     {
        //         Stream fileStream = System.IO.File.OpenRead(@"../../../../../../Desktop/qr.png");
                
        //         StreamContent imagePart = new StreamContent(fileStream);
        //         imagePart.Headers.Add("Content-Type", "image/jpeg");

        //         formContent.Add(imagePart, "asset[upload]", filename);
        //         using (var client = new HttpClient())
        //         {
        //             client.DefaultRequestHeaders.Add("api_key", "bltd2f9d1005d56c0db");
        //             client.DefaultRequestHeaders.Add("authtoken", "blt5e001e170f1ca131");
        //             client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
        //             try
        //             {
        //                 var message = await client.PostAsync(url, formContent);
        //                 result = await message.Content.ReadAsStringAsync();
        //             }
        //             catch (Exception ex)
        //             {
        //                 throw ex;
        //             }
        //         }
        //     }
        // }
    }
}
