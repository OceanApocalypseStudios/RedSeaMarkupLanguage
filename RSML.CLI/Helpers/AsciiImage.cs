using System;
using System.IO;
using System.Text;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace OceanApocalypseStudios.RSML.CLI.Helpers
{

	public class AsciiImage(
		string imagePath
	) : IDisposable
	{

		private const string CHARS = "@%#*+=-:. ";
		private readonly Image<Rgba32> image = Image.Load<Rgba32>(imagePath);

		public AsciiImage(FileInfo file) : this(file.FullName) { }

		/// <inheritdoc />
		public void Dispose()
		{

			image.Dispose();
			GC.SuppressFinalize(this);

		}

		public string GetRenderable(int width, int height, bool adjustForFontAspect = true)
		{

			if (adjustForFontAspect)
				height /= 2;

			// Resize using ImageSharp
			image.Mutate(ctx => ctx.Resize(width, height));

			StringBuilder builder = new();

			for (int y = 0; y < image.Height; y++)
			{

				for (int x = 0; x < image.Width; x++)
				{

					var pixel = image[x, y];
					int gray = (pixel.R + pixel.G + pixel.B) / 3;
					int index = gray * (CHARS.Length - 1) / 255;

					builder.Append("[rgb(")
						   .Append(pixel.R)
						   .Append(',')
						   .Append(pixel.G)
						   .Append(',')
						   .Append(pixel.B)
						   .Append(')')
						   .Append(']')
						   .Append(CHARS[index])
						   .Append("[/]");

				}

				builder.AppendLine();

			}

			return builder.ToString();

		}

	}

}
