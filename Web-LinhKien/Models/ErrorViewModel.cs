namespace Web_LinhKien.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public string? Message { get; set; } // Thêm thuộc tính Message để truyền thông báo lỗi tùy chỉnh
}