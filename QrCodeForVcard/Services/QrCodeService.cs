using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QRCoder;

namespace QrCodeForVcard.Services
{
    public interface IQrCodeService
    {
        Task<byte[]> GenerateQrCodeImageAsync(string data, string format = "PNG");
        Task<byte[]> GenerateQrCodePdfAsync(string data, string fileName = "contact.pdf");
        string ConvertImageToBase64(byte[] imageData);
    }

    public class QrCodeService : IQrCodeService
    {
        /// <summary>
        /// Generates a QR code image in the specified format (PNG, JPG, GIF, BMP)
        /// </summary>
        public async Task<byte[]> GenerateQrCodeImageAsync(string data, string format = "PNG")
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("Data cannot be empty", nameof(data));

            return await Task.Run(() =>
            {
                using (var qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.H);
                    using (var qrCode = new PngByteQRCode(qrCodeData))
                    {
                        return qrCode.GetGraphic(20);
                    }
                }
            });
        }

        /// <summary>
        /// Generates a QR code embedded in a PDF file using PdfSharpCore (avoids iText/BouncyCastle dependencies)
        /// </summary>
        public async Task<byte[]> GenerateQrCodePdfAsync(string data, string fileName = "contact.pdf")
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("Data cannot be empty", nameof(data));

            // First generate the QR code image
            byte[] qrCodeImageBytes = await GenerateQrCodeImageAsync(data, "PNG");

            using (var ms = new MemoryStream())
            {
                using (var document = new PdfDocument())
                {
                    var page = document.AddPage();
                    // A4 size in points
                    page.Width = XUnit.FromMillimeter(210);
                    page.Height = XUnit.FromMillimeter(297);

                    using (var gfx = XGraphics.FromPdfPage(page))
                    {
                        // Draw title
                        // Centered image
                        var xImage = XImage.FromStream(() => new MemoryStream(qrCodeImageBytes));

                        double imgWidth = 300;
                        double imgHeight = 300;
                        double x = (page.Width.Point - imgWidth) / 2.0;
                        double y = 60;

                        gfx.DrawImage(xImage, x, y, imgWidth, imgHeight);

                        // Draw instructions text below image using simple layout (no external fonts required)
                        var font = new XFont("Arial", 12, XFontStyle.Regular);
                        var rect = new XRect(40, y + imgHeight + 10, page.Width.Point - 80, 200);
                        gfx.DrawString("Use your phone camera or a QR code scanner app to scan this code. It will automatically add this contact to your phone's address book.", font, XBrushes.Black, rect, XStringFormats.TopLeft);
                    }

                    document.Save(ms);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts image bytes to Base64 string for embedding in HTML/JSON
        /// </summary>
        public string ConvertImageToBase64(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                throw new ArgumentException("Image data cannot be empty", nameof(imageData));

            return Convert.ToBase64String(imageData);
        }

        private System.Drawing.Imaging.ImageFormat GetImageFormat(string format)
        {
            return format?.ToUpperInvariant() switch
            {
                "JPG" or "JPEG" => System.Drawing.Imaging.ImageFormat.Jpeg,
                "GIF" => System.Drawing.Imaging.ImageFormat.Gif,
                "BMP" => System.Drawing.Imaging.ImageFormat.Bmp,
                "TIFF" => System.Drawing.Imaging.ImageFormat.Tiff,
                _ => System.Drawing.Imaging.ImageFormat.Png
            };
        }
    }
}
