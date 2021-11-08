namespace Email.NET.Test.Models
{
    using System;
using System.IO;
    using Xunit;

    public class AttachmentShould
    {
        #region base attachment file name tests

        [Fact]
        public void ThrowIfFileNameIsNull()
        {
            // arrange

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var attachment = new Base64Attachement(null, MockData.TestFileBase64Value);
            });
        }

        [Fact]
        public void ThrowIfFileNameIsEmpty()
        {
            // arrange

            // assert
            Assert.Throws<ArgumentException>(() =>
            {
                // act
                var attachment = new Base64Attachement(string.Empty, MockData.TestFileBase64Value);
            });
        }

        [Fact]
        public void ThrowIfFileNameHasNoExtension()
        {
            // arrange

            // assert
            Assert.Throws<ArgumentException>(() =>
            {
                // act
                var attachment = new Base64Attachement("test_file", MockData.TestFileBase64Value);
            });
        }

        #endregion

        #region base64 attachment

        [Fact]
        public void CreateBase64Attachment()
        {
            // arrange
            var fileName = "test_file.txt";
            var expectedfileArrayData = MockData.TestFileAsByteArray();

            // act
            var attachment = new Base64Attachement(fileName, MockData.TestFileBase64Value);

            // assert
            Assert.Equal("test_file.txt", attachment.FileName);
            Assert.Equal("text/plain", attachment.FileType);
            Assert.Equal(".txt", attachment.Extension);
            Assert.Equal(expectedfileArrayData, attachment.File);
        }

        [Fact]
        public void ThrowIfBase64IsNull()
        {
            // arrange
            var fileName = "test_file.txt";

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var attachment = new Base64Attachement(fileName, null);
            });
        }

        [Fact]
        public void ThrowIfBase64IsEmpty()
        {
            // arrange
            var fileName = "test_file.txt";

            // assert
            Assert.Throws<ArgumentException>(() =>
            {
                // act
                var attachment = new Base64Attachement(fileName, string.Empty);
            });
        }



        #endregion

        #region Byte[] attachment

        [Fact]
        public void CreateByteArrayAttachment()
        {
            // arrange
            var fileName = "test_file.txt";
            var expectedfileArrayData = MockData.TestFileAsByteArray();

            // act
            var attachment = new ByteArrayAttachment(fileName, MockData.TestFileAsByteArray());

            // assert
            Assert.Equal("test_file.txt", attachment.FileName);
            Assert.Equal("text/plain", attachment.FileType);
            Assert.Equal(".txt", attachment.Extension);
            Assert.Equal(expectedfileArrayData, attachment.File);
        }

        [Fact]
        public void ThrowIfeByteArrayIsNull()
        {
            // arrange
            var fileName = "test_file.txt";

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var attachment = new ByteArrayAttachment(fileName, null);
            });
        }

        [Fact]
        public void ThrowIfeByteArrayIsEmpty()
        {
            // arrange
            var fileName = "test_file.txt";

            // assert
            Assert.Throws<ArgumentException>(() =>
            {
                // act
                var attachment = new ByteArrayAttachment(fileName, Array.Empty<byte>());
            });
        }



        #endregion

        #region file path attachment

        [Fact]
        public void CreateFilePathAttachment()
        {
            // arrange
            var fileName = "test_file.txt"; 
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Email.Net", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllBytes(filePath, MockData.TestFileAsByteArray());

            // act
            var attachment = new FilePathAttachment(filePath);

            // assert
            Assert.Equal(fileName, attachment.FileName);
            Assert.Equal(filePath, attachment.FilePath);
            Assert.Equal("text/plain", attachment.FileType);
            Assert.Equal(".txt", attachment.Extension);

            File.Delete(filePath);
            Directory.Delete(Path.GetDirectoryName(filePath));
        }

        [Fact]
        public void ThrowIfFileNotExistIsNull()
        {
            // arrange
            var fileName = "test_file_not_exis.txt";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Email.Net", fileName);

            // assert
            Assert.Throws<FileNotFoundException>(() =>
            {
                // act
                var attachment = new FilePathAttachment(filePath);
            });
        }

        [Fact]
        public void ThrowIfFilePathIsNull()
        {
            // arrange

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var attachment = new FilePathAttachment(null);
            });
        }

        [Fact]
        public void ThrowIfFilePathIsEmpty()
        {
            // arrange

            // assert
            Assert.Throws<ArgumentException>(() =>
            {
                // act
                var attachment = new FilePathAttachment(string.Empty);
            });
        }



        #endregion
    }
}
