namespace BidaPlatform.Application.Common.Errors;

public static class AppErrorCode
{
    // Order
    public const string OrderNotFound = "";
    public const string OrderInvalidStatus = "";

    // Payment
    public const string PaymentAlreadyExists = "";
    public const string PaymentNotFound = "";

    // PayOS
    public const string PaymentGatewayFailed = "";
    public const string InvalidWebhookSignature = "";
}
