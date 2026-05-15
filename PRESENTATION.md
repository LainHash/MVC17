# Bản Thuyết Trình Dự Án MVC17

## Ứng Dụng Quản Lý & Bán Hàng Laptop Trực Tuyến

---

## 📋 I. GIỚI THIỆU DỰ ÁN

### 1. Tên Dự Án

**MVC17** - Hệ thống Quản lý và Bán Hàng Laptop Trực Tuyến

### 2. Mô Tả Dự Án

Ứng dụng ASP.NET Core MVC hiện đại được xây dựng nhằm:

- 🛍️ Cung cấp nền tảng bán laptop trực tuyến toàn diện
- 📦 Quản lý kho hàng, đơn hàng và khách hàng
- 👥 Hỗ trợ xác thực người dùng an toàn bằng JWT
- 📊 Cung cấp thống kê kinh doanh chi tiết cho quản trị viên
- 🛒 Hỗ trợ giỏ hàng và quy trình thanh toán

### 3. Mục Tiêu Dự Án

- ✅ Tạo trải nghiệm mua sắm liền mạch cho khách hàng
- ✅ Cung cấp công cụ quản lý toàn diện cho quản trị viên
- ✅ Tích hợp hệ thống quản lý đơn hàng chuyên nghiệp
- ✅ Đảm bảo bảo mật dữ liệu người dùng

---

## 👥 II. HÀNH TRÌNH KHÁCH HÀNG: TỪ CHƯA ĐĂNG NHẬP ĐẾN ĐĂNG NHẬP

Phần này trình bày chi tiết trải nghiệm khách hàng từ lần truy cập đầu tiên (chưa có tài khoản) cho đến khi đã xác thực thành công, được tổ chức theo từng View.

### 1. GIAI ĐOẠN 1: KHÁCH HÀNG CHƯA ĐĂNG NHẬP

Khách hàng chưa xác thực hoàn toàn có thể:

- ✅ Xem trang chủ với sản phẩm nổi bật
- ✅ Duyệt danh sách sản phẩm
- ✅ Xem chi tiết sản phẩm
- ❌ KHÔNG thể thanh toán hoặc tạo đơn hàng
- ❌ KHÔNG thể xem lịch sử mua hàng
- ❌ KHÔNG thể truy cập hồ sơ cá nhân

#### View 1.1: **Home/Index** - Trang Chủ

```
📌 Vị trí: /Home/Index (hoặc /)
🎯 Mục đích: Cánh cửa vào chính của ứng dụng
👥 Truy cập: Tất cả (authenticated & unauthenticated)

Thành phần chính:
├── Carousel/Banner - Quảng cáo, khuyến mãi
├── Trending Products (TrendingProduct ViewComponent)
│   └── Hiển thị 6 sản phẩm bán chạy nhất
├── Featured Products
│   └── Danh sách sản phẩm nổi bật
├── Blog & News (_BlogNews.cshtml partial)
│   └── Tin tức, cập nhật hàng mới
├── About Section (_About.cshtml partial)
│   └── Giới thiệu công ty
├── Team & Support (_TeamSupport.cshtml partial)
│   └── Đội ngũ hỗ trợ khách hàng
└── Navigation Links
    ├── Browse Products (→ Product/Index)
    ├── Login (→ Account/Login)
    └── Register (→ Account/Register)

Hành động có sẵn:
→ Nhấp vào sản phẩm để xem chi tiết (Product/Details/{id})
→ Nhấp "Đăng nhập" hoặc "Đăng ký" để xác thực
→ Duyệt các danh mục sản phẩm
```

#### View 1.2: **Product/Index** - Danh Sách Sản Phẩm

```
📌 Vị trí: /Product/Index
🎯 Mục đích: Hiển thị tất cả sản phẩm có sẵn
👥 Truy cập: Tất cả người dùng

Thành phần chính:
├── Filter Dropdown (FilterDropdown ViewComponent)
│   ├── Lọc theo danh mục (Category)
│   ├── Lọc theo nhà cung cấp (Supplier)
│   ├── Lọc theo khoảng giá
│   └── Sắp xếp (giá, tên, mới nhất)
├── Product Grid (_ProductGrid.cshtml partial)
│   ├── Hiển thị 12 sản phẩm/trang (mặc định)
│   ├── Mỗi sản phẩm hiển thị:
│   │   ├── Hình ảnh
│   │   ├── Tên sản phẩm
│   │   ├── Giá
│   │   ├── Đánh giá
│   │   ├── Nút "Xem Chi Tiết"
│   │   └── Nút "Thêm Giỏ Hàng" (chỉ khi đã login)
│   └── [Phân trang] (Pagination ViewComponent)
│       ├── Previous/Next buttons
│       ├── Số trang
│       └── "Trang X/Y"

Hành động có sẵn:
→ Lọc/sắp xếp sản phẩm
→ Nhấp vào sản phẩm xem chi tiết (Product/Details/{id})
→ Phân trang để xem sản phẩm khác
→ KHÔNG thể thêm giỏ hàng (không đăng nhập)
```

#### View 1.3: **Product/Details/{id}** - Chi Tiết Sản Phẩm

```
📌 Vị trí: /Product/Details/{id}
🎯 Mục đích: Hiển thị thông tin chi tiết về 1 sản phẩm
👥 Truy cập: Tất cả người dùng

Thành phần chính:
├── Hình ảnh sản phẩm (Gallery)
│   ├── Hình chính
│   └── Hình phụ (thumbnails)
├── Thông tin cơ bản
│   ├── Tên sản phẩm
│   ├── Giá hiện tại
│   ├── Giá cũ (nếu có discount)
│   ├── Tình trạng kho (In stock / Out of stock)
│   ├── Nhà cung cấp
│   ├── Danh mục
│   └── Đánh giá trung bình
├── Thông số kỹ thuật (ProductSpec ViewComponent)
│   ├── CPU: Model, Speed (GHz), Cores
│   ├── GPU: Model, VRAM (GB)
│   ├── RAM: Dung lượng (GB), Type
│   ├── Storage: Dung lượng (GB/TB), Type
│   └── Khác: Màn hình, Trọng lượng, OS
├── Mô tả sản phẩm (full HTML)
├── Bảng so sánh (nếu có)
├── Nhận xét/Review (nếu được bật)
└── Hành động
    ├── Nút "Thêm Giỏ Hàng" (disabled nếu chưa login)
    ├── Nút "Quay Lại"
    └── Nút "Tiếp Tục Mua Sắm"

Hành động có sẵn:
→ Xem chi tiết thông số kỹ thuật
→ Đọc mô tả sản phẩm
→ Nếu chưa đăng nhập: Nhấp "Thêm Giỏ" → Yêu cầu đăng nhập
→ Quay lại danh sách sản phẩm
→ Tiếp tục duyệt sản phẩm khác
```

