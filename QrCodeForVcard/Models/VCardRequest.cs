namespace QrCodeForVcard.Models
{
    public class VCardRequest
    {
        /// <summary>
        /// First name of the contact (required)
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the contact (optional)
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Organization/Company name (optional)
        /// </summary>
        public string? Organization { get; set; }

        /// <summary>
        /// Job title (optional)
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Email address (optional)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Mobile phone number (optional)
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Street address (optional)
        /// </summary>
        public string? Street { get; set; }

        /// <summary>
        /// City (optional)
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// State/Province (optional)
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Postal code (optional)
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// Country (optional)
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Website URL (optional)
        /// </summary>
        public string? Website { get; set; }

        /// <summary>
        /// Notes (optional)
        /// </summary>
        public string? Notes { get; set; }
    }

    public class VCardQrResponse
    {
        /// <summary>
        /// Download URL for the QR code image
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Download URL for the QR code PDF
        /// </summary>
        public string? PdfUrl { get; set; }

        /// <summary>
        /// Base64 encoded QR code image (optional, for immediate display)
        /// </summary>
        public string? Base64Image { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; } = "QR Code generated successfully";
    }
}
