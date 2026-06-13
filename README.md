# vCard QR Code API

A .NET 10 ASP.NET Core API that generates QR codes from vCard (contact) information. The QR codes can be downloaded as PNG/JPG images or PDF files. When scanned with a phone camera, the QR code automatically adds the contact to the phone's address book.

## Features

✅ **Multiple Output Formats**
- PNG images (default)
- JPEG images
- PDF documents with QR code and instructions
- Base64-encoded JSON response for web integration

✅ **Comprehensive Contact Information**
- First Name (required)
- Last Name
- Email
- Phone Number
- Organization
- Job Title
- Full Address (Street, City, State, Postal Code, Country)
- Website URL
- Notes

✅ **vCard RFC 6350 Compliant**
- Generates standard vCard format that works with all phones and contact managers
- Proper escaping of special characters
- Complete address formatting

✅ **API Documentation**
- Swagger UI for interactive API testing
- Detailed endpoint documentation
- XML code comments

## Getting Started

### Prerequisites

- .NET 10 SDK or later
- Visual Studio 2026 or any .NET-compatible IDE

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd QrCodeForVcard
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run
```

The API will be available at `https://localhost:7154` (or the configured port).

## API Endpoints

### Base URL
```
https://localhost:7154/api/vcardqr
```

### Generate QR Code

**Endpoint:** `POST /api/vcardqr/generate`

**Query Parameters:**
- `format` (optional): Output format
  - `image` or `png` - Returns PNG image (default)
  - `jpg` or `jpeg` - Returns JPEG image
  - `pdf` - Returns PDF document
  - `both` - Returns JSON with Base64-encoded image

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1-555-0123",
  "organization": "Acme Corporation",
  "title": "Software Engineer",
  "street": "123 Main Street",
  "city": "New York",
  "state": "NY",
  "postalCode": "10001",
  "country": "USA",
  "website": "https://johndoe.com",
  "notes": "Available for consulting"
}
```

**Responses:**

#### Image Format (PNG/JPG)
- **Status:** 200 OK
- **Content-Type:** `image/png` or `image/jpeg`
- **Body:** Binary image file

Example:
```bash
curl -X POST "https://localhost:7154/api/vcardqr/generate?format=png" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"John","lastName":"Doe","email":"john@example.com"}' \
  -o contact_qr.png
```

#### PDF Format
- **Status:** 200 OK
- **Content-Type:** `application/pdf`
- **Body:** PDF file containing QR code with instructions

Example:
```bash
curl -X POST "https://localhost:7154/api/vcardqr/generate?format=pdf" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Jane","lastName":"Smith"}' \
  -o contact.pdf
```

#### JSON Format (Base64 Image)
- **Status:** 200 OK
- **Content-Type:** `application/json`
- **Body:** JSON object with Base64-encoded image

Example Response:
```json
{
  "message": "QR code generated successfully",
  "qrCode": {
	"base64Image": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA...",
	"pngSize": 2048,
	"pdfSize": 4096
  },
  "contact": {
	"fullName": "Alice Williams",
	"email": "alice.williams@tech.com",
	"phone": "+1-555-0999",
	"organization": "Digital Solutions LLC"
  }
}
```

## Error Handling

The API returns appropriate HTTP status codes and error messages:

| Status Code | Meaning | Example |
|-------------|---------|---------|
| 200 | Success | QR code generated successfully |
| 400 | Bad Request | Missing required `FirstName` field |
| 500 | Server Error | Error during QR code generation |

**Error Response Format:**
```json
{
  "error": "FirstName is required"
}
```

## Swagger UI

Access the interactive API documentation at:
```
https://localhost:7154
```

The Swagger UI provides:
- Interactive endpoint testing
- Request/response examples
- Parameter documentation
- Direct API calls from the browser

## Usage Examples

### Example 1: Generate PNG Image
```bash
curl -X POST "https://localhost:7154/api/vcardqr/generate?format=png" \
  -H "Content-Type: application/json" \
  -d '{
	"firstName": "John",
	"lastName": "Doe",
	"email": "john.doe@example.com",
	"phone": "+1-555-0123"
  }' \
  -o contact_qr.png
```

### Example 2: Generate PDF
```bash
curl -X POST "https://localhost:7154/api/vcardqr/generate?format=pdf" \
  -H "Content-Type: application/json" \
  -d '{
	"firstName": "Jane",
	"lastName": "Smith",
	"email": "jane.smith@example.com",
	"organization": "Tech Corp"
  }' \
  -o jane_smith.pdf
