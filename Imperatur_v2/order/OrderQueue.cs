using System;
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
        private IAccountHandlerInterface m_oAccountHandler;
        private ITradeHandlerInterface m_oTradeHandler;
        private bool TryLoadFromStorage;
        private string m_oLastErrorMessage;
        public OrderQueue(IAccountHandlerInterface AccountHandler, ITradeHandlerInterface TradeHandler)
        {
            TryLoadFromStorage = false;
            m_oLastErrorMessage = "";
            m_oOrders = new ObservableRangeCollection<IOrder>();
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
                    /*
                    foreach (IOrder item in m_oOrders)
                    {
                        item.SaveOrderEvent -= Item_SaveOrderEvent;
                        item.SaveOrderEvent += Item_SaveOrderEvent;
                    }*/
                }
                catch (Exception ex)
                {
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
        public bool SaveOrders()
        {
            foreach (IOrder oO in m_oOrders)
            {
                SaveSingleOrder(oO);
            }
            return true;
        }


        private bool SaveSingleOrder(IOrder oI)
        {
            try
            {
                json.SerializeJSONdata.SerializeObject((Order)oI,
                  string.Format(@"{0}\{1}\{2}.json", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.OrderDirectory, oI.Identifier));
            }
            catch(Exception ex)
            {
                int gg = 0;
            }
            return true;
        }

        private void Item_SaveOrderEvent(object sender, events.SaveOrderEventArg e)
        {
            SaveOrder(e.Identifier);
        }

        private void M_oOrders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            /*
            foreach (IOrder item in m_oOrders)
            {
                item.SaveOrderEvent -= Item_SaveOrderEvent;
                item.SaveOrderEvent += Item_SaveOrderEvent;
            }
            */
            SaveOrders();
        }

        /*
        private void Item_SaveOrderEvent(object sender, events e)
        {
            SaveAccount(e.Identifier);
        }
        */
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
            if (m_oOrders.Count() == 0)
            {
                return true;
            }

            List<IOrder> ToRemove = m_oOrders.Where(x => x.ValidToDate < DateTime.Now).ToList();
            m_oOrders.RemoveRange(ToRemove);
            RemoveFilesFromStorage(ToRemove);

            if (m_oOrders.Count() == 0)
            {
                return true;
            }


            ToRemove.Clear();
            List<IOrder> ToAdd = new List<IOrder>();
            bool bReturn = false;
            foreach (IOrder oI in m_oOrders)
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
                catch(Exception ex)
                {
                    int gg = 0;
                }
                  
            }
            m_oOrders.RemoveRange(ToRemove);
            RemoveFilesFromStorage(ToRemove);
            m_oOrders.AddRange(ToAdd);
            return bReturn;
        }

        public bool AddOrder(IOrder Order)
        {
            m_oOrders.Add(Order);
            m_oOrders.CollectionChanged -= M_oOrders_CollectionChanged;
            m_oOrders.CollectionChanged += M_oOrders_CollectionChanged;
            return true;
        }
    }
}
