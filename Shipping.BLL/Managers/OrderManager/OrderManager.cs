﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shipping.BLL.Dtos;
using Shipping.DAL;
using Shipping.DAL.Data;
using Shipping.DAL.Data.Models;
using Shipping.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;


namespace Shipping.BLL.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IRepository<ShippingType> _shippingRepository;
        private readonly IRepository<DeliverToVillage> _deliverToVillageRepository;
        private readonly IRepository<Weight> _weightRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<SpecialPrice> _specialPriceRepository;

        public OrderManager(IOrderRepository orderRepository, IProductRepository productRepository, IRepository<ShippingType> shippingRepository, IRepository<DeliverToVillage> deliverToVillageRepository, IRepository<Weight> weightRepository, IRepository<City> cityRepository, IRepository<SpecialPrice> specialPriceRepository)
        {
            this._orderRepository = orderRepository;
            this._productRepository = productRepository;
            this._shippingRepository = shippingRepository;
            this._deliverToVillageRepository = deliverToVillageRepository;
            this._weightRepository = weightRepository;
            this._cityRepository = cityRepository;
            this._specialPriceRepository = specialPriceRepository;
        }

        public async Task<bool> Add(AddOrderDto orderDto)
        {

            double costDeliverToVillage = await Cost_DeliverToVillageAsync(orderDto.DeliverToVillage);

            double countWeight = CountWeight(orderDto.Products);

            double costAllProducts = Cost_AllProducts(orderDto.Products);

            double costAddititonalWeight = await Cost_AdditionalWeight(countWeight);

            double costShippingType = await Cost_ShippingType(orderDto.ShippingTypeId);

            double cityShippingPrice = (double)await GetSpecialPricesWithMerchantandCityId(orderDto.MerchantId, orderDto.CityId);
            if (cityShippingPrice == 0)
            {
                cityShippingPrice = await CityShippingPrice(orderDto.CityId);
            }

            Order order = new Order()
            {
                MerchantId = orderDto.MerchantId,
                orderType = orderDto.orderType,
                ClientName = orderDto.ClientName,
                FirstPhoneNumber = orderDto.FirstPhoneNumber,
                SecondPhoneNumber = orderDto.SecondPhoneNumber,
                Email = orderDto.Email,
                GovernorateId = orderDto.GovernorateId,
                CityId = orderDto.CityId,
                Street = orderDto.Street,
                DeliverToVillage = orderDto.DeliverToVillage,
                ShippingTypeId = orderDto.ShippingTypeId,
                PaymentType = orderDto.PaymentType,
                BranchId = orderDto.BranchId,
                orderStatus = OrderStatus.New,
                Date = DateTime.Now,
                Notes = orderDto.Notes,
                isDeleted = false,
                ProductTotalCost = costAllProducts,
                OrderShippingTotalCost = costDeliverToVillage + costAddititonalWeight + cityShippingPrice + costShippingType,// check if has special price
                Weight = countWeight,
                Products = orderDto.Products.Select(prod => new Product
                {
                    Name = prod.Name,
                    Quantity = prod.Quantity,
                    Price = prod.Price,
                    Weight = prod.Weight,
                    isDeleted = false,
                }).ToList(),
            };


            bool isSuccesfullOrder = _orderRepository.Add(order);
            bool isSuccesfullProduct = _productRepository.AddRange(order.Products.ToList());
            if (isSuccesfullOrder && isSuccesfullProduct)
            {
                bool isSaved = _orderRepository.SaveChanges();
                if (isSaved)
                {
                    return true;
                }
            }
            return false;

        }

        public async Task<bool> Update(UpdateOrderDto newOrder)
        {
            double costDeliverToVillage = await Cost_DeliverToVillageAsync(newOrder.DeliverToVillage);

            double countWeight = CountWeight(newOrder.Products);

            double costAllProducts = Cost_AllProducts(newOrder.Products);

            double costAddititonalWeight = await Cost_AdditionalWeight(countWeight);

            double costShippingType = await Cost_ShippingType(newOrder.ShippingTypeId);

            double cityShippingPrice = (double)await GetSpecialPricesWithMerchantandCityId(newOrder.MerchantId, newOrder.CityId);
            if (cityShippingPrice == 0)
            {
                cityShippingPrice = await CityShippingPrice(newOrder.CityId);
            }

            var oldOrder = _orderRepository.GetById(newOrder.Id);

            if (oldOrder != null)
            {
                oldOrder.orderType = newOrder.orderType;
                oldOrder.ClientName = newOrder.ClientName;
                oldOrder.FirstPhoneNumber = newOrder.FirstPhoneNumber;
                oldOrder.SecondPhoneNumber = newOrder.SecondPhoneNumber;
                oldOrder.Email = newOrder.Email;
                oldOrder.GovernorateId = newOrder.GovernorateId;
                oldOrder.CityId = newOrder.CityId;
                oldOrder.Street = newOrder.Street;
                oldOrder.DeliverToVillage = newOrder.DeliverToVillage;
                oldOrder.ShippingTypeId = newOrder.ShippingTypeId;
                oldOrder.PaymentType = newOrder.PaymentType;
                oldOrder.BranchId = newOrder.BranchId;
                oldOrder.orderStatus = newOrder.orderStatus;
                oldOrder.Date = DateTime.Now;
                oldOrder.Notes = newOrder.Notes;
                oldOrder.isDeleted = false;
                oldOrder.ProductTotalCost = costAllProducts;
                oldOrder.OrderShippingTotalCost = costDeliverToVillage + costAddititonalWeight + cityShippingPrice + costShippingType;
                oldOrder.Weight = countWeight;
                var newProducts = newOrder.Products.Select(prod => new Product
                {
                    OrderId = oldOrder.Id,
                    Name = prod.Name,
                    Quantity = prod.Quantity,
                    Price = prod.Price,
                    Weight = prod.Weight,
                    isDeleted = false,
                }).ToList();

                var products = _productRepository.GetProductsByOrderId(oldOrder.Id).ToList();
                var deleteFlag = _productRepository.DeleteRange(products);
                var addFlag = _productRepository.AddRange(newProducts);

                if (deleteFlag && addFlag)
                {
                    bool isSaved = _orderRepository.SaveChanges();
                    if (isSaved)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Delete(int orderId)
        {
            var order = _orderRepository.GetById(orderId);

            if (order != null)
            {
                List<Product> products = _productRepository.GetProductsByOrderId(orderId);
                bool deleted = _productRepository.DeleteRange(products);
                if (deleted)
                {
                    bool isSuccesfull = _orderRepository.Delete(order);
                    if (isSuccesfull)
                    {
                        _orderRepository.SaveChanges();
                        return true;
                    }
                }

            }
            return false;
        }

        public IEnumerable<ReadOrderReportsDto> GetAll(int pageNumer, int pageSize)
        {
            return _orderRepository.GetAll(pageNumer,pageSize).Select(s => new ReadOrderReportsDto
            {
                Merchant = s.Merchant!.Name,
                orderStatus = s.orderStatus,
                FirstPhoneNumber = s.FirstPhoneNumber,
                OrderShippingTotalCost = s.OrderShippingTotalCost,
                ProductTotalCost = s.ProductTotalCost,
                Date = s.Date,
                ClientName = s.ClientName,
                Governorate = s.Governorate!.Name,
                City = s.City!.Name,
            });
        }

        public int CountAll()
        {
            return _orderRepository.CountAll();
        }
        public IEnumerable<ReadOrderReportsDto> SearchByDateAndStatus(int pageNumer, int pageSize, DateTime fromDate, DateTime toDate, OrderStatus status)
        {
            return _orderRepository.SearchByDateAndStatus(pageNumer, pageSize,fromDate,toDate,status).Select(s => new ReadOrderReportsDto
            {
                Merchant = s.Merchant!.Name,
                orderStatus = s.orderStatus,
                FirstPhoneNumber = s.FirstPhoneNumber,
                OrderShippingTotalCost = s.OrderShippingTotalCost,
                ProductTotalCost = s.ProductTotalCost,
                Date = s.Date,
                ClientName = s.ClientName,
                Governorate = s.Governorate!.Name,
                City = s.City!.Name,
            });
        }

        public int CountOrdersByDateAndStatus(DateTime fromDate, DateTime toDate, OrderStatus status)
        {
            return _orderRepository.CountOrdersByDateAndStatus(fromDate,toDate,status);
        }
        public UpdateOrderDto GetById(int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order != null)
            {
                UpdateOrderDto result = new UpdateOrderDto()
                {
                    Id = order.Id,
                    MerchantId = order.MerchantId,
                    PaymentType = order.PaymentType,
                    Email = order.Email,
                    BranchId = order.BranchId,
                    CityId = order.CityId,
                    DeliverToVillage = order.DeliverToVillage ?? false,
                    FirstPhoneNumber = order.FirstPhoneNumber,
                    SecondPhoneNumber = order.SecondPhoneNumber,
                    GovernorateId = order.GovernorateId,
                    Notes = order.Notes,
                    ShippingTypeId = order.ShippingTypeId,
                    orderType = order.orderType,
                    Street = order.Street,
                    ClientName = order.ClientName,
                    Products = order.Products.Select(prod => new ProductDto
                    {
                        Name = prod.Name,
                        Quantity = prod.Quantity,
                        Price = prod.Price,
                        Weight = prod.Weight,
                    }).ToList()
            };
                return result;
            }

            return null;
        }

        public IEnumerable<ReadOrderDto> GetAllByStatus(OrderStatus orderStatus)
        {
            return _orderRepository.GetAllByStatus(orderStatus).Select(s => new ReadOrderDto
            {
                Date = s.Date,
                ClientName = s.ClientName,
                Governorate = s.Governorate!.Name,
                City = s.City!.Name,
                orderStatus = s.orderStatus,
                Cost = s.ProductTotalCost + s.OrderShippingTotalCost
            });
        }

        private async Task<double> CityShippingPrice(int cityId)
        {
            var result = await _cityRepository.GetByIdAsync(cityId);
            if (result != null)
            {
                return result.Price;
            }
            return 0;
        }

        private async Task<double> Cost_DeliverToVillageAsync(bool isDeliverToVillage)
        {
            var result = await _deliverToVillageRepository.GetAllAsync();

            if (isDeliverToVillage && result != null)
            {
                return result.Select(s => s.AdditionalCost).FirstOrDefault();
            }
            return 0;
        }

        private async Task<double> Cost_ShippingType(int shippingTypeId)
        {
            var result = await _shippingRepository.GetByIdAsync(shippingTypeId);
            if (result != null)
            {
                return result.Cost;
            }
            return 0;
        }

        private async Task<double> Cost_AdditionalWeight(double totalWeight)
        {
            double cost = 0;
            double defaultWeight = 0;
            double additionalPrice = 0;
            var result = await _weightRepository.GetAllAsync();
            if (result != null)
            {
                defaultWeight = result.Select(w => w.DefaultWeight).FirstOrDefault();

                if (totalWeight > defaultWeight)
                {

                    additionalPrice = result.Select(w => w.AdditionalPrice).FirstOrDefault();

                    totalWeight = totalWeight - defaultWeight;

                    cost = totalWeight * additionalPrice;
                }
            }

            return cost;
        }

        private double CountWeight(List<ProductDto> products)
        {
            double weight = 0;
            foreach (var item in products)
            {
                weight += item.Weight * item.Quantity;
            }
            return weight;
        }

        private double Cost_AllProducts(List<ProductDto> products)
        {
            double price = 0;
            foreach (var item in products)
            {
                price += item.Price * item.Quantity;
            }
            return price;
        }

        public async Task<decimal> GetSpecialPricesWithMerchantandCityId(string merchantId, int cityId)
        {
            decimal totalPrice = 0;
            var result = await _specialPriceRepository.GetAllAsync();
            if (result != null)
            {
                var specialPrices = result.Where(sp => sp.CityId == cityId & sp.MerchentId == merchantId).FirstOrDefault();
                if (specialPrices != null)
                {
                    totalPrice = specialPrices.Price;
                }
            }
            return totalPrice;

        }

        public List<int> CountOrdersForEmployeeByStatus()
        {
            var listOrderStatus = _orderRepository.CountOrdersForEmployeeByStatus();
            int[] countOrdres = new int[11];//size of enum

            var g = listOrderStatus.GroupBy(i => i);

            foreach (var grp in g)
            {
                countOrdres[grp.Key] = grp.Count();
            }
            return countOrdres.ToList();
        }

        public List<int> CountOrdersForMerchantByStatus(string merchantId)
        {
            var listOrderStatus = _orderRepository.CountOrdersForMerchantByStatus(merchantId);
            int[] countOrdres = new int[11];//size of enum

            var g = listOrderStatus.GroupBy(i => i);

            foreach (var grp in g)
            {
                countOrdres[grp.Key] = grp.Count();
            }
            return countOrdres.ToList();
        }

        public IEnumerable<ReadOrderDto> GetOrdersForEmployee(string searchText,int statusId, int pageNumer, int pageSize)
        {
            if (statusId > 10 || statusId < 0)
            {
                return null!;
            }
            return _orderRepository.GetOrdersForEmployee(searchText,statusId, pageNumer, pageSize).Select(o => new ReadOrderDto
            {
                ClientName = o.ClientName,
                Date = o.Date,
                Governorate = o.Governorate!.Name,
                City = o.City!.Name,
                Cost = o.ProductTotalCost + o.OrderShippingTotalCost

            });
        }

        public IEnumerable<ReadOrderDto> GetOrdersForMerchant(string searchText, string merchantId, int statusId, int pageNumer, int pageSize)
        {
            if (statusId > 10 || statusId < 0)
            {
                return null!;
            }
            return _orderRepository.GetOrdersForMerchant(searchText,merchantId, statusId, pageNumer, pageSize).Select(o => new ReadOrderDto
            {
                ClientName = o.ClientName,
                Date = o.Date,
                Governorate = o.Governorate!.Name,
                City = o.City!.Name,
                Cost = o.ProductTotalCost + o.OrderShippingTotalCost

            });
        }

        public int GetCountOrdersForEmployee(int statusId, string searchText)
        {
            if (statusId > 10 || statusId < 0)
            {
                return 0;
            }
            return _orderRepository.GetCountOrdersForEmployee(statusId,searchText);
        }

        public int GetCountOrdersForMerchant(string merchantId, int statusId, string searchText)
        {
            if (statusId > 10 || statusId < 0)
            {
                return 0;
            }
            return _orderRepository.GetCountOrdersForMerchant(merchantId, statusId,searchText);
        }

        
    }
}