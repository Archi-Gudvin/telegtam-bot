using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{ 
    struct MessageLog
    {
        public string Time { get { return this.time; } set { this.time= value; } }

        public long Id { get { return this.id; } set { this.id = value; } }

        public string Msg { get { return this.msg; } set { this.msg = value; } }

        public string FirstName { get { return this.firstName; } set { this.firstName = value; } }

        public MessageLog(string Time, string Msg, string FirstName, long Id)
        {
            this.time = Time;
            this.msg = Msg;
            this.firstName = FirstName;
            this.id = Id;
        }

        private string time;
        private long id;
        private string msg;
        private string firstName;
    }
}
