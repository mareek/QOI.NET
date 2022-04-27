namespace QOI.Test
{
    public class IntegrationTestBase
    {
        protected static DirectoryInfo TestImagesDirectory => new("TestImages");

        public static IEnumerable<FileInfo[]> GetReferenceImageCouples()
        {
            var qoiFilesByName = TestImagesDirectory.EnumerateFiles("*.qoi")
                                                    .ToDictionary(f => Path.GetFileNameWithoutExtension(f.FullName));
            var referencePngFiles = TestImagesDirectory.EnumerateFiles("*.png").ToArray();
            foreach (var referencePngFile in referencePngFiles)
            {
                var referenceQoiFile = qoiFilesByName[Path.GetFileNameWithoutExtension(referencePngFile.FullName)];
                yield return new[] { referencePngFile, referenceQoiFile };
            }
        }
    }
}