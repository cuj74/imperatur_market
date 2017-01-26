﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Imperatur_v2.shared;
using Imperatur_v2.handler;

namespace Imperatur_v2.order
{
    public class OrderQueue : IOrderQueue
    {
        private ObservableRangeCollection<IOrder> m_oOrders;
        private List<Guid> m_oNewOrders;
        private IAccountHandlerInterface m_oAccountHandler;
        private ITradeHandlerInterface m_oTradeHandler;
        private bool TryLoadFromStorage;
        private string m_oLastErrorMessage;
        public OrderQueue(IAccountHandlerInterface AccountHandler, ITradeHandlerInterface TradeHandler)
        {
            TryLoadFromStorage = false;
            m_oLastErrorMessage = "";
            m_oOrders = new ObservableRangeCollection<IOrder>();
            m_oNewOrders = new List<Guid>();
            m_oAccountHandler = AccountHandler;
            m_oTradeHandler = TradeHandler;
            InitOrders();
        }
        
        public delegate void SaveOrderHandler (object sender, events.SaveOrderEventArg e);
        public List<IOrder> Orders
        {
            get
            {
                 return InitOrders();
            }
        }

        private List<IOrder> InitOrders()
        {
            if (TryLoadFromStorage == false)
            {
                try
                {
                    m_oOrders = LoadOrders();
                    m_oOrders.CollectionChanged -= M_oOrders_CollectionChanged;
                    m_oOrders.CollectionChanged += M_oOrders_CollectionChanged;
                }
                catch (Exception ex)
                {
                    ImperaturGlobal.GetLog().Error("Loading orders went wrong", ex);
                    m_oOrders = new ObservableRangeCollection<IOrder>();
                }
            }
            TryLoadFromStorage = true;

            return new List<IOrder>((IEnumerable<IOrder>)m_oOrders);
        }

        public bool SaveOrder(Guid Identifier)
        {
            foreach (IOrder oI in m_oOrders.Where(a => a.Identifier.Equals(Identifier)))
            {
                SaveSingleOrder(oI);
            }
            return true;
        }

        private void SaveOrdersParallell(IOrder[] OrdersToSave)
        {
            Parallel.For(0, OrdersToSave.Length - 1, new ParallelOptions { MaxDegreeOfParallelism = 30 },
              i =>
              {
                  SaveSingleOrder(OrdersToSave[i]);
              });
        }

        
        public bool SaveOrders()
        {
            if (m_oOrders.Count() > 0 && m_oNewOrders != null && m_oNewOrders.Count > 0 && m_oOrders.Where(x=>x==null).Count()==0)
            {
                var OrdersToSave = from no in m_oNewOrders
                                   join eo in m_oOrders on no equals eo.Identifier
                                   select eo;
                if (OrdersToSave != null && OrdersToSave.Count() > 0)
                {
                    SaveOrdersParallell(OrdersToSave.ToArray());
                }
            }
            SaveOrdersParallell(m_oOrders.ToArray());
            m_oNewOrders.Clear();
            return true;
        }


        private bool SaveSingleOrder(IOrder oI)
        {
            if (oI != null)
            {
                try
                {
                    json.SerializeJSONdata.SerializeObject((Order)oI,
                      string.Format(@"{0}\{1}\{2}.json", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.OrderDirectory, oI.Identifier));
                }
                catch (Exception ex)
                {
                    ImperaturGlobal.GetLog().Error("Couldn't save order", ex);
                }
            }
            return true;
        }

        private void Item_SaveOrderEvent(object sender, events.SaveOrderEventArg e)
        {
            SaveOrder(e.Identifier);
        }

        private void M_oOrders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SaveOrders();
        }

