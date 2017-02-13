﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using System;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure
{
    public class OrderingContext
      : DbContext,IUnitOfWork

    {
        const string DEFAULT_SCHEMA = "ordering";

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<PaymentMethod> Payments { get; set; }

        public DbSet<Buyer> Buyers { get; set; }

        public DbSet<CardType> CardTypes { get; set; }

        public DbSet<OrderStatusType> OrderStatus { get; set; }


        public OrderingContext(DbContextOptions options) : base(options)
        {
          
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Buyer>(ConfigureBuyer);
            modelBuilder.Entity<PaymentMethod>(ConfigurePayment);
            modelBuilder.Entity<Order>(ConfigureOrder);
            modelBuilder.Entity<OrderItem>(ConfigureOrderItems);
            modelBuilder.Entity<CardType>(ConfigureCardTypes);
            modelBuilder.Entity<OrderStatusType>(ConfigureOrderStatus);
        }

        void ConfigureBuyer(EntityTypeBuilder<Buyer> buyerConfiguration)
        {
            buyerConfiguration.ToTable("buyers", DEFAULT_SCHEMA);

            buyerConfiguration.HasKey(b => b.Id);

            buyerConfiguration.Property(b => b.Id);
                //.ForSqlServerUseSequenceHiLo("buyerseq", DEFAULT_SCHEMA);

            buyerConfiguration.Property(b=>b.FullName)
                .HasMaxLength(200)
                .IsRequired();

            buyerConfiguration.HasIndex("FullName")
              .IsUnique(false);

            var navigation = buyerConfiguration.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

        void ConfigurePayment(EntityTypeBuilder<PaymentMethod> paymentConfiguration)
        {
            paymentConfiguration.ToTable("paymentmethods", DEFAULT_SCHEMA);

            paymentConfiguration.HasKey(b => b.Id);

            paymentConfiguration.Property(b => b.Id);
                //.ForSqlServerUseSequenceHiLo("paymentseq", DEFAULT_SCHEMA);

            paymentConfiguration.Property<Guid>("BuyerId")
                .IsRequired();

            paymentConfiguration.Property<string>("CardHolderName")
                .HasMaxLength(200)
                .IsRequired();

            paymentConfiguration.Property<string>("Alias")
                .HasMaxLength(200)
                .IsRequired();

            paymentConfiguration.Property<string>("CardNumber")
                .HasMaxLength(25)
                .IsRequired();

            paymentConfiguration.Property<DateTime>("Expiration")
                .IsRequired();

            paymentConfiguration.Property<int>("CardTypeId")
                .IsRequired();

            paymentConfiguration.HasOne(p => p.CardType)
                .WithMany()
                .HasForeignKey("CardTypeId");
        }

        void ConfigureOrder(EntityTypeBuilder<Order> orderConfiguration)
        {
            orderConfiguration.ToTable("orders", DEFAULT_SCHEMA);

            orderConfiguration.HasKey(o => o.Id);

            orderConfiguration.Property(o => o.Id);
                //.ForSqlServerUseSequenceHiLo("orderseq", DEFAULT_SCHEMA);

            orderConfiguration.Property<DateTime>("OrderDate").IsRequired();
            orderConfiguration.Property<string>("Street").IsRequired();
            orderConfiguration.Property<string>("State").IsRequired();
            orderConfiguration.Property<string>("City").IsRequired();
            orderConfiguration.Property<string>("ZipCode").IsRequired();
            orderConfiguration.Property<string>("Country").IsRequired();
            orderConfiguration.Property<Guid>("BuyerId").IsRequired();
            orderConfiguration.Property<int>("OrderStatusId").IsRequired();
            orderConfiguration.Property<Guid>("PaymentMethodId").IsRequired();

            var navigation = orderConfiguration.Metadata.FindNavigation(nameof(Order.OrderItems));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            orderConfiguration.HasOne(o => o.PaymentMethod)
                .WithMany()
                .HasForeignKey("PaymentMethodId")
                .OnDelete(DeleteBehavior.Restrict);

            orderConfiguration.HasOne(o => o.Buyer)
                .WithMany()
                .HasForeignKey("BuyerId");

            orderConfiguration.HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey("OrderStatusId");
        }

        void ConfigureOrderItems(EntityTypeBuilder<OrderItem> orderItemConfiguration)
        {
            orderItemConfiguration.ToTable("orderItems", DEFAULT_SCHEMA);

            orderItemConfiguration.HasKey(o => o.Id);

            orderItemConfiguration.Property(o => o.Id);
                //.ForSqlServerUseSequenceHiLo("orderitemseq");

            orderItemConfiguration.Property<Guid>("OrderId")
                .IsRequired();

            orderItemConfiguration.Property<decimal>("Discount")
                .IsRequired();

            orderItemConfiguration.Property<Guid>("ProductId")
                .IsRequired();

            orderItemConfiguration.Property<string>("ProductName")
                .IsRequired();

            orderItemConfiguration.Property<decimal>("UnitPrice")
                .IsRequired();

            orderItemConfiguration.Property<int>("Units")
                .IsRequired();

            orderItemConfiguration.Property<string>("PictureUrl")
                .IsRequired(false);
        }

        void ConfigureOrderStatus(EntityTypeBuilder<OrderStatusType> orderStatusConfiguration)
        {
            orderStatusConfiguration.ToTable("orderstatus", DEFAULT_SCHEMA);

            orderStatusConfiguration.HasKey(o => o.Id);

            orderStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            orderStatusConfiguration.Property(o => o.DisplayName)
                 .HasMaxLength(200)
                 .IsRequired();

            orderStatusConfiguration.Property(o => o.Value)
                .HasMaxLength(200)
                .IsRequired();
        }

        void ConfigureCardTypes(EntityTypeBuilder<CardType> cardTypesConfiguration)
        {
            cardTypesConfiguration.ToTable("cardtypes", DEFAULT_SCHEMA);

            cardTypesConfiguration.HasKey(ct => ct.Id);

            cardTypesConfiguration.Property(ct => ct.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            cardTypesConfiguration.Property(o => o.DisplayName)
                .HasMaxLength(200)
                .IsRequired();

            cardTypesConfiguration.Property(o => o.Value)
                .HasMaxLength(200)
                .IsRequired();
        }

        
    }
}
