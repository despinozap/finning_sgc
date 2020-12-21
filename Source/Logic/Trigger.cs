using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Logic
{
    public class Trigger
    {

        public static int executeDailyTrigger(string server_home_path)
        {
            if (server_home_path == null)
            {
                return -1;
            }

            string date = LogicController.getFechaUltimoTriggerEjecutado();
            if (date == null)
            {
                return -1;
            }


            DateTime dt = Convert.ToDateTime(date);
            if (dt == Convert.ToDateTime(DateTime.Now.ToShortDateString()))
            {
                return 0;
            }

            int execute_trigger = EmailSender.executeSendMailAllCentro(server_home_path);
            if (execute_trigger < 0)
            {
                return -1;
            }


            if (LogicController.setFechaUltimoTriggerEjecutado(DateTime.Now.ToShortDateString()))
            {
                return 1;
            }
            else
            {
                return -1;
            }

        }
    }
}