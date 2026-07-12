using FileSystemCommands;

namespace task08tests
{
    public class FileSystemCommandsTests
    {
        [Fact]
        public void DirectorySizeCommand_ShouldCalculateSize()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
            File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

            var command = new DirectorySizeCommand(testDir);
            command.Execute(); // Проверяем, что не возникает исключений

            Directory.Delete(testDir, true);
        }

        [Fact]
        public void FindFilesCommand_ShouldFindMatchingFiles()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
            File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

            var command = new FindFilesCommand(testDir, "*.txt");
            command.Execute(); // Должен найти 1 файл

            Directory.Delete(testDir, true);
        }
        [Fact]
        public void DirectorySizeCommand_ShouldLog_WhenDirectoryDoesNotExist()
        {
            var nonExistentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            var output = new StringWriter();
            Console.SetOut(output);

            var command = new DirectorySizeCommand(nonExistentDir);
            command.Execute();

            var expectedOutput = $"Данная директория не найдена{Environment.NewLine}";
            Assert.Equal(expectedOutput, output.ToString());
        }
        [Fact]
        public void FindFilesCommand_ShouldLog_WhenDirectoryDoesNotExist()
        {
            var nonExistentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            var output = new StringWriter();
            Console.SetOut(output);

            var command = new FindFilesCommand(nonExistentDir, "*.txt");
            command.Execute();

            var expectedOutput = $"Данная директория не найдена{Environment.NewLine}";
            Assert.Equal(expectedOutput, output.ToString());
        }
    }
}
