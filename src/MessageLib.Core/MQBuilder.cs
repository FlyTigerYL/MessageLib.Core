using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace MessageLib.Core
{
    public class MQBusBuilder
    {
        public static string Connnection = "";
        public static IBus CreateMessageBus()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Connnection) || Connnection.ToLower().Contains("stop"))
                    return null;
                IBus bus = RabbitHutch.CreateBus(Connnection);
                return bus;
            }
            catch (EasyNetQException ex)
            {
                throw ex;
            }

        }
    }
}
