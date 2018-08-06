using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OPCDA
{
    class Program
    {
        // event handler
        public void OnOPCRead(object sender, EventArgs e)
        {

        }


        static void Main(string[] args)
        {
  

            try
            {
                RSLinxOPCDA rsLinxOPCDA = new RSLinxOPCDA();
                rsLinxOPCDA.Connect("opcda://PLC_Emulator/RSLinx Remote OPC Server");
                Console.WriteLine("Connected: " + rsLinxOPCDA.IsConnected.ToString());

                rsLinxOPCDA.CreateGroup("PLC_NWL", 1000);

                rsLinxOPCDA.AddItem("[PLC_NWL]TAG001");

                //rsLinxOPCDA.OnRead += OnOPCRead;

                rsLinxOPCDA.ReadItemsAsync();

                Console.Read();


                rsLinxOPCDA.StopReadItemsAsync();

            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            
            /*int ptimeBias = 0, pPercentDeadBand = 0, serverGroup = 0, revisedUpdateRate= 0, rrid = 0;
            object unk = null;

            oPCServer.AddGroup("", 1, 1, 0, ptimeBias, pPercentDeadBand, 1, serverGroup, revisedUpdateRate, rrid, unk);*/
        }
    }
}