#### View 1.4: **Home/Privacy** - Trang Chính Sách

```
📌 Vị trí: /Home/Privacy
🎯 Mục đích: Hiển thị chính sách bảo mật
👥 Truy cập: Tất cả người dùng

Thành phần chính:
├── Tiêu đề "Chính Sách Bảo Mật"
├── Nội dung chính sách
│   ├── Cách sử dụng dữ liệu
│   ├── Bảo vệ thông tin cá nhân
│   ├── Cookie policy
│   ├── Liên hệ support
│   └── Ngày cập nhật cuối
└── Nút "Chấp nhận" / "Quay Lại"

Hành động có sẵn:
→ Đọc và hiểu chính sách
→ Quay lại trang chủ
```

### 2. GIAI ĐOẠN 2: QUẢN LÝ ĐĂNG NHẬP / ĐĂNG KÝ

Khi khách hàng quyết định mua hàng, họ phải xác thực tài khoản.

#### View 2.1: **Account/Login** - Trang Đăng Nhập

```
📌 Vị trí: /Account/Login
🎯 Mục đích: Xác thực tài khoản có sẵn
👥 Truy cập: Chỉ người chưa đăng nhập (unauthenticated)

Thành phần chính:
├── Tiêu đề "Đăng Nhập"
├── Form Đăng Nhập
│   ├── Input Email
│   │   └── Type: email, Required: true, Placeholder: "your@email.com"
│   ├── Input Mật Khẩu
│   │   └── Type: password, Required: true, Placeholder: "Mật khẩu"
│   ├── Checkbox "Nhớ Tôi" (Remember Me)
│   ├── Nút "Đăng Nhập" (Primary button)
│   └── Nút "Quay Lại" / "Hủy"
├── Error Messages (nếu có)
│   ├── "Email hoặc mật khẩu không chính xác"
│   ├── "Tài khoản chưa xác nhận email"
│   └── "Tài khoản đã bị khóa"
├── Links bổ sung
│   ├── "Quên mật khẩu?" (nếu được bật)
│   ├── "Chưa có tài khoản? Đăng ký ngay"
│   └── "Quay về trang chủ"
├── Social Login (nếu được bật)
│   ├── "Đăng nhập với Google"
│   ├── "Đăng nhập với Facebook"
│   └── "Đăng nhập với Microsoft"
└── Footer
    └── "Liên hệ support nếu có vấn đề"

Quy trình Đăng Nhập:
1. Khách hàng nhập email và mật khẩu
2. Server kiểm tra email trong database
3. Server so sánh mật khẩu (BCrypt verify)
4. Nếu hợp lệ:
   ├── Kiểm tra tài khoản có active không
   ├── Tạo JWT Token
   ├── Lưu token trong HttpOnly Cookie
   ├── Merge cart từ Session vào database
   └── Redirect đến Home/Index (đã login)
5. Nếu sai:
   └── Hiển thị error message, yêu cầu nhập lại

Hành động có sẵn:
→ Nhập thông tin đăng nhập
→ Nhấp "Đăng Nhập" để xác thực
→ Chuyển đến trang "Đăng Ký" (Account/Register)
→ Quay lại trang chủ
```

#### View 2.2: **Account/Register** - Trang Đăng Ký

```
📌 Vị trí: /Account/Register
🎯 Mục đích: Tạo tài khoản mới
👥 Truy cập: Chỉ người chưa đăng nhập (unauthenticated)

Thành phần chính:
├── Tiêu đề "Đăng Ký Tài Khoản"
├── Form Đăng Ký (phần 1: Thông tin cơ bản)
│   ├── Input Email
│   │   └── Type: email, Required: true
│   ├── Input Mật Khẩu
│   │   └── Type: password, Required: true, Min length: 8
│   ├── Input Xác Nhận Mật Khẩu
│   │   └── Type: password, Required: true, Must match password
│   ├── Input Họ Tên
│   │   └── Type: text, Required: true
│   ├── Input Số Điện Thoại
│   │   └── Type: tel, Required: false
│   └── Checkbox "Tôi đồng ý với Điều Khoản"
│       └── Link đến Điều khoản dịch vụ
│
├── Form Đăng Ký (phần 2: Thông tin địa chỉ)
│   ├── Select Quốc Gia
│   │   └── Options từ UserProfileConstants.Countries
│   ├── Select Tỉnh/Thành Phố
│   │   └── Options từ UserProfileConstants.Cities
│   ├── Input Địa Chỉ Chi Tiết
│   │   └── Type: text, Required: false
│   ├── Input Mã ZIP/Postal Code
│   │   └── Type: text, Required: false
│   └── Input Ngày Sinh
│       └── Type: date, Required: false
│
├── Nút "Đăng Ký" (Primary button)
├── Nút "Quay Lại" (Secondary button)
├── Messages
│   ├── Error messages (validation errors)
│   ├── "Email đã được sử dụng"
│   ├── "Mật khẩu không đủ mạnh"
│   └── "Vui lòng điền đầy đủ thông tin bắt buộc"
├── Links bổ sung
│   ├── "Đã có tài khoản? Đăng nhập"
│   └── "Quay về trang chủ"
└── Footer
    └── "Chúng tôi sẽ gửi email xác nhận đến hộp thư của bạn"

Quy trình Đăng Ký:
1. Khách hàng điền thông tin form
2. Client-side validation (JavaScript)
3. Server nhận dữ liệu
4. Kiểm tra email đã tồn tại chưa
5. Kiểm tra mật khẩu độ mạnh
6. Nếu hợp lệ:
   ├── Tạo đối tượng User mới
   ├── Hash mật khẩu bằng BCrypt
   ├── Lưu vào database
   ├── Tạo JWT Token
   ├── Gửi email xác nhận (đến User.Email)
   ├── Lưu token trong Cookie
   └── Redirect đến Home/Index (đã login, nhưng chưa verify email)
7. Nếu không hợp lệ:
   └── Hiển thị lỗi, yêu cầu nhập lại

Email xác nhận gồm:
├── Chào mừng đến MVC17
├── Link xác nhận email (JWT token)
├── Hướng dẫn: "Nhấp link để kích hoạt tài khoản"
├── Thời hạn: 24 giờ
└── Liên hệ support nếu có vấn đề

Hành động có sẵn:
→ Điền thông tin đăng ký
→ Nhấp "Đăng Ký" để tạo tài khoản
→ Chuyển đến trang "Đăng Nhập" (Account/Login)
→ Quay lại trang chủ
```

