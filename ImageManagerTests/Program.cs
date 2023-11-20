using WebApp1.Data.FileManager;

const string basePath = "./";

var logoFileManager = new FileManager(Path.Combine(basePath, "logo"));
var logoImageManager = new ImageManager(logoFileManager);

// Save.
await using var stream = File.OpenRead("image.png");

var path = await logoImageManager.SaveImage(stream, "image.png");
Console.WriteLine(path);

// Save with resize.
await using var stream2 = File.OpenRead("image.png");

var path2 = await logoImageManager.SaveImage(stream2, "image.png", new ImageSizeOptions(500, 500));
Console.WriteLine(path2);

// Update.
await using var stream3 = File.OpenRead("j.jpg");

var path3 = await logoImageManager.UpdateImage(path!, stream3);
Console.WriteLine(path3);


// Update with resize.
await using var stream4 = File.OpenRead("j.jpg");

var path4 = await logoImageManager.UpdateImage(path2!, stream4 , new ImageSizeOptions(300, 300));
Console.WriteLine(path4);
