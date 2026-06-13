using Microsoft.AspNetCore.Mvc;
using QrCodeForVcard.Models;
using QrCodeForVcard.Services;

namespace QrCodeForVcard.Controllers
{
    /// <summary>
    /// API controller for generating QR codes from vCard (contact) information.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json", "image/png", "image/jpeg", "application/pdf")]
    public class VCardQrController : ControllerBase
    {
        private readonly IVCardService _vCardService;
        private readonly IQrCodeService _qrCodeService;
        private readonly ILogger<VCardQrController> _logger;

        /// <summary>
        /// Initializes a new instance of the VCardQrController class.
        /// </summary>
        /// <param name="vCardService">Service for generating vCard format strings</param>
        /// <param name="qrCodeService">Service for generating QR codes</param>
        /// <param name="logger">Logger instance</param>
        public VCardQrController(
            IVCardService vCardService,
            IQrCodeService qrCodeService,
            ILogger<VCardQrController> logger)
        {
            _vCardService = vCardService ?? throw new ArgumentNullException(nameof(vCardService));
            _qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Generates a QR code from vCard (contact) information.
        /// The QR code can be returned as an image file (PNG/JPG), a PDF document, or as Base64-encoded data in JSON.
        /// When scanned with a phone camera, the QR code will automatically add the contact to the phone's address book.
        /// </summary>
        /// <param name="request">Contact information to encode in the QR code. FirstName is required, all other fields are optional.</param>
        /// <param name="format">Output format for the QR code. Options:
        /// - "image" or "png": Returns PNG image file (default)
        /// - "jpg" or "jpeg": Returns JPEG image file
        /// - "pdf": Returns PDF document containing the QR code
        /// - "both": Returns JSON response with Base64-encoded PNG image and file size information</param>
        /// <returns>
        /// For image formats: Binary file download (PNG/JPG)
        /// For PDF format: PDF document download
        /// For "both" format: JSON object containing Base64 image data, file sizes, and contact information
        /// </returns>
        /// <response code="200">QR code generated successfully</response>
        /// <response code="400">Invalid request - missing required fields or invalid format parameter</response>
        /// <response code="500">Server error occurred during QR code generation</response>
        [HttpPost("generate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateQrCode(
            [FromBody] VCardRequest request,
            [FromQuery(Name = "format")] string? format = "image")
        {
            try
            {
                if (request == null)
                    return BadRequest(new { error = "Request body is required" });

                if (string.IsNullOrWhiteSpace(request.FirstName))
                    return BadRequest(new { error = "FirstName is required" });

                _logger.LogInformation("Generating QR code for contact: {FirstName} {LastName}", 
                    request.FirstName, request.LastName);

                // Generate vCard string
                string vCardData = _vCardService.GenerateVCardString(request);

                // Handle different format requests
                format = (format ?? "image").ToLowerInvariant();

                return format switch
                {
                    "image" or "png" => await GenerateImageResponse(vCardData, "PNG"),
                    "jpg" or "jpeg" => await GenerateImageResponse(vCardData, "JPG"),
                    "pdf" => await GeneratePdfResponse(vCardData, request),
                    "both" => await GenerateBothResponse(vCardData, request),
                    _ => BadRequest(new { error = "Invalid format. Valid options: 'image', 'png', 'jpg', 'jpeg', 'pdf', 'both'" })
                };
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid request: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code");
                // Return exception message for easier debugging in development/tests
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = ex.Message });
            }
        }

        /// <summary>
        /// Generates QR code and returns it as a downloadable image file.
        /// </summary>
        /// <param name="vCardData">Formatted vCard data to encode</param>
        /// <param name="imageFormat">Image format (PNG or JPG)</param>
        /// <returns>Image file for download</returns>
        private async Task<IActionResult> GenerateImageResponse(string vCardData, string imageFormat)
        {
            byte[] qrCodeImage = await _qrCodeService.GenerateQrCodeImageAsync(vCardData, imageFormat);
            string contentType = imageFormat.ToUpperInvariant() == "JPG" ? "image/jpeg" : "image/png";
            string fileName = $"contact_qr.{imageFormat.ToLowerInvariant()}";

            return File(qrCodeImage, contentType, fileName);
        }

        /// <summary>
        /// Generates QR code and returns it as a downloadable PDF file.
        /// </summary>
        /// <param name="vCardData">Formatted vCard data to encode</param>
        /// <param name="request">Contact information for the PDF filename</param>
        /// <returns>PDF file for download</returns>
        private async Task<IActionResult> GeneratePdfResponse(string vCardData, VCardRequest request)
        {
            string fileName = $"{request.FirstName}_{request.LastName ?? "contact"}.pdf".Replace(" ", "_");
            byte[] pdfBytes = await _qrCodeService.GenerateQrCodePdfAsync(vCardData, fileName);

            return File(pdfBytes, "application/pdf", fileName);
        }

        /// <summary>
        /// Generates both image and PDF formats, returning JSON with Base64-encoded image.
        /// Useful for web applications that need to display and download QR codes.
        /// </summary>
        /// <param name="vCardData">Formatted vCard data to encode</param>
        /// <param name="request">Contact information</param>
        /// <returns>JSON object containing Base64 image, file sizes, and contact summary</returns>
        private async Task<IActionResult> GenerateBothResponse(string vCardData, VCardRequest request)
        {
            // Generate both formats
            byte[] pngImage = await _qrCodeService.GenerateQrCodeImageAsync(vCardData, "PNG");
            byte[] pdfBytes = await _qrCodeService.GenerateQrCodePdfAsync(vCardData);

            // Convert image to Base64
            string base64Image = _qrCodeService.ConvertImageToBase64(pngImage);

            // Create temporary download IDs (in production, you might store these in a cache or database)
            string imageId = Guid.NewGuid().ToString();
            string pdfId = Guid.NewGuid().ToString();

            // For demo purposes, we'll return the Base64 directly
            // In production, you'd store these in a cache (like MemoryCache or Redis) with an expiration time
            var response = new
            {
                message = "QR code generated successfully",
                qrCode = new
                {
                    base64Image = $"data:image/png;base64,{base64Image}",
                    pngSize = pngImage.Length,
                    pdfSize = pdfBytes.Length
                },
                contact = new
                {
                    fullName = $"{request.FirstName} {request.LastName}".Trim(),
                    email = request.Email,
                    phone = request.Phone,
                    organization = request.Organization
                }
            };

            return Ok(response);
        }
    }
}