### 3. GIAI ĐOẠN 3: SAU KHI ĐĂNG NHẬP THÀNH CÔNG

Sau khi xác thực thành công, khách hàng nhận được JWT token và có thể truy cập các tính năng mua hàng.

#### View 3.1: **Home/Index (Authenticated)** - Trang Chủ (Đã Đăng Nhập)

```
📌 Vị trí: /Home/Index
🎯 Mục đích: Trang chủ cho khách hàng đã xác thực
👥 Truy cập: Người dùng đã login (authenticated)

Thay đổi so với trước:
├── Navigation Bar cập nhật
│   ├── Ẩn: "Đăng Nhập" / "Đăng Ký"
│   ├── Hiện: "Hồ Sơ" / "Giỏ Hàng" / "Đơn Hàng" / "Đăng Xuất"
│   ├── Hiển thị tên người dùng
│   └── Dropdown menu (Profile, Settings, Logout)
├── Cart Icon
│   ├── Số lượng sản phẩm trong giỏ
│   └── Link đến Cart/Index
├── Recommendations
│   ├── Dựa trên lịch sử duyệt
│   ├── Dựa trên lịch sử mua hàng
│   └── Similar products
└── Personalized content
    ├── "Chào mừng, [Tên Khách Hàng]!"
    ├── Danh sách sản phẩm đã xem
    └── Khuyến mãi riêng cho bạn

Hành động có sẵn:
→ Duyệt sản phẩm bình thường
→ Thêm sản phẩm vào giỏ hàng (hiện không disabled)
→ Xem giỏ hàng (Cart/Index)
→ Xem hồ sơ cá nhân (Account/Profile)
→ Xem lịch sử đơn hàng (Account/OrderHistory hoặc Order/History)
→ Đăng xuất (Account/Logout)
```

#### View 3.2: **Product/Index (Authenticated)** - Danh Sách Sản Phẩm (Đã Login)

```
📌 Vị trí: /Product/Index
🎯 Mục đích: Danh sách sản phẩm cho khách hàng đã xác thực
👥 Truy cập: Người dùng đã login

Thay đổi so với trước:
├── Nút "Thêm Giỏ Hàng" HIỆN và ENABLE
│   ├── Nhấp → Hiển thị modal xác nhận số lượng
│   └── Nhấp "Thêm" → Gửi request POST /Cart/AddToCart
├── Nút "So Sánh" (nếu có)
│   ├── Cho phép so sánh nhiều sản phẩm
│   └── Hiển thị bảng so sánh chi tiết
├── Nút "Yêu Thích/Wishlist" (nếu được bật)
│   └── Lưu sản phẩm yêu thích
├── Notification Toast
│   ├── Khi thêm giỏ: "Đã thêm 1 sản phẩm vào giỏ hàng!"
│   └── Hiển thị số sản phẩm hiện tại trong giỏ
└── Giỏ hàng icon cập nhật
    └── Hiển thị số lượng real-time

Hành động có sẵn:
→ Lọc/sắp xếp sản phẩm
→ Xem chi tiết sản phẩm
→ Thêm sản phẩm vào giỏ (POST /Cart/AddToCart)
→ So sánh sản phẩm (nếu có)
→ Thêm vào wishlist (nếu có)
→ Phân trang
```

#### View 3.3: **Cart/Index** - Giỏ Hàng

```
📌 Vị trí: /Cart/Index
🎯 Mục đích: Xem và quản lý giỏ hàng
👥 Truy cập: CHỈ người dùng đã login ([Authorize])

Thành phần chính:
├── Tiêu đề "Giỏ Hàng"
├── Bảng Chi Tiết Giỏ Hàng
│   ├── Cột: Sản Phẩm (hình + tên)
│   ├── Cột: Giá
│   ├── Cột: Số Lượng
│   │   ├── Nút "-" để giảm
│   │   ├── Input số lượng
│   │   └── Nút "+" để tăng
│   ├── Cột: Thành Tiền (giá × số lượng)
│   ├── Cột: Hành Động
│   │   ├── Nút "Xóa" (trash icon)
│   │   └── Nút "Lưu" để cập nhật số lượng
│   └── Mỗi hàng:
│       ├── Hình ảnh sản phẩm (thumbnail)
│       ├── Tên sản phẩm (có link đến Details)
│       ├── Thông số sản phẩm nếu cần
│       └── Giá/Thành tiền tính toán
│
├── Thông Tin Tóm Tắt (Summary)
│   ├── Tổng số sản phẩm
│   ├── Tổng cộng (subtotal)
│   ├── Phí vận chuyển (Shipping fee)
│   ├── Mã giảm giá (Discount code)
│   │   ├── Input nhập mã
│   │   └── Nút "Áp dụng"
│   ├── Thuế (Tax)
│   └── TỔNG CỘNG PHẢI TRẢ (Grand Total) [Highlight]
│
├── Hành Động
│   ├── Nút "Tiếp Tục Mua Sắm" (→ Product/Index)
│   ├── Nút "Cập Nhật Giỏ" (POST /Cart/UpdateQuantity)
│   ├── Nút "Thanh Toán" (→ Cart/Checkout hoặc Order/Create)
│   └── Nút "Xóa Tất Cả" (POST /Cart/ClearCart)
│
├── Thông Báo
│   ├── Nếu giỏ trống: "Giỏ hàng của bạn trống, hãy thêm sản phẩm!"
│   ├── Nếu cập nhật thành công: "Cập nhật giỏ hàng thành công!"
│   └── Nếu xóa thành công: "Đã xóa sản phẩm khỏi giỏ hàng"
│
└── Quy Tắc Giỏ Hàng
    ├── Lưu trữ: Session + Database (sau login merge)
    ├── Cập nhật: Real-time AJAX
    └── Tự động tính lại: subtotal, tax, total

Hành động có sẵn:
→ Xem danh sách sản phẩm trong giỏ
→ Cập nhật số lượng (POST /Cart/UpdateQuantity)
→ Xóa sản phẩm (POST /Cart/RemoveItem)
→ Áp dụng mã giảm giá
→ Tiếp tục mua sắm
→ Thanh toán (Cart/Checkout)
```