        private ObservableRangeCollection<IOrder> LoadOrders()
        {
            //TODO create new generic class for the load of different data!!
            ObservableRangeCollection<IOrder> OrdersFromFiles = new ObservableRangeCollection<IOrder>();

            string[] files = Directory.GetFiles(string.Format(@"{0}\{1}\", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.OrderDirectory), "*.json", SearchOption.TopDirectoryOnly);

            foreach (string Fa in files)
            {
                OrdersFromFiles.Add((IOrder)json.DeserializeJSON.DeserializeObjectFromFile(Fa));
            }
            return OrdersFromFiles;
        }
        private void RemoveFilesFromStorage(List<IOrder> ToRemove)
        {
            if (ToRemove.Count() == 0)
            {
                return;
            }
            DirectoryInfo di = new DirectoryInfo(string.Format(@"{0}\{1}\", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.OrderDirectory));
            var files = di.GetFiles();

            var FilesToDelete =
                from f in files
                join torem in ToRemove on f.Name equals torem.Identifier + ".json"
                select f;

            FilesToDelete.AsParallel().ForAll((f) => f.Delete()); 
           
        }

        public bool EvaluateOrdersInQueue()
        {
            if (ImperaturGlobal.ExchangeStatus != ExchangeStatus.Open)
            {
                return false;
            }
            //remove those that are not valid any more
            if (m_oOrders == null || m_oOrders.Count() == 0)
            {
                return true;
            }

            List<IOrder> ToRemove = m_oOrders.Where(x => x!= null && x.ValidToDate < DateTime.Now).ToList();
            RemoveFilesFromStorage(ToRemove);
            m_oOrders.RemoveRange(ToRemove);

            if (m_oOrders.Count() == 0)
            {
                return true;
            }


            ToRemove.Clear();
            
            bool bReturn = false;
            if (m_oOrders.Count() > 0 && m_oOrders.Where(x => x != null).Count() > 0)
            {
                List<IOrder> ToAdd = new List<IOrder>();
                foreach (IOrder oI in m_oOrders.Where(x => x != null).OrderBy(x => x.ValidToDate))
                {
                    IOrder StopLoss;
                    try
                    {
                        if (oI.ExecuteOrder(m_oAccountHandler, m_oTradeHandler, out StopLoss))
                        {
                            bReturn = true;
                            if (!oI.Equals(StopLoss))
                            {
                                ToAdd.Add(StopLoss);
                            }
                            ToRemove.Add(oI);
                        }
                    }
                    catch (Exception ex)
                    {
                        ImperaturGlobal.GetLog().Error("Exception in OrderQueue.EvaluateOrdersInQueue", ex);
                    }

                }
                RemoveFilesFromStorage(ToRemove);
                m_oOrders.RemoveRange(ToRemove);
                m_oOrders.AddRange(ToAdd);
            }
            return bReturn;
        }

        public bool AddOrder(IOrder Order)
        {
            //check that order doesn't exists, in that case remove and set new order
            List<IOrder> DuplicateOrders = m_oOrders.Where(o =>
                o.AccountIdentifier.Equals(Order.AccountIdentifier)
                &&
                o.Symbol.Equals(Order.Symbol)
                &&
                o.OrderType.Equals(Order.OrderType)
                ).ToList();
            RemoveFilesFromStorage(DuplicateOrders);
            m_oOrders.RemoveRange(DuplicateOrders);
            m_oNewOrders = new List<Guid>() { Order.Identifier };
            m_oOrders.Add(Order);
            m_oOrders.CollectionChanged -= M_oOrders_CollectionChanged;
            m_oOrders.CollectionChanged += M_oOrders_CollectionChanged;
            return true;
        }

        public bool AddOrders(List<IOrder> Orders)
        {
            m_oNewOrders = Orders.Where(x=>x != null).Select(x => x.Identifier).ToList();


            m_oOrders.AddRange(Orders);
            m_oOrders.CollectionChanged -= M_oOrders_CollectionChanged;
            m_oOrders.CollectionChanged += M_oOrders_CollectionChanged;
            return true;
        }

        public List<IOrder> GetOrdersForAccount(Guid AccountIdentifier)
        {
            return m_oOrders.Where(o => o != null && o.AccountIdentifier != null && o.AccountIdentifier.Equals(AccountIdentifier)).ToList();
        }

        public bool QueueMaintence(IAccountHandlerInterface AccountHandler)
        {

            //remove sell orders where the holding dont exists on the account
            try
            {
                
                List<OrderType> SellStoploss = new List<OrderType>();
                SellStoploss.Add(OrderType.Sell);
                SellStoploss.Add(OrderType.StopLoss);

                var OrderQuery =
                from t in m_oOrders
                join ssl in SellStoploss on t.OrderType equals ssl
                select t;


                var ObseleteOrders = from o in OrderQuery
                                     from a in AccountHandler.Accounts()
                                     .Where(a => a.Identifier.Equals(o.AccountIdentifier)
                                     &&
                                     a.GetSymbolInHoldings().Contains(o.Symbol)
                                     ).DefaultIfEmpty()
                                     where a == null
                                     select o;

                /*
                var ObseleteOrders = from a in AccountHandler.Accounts()
                                     from o in OrderQuery
                                     .Where(x => a.Identifier.Equals(x.AccountIdentifier)
                                     &&
                                     a.GetSymbolInHoldings().Contains(x.Symbol)
                                     ).DefaultIfEmpty() 
                                     select o;
                                     */
                /*
                var ObseleteOrders = from o in OrderQuery
                                     from a in AccountHandler.Accounts()
                                     .Where(a => a.Identifier.Equals(o.AccountIdentifier)
                                     &&
                                     a.GetSymbolInHoldings().Contains(o.Symbol)
                                     ).DefaultIfEmpty()
                                     select o;*/
                int gfgf = ObseleteOrders.Count();
                List<IOrder> sds = ObseleteOrders.ToList();
                //string[] sdfsd = ObseleteOrders.Select(x => x.Identifier.ToString()).ToArray();

                if (ObseleteOrders == null || ObseleteOrders.Count() == 0)
                {
                    return true;
                }
                RemoveFilesFromStorage(ObseleteOrders.Where(o=>o != null).ToList());
                m_oOrders.RemoveRange(ObseleteOrders.Where(o=>o != null).ToList());
                
            }
            catch (Exception ex)
            {
                ImperaturGlobal.GetLog().Error("Error in OrderQueue.QueueMaintence", ex);
            }

            return true;

        }
    }
}
