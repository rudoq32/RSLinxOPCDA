using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OPCDA
{
    class RSLinxOPCDA
    {
        private Opc.URL m_url;
        private Opc.Da.Server m_server = null;
        private OpcCom.Factory m_factory;
        private Opc.Da.Subscription m_groupRead;
        private Opc.Da.SubscriptionState m_groupState;
        private Opc.Da.Item[] m_items = null;
        private Thread m_workThread = null;

        // define event 
        public delegate void OnReadHandler(object sender, EventArgs e);
        public event OnReadHandler OnRead;

        public RSLinxOPCDA()
        {
            m_factory = new OpcCom.Factory();        
        }

        public bool IsConnected
        {
            get
            {
                return (m_server == null) ?  false :  m_server.IsConnected;
            }
        }

        public void Connect(string url)
        {
            try
            {
                m_url = new Opc.URL(url);
                m_server = new Opc.Da.Server(m_factory, null);
                m_server.Connect(m_url, new Opc.ConnectData(new System.Net.NetworkCredential()));
                //Console.WriteLine("IsConnected: " + m_server.IsConnected.ToString());
            }
            catch(Exception ee)
            {
                // aditional handling
                throw ee;
            }

        }

        public void CreateGroup(string groupName, int updateRate)
        {
            try
            {
                m_groupState = new Opc.Da.SubscriptionState();
                m_groupState.Name = groupName;
                m_groupState.UpdateRate = updateRate;
                m_groupState.Active = true;
                m_groupRead = (Opc.Da.Subscription)m_server.CreateSubscription(m_groupState);
                // callback when data are readed     
                // seems not supported
                m_groupRead.DataChanged += new Opc.Da.DataChangedEventHandler(group_DataChanged);
                
            }
            catch (Exception ee)
            {
                // aditional handling
                throw ee;
            }
        }

        // seems not supported
        public void group_DataChanged(object subscriptionHandle, object requestHndle, Opc.Da.ItemValueResult[] itemValues)
        {

        }
        

        public void AddItem(string itemName)
        {
            try
            {
                Opc.Da.Item item = new Opc.Da.Item();
                item.ItemName = itemName;
                if (m_items == null)
                    m_items = new Opc.Da.Item[1];
                else
                {
                    Array.Resize(ref m_items, m_items.Length + 1);
                }

                m_items[m_items.Length - 1] = item;
            }
            catch (Exception ee)
            {
                // aditional handling
                throw ee;
            }
        }

        public void ReadItemsAsync()
        {
            ThreadStart ts = delegate { ReadItems(this); };

            m_workThread = new Thread(ts);
            m_workThread.IsBackground = true;

            m_workThread.Start();
        }

        public void StopReadItemsAsync()
        {
            if (m_workThread != null)
                m_workThread.Abort();
            m_workThread = null;
        }

        protected void ReadItems(RSLinxOPCDA rsLinxOPCDA)
        {
            do
            {
                Opc.Da.ItemValueResult[] itemValues;
                itemValues = rsLinxOPCDA.m_server.Read(rsLinxOPCDA.m_items);

                foreach(Opc.Da.ItemValueResult itemValue in itemValues)
                {
                    Console.WriteLine("Time: " + itemValue.Timestamp.ToString() + " Item: " + itemValue.ItemName + ", value: " + itemValue.Value.ToString());
                }

                System.Threading.Thread.Sleep(rsLinxOPCDA.m_groupState.UpdateRate);
                
            } while (true);
        }


    }
}