#### View 3.4: **Order/Create hoặc Checkout** - Thanh Toán / Tạo Đơn Hàng

```
📌 Vị trí: /Order/Create hoặc /Cart/Checkout
🎯 Mục đích: Hoàn thành đơn hàng
👥 Truy cập: CHỈ người dùng đã login

Thành phần chính:
├── Tiêu đề "Hoàn Tất Đơn Hàng"
├── Phần 1: Thông Tin Giao Hàng
│   ├── Section "Địa Chỉ Giao Hàng"
│   ├── Input Họ Tên Người Nhận
│   ├── Input Số Điện Thoại
│   ├── Input Địa Chỉ Chi Tiết
│   ├── Select Tỉnh/Thành Phố
│   ├── Input Mã ZIP
│   ├── Checkbox "Sử Dụng Địa Chỉ Từ Hồ Sơ"
│   └── Nút "Chọn Địa Chỉ Từ Danh Sách Có Sẵn" (nếu có)
│
├── Phần 2: Thông Tin Đơn Hàng
│   ├── Section "Đơn Hàng"
│   ├── Danh sách sản phẩm (read-only)
│   │   └── Tương tự Cart/Index nhưng không edit được
│   ├── Tổng tiền hàng
│   ├── Phí vận chuyển (tính dựa trên địa chỉ)
│   ├── Mã giảm giá (nếu áp dụng)
│   └── TỔNG THANH TOÁN
│
├── Phần 3: Phương Thức Thanh Toán
│   ├── Radio "Thanh Toán Khi Nhận Hàng (COD)"
│   ├── Radio "Chuyển Khoản Ngân Hàng"
│   ├── Radio "Ví Điện Tử / E-wallet" (nếu có)
│   ├── Radio "Thẻ Tín Dụng" (nếu có)
│   └── Input Ghi Chú Đơn Hàng (textarea)
│
├── Phần 4: Xác Nhận & Điều Khoản
│   ├── Checkbox "Tôi đã đọc và đồng ý với Điều Khoản & Chính Sách"
│   ├── Checkbox "Gửi cho tôi thông tin khuyến mãi"
│   └── Link "Đọc Điều Khoản" (mở modal hoặc trang mới)
│
├── Nút Hành Động
│   ├── Nút "Đặt Hàng" (Primary, size large) [POST /Order/Create]
│   ├── Nút "Quay Lại Giỏ Hàng"
│   └── Nút "Tiếp Tục Mua Sắm"
│
└── Validation & Feedback
    ├── Client-side validation real-time
    ├── Error messages dưới mỗi input
    └── Loading spinner khi submit

Quy trình Tạo Đơn Hàng:
1. Khách hàng điền thông tin giao hàng
2. Chọn phương thức thanh toán
3. Đồng ý điều khoản
4. Nhấp "Đặt Hàng"
5. Server:
   ├── Validate dữ liệu
   ├── Tạo Invoice (OrderController.Create())
   ├── Tạo InvoiceDetails từ giỏ hàng
   ├── Cập nhật kho (ProductSku)
   ├── Xóa giỏ hàng (sau khi thành công)
   ├── Gửi email xác nhận đơn hàng
   └── Redirect Order/Details/{invoiceId}
6. Hiển thị: Trang xác nhận đơn hàng

Hành động có sẵn:
→ Điền/chọn thông tin giao hàng
→ Chọn phương thức thanh toán
→ Xem tóm tắt đơn hàng
→ Đặt hàng (POST /Order/Create)
→ Quay lại chỉnh sửa giỏ hàng nếu cần
```

#### View 3.5: **Order/Details/{id}** - Chi Tiết Đơn Hàng

