using Xunit;
using IIG.FileWorker;
using System.IO;
using System;
using System.Threading;

namespace KPI_lb2
{
    public class BlackBoxUnit
    {
        #region test
        // get filename tests
        [Theory]
        [InlineData("name.txt", "./name.txt")]
        [InlineData("name.png", "./name.png")]
        public void GetFilenameEqual(string filename, string path)
        {
            File.WriteAllText(path, filename);
            Assert.Equal(filename, BaseFileWorker.GetFileName(path));
        }

        [Theory]
        [InlineData("nameNE.txt", "./nameNE")]
        public void GetFilenameNotEqual(string filename, string path)
        {
            Directory.CreateDirectory(path);
            Assert.NotEqual(filename, BaseFileWorker.GetFileName(path));
        }

        [Theory]
        [InlineData("")]
        [InlineData("{Directory.GetCurrentDirectory()}noFile.txt")]
        [InlineData(null)]
        public void GetFilenameError(string path)
        {
            Assert.Throws<Exception>(() => BaseFileWorker.GetFileName(path));
        }

        //get full path tests
        [Theory]
        [InlineData("namePth.txt")]
        [InlineData("pict.png")]
        public void GetFullPath(string filename)
        {
            File.WriteAllText(Directory.GetCurrentDirectory()+"/"+filename, filename);
            string path = Path.GetFullPath(Directory.GetCurrentDirectory()+"/" + filename);
            Assert.Equal(path, BaseFileWorker.GetFullPath(filename));
        }

