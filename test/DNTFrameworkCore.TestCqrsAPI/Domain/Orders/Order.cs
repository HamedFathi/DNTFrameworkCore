﻿using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Parties;
using DNTFrameworkCore.TestCqrsAPI.Domain.Sales;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;
using DNTFrameworkCore.Timing;
using static DNTFrameworkCore.Functional.Result;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Orders
{
    public class Order : Entity<long>, IAggregateRoot
    {
        public OrderStatus Status { get; private set; }
        public SaleMethod SaleMethod { get; private set; }
        public Customer Customer { get; private set; }
        public Address ShippingAddress { get; private set; }
        public DateTime DateTime { get; private set; }
        private List<OrderLine> _lines;
        public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
        private List<OrderNote> _notes;
        public IReadOnlyCollection<OrderNote> Notes => _notes.AsReadOnly();

        private List<OrderHistory> _histories;
        public IReadOnlyCollection<OrderHistory> Histories => _histories.AsReadOnly();

        private Order()
        {
            _lines = new List<OrderLine>();
            _notes = new List<OrderNote>();
            _histories = new List<OrderHistory>();
        }

        public static Result<Order> Create(SaleMethod saleMethod, Customer customer, Address address)
        {
            if (saleMethod == null) throw new ArgumentNullException(nameof(saleMethod));
            if (customer == null) throw new ArgumentNullException(nameof(customer));
            if (address == null) throw new ArgumentNullException(nameof(address));

            var order = new Order
            {
                Status = OrderStatus.Pending,
                DateTime = SystemTime.Now(),
                Customer = customer,
                ShippingAddress = address,
                SaleMethod = saleMethod
            };

            return Result.Ok(order);
        }

        public Result AddOrderLine(int productId, Price unitPrice, int quantity = 1, decimal discount = 0)
        {
            //todo: business rules
            if (_lines.Any(line => line.ProductId == productId))
            {
            }

            return OrderLine.Create(productId, unitPrice, quantity, discount)
                .Then(line => _lines.Add(line));
        }

        public Result AddOrderNote(string content)
        {
            return OrderNote.Create(content)
                .Then(note => _notes.Add(note));
        }

        public Result Prepare()
        {
            if (!Equals(Status, OrderStatus.Pending))
            {
                return Fail(
                    $"Is not possible to change the order status from {Status.Name} to {OrderStatus.Preparation.Name}.");
            }

            Status = OrderStatus.Preparation;

            return Ok();
        }

        public Result Cancel()
        {
            if (Equals(Status, OrderStatus.Pending))
            {
                return Fail(
                    $"Is not possible to change the order status from {Status.Name} to {OrderStatus.Cancelled.Name}.");
            }

            //todo: business rules
            Status = OrderStatus.Cancelled;

            return Ok();
        }

        public Result Clear()
        {
            if (!Equals(Status, OrderStatus.Paid) && !Equals(Status, OrderStatus.Shipped))
            {
                return Fail(
                    $"Is not possible to change the order status from {Status.Name} to {OrderStatus.Completed.Name}.");
            }

            //todo: business rules
            Status = OrderStatus.Completed;

            return Ok();
        }

        public Result Ship()
        {
            if (!SaleMethod.Nature.ShipmentEnabled || !Equals(Status, OrderStatus.Paid))
            {
                return Fail(
                    $"Is not possible to change the order status from {Status.Name} to {OrderStatus.Shipped.Name}.");
            }

            //todo: business rules
            Status = OrderStatus.Shipped;

            return Ok();
        }
    }
}