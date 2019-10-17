using Xunit;
using md2docx;
using System.IO;
using Xunit.Abstractions;

namespace md2docxTests
{
    public class ImageGetterTests
    {
        private readonly ITestOutputHelper output;

        public ImageGetterTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("Fail.jpg")]
        public void DiskFileNotFound(string name)
        {
            output.WriteLine(System.IO.Directory.GetCurrentDirectory());
            DirectoryInfo sourceDir = new DirectoryInfo("../../../TestFiles/");
            string expectedPath = Path.Combine(sourceDir.FullName, name);
            output.WriteLine(expectedPath);
            byte[] expected = File.ReadAllBytes(expectedPath);

            string pathNotExist = "/this/path/should/not/be/found/unless/!@#$%^&*()";
            ImageGetter imageGetter = new ImageGetter();
            bool result = imageGetter.Load(pathNotExist);
            output.WriteLine(pathNotExist);
            byte[] actual = imageGetter.ImageData;

            Assert.False(result);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Fail.jpg")]
        public void NetFileNotFound(string name)
        {

            DirectoryInfo sourceDir = new DirectoryInfo("../../../TestFiles/");
            string expectedPath = Path.Combine(sourceDir.FullName, name);
            byte[] expected = File.ReadAllBytes(expectedPath);

            string pathNotExist = "http://Idontcarewhatthiswebsiteis/but/it/should/not/exist!.jpg";
            ImageGetter imageGetter = new ImageGetter();
            bool result = imageGetter.Load(pathNotExist);
            byte[] actual = imageGetter.ImageData;

            Assert.False(result);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("yaml.jpg", "yaml.jpg")]
        public void FileInDisk(string name, string data)
        {
            DirectoryInfo sourceDir = new DirectoryInfo("../../../TestFiles/");
            string expectedPath = Path.Combine(sourceDir.FullName, name);
            byte[] expected = File.ReadAllBytes(expectedPath);

            string actualPath = Path.Combine(sourceDir.FullName, data);
            ImageGetter imageGetter = new ImageGetter();
            bool result = imageGetter.Load(actualPath);
            byte[] actual = imageGetter.ImageData;

            Assert.True(result);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("https://raw.githubusercontent.com/CSUwangj/md2docx-csharp/master/docs/res/yaml.jpg", "yaml.jpg")]
        public void FileInNetwork(string url, string data)
        {
            DirectoryInfo sourceDir = new DirectoryInfo("../../../TestFiles/");
            string expectedPath = Path.Combine(sourceDir.FullName, data);
            byte[] expected = File.ReadAllBytes(expectedPath);

            ImageGetter imageGetter = new ImageGetter();
            bool result = imageGetter.Load(url);
            byte[] actual = imageGetter.ImageData;

            Assert.True(result);
            Assert.Equal(expected, actual);
        }
    }
}
