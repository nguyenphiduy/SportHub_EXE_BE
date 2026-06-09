using System.Security.Cryptography;
using System.Text;

namespace BidaPlatform.Application.Common.Security;

public static class PasswordGenerator
{
    private const string Chars =
        "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$";

    public static string Generate(int length)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        var sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
            sb.Append(Chars[bytes[i] % Chars.Length]);

        return sb.ToString();
    }
}