```

### Example 3: Get Base64 Image for Web Display
```bash
curl -X POST "https://localhost:7154/api/vcardqr/generate?format=both" \
  -H "Content-Type: application/json" \
  -d '{
	"firstName": "Alice",
	"lastName": "Williams",
	"email": "alice@example.com"
  }'
```

### Using with JavaScript/Frontend
```javascript
async function generateQRCode(contactInfo) {
  const response = await fetch('https://localhost:7154/api/vcardqr/generate?format=both', {
	method: 'POST',
	headers: {
	  'Content-Type': 'application/json',
	},
	body: JSON.stringify(contactInfo)
  });

  const data = await response.json();

  // Display the QR code
  document.getElementById('qrImage').src = data.qrCode.base64Image;

  return data;
}

// Usage
generateQRCode({
  firstName: 'John',
  lastName: 'Doe',
  email: 'john@example.com',
  phone: '+1-555-0123'
});
```

## NuGet Dependencies

- **QRCoder** - QR code generation
- **System.Drawing.Common** - Image handling
- **itext7** - PDF generation and manipulation
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI documentation
- **Microsoft.OpenApi** - OpenAPI specification support

## Project Structure

```
QrCodeForVcard/
├── Controllers/
│   └── VCardQrController.cs      # API endpoints
├── Models/
│   └── VCardRequest.cs            # Request/Response DTOs
├── Services/
│   ├── VCardService.cs            # vCard format generation
│   ├── IVCardService.cs           # vCard service interface
│   ├── QrCodeService.cs           # QR code generation
│   └── IQrCodeService.cs          # QR code service interface
├── Program.cs                     # Application configuration
├── QrCodeForVcard.csproj         # Project file
└── QrCodeForVcard.http           # HTTP test file
```

## Testing

### Using Visual Studio HTTP Test File

The project includes `QrCodeForVcard.http` with predefined test requests. Open this file in Visual Studio and use the "Send Request" links to test endpoints.

### Using Swagger UI

1. Run the application: `dotnet run`
2. Navigate to `https://localhost:7154`
3. Expand the `/api/vcardqr/generate` endpoint
4. Click "Try it out"
5. Enter contact information in the request body
6. Click "Execute"

### Using curl or Postman

See the "Usage Examples" section above for curl commands.

## Configuration

The API uses default settings from ASP.NET Core configuration. To customize:

1. Edit `appsettings.json` for production settings
2. Edit `appsettings.Development.json` for development settings

Example:
```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning"
	}
  }
}
```

## Security Considerations

⚠️ **For Production Deployment:**

1. **Enable HTTPS** - Always use HTTPS in production
2. **CORS Policy** - Configure CORS appropriately:
```csharp
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin", builder =>
	{
		builder.WithOrigins("https://yourdomain.com")
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});
```

3. **Rate Limiting** - Consider implementing rate limiting to prevent abuse
4. **Input Validation** - The API validates all inputs, but implement additional checks as needed
5. **Authentication** - Add API key or OAuth2 authentication if needed

## Performance Notes

- QR code generation is performed asynchronously
- Image and PDF generation is computationally efficient
- For high-traffic scenarios, consider adding caching or a message queue

## Troubleshooting

### Port Already in Use
If port 7154 is already in use, modify `Properties/launchSettings.json`:
```json
{
  "profiles": {
	"https": {
	  "commandName": "Project",
	  "dotnetRunMessages": true,
	  "launchBrowser": false,
	  "applicationUrl": "https://localhost:YOUR_PORT",
	  ...
	}
  }
}
```

### Certificate Issues
For development, trust the HTTPS certificate:
```bash
dotnet dev-certs https --trust
```

### Swagger Not Appearing
Ensure you're accessing the API in development mode. Swagger UI is only enabled when `app.Environment.IsDevelopment()` is true.

## Contributing

Contributions are welcome! Please submit pull requests or issues to the repository.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues or questions:
1. Check the Swagger UI documentation at the API root
2. Review the HTTP test file for example requests
3. Check the error messages returned by the API

## Version History

- **v1.0** - Initial release
  - QR code generation in multiple formats (PNG, JPEG, PDF)
  - Complete vCard RFC 6350 support
  - Swagger/OpenAPI documentation
  - Full contact information support