        [Theory]
        [InlineData("namePthPth.txt")]
        [InlineData("")]
        public void GetFullPathFromPath(string filename)
        {
            string path;
            if (filename != "")
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + "/" + filename, filename);
                path = Path.GetFullPath(Directory.GetCurrentDirectory() + "/" + filename);
            }
            else {
                path = Path.GetFullPath(Directory.GetCurrentDirectory());
            }
            Assert.Equal(path, BaseFileWorker.GetFullPath(path));
        }

        [Fact]
        public void GetFullPathError()
        {

            Assert.Throws<ArgumentException>(() => BaseFileWorker.GetFullPath(""));
            Assert.Throws<ArgumentNullException>(() => BaseFileWorker.GetFullPath(null));
            Assert.Throws<Exception>(() => BaseFileWorker.GetFullPath("noSuchFile.txt"));

            Assert.Throws<Exception>(() => BaseFileWorker.GetFullPath("inval:File.txt"));
        }

        //mkdir tests
        [Theory]
        [InlineData("dirName")]
        [InlineData("curr.txt")]
        public void MkDir(string dirName)
        {

            BaseFileWorker.MkDir(dirName);
            Assert.True(Directory.Exists(dirName));
        }

        [Theory]
        [InlineData("dirNameP")]
        [InlineData("currP.txt")]
        public void MkDirPath(string dirName)
        {
            string path_d = Path.GetFullPath(dirName);
            BaseFileWorker.MkDir(path_d);
            Assert.True(Directory.Exists(path_d));
        }

        [Fact]
        public void MkDirError()
        {
            Assert.Throws<ArgumentException>(() => BaseFileWorker.MkDir(""));
            Assert.Throws<ArgumentNullException>(() => BaseFileWorker.MkDir(null));
        }
        //read all tests

        [Theory]
        [InlineData("validRA.txt", "some dummy string")]
        [InlineData("ReadAll5.png", "")]
        public void ReadAll(string name, string content)
        {
            string path_ra = Directory.GetCurrentDirectory() +"/"+ name;
            if (content != "")
            {
                File.WriteAllText(path_ra, content);
            }
            else
            {
                content = System.Text.Encoding.Default.GetString(File.ReadAllBytes(path_ra));
            }
            Assert.Equal(content, BaseFileWorker.ReadAll(path_ra));

        }

        [Fact]
        public void ReadAllError()
        {
            Assert.Throws<Exception>(() => BaseFileWorker.ReadAll(Directory.GetCurrentDirectory()+"not_valid.txt"));
            Assert.Throws<Exception>(() => BaseFileWorker.ReadAll(""));
            Assert.Throws<Exception>(() => BaseFileWorker.ReadAll(null));
        }

        //read lines tests

        [Theory]
        [InlineData("validRL.txt", new string[] { "some", "dummy", "string" })]
        [InlineData("ReadLines5.png", null)]
        public void ReadLines(string name, string[] content)
        {
            string path_rl = Directory.GetCurrentDirectory()+"/" + name;
            if (content != null)
            {
                File.WriteAllLines(path_rl, content);
            }
            else
            {
                content = File.ReadAllLines(path_rl);
            }

            string[] read = BaseFileWorker.ReadLines(path_rl);
            Assert.Equal(content[0], read[0]);
            Assert.Equal(content[content.Length - 1], read[read.Length - 1]);

        }


        [Fact]
        public void ReadLinesError()
        {

            Assert.Throws<Exception>(() => BaseFileWorker.ReadAll("invalRL.txt"));
            Assert.Throws<Exception>(() => BaseFileWorker.ReadAll(""));
            Assert.Throws<Exception>(() => BaseFileWorker.ReadAll(null));
            Assert.Throws<Exception>(() => BaseFileWorker.ReadAll("@#%^\\"));
        }

        //try copy test
        [Theory]
        [InlineData("fromtrycpy1.txt", "totrycpy1.txt", true, 5)]
        [InlineData("fromtrycpy2.txt", "totrycpyl2.txt", false, 5)]
        public void TryCopy(string from, string to, bool rewrite, int num_of_tries)
        {
            string initialTo = "written to to";
            string initialFrom = "written to from";
            
            string check = initialFrom;
            if (rewrite)
            {
                File.WriteAllText(to, initialTo);
            }

            File.WriteAllText(from, initialFrom);
            BaseFileWorker.TryCopy(from, to, rewrite, num_of_tries);
            Assert.Equal(check, File.ReadAllText(to));
        }

        [Fact]
        public void TryCopyThread()
        {
            string threadTo = "threadTo.txt";
            string threadFrom = "threadFrom.txt";
            string initialTo = "written to to";
            string initialFrom = "written to from";
            File.WriteAllText(threadTo, initialTo);
            File.WriteAllText(threadFrom, initialFrom);

            //add thread
            (new Thread(() =>
            {
                var file = File.OpenRead(threadFrom);
                Thread.Sleep(2000);
                file.Close();
            })).Start();

            BaseFileWorker.TryCopy(threadFrom, threadTo, true, 100);

            Assert.Equal(initialFrom, File.ReadAllText(threadTo));
        }

        [Fact]
        public void TryCopyEmpty()
        {
            string emptyTo = "emptyTo.txt";
            string emptyFrom = "emptyFrom.txt";
            string initialTo = "smth";
            string initialFrom = "";
            File.WriteAllText(emptyTo, initialTo);
            File.WriteAllText(emptyFrom, initialFrom);
            BaseFileWorker.TryCopy(emptyFrom, emptyTo, true, 10);

            Assert.Equal(initialFrom, File.ReadAllText(emptyTo));

        }

        [Fact]
        public void TryCopyError()
        {

            string to = Directory.GetCurrentDirectory()+"toEr.txt";
            string from = Directory.GetCurrentDirectory()+"fromEr.txt";
            File.WriteAllText(to, "written to to");
            File.WriteAllText(from, "written to from");
            Assert.Throws<Exception>(() => BaseFileWorker.TryCopy(Directory.GetCurrentDirectory()+"not_valid_from.txt", to, true, 5));
            Assert.Throws<Exception>(() => BaseFileWorker.TryCopy(Directory.GetCurrentDirectory()+"not_valid_from", to, true, 5));

            Assert.Throws<Exception>(() => BaseFileWorker.TryCopy(from, Directory.GetCurrentDirectory()+"not_valid_to.txt", true, 5));
            Assert.Throws<Exception>(() => BaseFileWorker.TryCopy(from, Directory.GetCurrentDirectory()+"not_valid_to", true, 5));
            Assert.Throws<Exception>(() => BaseFileWorker.TryCopy(from, to, true, -1));
        }
        //try write tests
        [Theory]
        [InlineData("some input", "try_write.txt", 5)]
        [InlineData("", "try_write.txt", 5)]
        public void TryWrite(string input, string file, int tries)
        {
            BaseFileWorker.TryWrite(input, file, tries);

            Assert.Equal(input, File.ReadAllText(Directory.GetCurrentDirectory()+"/" + file));
        }

        [Fact]
        public void TryWriteThread()
        {
            string try_write_thread = Directory.GetCurrentDirectory()+"/"+"try_write_thread.txt";
            string str_from = "some info from user";
            (new Thread(() =>
            {
                var file = File.OpenRead(try_write_thread);
                Thread.Sleep(2000);
                file.Close();
            })).Start();

            BaseFileWorker.TryWrite(str_from, try_write_thread, 100);

            Assert.Equal(str_from, File.ReadAllText(try_write_thread));
        }

        [Fact]
        public void TryWriteError()
        {
            Assert.Throws<Exception>(() => BaseFileWorker.TryWrite("something", "try_write_err.txt", -1));
            Assert.Throws<Exception>(() => BaseFileWorker.TryWrite("something", null, 3));
        }

        //write tests
        [Theory]
        [InlineData("some input", "try_write1.txt")]
        [InlineData("", "try_write2.txt")]
        [InlineData("@#%%$", "try_write3.txt")]
        [InlineData("sdfxz", "try_write4.png")]
        public void Write(string input, string file)
        {
            BaseFileWorker.Write(input, file);

            Assert.Equal(input, File.ReadAllText(Directory.GetCurrentDirectory()+"/" + file));
        }


        [Fact]
        public void WriteError()
        {
            Assert.Throws<Exception>(() => BaseFileWorker.Write("something", ""));
            Assert.Throws<Exception>(() => BaseFileWorker.Write("something", "#$@!"));
            Assert.Throws<Exception>(() => BaseFileWorker.Write("something", "try_write_errtxt"));
            Assert.Throws<Exception>(() => BaseFileWorker.Write("something", "try_write_err.txt"));
            Assert.Throws<Exception>(() => BaseFileWorker.Write("something", null));
        }

        #endregion test
    }
        
}