```
📌 Vị trí: /Order/Details/{invoiceId}
🎯 Mục đích: Hiển thị chi tiết đơn hàng đã tạo
👥 Truy cập: Chủ nhân đơn hàng hoặc admin

Thành phần chính:
├── Header
│   ├── Tiêu đề "Chi Tiết Đơn Hàng"
│   ├── Mã Đơn Hàng (Invoice ID)
│   ├── Trạng Thái Đơn Hàng (Badge)
│   │   ├── 🟡 Đang chờ xử lý (Pending)
│   │   ├── 🔵 Đang vận chuyển (Shipping)
│   │   ├── 🟢 Hoàn thành (Completed)
│   │   ├── 🔴 Đã hủy (Cancelled)
│   │   └── ⚫ Hoàn tiền (Refunded)
│   └── Ngày tạo đơn hàng
│
├── Phần 1: Thông Tin Giao Hàng
│   ├── Tên người nhận
│   ├── Số điện thoại
│   ├── Địa chỉ giao hàng
│   ├── Tỉnh/Thành phố
│   ├── Mã ZIP
│   └── Ngày dự kiến giao
│
├── Phần 2: Danh Sách Sản Phẩm
│   └── Bảng
│       ├── Cột: Sản Phẩm (tên + SKU)
│       ├── Cột: Giá đơn vị
│       ├── Cột: Số lượng
│       └── Cột: Thành tiền
│
├── Phần 3: Tóm Tắt Thanh Toán
│   ├── Tổng tiền hàng (Subtotal)
│   ├── Phí vận chuyển (Shipping)
│   ├── Mã giảm giá đã áp dụng
│   ├── Thuế (Tax)
│   └── TỔNG THANH TOÁN (Grand Total) [Bold, Large]
│
├── Phần 4: Thông Tin Thanh Toán
│   ├── Phương thức thanh toán
│   ├── Trạng thái thanh toán (Paid/Unpaid/Pending)
│   └── Ghi chú thanh toán (nếu có)
│
├── Phần 5: Lịch Sử Trạng Thái
│   ├── Timeline
│   ├── Ngày/Giờ tạo
│   ├── Ngày/Giờ confirm
│   ├── Ngày/Giờ ship
│   ├── Ngày/Giờ hoàn thành
│   └── Ghi chú mỗi trạng thái (nếu có)
│
├── Hành Động
│   ├── Nút "In Hóa Đơn" (PDF)
│   ├── Nút "Theo Dõi Vận Chuyển"
│   ├── Nút "Liên Hệ Support"
│   └── Dựa trên trạng thái:
│       ├── Nếu Pending: Nút "Hủy Đơn Hàng"
│       ├── Nếu Completed: Nút "Trả Hàng / Hoàn Tiền"
│       └── Nếu Cancelled/Refunded: Nút "Mua Lại" (thêm lại vào giỏ)
│
└── Footer
    ├── Nút "Quay Lại Danh Sách Đơn Hàng"
    └── Nút "Tiếp Tục Mua Sắm"

Hành động có sẵn:
→ Xem chi tiết đơn hàng
→ In hóa đơn (PDF)
→ Theo dõi vận chuyển
→ Hủy đơn hàng (nếu Pending)
→ Trả hàng/hoàn tiền (nếu Completed)
→ Quay lại danh sách đơn hàng
```

#### View 3.6: **Account/OrderHistory** - Lịch Sử Đơn Hàng

```
📌 Vị trí: /Account/OrderHistory hoặc /Order/History
🎯 Mục đích: Xem tất cả đơn hàng của khách hàng
👥 Truy cập: CHỈ người dùng đã login ([Authorize])

Thành phần chính:
├── Tiêu đề "Lịch Sử Đơn Hàng"
├── Filter/Search
│   ├── Input tìm kiếm (mã đơn hàng, tên sản phẩm)
│   ├── Select lọc theo trạng thái
│   │   ├── Tất cả (All)
│   │   ├── Đang chờ (Pending)
│   │   ├── Đang vận chuyển (Shipping)
│   │   ├── Hoàn thành (Completed)
│   │   ├── Đã hủy (Cancelled)
│   │   └── Hoàn tiền (Refunded)
│   ├── Date range picker (từ ngày - đến ngày)
│   └── Nút "Tìm Kiếm" / "Reset"
│
├── Bảng Danh Sách Đơn Hàng
│   └── Cột:
│       ├── Mã Đơn Hàng (link → Order/Details)
│       ├── Ngày Đặt Hàng
│       ├── Tổng Tiền
│       ├── Trạng Thái (Badge màu)
│       ├── Số Sản Phẩm
│       ├── Ngày Dự Kiến Giao
│       └── Hành Động
│           ├── Nút "Chi Tiết"
│           ├── Nút "In Hóa Đơn"
│           └── Nút "Theo Dõi"
│
├── Phân Trang
│   └── Hiển thị 10/20/50 đơn hàng per page
│
├── Thống Kê
│   ├── Tổng số đơn hàng
│   ├── Tổng chi tiêu
│   ├── Đơn hàng hoàn thành
│   └── Đơn hàng đang chờ
│
└── Hành Động Bổ Sung
    ├── Nút "Tiếp Tục Mua Sắm" (→ Product/Index)
    ├── Nút "Yêu Cầu Hỗ Trợ"
    └── Nút "Quay Về Hồ Sơ" (Account/Profile)

Hành động có sẵn:
→ Tìm kiếm đơn hàng theo mã hoặc sản phẩm
→ Lọc theo trạng thái
→ Lọc theo khoảng thời gian
→ Xem chi tiết đơn hàng (Order/Details)
→ In hóa đơn (PDF)
→ Theo dõi vận chuyển
→ Phân trang
```

#### View 3.7: **Account/Profile** - Hồ Sơ Cá Nhân

```
📌 Vị trí: /Account/Profile
🎯 Mục đích: Xem và quản lý thông tin cá nhân
👥 Truy cập: CHỈ người dùng đã login ([Authorize])

Thành phần chính:
├── Tiêu đề "Hồ Sơ Cá Nhân"
├── Phần 1: Thông Tin Cơ Bản (Read-only hoặc Editable)
│   ├── Avatar/Hình đại diện
│   ├── Email (Read-only)
│   ├── Tên đầy đủ
│   ├── Số điện thoại
│   ├── Ngày sinh
│   ├── Giới tính
│   └── Nút "Chỉnh Sửa" (→ Account/Edit)
│
├── Phần 2: Địa Chỉ
│   ├── Địa chỉ chính (Primary)
│   │   ├── Tên địa chỉ
│   │   ├── Chi tiết địa chỉ
│   │   ├── Tỉnh/Thành phố
│   │   └── Nút "Chỉnh Sửa" / "Xóa"
│   ├── Danh sách địa chỉ khác (nếu có)
│   └── Nút "Thêm Địa Chỉ Mới"
│
├── Phần 3: Thông Tin Tài Khoản
│   ├── Ngày tạo tài khoản
│   ├── Trạng thái tài khoản (Active/Inactive)
│   ├── Email verification status
│   ├── Phone verification status
│   └── Nút "Xác Minh Email" (nếu chưa verify)
│
├── Phần 4: Bảo Mật
│   ├── Nút "Đổi Mật Khẩu" (→ Account/ChangePassword)
│   ├── Nút "Bảo Mật Tài Khoản"
│   └── Nút "Xem Hoạt Động Đăng Nhập"
│
├── Phần 5: Preferences
│   ├── Checkbox "Nhận Email Khuyến Mãi"
│   ├── Checkbox "Nhận Thông Báo SMS"
│   ├── Select Ngôn Ngữ
│   ├── Select Múi Giờ
│   └── Nút "Lưu"
│
├── Hành Động
│   ├── Nút "Lưu Thay Đổi" (nếu edit)
│   ├── Nút "Quay Lại"
│   └── Nút "Đăng Xuất"
│
└── Footer
    ├── Nút "Xóa Tài Khoản" (với xác nhận)
    └── "Liên hệ support nếu cần giúp đỡ"

Hành động có sẵn:
→ Xem thông tin cá nhân
→ Chỉnh sửa thông tin (Account/Edit)
→ Quản lý địa chỉ
→ Đổi mật khẩu (Account/ChangePassword)
→ Xác minh email (nếu chưa)
→ Cập nhật preferences
→ Xem hoạt động tài khoản
```

