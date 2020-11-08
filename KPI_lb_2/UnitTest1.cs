using Xunit;
using IIG.FileWorker;
using System.IO;
using System;

namespace KPI_lb2
{
    public class UnitTest1
    {
        #region test
        [Theory]
        [InlineData("well some text")]
        [InlineData(null)]
        public void Test1(string forTest1)
        {
            Assert.Equal(forTest1, WriteCheck(forTest1, "path1.txt"));
        }

        [Fact]
        public void Test1_null()
        {
            string forTest1Null = null;
            Assert.Equal("",WriteCheck(forTest1Null, "path1_null.txt"));
        }

        [Fact]
        public void Test2()
        {
            string forTest2 = "string to read";
            Assert.Equal(forTest2, ReadAllCheck(forTest2, "path2.txt"));
        }

        [Theory]
        [InlineData("first", "second", "jkl")]
        [InlineData("")]
        public void Test3(params string[] forTest3)
        {
            Assert.Equal(forTest3, ReadLinesCheck(forTest3, "path3.txt"));
        }

        [Theory]
        [InlineData("first, second jkl")]
        [InlineData(null)]
        public void Test4(string forTest4)
        {
            Assert.Equal(forTest4, TryWriteCheck(forTest4, "path4.txt", 5));
        }


        [Theory]
        [InlineData("got some text to copy")]
        [InlineData(null)]
        public void Test5(string text_to_copy)
        {
            string path = "path5.txt";
            string other_path = "other_path5.txt";
            File.Create(other_path).Close();

            File.WriteAllText(path, text_to_copy);
            Assert.True(TryCopyCheck(path, other_path, true, 5));
        }

        [Fact]
        public void Test5_1()
        {
            string path = "path5_1.txt";
            File.WriteAllText(path, "got some text to copy");
            // used to check if it creates directory
            Assert.True(TryCopyCheck(path, "other_path5_1.txt", true, 5));
        }

        [Fact]
        public void Test6()
        {
            string filename = "file_to_check6.txt";
            File.Create(filename).Close();
            Assert.Equal(filename, GetFilenameCheck(filename));
        }

        [Fact]
        public void Test6_1()
        {
            string filename = "file_to_check6_1.txt";
            Assert.NotEqual(filename, GetFilenameCheck(filename));
        }

        [Fact]
        public void Test7()
        {
            string filename = "file_to_check7.txt";
            File.Create(filename);
            Assert.Equal(Path.GetFullPath(filename), GetFullPath(filename));
        }

        [Theory]
        [InlineData("myNewDir")]
        [InlineData("MYNEWDIR")]
        public void Test8(string dirname)
        {
            Assert.True(MkDirCheck(dirname));
        }

        [Theory]
        [InlineData("./<*new")] // added to check for Error throw
        [InlineData(null)] // added to check for Error throw
        public void Test8_Fail(string dirname_f)
        {
            Assert.False(MkDirCheck(dirname_f));
        }

        #endregion test


        #region methods
        public string WriteCheck(string line, string path) {
            BaseFileWorker.Write(line, path);
            return File.ReadAllText(path);
        }

        public string ReadAllCheck(string line, string path) {
            File.WriteAllText(path, line);
            return BaseFileWorker.ReadAll(path);
        }

        public string[] ReadLinesCheck(string[] lines, string path)
        {
            File.WriteAllLines(path, lines);
            return BaseFileWorker.ReadLines(path);
        }

        public string TryWriteCheck(string line, string path, int tries) {
            BaseFileWorker.TryWrite(line, path, tries);
            return File.ReadAllText(path);
        }

        public bool TryCopyCheck(string toPath, string fromPath,bool rewrite, int tries) {
            BaseFileWorker.TryCopy(fromPath, toPath, rewrite, tries);
            return File.ReadAllText(fromPath).Equals(File.ReadAllText(toPath));
          
        }

        public string GetFilenameCheck(string path) {
            return BaseFileWorker.GetFileName(path);
        }

        public string GetFullPath(string path) {
            return BaseFileWorker.GetFullPath(path);
        }

        public bool MkDirCheck(string name){
            try {
                BaseFileWorker.MkDir(name);
            }
            catch (System.Exception e) {
                Console.WriteLine(e.StackTrace);
            }
            
            return Directory.Exists(name);
        }

        #endregion methods
    }
}
