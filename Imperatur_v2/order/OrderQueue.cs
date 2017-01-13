using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Imperatur_v2.shared;

namespace Imperatur_v2.order
{
    public class OrderQueue : IOrderQueue
    {
        private ObservableRangeCollection<IOrder> m_oOrders;
        private bool TryLoadFromStorage;
        private string m_oLastErrorMessage;
        public OrderQueue()
        {
            TryLoadFromStorage = false;
            m_oLastErrorMessage = "";
            m_oOrders = new ObservableRangeCollection<IOrder>();
        }

        public delegate void SaveOrderHandler (object sender, events.SaveOrderEventArg e);
        public List<IOrder> Orders
        {

            get
            {
                if (TryLoadFromStorage == false)
                {
                    try
                    {
                        m_oOrders = LoadOrders();
                        m_oOrders.CollectionChanged -= M_oOrders_CollectionChanged;
                        m_oOrders.CollectionChanged += M_oOrders_CollectionChanged;

                        foreach (IOrder item in m_oOrders)
                        {
                            item.SaveOrderEvent -= Item_SaveOrderEvent;
                            item.SaveOrderEvent += Item_SaveOrderEvent;
                        }
                    }
                    catch (Exception ex)
                    {
                        m_oOrders = new ObservableRangeCollection<IOrder>();

                    }
                }
                TryLoadFromStorage = true;

                return new List<IOrder>((IEnumerable<IOrder>)m_oOrders);
            }
            
        }

        public bool SaveOrder(Guid Identifier)
        {
            foreach (IOrder oI in m_oOrders.Where(a => a.Identifier.Equals(Identifier)))
            {
                SaveSingleOrder(oI);
            }
            return true;
        }
        private bool SaveSingleOrder(IOrder oI)
        {
            json.SerializeJSONdata.SerializeObject((Order)oI,
              string.Format(@"{0}\{1}\{2}.json", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.AcccountDirectory, oI.Identifier));
            return true;
        }

        private void Item_SaveOrderEvent(object sender, events.SaveOrderEventArg e)
        {
            SaveOrder(e.Identifier);
        }

        private void M_oOrders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
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

        public bool EvaluateOrdersInQueue()
        {
            //remove those that are not valid any more
            m_oOrders.RemoveRange(
                m_oOrders.Where(x=>x.
                )
            if (m_oOrders.Count() == 0)
            {
                return true;
            }
            foreach (IOrder oI in m_oOrders.where)
            // m_oOrders.Select(o=>o.ExecuteOrder())
        }
    }
}