#### View 3.8: **Account/ChangePassword** - Đổi Mật Khẩu

```
📌 Vị trí: /Account/ChangePassword
🎯 Mục đích: Thay đổi mật khẩu tài khoản
👥 Truy cập: CHỈ người dùng đã login ([Authorize])

Thành phần chính:
├── Tiêu đề "Đổi Mật Khẩu"
├── Form Đổi Mật Khẩu
│   ├── Input Mật Khẩu Hiện Tại
│   │   └── Type: password, Required: true
│   ├── Input Mật Khẩu Mới
│   │   └── Type: password, Required: true, Min: 8 ký tự
│   ├── Input Xác Nhận Mật Khẩu Mới
│   │   └── Type: password, Required: true, Must match
│   ├── Checkbox "Đăng Xuất Trên Tất Cả Thiết Bị" (nếu có)
│   └── Password Strength Indicator (visual)
│
├── Hành Động
│   ├── Nút "Đổi Mật Khẩu" (Primary)
│   ├── Nút "Hủy" (Secondary)
│   └── Link "Quay Về Hồ Sơ"
│
├── Messages
│   ├── Success: "Mật khẩu đã được thay đổi thành công!"
│   ├── Error: "Mật khẩu hiện tại không chính xác"
│   ├── Error: "Mật khẩu mới phải khác mật khẩu cũ"
│   └── Error: "Mật khẩu phải có ít nhất 8 ký tự"
│
└── Tips
    ├── "Mật khẩu mạnh bao gồm: chữ cái, số, ký tự đặc biệt"
    ├── "Không chia sẻ mật khẩu với bất kỳ ai"
    └── "Thay đổi mật khẩu thường xuyên để bảo mật"

Hành động có sẵn:
→ Nhập mật khẩu hiện tại (verification)
→ Nhập mật khẩu mới
→ Xem chỉ số độ mạnh mật khẩu
→ Đổi mật khẩu
```

---

## 🏗️ III. KIẾN TRÚC & CÔNG NGHỆ

### 1. Công Nghệ Sử Dụng

| Thành Phần         | Chi Tiết                    |
| ------------------ | --------------------------- |
| **Framework**      | ASP.NET Core 8.0            |
| **Pattern**        | MVC (Model-View-Controller) |
| **Database**       | SQL Server                  |
| **ORM**            | Entity Framework Core 8.0   |
| **Authentication** | JWT Bearer Token            |
| **Auto Mapping**   | AutoMapper v16.1.1          |
| **Hashing**        | BCrypt.Net-Next v4.1.0      |
| **Email**          | MailKit v4.16.0             |

### 2. Kiến Trúc Ứng Dụng

```
MVC17/
├── Controllers/          # Xử lý logic yêu cầu HTTP
├── Models/              # Thực thể cơ sở dữ liệu
├── Views/               # Giao diện người dùng (Razor)
├── ViewModels/          # Dữ liệu cho Views
├── DTOs/                # Data Transfer Objects
├── Services/            # Logic nghiệp vụ
├── Mappings/            # AutoMapper profiles
├── Helpers/             # Hàm hỗ trợ & hằng số
├── Data/                # DbContext
└── ViewComponents/      # Tái sử dụng UI components
```

### 3. Mô Hình Dữ Liệu

**Các Thực Thể Chính:**

- **Products** - Sản phẩm laptop
- **Laptop** - Chi tiết laptop (CPU, GPU, RAM, Storage)
- **Category** - Danh mục sản phẩm
- **Supplier** - Nhà cung cấp
- **Customer** - Khách hàng
- **Invoice** - Đơn hàng/Hóa đơn
- **User/Account** - Tài khoản người dùng
- **ShoppingCart** - Giỏ hàng
- **Employee** - Nhân viên
- **Department** - Phòng ban

---

## 🎯 III. CHỨC NĂNG CHÍNH

### 1. Quản Lý Sản Phẩm

| Chức Năng          | Mô Tả                                     |
| ------------------ | ----------------------------------------- |
| Danh sách sản phẩm | Hiển thị tất cả laptop có sẵn             |
| Tìm kiếm & Lọc     | Lọc theo danh mục, nhà cung cấp, giá cả   |
| Chi tiết sản phẩm  | Xem thông số kỹ thuật chi tiết            |
| Phân trang         | Quản lý hiển thị với phân trang linh hoạt |
| Thêm/Sửa/Xóa       | CRUD operations cho admin                 |

### 2. Quản Lý Giỏ Hàng

- ➕ Thêm sản phẩm vào giỏ hàng
- ➖ Xóa hoặc cập nhật số lượng
- 💾 Lưu giỏ hàng bằng Session
- 🔄 Xem lại giỏ hàng trước thanh toán

### 3. Quản Lý Đơn Hàng

- 📝 Tạo đơn hàng từ giỏ hàng
- 📊 Theo dõi trạng thái đơn hàng:
  - Đang chờ xử lý (Pending)
  - Đang vận chuyển (Shipping)
  - Hoàn thành (Completed)
  - Đã hủy (Cancelled)
  - Hoàn tiền (Refunded)
- 📋 Xem lịch sử đơn hàng
- 📄 In hóa đơn/chi tiết đơn hàng

### 4. Quản Lý Tài Khoản & Xác Thực

