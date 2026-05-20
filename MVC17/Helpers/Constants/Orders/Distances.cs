namespace MVC17.Helpers.Constants.Orders
{
    public static class Distances
    {
        public const float AverageMotorbikeSpeed = 40.0f;
        public const float AverageTruckSpeed = 60.0f;
        public const float AveragePlaneSpeed = 850.0f;
        public static Dictionary<string, float> DistancesFromHeadQuarters = new Dictionary<string, float>
        {
            { "Hồ Chí Minh", 0.0f },
            { "Tây Ninh", 85.6f },
            { "Đồng Nai", 30.4f },
            { "Vĩnh Long", 100.7f },
            { "Cần Thơ", 169.8f },
            { "Đồng Tháp", 129.4f },
            { "An Giang", 189.6f },
            { "Cà Mau", 302.5f },

            { "Lâm Đồng", 234.8f },
            { "Khánh Hòa", 320.7f },
            { "Đắk Lắk", 347.2f },
            { "Gia Lai", 433.9f },

            { "Quảng Ngãi", 694.3f },
            { "Đà Nẵng", 605.8f },
            { "Huế", 654.1f },
            { "Quảng Trị", 715.6f },
            { "Hà Tĩnh", 938.4f },
            { "Nghệ An", 887.3f },
            { "Thanh Hóa", 1128.6f },
            { "Ninh Bình", 1247.9f },

            { "Hưng Yên", 1298.7f },
            { "Hà Nội", 1302.4f },
            { "Bắc Ninh", 1318.6f },
            { "Hải Phòng", 1341.2f },
            { "Phú Thọ", 1365.8f },
            { "Quảng Ninh", 1386.9f },
            { "Thái Nguyên", 1378.5f },
            { "Lạng Sơn", 1456.3f },

            { "Lào Cai", 1572.8f },
            { "Lai Châu", 1687.1f },
            { "Điện Biên", 1762.4f },
            { "Sơn La", 1534.6f },
            { "Cao Bằng", 1598.7f },
            { "Tuyên Quang", 1438.2f }
        };

        // Các ngưỡng khoảng cách (km)
        private const float MotorbikeThreshold = 50.0f;   // <= 50 km: giao bằng xe máy
        private const float TruckThreshold = 300.0f;      // <= 300 km: giao bằng xe tải
                                                          // > 300 km: máy bay + xe tải + xe máy

        // Mặc định giao trong Hồ Chí Minh: 1 ngày
        private const float LocalDeliveryDays = 1.0f;

        /// <summary>
        /// Tính tổng thời gian vận chuyển (đơn vị: ngày).
        ///
        /// Logic:
        /// 1. Hồ Chí Minh -> Hồ Chí Minh: cố định 2 ngày.
        /// 2. <= 50 km:
        ///    - Chỉ dùng xe máy.
        /// 3. > 50 km và <= 300 km:
        ///    - Xe tải toàn tuyến.
        /// 4. > 300 km:
        ///    - Xe tải từ kho đến sân bay Tân Sơn Nhất: 20 km.
        ///    - Máy bay cho quãng đường chính.
        ///    - Xe tải từ sân bay địa phương đến trung tâm tỉnh: 30 km.
        ///    - Xe máy giao chặng cuối đến khách hàng: 10 km.
        ///
        /// Sau khi tính thời gian chạy, cộng thêm:
        /// - 1 ngày xử lý đơn hàng và đóng gói.
        /// - 0.5 ngày trung chuyển/bốc dỡ nếu có đi máy bay.
        /// </summary>
        public static float CalculateShippingDays(string destination)
        {
            if (!DistancesFromHeadQuarters.TryGetValue(destination, out float distance))
                throw new ArgumentException($"Không tìm thấy khoảng cách đến '{destination}'.");

            // Giao nội thành Hồ Chí Minh
            if (destination == "Hồ Chí Minh")
            {
                return LocalDeliveryDays;
            }

            // 1 ngày xử lý đơn hàng
            float totalHours = 24.0f;

            // Chặng rất gần: xe máy
            if (distance <= MotorbikeThreshold)
            {
                totalHours += distance / AverageMotorbikeSpeed;
            }
            // Chặng ngắn và trung bình: xe tải
            else if (distance <= TruckThreshold)
            {
                totalHours += distance / AverageTruckSpeed;
            }
            // Chặng xa: xe tải + máy bay + xe tải + xe máy
            else
            {
                const float truckToAirport = 20.0f;   // Kho -> sân bay Tân Sơn Nhất
                const float truckFromAirport = 30.0f; // Sân bay đích -> trung tâm tỉnh
                const float lastMile = 10.0f;         // Giao cuối cùng đến khách hàng

                // Quãng đường bay chính
                float flightDistance = Math.Max(
                    0,
                    distance - truckToAirport - truckFromAirport - lastMile);

                // Xe tải đến sân bay
                totalHours += truckToAirport / AverageTruckSpeed;

                // Bay
                totalHours += flightDistance / AveragePlaneSpeed;

                // Xe tải từ sân bay đến trung tâm tỉnh
                totalHours += truckFromAirport / AverageTruckSpeed;

                // Xe máy giao chặng cuối
                totalHours += lastMile / AverageMotorbikeSpeed;

                // 0.5 ngày (12 giờ) cho trung chuyển, bốc dỡ, nhận hàng sân bay
                totalHours += 12.0f;
            }

            // Làm tròn đến 1 chữ số thập phân
            float totalDays = totalHours / 24.0f;
            return MathF.Round(totalDays, 1);
        }

        // ===== Đơn giá vận chuyển (VNĐ/km) =====
        public const decimal MotorbikeRatePerKm = 5_000m;
        public const decimal TruckRatePerKm = 15_000m;
        public const decimal PlaneRatePerKm = 50_000m;

        // ===== Phụ phí cố định =====
        public const decimal PackagingFee = 10_000m;       // đóng gói
        public const decimal AirportHandlingFee = 100_000m; // bốc dỡ sân bay

        /// <summary>
        /// Tính phí vận chuyển (VNĐ).
        ///
        /// Logic:
        /// 1. Hồ Chí Minh -> Hồ Chí Minh: phí cố định 30.000 VNĐ.
        /// 2. <= 50 km: xe máy.
        /// 3. > 50 km và <= 300 km: xe tải.
        /// 4. > 300 km: xe tải + máy bay + xe tải + xe máy.
        /// </summary>
        public static decimal CalculateShippingFee(string destination)
        {
            if (!DistancesFromHeadQuarters.TryGetValue(destination, out float distance))
                throw new ArgumentException($"Không tìm thấy khoảng cách đến '{destination}'.");

            // Nội thành Hồ Chí Minh
            if (destination == "Hồ Chí Minh")
                return 30_000m;

            decimal total = PackagingFee;

            // ===== Chặng gần: xe máy =====
            if (distance <= MotorbikeThreshold)
            {
                total += (decimal)distance * MotorbikeRatePerKm;
            }
            // ===== Chặng trung bình: xe tải =====
            else if (distance <= TruckThreshold)
            {
                total += (decimal)distance * TruckRatePerKm;
            }
            // ===== Chặng xa: xe tải + máy bay + xe tải + xe máy =====
            else
            {
                const float truckToAirport = 20.0f;   // Kho -> sân bay Tân Sơn Nhất
                const float truckFromAirport = 30.0f; // Sân bay đích -> trung tâm tỉnh
                const float lastMile = 10.0f;         // Trung tâm -> khách hàng

                // Tính quãng đường bay chính
                float flightDistance = Math.Max(
                    0,
                    distance - truckToAirport - truckFromAirport - lastMile);

                // Xe tải đến sân bay
                total += (decimal)truckToAirport * TruckRatePerKm;

                // Máy bay
                total += (decimal)flightDistance * PlaneRatePerKm;

                // Xe tải từ sân bay đến trung tâm tỉnh
                total += (decimal)truckFromAirport * TruckRatePerKm;

                // Xe máy giao chặng cuối
                total += (decimal)lastMile * MotorbikeRatePerKm;

                // Phụ phí sân bay
                total += AirportHandlingFee;
            }

            // Làm tròn đến 1.000 VNĐ
            total = Math.Ceiling(total / 1_000m) * 1_000m;

            return total;
        }
    }
}
