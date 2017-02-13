﻿using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TechTalk.SpecFlow;

namespace Shopping.Domain.SpecFlowTests
{
    [Binding]
    public class CheckoutSteps
    {
        private Order _order;
        private OrderCheckedOutEvent receivedEvent= null;

        public CheckoutSteps()
        {
            DomainEventBus.Instance.Subscribe(CheckReceivedEvent);
        }

        [Given(@"An order with (.*) units of a given product \(id (.*)\)")]
        public void GivenAnOrderWithUnitsOfAGivenProductId(int units, Guid productId)
        {
            _order = new Order(Guid.Empty, Guid.Empty, new Address("", "", "", "", ""));
            _order.AddOrderItem(productId, "", 100, 0, "", units);
        }
        
        [When(@"I check out the order")]
        public void WhenICheckOutTheOrder()
        {
            _order.CheckOut();
        }

        [Then(@"a checkedOut event should be emited indicating that (.*) units of product \(id (.*)\) are pending to be processed")]
        public void ThenACheckedOutEventShouldBeEmitedIndicatingThatUnitsOfProductIdArePendingToBeProcessed(int units, Guid productId)
        {
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("Order", receivedEvent.AggregateSource);
            Assert.AreEqual(1, receivedEvent.ProductIds.Count);
            foreach( var p in receivedEvent.ProductIds)
            {    
                Assert.AreEqual(productId, p.ProductId);
                Assert.AreEqual(units, p.Units);
            }
            Assert.AreEqual(_order.OrderStatus, OrderStatusType.CheckedOut);

        }

        private void CheckReceivedEvent(IDomainEvent ev )
        {
            if (ev is OrderCheckedOutEvent)
            {
                receivedEvent = (OrderCheckedOutEvent)ev;
            }
        }
    }
}