- 👤 Đăng ký tài khoản mới
- 🔐 Đăng nhập với JWT Authentication
- 🔒 Mật khẩu được mã hóa bằng BCrypt
- 👥 Phân quyền người dùng (Roles & Permissions)
- ✉️ Gửi email xác thực/thông báo

### 5. Thống Kê & Báo Cáo

**Bảng Điều Khiển Quản Trị:**

- 💰 Doanh thu theo danh mục
- 👥 Doanh thu theo khách hàng
- 🏭 Doanh thu theo nhà cung cấp
- 📦 Doanh thu theo sản phẩm
- 👨‍💼 Doanh thu theo nhân viên
- 📈 Sản phẩm bán chạy
- ⚠️ Cảnh báo hàng tồn kho thấp

### 6. Tính Năng Khác

- 📌 Sản phẩm xu hướng/nổi bật
- 🎨 Menu danh mục động
- 🔍 Dropdown lọc nâng cao
- 📱 Responsive Design

---

## 🔐 IV. BẢNG SECURITY & AUTHENTICATION

### 1. Xác Thực (Authentication)

```csharp
JWT Bearer Token:
- Token được lưu trong Cookie
- Kiểm tra ở header hoặc cookie
- Validation parameters:
  • ValidateIssuer
  • ValidateAudience
  • ValidateLifetime
  • ValidateIssuerSigningKey
```

### 2. Phân Quyền (Authorization)

- **Roles:** Admin, Manager, Employee, Customer
- **Permissions:** Kiểm soát truy cập tài nguyên
- **[Authorize]** attribute bảo vệ controller/action

### 3. Bảo Mật Dữ Liệu

- 🔐 Mật khẩu được hash bằng BCrypt
- 📧 Email verification
- 🍪 Secure cookies
- 🛡️ SQL injection prevention qua EF Core
- 🚫 CORS configuration (nếu có)

---

## 📊 V. CONTROLLERS VÀ ENDPOINTS

### 1. HomeController

- `GET /` - Trang chủ
- Hiển thị sản phẩm nổi bật, xu hướng

### 2. ProductController

- `GET /Product/Index` - Danh sách sản phẩm
- `GET /Product/Details/{id}` - Chi tiết sản phẩm
- `POST /Product/Create` - Tạo sản phẩm (admin)
- `POST /Product/Edit` - Sửa sản phẩm (admin)
- `POST /Product/Delete` - Xóa sản phẩm (admin)

### 3. CartController

- `POST /Cart/AddToCart` - Thêm vào giỏ
- `GET /Cart/View` - Xem giỏ hàng
- `POST /Cart/RemoveItem` - Xóa khỏi giỏ
- `POST /Cart/UpdateQuantity` - Cập nhật số lượng
- `POST /Cart/Checkout` - Thanh toán

### 4. OrderController

- `GET /Order/History` - Lịch sử đơn hàng
- `GET /Order/Details/{id}` - Chi tiết đơn hàng
- `POST /Order/Create` - Tạo đơn hàng
- `POST /Order/Cancel` - Hủy đơn hàng
- `POST /Order/Refund` - Hoàn tiền

### 5. AccountController

- `GET /Account/Login` - Trang đăng nhập
- `POST /Account/Login` - Xử lý đăng nhập
- `GET /Account/Register` - Trang đăng ký
- `POST /Account/Register` - Xử lý đăng ký
- `GET /Account/Logout` - Đăng xuất
- `GET /Account/Profile` - Hồ sơ người dùng

### 6. StatisticController

- `GET /Statistic/Dashboard` - Bảng điều khiển
- `GET /Statistic/RevenueByCategory` - Doanh thu theo danh mục
- `GET /Statistic/RevenueByCustomer` - Doanh thu theo KH
- `GET /Statistic/RevenueByEmployee` - Doanh thu theo NV
- `GET /Statistic/LowStockProducts` - Cảnh báo hàng tồn kho

---

## 🎨 VI. CÔNG CỤ & THÀNH PHẦN UI

### View Components

1. **CategoryMenu** - Menu danh mục động
2. **ProductSpec** - Hiển thị thông số sản phẩm
3. **CreateProductSpec** - Form thêm thông số sản phẩm
4. **UpdateProductSpec** - Form sửa thông số
5. **FilterDropdown** - Dropdown lọc nâng cao
6. **Pagination** - Điều khiển phân trang
7. **TrendingProduct** - Danh sách sản phẩm xu hướng

### ViewModels

- **ProductVM** - Dữ liệu sản phẩm
- **CategoryMenuVM** - Dữ liệu menu danh mục
- **OrderHistoryVM** - Dữ liệu lịch sử đơn hàng
- **PaginateVM** - Dữ liệu phân trang
- **TrendingProductVM** - Dữ liệu sản phẩm xu hướng

---

## 📁 VII. CẤU TRÚC THƯ MỤC CHI TIẾT

### Controllers/ (6 controllers)

- AccountController - Quản lý tài khoản
- CartController - Quản lý giỏ hàng
- HomeController - Trang chủ
- OrderController - Quản lý đơn hàng
- ProductController - Quản lý sản phẩm
- StatisticController - Thống kê và báo cáo

### Models/ (40+ models)

**Bảng chính:**

- Laptop, LaptopComponent
- Cpu, Gpu, Ram, Storage
- Category, Supplier
- Product, ProductSku
- Invoice, InvoiceDetail, InvoiceStatus
- Customer, User, Account, Role, Access
- Employee, Department, Position
- ShoppingCart, CartItem
- PersonalInformation, Discount

**View Models (VW-):**

- VwProduct, VwLaptopSpec, VwCpuSpec, VwGpuSpec, VwRamSpec, VwStorageSpec
- VwInvoice, VwInvoiceDetail, VwOrderedItem, VwCartItem
- VwCustomerProfile, VwTrendingProduct
- VwsCancelledOrder, VwsCompletedOrder, VwsPendingOrder, VwsRefundedOrder, VwsShippingOrder
- VwsRevenueByCategory, VwsRevenueByCustomer, VwsRevenueByEmployee, VwsRevenueByProduct, VwsRevenueBySupplier
- VwsLowStockProduct, VwsTotalCustomer, VwsTotalProduct

