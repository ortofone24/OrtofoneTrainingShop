﻿using OrtofoneTrainingShop.Models.Data;
using System;

namespace OrtofoneTrainingShop.Models.ViewModels.Shop
{
    public class OrderVM
    {
        public OrderVM()
        {
            
        }

        public OrderVM(OrderDTO row)
        {
            OrderId = row.OrderId;
            UserId = row.UserId;
            CreateAt = row.CreatedAt;
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreateAt { get; set; }
    }
}