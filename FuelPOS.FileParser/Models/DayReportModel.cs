﻿using System.Collections.Generic;

namespace POSFileParser.Models
{
    public class DayReportModel
    {
        public StatusModel Status { get; set; }
        public HeaderReceiptModel HeaderReceipt { get; set; }
        public List<MerchantInfoModel> MerchantInfo { get; set; }
        public List<FuelInfoModel> StartFuelInfo { get; set; }
        public List<FuelInfoModel> FuelInfo { get; set; }
        public List<ArticleSoldInfoModel> ArticleSoldInfo { get; set; }
        public List<CurrencyInfoModel> CurrencyInfo { get; set; }
        public List<LionLoyaltyModel> LionLoyalty { get; set; }
    }
}
