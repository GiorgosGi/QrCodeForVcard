using QrCodeForVcard.Models;
using System.Text;

namespace QrCodeForVcard.Services
{
    public interface IVCardService
    {
        string GenerateVCardString(VCardRequest vCardRequest);
    }

    public class VCardService : IVCardService
    {
        /// <summary>
        /// Generates a vCard format string from a VCardRequest
        /// Following RFC 6350 standard
        /// </summary>
        public string GenerateVCardString(VCardRequest vCardRequest)
        {
            if (string.IsNullOrWhiteSpace(vCardRequest.FirstName))
                throw new ArgumentException("FirstName is required", nameof(vCardRequest.FirstName));

            var sb = new StringBuilder();

            // vCard version and required fields
            sb.AppendLine("BEGIN:VCARD");
            sb.AppendLine("VERSION:3.0");

            // Full name
            string fullName = BuildFullName(vCardRequest.FirstName, vCardRequest.LastName);
            sb.AppendLine($"FN:{EscapeVCardString(fullName)}");

            // Name (N property: Last Name;First Name;Middle Name;Prefix;Suffix)
            sb.AppendLine($"N:{EscapeVCardString(vCardRequest.LastName ?? string.Empty)};{EscapeVCardString(vCardRequest.FirstName)};;;");

            // Organization
            if (!string.IsNullOrWhiteSpace(vCardRequest.Organization))
                sb.AppendLine($"ORG:{EscapeVCardString(vCardRequest.Organization)}");

            // Title/Role
            if (!string.IsNullOrWhiteSpace(vCardRequest.Title))
                sb.AppendLine($"TITLE:{EscapeVCardString(vCardRequest.Title)}");

            // Telephone
            if (!string.IsNullOrWhiteSpace(vCardRequest.Phone))
                sb.AppendLine($"TEL;TYPE=CELL:{EscapeVCardString(vCardRequest.Phone)}");

            // Email
            if (!string.IsNullOrWhiteSpace(vCardRequest.Email))
                sb.AppendLine($"EMAIL:{EscapeVCardString(vCardRequest.Email)}");

            // Address
            if (HasAddressData(vCardRequest))
            {
                string adr = $"ADR;;{EscapeVCardString(vCardRequest.Street ?? string.Empty)};" +
                    $"{EscapeVCardString(vCardRequest.City ?? string.Empty)};" +
                    $"{EscapeVCardString(vCardRequest.State ?? string.Empty)};" +
                    $"{EscapeVCardString(vCardRequest.PostalCode ?? string.Empty)};" +
                    $"{EscapeVCardString(vCardRequest.Country ?? string.Empty)}";
                sb.AppendLine(adr);
            }

            // Website
            if (!string.IsNullOrWhiteSpace(vCardRequest.Website))
                sb.AppendLine($"URL:{EscapeVCardString(vCardRequest.Website)}");

            // Notes
            if (!string.IsNullOrWhiteSpace(vCardRequest.Notes))
                sb.AppendLine($"NOTE:{EscapeVCardString(vCardRequest.Notes)}");

            // Timestamp
            sb.AppendLine($"REV:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");

            sb.AppendLine("END:VCARD");

            return sb.ToString();
        }

        private static string BuildFullName(string firstName, string? lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                return firstName.Trim();

            return $"{firstName.Trim()} {lastName.Trim()}";
        }

        private static bool HasAddressData(VCardRequest vCard)
        {
            return !string.IsNullOrWhiteSpace(vCard.Street) ||
                   !string.IsNullOrWhiteSpace(vCard.City) ||
                   !string.IsNullOrWhiteSpace(vCard.State) ||
                   !string.IsNullOrWhiteSpace(vCard.PostalCode) ||
                   !string.IsNullOrWhiteSpace(vCard.Country);
        }

        private static string EscapeVCardString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Escape special characters in vCard format
            return value
                .Replace("\\", "\\\\")
                .Replace(";", "\\;")
                .Replace(",", "\\,")
                .Replace("\n", "\\n")
                .Replace("\r", string.Empty);
        }
    }
}
