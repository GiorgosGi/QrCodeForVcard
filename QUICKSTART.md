# Quick Start Guide - vCard QR Code API

## 🚀 Quick Setup (2 minutes)

### 1. Run the Application
```bash
cd QrCodeForVcard
dotnet run
```

Application will start at: `https://localhost:7154`

### 2. Open Swagger UI
Navigate to: `https://localhost:7154`

You'll see the interactive Swagger documentation with all endpoints.

## 📝 Generate Your First QR Code

### Option A: Using Swagger UI (Easiest)

1. Open Swagger UI in your browser
2. Click on `POST /api/vcardqr/generate`
3. Click "Try it out"
4. Replace the example JSON with:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "phone": "+1-555-0123"
}
```
5. Add `?format=png` to the URL (or choose from format dropdown)
6. Click "Execute"
7. Download the PNG image

### Option B: Using PowerShell

```powershell
$body = @{
	firstName = "John"
	lastName = "Doe"
	email = "john@example.com"
	phone = "+1-555-0123"
} | ConvertTo-Json

Invoke-WebRequest -Uri "https://localhost:7154/api/vcardqr/generate?format=png" `
	-Method POST `
	-ContentType "application/json" `
	-Body $body `
	-OutFile "contact.png"
```

### Option C: Using curl

```bash
curl -X POST "https://localhost:7154/api/vcardqr/generate?format=png" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"John","lastName":"Doe","email":"john@example.com","phone":"+1-555-0123"}' \
  -o contact.png
```

## 📦 Output Formats

### Get PNG Image
```
GET /api/vcardqr/generate?format=png
```
Returns: Downloadable PNG file

### Get PDF Document
```
GET /api/vcardqr/generate?format=pdf
```
Returns: PDF with QR code and instructions

### Get JSON with Base64
```
GET /api/vcardqr/generate?format=both
```
Returns: JSON with Base64-encoded image (great for web)

## 🔗 Using in a Web Application

### HTML + JavaScript Example

```html
<!DOCTYPE html>
<html>
<head>
	<title>QR Code Generator</title>
	<style>
		body { font-family: Arial; max-width: 600px; margin: 50px auto; }
		input, textarea { width: 100%; padding: 8px; margin: 5px 0; }
		button { padding: 10px 20px; background: #007bff; color: white; border: none; cursor: pointer; }
		#qrCode { margin-top: 20px; text-align: center; }
		img { max-width: 300px; border: 1px solid #ccc; }
	</style>
</head>
<body>
	<h1>Generate Contact QR Code</h1>
	<form id="contactForm">
		<input type="text" id="firstName" placeholder="First Name" required>
		<input type="text" id="lastName" placeholder="Last Name">
		<input type="email" id="email" placeholder="Email">
		<input type="tel" id="phone" placeholder="Phone">
		<input type="text" id="organization" placeholder="Organization">
		<button type="button" onclick="generateQR()">Generate QR Code</button>
	</form>

	<div id="qrCode"></div>

	<script>
		async function generateQR() {
			const contactInfo = {
				firstName: document.getElementById('firstName').value,
				lastName: document.getElementById('lastName').value,
				email: document.getElementById('email').value,
				phone: document.getElementById('phone').value,
				organization: document.getElementById('organization').value
			};

			try {
				const response = await fetch('https://localhost:7154/api/vcardqr/generate?format=both', {
					method: 'POST',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(contactInfo)
				});

				const data = await response.json();

				document.getElementById('qrCode').innerHTML = 
					`<h2>Your QR Code:</h2>
					 <img src="${data.qrCode.base64Image}" alt="QR Code">
					 <p>Scan with your phone camera to add contact</p>`;
			} catch (error) {
				alert('Error: ' + error.message);
			}
		}
	</script>
</body>
</html>
```

## 📋 Contact Fields Reference

| Field | Type | Required | Example |
|-------|------|----------|---------|
| firstName | string | ✅ Yes | "John" |
| lastName | string | ❌ No | "Doe" |
| email | string | ❌ No | "john@example.com" |
| phone | string | ❌ No | "+1-555-0123" |
| organization | string | ❌ No | "Acme Corp" |
| title | string | ❌ No | "Engineer" |
| street | string | ❌ No | "123 Main St" |
| city | string | ❌ No | "New York" |
| state | string | ❌ No | "NY" |
| postalCode | string | ❌ No | "10001" |
| country | string | ❌ No | "USA" |
| website | string | ❌ No | "https://johndoe.com" |
| notes | string | ❌ No | "Available for consulting" |

## ✅ Test the API

## 🎯 Common Use Cases

### 1. Business Card QR Code
```json
{
  "firstName": "Sarah",
  "lastName": "Johnson",
  "email": "sarah@company.com",
  "phone": "+1-555-9876",
  "organization": "TechCorp",
  "title": "Product Manager",
  "street": "456 Business Ave",
  "city": "San Francisco",
  "state": "CA",
  "country": "USA",
  "website": "https://techcorp.com"
}
```

### 2. Simple Contact
```json
{
  "firstName": "John",
  "phone": "+1-555-0123"
}
```

### 3. Professional Card
```json
{
  "firstName": "Dr.",
  "lastName": "Smith",
  "email": "doctor@clinic.com",
  "phone": "+1-555-5555",
  "organization": "City Medical Clinic",
  "title": "Physician",
  "street": "789 Health Plaza",
  "city": "Boston",
  "state": "MA",
  "country": "USA"
}
```

## 🛠️ Development Tips

- **Test File**: Use `QrCodeForVcard.http` in Visual Studio for quick testing
- **Documentation**: Full API docs available at `/swagger`
- **XML Comments**: All code is documented with XML comments
- **Async**: All operations are async for better performance

## ⚙️ Troubleshooting

**Q: Getting SSL certificate error?**
```bash
dotnet dev-certs https --trust
```

**Q: Port 7154 already in use?**
Edit `Properties/launchSettings.json` and change the port number.

**Q: Not seeing Swagger UI?**
Make sure you're accessing it while in Development mode.

## 📚 More Information

See `README.md` for:
- Detailed API documentation
- Deployment instructions
- Security considerations
- Advanced configuration

## 🎓 Learn More

- vCard Format: https://en.wikipedia.org/wiki/VCard
- QRCoder Library: https://github.com/codebude/QRCoder
- Swagger/OpenAPI: https://swagger.io/

---

**Need help?** Check the Swagger UI or review the HTTP test file for more examples!
