using Business_Layer.Business;
using Business_Layer.Interfaces;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.BusinessTests
{
    public class ImagesBusinessTests
    {
        private readonly ImagesBusiness _imagesBusiness;
        public ImagesBusinessTests()
        {
            var mockImageLogger = new Mock<ILogger<ImagesBusiness>>();
            _imagesBusiness = new ImagesBusiness(mockImageLogger.Object);
        }

        [Fact]
        public void IsAnimatedWebP_ShouldReturnFalse_WhenStreamIsNull()
        {
            Stream? stream = null;
            bool result = _imagesBusiness.IsAnimatedWebP(stream!);
            Assert.False(result);
        }

        [Fact]
        public void IsAnimatedWebP_ShouldReturnFalse_WhenStreamIsEmpty()
        {
            using var stream = new MemoryStream();
            bool result = _imagesBusiness.IsAnimatedWebP(stream);
            Assert.False(result);
        }

        [Fact]
        public void IsAnimatedWebP_ShouldReturnFalse_WhenStreamDoesNotContainAnim()
        {
            byte[] data = Encoding.ASCII.GetBytes("RIFFWEBP");
            using var stream = new MemoryStream(data);
            bool result = _imagesBusiness.IsAnimatedWebP(stream);
            Assert.False(result);
        }

        [Fact]
        public void IsAnimatedWebP_ShouldReturnTrue_WhenStreamContainsAnim()
        {
            byte[] data = Encoding.ASCII.GetBytes("RIFFANIMWEBP");
            using var stream = new MemoryStream(data);
            bool result = _imagesBusiness.IsAnimatedWebP(stream);
            Assert.True(result);
        }

        [Fact]
        public void IsAnimatedWebP_ShouldWork_WithLargeStream()
        {
            string content = new string('A', 1000) + "ANIM" + new string('B', 1000);
            byte[] data = Encoding.ASCII.GetBytes(content);
            using var stream = new MemoryStream(data);
            bool result = _imagesBusiness.IsAnimatedWebP(stream);
            Assert.True(result);
        }

        [Fact]
        public async Task StreamImage_ShouldReturnFalse_WhenFilePathIsNull()
        {
            var fileMock = new Mock<IFormFile>();

            var result = await _imagesBusiness.StreamImage(null!, fileMock.Object);

            Assert.False(result);
        }

        [Fact]
        public async Task StreamImage_ShouldReturnFalse_WhenFileIsNull()
        {
            var result = await _imagesBusiness.StreamImage("test.webp", null!);

            Assert.False(result);
        }

        [Fact]
        public async Task StreamImage_ShouldReturnFalse_WhenFileIsEmpty()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            var result = await _imagesBusiness.StreamImage("test.webp", fileMock.Object);

            Assert.False(result);
        }

        [Fact]
        public async Task StreamImage_ShouldReturnFalse_WhenExceptionOccurs()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new Exception("Disk error"));

            var result = await _imagesBusiness.StreamImage("test.webp", fileMock.Object);

            Assert.False(result);
        }

        [Fact]
        public async Task StreamImage_ShouldReturnTrue_WhenFileIsSavedSuccessfully()
        {
            var content = "Fake Image Content";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                    .Returns<Stream, CancellationToken>((s, _) => stream.CopyToAsync(s));

            string path = Path.GetTempFileName(); 

            var result = await _imagesBusiness.StreamImage(path, fileMock.Object);

            Assert.True(result);
            Assert.True(File.Exists(path));
            Assert.True(new FileInfo(path).Length > 0);

            File.Delete(path);
        }

    }
}
