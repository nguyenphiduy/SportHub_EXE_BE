using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Infrastructure.Services;

public class VenueInsightPromptBuilder
{
    public static string BuildPrompt(
        Venue venue,
        List<BilliardSession> recentSessions,
        decimal totalRevenue,
        decimal avgRevenue,
        int activeTables,
        int totalTables)
    {
        var sessionSummary = BuildSessionSummary(recentSessions);
        var tableUtilization = CalculateTableUtilization(recentSessions, activeTables, totalTables);
        var peakHours = FindPeakHours(recentSessions);

        return $@"Bạn là chuyên gia phân tích kinh doanh bida. Hãy phân tích dữ liệu sau và đưa ra insights ngắn gọn bằng tiếng Việt.

THÔNG TIN QUÁN:
- Tên: {venue.Name}
- Địa chỉ: {venue.Address}
- Tổng số bàn: {totalTables}
- Số bàn đang hoạt động: {activeTables}

DOANH THU GẦN ĐÂY:
- Tổng doanh thu 30 phiên gần nhất: {totalRevenue:N0} VND
- Doanh thu trung bình mỗi phiên: {avgRevenue:N0} VND
- Số phiên đã chơi: {recentSessions.Count}

{sessionSummary}

TỶ LỆ SỬ DỤNG BÀN: {tableUtilization}

GIỜ CAO ĐIỂM: {peakHours}

Hãy phân tích và trả lời theo định dạng sau (chỉ trả về các dòng này, không thêm giải thích):

summary: [Tóm tắt 1-2 câu về tình hình kinh doanh tổng quan]
trend: [Xu hướng ngắn hạn: tăng/giảm/ổn định, kèm lý do]
recommendation: [1-2 khuyến nghị cụ thể để cải thiện doanh thu]
estimated: [Ước tính doanh thu kỳ tới, là một con số VND, không có chữ VND]
";
    }

    private static string BuildSessionSummary(List<BilliardSession> sessions)
    {
        if (sessions.Count == 0)
            return "Chưa có dữ liệu phiên chơi.";

        var completed = sessions.Count(s => s.Status == BilliardSessionStatus.Completed);
        var inProgress = sessions.Count(s => s.Status == BilliardSessionStatus.Active);
        var cashPayments = sessions.Count(s => s.PaymentMethod == BilliardPaymentMethod.Cash);
        var qrPayments = sessions.Count(s => s.PaymentMethod == BilliardPaymentMethod.BankTransfer);

        return $@"CHI TIẾT PHIÊN CHƠI:
- Hoàn thành: {completed}
- Đang chơi: {inProgress}
- Thanh toán tiền mặt: {cashPayments}
- Thanh toán QR: {qrPayments}";
    }

    private static string CalculateTableUtilization(List<BilliardSession> sessions, int activeTables, int totalTables)
    {
        if (totalTables == 0)
            return "0%";

        var utilizationRate = sessions.Count > 0
            ? (double)sessions.Count / (totalTables * 30) * 100
            : 0;

        return $"{Math.Min(utilizationRate, 100):F1}%";
    }

    private static string FindPeakHours(List<BilliardSession> sessions)
    {
        if (sessions.Count == 0)
            return "Chưa có đủ dữ liệu";

        var peakGroups = sessions
            .Where(s => s.StartTime.Hour is >= 17 and <= 22)
            .GroupBy(s => s.StartTime.Hour)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => $"{g.Key}:00 - {g.Key + 1}:00 ({g.Count()} phiên)")
            .ToList();

        return peakGroups.Count > 0
            ? string.Join(", ", peakGroups)
            : "Chưa có đủ dữ liệu";
    }
}
