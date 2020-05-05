using System.Collections.Generic;
using System.Threading.Tasks;
using ZXing;
using System.IO;
using System;
using System.Drawing;
using System.Globalization;
using System.Net.Http;

namespace NosAyudamos
{
    class PersonalId
    {
        public PersonalId(
            string firstName,
            string lastName,
            string nationalId,
            string dateOfBirth,
            string sex)
            => (FirstName, LastName, NationalId, DateOfBirth, Sex)
            = (firstName, lastName, nationalId, dateOfBirth, sex);

        public string FirstName { get; }
        public string LastName { get; }
        public string NationalId { get; }
        public string DateOfBirth { get; }
        public string Sex { get; }
    }

    static class PersonalIdRecognizerExtensions
    {
        public static Task<PersonalId?> RecognizeAsync(this IPersonalIdRecognizer recognizer, string? imageUrl)
        {
            if (imageUrl == null ||
                !Uri.TryCreate(imageUrl, UriKind.Absolute, out var imageUri) ||
                imageUri == null)
            {
                return Task.FromResult<PersonalId?>(default);
            }

            return (recognizer ?? throw new ArgumentNullException(nameof(recognizer))).RecognizeAsync(imageUri);
        }
    }

    class PersonalIdRecognizer : IPersonalIdRecognizer
    {
        readonly Lazy<BarcodeReader> reader;
        readonly HttpClient httpClient;

        public PersonalIdRecognizer(HttpClient httpClient)
        {
            this.httpClient = httpClient;

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

        public async Task<PersonalId?> RecognizeAsync(Uri imageUri)
        {
            var bytes = await httpClient.GetByteArrayAsync(imageUri);

            using var mem = new MemoryStream(bytes);
            using var image = (Bitmap)Image.FromStream(mem);

            var result = reader.Value.Decode(image);
            if (result != null)
            {
                //00501862505@ANDERSON@JAMIE FALKLAND@M@19055847@A@13/10/1974@03/07/2017
                var elements = result.Text.Split("@");

                if (elements.Length > 0)
                {
                    return new PersonalId(
                        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(elements[2].ToLower(CultureInfo.CurrentCulture)),
                        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(elements[1].ToLower(CultureInfo.CurrentCulture)),
                        elements[4],
                        elements[6],
                        elements[3]);
                }
            }

            return null;
        }
    }

    interface IPersonalIdRecognizer
    {
        Task<PersonalId?> RecognizeAsync(Uri imageUri);
    }
}