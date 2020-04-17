using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using ZXing;
using System.IO;
using System;
using System.Drawing;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace NosAyudamos
{
    public interface IPersonRecognizer
    {
        Task<Person?> RecognizeAsync(Uri imageUri);
    }

    public static class PersonRecognizerExtensions
    {
        public static Task<Person?> RecognizeAsync(this IPersonRecognizer recognizer, string? imageUrl)
        {
            if (imageUrl == null ||
                !Uri.TryCreate(imageUrl, UriKind.Absolute, out var imageUri) ||
                imageUri == null)
            {
                return Task.FromResult<Person?>(default);
            }

            return (recognizer ?? throw new ArgumentNullException(nameof(recognizer))).RecognizeAsync(imageUri);
        }
    }

    public class PersonRecognizer : IPersonRecognizer
    {
        private readonly Lazy<BarcodeReader> reader;

        public PersonRecognizer()
        {
            reader = new Lazy<BarcodeReader>(
                () => new BarcodeReader()
                {
                    AutoRotate = true,
                    TryInverted = true,
                    Options = new ZXing.Common.DecodingOptions()
                    {
                        TryHarder = true,
                        PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.PDF_417 }
                    },
                });
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308")]
        public async Task<Person?> RecognizeAsync(Uri imageUri)
        {
            var bytes = await Utility.DownloadBlobAsync(imageUri);

            using var mem = new MemoryStream(bytes);
            using var image = (Bitmap)Image.FromStream(mem);

            var result = reader.Value.Decode(image);
            if (result != null)
            {
                //00501862505@ANDERSON@JAMIE FALKLAND@M@19055847@A@13/10/1974@03/07/2017
                var elements = result.Text.Split("@");

                if (elements.Length > 0)
                {
                    var textInfo = CultureInfo.InvariantCulture.TextInfo;

                    return new Person
                    {
                        LastName = textInfo.ToTitleCase(elements[1].ToLowerInvariant()),
                        FirstName = textInfo.ToTitleCase(elements[2].ToLowerInvariant()),
                        Sex = elements[3],
                        NationalId = elements[4],
                        DateOfBirth = elements[6]
                    };
                }
            }

            return null;
        }
    }
}