### Services/

- **Interfaces/** - Định nghĩa hợp đồng dịch vụ
- **Implementations/** - Triển khai các dịch vụ
  - ProductService
  - OrderService
  - AccountService
  - CartService
  - StatisticService

### DTOs/

**Accounts/**

- LoginRequest
- RegisterRequest
- AccountResponse

**Orders/**

- CreateOrderRequest
- OrderDetailResponse
- OrderHistoryResponse

**Products/**

- CreateProductRequest
- UpdateProductRequest
- ProductDetailResponse

### Mappings/

- AccountMP - AutoMapper profiles cho Account
- OrderMP - AutoMapper profiles cho Order
- ProductMP - AutoMapper profiles cho Product

### Helpers/

**Constants/** - Các hằng số ứng dụng

- ProductConstants
- OrderConstants
- AccountConstants

**Utils/** - Hàm tiện ích

- StringUtils
- DateUtils
- ValidationUtils

---

## 🔧 VIII. CẤU HÌNH & DEPENDENCIES

### appsettings.json

```json
{
  "Logging": { ... },
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;"
  },
  "Jwt": {
    "Issuer": "...",
    "Audience": "...",
    "Key": "..."
  },
  "Email": {
    "SmtpServer": "...",
    "Port": 587,
    "FromAddress": "..."
  }
}
```

### Packages

- **AutoMapper 16.1.1** - Mapping objects
- **BCrypt.Net-Next 4.1.0** - Password hashing
- **MailKit 4.16.0** - Email sending
- **MimeKit 4.16.0** - Email MIME
- **EntityFramework Core 8.0.26** - ORM
- **JWT Bearer 8.0.26** - Authentication

---

## 🚀 IX. QUY TRÌNH HOẠT ĐỘNG

### 1. Quy Trình Mua Hàng

```
Khách hàng → Duyệt sản phẩm → Thêm giỏ hàng
→ Xem giỏ hàng → Thanh toán → Tạo đơn hàng
→ Xác nhận → Vận chuyển → Hoàn thành
```

### 2. Quy Trình Quản Lý Sản Phẩm

```
Admin → Thêm sản phẩm → Nhập thông số chi tiết
→ Đặt giá & kho → Hiển thị → Cập nhật/Xóa
```

### 3. Quy Trình Xác Thực

```
Người dùng → Đăng ký/Đăng nhập
→ Tạo JWT Token → Lưu Cookie
→ Kiểm tra Authorization → Truy cập tài nguyên
```

---

## 📈 X. QUY MỐ & HIỆU SUẤT

### Số Lượng

- **40+ Models** - Bao phủ toàn bộ miền
- **6 Controllers** - Xử lý tất cả endpoint
- **10+ View Components** - Tái sử dụng UI
- **Hàng trăm Views** - Giao diện toàn diện

### Features

- ✅ Pagination - Hỗ trợ phân trang hiệu quả
- ✅ Async/Await - Các operations không chặn
- ✅ AutoMapper - Giảm boilerplate mapping
- ✅ Service Layer - Tách biệt logic nghiệp vụ
- ✅ ViewComponents - Tăng tái sử dụng code

---

## 💡 XI. ĐIỂM NỔI BẬT

### ✨ Ưu Điểm

1. **Kiến trúc sạch** - Tách biệt concerns rõ ràng
2. **Bảo mật** - JWT + BCrypt + authorization
3. **Tính năng đầy đủ** - Bán hàng, quản lý, thống kê
4. **Mở rộng dễ** - Dễ thêm tính năng mới
5. **Performance** - Async operations, lazy loading
6. **User-friendly** - UI responsive, UX tốt
7. **Database design** - Schema normalization tốt

### 🎯 Hướng Phát Triển

- 📱 Mobile app support (API endpoints)
- 🔔 Real-time notifications (SignalR)
- 💳 Multiple payment gateways
- 📊 Advanced analytics & reporting
- 🌍 Multi-language support
- 🎁 Loyalty program & rewards
- 📧 Email marketing automation
- 🔍 Full-text search engine

---

## 📋 XII. DANH SÁCH KIỂM TRA (CHECKLIST)

### ✅ Đã Hoàn Thành

- [x] Xác thực và phân quyền người dùng
- [x] Quản lý sản phẩm (CRUD)
- [x] Giỏ hàng và thanh toán
- [x] Quản lý đơn hàng
- [x] Thống kê và báo cáo
- [x] Email notification
- [x] Session management
- [x] Database schema

### 📌 Có Thể Cần Cải Thiện

- [ ] Tối ưu hóa hiệu suất truy vấn
- [ ] Thêm unit tests
- [ ] API documentation (Swagger)
- [ ] Caching layer
- [ ] Logging enhancement
- [ ] Error handling centralized
- [ ] Input validation consistent
- [ ] Performance monitoring

---

## 📞 XIII. THÔNG TIN LIÊN HỆ & HỖ TRỢ

### Công Nghệ Stack

- **.NET** - `8.0`
- **Database** - `SQL Server`
- **Architecture** - `MVC Pattern`
- **Source Control** - `Git`

### Cách Sử Dụng

1. Clone repository
2. Setup database connection (appsettings.json)
3. Run `dotnet restore`
4. Run `dotnet ef database update`
5. Run `dotnet run`
6. Access at `https://localhost:5001`

---

## 🎓 XIV. KẾT LUẬN

MVC17 là một **ứng dụng e-commerce hoàn chỉnh** được phát triển bằng công nghệ ASP.NET Core hiện đại, cung cấp:

✅ **Trải nghiệm mua sắm chuyên nghiệp** cho khách hàng
✅ **Công cụ quản lý toàn diện** cho quản trị viên
✅ **Bảo mật cao** với JWT và BCrypt
✅ **Kiến trúc mở rộng** dễ bảo trì và phát triển
✅ **Performance tốt** với async operations

Dự án sẵn sàng cho **triển khai sản xuất** hoặc **mở rộng thêm tính năng** theo nhu cầu kinh doanh.

---

**Ngày tạo:** 2026-05-15
**Version:** 1.0
**Status:** ✅ Production Ready
