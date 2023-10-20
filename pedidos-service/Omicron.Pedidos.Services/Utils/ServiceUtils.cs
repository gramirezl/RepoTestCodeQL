﻿// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Enums;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// the class for utils.
    /// </summary>
    public static class ServiceUtils
    {
        /// <summary>
        /// creates the result.
        /// </summary>
        /// <param name="success">if it was successful.</param>
        /// <param name="code">the code.</param>
        /// <param name="userError">the user error.</param>
        /// <param name="responseObj">the responseobj.</param>
        /// <param name="exceptionMessage">the exception message.</param>
        /// <param name="comments">The comments.</param>
        /// <returns>the resultModel.</returns>
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage, string comments = null)
        {
            return new ResultModel
            {
                Success = success,
                Response = responseObj,
                UserError = userError,
                ExceptionMessage = exceptionMessage,
                Code = code,
                Comments = comments,
            };
        }

        /// <summary>
        /// Creates the order logs mode.
        /// </summary>
        /// <param name="user">the user.</param>
        /// <param name="saleslogs">add sales logs.</param>
        /// <returns> the list to insert.</returns>
        public static List<SalesLogs> AddSalesLog(string user, List<UserOrderModel> saleslogs)
        {
            var listToReturn = new List<SalesLogs>();

            saleslogs.ForEach(x =>
            {
                listToReturn.Add(new SalesLogs
                {
                    SalesOrderId = int.TryParse(x.Salesorderid, out int saleOrderInt) ? saleOrderInt : 0,
                    ProductionOrderId = int.TryParse(x.Productionorderid, out int productOrderInt) ? productOrderInt : 0,
                    StatusSalesOrder = string.IsNullOrEmpty(x.Productionorderid) ? x.Status : string.Empty,
                    StatusProductionOrder = !string.IsNullOrEmpty(x.Productionorderid) ? x.Status : string.Empty,
                    DataCheckin = CalculateDateNow(1),
                    UserId = user,
                    IsProductionOrder = !string.IsNullOrEmpty(x.Productionorderid),
                });
            });

            return listToReturn;
        }

        /// <summary>
        /// Gets the list of keys by a value.
        /// </summary>
        /// <param name="dictResult">the dict.</param>
        /// <param name="correctValue">the correct value.</param>
        /// <returns>the list.</returns>
        public static List<string> GetValuesByExactValue(Dictionary<string, string> dictResult, string correctValue)
        {
            var listToReturn = new List<string>();
            foreach (var k in dictResult.Keys)
            {
                if (dictResult[k].Equals(correctValue))
                {
                    listToReturn.Add(k);
                }
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the list of keys by a value.
        /// </summary>
        /// <param name="dictResult">the dict.</param>
        /// <param name="correctValue">the correct value.</param>
        /// <returns>the list.</returns>
        public static List<string> GetValuesByContainsKeyValue(Dictionary<string, string> dictResult, string correctValue)
        {
            var listToReturn = new List<string>();
            foreach (var k in dictResult.Keys)
            {
                if (k.Contains(correctValue))
                {
                    listToReturn.Add(dictResult[k]);
                }
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the list of keys by a value.
        /// </summary>
        /// <param name="dictResult">the dict.</param>
        /// <param name="correctValue">the correct value.</param>
        /// <returns>the list.</returns>
        public static List<string> GetValuesContains(Dictionary<string, string> dictResult, string correctValue)
        {
            var listToReturn = new List<string>();
            foreach (var k in dictResult.Keys)
            {
                if (dictResult[k].Contains(correctValue))
                {
                    listToReturn.Add(k);
                }
            }

            return listToReturn;
        }

        /// <summary>
        /// gets the products Id to return.
        /// </summary>
        /// <param name="listWithError">the error list.</param>
        /// <returns>the list of values.</returns>
        public static List<string> GetErrorsFromSapDiDic(List<string> listWithError)
        {
            var listToReturn = new List<string>();

            listWithError.ForEach(x =>
            {
                var order = x.Split("-");
                if (order.Length > 2)
                {
                    listToReturn.Add($"{order[1]}-{order[2]}");
                }
                else
                {
                    listToReturn.Add(order[1]);
                }
            });

            return listToReturn;
        }

        /// <summary>
        /// Groups the data for the front by status.
        /// </summary>
        /// <param name="sapOrders">the sap ordrs.</param>
        /// <param name="userOrders">the user ordes.</param>
        /// <param name="isTecnic">Is tecnic.</param>
        /// <returns>the data froupted.</returns>
        public static QfbOrderModel GroupUserOrder(List<CompleteFormulaWithDetalle> sapOrders, List<UserOrderModel> userOrders, bool isTecnic)
        {
            var result = new QfbOrderModel
            {
                Status = new List<QfbOrderDetail>(),
            };
            var enums = isTecnic ? Enum.GetValues(typeof(ServiceEnums.StatusTecnic)) : Enum.GetValues(typeof(ServiceEnums.Status));
            foreach (var status in enums)
            {
                BuildGroupUserOrderResult(sapOrders, userOrders, result, status);
            }

            return result;
        }

        /// <summary>
        /// gets the user by role.
        /// </summary>
        /// <param name="userService">the user service.</param>
        /// <param name="role">the role.</param>
        /// <param name="active">if the return data is by active or all.</param>
        /// <returns>the users.</returns>
        public static async Task<List<UserModel>> GetUsersByRole(IUsersService userService, string role, bool active)
        {
            var resultUsers = await userService.SimpleGetUsers(string.Format(ServiceConstants.GetUsersByRole, role));
            var allUsers = JsonConvert.DeserializeObject<List<UserModel>>(JsonConvert.SerializeObject(resultUsers.Response));

            if (active)
            {
                return allUsers.Where(x => x.Activo == 1 && x.Asignable == 1).ToList();
            }

            return allUsers;
        }

        /// <summary>
        /// gets the updatefaborder model from the list of orders.
        /// </summary>
        /// <param name="ordersWithDetail">the details.</param>
        /// <returns>the data.</returns>
        public static List<UpdateFabOrderModel> GetOrdersToAssign(List<OrderWithDetailModel> ordersWithDetail)
        {
            var orderToSend = new List<UpdateFabOrderModel>();

            ordersWithDetail.ForEach(order =>
            {
                order.Detalle
                .Where(d => d.Status.Equals("P"))
                .ToList()
                .ForEach(x =>
                {
                    orderToSend.Add(new UpdateFabOrderModel
                    {
                        OrderFabId = x.OrdenFabricacionId,
                        Status = ServiceConstants.StatusSapLiberado,
                    });
                });
            });

            return orderToSend;
        }

        /// <summary>
        /// Create a cancellation order fail object.
        /// </summary>
        /// <param name="cancellationModel">Model with data.</param>
        /// <param name="reason">Fail reason.</param>
        /// <returns>Formated object.</returns>
        public static object CreateCancellationFail(OrderIdModel cancellationModel, string reason)
        {
            return new
            {
                cancellationModel.OrderId,
                cancellationModel.UserId,
                reason,
            };
        }

        /// <summary>
        /// Gets a list divided in sublists.
        /// </summary>
        /// <typeparam name="Tsource">the original list.</typeparam>
        /// <param name="listToSplit">the original list to split.</param>
        /// <param name="maxCount">the max count per group.</param>
        /// <returns>the list of list.</returns>
        public static List<List<Tsource>> GetGroupsOfList<Tsource>(List<Tsource> listToSplit, int maxCount)
        {
            var listToReturn = new List<List<Tsource>>();
            var offset = 0;

            while (offset < listToSplit.Count)
            {
                var sublist = new List<Tsource>();
                sublist.AddRange(listToSplit.Skip(offset).Take(maxCount).ToList());
                listToReturn.Add(sublist);
                offset += maxCount;
            }

            return listToReturn;
        }

        /// <summary>
        /// gets the date filter for sap.
        /// </summary>
        /// <param name="filter">the dictionary.</param>
        /// <returns>the datetime.</returns>
        public static Dictionary<string, DateTime> GetDateFilter(Dictionary<string, string> filter)
        {
            if (filter.ContainsKey(ServiceConstants.FechaFin))
            {
                return GetDictDates(filter[ServiceConstants.FechaFin]);
            }

            if (filter.ContainsKey(ServiceConstants.FechaInicio))
            {
                return GetDictDates(filter[ServiceConstants.FechaInicio]);
            }

            return new Dictionary<string, DateTime>();
        }

        /// <summary>
        /// Gets the orders with their details.
        /// </summary>
        /// <param name="sapAdapter">the sapAdapter.</param>
        /// <param name="salesOrdersId">the "Pedido" id.</param>
        /// <returns>the data.</returns>
        public static async Task<List<OrderWithDetailModel>> GetOrdersWithFabOrders(ISapAdapter sapAdapter, List<int> salesOrdersId)
        {
            var sapResponse = await sapAdapter.PostSapAdapter(salesOrdersId, ServiceConstants.GetOrderWithDetail);
            return JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(sapResponse.Response));
        }

        /// <summary>
        /// Get sales order from SAP.
        /// </summary>
        /// <param name="salesOrder">Sales order in local db.</param>
        /// <param name="sapAdapter">The sap adapter.</param>
        /// <returns>Preproduction orders.</returns>
        public static async Task<List<CompleteDetailOrderModel>> GetPreProductionOrdersFromSap(UserOrderModel salesOrder, ISapAdapter sapAdapter)
        {
            var sapResults = await GetSalesOrdersFromSapBySaleId(new List<int> { int.Parse(salesOrder.Salesorderid) }, sapAdapter);
            return sapResults.PreProductionOrders;
        }

        /// <summary>
        /// check if the folder exist and created is if not.
        /// </summary>
        /// <param name="salesOrderId">Sales order id.</param>
        /// <param name="sapAdapter">The sap adapter.</param>
        /// <returns>Sales order.</returns>
        public static async Task<List<OrderWithDetailModel>> GetSalesOrdersFromSap(List<int> salesOrderId, ISapAdapter sapAdapter)
        {
            var orders = await sapAdapter.PostSapAdapter(salesOrderId, ServiceConstants.GetOrderWithDetail);
            return JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(orders.Response));
        }

        /// <summary>
        /// gets the dictionary.
        /// </summary>
        /// <param name="dateRange">the date range.</param>
        /// <returns>the data.</returns>
        public static Dictionary<string, DateTime> GetDictDates(string dateRange)
        {
            var dictToReturn = new Dictionary<string, DateTime>();
            var dates = dateRange.Split("-");

            var dateInicioArray = GetDatesAsArray(dates[0]);
            var dateFinArray = GetDatesAsArray(dates[1]);

            var dateInicio = new DateTime(dateInicioArray[2], dateInicioArray[1], dateInicioArray[0]);
            var dateFin = new DateTime(dateFinArray[2], dateFinArray[1], dateFinArray[0]);
            dictToReturn.Add(ServiceConstants.FechaInicio, dateInicio);
            dictToReturn.Add(ServiceConstants.FechaFin, dateFin);
            return dictToReturn;
        }

        /// <summary>
        /// Get sales order from SAP.
        /// </summary>
        /// <param name="salesOrderId">Sales order id.</param>
        /// <param name="sapAdapter">The sap adapter.</param>
        /// <returns>Sales order.</returns>
        public static async Task<(List<OrderWithDetailModel> SapOrder, List<CompleteDetailOrderModel> ProductionOrders, List<CompleteDetailOrderModel> PreProductionOrders)> GetSalesOrdersFromSapBySaleId(List<int> salesOrderId, ISapAdapter sapAdapter)
        {
            var orders = await sapAdapter.PostSapAdapter(salesOrderId, ServiceConstants.GetOrderWithDetail);
            var sapOrders = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(orders.Response));
            var preProductionOrders = new List<CompleteDetailOrderModel>();
            var productionOrders = new List<CompleteDetailOrderModel>();

            sapOrders.ForEach(x =>
            {
                preProductionOrders.AddRange(x.Detalle.Where(y => string.IsNullOrEmpty(y.Status)));
                productionOrders.AddRange(x.Detalle.Where(y => !string.IsNullOrEmpty(y.Status)));
            });

            return (sapOrders, productionOrders.ToList(), preProductionOrders.ToList());
        }

        /// <summary>
        /// validates if null and turns to upper.
        /// </summary>
        /// <param name="value">the value.</param>
        /// <param name="last">The long of subtring from end to last.</param>
        /// <returns>the data.</returns>
        public static string GetSubstring(this string value, int last)
        {
            return !string.IsNullOrEmpty(value) ? value.Substring(value.Length - last, last).ToUpper() : value;
        }

        /// <summary>
        /// Get Qfbs Info By QfbId.
        /// </summary>
        /// <param name="qfbIds">QfbIds.</param>
        /// <param name="userService">User service.</param>
        /// <returns>Tecnic info.</returns>
        public static async Task<List<QfbTecnicInfoDto>> GetQfbInfoById(List<string> qfbIds, IUsersService userService)
        {
            var resultUsers = await userService.PostSimpleUsers(qfbIds, ServiceConstants.GetQfbInfoById);
            return JsonConvert.DeserializeObject<List<QfbTecnicInfoDto>>(JsonConvert.SerializeObject(resultUsers.Response));
        }

        /// <summary>
        /// split the dates to int array.
        /// </summary>
        /// <param name="date">the date in string.</param>
        /// <returns>the dates.</returns>
        private static List<int> GetDatesAsArray(string date)
        {
            var dateArrayNum = new List<int>();
            var dateArray = date.Split("/");

            dateArray.ToList().ForEach(x =>
            {
                int.TryParse(x, out int result);
                dateArrayNum.Add(result);
            });

            return dateArrayNum;
        }

        private static DateTime CalculateDateNow(int tries)
        {
            if (tries == 3)
            {
                return DateTime.Now;
            }

            try
            {
                var dateLinux = (tries == 1) ?
                    TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City")) :
                    TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"));
                return dateLinux;
            }
            catch
            {
                var localTries = tries == 1 ? 2 : 3;
                return CalculateDateNow(localTries);
            }
        }

        private static void BuildGroupUserOrderResult(List<CompleteFormulaWithDetalle> sapOrders, List<UserOrderModel> userOrders, QfbOrderModel result, object status)
        {
            var statusId = (int)Enum.Parse(typeof(ServiceEnums.Status), status.ToString());
            var orders = new QfbOrderDetail
            {
                StatusName = statusId == (int)ServiceEnums.Status.Proceso ? ServiceConstants.ProcesoStatus : status.ToString(),
                StatusId = statusId,
                Orders = new List<FabOrderDetail>(),
            };

            var ordersDetail = new List<FabOrderDetail>();

            userOrders
                .Where(x => x.Status.Equals(status.ToString()))
                .ToList()
                .ForEach(o =>
                {
                    int.TryParse(o.Productionorderid, out int orderId);
                    var sapOrder = sapOrders.FirstOrDefault(s => s.ProductionOrderId == orderId);

                    if (sapOrder != null)
                    {
                        var destiny = sapOrder.DestinyAddress.Split(",");

                        var order = new FabOrderDetail
                        {
                            BaseDocument = sapOrder.BaseDocument,
                            Container = sapOrder.Container,
                            Tag = sapOrder.ProductLabel,
                            DescriptionProduct = sapOrder.ProductDescription,
                            FinishDate = sapOrder.OrderCreateDate,
                            PlannedQuantity = sapOrder.PlannedQuantity,
                            ProductionOrderId = sapOrder.ProductionOrderId,
                            StartDate = sapOrder.FabDate,
                            ItemCode = sapOrder.Code,
                            HasMissingStock = sapOrder.HasMissingStock,
                            Destiny = destiny.Count() < 3 || destiny[destiny.Count() - 3].Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                            FinishedLabel = o.FinishedLabel,
                            AreBatchesComplete = o.AreBatchesComplete == 1,
                            PatientName = sapOrder.PatientName,
                            ClientDxp = sapOrder.ClientDxp,
                            ShopTransaction = sapOrder.ShopTransaction,
                            TechnicalSign = o.StatusForTecnic == ServiceConstants.SignedStatus,
                            QfbName = o.QfbName,
                            HasTechnicalAssigned = !string.IsNullOrEmpty(o.TecnicId),
                        };

                        ordersDetail.Add(order);
                    }
                });

            orders.Orders = ordersDetail.OrderByDescending(x => x.AreBatchesComplete).ThenBy(y => y.ProductionOrderId).ToList();
            result.Status.Add(orders);
        }
    }
}
